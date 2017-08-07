using System;
using Common.Factorys;
using Common.Utils;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Show.Domain
{
    public class ShowServiceFactory : IServiceFactory<IShowService>
    {
        private const string ServiceName = "ShowService";

        public IShowService Get(ServicePartitionKey servicePartitionKey)
        {
            return new ServiceProxyFactory().CreateServiceProxy<IShowService>(GetServiceUri(), servicePartitionKey);
        }

        public Uri GetServiceUri() => new ServiceUriBuilder(ServiceName).ToUri();
    }
}