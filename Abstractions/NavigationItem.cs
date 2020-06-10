using System;
using System.Collections.Generic;
using System.Text;

namespace Abstractions
{
    public class NavigationItem
    {
        public string? Title { get; set; }

        public string? RelativeUrl { get; set; }

        public NavigationItem? Parent { get; set; }

        public IEnumerable<NavigationItem>? ChildItems { get; set; }
    }
}
