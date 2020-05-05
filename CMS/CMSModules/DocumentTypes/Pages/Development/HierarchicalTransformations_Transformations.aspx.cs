using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject("cms.transformation", "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_Transformations : CMSDesignPage
{
    #region "Variables"

    private bool editOnlyCode;
    private TransformationInfo mTransInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Transformation info object
    /// </summary>
    public TransformationInfo TransInfo
    {
        get
        {
            return mTransInfo ?? (mTransInfo = TransformationInfoProvider.GetTransformation(QueryHelper.GetInteger("objectid", 0)) ?? TransformationInfoProvider.GetTransformation(QueryHelper.GetString("name", "")));
        }
        set
        {
            mTransInfo = value;
        }
    }

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
        ucTransf.DialogMode = editOnlyCode;
        UserInfo ui = MembershipContext.AuthenticatedUser;
        ucNewTransf.IsSiteManager = !editOnlyCode && ui.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        PageTitle.Visible = true;

        bool add = QueryHelper.GetBoolean("add", false);
        string templateType = QueryHelper.GetString("templatetype", "all");

        if (!RequestHelper.IsPostBack())
        {
            // Load transformations
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.all"), "all"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.item"), "item"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.alternatingitem"), "alternatingitem"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.firstitem"), "firstitem"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.lastitem"), "lastitem"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.header"), "header"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.footer"), "footer"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.singleitem"), "singleitem"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.separator"), "separator"));
            drpTransformations.Items.Add(new ListItem(GetString("hiertransf.currentitem"), "currentitem"));
        }

        var ti = TransInfo;

        ucNewTransf.TransInfo = ti;
        ucTransf.TransInfo = ti;

        if (!String.IsNullOrEmpty(drpTransformations.SelectedValue))
        {
            templateType = drpTransformations.SelectedValue;
        }
        else
        {
            drpTransformations.SelectedValue = templateType;
        }

        ucTransf.TemplateType = templateType;

        if (!add)
        {
            // Show new transformation link
            CurrentMaster.HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("DocumentType_Edit_Transformation_List.btnAdd"),
                RedirectUrl = ResolveUrl(String.Format(
                        "HierarchicalTransformations_Transformations.aspx?add=true&objectid={0}&templatetype={1}&editonlycode={2}&aliaspath={3}&instanceguid={4}",
                        ti.TransformationID, 
                        templateType, 
                        editOnlyCode,
                        QueryHelper.GetString("aliaspath", String.Empty), 
                        QueryHelper.GetGuid("instanceguid", Guid.Empty)
                ))
            });
        }
        else
        {
            // Set breadcrumbs
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("documenttype_edit_transformation_list.titlelist"),
                RedirectUrl = ResolveUrl(String.Format(
                    "HierarchicalTransformations_Transformations.aspx?objectid={0}&templatetype={1}&editonlycode={2}&aliaspath={3}&instanceguid={4}",
                    ti.TransformationID, 
                    templateType, 
                    editOnlyCode, 
                    QueryHelper.GetString("aliaspath", String.Empty), 
                    QueryHelper.GetGuid("instanceguid", Guid.Empty)
                ))
            });
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("documenttype_edit_transformation_list.btnAdd")
            });

            ucNewTransf.Visible = true;
            ucTransf.Visible = false;

            pnlFilter.Visible = false;

            ucNewTransf.OnSaved += ucNewTransf_OnSaved;
        }

        // Set proper position for templates list
        if (!RequestHelper.IsPostBack())
        {
            if (templateType != String.Empty)
            {
                ucTransf.TemplateType = templateType;
            }
        }
    }


    /// <summary>
    /// Occurs when new hierarchical transformation is saved.
    /// </summary>    
    private void ucNewTransf_OnSaved(object sender, EventArgs e)
    {
        // Redirect to edit page whit param show info true
        URLHelper.Redirect(UrlResolver.ResolveUrl(String.Format(
            "HierarchicalTransformations_Transformations_Edit.aspx?showinfo=true&guid={0}&objectid={1}&templatetype={2}&editonlycode={3}&aliaspath={4}&instanceguid={5}",
            ucNewTransf.HierarchicalID, 
            ucNewTransf.TransInfo.TransformationID, 
            ucTransf.TemplateType, editOnlyCode, 
            QueryHelper.GetString("aliaspath", String.Empty), 
            QueryHelper.GetGuid("instanceguid", Guid.Empty))));
    }

    #endregion
}