using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Fabric.Query;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Factorys;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.Utils
{
    public class PartitionUtils
    {
        /// <summary>
        /// Queries all Partition of given Service <typeparam name="TService"></typeparam> with given Items <typeparam name="TItem"></typeparam>
        /// </summary>
        /// <typeparam name="TService">Type of all Service instances which will be queried on all partitions</typeparam>
        /// <typeparam name="TItem">Type for what will be queried for</typeparam>
        /// <param name="serviceFactory">Factory of Type <typeparam name="IServiceFactory{TService}"> which creates the service</typeparam></param>
        /// <param name="partitionManager">Partition Manager, which wrapps the partition call to service fabric</param>
        /// <param name="predicate">actual query on the data item</param>
        /// <returns>All Items which had been queried for</returns>
public static async Task<IEnumerable<TItem>> SelectManyService<TService, TItem>(
    IServiceFactory<TService> serviceFactory,
    IPartitionManager partitionManager,
    Func<TService, IEnumerable<TItem>> predicate) where TService : IService
{
    var partitions = await GetServicePartitionList(serviceFactory, partitionManager);
    var items = new BlockingCollection<TItem>();
    Parallel.ForEach(partitions, (partition, state) =>
    {
        var service = serviceFactory.Get(partition.GetPartitionKey());
        var resItems = predicate(service);
        if (resItems.Any())
        {
            resItems.ForEach(i => items.Add(i));
            state.Break();
        }
    });
    return items;
}

        private static async Task<ServicePartitionList> GetServicePartitionList<TService>(IServiceFactory<TService> serviceFactory,
            IPartitionManager partitionManager) where TService : IService
        {
            var uri = serviceFactory.GetServiceUri();
            var partitions = await partitionManager.GetPartitionListAsync(uri);
            return partitions;
        }

        /// <summary>
        /// Queries all Partition of given Service <typeparam name="TService"></typeparam> with given Items <typeparam name="TItem"></typeparam>
        /// </summary>
        /// <typeparam name="TService">Type of all Service instances which will be queried on all partitions</typeparam>
        /// <typeparam name="TItem">Type for what will be queried for</typeparam>
        /// <param name="serviceFactory">Factory of Type <typeparam name="IServiceFactory{TService}"> which creates the service</typeparam></param>
        /// <param name="partitionManager">Partition Manager, which wrapps the partition call to service fabric</param>
        /// <param name="predicate">actual query on the data item</param>
        /// <returns>All Items which had been queried for</returns>
        public static async Task<IEnumerable<TItem>> SelectManyService<TService, TItem>(
            IServiceFactory<TService> serviceFactory,
            IPartitionManager partitionManager,
            Func<TService, TItem> predicate) where TService : IService
        {
            var partitions = await GetServicePartitionList(serviceFactory, partitionManager);
            var items = new BlockingCollection<TItem>();
            Parallel.ForEach(partitions, (partition, state) =>
            {
                var service = serviceFactory.Get(partition.GetPartitionKey());
                var resItem = predicate(service);
                if (resItem != null)
                {
                    items.Add(resItem);
                    state.Break();
                }
            });
            return items;
        }

        /// <summary>
        /// Queries all Partition of given Service <typeparam name="TService"></typeparam> with given Items <typeparam name="TItem"></typeparam>
        /// </summary>
        /// <typeparam name="TService">Type of all Service instances which will be queried on all partitions</typeparam>
        /// <typeparam name="TItem">Type for what will be queried for</typeparam>
        /// <param name="serviceFactory">Factory of Type <typeparam name="IServiceFactory{TService}"> which creates the service</typeparam></param>
        /// <param name="partitionManager">Partition Manager, which wrapps the partition call to service fabric</param>
        /// <param name="action">actual query on the data item</param>
        /// <returns>All Items which had been queried for</returns>
        public static async Task ForEachService<TService>(
            IServiceFactory<TService> serviceFactory,
            IPartitionManager partitionManager,
            Action<TService> action) where TService : IService
        {
            var partitions = await GetServicePartitionList(serviceFactory, partitionManager);
            Parallel.ForEach(partitions, (partition, state) =>
            {
                var service = serviceFactory.Get(partition.GetPartitionKey());
                action(service);
            });
        }
    }
}
