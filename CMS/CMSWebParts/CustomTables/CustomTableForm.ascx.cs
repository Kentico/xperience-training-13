using System;

using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.WebAnalytics;

public partial class CMSWebParts_CustomTables_CustomTableForm : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Custom table used for edit item.
    /// </summary>
    public string CustomTable
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomTable"), String.Empty);
        }
        set
        {
            SetValue("CustomTable", value);
        }
    }


    /// <summary>
    /// Key name used to identify edited object.
    /// </summary>
    public string ItemKeyName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemKeyName"), "edititemid");
        }
        set
        {
            SetValue("ItemKeyName", value);
        }
    }


    /// <summary>
    /// Gets or sets the alternative form full name (ClassName.AlternativeFormName).
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), "");
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the WebPart use colon behind label.
    /// </summary>
    public bool UseColonBehindLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseColonBehindLabel"), true);
        }
        set
        {
            SetValue("UseColonBehindLabel", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after validation failed.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidationErrorMessage"), "");
        }
        set
        {
            SetValue("ValidationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
        }
        set
        {
            if (value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        form.OnAfterSave += form_OnAfterSave;
        form.OnBeforeSave += form_OnBeforeSave;
        base.OnLoad(e);
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data for partial caching.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Setups control.
    /// </summary>
    private void SetupControl()
    {
        if (StopProcessing)
        {
            form.StopProcessing = true;
        }
        else
        {
            DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(CustomTable);
            if (customTable == null)
            {
                return;
            }

            form.CustomTableId = customTable.ClassID;
            form.UseColonBehindLabel = UseColonBehindLabel;
            form.ValidationErrorMessage = ValidationErrorMessage;
            form.AlternativeFormFullName = AlternativeFormName;
            form.ItemID = QueryHelper.GetInteger(ItemKeyName, 0);

            if (form.ItemID > 0)
            {
                CheckReadPermissions(customTable);
            }
        }
    }


    private void CheckReadPermissions(DataClassInfo customTable)
    {
        // Check 'Read' permission
        if (customTable.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            return;
        }

        // Show error message
        form.MessagesPlaceHolder.ClearLabels();
        form.ShowError(String.Format(GetString("customtable.permissiondenied.read"), customTable.ClassName));
        form.StopProcessing = true;
    }


    /// <summary>
    /// OnBeforeSave event handler
    /// </summary>
    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        CheckPermissions();
    }


    /// <summary>
    /// OnAfterSave event handler
    /// </summary>
    protected void form_OnAfterSave(object sender, EventArgs e)
    {
        if (form.IsInsertMode)
        {
            // Redirect to edit mode after new item submit
            URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, ItemKeyName, form.ItemID.ToString()));
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Checks create or modify permission.
    /// </summary>
    private void CheckPermissions()
    {
        CustomTableItem ctItem = form.EditedObject;
        // If editing item
        if (ctItem.ItemID > 0)
        {
            // Check 'Modify' permission
            if (!ctItem.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                // Show error message
                form.MessagesPlaceHolder.ClearLabels();
                form.ShowError(String.Format(GetString("customtable.permissiondenied.modify"), ctItem.ClassName));
                form.StopProcessing = true;
            }
        }
        else
        {
            // Check 'Create' permission
            if (!ctItem.CheckPermissions(PermissionsEnum.Create, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                // Show error message
                form.MessagesPlaceHolder.ClearLabels();
                form.ShowError(String.Format(GetString("customtable.permissiondenied.create"), ctItem.ClassName));
                form.StopProcessing = true;
            }
        }
    }
    
    #endregion
}
