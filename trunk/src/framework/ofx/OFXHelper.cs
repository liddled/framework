using System;
using System.Xml;

namespace DL.Framework.OFX
{
    public static class OFXHelper
    {
        /// <summary>
        /// Converts string representation of AccountInfo to enum AccountInfo
        /// </summary>
        /// <param name="bankAccountType">representation of AccountInfo</param>
        /// <returns>AccountInfo</returns>
        public static OFXBankAccountType GetBankAccountType(this string bankAccountType)
        {
            return (OFXBankAccountType)Enum.Parse(typeof(OFXBankAccountType), bankAccountType);
        }

        /// <summary>
        /// Creates an account from an ofx document xml node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        public static OFXAccount CreateAccount(XmlNode node, OFXAccountType accountType)
        {
            OFXAccount account;

            switch (accountType)
            {
                case OFXAccountType.BANK:
                    account = CreateBankAccount(node, accountType);
                    break;
                default:
                    throw new ApplicationException(String.Format("Account type not supported: {0}", accountType));
            }

            return account;
        }

        /// <summary>
        /// Creates a bank account from an ofx document xml node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="accountType"></param>
        /// <returns></returns>
        private static OFXBankAccount CreateBankAccount(XmlNode node, OFXAccountType accountType)
        {
            var account = new OFXBankAccount
            {
                AccountType = accountType,
                AccountID = node.GetValue("ACCTID"),
                AccountKey = node.GetValue("ACCTKEY"),
                BankID = node.GetValue("BANKID"),
                BranchID = node.GetValue("BRANCHID")
            };

            //Get Bank Account Type from XML
            var bankAccountType = node.GetValue("ACCTTYPE");

            //Check that it has been set
            if (String.IsNullOrEmpty(bankAccountType))
                throw new OFXParseException("Bank Account type unknown");

            //Set bank account enum
            account.BankAccountType = bankAccountType.GetBankAccountType();

            return account;
        }

        /// <summary>
        /// Flips date from YYYYMMDD to DDMMYYYY         
        /// </summary>
        /// <param name="date">Date in YYYYMMDD format</param>
        /// <returns>Date in format DDMMYYYY</returns>
        public static DateTime ToDate(this string date)
        {
            try
            {
                if (date.Length < 8)
                {
                    return new DateTime();
                }

                var dd = Int32.Parse(date.Substring(6, 2));
                var mm = Int32.Parse(date.Substring(4, 2));
                var yyyy = Int32.Parse(date.Substring(0, 4));

                return new DateTime(yyyy, mm, dd);
            }
            catch
            {
                throw new OFXParseException("Unable to parse date");
            }
        }

        /// <summary>
        /// Returns value of specified node
        /// </summary>
        /// <param name="node">Node to look for specified node</param>
        /// <param name="xpath">XPath for node you want</param>
        /// <returns></returns>
        public static string GetValue(this XmlNode node, string xpath)
        {
            var tempNode = node.SelectSingleNode(xpath);
            return tempNode != null ? tempNode.FirstChild.Value : "";
        }
    }
}