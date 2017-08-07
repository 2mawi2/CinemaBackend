using System.Fabric;
using System.Fabric.Query;
using Microsoft.ServiceFabric.Services.Client;

namespace Common.Extensions
{
    public static class ServiceFabricExtensions
    {
        public static ServicePartitionKey GetPartitionKey(this Partition source)
        {
            return source == null ? null : new ServicePartitionKey(((Int64RangePartitionInformation)source.PartitionInformation).LowKey);
        }
    }
}
