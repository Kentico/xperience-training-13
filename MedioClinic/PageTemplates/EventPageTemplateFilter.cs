using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Components;

namespace MedioClinic.PageTemplates
{
    /// <summary>
    /// Event page template filter.
    /// </summary>
    public class EventPageTemplateFilter : IPageTemplateFilter
    {
        /// <summary>
        /// Allows the event page template to be used only with pages of type EventLandingPage.
        /// </summary>
        /// <param name="pageTemplates"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
        {
            if (context.PageType.Equals(CMS.DocumentEngine.Types.MedioClinic.EventLandingPage.CLASS_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                return pageTemplates.Where(t => GetEventPageTemplates().Contains(t.Identifier));
            }

            return pageTemplates.Where(t => !GetEventPageTemplates().Contains(t.Identifier));
        }

        public IEnumerable<string> GetEventPageTemplates() => new string[] { ComponentIdentifiers.EventPageTemplate };
    }
}
