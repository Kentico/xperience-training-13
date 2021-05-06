using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Components;
using MedioClinic.Models;
using MedioClinic.Models.FormComponents;

// TODO: Localize.
[assembly: RegisterPageTemplate(
    ComponentIdentifiers.BasicPageTemplate,
    "Basic page template",
    typeof(PageTemplateProperties),
    Description = "A barebone page template with no special layout.")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.AirportSelectionFormComponent,
    typeof(AirportSelectionComponent),
    "{$FormComponent.AirportSelection.Name$}",
    IsAvailableInFormBuilderEditor = false,
    ViewName = "FormComponents/_AirportSelection",
    Description = "{$FormComponent.AirportSelection.Description$}",
    IconClass = "icon-menu")]
