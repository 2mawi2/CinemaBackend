using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.Model;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace Common.Data
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : IItem
    {
        private readonly string _itemItemDictId;
        private readonly IReliableStateManager _stateManager;

        protected Repository(IReliableStateManager stateManager, string itemItemDictId)
        {
            _stateManager = stateManager;
            _itemItemDictId = itemItemDictId;
        }

        private async Task<IReliableDictionary<ItemId, TEntity>> GetReliableDictionary()
        {
            return await _stateManager.GetOrAddAsync<IReliableDictionary<ItemId, TEntity>>(_itemItemDictId);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(Func<TEntity, bool> predicate)
        {
            var reliableDictionary = await GetReliableDictionary();
            var entities = new BlockingCollection<TEntity>();
            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerator = (await reliableDictionary.CreateEnumerableAsync(tx))
                    .GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    if (predicate(enumerator.Current.Value))
                    {
                        entities.Add(enumerator.Current.Value);
                    }
                }
            }
            return entities;
        }

        public virtual async Task<bool> AddAsync(TEntity item)
        {
            var reliableDictionary = await GetReliableDictionary();
            using (var tx = _stateManager.CreateTransaction())
            {
                await reliableDictionary.AddAsync(tx, item.Id, item);
                await tx.CommitAsync();
            }
            return true;
        }

        public virtual async Task<bool> UpdateAsync(ItemId id, TEntity newValue)
        {
            var reliableDictionary = await GetReliableDictionary();
            using (var tx = _stateManager.CreateTransaction())
            {
                var oldValue = await GetByIdAsync(id);
                await reliableDictionary.TryUpdateAsync(tx, id, newValue, oldValue);
                await tx.CommitAsync();
            }
            return true;
        }

        public virtual async Task<bool> AddAsync(IEnumerable<TEntity> entities)
        {
            var reliableDictionary = await GetReliableDictionary();
            using (var tx = _stateManager.CreateTransaction())
            {
                foreach (var i in entities)
                {
                    await reliableDictionary.AddAsync(tx, i.Id, i);
                }
                await tx.CommitAsync();
            }
            return true;
        }

        public virtual async Task<bool> RemoveAsync(ItemId id)
        {
            var reliableDictionary = await GetReliableDictionary();
            using (var tx = _stateManager.CreateTransaction())
            {
                await reliableDictionary.TryRemoveAsync(tx, id);
                await tx.CommitAsync();
            }
            return true;
        }

        public virtual async Task<TEntity> GetByIdAsync(ItemId id)
        {
            var reliableDictionary = await GetReliableDictionary();
            ConditionalValue<TEntity> item;
            using (var tx = _stateManager.CreateTransaction())
            {
                item = await reliableDictionary.TryGetValueAsync(tx, id);
            }
            return item.HasValue ? item.Value : default(TEntity);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var reliableDictionary = await GetReliableDictionary();
            var entities = new BlockingCollection<TEntity>();
            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerator = (await reliableDictionary.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    entities.Add(enumerator.Current.Value);
                }
            }
            return entities;
        }
    }
}