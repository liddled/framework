using System;

namespace DL.Framework.OFX
{
    public static class OFXConstants
    {
        public const string BankAccount = "OFX/BANKMSGSRSV1/STMTTRNRS/STMTRS";
        public const string CCAccount = "OFX/CREDITCARDMSGSRSV1/CCSTMTTRNRS/CCSTMTRS";
        public const string InsufficientFunds = "There are insufficent funds to pay your {item}. We have allocated what you have evenly between the {item} but there is no room in the budget for anything else, sorry...";
        public const string NoFunds = "There are no funds for your {item} sorry...";
        public const string NoMoney = "You are spending beyond your means so we have had to use your savings to create this budget. Perhaps you need to review your spending...";
        public const string SignOn = "OFX/SIGNONMSGSRSV1/SONRS";
    }
}