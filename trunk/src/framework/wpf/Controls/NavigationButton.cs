using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DL.Framework.WPF
{
    public class NavigationButton : Button
    {
        public static readonly DependencyProperty ActiveRecordCountProperty = DependencyProperty.Register("ActiveRecordCount", typeof(int), typeof(NavigationButton));
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(NavigationButton));

        public int ActiveRecordCount
        {
            get { return (int)GetValue(ActiveRecordCountProperty); }
            set { SetValue(ActiveRecordCountProperty, value); }
        }

        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }
}
