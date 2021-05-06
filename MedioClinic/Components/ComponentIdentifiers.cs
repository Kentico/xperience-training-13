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

        public const string BasicPageTemplate = Prefix + "BasicPageTemplate";

        public const string EventPageTemplate = Prefix + "EventPageTemplate";

        public const string AirportSelectionFormComponent = FormComponentPrefix + "AirportSelection";
    }
}
