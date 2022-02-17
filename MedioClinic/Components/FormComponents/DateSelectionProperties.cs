using CMS.DataEngine;

using Kentico.Forms.Web.Mvc;

using System;

namespace MedioClinic.Components.FormComponents
{
    public class DateSelectionProperties : FormComponentProperties<DateTime>
    {
        public DateSelectionProperties() : base(FieldDataType.DateTime)
        {
        }

        [DefaultValueEditingComponent(ComponentIdentifiers.DateSelectionFormComponent)]
        public override DateTime DefaultValue { get; set; } = DateTime.Today;
    }
}