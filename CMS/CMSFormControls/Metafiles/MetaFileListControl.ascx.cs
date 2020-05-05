using System;
using System.ComponentModel;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSFormControls_Metafiles_MetaFileListControl : ReadOnlyFormEngineUserControl
{

    #region "Properties"

    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return fileList.Enabled;
        }
        set
        {
            fileList.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return fileList.IsLiveSite;
        }
        set
        {
            fileList.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets object category.
    /// </summary>
    public string ObjectCategory
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectCategory"), ObjectAttachmentsCategories.THUMBNAIL);
        }
        set
        {
            SetValue("ObjectCategory", value);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event fired before upload processing.
    /// </summary>
    public event CancelEventHandler OnBeforeUpload
    {
        add
        {
            fileList.OnBeforeUpload += value;
        }
        remove
        {
            fileList.OnBeforeUpload -= value;
        }
    }


    /// <summary>
    /// Event fired after upload processing.
    /// </summary>
    public event EventHandler OnAfterUpload
    {
        add
        {
            fileList.OnAfterUpload += value;
        }
        remove
        {
            fileList.OnAfterUpload -= value;
        }
    }


    /// <summary>
    /// Event fired before delete processing.
    /// </summary>
    public event CancelEventHandler OnBeforeDelete
    {
        add
        {
            fileList.OnBeforeDelete += value;
        }
        remove
        {
            fileList.OnBeforeDelete -= value;
        }
    }


    /// <summary>
    /// Event fired after delete processing.
    /// </summary>
    public event EventHandler OnAfterDelete
    {
        add
        {
            fileList.OnAfterDelete += value;
        }
        remove
        {
            fileList.OnAfterDelete -= value;
        }
    }

    #endregion


    #region "lifecycle methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control properties from parent Form
        if (Form != null)
        {
            if (Form.Mode == FormModeEnum.Insert)
            {
                FieldInfo.Visible = false;
                Visible = false;

                return;
            }

            InitializeUploadControl();

            // Set metafile category
            fileList.Category = ObjectCategory;
        }

        CheckFieldEmptiness = false;

    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the upload control.
    /// </summary>
    private void InitializeUploadControl()
    {
        if (Form.EditedObject is BaseInfo)
        {
            BaseInfo info = (BaseInfo)Form.EditedObject;
            fileList.ObjectType = info.TypeInfo.ObjectType;

            if (info.Generalized.ObjectSiteID > 0)
            {
                fileList.SiteID = info.Generalized.ObjectSiteID;
            }

            fileList.ObjectID = info.Generalized.ObjectID;
        }
        else if (Form.EditedObject is IDataClass)
        {
            IDataClass item = (IDataClass)Form.EditedObject;
            fileList.ObjectType = item.ClassName;

            if (!string.IsNullOrEmpty(Form.SiteName))
            {
                fileList.SiteID = SiteInfoProvider.GetSiteID(Form.SiteName);
            }

            fileList.ObjectID = item.ID;
        }
    }

    #endregion


}