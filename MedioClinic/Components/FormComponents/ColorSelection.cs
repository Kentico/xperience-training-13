using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;
using Kentico.Web.Mvc;

using Common.Configuration;

namespace MedioClinic.Components.FormComponents
{
    public class ColorSelection : SelectorFormComponent<ColorSelectionProperties>
    {
        // Retrieves data to be displayed in the selector
        protected override IEnumerable<HtmlOptionItem> GetHtmlOptions()
        {
            var colors = MaterializeCss.Colors.Select(color => new HtmlOptionItem
            {
                Value = color.Key,
                Text = color.Value
            });

            return colors;
        }
    }
}
