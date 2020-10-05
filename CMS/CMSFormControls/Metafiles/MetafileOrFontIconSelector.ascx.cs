using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


/// <summary>
/// Form control providing functionality for defining an image by a meta file or a font icon.
/// </summary>
public partial class CMSFormControls_Metafiles_MetafileOrFontIconSelector : FormEngineUserControl
{
    #region "Variables"

    private string mValue;
    private BaseInfo mFormObject;
    private FormFieldInfo mIconCssFieldInfo;

    #endregion


    #region "Public properties"

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
            mValue = ValidationHelper.GetString(value, null);
        }
    }


    /// <summary>
    /// Gets or sets if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            SetEnabled(base.Enabled);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Currently edited object in the form.
    /// </summary>
    private BaseInfo FormObject
    {
        get
        {
            if ((mFormObject == null) && (Form != null))
            {
                mFormObject = Form.Data as BaseInfo;
            }

            return mFormObject;
        }
    }


    /// <summary>
    /// Gets or sets the name of the column which stores the icon CSS class.
    /// </summary>
    private string IconCssFieldName
    {
        get
        {
            return GetValue("IconCssFieldName", String.Empty);
        }
        set
        {
            SetValue("IconCssFieldName", value);
        }
    }


    /// <summary>
    /// Gets the form field info for the field specified by the <see cref="IconCssFieldName"/> property.
    /// </summary>
    private FormFieldInfo IconCssFieldInfo
    {
        get
        {
            if ((mIconCssFieldInfo == null) && (Form != null) && (Form.FormInformation != null))
            {
                mIconCssFieldInfo = Form.FormInformation.GetFormField(IconCssFieldName);
            }

            return mIconCssFieldInfo;
        }
    }


    /// <summary>
    /// Gets or sets the category of the image.
    /// </summary>
    private string Category
    {
        get
        {
            return GetValue("Category", ObjectAttachmentsCategories.THUMBNAIL);
        }
        set
        {
            SetValue("Category", value);
        }
    }


    private ThumbnailTypeEnum ThumbnailType
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<ThumbnailTypeEnum>(drpThumbnailType.SelectedValue);
        }
    }


    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CheckFieldEmptiness = false;

        if (Form != null)
        {
            Form.OnBeforeSave += Form_OnBeforeSave;
            Form.OnAfterSave += Form_OnAfterSave;
        }

        if (!RequestHelper.IsPostBack() && (FormObject != null))
        {
            InitializeThumbnailTypeDropDownList();

            fontIconSelector.Value = FormObject.GetStringValue(IconCssFieldName, null);
            drpThumbnailType.SelectedValue = String.IsNullOrEmpty((string)fontIconSelector.Value) ? ThumbnailTypeEnum.Metafile.ToStringRepresentation() : ThumbnailTypeEnum.CssClass.ToStringRepresentation();
        }

        SetupFileUploaderControl();
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Setup control visibility
        switch (ThumbnailType)
        {
            case ThumbnailTypeEnum.CssClass:
                plcMetaFile.Visible = false;
                plcCssClass.Visible = true;
                break;

            case ThumbnailTypeEnum.Metafile:
                plcMetaFile.Visible = true;
                plcCssClass.Visible = false;
                break;
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the OnBeforeSave event of the Form control.
    /// </summary>
    private void Form_OnBeforeSave(object sender, EventArgs e)
    {
        if (FormObject != null)
        {
            switch (ThumbnailType)
            {
                case ThumbnailTypeEnum.Metafile:
                    // Clear the Icon CSS class field
                    FormObject.SetValue(IconCssFieldName, string.Empty);
                    fontIconSelector.Value = string.Empty;

                    break;

                case ThumbnailTypeEnum.CssClass:
                    // Delete uploaded metafile
                    Guid metaFileguid = ValidationHelper.GetGuid(Value, Guid.Empty);
                    if (metaFileguid != Guid.Empty)
                    {
                        MetaFileInfo metaFile = MetaFileInfoProvider.GetMetaFileInfo(metaFileguid, FormObject.Generalized.ObjectSiteName, true);
                        MetaFileInfo.Provider.Delete(metaFile);
                    }

                    // Delete the metafile thumbnail
                    Value = null;
                    FormObject.SetValue(Field, null);

                    // Update the Icon CSS class field
                    FormObject.SetValue(IconCssFieldName, fontIconSelector.Value);
                    break;
            }
        }
    }


    /// <summary>
    /// Handles the OnAfterSave event of the Form control.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        if (Form.Mode == FormModeEnum.Insert)
        {
            // Update the fileUploader.ObjectID property with the just-created object id
            SetupFileUploaderControl();

            // Upload new metafile
            fileUploader.UploadFile();
        }
    }


    private void InitializeThumbnailTypeDropDownList()
    {
        drpThumbnailType.Items.Add(new ListItem
        {
            Text = "metafileorfonticonselector.thumbnailtype.image",
            Value = ThumbnailTypeEnum.Metafile.ToStringRepresentation()
        });
        drpThumbnailType.Items.Add(new ListItem
        {
            Text = "metafileorfonticonselector.thumbnailtype.iconclass",
            Value = ThumbnailTypeEnum.CssClass.ToStringRepresentation()
        });
    }


    private void SetupFileUploaderControl()
    {
        fileUploader.Category = Category;
        fileUploader.AllowedExtensions = String.Join(";", ImageHelper.ImageExtensions);

        if (Form.EditedObject is BaseInfo)
        {
            BaseInfo info = (BaseInfo)Form.EditedObject;
            fileUploader.ObjectType = info.TypeInfo.ObjectType;

            if (info.Generalized.ObjectSiteID > 0)
            {
                fileUploader.SiteID = info.Generalized.ObjectSiteID;
            }

            fileUploader.ObjectID = info.Generalized.ObjectID;
        }
        else if (Form.EditedObject is IDataClass)
        {
            IDataClass item = (IDataClass)Form.EditedObject;

            fileUploader.ObjectType = item.ClassName;

            if (!string.IsNullOrEmpty(Form.SiteName))
            {
                fileUploader.SiteID = SiteInfoProvider.GetSiteID(Form.SiteName);
            }

            fileUploader.ObjectID = item.ID;
        }
    }


    /// <summary>
    /// Sets if nested controls are enabled.
    /// </summary>
    /// <param name="enabled">If true, nested controls will be enabled.</param>
    private void SetEnabled(bool enabled)
    {
        fileUploader.Enabled = enabled;
        drpThumbnailType.Enabled = enabled;
    }


    /// <summary>
    /// Returns true if entered icon css class is valid. Otherwise sets error message and returns false.
    /// </summary>
    public override bool IsValid()
    {
        // Check the max length of the css class text box field
        if ((ThumbnailType == ThumbnailTypeEnum.CssClass) && (IconCssFieldInfo != null) && (fontIconSelector.Text.Length > IconCssFieldInfo.Size))
        {
            ValidationError = String.Format(ResHelper.GetString("BasicForm.InvalidLength"), IconCssFieldInfo.Size);

            return false;
        }

        return base.IsValid();
    }

    #endregion


    #region "Thumbnail type enum"

    /// <summary>
    /// Thumbnail enumeration - used for distinguishing between thumbnail types
    /// </summary>
    protected enum ThumbnailTypeEnum
    {
        /// <summary>
        /// Thumbnail is defined by a metafile
        /// </summary>
        [EnumDefaultValue]
        [EnumStringRepresentation("metafile")]
        Metafile,

        /// <summary>
        /// Thumbnail is a font icon and is defined by a css class
        /// </summary>
        [EnumStringRepresentation("cssclass")]
        CssClass
    }

    #endregion
}