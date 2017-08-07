using System;
using Common.Utils;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace Common.Factorys
{
    public class ActorFactory : IActorFactory
    {
        public TActor Get<TActor>(string serviceName, Guid guid) where TActor : IActor
        {
            return ActorProxy.Create<TActor>(new ActorId(guid), new ServiceUriBuilder(serviceName).ToUri());
        }
    }
}
