using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TinyCsvParser.Mapping;

namespace XperienceAdapter.Generator
{
    internal class NewsletterLinkClickMapping : CsvMapping<NewsletterLinkClick>
    {
        public NewsletterLinkClickMapping() : base()
        {
            MapProperty(0, m => m.NewsletterName);
            MapProperty(1, m => m.IssueDisplayName);
            MapProperty(2, m => m.ClickedLinkEmail);
        }
    }
}
