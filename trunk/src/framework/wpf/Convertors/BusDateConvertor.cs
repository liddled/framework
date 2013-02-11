using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using DL.Framework.Common;

namespace DL.Framework.WPF
{
    public class BusDateConverter : IValueConverter
    {
        public static T GetPropValue<T>(object src, string propName) where T : class
        {
            var value = src.GetType().GetProperty(propName).GetValue(src, null);
            if (value == null) return default(T);
            return (T)value;
        }

        public static T GetEnumValue<T>(object src, string enumName)
        {
            var propertyInfo = src.GetType().GetProperty(enumName);
            if (propertyInfo == null || !propertyInfo.PropertyType.IsEnum) return default(T);
            var value = propertyInfo.GetValue(src, null);
            if (value == null) return default(T);
            return (T)value;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (parameter == null)
                return null;
            
            var strFormatString = parameter.ToString().Split('|');
            var v1 = strFormatString[0];
            var v2 = strFormatString[1];

            var busDate = GetPropValue<BusDate>(value, v1);
            var dateView = GetEnumValue<DateView>(value, v2);

            return busDate.ToString(dateView);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
