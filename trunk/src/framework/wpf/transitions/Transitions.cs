using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DL.Framework.WPF
{
    public delegate void TransitionCallBackDelegate(FrameworkElement currentView, FrameworkElement newView);

    public static class Transitions
    {
        private const double AnimationDuration = 400;

        public static void NoTransition(FrameworkElement currentView, FrameworkElement newView)
        {
            if (currentView != null)
                currentView.Visibility = Visibility.Collapsed;
        }

        public static void DoublePaneRightToLeftTransition(FrameworkElement container, FrameworkElement currentView, FrameworkElement newView)
        {
            if (currentView == null)
            {
                NoTransition(currentView, newView);
                return;
            }

            var newViewName = "translateNewView";
            var currentViewName = "translateCurrentView";

            var sb = new Storyboard();
            NameScope.SetNameScope(container, new NameScope());

            var translateCurrentView = currentView.RenderTransform as TranslateTransform;
            if (translateCurrentView == null)
            {
                translateCurrentView = new TranslateTransform();
                currentView.RenderTransformOrigin = new Point(0.5, 0.5);
                currentView.RenderTransform = translateCurrentView;
            }

            container.RegisterName(currentViewName, translateCurrentView);

            var doubleAnimationNewView = new DoubleAnimationUsingKeyFrames {BeginTime = TimeSpan.FromSeconds(0)};
            Storyboard.SetTargetName(doubleAnimationNewView, newViewName);
            Storyboard.SetTargetProperty(doubleAnimationNewView, new PropertyPath("X"));
            doubleAnimationNewView.KeyFrames.Add(new SplineDoubleKeyFrame(newView.ActualWidth + 10, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            doubleAnimationNewView.KeyFrames.Add(new SplineDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration))));
            sb.Children.Add(doubleAnimationNewView);

            var translateNewView = newView.RenderTransform as TranslateTransform;
            if (translateNewView == null)
            {
                translateNewView = new TranslateTransform();
                newView.RenderTransformOrigin = new Point(0.5, 0.5);
                newView.RenderTransform = translateNewView;
            }

            container.RegisterName(newViewName, translateNewView);

            var doubleAnimationCurrentView = new DoubleAnimationUsingKeyFrames
                                                 {BeginTime = TimeSpan.FromSeconds(0), FillBehavior = FillBehavior.Stop};
            Storyboard.SetTargetName(doubleAnimationCurrentView, currentViewName);
            Storyboard.SetTargetProperty(doubleAnimationCurrentView, new PropertyPath("X"));
            doubleAnimationCurrentView.KeyFrames.Add(new SplineDoubleKeyFrame((newView.ActualWidth + 10) * -1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(AnimationDuration))));
            sb.Children.Add(doubleAnimationCurrentView);

            sb.Begin(container);
        }

        public static void SinglePaneRightToLeftTransition(FrameworkElement container, FrameworkElement currentView, FrameworkElement newView)
        {
            var translateNewView = newView.RenderTransform as TranslateTransform;
            if (translateNewView == null)
            {
                translateNewView = new TranslateTransform();
                container.RenderTransformOrigin = new Point(0.5, 0.5);
                container.RenderTransform = translateNewView;
            }

            var animation = new DoubleAnimation(newView.ActualWidth + 20, 0, new Duration(TimeSpan.FromMilliseconds(AnimationDuration)))
                                { DecelerationRatio = 1 };

            translateNewView.BeginAnimation(TranslateTransform.XProperty, animation);

            if (currentView != null)
                currentView.Visibility = Visibility.Collapsed;

            newView.Visibility = Visibility.Visible;
        }

        public static void FadeTransition(FrameworkElement currentView, FrameworkElement newView)
        {
            if (currentView != null)
            {
                currentView.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(AnimationDuration)), FillBehavior.Stop));
            }
            newView.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(AnimationDuration))));
        }
    }
}
