using System;

using CMS.Base;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_Transformations_Edit : CMSDesignPage
{
    #region "Variables"

    private bool editOnlyCode;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        // Page has been opened from CMSDesk
        editOnlyCode = QueryHelper.GetBoolean("editonlycode", false);
        
        if (!editOnlyCode)
        {
            CheckGlobalAdministrator();
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UserInfo ui = MembershipContext.AuthenticatedUser;

        // If site manager set directly (or window not in dialog mode) -  set site manager flag to unigrid
        // In some cases dialog mode may be used in site manager (hier. transformation)
        bool isSiteManager = QueryHelper.GetBoolean("sitemanager", false);
        if ((isSiteManager || !editOnlyCode) && ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            ucTransf.IsSiteManager = true;
        }

        PageTitle.Visible = true;

        int transformationID = QueryHelper.GetInteger("objectid", 0);
        string selectedTemplate = QueryHelper.GetString("templatetype", String.Empty);
        if (!String.IsNullOrEmpty(ucTransf.SelectedItemType))
        {
            selectedTemplate = ucTransf.SelectedItemType;
        }
        Guid guid = QueryHelper.GetGuid("guid", Guid.Empty);
        bool showInfoLabel = QueryHelper.GetBoolean("showinfo", false);

        TransformationInfo ti = TransformationInfoProvider.GetTransformation(transformationID);

        ucTransf.ShowInfoLabel = showInfoLabel;
        ucTransf.TransInfo = ti;
        ucTransf.HierarchicalID = guid;

        //Set breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("documenttype_edit_transformation_list.titlelist"),
            RedirectUrl = ResolveUrl(String.Format("~/CMSModules/DocumentTypes/Pages/Development/HierarchicalTransformations_Transformations.aspx?objectid={0}&templatetype={1}&editonlycode={2}&tabmode={3}&aliaspath={4}&instanceguid={5}",
                        ti.TransformationID, selectedTemplate, editOnlyCode, QueryHelper.GetInteger("tabmode", 0), QueryHelper.GetString("aliaspath", String.Empty), QueryHelper.GetGuid("instanceguid", Guid.Empty)))
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("documenttype_edit_transformation_list.edit"),
        });
    }

    #endregion
}