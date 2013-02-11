/*using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;

namespace DL.Framework.WPF
{
    public class ViewManager : INotifyCollectionChanged
    {
        #region Fields

        private Panel m_panel;
        private TransitionCallBackDelegate m_transition;
        private ObservableCollection<ViewContainer> m_views;
        private Stack<FrameworkElement> m_currentViews;

        #endregion

        #region Constructors

        private ViewManager(Panel panel, TransitionCallBackDelegate transition)
        {
            m_panel = panel;
            m_transition = transition;
            m_views = new ObservableCollection<ViewContainer>();
            m_currentViews = new Stack<FrameworkElement>();
        }

        public static ViewManager Create(Panel panel, TransitionCallBackDelegate transition)
        {
            if (panel == null)
                throw new ArgumentNullException("panel");

            return new ViewManager(panel, transition);
        }

        #endregion

        #region Properties

        public FrameworkElement CurrentView
        {
            get
            {
                if (m_currentViews.Count == 0)
                    return null;

                return m_currentViews.Peek();
            }
            set
            {
                m_currentViews.Clear();
                if (value != null)
                    m_currentViews.Push(value);
            }
        }

        #endregion

        #region Implementation

        private FrameworkElement GetView(ViewKey key)
        {
            var container = m_views.SingleOrDefault(f => f.ViewKey.Equals(key));

            if (container == null)
                return null;

            return container.View;
        }

        public void Transition<T>(string key)
            where T : FrameworkElement
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            Transition<T>(new ViewKey(key), null);
        }

        public void Transition<T>(ViewKey viewKey, object dataContext)
            where T : FrameworkElement
        {
            if (viewKey == null)
                throw new ArgumentNullException("viewKey");

            var view = GetView(viewKey);

            if (view == null)
            {
                view = (T)Activator.CreateInstance(typeof(T));
                m_views.Add(new ViewContainer(viewKey, view));
            }

            //If objNavigateKey.RecordPrimaryKey IsNot Nothing AndAlso TypeOf objDataContext Is ILoadable Then
            //    CType(objDataContext, ILoadable).LoadRecord(objNavigateKey.RecordPrimaryKey)

            if (view.DataContext is ILoadable)
                ((ILoadable)view.DataContext).Load(viewKey);

            PerformTransition(viewKey);
            
            //ElseIf objNavigateKey.RecordPrimaryKey IsNot Nothing AndAlso TypeOf fwe Is ILoadable Then
            //    CType(fwe, ILoadable).LoadRecord(objNavigateKey.RecordPrimaryKey)
            //End If

            //if (dataContext != null)
                //view.DataContext = dataContext;
        }

        private void PerformTransition(ViewKey viewKey)
        {
            var newView = GetView(viewKey);
            var currentView = CurrentView;

            if (currentView != null && currentView.Equals(newView))
                return;

            if (!m_panel.Children.Contains(newView))
                m_panel.Children.Add(newView);

            foreach (FrameworkElement element in m_panel.Children)
                if (element != newView && element != currentView)
                    if (element.Visibility == Visibility.Visible)
                        element.Visibility = Visibility.Collapsed;

            if (currentView != null)
            {
                //TODO some stuff missing here
                currentView.Visibility = Visibility.Collapsed;
                currentView.SetValue(Panel.ZIndexProperty, 0);
            }

            newView.SetValue(Panel.ZIndexProperty, 99);

            newView.Visibility = Visibility.Visible;
            newView.Opacity = 1;

            m_panel.InvalidateVisual();

            try
            {
                newView.UpdateLayout();
            }
            catch (Exception ex)
            {
                var error = new StringBuilder();
                error.Append(ex.Message);
                if (ex.InnerException != null)
                {
                    error.Append(Environment.NewLine).Append(Environment.NewLine);
                    error.Append(ex.InnerException.Message);
                    error.Append(ex.InnerException.StackTrace);
                }
                MessageBox.Show(error.ToString());
            }

            if (m_transition != null)
            {
                m_transition(currentView, newView);
            }
            else if (currentView != null)
            {
                currentView.Visibility = Visibility.Collapsed;
            }

            CurrentView = newView;

            m_panel.Visibility = Visibility.Visible;
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
*/