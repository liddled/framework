using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace DL.Framework.Services
{
    [DesignerCategory("Code")]
    public abstract class StandardServiceInstaller : Installer
    {
        private ServiceProcessInstaller _serviceProcessInstaller;
        private ServiceInstaller _serviceInstaller;

        protected StandardServiceInstaller(string serviceName)
        {
            InitializeComponent();
            _serviceInstaller.ServiceName = serviceName;
        }

        private void InitializeComponent()
        {
            _serviceProcessInstaller = new ServiceProcessInstaller();
            _serviceInstaller = new ServiceInstaller();
            // 
            // ServiceProcessInstaller
            // 
            _serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            _serviceProcessInstaller.Password = null;
            _serviceProcessInstaller.Username = null;
            // 
            // ServiceInstaller
            // 
            _serviceInstaller.StartType = ServiceStartMode.Manual;
            // 
            // ProjectInstaller
            // 
            Installers.AddRange(new Installer[] { _serviceProcessInstaller, _serviceInstaller });
        }
    }
}
