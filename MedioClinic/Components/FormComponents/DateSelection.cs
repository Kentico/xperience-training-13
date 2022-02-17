using Kentico.Forms.Web.Mvc;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedioClinic.Components.FormComponents
{
    public class DateSelection : FormComponent<DateSelectionProperties, DateTime>
    {
        [BindableProperty]
        [DataType(DataType.Date)]
        public DateTime? Value { get; set; }

        public override DateTime GetValue() => Value ?? DateTime.Today;

        public override void SetValue(DateTime value)
        {
            Value = value;
        }
    }
}
