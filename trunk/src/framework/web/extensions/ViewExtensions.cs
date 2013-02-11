using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.UI;
using DL.Framework.Common;

namespace DL.Framework.Web
{
    public static class ViewExtensions
    {
        public static readonly string DATE_FORMAT = "dd MMMM yyyy";

        public static readonly string CSS_MASTER_SPRITE = "dl-master-sprite";
        public static readonly string CSS_UI_CORNER_ALL = "dl-ui-corner-all";
        public static readonly string CSS_UI_BUTTON = "dl-ui-button";
        public static readonly string CSS_UI_BUTTON_TEXT = "dl-ui-button-text";
        public static readonly string CSS_UI_DATE = "dl-ui-date";
        public static readonly string CSS_UI_FORM_FIELD = "dl-ui-form-field";
        public static readonly string CSS_UI_FORM_FIELD_SMALL = "dl-ui-form-field-small";
        public static readonly string CSS_UI_FORM_FIELD_LARGE = "dl-ui-form-field-large";
        public static readonly string CSS_UI_ICON = "dl-ui-icon";

        public static readonly string CSS_UI_NEUTRAL = "neutral";
        public static readonly string CSS_UI_NEGATIVE = "negative";
        public static readonly string CSS_UI_POSITIVE = "positive";

        public static readonly string CSS_UI_BOX = "dl-box";
        public static readonly string CSS_UI_BOX_NEGATIVE = "dl-box-negative";
        public static readonly string CSS_UI_BOX_POSITIVE = "dl-box-positive";

        public static MvcHtmlString BaseUrl(this UrlHelper helper)
        {
            var contextUri = new Uri(helper.RequestContext.HttpContext.Request.Url, helper.RequestContext.HttpContext.Request.RawUrl);
            var realmUri = new UriBuilder(contextUri) { Path = helper.RequestContext.HttpContext.Request.ApplicationPath, Query = null, Fragment = null };
            return MvcHtmlString.Create(realmUri.Uri.ToString());
        }

        public static MvcHtmlString Theme(this HtmlHelper helper)
        {
            if (!helper.ViewContext.RouteData.Values.ContainsKey("controller"))
                return MvcHtmlString.Empty;

            return MvcHtmlString.Create(helper.ViewContext.RouteData.Values["controller"].ToString().ToLower());
        }

        public static bool IsCurrentController(this HtmlHelper helper, string controllerName)
        {
            var currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];

