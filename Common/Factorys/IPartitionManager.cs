using System;
using System.Fabric.Query;
using System.Threading.Tasks;

namespace Common.Factorys
{
    public  interface IPartitionManager
    {
        Task<ServicePartitionList> GetPartitionListAsync(Uri serviceName);
    }
}