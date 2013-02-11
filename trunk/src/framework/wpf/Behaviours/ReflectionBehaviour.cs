using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace DL.Framework.WPF
{
    public class ReflectionBehavior : Behavior<FrameworkElement>
    {
        private Border _reflectionContainer;

        protected override void OnAttached()
        {
            base.OnAttached();

            ReplaceControlWithProxyGrid();

            AddReflectionEffect();
        }

        private void ReplaceControlWithProxyGrid()
        {
            var associatedObjectParent = AssociatedObject.Parent;

            if (associatedObjectParent is Panel)
            {
                var gridProxy = new Grid();
                _reflectionContainer = new Border();

                var parentPanel = (Panel)associatedObjectParent;
                int indexOfAssociatedObject = parentPanel.Children.IndexOf(AssociatedObject);
                parentPanel.Children.RemoveAt(indexOfAssociatedObject);
                gridProxy.Children.Add(AssociatedObject);
                gridProxy.Children.Add(_reflectionContainer);
                parentPanel.Children.Insert(indexOfAssociatedObject, gridProxy);

                MoveGridProperties(AssociatedObject, gridProxy);
            }
            else if (associatedObjectParent is ContentControl)
            {
                var gridProxy = new Grid();
                _reflectionContainer = new Border();

                var parentContentControl = (ContentControl)associatedObjectParent;
                parentContentControl.Content = null;
                gridProxy.Children.Add(AssociatedObject);
                gridProxy.Children.Add(_reflectionContainer);
                parentContentControl.Content = gridProxy;

                MoveGridProperties(AssociatedObject, gridProxy);
            }
            else
            {
                throw new NotImplementedException(String.Format("The ReflectionBehavior doesn't support {0} as a parent for the element to reflect", associatedObjectParent.GetType().ToString()));
            }
        }

        private static void MoveGridProperties(DependencyObject sourceObject, DependencyObject targetObject)
        {
            // move grid attached properties from control to proxy
            MoveProperty(sourceObject, targetObject, Grid.ColumnProperty);
            MoveProperty(sourceObject, targetObject, Grid.ColumnSpanProperty);
            MoveProperty(sourceObject, targetObject, Grid.RowProperty);
            MoveProperty(sourceObject, targetObject, Grid.RowSpanProperty);
            MoveProperty(sourceObject, targetObject, Grid.IsSharedSizeScopeProperty);
        }

        /// <summary>
        /// Moves dependency property value from a source to target, if exists.
        /// </summary>
        /// <param name="sourceObject">The source object.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="property">The property to copy.</param>
        private static void MoveProperty(DependencyObject sourceObject, DependencyObject targetObject, DependencyProperty property)
        {
            // get value from source object
            object propertyValue = sourceObject.ReadLocalValue(property);

            // if property value exists
            if (propertyValue != DependencyProperty.UnsetValue)
            {
                // copy value on target object
                targetObject.SetValue(property, propertyValue);

                // remove value from source object
                sourceObject.ClearValue(property);
            }
        }

        private void AddReflectionEffect()
        {
            // set reflection container height and width 
            _reflectionContainer.SetBinding(FrameworkElement.HeightProperty, new Binding("ActualHeight") { Source = AssociatedObject });
            _reflectionContainer.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth") { Source = AssociatedObject });

            // set reflection transparency effect
            var opacityBrush = new LinearGradientBrush
            {
                StartPoint = new Point(1, 0),
            };
            opacityBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(128, 0, 0, 0), Offset = 0 });
            opacityBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0, 0, 0, 0), Offset = 0.05 });
            opacityBrush.GradientStops.Add(new GradientStop { Color = Color.FromArgb(0, 0, 0, 0), Offset = 0.2 });
            _reflectionContainer.OpacityMask = opacityBrush;

            // set reflection effect
            var visualBrush = new VisualBrush
            {
                Visual = AssociatedObject,
                AutoLayoutContent = false,
                Stretch = Stretch.None
            };
            var transformGroup = new TransformGroup();
            var scaleTransform = new ScaleTransform(1, -1, 0, 1);
            var translateTransform = new TranslateTransform(0, -1);
            transformGroup.Children.Add(scaleTransform);
            transformGroup.Children.Add(translateTransform);
            visualBrush.RelativeTransform = transformGroup;
            _reflectionContainer.Background = visualBrush;

            // move reflection effect to the bottom of the control
            var renderTranslateTransform = new TranslateTransform();
            BindingOperations.SetBinding(renderTranslateTransform, TranslateTransform.YProperty, new Binding("ActualHeight") { Source = AssociatedObject });
            _reflectionContainer.RenderTransform = renderTranslateTransform;
        }
    }
}
