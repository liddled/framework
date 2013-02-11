namespace DL.Framework.OFX
{
    public class OFXAccount
    {
        public string AccountID { get; set; }
        public string AccountKey { get; set; }
        public OFXAccountType AccountType { get; set; }
    }

    public class OFXBankAccount : OFXAccount
    {
        public string BankID { get; set; }
        public string BranchID { get; set; }
        public OFXBankAccountType BankAccountType { get; set; }
    }
}