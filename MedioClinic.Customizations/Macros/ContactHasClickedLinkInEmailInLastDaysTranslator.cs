using CMS.Base;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Core.Internal;
using CMS.DataEngine;
using CMS.Newsletters;

using System;

namespace MedioClinic.Customizations.Macros
{
    public class ContactHasClickedLinkInEmailInLastDaysTranslator : IMacroRuleInstanceTranslator
    {
        public ObjectQuery<ContactInfo> Translate(MacroRuleInstance macroRuleInstance)
        {
            if (macroRuleInstance is null)
            {
                throw new ArgumentNullException(nameof(macroRuleInstance));
            }

            if (!macroRuleInstance.MacroRuleName.Equals(ContactInfoMethods.ContactHasClickedLinkInEmailInLastDaysName, StringComparison.Ordinal))
            {
                throw new ArgumentException($"[{nameof(ContactHasClickedLinkInEmailInLastDaysTranslator)}.{nameof(ContactHasClickedLinkInEmailInLastDaysTranslator.Translate)}]: Only macro rule instances of type {ContactInfoMethods.ContactHasClickedLinkInEmailInLastDaysName} can be translated.");
            }

            var linkStringValue = macroRuleInstance.Parameters["_link"]?.Value;
            var guidParsed = Guid.TryParse(linkStringValue, out var linkGuid);
            var daysStringValue = macroRuleInstance.Parameters["_days"]?.Value;
            var days = daysStringValue?.ToInteger(0);

            if (!guidParsed || !days.HasValue)
            {
                throw new InvalidCastException($"The _link or _days parameters could not be converted from their string representation. Link: {linkStringValue}, days: {daysStringValue}.");
            }

            var dateTimeNow = Service.Resolve<IDateTimeNowService>().GetDateTimeNow();
            var earliestDate = dateTimeNow.AddDays(-days.Value);

            // TODO: Create an index on Newsletter_ClickedLink.ClickedLinkEmail.
            var query = ContactInfo.Provider
                .Get()
                .Source(querySource => querySource.InnerJoin<ClickedLinkInfo>(nameof(ContactInfo.ContactEmail), nameof(ClickedLinkInfo.ClickedLinkEmail)))
                .Source(querySource => querySource.InnerJoin<LinkInfo>(nameof(ClickedLinkInfo.ClickedLinkNewsletterLinkID), LinkInfo.TYPEINFO.IDColumn))
                .WhereEquals(LinkInfo.TYPEINFO.GUIDColumn, linkGuid);

            if (days > 0)
            {
                query = query
                    .WhereGreaterOrEquals(nameof(ClickedLinkInfo.ClickedLinkTime), earliestDate);
            }

            return query
                .Columns(ContactInfo.TYPEINFO.IDColumn, ContactInfo.TYPEINFO.GUIDColumn)
                .Distinct();
        }
    }
}