            return currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsCurrentAction(this HtmlHelper helper, string actionName, string controllerName)
        {
            var currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
            var currentActionName = (string)helper.ViewContext.RouteData.Values["action"];

            return currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) && currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase);
        }

        #region ModelState

        public static void AddModelError(this ModelStateDictionary modelState, string key, string errorMessage, string attemptedValue)
        {
            modelState.AddModelError(key, errorMessage);
            modelState.SetModelValue(key, new ValueProviderResult(attemptedValue, attemptedValue, null));
        }

        #endregion

        #region Money Formatting

        public static MvcHtmlString CulturedAmount(this decimal value, string format, string locale)
        {
            if (String.IsNullOrEmpty(locale) && HttpContext.Current.Request.UserLanguages != null)
            {
                locale = HttpContext.Current.Request.UserLanguages[0];
            }

            return MvcHtmlString.Create(value.ToString(format, CultureInfo.CreateSpecificCulture(locale)));
        }

        #endregion

        #region Text Formatting

        public static MvcHtmlString BreakText(this string text)
        {
            return String.IsNullOrEmpty(text) ? MvcHtmlString.Empty : MvcHtmlString.Create(text.Replace("\r\n", "</ br>"));
        }

        public static MvcHtmlString PluralText(this HtmlHelper helper, decimal amount, string text, string textPlural)
        {
            return PluralText(amount, text, textPlural);
        }

        public static MvcHtmlString PluralText(decimal amount, string text, string textPlural)
        {
            if (amount == 1 || amount == -1)
                return MvcHtmlString.Create(string.Format("{0}", text));
            
            return MvcHtmlString.Create(string.Format("{0}", textPlural));
        }

        #endregion

        #region Action Links

        public static RouteValueDictionary GetPageRouteValues(HtmlHelper helper)
        {
            var routeValues = new RouteValueDictionary(helper.ViewContext.RouteData.Values);
            var queryString = helper.ViewContext.HttpContext.Request.QueryString;

            return GetPageRouteValues(routeValues, queryString);
        }

        public static RouteValueDictionary GetPageRouteValues(RouteValueDictionary routeValues, NameValueCollection queryString)
        {
            foreach (string key in queryString)
                routeValues[key] = queryString[key];

            return routeValues;
        }

        public static RouteValueDictionary GetPageRouteValues(HtmlHelper helper, object routeValues)
        {
            return GetPageRouteValues(helper, routeValues, null);
        }

        public static RouteValueDictionary GetPageRouteValues(HtmlHelper helper, object routeValues, object removeRouteValues)
        {
            var pageRoutes = GetPageRouteValues(helper);

            if (routeValues != null)
            {
                var linkRoutes = new RouteValueDictionary(routeValues);
                foreach (var route in linkRoutes)
                {
                    if (pageRoutes.ContainsKey(route.Key) && route.Key.Equals("categories", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var value = pageRoutes[route.Key].ToString();
                        if (!value.Contains(route.Value.ToString()))
                        {
                            var valueList = value.SplitToList();
                            valueList.Add(route.Value.ToString());
                            pageRoutes[route.Key] = valueList.ConvertListToString(Constants.Space);
                        }
                    }
                    else
                    {
                        pageRoutes[route.Key] = route.Value;
                    }
                }
            }

            if (removeRouteValues != null)
            {
                var linkRoutes = new RouteValueDictionary(removeRouteValues);
                foreach (var route in linkRoutes)
                {
                    if (!pageRoutes.ContainsKey(route.Key)) continue;
                    var value = pageRoutes[route.Key].ToString();
                    if (value.Contains(route.Value.ToString()))
                    {
                        var valueList = value.SplitToList();
                        valueList.Remove(route.Value.ToString());
                        if (valueList.Count > 0)
                        {
                            pageRoutes[route.Key] = valueList.ConvertListToString(Constants.Space);
                        }
                        else
                        {
                            pageRoutes.Remove(route.Key);
                        }
                    }
                    else
                    {
                        pageRoutes.Remove(route.Key);
                    }
                }
            }

            return pageRoutes;
        }
        

        public static MvcHtmlString PageRouteActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return helper.PageRouteActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes, null);
        }

        public static MvcHtmlString PageRouteActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, object removeRouteValues)
        {
            var pageRoutes = GetPageRouteValues(helper, routeValues, removeRouteValues);
            return helper.ActionLink(linkText, actionName, controllerName, pageRoutes, new RouteValueDictionary(htmlAttributes));
        }

        #endregion

        #region Buttons

        public static MvcHtmlString Button(this HtmlHelper helper, string id, string text)
        {
            return helper.Button(id, "button", null, text, null);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string id, string type, string text)
        {
            return helper.Button(id, type, null, text, null);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string id, string type, string href, string text)
        {
            return helper.Button(id, type, href, text, null);
        }

        public static MvcHtmlString Button(this HtmlHelper helper, string id, string type, string href, string text, string image)
        {
            var button = new TagBuilder("button");
            button.Attributes.Add("id", id);
            button.Attributes.Add("type", type);
            button.AddCssClass(CSS_UI_BUTTON);

            if (!String.IsNullOrEmpty(href))
            {
                button.Attributes.Add("href", href);
                button.Attributes.Add("onclick", string.Format("window.location=this.getAttribute('href');return false;"));
            }

            if (!String.IsNullOrEmpty(image))
            {
                var img = new TagBuilder("span");
                img.AddCssClass(CSS_MASTER_SPRITE);
                img.AddCssClass(image);

                button.InnerHtml += img.ToString();

                if (!String.IsNullOrEmpty(text)) button.InnerHtml += " ";
            }

            if (!String.IsNullOrEmpty(text))
            {
                var span = new TagBuilder("span");
                span.AddCssClass(CSS_UI_BUTTON_TEXT);
                span.InnerHtml = text;

                button.InnerHtml += span.ToString();
            }

            return MvcHtmlString.Create(button.ToString());
        }

        public static MvcHtmlString IconButton(this HtmlHelper helper, string id, string type, string imageClass)
        {
            var button = new TagBuilder("button");
            button.Attributes.Add("id", id);
            button.Attributes.Add("type", type);
            button.AddCssClass(CSS_MASTER_SPRITE);
            button.AddCssClass(CSS_UI_ICON);
            button.AddCssClass(imageClass);

            return MvcHtmlString.Create(button.ToString(TagRenderMode.SelfClosing));
        }

        #endregion

        #region Drop Down List

        public static MvcHtmlString CascadingDropDownScript(this HtmlHelper helper, string selectId1, string selectId2, IEnumerable items, string dataKeyField, string dataValueField, string dataTextField)
        {
            var jsArray = String.Join(",", 
                (from object item in items 
                 let key = DataBinder.GetPropertyValue(item, dataKeyField).ToString()
                 let value = DataBinder.GetPropertyValue(item, dataValueField).ToString()
                 let text = DataBinder.GetPropertyValue(item, dataTextField).ToString()
                 select String.Format("{{key:'{0}',value:'{1}',text:'{2}'}}", key, value, text)).ToArray());

            var script = new TagBuilder("script");
            script.Attributes.Add("type", "text/javascript");

            var scriptHtml = new StringBuilder();

            scriptHtml.AppendFormat("$('#{0}').change(function()", selectId1).Append("{");
            scriptHtml.AppendFormat("$('#{0} > option').remove();", selectId2);
            scriptHtml.Append("var selectedValue=$(this).val();");
            scriptHtml.AppendFormat("$.each([{0}],function(index, option)", jsArray).Append("{");
            scriptHtml.Append("if(selectedValue==option.key){");
            scriptHtml.AppendFormat("$('#{0}').append(", selectId2);
            scriptHtml.Append("$('<option></option>').val(option.value).html(option.text)");
            scriptHtml.Append(");}});});");

            script.InnerHtml = scriptHtml.ToString();

            return MvcHtmlString.Create(script.ToString());
        }

        public static MvcHtmlString StatusDropDownList(this HtmlHelper helper, string actionName, string controllerName)
        {
            object selectedStatus = TextRepTx.ToText(Status.Active, ETextRepType.Web);
            var strSelectedStatus = helper.ViewContext.HttpContext.Request.QueryString["status"];

            if (!string.IsNullOrEmpty(strSelectedStatus))
                selectedStatus = TextRepTx.ToEnum<Status>(strSelectedStatus, ETextRepType.Web);

            IDictionary<string, string> values = new Dictionary<string, string>();

            foreach (Status item in Enum.GetValues(typeof(Status)))
                values.Add(TextRepTx.ToText(item, ETextRepType.Web), TextRepTx.ToText(item, ETextRepType.Display));

            var routeValues = GetPageRouteValues(helper);
            routeValues["controller"] = controllerName;
            routeValues["action"] = actionName;
            routeValues.Remove("status");

            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var href = url.Action(actionName, controllerName, routeValues);

            return helper.DropDownList("status", 
                new SelectList(values, "key", "value", selectedStatus), 
                new { onchange = string.Format("location.href='{0}{1}status='+this.value", href, href.Contains("?") ? "&" : "?") }
            );
        }

        public static MvcHtmlString DateViewDropDownList(this HtmlHelper helper, string actionName, string controllerName)
        {
            object selectedView = TextRepTx.ToText(DateView.Month, ETextRepType.Web);
            var strSelectedView = helper.ViewContext.HttpContext.Request.QueryString["view"];

            if (!String.IsNullOrEmpty(strSelectedView))
                selectedView = TextRepTx.ToEnum<DateView>(strSelectedView, ETextRepType.Web);

            IDictionary<string, string> values = new Dictionary<string, string>();

            foreach (DateView item in Enum.GetValues(typeof(DateView)))
                values.Add(TextRepTx.ToText(item, ETextRepType.Web), TextRepTx.ToText(item, ETextRepType.Display));

            var routeValues = GetPageRouteValues(helper);
            routeValues["controller"] = controllerName;
            routeValues["action"] = actionName;
            routeValues.Remove("view");

            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var href = url.Action(actionName, controllerName, routeValues);

            return helper.DropDownList("view",
                new SelectList(values, "key", "value", selectedView),
                new { onchange = string.Format("location.href='{0}{1}view='+this.value", href, href.Contains("?") ? "&" : "?") }
            );
        }

        #endregion

        #region CSS

        public static MvcHtmlString ColourClass(this HtmlHelper helper, decimal value)
        {
            return MvcHtmlString.Create(
                (value == 0) ? CSS_UI_NEUTRAL : (value < 0) ? CSS_UI_NEGATIVE : CSS_UI_POSITIVE
            );
        }

        public static MvcHtmlString BoxClass(this HtmlHelper helper, decimal value)
        {
            return MvcHtmlString.Create(
                (value == 0) ? CSS_UI_BOX : (value < 0) ? CSS_UI_BOX_NEGATIVE : CSS_UI_BOX_POSITIVE
            );
        }

        #endregion

        public static Int64 GetUniqueId()
        {
            var dt = new DateTime(1970, 1, 1);
            var ts = (DateTime.Now.ToUniversalTime() - dt);
            return (Int64)(ts.TotalMilliseconds + 0.5);
        }
    }
}
