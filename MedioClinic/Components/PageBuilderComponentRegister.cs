using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Content.Web.Mvc;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.PageBuilder.Web.Mvc.Personalization;

using MedioClinic.Components;
using MedioClinic.Components.FieldValidationRules;
using MedioClinic.Components.FormComponents;
using MedioClinic.Components.InlineEditors;
using MedioClinic.Components.Sections;
using MedioClinic.Components.Widgets;
using MedioClinic.Controllers;
using MedioClinic.Models;
using MedioClinic.PageTemplates;
using MedioClinic.Personalization;

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
    propertiesType: typeof(TwoColumnProperties),
    customViewName: "~/Components/Sections/_TwoColumn.cshtml",
    Description = "{$" + ComponentIdentifiers.TwoColumnSection + ".Description$}",
    IconClass = "icon-l-cols-2")]

[assembly: RegisterWidget(
    ComponentIdentifiers.TextWidget,
    "{$" + ComponentIdentifiers.TextWidget + ".Title$}",
    typeof(TextProperties),
    customViewName: "~/Components/Widgets/_Text.cshtml",
    AllowCache = true,
    Description = "{$" + ComponentIdentifiers.TextWidget + ".Description$}",
    IconClass = "icon-l-text")]

[assembly: RegisterWidget(
    ComponentIdentifiers.ImageWidget,
    typeof(ImageViewComponent),
    "{$" + ComponentIdentifiers.ImageWidget + ".Title$}",
    typeof(ImageProperties),
    AllowCache = true,
    Description = "{$" + ComponentIdentifiers.ImageWidget + ".Description$}",
    IconClass = "icon-picture")]

[assembly: RegisterPageBuilderLocalizationResource(typeof(ImageUploaderResource), "en-US", "es-ES")]

[assembly: RegisterWidget(
    ComponentIdentifiers.SlideshowWidget,
    typeof(SlideshowViewComponent),
    "{$" + ComponentIdentifiers.SlideshowWidget + ".Title$}",
    typeof(SlideshowProperties),
    AllowCache = true,
    Description = "{$" + ComponentIdentifiers.SlideshowWidget + ".Description$}",
    IconClass = "icon-carousel")]

[assembly: RegisterPageBuilderLocalizationResource(typeof(SlideshowEditorResource), "en-US", "es-ES")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.MediaLibrarySelectionFormComponent,
    typeof(MediaLibrarySelection),
    "{$" + ComponentIdentifiers.MediaLibrarySelectionFormComponent + ".Title$}",
    IsAvailableInFormBuilderEditor = false,
    ViewName = "~/Components/FormComponents/_MediaLibrarySelection.cshtml",
    Description = "{$" + ComponentIdentifiers.MediaLibrarySelectionFormComponent + ".Description$}",
    IconClass = "icon-menu")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.CheckBoxFormComponent,
    typeof(CheckBox),
    "{$" + ComponentIdentifiers.CheckBoxFormComponent + ".Title$}",
    IsAvailableInFormBuilderEditor = true,
    ViewName = "~/Components/FormComponents/_CheckBox.cshtml",
    Description = "{$" + ComponentIdentifiers.CheckBoxFormComponent + ".Description$}",
    IconClass = "icon-cb-check-preview")]

[assembly: RegisterFormValidationRule(
    ComponentIdentifiers.ImageValidationRule,
    typeof(MediaLibraryImageDimensionValidationRule),
    "{$" + ComponentIdentifiers.ImageValidationRule + ".Title$}",
    Description = "{$" + ComponentIdentifiers.ImageValidationRule + ".Description$}")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.MediaLibraryUploaderFormComponent,
    typeof(MediaLibraryUploader),
    "{$" + ComponentIdentifiers.MediaLibraryUploaderFormComponent + ".Title$}",
    IsAvailableInFormBuilderEditor = true,
    ViewName = "~/Components/FormComponents/_MediaLibraryUploader.cshtml",
    Description = "{$" + ComponentIdentifiers.MediaLibraryUploaderFormComponent + ".Description$}",
    IconClass = "icon-picture")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.DateSelectionFormComponent,
    typeof(DateSelection),
    "{$" + ComponentIdentifiers.DateSelectionFormComponent + ".Title$}",
    IsAvailableInFormBuilderEditor = true,
    ViewName = "~/Components/FormComponents/_DateSelection.cshtml",
    Description = "{$" + ComponentIdentifiers.DateSelectionFormComponent + ".Description$}",
    IconClass = "icon-calendar")]

