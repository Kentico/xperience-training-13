using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSFormControls_Metafiles_MetaFileUploadControl : FormEngineUserControl
{
    #region "Variables"

    private string mValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return mValue;
        }
        set
        {
            mValue = value?.ToString();
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return fileUploader.IsLiveSite;
        }
        set
        {
            fileUploader.IsLiveSite = value;
        }
    }


    protected override FormEngineUserControl UnderlyingFormControl => fileUploader;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set control properties from parent Form
        if (Form != null)
        {
            Form.OnAfterSave += Form_OnAfterSave;

            InitializeUploadControl();

            // Set metafile category
            fileUploader.Category = ValidationHelper.GetString(GetValue("ObjectCategory"), ObjectAttachmentsCategories.THUMBNAIL);
        }

        // Set style properties of control
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            fileUploader.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            fileUploader.CssClass = CssClass;
            CssClass = null;
        }

        CheckFieldEmptiness = false;
    }


    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        if (Form.Mode == FormModeEnum.Insert)
        {
            InitializeUploadControl();

            // Upload new metafile
            fileUploader.UploadFile();
        }
    }


    private void InitializeUploadControl()
    {
        var info = Form.EditedObject as BaseInfo;
        if (info != null)
        {
            fileUploader.ObjectType = info.TypeInfo.ObjectType;

            if (info.Generalized.ObjectSiteID > 0)
            {
                fileUploader.SiteID = info.Generalized.ObjectSiteID;
            }

            fileUploader.ObjectID = info.Generalized.ObjectID;
        }
        else
        {
            var item = Form.EditedObject as IDataClass;
            if (item != null)
            {
                fileUploader.ObjectType = item.ClassName;

                if (!string.IsNullOrEmpty(Form.SiteName))
                {
                    fileUploader.SiteID = SiteInfoProvider.GetSiteID(Form.SiteName);
                }

                fileUploader.ObjectID = item.ID;
            }
        }
    }

    #endregion
}