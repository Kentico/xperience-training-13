using System;

using CMS.BannerManagement;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_BannerManagement_Controls_CategoryEdit : CMSAdminControl
{
    #region "Properties"

    private BannerCategoryInfo TypedEditedObject
    {
        get
        {
            return (BannerCategoryInfo)UIContext.EditedObject;
        }
    }


    /// <summary>
    /// Indicates whether the previous banner category was saved
    /// </summary>
    private bool WasSaved
    {
        get
        {
            if (QueryHelper.GetInteger("saved", 0) == 1)
            {
                return true;
            }
            return false;
        }
    }


    /// <summary>
    /// ID of site of the currently processed tag group
    /// </summary>
    private int? SiteID
    {
        get
        {
            int query = QueryHelper.GetInteger("siteid", 0);

            if ((query == 0) || (query == -4))
            {
                return null;
            }

            return query;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeValidate += EditForm_OnBeforeValidate;
        EditForm.OnCheckPermissions += EditForm_OnCheckPermissions;

        EditForm.RefreshHeader = true;
        EditForm.RedirectUrlAfterCreate = GetEditUrl();

        if (WasSaved)
        {
            ((CMSPage)Page).ShowChangesSaved();
        }
    }

    #endregion


    #region "Private methods"

    private void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        // Set SiteID if creating a new object
        if ((TypedEditedObject == null) || (TypedEditedObject.Generalized.ObjectID == 0))
        {
            EditForm.Data["BannerCategorySiteID"] = SiteID;
        }
    }


    private void EditForm_OnCheckPermissions(object sender, EventArgs e)
    {
        int? siteIDToCheck;

        // Editing an existing banner category
        if ((TypedEditedObject != null) && (TypedEditedObject.BannerCategoryID > 0))
        {
            siteIDToCheck = TypedEditedObject.BannerCategorySiteID;
        }
        // Creating a new one
        else
        {
            siteIDToCheck = SiteID;
        }
        ((CMSBannerManagementPage)Page).CheckModifyPermission(siteIDToCheck);
    }


    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.BannerManagement", "EditBannerCategory");

        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, false), "objectid={%EditedObject.ID%}&siteid={?siteid?}&saved=1");
        }

        return String.Empty;
    }

    #endregion
}
