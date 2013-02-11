using System;
using System.Web.Mvc;
using DL.Framework.Common;

namespace DL.Framework.Web
{
    public class DateViewModelBinder : DefaultModelBinder
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

        internal DateView BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = DateView.Month;

            var displayName = bindingContext.ModelMetadata.DisplayName ?? "View";
            var fullPropertyKey = bindingContext.ModelName;

            var valueResult = bindingContext.ValueProvider.GetValue(fullPropertyKey);

            if (valueResult != null && !String.IsNullOrWhiteSpace(valueResult.AttemptedValue))
            {
                try
                {
                    model = TextRepTx.ToEnum<DateView>(valueResult.AttemptedValue, ETextRepType.Web);
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
