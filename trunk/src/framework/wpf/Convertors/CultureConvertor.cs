using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace DL.Framework.WPF
{
    [ValueConversion(typeof(string), typeof(XmlLanguage))]
    public class CultureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tag = value as string;
            if (tag == null) return XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            return XmlLanguage.GetLanguage(tag);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XmlLanguage lang = value as XmlLanguage;
            if (lang == null) return XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag).IetfLanguageTag;
            return lang.IetfLanguageTag;
        }
    }
}
