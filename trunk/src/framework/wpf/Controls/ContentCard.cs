using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DL.Framework.WPF
{
    public class ContentCard : HeaderedContentControl
    {
        static ContentCard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentCard), new FrameworkPropertyMetadata(typeof(ContentCard)));
        }

        public double DogEar
        {
            get { return (double)GetValue(DogEarProperty); }
            set { SetValue(DogEarProperty, value); }
        }

        public static readonly DependencyProperty DogEarProperty =
            DependencyProperty.Register("DogEar",
            typeof(double),
            typeof(ContentCard),
            new UIPropertyMetadata(28.0, DogEarPropertyChanged));

        private static void DogEarPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            ((ContentCard)obj).InvalidateVisual();
        }

        public ContentCard()
        {
            SizeChanged += ContentCardSizeChanged;
        }

        private void ContentCardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var clip = new PathGeometry
            {
                Figures = new PathFigureCollection
                (
                    new []
                    {
                        new PathFigure
                        (
                            new Point(0, 0),
                            new[]
                            {
                                new LineSegment(new Point(ActualWidth - DogEar, 0), true),
                                new LineSegment(new Point(ActualWidth, DogEar), true), 
                                new LineSegment(new Point(ActualWidth, ActualHeight), true),
                                new LineSegment(new Point(0, ActualHeight), true)
                            },
                            true
                        )
                    }
                )
            };

            Clip = clip;
        }
    }
}
