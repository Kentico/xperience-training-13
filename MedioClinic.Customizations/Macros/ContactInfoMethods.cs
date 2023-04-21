using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Core.Internal;
using CMS.MacroEngine;
using CMS.Newsletters;

using MedioClinic.Customizations.Helpers;
using MedioClinic.Customizations.Macros;

[assembly: RegisterExtension(typeof(ContactInfoMethods), typeof(ContactInfo))]

namespace MedioClinic.Customizations.Macros
{
    public class ContactInfoMethods : MacroMethodContainer
    {
        public const string ComesFromBigUsCityName = "MedioClinic.ContactComesFromBigUsCity";

        public const string ContactHasClickedLinkInEmailInLastDaysName = "MedioClinic.ContactHasClickedLinkInEmailInLastDays";

        [MacroMethod(typeof(bool), "Returns true if the contact's city is found in the MedioClinic.BigUsCities custom table and if the contact's state is an American one.", 1)]
        [MacroMethodParam(0, "contact", typeof(object), "Contact info object.")]
        public static object ComesFromBigUsCity(EvaluationContext context, params object[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    {
                        if (parameters[0] is ContactInfo)
                        {
                            return ComesFromBigUsCity(parameters[0] as ContactInfo);
                        }
                        else
                        {
                            throw new ArgumentException($"The argument is not of the {typeof(ContactInfo)} type.", "contact");
                        }
                    }
                default:
                    throw new NotSupportedException();
            }
        }

        private static bool ComesFromBigUsCity(ContactInfo contact)
        {
            if (contact != null)
            {
                return CountryHelper.ContactComesFromBigUsCity(contact);
            }

            return false;
        }
    }
}
