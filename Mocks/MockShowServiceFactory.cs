using System;
using Common.Factorys;
using Microsoft.ServiceFabric.Services.Client;
using Show.Domain;
using Web.Service.Controllers;

namespace Reservation.UnitTests
{
    public class MockShowServiceFactory : IServiceFactory<IShowService>
    {
        private readonly IShowService _serviceOverrride;

        public MockShowServiceFactory(IShowService serviceOverrride = null)
        {
            _serviceOverrride = serviceOverrride;
        }

        public IShowService Get(ServicePartitionKey id)
        {
            return _serviceOverrride ?? new MockShowService();
        }

        public Uri GetServiceUri()
        {
            return new Uri("fabric:/someapp");
        }
    }
}