using System;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Factorys
{
    public interface IServiceFactory<out TService> where TService : IService
    {
        TService Get(ServicePartitionKey id);
        Uri GetServiceUri();
    }
}