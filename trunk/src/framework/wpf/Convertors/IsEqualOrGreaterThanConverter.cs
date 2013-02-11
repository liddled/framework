using System;
using System.Globalization;
using System.Windows.Data;

namespace DL.Framework.WPF
{
    public class IsEqualOrGreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double v1;
            double v2;

            if (String.IsNullOrEmpty(value.ToString()) || !Double.TryParse(value.ToString(), out v1) || String.IsNullOrEmpty(parameter.ToString()) || !Double.TryParse(parameter.ToString(), out v2))
                return Binding.DoNothing;

            return v1 >= v2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
