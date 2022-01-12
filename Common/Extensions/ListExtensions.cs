using System.Collections.Generic;

namespace Common.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Adds a range to a list, if the range is not null.
        /// </summary>
        /// <typeparam name="TItem">List item type.</typeparam>
        /// <param name="list">List to add to.</param>
        /// <param name="rangeToAdd">Range to add.</param>
        public static void AddNonNullRange<TItem>(this List<TItem> list, IEnumerable<TItem> rangeToAdd)
        {
            if (list != null && rangeToAdd != null)
            {
                list.AddRange(rangeToAdd);
            }
        }

        /// <summary>
        /// Adds an item if it is not null.
        /// </summary>
        /// <typeparam name="TItem">List item type.</typeparam>
        /// <param name="list">List to add to.</param>
        /// <param name="item">Item to add.</param>
        public static void AddIfNotNull<TItem>(this List<TItem> list, TItem item)
            where TItem : class
        {
            if (list != null && item != null)
            {
                list.Add(item);
            }
        }
    }
}