using System;
using System.ComponentModel;
using System.Data;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_FormControls_SelectEproductFiles : FormEngineUserControl
{
    string allowedExtensions = null;

    private int mSKUID = 0;
    private int mSiteID = -1;


    #region "Properties"

    /// <summary>
    /// Gets or sets the e-product SKU ID.
    /// </summary>
    public int SKUID
    {
        get
        {
            return (mSKUID > 0) ? mSKUID : 0;
        }
        set
        {
            mSKUID = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return (mSiteID >= 0) ? mSiteID : SiteContext.CurrentSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets type of ECommerce object (SKU or SKUOption)
    /// </summary>
    public string SKUObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the e-product files count.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();

            if (SKUID == 0)
            {
                return null;
            }

            return fileListElem.FileCount;
        }
        set
        {
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the information and error messages are display by the control itself.
    /// </summary>
    public bool ShowMessages
    {
        get
        {
            return plcMessages.Visible;
        }
        set
        {
            plcMessages.Visible = value;
        }
    }


    /// <summary>
    /// Gets a messages placeholder control.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            if ((Form != null) && (Form.MessagesPlaceHolder != null))
            {
                return Form.MessagesPlaceHolder;
            }
            return plcMessages;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
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
            fileElem.Enabled = value;
            fileListElem.Enabled = value;
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            fileListElem.StopProcessing = true;
            return;
        }

        TryInitByForm();


        if (SiteID == 0)
        {
            allowedExtensions = SettingsKeyInfoProvider.GetValue("CMSUploadExtensions");
        }
        else if (SiteID > 0)
        {
            allowedExtensions = SettingsKeyInfoProvider.GetValue(SiteInfoProvider.GetSiteName(SiteID) + ".CMSUploadExtensions");
        }

        InitUploaders();
    }

    #endregion


    #region "Initialization"

    private void TryInitByForm()
    {
        if (Form == null)
        {
            return;
        }

        if (Form.Data.ContainsColumn("SKUSiteID"))
        {
            SiteID = ValidationHelper.GetInteger(Form.Data.GetValue("SKUSiteID"), 0);
        }

        if (Form.Data.ContainsColumn("SKUID"))
        {
            SKUID = ValidationHelper.GetInteger(Form.Data.GetValue("SKUID"), 0);
        }

        if (Form.AdditionalData.ContainsKey("IsInCompare"))
        {
            Enabled = !ValidationHelper.GetBoolean(Form.AdditionalData["IsInCompare"], true);
        }

        if (Form.AdditionalData.ContainsKey("SKUObjectType"))
        {
            SKUObjectType = ValidationHelper.GetString(Form.AdditionalData["SKUObjectType"], SKUInfo.OBJECT_TYPE_SKU);
        }
    }


    private void InitUploaders()
    {
        // Init the file uploader
        fileElem.ObjectType = SKUObjectType;
        fileElem.Category = ObjectAttachmentsCategories.EPRODUCT;
        fileElem.SiteID = SiteID;
        fileElem.AllowedExtensions = allowedExtensions;

        // Init the file list uploader
        fileListElem.ObjectType = SKUObjectType;
        fileListElem.Category = ObjectAttachmentsCategories.EPRODUCT;
        fileListElem.ObjectID = SKUID;
        fileListElem.SiteID = SiteID;
        fileListElem.AllowedExtensions = allowedExtensions;

        // Enable uploaders if user has sufficient permissions
        fileListElem.Enabled = (ECommerceContext.IsUserAuthorizedToModifySKU(SiteID == 0));

        if (SKUID == 0)
        {
            // Use the file uploader for the new e-product
            plcNewProduct.Visible = true;
            fileListElem.StopProcessing = true;

            if (Form != null)
            {
                Form.OnUploadFile += OnUploadFile;
            }
        }
        else
        {
            // Use the file list uploader for the existing e-product
            plcExistingProduct.Visible = true;
            fileListElem.ReloadData(true);
            fileElem.StopProcessing = true;

            fileListElem.OnAfterUpload += OnAfterUpload;
            fileListElem.OnBeforeDelete += OnBeforeDelete;
        }
    }


    private void OnBeforeDelete(object sender, CancelEventArgs args)
    {
        bool result = ProcessDeleteForExistingEproduct();
        if (result)
        {
            ShowChangesSaved();
        }
        else
        {
            args.Cancel = true;
            ShowError(ErrorMessage);
        }
    }


    private void OnAfterUpload(object sender, EventArgs args)
    {
        int skuFileId = ProcessUploadForExistingEproduct();
        if (skuFileId > 0)
        {
            ShowChangesSaved();
        }
        else
        {
            ShowError(ErrorMessage);
        }
    }


    private void OnUploadFile(object sender, EventArgs args)
    {
        TryInitByForm();

        if ((fileElem.PostedFile != null) && (fileElem.PostedFile.ContentLength > 0))
        {
            int skuFileId = ProcessUploadForNewEproduct();
            if (skuFileId > 0)
            {
                // Update product with number of uploaded files (for new product it is one file)
                SKUInfo newProduct = SKUInfo.Provider.Get(SKUID);
                newProduct.SetValue("SKUEProductFilesCount", 1);
                newProduct.Update();

                ShowChangesSaved();
            }
            else
            {
                ShowError(ErrorMessage);
            }
        }
    }

    #endregion


    #region "Save"

    /// <summary>
    /// Validates the form control and returns true if it is valid, otherwise sets the validation error message and returns false.
    /// </summary>
    public override bool IsValid()
    {
        if (SKUID == 0)
        {
            if (!fileElem.IsValid())
            {
                ValidationError = fileElem.ValidationError;
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Processes a meta file upload for a new e-product.
    /// Returns the ID of the related SKU file object if it was created or updated successfully, otherwise returns 0.
    /// </summary>
    private int ProcessUploadForNewEproduct()
    {
        if (SKUID == 0)
        {
            // SKU does not exist
            return 0;
        }

        // Upload the file
        fileElem.ObjectID = SKUID;
        fileElem.UploadFile();

        MetaFileInfo metaFile = fileElem.CurrentlyHandledMetaFile;
        if (metaFile == null)
        {
            // The file was not uploaded
            return 0;
        }

        // Create a new SKU file
        SKUFileInfo skuFile = new SKUFileInfo()
        {
            FileSKUID = SKUID,
            FileMetaFileGUID = metaFile.MetaFileGUID,
            FileName = metaFile.MetaFileName,
            FilePath = MetaFileInfoProvider.GetMetaFileUrl(metaFile.MetaFileGUID, metaFile.MetaFileName),
            FileType = MediaSourceEnum.MetaFile.ToString()
        };
        SKUFileInfo.Provider.Set(skuFile);
        return skuFile.FileID;
    }


    /// <summary>
    /// Processes a meta file upload for an existing e-product.
    /// Returns the ID of the related SKU file object if it was created or updated successfully, otherwise returns 0.
    /// </summary>
    private int ProcessUploadForExistingEproduct()
    {
        if (SKUID == 0)
        {
            // SKU does not exist
            return 0;
        }

        MetaFileInfo metaFile = fileListElem.CurrentlyHandledMetaFile;
        SKUFileInfo skuFile = null;

        DataSet skuFiles = SKUFileInfo.Provider.Get().WhereEquals("FileMetaFileGUID", metaFile.MetaFileGUID);
        if (DataHelper.DataSourceIsEmpty(skuFiles))
        {
            // Create a new SKU file
            skuFile = new SKUFileInfo()
            {
                FileSKUID = SKUID,
                FileMetaFileGUID = metaFile.MetaFileGUID
            };
        }
        else
        {
            // Get an existing SKU file
            skuFile = new SKUFileInfo(skuFiles.Tables[0].Rows[0]);
        }

        skuFile.FileName = metaFile.MetaFileName;
        skuFile.FilePath = MetaFileInfoProvider.GetMetaFileUrl(metaFile.MetaFileGUID, metaFile.MetaFileName);
        skuFile.FileType = MediaSourceEnum.MetaFile.ToString();

        SKUFileInfo.Provider.Set(skuFile);
        return skuFile.FileID;
    }


    /// <summary>
    /// Checks and processes the dependencies of the meta file and related SKU file that is being deleted.
    /// Returns true if the related SKU file object that is being deleted has no existing dependencies and it can be deleted, otherwise returns false.
    /// </summary>
    private bool ProcessDeleteForExistingEproduct()
    {
        MetaFileInfo metaFile = fileListElem.CurrentlyHandledMetaFile;

        DataSet skuFiles = SKUFileInfo.Provider.Get().WhereEquals("FileMetaFileGUID", metaFile.MetaFileGUID);
        if (!DataHelper.DataSourceIsEmpty(skuFiles))
        {
            SKUFileInfo skuFile = new SKUFileInfo(skuFiles.Tables[0].Rows[0]);
            if (skuFile.Generalized.CheckDependencies())
            {
                ErrorMessage = EcommerceUIHelper.GetDependencyMessage(skuFile);
                return false;
            }

            SKUFileInfo.Provider.Delete(skuFile);
        }

        return true;
    }

    #endregion
}
