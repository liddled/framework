using System;
using System.Text;

namespace DL.Framework.OFX
{
    public class OFXTransaction
    {
        public OFXTransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string TransactionID { get; set; }
        public string Name { get; set; }
        public DateTime TransactionInitializationDate { get; set; }
        public DateTime FundAvaliabilityDate { get; set; }
        public string Memo { get; set; }
        public string IncorrectTransactionID { get; set; }
        public OFXTransactionCorrectionType TransactionCorrectionAction { get; set; }
        public string ServerTransactionID { get; set; }
        public string CheckNum { get; set; }
        public string ReferenceNumber { get; set; }
        public string Sic { get; set; }
        public string PayeeID { get; set; }
        public OFXAccount TransactionSenderAccount { get; set; }
        public string Currency { get; set; }

        public string Description
        {
            get
            {
                var description = new StringBuilder(Name);
                if (!String.IsNullOrWhiteSpace(Memo))
                {
                    if (!String.IsNullOrWhiteSpace(Name))
                        description.Append(" - ");
                    description.Append(Memo);
                }
                return description.ToString();
            }
        }

        /// <summary>
        /// Returns TransactionType from string version
        /// </summary>
        /// <param name="transactionType">string version of transaction type</param>
        /// <returns>Enum version of given transaction type string</returns>
        public static OFXTransactionType GetTransactionType(string transactionType)
        {
            return (OFXTransactionType)Enum.Parse(typeof(OFXTransactionType), transactionType);
        }

        /// <summary>
        /// Returns TransactionCorrectionType from string version
        /// </summary>
        /// <param name="transactionCorrectionType">string version of Transaction Correction Type</param>
        /// <returns>Enum version of given TransactionCorrectionType string</returns>
        public static OFXTransactionCorrectionType GetTransactionCorrectionType(string transactionCorrectionType)
        {
            return (OFXTransactionCorrectionType)Enum.Parse(typeof(OFXTransactionCorrectionType), transactionCorrectionType);
        }
    }
}