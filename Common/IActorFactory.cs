using System;
using Microsoft.ServiceFabric.Actors;

namespace Common
{
    public interface IActorFactory
    {
        TActor Get<TActor>(string serviceName, Guid guid) where TActor : IActor;
    }
}