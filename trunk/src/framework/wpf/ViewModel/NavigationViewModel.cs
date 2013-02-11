using System;
using System.Collections.Generic;
using System.Linq;
using DL.Framework.Common;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace DL.Framework.WPF
{
    public abstract class NavigationViewModel<T> : NotifyViewModel, INavigationAware, IViewModel where T : ViewModelKey
    {
        public T ViewModelKey { get; private set; }

        protected virtual T GetViewModelKey(UriQuery query)
        {
            return null;
        }

        protected virtual void Subscribe()
        {
        }

        protected virtual void UnSubscribe()
        {
        }

        public virtual void Load()
        {
        }

        public IEnumerable<DateView> DateViewEnumValues
        {
            get { return Enum.GetValues(typeof(DateView)).Cast<DateView>(); }
        }

        public IEnumerable<Status> StatusEnumValues
        {
            get { return Enum.GetValues(typeof(Status)).Cast<Status>(); }
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UnSubscribe();
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            UnSubscribe();
            
            ViewModelKey = GetViewModelKey(navigationContext.Parameters);
            Load();

            Subscribe();
        }
    }
}

