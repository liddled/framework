using System.Diagnostics;
using System.Web.Mvc;
using Common.Logging;

namespace DL.Framework.Web
{
    public class StopWatchAttribute : ActionFilterAttribute
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var stopWatch = Stopwatch.StartNew();
            filterContext.Controller.ViewBag.StopWatch = stopWatch;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var controler = filterContext.RouteData.Values["controller"].ToString();
            var action = filterContext.RouteData.Values["action"].ToString();
            var url = filterContext.HttpContext.Request.Url.ToString();
            
            var timer = (Stopwatch)filterContext.Controller.ViewBag.StopWatch;
            timer.Stop();

            Log.DebugFormat("{0}/{1} ({2}) load time: {3}", controler, action, url, timer.Elapsed);
        }
    }
}
