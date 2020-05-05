using System;
using System.Linq;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_PageTitle : CMSAbstractUIWebpart
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        bool displayTitle = UIContext.DisplayTitle;

        // Page title control behaves differently in dialog mode (i.e. uses different help icon)
        ucPageTitle.IsDialog = ValidationHelper.GetBoolean(UIContext["dialog"], false);

        if (TitleText != String.Empty)
        {
            SetTitle(ucPageTitle, TitleText);
        }

        if (!displayTitle)
        {
            ucPageTitle.HideTitle = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (UIContext.DisplayBreadcrumbs)
        {
            SetBreadcrumbs(ucPageTitle);
        }
    }

    #endregion
}
