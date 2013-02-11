using System;
using System.Web.Mvc;
using DL.Framework.Common;

namespace DL.Framework.Web
{
    public abstract class DomainObjectModelBinder<T> : DefaultModelBinder where T : class, IItemKey
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var domainObject = base.BindModel(controllerContext, bindingContext) as DomainObject<T>;

            if (domainObject != null)
            {
                if (!String.IsNullOrEmpty(controllerContext.HttpContext.Request["delete"]))
                {
                    domainObject.Status = controllerContext.HttpContext.Request["delete"].Contains("true") ? Status.Inactive : Status.Active;
                }
            }

            return domainObject;
        }
    }
}
