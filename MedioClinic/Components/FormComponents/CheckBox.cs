using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Kentico.Forms.Web.Mvc;

namespace MedioClinic.Components.FormComponents
{
    /// <summary>
    /// Represents a checkbox form component.
    /// </summary>
    public class CheckBox : FormComponent<CheckBoxProperties, bool>
    {
        /// <summary>
        /// Represents the input value in the resulting HTML.
        /// </summary>
        [BindableProperty]
        public bool Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets name of the <see cref="Value"/> property.
        /// </summary>
        public override string LabelForPropertyName => nameof(Value);

        /// <summary>
        /// Gets the <see cref="Value"/>.
        /// </summary>
        public override bool GetValue() => Value;

        /// <summary>
        /// Sets the <see cref="Value"/>.
        /// </summary>
        public override void SetValue(bool value) => Value = value;

        public override bool CustomAutopostHandling => true;
    }
}
