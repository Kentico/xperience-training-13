using System;

using CMS.BannerManagement;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_BannerManagement_Controls_BannerEdit : CMSAdminControl
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        EditForm.OnBeforeValidate += EditForm_OnBeforeValidate;
        EditForm.OnCheckPermissions += EditForm_OnCheckPermissions;

        EditForm.RefreshHeader = true;
        EditForm.RedirectUrlAfterCreate = GetEditUrl();
    }

    
    private void EditForm_OnBeforeValidate(object sender, EventArgs e)
    {
        BannerCategoryInfo catInfo = (BannerCategoryInfo)UIContext.EditedObjectParent;

        EditForm.Data["BannerCategoryID"] = catInfo.BannerCategoryID;
        EditForm.Data["BannerSiteID"] = catInfo.BannerCategorySiteID;
    }


    private void EditForm_OnCheckPermissions(object sender, EventArgs e)
    {
        BannerCategoryInfo catInfo = (BannerCategoryInfo)UIContext.EditedObjectParent;
        ((CMSBannerManagementPage)Page).CheckModifyPermission(catInfo.BannerCategorySiteID);
    }
    

    /// <summary>
    /// Creates URL for editing.
    /// </summary>
    private String GetEditUrl()
    {
        UIElementInfo uiChild = UIElementInfoProvider.GetUIElementInfo("CMS.BannerManagement", "EditBanner");
        if (uiChild != null)
        {
            return URLHelper.AppendQuery(UIContextHelper.GetElementUrl(uiChild, UIContext), "objectid={%EditedObject.ID%}&siteid={?siteid?}&parentobjectid={?parentobjectid?}&displaytitle=false&saved=1");
        }

        return String.Empty;
    }
    
    #endregion
}