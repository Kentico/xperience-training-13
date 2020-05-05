using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.BannerManagement;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("banner.bannercategory_list.title")]

[Action(0, ResourceString = "banner.bannercategory_list.newcategory", TargetUrl = "~/CMSModules/BannerManagement/Tools/Category/Category_New.aspx?siteid={%SelectedSiteID%}")]
[UIElement(ModuleName.BANNERMANAGEMENT, "BannerManagement")]
public partial class CMSModules_BannerManagement_Tools_Category_Category_List : CMSBannerManagementPage
{
    #region "Private fields"

    private int selectedSiteID;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the grid view      
        gridBannerManagement.OnAction += gridBannerManagement_OnAction;
        gridBannerManagement.OnExternalDataBound += gridBannerManagement_OnExternalDataBound;


        // If user can view global and local rooms, display site selector
        if (ReadAllowed && ReadGlobalAllowed)
        {
            CurrentMaster.DisplaySiteSelectorPanel = true;

            // Set selected site in SiteSelector only if is not postback (that means if user has returned from another page)
            if (!RequestHelper.IsPostBack())
            {
                siteOrGlobalSelector.SiteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
            }

            // Get site id from site selector
            selectedSiteID = siteOrGlobalSelector.SiteID;

            // Security check: user can select global (-4) this site and global (-5) or current site, if something else was selected, set it back to current site
            if ((selectedSiteID != -4) && (selectedSiteID != -5) && (selectedSiteID != SiteContext.CurrentSiteID))
            {
                selectedSiteID = siteOrGlobalSelector.SiteID = SiteContext.CurrentSiteID;
            }
        }
        else
        {
            if (ReadAllowed)
            {
                selectedSiteID = SiteContext.CurrentSiteID;
            }
            else
            {
                selectedSiteID = -4;
            }
        }

        // -4 is global
        if (selectedSiteID == -4)
        {
            gridBannerManagement.WhereCondition = "(BannerCategorySiteID IS NULL)";
        }
        // Global and this site
        else if (selectedSiteID == -5)
        {
            gridBannerManagement.WhereCondition = string.Format("(BannerCategorySiteID IS NULL OR BannerCategorySiteID = {0})", SiteContext.CurrentSiteID);
        }
        // Single site
        else
        {
            gridBannerManagement.WhereCondition = "(BannerCategorySiteID = " + selectedSiteID + ")";
        }

        // Store selected site ID to MacroResolver, so it can be retrieved in ActionAttribute's Apply method (on second pass, which is called on PreRender)
        MacroContext.CurrentResolver.SetNamedSourceData("SelectedSiteID", selectedSiteID);

        gridBannerManagement.EditActionUrl = GetEditUrl();

        RegisterRefreshUsingPostBackScript();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (selectedSiteID == -5)
        {
            HeaderActions.ActionsList[0].Enabled = false;

            FormEngineUserControl label = LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
            if (label != null)
            {
                label.Value = GetString("banner.choosegloborsite");

                HeaderActions.AdditionalControls.Add(label);
                HeaderActions.AdditionalControlsCssClass += "header-actions-label control-group-inline";
                HeaderActions.ReloadAdditionalControls();
            }
        }
    }

    #endregion


    #region "UniGrid Events"

    private void gridBannerManagement_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int categoryID = ValidationHelper.GetInteger(actionArgument, 0);

                BannerCategoryInfo bannerCategory = BannerCategoryInfoProvider.GetBannerCategoryInfo(categoryID);

                // If category wasn't found 
                if (bannerCategory == null)
                {
                    ShowError(GetString("banner.bannercategory_list.nocategoryid"));

                    return;
                }

                CheckModifyPermission(bannerCategory.BannerCategorySiteID);

                // Delete the class
                bannerCategory.Delete();

                break;
        }
    }


    private object gridBannerManagement_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();

        switch (sourceName)
        {
            case "delete":
                DataRow row = ((DataRowView)((GridViewRow)parameter).DataItem).Row;

                int? siteID = row.IsNull("BannerCategorySiteID") ? (int?)null : ValidationHelper.GetInteger(row["BannerCategorySiteID"], 0);

                CMSGridActionButton button = ((CMSGridActionButton)sender);

                if (!HasUserModifyPermission(siteID))
                {
                    button.Enabled = false;
                }
                
                break;
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    private void RegisterRefreshUsingPostBackScript()
    {
        string script = @"
function RefreshUsingPostBack()
{{
    {0};
}}";
        script = string.Format(script, ControlsHelper.GetPostBackEventReference(btnHiddenPostBackButton, null));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshUsingPostBack", script, true);
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.BannerManagement", "EditBannerCategory");

        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, false), "objectid={0}&siteid={%SelectedSiteID%}");
        }

        return String.Empty;
    }

    #endregion
}