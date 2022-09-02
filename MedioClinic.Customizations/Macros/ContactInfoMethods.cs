﻿using System;

using CMS;
using CMS.ContactManagement;
using CMS.MacroEngine;

using MedioClinic.Customizations.Helpers;
using MedioClinic.Customizations.Macros;

[assembly: RegisterExtension(typeof(ContactInfoMethods), typeof(ContactInfo))]

namespace MedioClinic.Customizations.Macros
{
    public class ContactInfoMethods : MacroMethodContainer
    {
        public const string ComesFromBigUsCityName = "MedioClinic.ContactComesFromBigUsCity";

        [MacroMethod(typeof(bool), "Returns true if the contact's city is found in the MedioClinic.BigUsCities custom table and if the contact's state is an american one.", 1)]
        [MacroMethodParam(0, "contact", typeof(object), "Contact info object.")]
        public static object ComesFromBigUsCity(EvaluationContext context, params object[] parameters)
        {
            switch (parameters.Length)
            {
                case 1:
                    return ComesFromBigUsCity(parameters[0]);
                default:
                    throw new NotSupportedException();
            }
        }

        private static bool ComesFromBigUsCity(object contact)
        {
            var contactInfo = contact as ContactInfo;

            if (contactInfo != null)
            {
                return CountryHelper.ContactComesFromBigUsCity(contactInfo) 
                    && CountryHelper.IsUsState(contactInfo.ContactStateID);
            }

            return false;
        }
    }
}
