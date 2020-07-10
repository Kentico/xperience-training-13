using System;
using System.Collections.Generic;

namespace XperienceAdapter.Dtos
{
    /// <summary>
    /// Base page DTO.
    /// </summary>
    public class BasePage
    {
        public virtual IEnumerable<string> SourceColumns => new List<string>
        {
            "DocumentID",
            "DocumentGUID",
            "DocumentName",
            "NodeID",
            "NodeAliasPath",
            "NodeParentID"
        };

        public int NodeId { get; set; }

        public Guid Guid { get; set; }

        public string? Name { get; set; }

        public string? NodeAliasPath { get; set; }

        public int? ParentId { get; set; }

        /// <summary>
        /// In the form of RFC 5646 (e.g. "en-US").
        /// </summary>
        public string? Culture { get; set; }

        public IList<PageAttachment> Attachments { get; } = new List<PageAttachment>();
    }

    /// <summary>
    /// Page attachment.
    /// </summary>
    public class PageAttachment
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public string? Title { get; set; }

        public string? FileName { get; set; }

        public string? Extension { get; set; }

        public string? MimeType { get; set; }

        public string? ServerPath { get; set; }
    }
}
