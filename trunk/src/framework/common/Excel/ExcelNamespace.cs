using System.Collections.Generic;

namespace DL.Framework.Common
{
    public class ExcelNamespace : IExcelNode
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<IExcelNode> Children { get; set; }
    }
}
