using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using XperienceAdapter;

namespace Business.Dtos
{
    public class NavigationItemDto : BasePageDto
    {
        public override IEnumerable<string> SourceColumns =>
            base.SourceColumns.Concat(new[] { "NodeParentID" });

        public string? RelativeUrl { get; set; }

        public NavigationItemDto? Parent { get; set; }

        public int? ParentId { get; set; }

        public List<NavigationItemDto> AllParents { get; } = new List<NavigationItemDto>();

        public List<NavigationItemDto> ChildItems { get; } = new List<NavigationItemDto>();
    }
}
