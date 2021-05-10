using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using MedioClinic.Components;
using MedioClinic.Models;
using MedioClinic.Components.FormComponents;
using MedioClinic.PageTemplates;

[assembly: RegisterPageTemplate(
    ComponentIdentifiers.BasicPageTemplate,
    "{$" + ComponentIdentifiers.BasicPageTemplate + ".Title$}",
    typeof(PageTemplateProperties),
    customViewName: "~/PageTemplates/_BasicTemplate.cshtml",
    Description = "{$" + ComponentIdentifiers.BasicPageTemplate + ".Description$}")]

[assembly: RegisterPageTemplate(
    ComponentIdentifiers.EventPageTemplate,
    "{$" + ComponentIdentifiers.EventPageTemplate + ".Title$}",
    typeof(EventTemplateProperties),
    customViewName: "~/PageTemplates/_EventTemplate.cshtml",
    Description = "{$" + ComponentIdentifiers.EventPageTemplate + ".Description$}")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.AirportSelectionFormComponent,
    typeof(AirportSelection),
    "{$" + ComponentIdentifiers.AirportSelectionFormComponent + ".Title$}",
    IsAvailableInFormBuilderEditor = false,
    ViewName = "~/Components/FormComponents/_AirportSelection.cshtml",
    Description = "{$" + ComponentIdentifiers.AirportSelectionFormComponent + ".Description$}",
    IconClass = "icon-menu")]

[assembly: RegisterSection(
    ComponentIdentifiers.SingleColumnSection,
    "{$" + ComponentIdentifiers.SingleColumnSection + ".Title$}",
    customViewName: "~/Components/Sections/_SingleColumn.cshtml",
    Description = "{$" + ComponentIdentifiers.SingleColumnSection + ".Description$}",
    IconClass = "icon-square")]

[assembly: RegisterSection(
    ComponentIdentifiers.TwoColumnSection,
    "{$" + ComponentIdentifiers.TwoColumnSection + ".Title$}",
    propertiesType: typeof(MedioClinic.Components.Sections.TwoColumnProperties),
    customViewName: "~/Components/Sections/_TwoColumn.cshtml",
    Description = "{$" + ComponentIdentifiers.TwoColumnSection + ".Description$}",
    IconClass = "icon-l-cols-2")]

[assembly: RegisterWidget(
    ComponentIdentifiers.TextWidget,
    "{$" + ComponentIdentifiers.TextWidget + ".Title$}",
    typeof(MedioClinic.Components.Widgets.TextProperties),
    customViewName: "~/Components/Widgets/_Text.cshtml",
    Description = "{$" + ComponentIdentifiers.TextWidget + ".Description$}",
    IconClass = "icon-l-text")]