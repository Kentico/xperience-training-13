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
using MedioClinic.PageTemplates.LandingPage;

[assembly: RegisterPageTemplate(
    ComponentIdentifiers.BasicPageTemplate,
    "{$PageTemplate.BasicTemplate.Title$}",
    typeof(PageTemplateProperties),
    Description = "{$PageTemplate.BasicTemplate.Description$}")]

[assembly: RegisterPageTemplate(
    ComponentIdentifiers.EventPageTemplate,
    "{$PageTemplate.EventTemplate.Title$}",
    typeof(EventLandingPageProperties),
    Description = "{$PageTemplate.EventTemplate.Description$}")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.AirportSelectionFormComponent,
    typeof(AirportSelectionComponent),
    "{$FormComponent.AirportSelection.Name$}",
    IsAvailableInFormBuilderEditor = false,
    ViewName = "FormComponents/_AirportSelection",
    Description = "{$FormComponent.AirportSelection.Description$}",
    IconClass = "icon-menu")]
