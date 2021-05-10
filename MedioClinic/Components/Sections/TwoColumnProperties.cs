using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

namespace MedioClinic.Components.Sections
{
    public class TwoColumnProperties : ISectionProperties
    {
        [EditingComponent(
            IntInputComponent.IDENTIFIER,
            DefaultValue = 6,
            Label = "{$" + ComponentIdentifiers.TwoColumnSection + ".LeftColumnWidth.Title$}",
            ExplanationText = "{$" + ComponentIdentifiers.TwoColumnSection + ".LeftColumnWidth.Description$}",
            Order = 0)]
        public int LeftColumnWidth { get; set; }
    }
}
