using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TinyCsvParser.Mapping;

namespace XperienceAdapter.Generator
{
    internal class NewsletterSubscriberMapping : CsvMapping<NewsletterSubscriber>
    {
        public NewsletterSubscriberMapping() : base() 
        {
            MapProperty(0, m => m.SubscriberEmail);
            MapProperty(1, m => m.NewsletterName);
        }
    }
}
