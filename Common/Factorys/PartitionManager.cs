using System;
using System.Fabric;
using System.Fabric.Query;
using System.Threading.Tasks;

namespace Common.Factorys
{
    public class PartitionManager : IPartitionManager
    {
        private static readonly FabricClient FabricClient = new FabricClient();

        public Task<ServicePartitionList> GetPartitionListAsync(Uri serviceName)
        {
            return FabricClient.QueryManager.GetPartitionListAsync(serviceName);
        }
    }
}