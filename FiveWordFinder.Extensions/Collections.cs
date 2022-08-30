namespace FiveWordFinder.Extensions
{
    public static class Collections
    {
        /// <summary>
        /// Compares a collection with a System.Collections.Generic.HashSet returning only the elements that are present in both.
        /// Does not modify either the collection or the hash set being compared.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static IEnumerable<T> IntersectWithHashSet<T>(this IEnumerable<T> first, HashSet<T> second)
        {
            return first.Where(x => second.Contains(x)).ToArray();
        }

        /// <summary>
        /// Compares two collections returning only the elements that are present in both as an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static T[] IntersectWith<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return first.Where(x => second.Contains(x)).ToArray();
        }

        /// <summary>
        /// Creates a new array and copies the values from the stack in the original order of being added to the stack.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <returns></returns>
        public static T[] ToArrayFiFo<T>(this Stack<T> stack)
        {
            var result = stack.ToArray();

            for (int i = 0, j = result.Length - 1; i < result.Length / 2; i++, j--)
            {
                var tmp = result[i];
                result[i] = result[j];
                result[j] = tmp;
            }

            return result;
        }

        /// <summary>
        /// Inserts an item to the collection at the default sorted position.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item">The item to insert into the collection.</param>
        /// <returns></returns>
        public static int InsertSorted<TItem>(this IList<TItem> collection, TItem item) where TItem : IComparable<TItem>
        {
            return InsertSorted(collection, item, (a, b) => a.CompareTo(b));
        }

        /// <summary>
        /// Inserts an item into the collection in sorted position based on the given comparison function.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item">The item to insert into the collection</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static int InsertSorted<TItem>(this IList<TItem> collection, TItem item, Comparison<TItem> comparison)
        {
            int rangeStart = 0;
            int rangeEnd = collection.Count;

            while (rangeEnd > rangeStart)
            {
                int midIndex = rangeStart + (rangeEnd - rangeStart) / 2;
                var compare = comparison(collection[midIndex], item);
                if (compare == 0)
                {
                    rangeStart = midIndex;
                    break;
                }
                else if (compare < 0)
                {
                    rangeStart = midIndex + 1;
                }
                else if (compare > 0)
                {
                    rangeEnd = midIndex;
                }
            }

            collection.Insert(rangeStart, item);
            return rangeStart;
        }
    }
}