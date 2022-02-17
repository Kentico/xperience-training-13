using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedioClinic.Components
{
    public static class ComponentIdentifiers
    {
        private const string Prefix = "MedioClinic.";

        private const string FormComponentPrefix = Prefix + "FormComponent.";

        private const string PageTemplatePrefix = Prefix + "PageTemplate.";

        private const string PageSectionPrefix = Prefix + "Section.";

        private const string WidgetPrefix = Prefix + "Widget.";

        private const string FieldValidationRulePrefix = Prefix + "FieldValidationRule.";

        public const string BasicPageTemplate = PageTemplatePrefix + "Basic";

        public const string EventPageTemplate = PageTemplatePrefix + "Event";

        public const string AirportSelectionFormComponent = FormComponentPrefix + "AirportSelection";

        public const string SingleColumnSection = PageSectionPrefix + "SingleColumn";

        public const string TwoColumnSection = PageSectionPrefix + "TwoColumn";

        public const string TextWidget = WidgetPrefix + "Text";

        public const string ImageWidget = WidgetPrefix + "Image";

        public const string MediaLibrarySelectionFormComponent = FormComponentPrefix + "MediaLibrarySelection";

        public const string SlideshowWidget = WidgetPrefix + "Slideshow";

        public const string ImageValidationRule = FieldValidationRulePrefix + "MediaLibraryImageDimension";

        public const string MediaLibraryUploaderFormComponent = FormComponentPrefix + "MediaLibraryUploader";

        public const string CheckBoxFormComponent = FormComponentPrefix + "CheckBox";

        public const string DateSelectionFormComponent = FormComponentPrefix + "DateSelection";

        public const string FileDownloadWidget = WidgetPrefix + "FileDownload";
    }
}
