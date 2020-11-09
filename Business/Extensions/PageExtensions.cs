using System;
using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine;

namespace Business.Extensions
{
    public static class PageExtensions
    {
        public static IEnumerable<DayOfWeek> ToDaysOfWeek(this IEnumerable<TreeNode>? pages)
        {
            var daysOfWeek = new List<DayOfWeek>();

            if (pages?.Any() == true)
            {
                foreach (var page in pages)
                {
                    if (Enum.TryParse<DayOfWeek>(page.NodeAlias, out var dayOfWeek))
                    {
                        daysOfWeek.Add(dayOfWeek);
                    }
                } 
            }

            return daysOfWeek;
        }
    }
}
