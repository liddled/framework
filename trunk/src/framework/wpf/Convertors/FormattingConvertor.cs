using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace DL.Framework.WPF
{
    public class FormattingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (parameter != null)
            {
                string strFormatString = parameter.ToString();

                if (String.IsNullOrEmpty(strFormatString))
                    return String.Format(strFormatString, value);
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TypeConverter objTypeConverter = TypeDescriptor.GetConverter(targetType);
            object objReturnValue = null;

            if (objTypeConverter.CanConvertFrom(value.GetType()))
            {
                try
                {
                    objReturnValue = objTypeConverter.ConvertFrom(value);
                }
                catch
                {
                    objReturnValue = value;
                }
            }

            return objReturnValue;
        }
    }
}
