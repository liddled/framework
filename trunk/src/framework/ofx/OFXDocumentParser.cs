using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Sgml;

namespace DL.Framework.OFX
{
    public class OFXDocumentParser
    {
        private const string StripBraces = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";

        /// <summary>
        /// Converts file to an OFXDocument.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public OFXDocument Import(FileStream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return Import(reader.ReadToEnd());
            }
        }

        /// <summary>
        /// Converts string to an OFXDocument.
        /// </summary>
        /// <param name="ofx"></param>
        /// <returns></returns>
        public OFXDocument Import(string ofx)
        {
            return Parse(ofx);
        }

        /// <summary>
        /// Parses the string into an OFXDocument.
        /// </summary>
        /// <param name="ofxString"></param>
        /// <returns></returns>
        private OFXDocument Parse(string ofxString)
        {
            // convert to xml if required
            if (!IsXmlVersion(ofxString))
            {
                ofxString = SgmlToXml(ofxString);
            }

            var doc = new XmlDocument();
            doc.Load(new StringReader(ofxString));

            DateTime statementStart, statementEnd;

            var accountType = GetAccountType(ofxString);
            var currency = ParseCurrencyNode(doc, accountType);
            var signOn = ParseSignOnNode(doc);
            var account = ParseAccountNode(doc, accountType);
            var balance = ParseBalanceNode(doc, accountType);
            var transactions = ParseTransationsNode(doc, accountType, currency, out statementStart, out statementEnd);

            return new OFXDocument
            {
                AccountType = accountType,
                Currency = currency,
                SignOn = signOn,
                Account = account,
                Balance = balance,
                StatementStart = statementStart,
                StatementEnd = statementEnd,
                Transactions = transactions
            };
        }

        /// <summary>
        /// Check if OFX file is in SGML or XML format
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static bool IsXmlVersion(string file)
        {
            return (file.IndexOf("OFXHEADER:100") == -1);
        }

        /// <summary>
        /// Converts SGML to XML.
        /// </summary>
        /// <param name="file">OFX File (SGML Format)</param>
        /// <returns>OFX File in XML format</returns>
        private string SgmlToXml(string file)
        {
            var reader = new SgmlReader
            {
                DocType = "OFX",
                InputStream = new StringReader(ParseHeader(file)),
                WhitespaceHandling = WhitespaceHandling.None
            };

            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    AutoCloseElementsInternal(reader, writer);
                }
                return sw.ToString();
            }
        }

        /// <summary>
        /// Closes xml nodes correctly due to OFX not always valid XML.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        protected internal static void AutoCloseElementsInternal(SgmlReader reader, XmlWriter writer)
        {
            object msgBody = reader.NameTable.Add("MSGBODY");

            object previousElement = null;
            var elementsWeAlreadyEnded = new Stack();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        previousElement = reader.LocalName;
                        writer.WriteStartElement(reader.LocalName);
                        break;
                    case XmlNodeType.Text:
                        if (String.IsNullOrEmpty(reader.Value) == false)
                        {
                            writer.WriteString(reader.Value.Trim());
                            if (previousElement != null && !previousElement.Equals(msgBody))
                            {
                                writer.WriteEndElement();
                                elementsWeAlreadyEnded.Push(previousElement);
                            }
                        }
                        else
                            Debug.Assert(true, "big problems?");
                        break;
                    case XmlNodeType.EndElement:
                        if (elementsWeAlreadyEnded.Count > 0 && ReferenceEquals(elementsWeAlreadyEnded.Peek(), reader.LocalName))
                        {
                            elementsWeAlreadyEnded.Pop();
                        }
                        else
                        {
                            writer.WriteEndElement();
                        }
                        break;
                    default:
                        writer.WriteNode(reader, false);
                        break;
                }
            }
        }

        /// <summary>
        /// Checks that the file is supported by checking the header. Removes the header.
        /// </summary>
        /// <param name="file">OFX file</param>
        /// <returns>File, without the header</returns>
        private static string ParseHeader(string file)
        {
            //Select header of file and split into array
            //End of header worked out by finding first instance of '<'
            //Array split based of new line & carrige return
            var header = file.Substring(0, file.IndexOf('<'))
               .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //Check that no errors in header
            CheckHeader(header);

            //Remove header
            return file.Substring(file.IndexOf('<') - 1);
        }

        /// <summary>
        /// Checks that all the elements in the header are supported
        /// </summary>
        /// <param name="header">Header of OFX file in array</param>
        private static void CheckHeader(string[] header)
        {
            if (header[0] != "OFXHEADER:100")
                throw new OFXParseException("Incorrect header format");

            if (header[1] != "DATA:OFXSGML")
                throw new OFXParseException("Data type unsupported: " + header[1] + ". OFXSGML required");

            if (header[2] != "VERSION:102")
                throw new OFXParseException("OFX version unsupported. " + header[2]);

            if (header[3] != "SECURITY:NONE")
                throw new OFXParseException("OFX security unsupported");

            if (header[4] != "ENCODING:USASCII")
                throw new OFXParseException("ASCII Format unsupported:" + header[4]);

            if (header[5] != "CHARSET:1252")
                throw new OFXParseException("Charecter set unsupported:" + header[5]);

            if (header[6] != "COMPRESSION:NONE")
                throw new OFXParseException("Compression unsupported");

            if (header[7] != "OLDFILEUID:NONE")
                throw new OFXParseException("OLDFILEUID incorrect");
        }

        /// <summary>
        /// Checks account type of supplied file
        /// </summary>
        /// <param name="file">OFX file want to check</param>
        /// <returns>Account type for account supplied in ofx file</returns>
        private static OFXAccountType GetAccountType(string file)
        {
            if (file.IndexOf("<CREDITCARDMSGSRSV1>") != -1)
                return OFXAccountType.CC;

            if (file.IndexOf("<BANKMSGSRSV1>") != -1)
                return OFXAccountType.BANK;

            throw new OFXException("Unsupported Account Type");
        }

        /// <summary>
        /// Returns the currency value from the xml document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        private static string ParseCurrencyNode(XmlDocument document, OFXAccountType accountType)
        {
            var currencyNode = document.SelectSingleNode(GetXPath(accountType, OFXSection.CURRENCY));

            if (currencyNode == null)
                throw new NullReferenceException("Could not find currency node");

            return currencyNode.FirstChild.Value;
        }

        /// <summary>
        /// Returns the OFX SignOn from the xml document.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private static OFXSignOn ParseSignOnNode(XmlDocument document)
        {
            var node = document.SelectSingleNode(OFXConstants.SignOn);

            if (node == null)
                throw new OFXParseException("Sign On information not found");

            var ofxSignOn = new OFXSignOn
            {
                StatusCode = Convert.ToInt32(node.GetValue("STATUS/CODE")),
                StatusSeverity = node.GetValue("STATUS/SEVERITY"),
                Language = node.GetValue("LANGUAGE"),
                IntuBid = node.GetValue("INTU.BID")
            };

            var dtServer = Regex.Replace(node.GetValue("DTSERVER"), StripBraces, "");
            ofxSignOn.DTServer = Convert.ToInt64(dtServer);

            return ofxSignOn;
        }

        /// <summary>
        /// Returns the OFX Account from the xml document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        private static OFXAccount ParseAccountNode(XmlDocument document, OFXAccountType accountType)
        {
            var node = document.SelectSingleNode(GetXPath(accountType, OFXSection.ACCOUNTINFO));
            
            if (node == null)
                throw new OFXParseException("Account information not found");

            return OFXHelper.CreateAccount(node, accountType);
        }

        /// <summary>
        /// Returns the OFX Sender Account from the xml node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static OFXAccount ParseSenderAccountNode(XmlNode node)
        {
            var senderBankAccountNode = node.SelectSingleNode("BANKACCTTO");
            if (senderBankAccountNode != null)
            {
                return OFXHelper.CreateAccount(node, OFXAccountType.BANK);
            }

            var senderCreditCardAccountNode = node.SelectSingleNode("CCACCTTO");
            if (senderCreditCardAccountNode != null)
            {
                return OFXHelper.CreateAccount(node, OFXAccountType.CC);
            }

            return null;
        }

        /// <summary>
        /// Returns the OFX Balance from the xml document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        private static OFXBalance ParseBalanceNode(XmlDocument document, OFXAccountType accountType)
        {
            var ledgerNode = document.SelectSingleNode(GetXPath(accountType, OFXSection.BALANCE) + "/LEDGERBAL");
            var avaliableNode = document.SelectSingleNode(GetXPath(accountType, OFXSection.BALANCE) + "/AVAILBAL");

            // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a 'null' situation
            if (ledgerNode == null) // && avaliableNode != null
                throw new OFXParseException("Balance information not found");

            var ofxBalance = new OFXBalance();

            var tempLedgerBalance = ledgerNode.GetValue("BALAMT");

            if (String.IsNullOrEmpty(tempLedgerBalance))
                throw new OFXParseException("Ledger balance has not been set");

            // ***** Forced Invariant Culture. 
            // If you don't force it, it will use the computer's default (defined in windows control panel, regional settings)
            // So, if the number format of the computer in use it's different from OFX standard (i suppose the english/invariant), 
            // the next line of could crash or (worse) the number would be wrongly interpreted. 
            // For example, my computer has a brazilian regional setting, with "." as thousand separator and "," as 
            // decimal separator, so the value "10.99" (ten 'dollars' (or whatever currency) and ninetynine cents) would be interpreted as "1099" 
            // (one thousand and ninetynine dollars - the "." would be ignored)
            ofxBalance.LedgerBalance = Convert.ToDouble(tempLedgerBalance, CultureInfo.InvariantCulture);

            // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a null situation
            if (avaliableNode != null)
            {
                var tempAvaliableBalance = avaliableNode.GetValue("BALAMT");

                if (String.IsNullOrEmpty(tempAvaliableBalance))
                    throw new OFXParseException("Avaliable balance has not been set");

                // ***** Forced Invariant Culture. (same commment as above)
                ofxBalance.AvaliableBalance = Convert.ToDouble(tempAvaliableBalance, CultureInfo.InvariantCulture);
                ofxBalance.AvaliableBalanceDate = avaliableNode.GetValue("DTASOF").ToDate();
            }

            ofxBalance.LedgerBalanceDate = ledgerNode.GetValue("DTASOF").ToDate();

            return ofxBalance;
        }

        /// <summary>
        /// Returns list of OFXTransaction from the xml document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="accountType"></param>
        /// <param name="currency"></param>
        /// <param name="statementStart"></param>
        /// <param name="statementEnd"></param>
        /// <returns></returns>
        private static IList<OFXTransaction> ParseTransationsNode(XmlDocument document, OFXAccountType accountType, string currency, out DateTime statementStart, out DateTime statementEnd)
        {
            var xpath = GetXPath(accountType, OFXSection.TRANSACTIONS);

            statementStart = document.GetValue(xpath + "/DTSTART").ToDate();
            statementEnd = document.GetValue(xpath + "/DTEND").ToDate();

            var transactionNodes = document.SelectNodes(xpath + "/STMTTRN");

            var transactions = new List<OFXTransaction>();

            if (transactionNodes != null)
            {
                foreach (XmlNode node in transactionNodes)
                {
                    var ofxTransaction = ParseTransactionNode(node, currency);
                    transactions.Add(ofxTransaction);
                }
            }

            return transactions;
        }

        /// <summary>
        /// Returns an OFXTransaction from the xml node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private static OFXTransaction ParseTransactionNode(XmlNode node, string currency)
        {
            var ofxTransaction = new OFXTransaction
            {
                TransactionType = OFXTransaction.GetTransactionType(node.GetValue("TRNTYPE")),
                Date = node.GetValue("DTPOSTED").ToDate(),
                TransactionInitializationDate = node.GetValue("DTUSER").ToDate(),
                FundAvaliabilityDate = node.GetValue("DTAVAIL").ToDate(),
                IncorrectTransactionID = node.GetValue("CORRECTFITID"),
                ServerTransactionID = node.GetValue("SRVRTID"),
                CheckNum = node.GetValue("CHECKNUM"),
                ReferenceNumber = node.GetValue("REFNUM"),
                Sic = node.GetValue("SIC"),
                PayeeID = node.GetValue("PAYEEID"),
                Name = WebUtility.HtmlDecode(node.GetValue("NAME") ?? String.Empty),
                Memo = WebUtility.HtmlDecode(node.GetValue("MEMO") ?? String.Empty)
            };

            try
            {
                ofxTransaction.Amount = Convert.ToDecimal(node.GetValue("TRNAMT"), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new OFXParseException("Transaction Amount unknown", ex);
            }

            try
            {
                ofxTransaction.TransactionID = node.GetValue("FITID");
            }
            catch (Exception ex)
            {
                throw new OFXParseException("Transaction ID unknown", ex);
            }

            var tempCorrectionAction = node.GetValue("CORRECTACTION");
            ofxTransaction.TransactionCorrectionAction = !String.IsNullOrEmpty(tempCorrectionAction)
                                             ? OFXTransaction.GetTransactionCorrectionType(tempCorrectionAction)
                                             : OFXTransactionCorrectionType.NA;

            var currencyNode = node.SelectSingleNode("CURRENCY");
            if (currencyNode != null)
            {
                ofxTransaction.Currency = node.GetValue("CURRENCY");
            }
            else
            {
                var originalCurrencyNode = node.SelectSingleNode("ORIGCURRENCY");
                ofxTransaction.Currency = (originalCurrencyNode != null)  ? node.GetValue("ORIGCURRENCY") : currency;
            }

            ofxTransaction.TransactionSenderAccount = ParseSenderAccountNode(node);

            return ofxTransaction;
        }

        /// <summary>
        /// Returns the correct xpath to specified section for given account type.
        /// </summary>
        /// <param name="type">Account type</param>
        /// <param name="section">Section of OFX document, e.g. Transaction Section</param>
        /// <exception cref="OFXException">Thrown in account type not supported</exception>
        private static string GetXPath(OFXAccountType type, OFXSection section)
        {
            string xpath, accountInfo;

            switch (type)
            {
                case OFXAccountType.BANK:
                    xpath = OFXConstants.BankAccount;
                    accountInfo = "/BANKACCTFROM";
                    break;
                case OFXAccountType.CC:
                    xpath = OFXConstants.CCAccount;
                    accountInfo = "/CCACCTFROM";
                    break;
                default:
                    throw new OFXException("Account Type not supported. Account type " + type);
            }

            switch (section)
            {
                case OFXSection.ACCOUNTINFO:
                    return xpath + accountInfo;
                case OFXSection.BALANCE:
                    return xpath;
                case OFXSection.TRANSACTIONS:
                    return xpath + "/BANKTRANLIST";
                case OFXSection.SIGNON:
                    return OFXConstants.SignOn;
                case OFXSection.CURRENCY:
                    return xpath + "/CURDEF";
                default:
                    throw new OFXException("Unknown section found when retrieving XPath. Section " + section);
            }
        }
    }
}