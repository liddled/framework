using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DL.Framework.Services
{
    public class DependencyInjectionServiceHost : ServiceHost
    {
        public delegate object CreatorDelegate();

        private readonly CreatorDelegate _creator;

        public DependencyInjectionServiceHost(Type type, CreatorDelegate creator, params Uri[] baseAddresses)
            : base(type, baseAddresses)
        {
            _creator = creator;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(new DependencyInjectionServiceBehaviour(_creator));
            base.OnOpening();
        }

        public class DependencyInjectionInstanceProvider : IInstanceProvider
        {
            private readonly CreatorDelegate _creator;

            public DependencyInjectionInstanceProvider(CreatorDelegate creator)
            {
                _creator = creator;
            }

            public object GetInstance(InstanceContext instanceContext, Message message)
            {
                return GetInstance(instanceContext);
            }

            public object GetInstance(InstanceContext instanceContext)
            {
                return _creator();
            }

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
            }
        }

        public class DependencyInjectionServiceBehaviour : IServiceBehavior
        {
            private readonly CreatorDelegate _creator;

            public DependencyInjectionServiceBehaviour(CreatorDelegate creator)
            {
                _creator = creator;
            }

            public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
                foreach (var cdb in serviceHostBase.ChannelDispatchers)
                {
                    var cd = cdb as ChannelDispatcher;
                    if (cd == null) continue;
                    foreach (var ed in cd.Endpoints)
                    {
                        ApplyDispatchBehavior(ed);
                    }
                }
            }

            private void ApplyDispatchBehavior(EndpointDispatcher ed)
            {
                ed.DispatchRuntime.InstanceProvider = new DependencyInjectionInstanceProvider(_creator);
            }

            public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
            {
            }
        }
    }
}
