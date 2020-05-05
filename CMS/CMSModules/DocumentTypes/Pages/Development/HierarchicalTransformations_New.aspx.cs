using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("documenttype_edit_transformation_edit.newtransformation")]
public partial class CMSModules_DocumentTypes_Pages_Development_HierarchicalTransformations_New : CMSDesignPage
{
    private bool editOnlyCode;


    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Page has been opened from CMSDesk
        editOnlyCode = QueryHelper.GetBoolean("editonlycode", false);

        if (editOnlyCode)
        {
            plcDocTypeFilter.Visible = true;

            ucDocFilter.ShowCustomTableClasses = false;
        }
        else
        {
            CheckGlobalAdministrator();
        }

        ucTransf.OnSaved += ucTransf_OnSaved;

        if (!editOnlyCode)
        {
            var documentTypeID = QueryHelper.GetInteger("parentobjectid", 0);
            if (documentTypeID > 0)
            {
                var documentType = DataClassInfoProvider.GetDataClassInfo(documentTypeID);
                if (documentType == null)
                {
                    // For situation when object is deleted in another tab
                    RedirectToInformation("editedobject.notexists");
                }
            }

            ucTransf.DocumentTypeID = documentTypeID;

            // Initializes page breadcrumbs
            PageBreadcrumbs.Items.Add(new BreadcrumbItem
            {
                Text = GetString("documenttype_edit_transformation_list.title"),
                RedirectUrl = ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/DocumentType_edit_Transformation_list.aspx?parentobjectid=" + documentTypeID),
            });

            PageBreadcrumbs.Items.Add(new BreadcrumbItem
            {
                Text = GetString("DocumentType_Edit_Transformation_List.btnHierarchicalNew"),
            });

        }
        else
        {
            ucTransf.DocumentTypeID = ucDocFilter.ClassId;
        }
    }


    /// <summary>
    /// Occurs when new transformation is saved.
    /// </summary>
    protected void ucTransf_OnSaved(object sender, EventArgs e)
    {
        //Transfer to transformation list
        var query = String.Format("?objectid={0}&classid={1}", ucTransf.TransInfo.TransformationID, ucTransf.DocumentTypeID);

        var url = UIContextHelper.GetElementUrl("CMS.DocumentEngine", "EditTransformation");

        url = URLHelper.AppendQuery(url, RequestContext.CurrentQueryString);
        url = URLHelper.AppendQuery(url, query);

        if (editOnlyCode)
        {
            // Add hash for dialog URL, dialog=1 is added by CurrentQueryString append.
            url = ApplicationUrlHelper.AppendDialogHash(url);
        }

        URLHelper.Redirect(url);
    }
}