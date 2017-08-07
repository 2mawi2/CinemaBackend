using System;
using System.Fabric.Query;
using System.Threading.Tasks;
using Common.Factorys;
using Web.Service.Controllers;

namespace Mocks
{
    public class MockPartitionManager : IPartitionManager
    {
        public async Task<ServicePartitionList> GetPartitionListAsync(Uri serviceName)
        {
            return await Task.FromResult(new ServicePartitionList
            {
                null,
                null,
                null
            });
        }
    }
}