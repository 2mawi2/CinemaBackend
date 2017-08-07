// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Mocks
{
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Data.Notifications;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    public class MockReliableStateManager : IReliableStateManagerReplica
    {
        public ConcurrentDictionary<Uri, IReliableState> store = new ConcurrentDictionary<Uri, IReliableState>();
        private Func<CancellationToken, Task<bool>> datalossFunction;

        private Dictionary<Type, Type> dependencyMap = new Dictionary<Type, Type>()
        {
            {typeof(IReliableDictionary<,>), typeof(MockReliableDictionary<,>)},
            {typeof(IReliableQueue<>), typeof(MockReliableQueue<>)}
        };

        public Func<CancellationToken, Task<bool>> OnDataLossAsync
        {
            set { datalossFunction = value; }

            get { return datalossFunction; }
        }

        public event EventHandler<NotifyTransactionChangedEventArgs> TransactionChanged;
        public event EventHandler<NotifyStateManagerChangedEventArgs> StateManagerChanged;

        public Task ClearAsync(ITransaction tx)
        {
            store.Clear();
            return Task.FromResult(true);
        }

        public Task ClearAsync()
        {
            store.Clear();
            return Task.FromResult(true);
        }

        public ITransaction CreateTransaction()
        {
            return new MockTransaction();
        }

        public Task RemoveAsync(string name)
        {
            IReliableState result;
            store.TryRemove(ToUri(name), out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(ITransaction tx, string name)
        {
            IReliableState result;
            store.TryRemove(ToUri(name), out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(string name, TimeSpan timeout)
        {
            IReliableState result;
            store.TryRemove(ToUri(name), out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(ITransaction tx, string name, TimeSpan timeout)
        {
            IReliableState result;
            store.TryRemove(ToUri(name), out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(Uri name)
        {
            IReliableState result;
            store.TryRemove(name, out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(Uri name, TimeSpan timeout)
        {
            IReliableState result;
            store.TryRemove(name, out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(ITransaction tx, Uri name)
        {
            IReliableState result;
            store.TryRemove(name, out result);

            return Task.FromResult(true);
        }

        public Task RemoveAsync(ITransaction tx, Uri name, TimeSpan timeout)
        {
            IReliableState result;
            store.TryRemove(name, out result);

            return Task.FromResult(true);
        }

        public Task<ConditionalValue<T>> TryGetAsync<T>(string name) where T : IReliableState
        {
            IReliableState item;
            bool result = store.TryGetValue(ToUri(name), out item);

            return Task.FromResult(new ConditionalValue<T>(result, (T) item));
        }

        public Task<ConditionalValue<T>> TryGetAsync<T>(Uri name) where T : IReliableState
        {
            IReliableState item;
            bool result = store.TryGetValue(name, out item);

            return Task.FromResult(new ConditionalValue<T>(result, (T) item));
        }

        public Task<T> GetOrAddAsync<T>(string name) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(ToUri(name), GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(ITransaction tx, string name) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(ToUri(name), GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(string name, TimeSpan timeout) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(ToUri(name), GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(ITransaction tx, string name, TimeSpan timeout) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(ToUri(name), GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(Uri name) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(name, GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(Uri name, TimeSpan timeout) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(name, GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(ITransaction tx, Uri name) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(name, GetDependency(typeof(T))));
        }

        public Task<T> GetOrAddAsync<T>(ITransaction tx, Uri name, TimeSpan timeout) where T : IReliableState
        {
            return Task.FromResult((T) store.GetOrAdd(name, GetDependency(typeof(T))));
        }

        public bool TryAddStateSerializer<T>(IStateSerializer<T> stateSerializer)
        {
            throw new NotImplementedException();
        }

        public IReliableState GetDependency(Type t)
        {
            Type mockType = dependencyMap[t.GetGenericTypeDefinition()];
            return (IReliableState) Activator.CreateInstance(mockType.MakeGenericType(t.GetGenericArguments()));
        }

        public static Uri ToUri(string name)
        {
            return new Uri("mock://" + name, UriKind.Absolute);
        }

        public IAsyncEnumerator<IReliableState> GetAsyncEnumerator()
        {
            return new MockAsyncEnumerator<IReliableState>(store.Values.GetEnumerator());
        }

        public void Initialize(StatefulServiceInitializationParameters initializationParameters)
        {
        }

        public Task<IReplicator> OpenAsync(ReplicaOpenMode openMode, IStatefulServicePartition partition,
            CancellationToken cancellationToken)
        {
            return null;
        }

        public Task ChangeRoleAsync(ReplicaRole newRole, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        public void Abort()
        {
        }

        public Task BackupAsync(Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException();
        }

        public Task BackupAsync(BackupOption option, TimeSpan timeout, CancellationToken cancellationToken,
            Func<BackupInfo, CancellationToken, Task<bool>> backupCallback)
        {
            throw new NotImplementedException();
        }

        public Task RestoreAsync(string backupFolderPath)
        {
            throw new NotImplementedException();
        }

        public Task RestoreAsync(string backupFolderPath, RestorePolicy restorePolicy,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}