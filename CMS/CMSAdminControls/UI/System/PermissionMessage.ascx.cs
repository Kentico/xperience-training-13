using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_System_PermissionMessage : CMSUserControl
{
    #region "Variables"

    private string mResource;
    private string mPermission;
    private string mErrorMessage;
    private bool? mDisplayMessage = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Resource name.
    /// </summary>
    public string Resource
    {
        get
        {
            return mResource;
        }
        set
        {
            mResource = value;
        }
    }


    /// <summary>
    /// Permission name.
    /// </summary>
    public string Permission
    {
        get
        {
            return mPermission;
        }
        set
        {
            mPermission = value;
        }
    }


    /// <summary>
    /// Error message.
    /// </summary>
    public string ErrorMessage
    {
        get
        {
            return DataHelper.GetNotEmpty(mErrorMessage, GetString("General.PermissionResource"));
        }
        set
        {
            mErrorMessage = value;
        }
    }


    public bool DisplayMessage
    {
        get
        {
            return ValidationHelper.GetBoolean(mDisplayMessage, true);
        }
        set
        {
            mDisplayMessage = value;
        }
    }

    #endregion


    /// <summary>
    /// Display message.
    /// </summary>
    /// <param name="e">Event args</param>
    protected override void OnPreRender(EventArgs e)
    {
        if (mDisplayMessage != null)
        {
            Visible = DisplayMessage;
        }

        if (!String.IsNullOrEmpty(ErrorMessage))
        {
            lblPermission.Text = String.Format(ErrorMessage, Permission, Resource);
        }

        base.OnPreRender(e);
    }
}