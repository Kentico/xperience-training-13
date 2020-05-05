using System;

using CMS.Chat.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[Action(0, ResourceString = "chat.chatsupportcannedresponse.new", TargetUrl = "Edit.aspx?siteid={%SelectedSiteID%}")]

public partial class CMSModules_Chat_Pages_Tools_ChatSupportCannedResponse_List : CMSChatPage
{
    #region "Private fields"

    private int selectedSiteID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If user can view global and classic sites, display site selector
        if (ReadGlobalAllowed)
        {
            if (ReadAllowed)
            {
                CurrentMaster.DisplaySiteSelectorPanel = true;

                if (!RequestHelper.IsPostBack())
                {
                    siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
                }

                // Get site id from site selector
                selectedSiteID = siteOrGlobalSelector.SiteID;

                // Security check: user can select global (-4) this site and global (-5) or current site, if something else was selected, set it back to current site
                if ((selectedSiteID != -4) && (selectedSiteID != -5) && (selectedSiteID != SiteContext.CurrentSiteID))
                {
                    selectedSiteID = SiteContext.CurrentSiteID;
                    siteOrGlobalSelector.SiteID = selectedSiteID;
                }
            }
            else
            {
                selectedSiteID = -4;
            }
        }
        else
        {
            selectedSiteID = SiteContext.CurrentSiteID;
        }
        
        listElem.SiteID = selectedSiteID;

        // Store selected site ID to MacroResolver, so it can be retrieved in ActionAttribute's Apply method (on second pass, which is called on PreRender)
        MacroContext.CurrentResolver.SetNamedSourceData("SelectedSiteID", selectedSiteID);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Disable action and display label if site is set to "this site and global"
        if (selectedSiteID == -5)
        {
            HeaderActions.ActionsList[0].Enabled = false;
            HeaderActions.ActionsList[0].Tooltip = GetString("chat.cannedresponse.chooseglobalorsite");

            FormEngineUserControl label = this.LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
            label.Value = GetString("chat.cannedresponse.chooseglobalorsite");

            HeaderActions.AdditionalControls.Add(label);
            HeaderActions.AdditionalControlsCssClass += "header-actions-label control-group-inline";
            HeaderActions.ReloadAdditionalControls();
        }
        else
        {
            HeaderActions.ActionsList[0].Enabled = HasUserModifyPermission(selectedSiteID);
        }
    }

    #endregion
}