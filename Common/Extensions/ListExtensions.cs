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
    }
}