using System;
using System.Data;

using CMS.Base;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Pages_Administration_Membership_Membership_Edit_Products : CMSMembershipPage
{
    #region "Variables"

    private int membershipID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query parameters
        membershipID = QueryHelper.GetInteger("membershipid", 0);

        // Get membership
        var mi = MembershipInfo.Provider.Get(membershipID);

        EditedObject = mi;

        // Check permissions
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            if (mi != null)
            {
                if (mi.MembershipSiteID != SiteContext.CurrentSiteID)
                {
                    RedirectToAccessDenied(GetString("general.actiondenied"));
                }
            }
        }

        // Products and product options associated with this membership
        productsUniGridElem.WhereCondition = $"(SKUMembershipGUID = '{mi.MembershipGUID}') AND ((SKUSiteID = '{mi.MembershipSiteID}') OR SKUSiteID IS NULL)";
    }

    #endregion


    #region "Methods"

    protected object productsUniGridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        var row = (DataRowView)parameter;

        switch (sourceName.ToLowerInvariant())
        {
            case "skuprice":
                // Get information needed to format SKU price
                var value = ValidationHelper.GetDecimal(row["SKUPrice"], 0);
                int siteId = ValidationHelper.GetInteger(row["SKUSiteID"], 0);

                // Return formatted SKU price
                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);

            case "skuvalidity":
                // Get information needed to format SKU validity
                ValidityEnum validity = DateTimeHelper.GetValidityEnum(ValidationHelper.GetString(row["SKUValidity"], null));
                int validFor = ValidationHelper.GetInteger(row["SKUValidFor"], 0);
                DateTime validUntil = ValidationHelper.GetDateTime(row["SKUValidUntil"], DateTimeHelper.ZERO_TIME);

                // Return formatted SKU validity
                return DateTimeHelper.GetFormattedValidity(validity, validFor, validUntil);

            case "skuisproductoption":
                var skuInfo = SKUInfo.Provider.Get(ValidationHelper.GetInteger(row["SKUID"], 0));
                if (skuInfo != null && skuInfo.IsProductOption)
                {
                    return GetString("general.yes");
                }

                return GetString("general.no");

            case "skuproductoptioncategoryname":
                var resultCategoryName = "";
                var optionCategoryID = ValidationHelper.GetInteger(row["SKUOptionCategoryID"], 0);

                var optionCategory = OptionCategoryInfo.Provider.Get(optionCategoryID);
                if (optionCategory != null)
                {
                    resultCategoryName = optionCategory.CategoryDisplayName;
                }

                return HTMLHelper.HTMLEncode(resultCategoryName);
        }
        return null;
    }

    #endregion
}