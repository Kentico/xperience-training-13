using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Objects_Controls_RecycleBinMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Main menu       
        string menuId = ContextMenu.MenuID;
        string parentElemId = ContextMenu.ParentElementClientID;

        string actionPattern = "return ContextBinAction_" + parentElemId + "('{0}', GetContextMenuParameter('" + menuId + "'));";
        string confPattern = "if(confirm(" + ScriptHelper.GetLocalizedString("objectversioning.recyclebin.confirmrestore") + ")) {0} return false;";

        pnlRestoreBindings.Attributes.Add("onclick", string.Format(confPattern, string.Format(actionPattern, "restorewithoutbindings")));

        // Display restore to current site only if current site available
        SiteInfo si = SiteContext.CurrentSite;
        if (si != null)
        {
            pnlRestoreCurrent.Attributes.Add("onclick", string.Format(confPattern, string.Format(actionPattern, "restorecurrentsite")));
            lblRestoreCurrent.Text = String.Format(ResHelper.GetString("objectversioning.recyclebin.restoretocurrentsite"), HTMLHelper.HTMLEncode(si.DisplayName));
        }
        else
        {
            pnlRestoreCurrent.Visible = false;
        }
    }
}
