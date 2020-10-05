using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_OnlineMarketing_Controls_Content_MenuAddWidgetVariant : CMSAbstractPortalUserControl
{
    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = MembershipContext.AuthenticatedUser.PreferredUICultureCode;

        // Add MVT variant
        iAddMVTVariant.Text = ResHelper.GetString("mvtvariant.new", culture);
        iAddMVTVariant.Attributes.Add("onclick", "ContextAddWebPartMVTVariant(GetContextMenuParameter('addWidgetVariantMenu'));");

        pnlWebPartMenu.Attributes.Add("onmouseover", "ActivateParentBorder();");
    }

    #endregion
}