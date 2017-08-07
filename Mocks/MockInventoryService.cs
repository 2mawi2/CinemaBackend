// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

using Common.Model;

namespace Mocks
{
    using Common;
    using Inventory.Domain;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockInventoryService : IInventoryService
    {
        public MockInventoryService()
        {
            AddStockAsyncFunc = (itemId, quantity) => Task.FromResult(quantity);
            RemoveStockAsyncFunc = (itemId, quantity, amId) => Task.FromResult(quantity);
            IsItemInInventoryAsyncFunc = (itemId) => Task.FromResult(true);
            GetCustomerInventoryAsyncFunc =
                () => Task.FromResult<IEnumerable<InventoryItemView>>(
                    new List<InventoryItemView>() {new InventoryItemView()});
            CreateInventoryItemAsyncFunc = item => Task.FromResult(true);
        }

        public Func<InventoryItemId, int, Task<int>> AddStockAsyncFunc { get; set; }

        public Func<InventoryItem, Task<bool>> CreateInventoryItemAsyncFunc { get; set; }

        public Func<Task<IEnumerable<InventoryItemView>>> GetCustomerInventoryAsyncFunc { get; set; }

        public Func<InventoryItemId, Task<bool>> IsItemInInventoryAsyncFunc { get; set; }

        public Func<InventoryItemId, int, CustomerOrderActorMessageId, Task<int>> RemoveStockAsyncFunc { get; set; }

        public Task<int> AddStockAsync(InventoryItemId itemId, int quantity)
        {
            return AddStockAsyncFunc(itemId, quantity);
        }

        public Task<bool> CreateInventoryItemAsync(InventoryItem item)
        {
            return CreateInventoryItemAsyncFunc(item);
        }

        public Task<IEnumerable<InventoryItemView>> GetCustomerInventoryAsync()
        {
            return GetCustomerInventoryAsyncFunc();
        }

        public Task<IEnumerable<InventoryItemView>> GetCustomerInventoryAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsItemInInventoryAsync(InventoryItemId itemId)
        {
            return IsItemInInventoryAsyncFunc(itemId);
        }

        public Task<bool> IsItemInInventoryAsync(InventoryItemId itemId, CancellationToken cancellationToken)
        {
            return IsItemInInventoryAsyncFunc(itemId);
        }

        public Task<int> RemoveStockAsync(InventoryItemId itemId, int quantity, CustomerOrderActorMessageId amId)
        {
            return RemoveStockAsyncFunc(itemId, quantity, amId);
        }
    }
}