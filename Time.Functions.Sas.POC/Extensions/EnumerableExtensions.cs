using System;
using System.Collections.Generic;
using System.Linq;

namespace Time.Core.Extensions
{
    public static class EnumerableExtensions
    {
        #region Chunk
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (chunksize > 0)
            {
                return source.Select((value, index) => new { ChunkIndex = index / chunksize, value })
                    .GroupBy(pair => pair.ChunkIndex)
                    .Select(grp => grp.Select(g => g.value));
            }
            else if (chunksize == 0)
            {
                return new List<IEnumerable<T>>() { source };
            }
            else
            {
                throw new InvalidOperationException("Negative chunkSize is invalid");
            }
        }
        #endregion

        #region ToHashSet
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return items == null ? new HashSet<T>() : new HashSet<T>(items);
        } 
        #endregion

        #region Batch
        // Batch partitioning is taken from the following MSDN blog: http://blogs.msdn.com/b/pfxteam/archive/2012/11/16/plinq-and-int32-maxvalue.aspx
        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return YieldBatchElements(enumerator, batchSize - 1);
                }
            }
        }
        #endregion

        #region YieldBatchElements
        private static IEnumerable<T> YieldBatchElements<T>(
            IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (int i = 0; i < batchSize && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        } 
        #endregion
    }
}
