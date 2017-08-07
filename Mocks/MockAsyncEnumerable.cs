namespace Mocks
{
    using Microsoft.ServiceFabric.Data;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Simple wrapper for a synchronous IEnumerable of T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MockAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        private IEnumerable<T> enumerable;

        public MockAsyncEnumerable(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new MockAsyncEnumerator<T>(enumerable.GetEnumerator());
        }
    }

    /// <summary>
    /// Simply wrapper for a synchronous IEnumerator of T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class MockAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;

        public MockAsyncEnumerator(IEnumerator<T> enumerator)
        {
            this.enumerator = enumerator;
        }


        public T Current
        {
            get
            {
                return enumerator.Current;
            }
        }

        public void Dispose()
        {
            enumerator.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(enumerator.MoveNext());
        }

        public void Reset()
        {
            enumerator.Reset();
        }
    }

}
