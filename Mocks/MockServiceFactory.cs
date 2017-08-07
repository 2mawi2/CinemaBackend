using System;
using Common.Factorys;
using Microsoft.ServiceFabric.Services.Client;
using Reservation.Domain;
using Web.Service.Controllers;

namespace Mocks
{
    public class MockReservationServiceFactory : IServiceFactory<IReservationService>
    {
        private readonly IReservationService _serviceOverrride;

        public MockReservationServiceFactory(IReservationService serviceOverrride = null)
        {
            _serviceOverrride = serviceOverrride;
        }

        public IReservationService Get(ServicePartitionKey id)
        {
            return _serviceOverrride ?? new MockReservationService();
        }

        public Uri GetServiceUri()
        {
            return new Uri("fabric:/someapp");
        }
    }
}