using System;
using System.Collections.Generic;
using System.Text;

namespace XperienceAdapter
{
    public class BasePageDto
    {
        public virtual IEnumerable<string> SourceColumns => new List<string>
        {
            "DocumentID",
            "DocumentGUID",
            "DocumentName",
            "NodeID",
            "NodeAliasPath"
        };

        public int Id { get; set; }

        public Guid Guid { get; set; }

        public string? Name { get; set; }

        public string? NodeAliasPath { get; set; }
    }
}
