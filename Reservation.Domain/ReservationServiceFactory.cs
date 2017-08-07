using System;
using Common.Factorys;
using Common.Utils;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Reservation.Domain
{
    public class ReservationServiceFactory : IServiceFactory<IReservationService>
    {
        private const string ServiceName = "ReservationService";

        public IReservationService Get(ServicePartitionKey servicePartitionKey)
        {
            return new ServiceProxyFactory()
                .CreateServiceProxy<IReservationService>(GetServiceUri(), servicePartitionKey);
        }

        public Uri GetServiceUri() => new ServiceUriBuilder(ServiceName).ToUri();
    }
}