using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Core.Internal;
using CMS.MacroEngine;
using CMS.Newsletters;

using MedioClinic.Customizations.Macros;

[assembly: RegisterExtension(typeof(ContactInfoMethods), typeof(ContactInfo))]

namespace MedioClinic.Customizations.Macros
{
    public class ContactInfoMethods : MacroMethodContainer
    {
        public const string ComesFromBigUsCityName = "MedioClinic.ContactComesFromBigUsCity";

        public const string ContactHasClickedLinkInEmailInLastDaysName = "MedioClinic.ContactHasClickedLinkInEmailInLastDays";

        [MacroMethod(typeof(bool), "Returns true if the contact clicked a specific link.", 1)]
        [MacroMethodParam(0, "contact", typeof(object), "Contact info object.")]
        [MacroMethodParam(1, "_link", typeof(Guid), "Link GUID.")]
        [MacroMethodParam(2, "_days", typeof(int), "Number of days.")]
        public static object ContactHasClickedLinkInEmailInLastDays(EvaluationContext context, params object[] parameters)
        {
            if (parameters.Length < 3)
            {
                throw new NotSupportedException();
            }

            var contact = parameters[0] as ContactInfo;
            var guidParsed = Guid.TryParse((string)parameters[1], out var linkGuid);
            var days = parameters[2]?.ToInteger(0);

            return guidParsed && days.HasValue ? ContactHasClickedLinkInEmailInLastDays(contact, linkGuid, days.Value) : false;
        }

        private static bool ContactHasClickedLinkInEmailInLastDays(ContactInfo contact, Guid linkGuid, int days)
        {
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            if (linkGuid == Guid.Empty)
            {
                throw new ArgumentException(nameof(linkGuid));
            }

            if (days < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(days));
            }

            var dateTimeNow = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
            var earliestDate = dateTimeNow.AddDays(-days);

            var query = ClickedLinkInfo.Provider
                .Get()
                .Source(querySource => querySource.InnerJoin<LinkInfo>(nameof(ClickedLinkInfo.ClickedLinkNewsletterLinkID), LinkInfo.TYPEINFO.IDColumn))
                .WhereEquals(LinkInfo.TYPEINFO.GUIDColumn, linkGuid)
                .WhereEquals(nameof(ClickedLinkInfo.ClickedLinkEmail), contact.ContactEmail);

            if (days > 0)
            {
                query = query
                    .WhereGreaterOrEquals(nameof(ClickedLinkInfo.ClickedLinkTime), earliestDate);
            }

            return query.TopN(1).Column(ClickedLinkInfo.TYPEINFO.IDColumn).Count > 0;
        }
    }
}