[assembly: RegisterPageBuilderLocalizationResource(typeof(PageBuilderSharedResource), "en-US", "es-ES")]

[assembly: RegisterWidget(
    ComponentIdentifiers.FileDownloadWidget,
    typeof(FileDownloadViewComponent),
    "{$" + ComponentIdentifiers.FileDownloadWidget + ".Title$}", 
    typeof(FileDownloadProperties),
    AllowCache = true,
    Description = "File download widget",
    IconClass = "icon-arrow-down-line")]

[assembly: RegisterPersonalizationConditionType(
    ComponentIdentifiers.IsInPersonaCondition, 
    typeof(IsInPersonaConditionType), 
    "{$" + ComponentIdentifiers.IsInPersonaCondition + ".Title$}", 
    ControllerType = typeof(IsInPersonaController), 
    Description = "{$" + ComponentIdentifiers.IsInPersonaCondition + ".Description$}",
    IconClass = "icon-app-personas")]

[assembly: RegisterPersonalizationConditionType(
    ComponentIdentifiers.ComesFromBigUsCityCondition,
    typeof(ComesFromBigUsCityConditionType),
    "{$" + ComponentIdentifiers.ComesFromBigUsCityCondition + ".Title$}",
    ControllerType = typeof(ComesFromBigUsCityController),
    Description = "{$" + ComponentIdentifiers.ComesFromBigUsCityCondition + ".Description$}",
    IconClass = "icon-app-personas")]

[assembly: RegisterFormComponent(
    ComponentIdentifiers.ColorSelectionFormComponent,
    typeof(ColorSelection),
    "{$" + ComponentIdentifiers.ColorSelectionFormComponent + ".Title$}",
    IsAvailableInFormBuilderEditor = false,
    ViewName = "~/Components/FormComponents/_ColorSelection.cshtml",
    Description = "{$" + ComponentIdentifiers.ColorSelectionFormComponent + ".Description$}",
    IconClass = "icon-menu")]

[assembly: RegisterWidget(
    ComponentIdentifiers.ButtonWidget,
    "{$" + ComponentIdentifiers.ButtonWidget + ".Title$}",
    typeof(ButtonProperties),
    customViewName: "~/Components/Widgets/_Button.cshtml",
    AllowCache = true,
    Description = "{$" + ComponentIdentifiers.ButtonWidget + ".Description$}",
    IconClass = "icon-square-dashed-line")]

//[assembly: RegisterFormComponent(
//    ComponentIdentifiers.NewsletterSelectionFormComponent,
//    typeof(NewsletterSelection),
//    "{$" + ComponentIdentifiers.NewsletterSelectionFormComponent + ".Title$}",
//    IsAvailableInFormBuilderEditor = true,
//    ViewName = "~/Components/FormComponents/_NewsletterSelection.cshtml",
//    Description = "{$" + ComponentIdentifiers.NewsletterSelectionFormComponent + ".Description$}",
//    IconClass = "icon-messages")]

//[assembly: RegisterWidget(
//    ComponentIdentifiers.NewsletterSubscriptionWidget,
//    typeof(NewsletterSubscriptionViewComponent),
//    "{$" + ComponentIdentifiers.NewsletterSubscriptionWidget + ".Title$}",
//    typeof(NewsletterSubscriptionProperties),
//    AllowCache = true,
//    Description = "{$" + ComponentIdentifiers.NewsletterSubscriptionWidget + ".Description$}",
//    IconClass = "icon-messages")]