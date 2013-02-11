using System;
using System.Web.Mvc;
using DL.Framework.Common;

namespace DL.Framework.Web
{
    public class BusDateModelBinder : DefaultModelBinder
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

        internal BusDate BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = bindingContext.Model as BusDate;

            var displayName = bindingContext.ModelMetadata.DisplayName;
            var fullPropertyKey = bindingContext.ModelName;
            var valueResult = bindingContext.ValueProvider.GetValue(fullPropertyKey);

            if (valueResult != null)
            {
                if (!string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
                {
                    DateTime dateTime;
                    if (DateTime.TryParse(valueResult.AttemptedValue, out dateTime))
                    {
                        model = new BusDate(dateTime);
                    }
                    else
                    {
                        bindingContext.ModelState.AddModelError(fullPropertyKey, String.Format("The value '{0}' is not valid for {1}.", valueResult.AttemptedValue, displayName), valueResult.AttemptedValue);
                    }
                }
            }
            else
            {
                bindingContext.ModelState.AddModelError(fullPropertyKey, String.Format("Unable to locate value for {0}", displayName), valueResult.AttemptedValue);
            }

            return model;
        }
    }
}
