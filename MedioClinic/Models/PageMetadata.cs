using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Content.Web.Mvc;

namespace MedioClinic.Models
{
    public class PageMetadata : IPageMetadata
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Keywords { get; set; }
    }
}
