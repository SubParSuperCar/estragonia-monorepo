using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estragonia;

internal static class AsyncEnumerableHelper
{
    public static IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> enumerable)
    {
        return new EnumerableAsyncWrapper<T>(enumerable);
    }

    private sealed class EnumerableAsyncWrapper<T> : IAsyncEnumerable<T>
    {
        private readonly IEnumerable<T> _enumerable;

        public EnumerableAsyncWrapper(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable;
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new EnumeratorAsyncWrapper<T>(_enumerable.GetEnumerator(), cancellationToken);
        }
    }

    private sealed class EnumeratorAsyncWrapper<T> : IAsyncEnumerator<T>
    {
        private readonly CancellationToken _cancellationToken;

        private readonly IEnumerator<T> _enumerator;

        public EnumeratorAsyncWrapper(IEnumerator<T> enumerator, CancellationToken cancellationToken)
        {
            _enumerator = enumerator;
            _cancellationToken = cancellationToken;
        }

        public T Current
            => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return _cancellationToken.IsCancellationRequested
                ? new ValueTask<bool>(Task.FromCanceled<bool>(_cancellationToken))
                : new ValueTask<bool>(_enumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return default;
        }
    }
}
