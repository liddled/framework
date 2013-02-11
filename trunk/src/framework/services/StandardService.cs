using System;
using System.ComponentModel;
using System.Configuration;
using System.ServiceProcess;
using Common.Logging;

namespace DL.Framework.Services
{
    [DesignerCategory("Code")]
    public abstract class StandardService : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        
        protected static string FullServiceName;
        protected Container components = null;

        protected StandardService()
        {
            this.CanPauseAndContinue = true;
            this.ServiceName = FullServiceName;
            InitializeComponent();
        }

        static StandardService()
        {
            var serviceName = ConfigurationManager.AppSettings["Component"];
            if (String.IsNullOrEmpty(serviceName))
                throw new ApplicationException("Component is mandatory in the app.config for the required service");
            FullServiceName = serviceName;
        }

        public static void RunMain(string[] args, StandardService service)
        {
            try
            {
                if (args.Length >= 1 && args[0].ToLower() == "/debug")
                {
                    var remainingArgs = new string[args.Length - 1];
                    if (args.Length > 1)
                        Array.Copy(args, 1, remainingArgs, 0, args.Length - 1);

                    service.OnStart(remainingArgs);
                    Console.WriteLine("Press any key to stop service...");
                    Console.ReadLine();
                    service.OnStop();
                }
                else
                {
                    Log.DebugFormat("Starting up as installed service");
                    Run(service);
                }
            }
            catch (Exception ex)
            {
                var exceptionString = String.Format("Unexpected exception on main service thread");
                Log.Error(exceptionString, ex);
            }
        }

        private void InitializeComponent()
        {
            components = new Container();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
