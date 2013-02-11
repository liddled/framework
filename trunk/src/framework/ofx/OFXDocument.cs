using System;
using System.Collections.Generic;

namespace DL.Framework.OFX
{
    public class OFXDocument
    {
        public DateTime StatementStart { get; set; }
        public DateTime StatementEnd { get; set; }
        public OFXAccountType AccountType { get; set; }
        public string Currency { get; set; }
        public OFXSignOn SignOn { get; set; }
        public OFXAccount Account { get; set; }
        public OFXBalance Balance { get; set; }
        public IList<OFXTransaction> Transactions { get; set; }
    }
}