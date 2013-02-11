using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;
using Common.Logging;

namespace DL.Framework.Services
{
    public class FaultHandledServiceHost
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private ServiceHost _serviceHost;
        private readonly Type _serviceType;
        private readonly DependencyInjectionServiceHost.CreatorDelegate _creator;
        private readonly Uri[] _baseAddresses;

        private bool _closing;
        private string _announcementEndpointAddress;

        public FaultHandledServiceHost(Type serviceType, DependencyInjectionServiceHost.CreatorDelegate creator, params Uri[] baseAddresses)
        {
            _serviceType = serviceType;
            _creator = creator;
            _baseAddresses = baseAddresses;
        }

        public void Open(string announcementEndpointAddress = null)
        {
            _announcementEndpointAddress = announcementEndpointAddress;
            _serviceHost = _creator == null ? new ServiceHost(_serviceType) : new DependencyInjectionServiceHost(_serviceType, _creator, _baseAddresses);
            _serviceHost.Faulted += ServiceHostFaulted;
            _serviceHost.Open();
            AnnounceEndPoints(online: true);
        }

        public void AnnounceEndPoints(bool online)
        {
            if (String.IsNullOrWhiteSpace(_announcementEndpointAddress))
                return;

            var announcementEndpoint = new AnnouncementEndpoint(new NetTcpBinding(SecurityMode.None),
                                                                new EndpointAddress(new Uri(_announcementEndpointAddress)));
            var netTcpEndpoints = _serviceHost.Description.Endpoints.Where(e => e.Address.Uri.AbsoluteUri.StartsWith("net.tcp")).ToArray();

            var announced = false;
            var countTry = online ? 60 : 1;
            while (!announced && countTry > 0)
            {
                try
                {
                    var client = new AnnouncementClient(announcementEndpoint);
                    foreach (var tcpEndpoint in netTcpEndpoints)
                    {
                        var endpointDiscoveryMetadata = EndpointDiscoveryMetadata.FromServiceEndpoint(tcpEndpoint);
                        if (endpointDiscoveryMetadata == null) continue;
                        if (online)
                        {
                            client.AnnounceOnline(endpointDiscoveryMetadata);
                        }
                        else
                        {
                            client.AnnounceOffline(endpointDiscoveryMetadata);
                        }
                    }
                    announced = true;
                }
                catch (EndpointNotFoundException e)
                {
                    Log.WarnFormat("Announce end point {0} not found.", announcementEndpoint);
                    Log.WarnFormat("error message :{0}", e.Message);
                    Log.WarnFormat("error stack trace:{0}", e.StackTrace);
                    Thread.Sleep(1000);
                }
                finally
                {
                    countTry--;
                }
            }

            if (!announced && online)
                Log.ErrorFormat("The service ({0}) end points were not announced.", _serviceType.Name);
        }

        public void Close()
        {
            if (_serviceHost == null) return;
            AnnounceEndPoints(online: false);
            _closing = true;
            _serviceHost.Close();

            if (_serviceHost.State != CommunicationState.Closed)
            {
                Log.ErrorFormat("Aborting the service {0}...", _serviceType.Name);
                _serviceHost.Abort();
            }

            _serviceHost = null;
            _closing = false;
        }

        private void ServiceHostFaulted(object sender, EventArgs e)
        {
            if (_closing)
                return;

            string errorMsg = String.Format("Service faulted, restarting ({0}", _serviceType.Name);
            Log.Error(errorMsg);
            _serviceHost = _creator == null ? new ServiceHost(_serviceType) : new DependencyInjectionServiceHost(_serviceType, _creator, _baseAddresses);
            _serviceHost.Faulted += ServiceHostFaulted;
        }
    }
}
