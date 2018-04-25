using Stylelabs.Integration.Reference.DurableFunctions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stylelabs.Integration.Reference.DurableFunctions.Helpers
{
    public static class CollectionExtensions
    {
        public static IEnumerable<IEnumerable<T>> SplitIntoBatches<T>(this IEnumerable<T> input, int batchSize)
        {
            Guard.NotNull("input", input);
            Guard.GreaterThan("batchSize", batchSize, 0);

            var enumerable = input as T[] ?? input.ToArray();
            int nrOfBatches = (int)Math.Ceiling(enumerable.Count() / (decimal)batchSize);

            for (int i = 0; i < nrOfBatches; i++)
            {
                yield return enumerable.Skip(i * batchSize).Take(batchSize);
            }
        }
    }
}
