using System;
using System.Data;
using System.Web;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_FormControls_SelectProductImage : FormEngineUserControl
{
    string allowedExtensions = null;

    private int mSKUID = 0;
    private int mSiteID = -1;


    #region "Properties"

    /// <summary>
    /// Gets or sets the SKU ID.
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
    /// Default value is current site ID.
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
    /// Gets type of SKU object (SKU, SKUOption, SKUVariant)
    /// </summary>
    public string SKUObjectType
    {
        get
        {
            var sku = EditedObject is SKUTreeNode node ? node.SKU : EditedObject as SKUInfo;

            return sku != null ? sku.TypeInfo.ObjectType : SKUInfo.OBJECT_TYPE_SKU;
        }
    }


    /// <summary>
    /// Gets or sets the product image path as a string.
    /// </summary>
    public override object Value
    {
        get
        {
            return MetaFilePath;
        }
        set
        {
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
            metaFileElem.Enabled = value;
        }
    }


    /// <summary>
    /// Gets the image path from the meta file control.
    /// </summary>
    private string MetaFilePath
    {
        get
        {
            string path = null;
            DataSet metaFiles = MetaFileInfoProvider.GetMetaFiles(SKUID, SKUObjectType, ObjectAttachmentsCategories.IMAGE, null, null);

            if (!DataHelper.DataSourceIsEmpty(metaFiles))
            {
                MetaFileInfo metaFile = new MetaFileInfo(metaFiles.Tables[0].Rows[0]);
                path = MetaFileInfoProvider.GetMetaFileUrl(metaFile.MetaFileGUID, metaFile.MetaFileName);
            }

            return path;
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

    #endregion


    #region "Lifecycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        TryInitByForm();

        if (SiteID == 0)
        {
            allowedExtensions = SettingsKeyInfoProvider.GetValue("CMSUploadExtensions");
        }
        else if (SiteID > 0)
        {
            allowedExtensions = SettingsKeyInfoProvider.GetValue(SiteInfoProvider.GetSiteName(SiteID) + ".CMSUploadExtensions");
        }

        InitMetaFileControl();
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
    }


    private void InitMetaFileControl()
    {
        metaFileElem.Visible = true;
        metaFileElem.ObjectID = SKUID;
        metaFileElem.ObjectType = SKUObjectType;
        metaFileElem.Category = ObjectAttachmentsCategories.IMAGE;
        metaFileElem.SiteID = SiteID;
        metaFileElem.AllowedExtensions = allowedExtensions;

        metaFileElem.OnAfterUpload += delegate(object sender, EventArgs args)
        {
            bool result = UpdateProductImagePath();
            if (result)
            {
                ShowChangesSaved();
            }
        };

        metaFileElem.OnAfterDelete += delegate(object sender, EventArgs args)
        {
            bool result = UpdateProductImagePath();
            if (result)
            {
                ShowChangesSaved();
            }
        };

        if (Form != null)
        {
            Form.OnUploadFile += delegate(object sender, EventArgs args)
            {
                TryInitByForm();

                int metaFileId = UploadImageForNewProduct();
                if (metaFileId > 0)
                {
                    UpdateProductImagePath();
                    ShowChangesSaved();
                }
            };
        }
    }

    #endregion


    #region "Save"

    /// <summary>
    /// Validates the form control and returns true if it is valid, otherwise sets the validation error message and returns false.
    /// </summary>
    public override bool IsValid()
    {
        if (!metaFileElem.IsValid())
        {
            ValidationError = metaFileElem.ValidationError;
            return false;
        }

        return true;
    }


    /// <summary>
    /// Uploads the product image meta file for a new product.
    /// Returns the ID of the uploaded meta file if it was uploaded successfully, otherwise returns 0.
    /// </summary>
    private int UploadImageForNewProduct()
    {
        if (SKUID == 0)
        {
            // SKU does not exist
            return 0;
        }

        HttpPostedFile file = metaFileElem.PostedFile;
        if ((file == null) || (file.ContentLength == 0))
        {
            // No file was posted
            return 0;
        }

        metaFileElem.ObjectID = SKUID;
        metaFileElem.UploadFile();

        MetaFileInfo metaFile = metaFileElem.CurrentlyHandledMetaFile;

        return (metaFile == null) ? 0 : metaFile.MetaFileID;
    }


    /// <summary>
    /// Updates the product image path.
    /// Returns true if the path was updated successfully, otherwise returns false.
    /// </summary>
    private bool UpdateProductImagePath()
    {
        var path = ValidationHelper.GetString(Value, null);

        if (Form != null)
        {
            var node = Form.Data as SKUTreeNode;
            if (node != null)
            {
                node.SetValue("SKUImagePath", path);
                node.Update();
                return true;
            }
        }

        SKUInfo sku = SKUInfo.Provider.Get(SKUID);
        if (sku != null)
        {
            sku.SKUImagePath = path;
            SKUInfo.Provider.Set(sku);
            return true;
        }

        return false;
    }

    #endregion
}
