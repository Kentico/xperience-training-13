using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Components;
using MedioClinic.Models;
using MedioClinic.Components.FormComponents;

namespace MedioClinic.PageTemplates
{
    public class EventTemplateProperties : PageTemplateProperties
    {
        [EditingComponent(ComponentIdentifiers.AirportSelectionFormComponent,
            Label = "{$" + ComponentIdentifiers.AirportSelectionFormComponent + ".LocationAirport$}",
            Order = 0)]
        public string? EventLocationAirport { get; set; }
    }
}
