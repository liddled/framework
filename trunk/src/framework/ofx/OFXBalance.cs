using System;

namespace DL.Framework.OFX
{
    public class OFXBalance
    {
        public double LedgerBalance { get; set; }
        public DateTime LedgerBalanceDate { get; set; }
        public double AvaliableBalance { get; set; }
        public DateTime? AvaliableBalanceDate { get; set; }
    }
}