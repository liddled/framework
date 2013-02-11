using System;
using System.Web.Mvc;
using DL.Framework.Common;

namespace DL.Framework.Web
{
    public class StatusModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            return BindComplexModel(controllerContext, bindingContext);
        }

        internal Status BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = Status.Active;

            var displayName = bindingContext.ModelMetadata.DisplayName ?? "Status";
            var fullPropertyKey = bindingContext.ModelName;

            var valueResult = bindingContext.ValueProvider.GetValue(fullPropertyKey);

            if (valueResult != null && !String.IsNullOrWhiteSpace(valueResult.AttemptedValue))
            {
                try
                {
                    model = TextRepTx.ToEnum<Status>(valueResult.AttemptedValue, ETextRepType.Web);
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(fullPropertyKey, String.Format("The value '{0}' is not valid for {1}.", valueResult.AttemptedValue, displayName), valueResult.AttemptedValue);
                }
            }

            return model;
        }
    }
}
