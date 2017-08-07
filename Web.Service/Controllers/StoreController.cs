// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Common.Utils;

namespace Web.Service.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Fabric.Query;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Common;
    using Inventory.Domain;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class StoreController : ApiController
    {
        public const string InventoryServiceName = "InventoryService";
        private static FabricClient fc = new FabricClient();

        /// <summary>
        /// Right now, this method makes an API call via a ServiceProxy to retrieve Inventory Data directly
        /// from InventoryService. In the future, this call will be made with a specified category parameter, 
        /// and based on this could call a specific materialized view to return. There would be no option 
        /// to return the entire inventory service in one call, as this would be slow and expensive at scale.  
        /// </summary>
        /// <returns>Task of type IEnumerable of InventoryItemView objects</returns>
        [HttpGet]
        [Route("api/store")]
        public async Task<IEnumerable<InventoryItemView>> GetStore()
        {
            var builder = new ServiceUriBuilder(InventoryServiceName);
            var serviceName = builder.ToUri();
            var itemList = new List<InventoryItemView>();
            var partitions = await fc.QueryManager.GetPartitionListAsync(serviceName);

            foreach (Partition p in partitions)
            {
                var minKey = ((Int64RangePartitionInformation) p.PartitionInformation).LowKey;
                var inventoryServiceClient = ServiceProxy
                    .Create<IInventoryService>(serviceName, new ServicePartitionKey(minKey));

                var result = await inventoryServiceClient
                    .GetCustomerInventoryAsync(CancellationToken.None);
                if (result != null)
                {
                    itemList.AddRange(result);
                }
            }

            return itemList;
        }
    }
}