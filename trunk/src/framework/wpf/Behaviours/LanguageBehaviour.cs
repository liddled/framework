using System.Windows;
using System.Windows.Markup;

namespace DL.Framework.WPF
{
    public class LanguageBehaviour
    {
        public static DependencyProperty LanguageProperty = DependencyProperty.RegisterAttached("Language", typeof(string), typeof(LanguageBehaviour), new UIPropertyMetadata(LanguageBehaviour.OnLanguageChanged));

        public static void SetLanguage(FrameworkElement target, string value)
        {
            target.SetValue(LanguageBehaviour.LanguageProperty, value);
        }

        public static string GetLanguage(FrameworkElement target)
        {
            return (string)target.GetValue(LanguageBehaviour.LanguageProperty);
        }

        private static void OnLanguageChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = target as FrameworkElement;
            element.Language = XmlLanguage.GetLanguage(e.NewValue.ToString());
        }
    }
}
