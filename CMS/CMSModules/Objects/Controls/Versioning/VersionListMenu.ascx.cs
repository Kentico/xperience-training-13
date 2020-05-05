using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Objects_Controls_Versioning_VersionListMenu : CMSContextMenuControl
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string menuId = ContextMenu.MenuID;
        string parentElemId = ContextMenu.ParentElementClientID;

        var uiContext = UIContextHelper.GetUIContext(this);
        var info = uiContext.EditedObject as BaseInfo;

        if ((info != null) && info.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            pnlRestoreChilds.Attributes.Add("onclick", "if(confirm(" + ScriptHelper.GetLocalizedString("objectversioning.versionlist.confirmfullrollback") + ")) { ContextVersionAction_" + parentElemId + "('fullrollback', GetContextMenuParameter('" + menuId + "'));} return false;");
        }
        else
        {
            pnlRestoreChilds.AddCssClass("ItemDisabled");
        }
    }
}
