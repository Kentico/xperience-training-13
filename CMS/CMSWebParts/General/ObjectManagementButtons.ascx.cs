using System;
using System.Linq;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_General_ObjectManagementButtons : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Custom  edit button caption.
    /// </summary>
    public string EditText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EditText"), String.Empty);
        }
        set
        {
            SetValue("EditText", value);
        }
    }


    /// <summary>
    /// Custom delete button caption.
    /// </summary>
    public string DeleteText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DeleteText"), String.Empty);
        }
        set
        {
            SetValue("DeleteValue", value);
        }
    }


    /// <summary>
    /// Object type
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectType"), String.Empty);
        }
        set
        {
            SetValue("ObjectType", value);
        }
    }


    /// <summary>
    /// Redirect URL to edit control.
    /// </summary>
    public string RedirectUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectUrl"), String.Empty);
        }
        set
        {
            SetValue("RedirectUrl", value);
        }
    }


    /// <summary>
    /// Redirect URL to items listing.
    /// </summary>
    public string RedirectListUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectListUrl"), String.Empty);
        }
        set
        {
            SetValue("RedirectListUrl", value);
        }
    }


    /// <summary>
    /// Query string key name for object identifier.
    /// </summary>
    public string ObjectKeyName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectKeyName"), String.Empty);
        }
        set
        {
            SetValue("ObjectKeyName", value);
        }
    }


    /// <summary>
    /// Gets or sets if buttons visibility is checked by permissions
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), true);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    private void SetupControl()
    {
        int objectID = QueryHelper.GetInteger(ObjectKeyName, 0);
        if (objectID > 0)
        {
            objButtons.ObjectID = objectID;
            objButtons.ObjectType = ObjectType;
            objButtons.RedirectUrl = RedirectUrl;
            objButtons.RedirectListUrl = RedirectListUrl;
            objButtons.EditText = EditText;
            objButtons.ObjectKeyName = ObjectKeyName;
            objButtons.DeleteText = DeleteText;
            objButtons.CheckPermissions = CheckPermissions;
        }
        else
        {
            objButtons.Visible = false;
        }
    }
}
