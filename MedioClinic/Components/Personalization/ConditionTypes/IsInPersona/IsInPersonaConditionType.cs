using System;

using CMS.ContactManagement;
using CMS.Personas;
using Kentico.PageBuilder.Web.Mvc.Personalization;

namespace MedioClinic.Personalization
{
    /// <summary>
    /// Personalization condition type based on persona.
    /// </summary>
    public class IsInPersonaConditionType : ConditionType
    {
        private readonly Lazy<PersonaInfo> _persona;

        /// <summary>
        /// Selected persona code name.
        /// </summary>
        public string? PersonaCodeName { get; set; }

        /// <summary>
        /// Automatically populated variant name.
        /// </summary>
        public override string VariantName
        {
            get => _persona.Value.PersonaDisplayName;
            set
            {
                // No need to set automatically generated variant name
            }
        }

        /// <summary>
        /// Creates an empty instance of <see cref="IsInPersonaConditionType"/> class.
        /// </summary>
        public IsInPersonaConditionType()
        {
            _persona = new Lazy<PersonaInfo>(() => PersonaInfo.Provider.Get(PersonaCodeName));
        }

        /// <summary>
        /// Evaluate condition type.
        /// </summary>
        /// <returns>Returns <c>true</c> if implemented condition is met.</returns>
        public override bool Evaluate()
        {
            if (_persona.Value == null)
            {
                return false;
            }

            var contact = ContactManagementContext.GetCurrentContact(false);

            return contact?.ContactPersonaID == _persona.Value.PersonaID;
        }
    }
}