using System;
using System.Reflection;

namespace DL.Framework.Common
{
    public class ExcelNode : IExcelNode
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string MethodName { get; set; }
        public object ClassObject { get; set; }

        public object GetInstance(string header = null)
        {
            if (!String.IsNullOrWhiteSpace(header))
            {
                var mi = ClassObject.GetType().GetMethod("ResetEndPointSuffix");
                //if (mi != null && !UtilityHelper.IsQpCommonService(ClassObject.GetType()))
                    //mi.Invoke(ClassObject, new Object[] { header });
            }

            var instanceProperty = ClassObject.GetType().GetProperty("Instance");
            if (instanceProperty == null)
                throw new Exception(String.Format("Type {0} does not have an Instance property", ClassObject.GetType()));
            
            var instance = instanceProperty.GetValue(ClassObject, null);
            if (instance == null)
                throw new Exception(String.Format("Type {0} Instance property returning null", ClassObject.GetType()));

            return instance;
        }

        public MethodInfo GetMethod()
        {
            var mi = GetInstance().GetType().GetMethod(MethodName);
            return mi;
        }
    }
}
