using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_CustomTables_Controls_CustomTableForm : CMSUserControl
{
    #region "Private variables"

    private string editItemPage = "~/CMSModules/CustomTables/Tools/CustomTable_Data_EditItem.aspx";
    private DataClassInfo dci;
    private string mEditItemPageAdditionalParams;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Custom table class id.
    /// </summary>
    public int CustomTableId
    {
        get
        {
            return customTableForm.CustomTableId;
        }
        set
        {
            customTableForm.CustomTableId = value;
        }
    }


    /// <summary>
    /// Custom table item id.
    /// </summary>
    public int ItemId
    {
        get
        {
            return customTableForm.ItemID;
        }
        set
        {
            customTableForm.ItemID = value;
        }
    }


    /// <summary>
    /// Gets or sets additional parameters for edit page.
    /// </summary>
    public string EditItemPageAdditionalParams
    {
        get
        {
            return mEditItemPageAdditionalParams;
        }
        set
        {
            mEditItemPageAdditionalParams = value;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CustomTableId > 0)
        {
            // Get form info
            dci = DataClassInfoProvider.GetDataClassInfo(CustomTableId);

            if (dci != null)
            {
                customTableForm.OnBeforeDataRetrieval += customTableForm_OnBeforeDataRetrieval;
                customTableForm.OnAfterSave += customTableForm_OnAfterSave;
            }
        }

        // Show message about successful save
        if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
        {
            customTableForm.ShowChangesSaved();
        }
    }


    private void customTableForm_OnBeforeDataRetrieval(object sender, EventArgs e)
    {
        // If editing item
        if (ItemId > 0)
        {
            // Check 'Modify' permission
            if (!dci.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                // Show error message
                customTableForm.MessagesPlaceHolder.ClearLabels();
                customTableForm.ShowError(String.Format(GetString("customtable.permissiondenied.modify"), dci.ClassName));
                customTableForm.StopProcessing = true;
            }
        }
        else
        {
            // Check 'Create' permission
            if (!dci.CheckPermissions(PermissionsEnum.Create, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                // Show error message
                customTableForm.MessagesPlaceHolder.ClearLabels();
                customTableForm.ShowError(String.Format(GetString("customtable.permissiondenied.create"), dci.ClassName));
                customTableForm.StopProcessing = true;
            }
        }
    }


    private void customTableForm_OnAfterSave(object sender, EventArgs e)
    {
        string param = "&saved=1";

        // Include additional parameters if any
        param += (String.IsNullOrEmpty(EditItemPageAdditionalParams) ? String.Empty : "&" + EditItemPageAdditionalParams);

        // Reflect new in query string
        param += (QueryHelper.GetString("new", String.Empty) != String.Empty) ? "&new=1" : String.Empty;

        // Redirect to edit page with saved parameter and new itemId (new item)
        customTableForm.RedirectUrlAfterSave = URLHelper.ResolveUrl(editItemPage) + "?objectid=" + CustomTableId + "&itemid=" + customTableForm.ItemID + param;
    }
}