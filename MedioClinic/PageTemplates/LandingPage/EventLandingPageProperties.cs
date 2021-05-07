using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Components;
using MedioClinic.Models;
using MedioClinic.Models.FormComponents;

namespace MedioClinic.PageTemplates.LandingPage
{
    public class EventLandingPageProperties : PageTemplateProperties
    {
        [EditingComponent(ComponentIdentifiers.AirportSelectionFormComponent,
            Label = "{$PageTemplate.EventTemplate.LocationAirport$}",
            Order = 0)]
        public string? EventLocationAirport { get; set; }
    }
}
