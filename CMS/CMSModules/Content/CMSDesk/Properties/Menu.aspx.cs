using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Membership;
using CMS.UIControls;
using CMS.DocumentEngine;


[UIElement(ModuleName.CONTENT, "Properties.Menu")]
public partial class CMSModules_Content_CMSDesk_Properties_Menu : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Menu"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.Menu");
        }

        EnableSplitMode = true;

        // Register the scripts
        ScriptHelper.RegisterLoader(Page);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        pnlContent.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    private void ReloadData()
    {
        if (Node == null)
        {
            return;
        }

        if (!Node.IsNavigationItem())
        {
            RedirectToUINotAvailable();
        }

        chkShowInMenu.Checked = Node.DocumentShowInMenu;

        pnlForm.Enabled = DocumentManager.AllowSave;
    }

    #endregion
}
