using System.Collections.Generic;

namespace XperienceAdapter.Dtos
{
    public class NavigationItem
    {
        public string? Title { get; set; }

        public string? RelativeUrl { get; set; }

        public NavigationItem? Parent { get; set; }

        public IEnumerable<NavigationItem>? AllParents { get; set; }

        public IEnumerable<NavigationItem>? ChildItems { get; set; }
    }
}
