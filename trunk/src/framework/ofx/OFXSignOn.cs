using System;

namespace DL.Framework.OFX
{
    public class OFXSignOn
    {
        public string StatusSeverity { get; set; }
        public Int64 DTServer { get; set; }
        public int StatusCode { get; set; }
        public string Language { get; set; }
        public string IntuBid { get; set; }
    }
}