using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI.Internal;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_Ecommerce_Controls_UI_ProductEdit : CMSAdminControl, IPostBackEventHandler
{
    #region "Variables"

    private DataClassInfo mProductClass;
    private bool? mProductIsOrdered;

    private List<BasicForm> mAllSKUForms;
    private List<BasicForm> mSKURepresentingForms;
    private bool mEnabled = true;
    private bool mShowRemoveSKUBindingPanel;
    private bool validationErrorShown;

    private const int AUTOMATIC_TAX = -1;

    #endregion


    #region "Properties - new product"

    /// <summary>
    /// Gets the parent node ID value for the new product.
    /// </summary>
    public int NewProductParentNodeID
    {
        get
        {
            return DocumentManager.ParentNodeID;
        }
    }


    /// <summary>
    /// Gets the data class ID value for the new product.
    /// </summary>
    public int NewProductDataClassID
    {
        get
        {
            return DocumentManager.NewNodeClassID;
        }
    }


    /// <summary>
    /// Gets or sets the parent product option category for the new product option.
    /// </summary>
    public OptionCategoryInfo NewProductOptionCategory
    {
        get;
        set;
    }

    #endregion


    #region "Properties - product"

    /// <summary>
    /// Gets or sets the product to be edited.
    /// </summary>
    public BaseInfo Product
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets edited SKUInfo object. 
    /// </summary>
    public SKUInfo SKU
    {
        get
        {
            if (Product is SKUInfo)
            {
                return Product as SKUInfo;
            }
            if (Product is SKUTreeNode)
            {
                return ((SKUTreeNode)Product).SKU;
            }
            return null;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the edited product is a new product.
    /// </summary>
    private bool ProductIsNew
    {
        get
        {
            return Product.GetIntegerValue("SKUID", 0) == 0;
        }
    }


    /// <summary>
    /// Gets the edited product site ID.
    /// </summary>
    public int ProductSiteID
    {
        get
        {
            return Product.GetIntegerValue("SKUSiteID", 0);
        }
    }


    /// <summary>
    /// Gets the value that indicates if the edited product is a product option.
    /// </summary>
    private bool ProductIsProductOption
    {
        get
        {
            return Product.GetIntegerValue("SKUOptionCategoryID", 0) > 0;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the product is ordered.
    /// </summary>
    private bool ProductIsOrdered
    {
        get
        {
            int productId = Product.GetIntegerValue("SKUID", 0);
            if (productId <= 0)
            {
                return false;
            }

            if (!mProductIsOrdered.HasValue)
            {
                DataSet ds = OrderItemInfo.Provider.Get().TopN(1).WhereEquals("OrderItemSKUID", productId);
                mProductIsOrdered = !DataHelper.DataSourceIsEmpty(ds);
            }

            return (bool)mProductIsOrdered;
        }
    }


    /// <summary>
    /// Gets a value indicating whether product has variants.
    /// </summary>    
    private bool ProductHasVariants
    {
        get
        {
            if (SKU != null)
            {
                return SKU.HasVariants;
            }

            return false;
        }
    }

    #endregion


    #region "Properties - product data class"

    /// <summary>
    /// Gets the data class of the product.
    /// </summary>
    private DataClassInfo ProductClass
    {
        get
        {
            if (mProductClass == null)
            {
                if (NewProductDataClassID > 0)
                {
                    // Get from document manager
                    mProductClass = DocumentManager.NewNodeClass;
                }
                else
                {
                    // Get from DB
                    TreeNode node = Product as TreeNode;
                    if (node != null)
                    {
                        mProductClass = DataClassInfoProvider.GetDataClassInfo(node.ClassName);
                    }
                }
            }

            return mProductClass;
        }
    }

    #endregion


    #region "Properties - forms"

    /// <summary>
    /// Gets or sets the form mode.
    /// </summary>
    public FormModeEnum FormMode
    {
        get;
        set;
    } = FormModeEnum.Update;


    /// <summary>
    /// Gets the SKUBinding form field value.
    /// </summary>
    public FormEngineUserControl SKUBindingForm
    {
        get
        {
            return selectSkuBindingElem;
        }
    }


    /// <summary>
    /// Gets the SKUProductType form field value.
    /// </summary>
    private SKUProductTypeEnum SKURepresenting
    {
        get
        {
            object value = null;
            if (skuGeneralForm.FieldEditingControls != null)
            {
                value = skuGeneralForm.GetFieldValue("SKUProductType");
            }

            return EnumStringRepresentationExtensions.ToEnum<SKUProductTypeEnum>(ValidationHelper.GetString(value, ""));
        }
    }


    /// <summary>
    /// Gets the SKU representing form.
    /// </summary>
    private BasicForm SKURepresentingForm
    {
        get
        {
            switch (SKURepresenting)
            {
                case SKUProductTypeEnum.Membership:
                    return skuMembershipForm;
                case SKUProductTypeEnum.EProduct:
                    return skuEproductForm;
                case SKUProductTypeEnum.Bundle:
                    return skuBundleForm;
                default:
                    return null;
            }
        }
    }


    /// <summary>
    /// Gets the value that indicates if there are any custom SKU fields visible.
    /// </summary>
    private bool HasAnyCustomSKUFields
    {
        get
        {
            return skuCustomForm.IsAnyFieldVisible() && !DoNotCreateSKU && !UseExistingSKU;
        }
    }


    /// <summary>
    /// Gets the value that indicates if there are any document fields visible.
    /// </summary>
    private bool HasAnyDocumentFields
    {
        get
        {
            return documentForm.IsAnyFieldVisible();
        }
    }


    /// <summary>
    /// Gets the value that indicates if there are any custom properties fields visible.
    /// </summary>
    private bool HasAnyCustomPropertiesFields
    {
        get
        {
            return HasAnyCustomSKUFields || HasAnyDocumentFields;
        }
    }


    /// <summary>
    /// Gets the list of all the SKU forms.
    /// </summary>
    private List<BasicForm> SKUForms
    {
        get
        {
            if (mAllSKUForms == null)
            {
                mAllSKUForms = new List<BasicForm>
                {
                    skuGeneralForm,
                    skuMembershipForm,
                    skuEproductForm,
                    skuBundleForm,
                    skuCustomForm,
                    skuOtherForm
                };
            }
            return mAllSKUForms;
        }
    }


    /// <summary>
    /// Gets the list of all the SKU "representing" forms.
    /// </summary>
    public List<BasicForm> SKURepresentingForms
    {
        get
        {
            if (mSKURepresentingForms == null)
            {
                mSKURepresentingForms = new List<BasicForm>
                {
                    skuMembershipForm,
                    skuEproductForm,
                    skuBundleForm
                };
            }
            return mSKURepresentingForms;
        }
    }


    /// <summary>
    /// Gets the list of the SKU forms that are currently in use.
    /// </summary>
    private List<BasicForm> CurrentSKUForms
    {
        get
        {
            List<BasicForm> forms = new List<BasicForm>();
            forms.Add(skuGeneralForm);
            if (SKURepresentingForm != null)
            {
                forms.Add(SKURepresentingForm);
            }
            forms.Add(skuCustomForm);
            forms.Add(skuOtherForm);
            return forms;
        }
    }


    private bool IsGlobalProductSelected
    {
        get
        {
            return string.Equals(selectSkuBindingElem.Binding, CMSModules_Ecommerce_FormControls_SelectSKUBinding.CREATE_NEW_GLOBAL, StringComparison.OrdinalIgnoreCase);
        }
    }

    #endregion


    #region "Properties - general"

    /// <summary>
    /// Gets or sets the value that indicates if the control is displayed when the SKU is to be bound to an existing document.
    /// </summary>
    public bool IsBindSKUAction
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is displayed in CMS Desk - Content.
    /// </summary>
    public bool IsInContent
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is displayed in compare mode on the right side.
    /// </summary>
    public bool IsInCompare
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is in a dialog.
    /// </summary>
    public bool IsInDialog
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            headerActionsElem.Enabled = value;
            pnlCreateSkuBinding.Enabled = value;
            pnlRemoveSkuBinding.Enabled = value;
            SKUForms.ForEach(f => f.Enabled = value);
            documentForm.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            editMenuElem.StopProcessing = value;
            headerActionsElem.StopProcessing = value;
            SKUForms.ForEach(f => f.StopProcessing = value);
            documentForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the "create SKU binding" panel should be visible.
    /// </summary>
    private bool ShowCreateSKUBindingPanel
    {
        get
        {
            return ProductIsNew && !ProductIsProductOption && !IsInDialog &&
                (selectSkuBindingElem.AllowCreateNewGlobal || selectSkuBindingElem.AllowUseExisting || selectSkuBindingElem.AllowDoNotCreate) &&
                (ECommerceSettings.AllowProductsWithoutDocuments(ProductSiteID) || ECommerceSettings.AllowGlobalProducts(ProductSiteID) || AllowDoNoCreateSKU || IsBindSKUAction);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the "remove SKU binding" panel should be visible.
    /// </summary>
    public bool ShowRemoveSKUBindingPanel
    {
        get
        {
            bool show = mShowRemoveSKUBindingPanel;
            show &= !ShowCreateSKUBindingPanel;
            show &= !IsInDialog;
            show &= FormMode != FormModeEnum.InsertNewCultureVersion;

            TreeNode node = Product as TreeNode;
            if (node != null)
            {
                WorkflowInfo wf = node.GetWorkflow();
                show &= wf == null;
            }

            return show;
        }
        set
        {
            mShowRemoveSKUBindingPanel = value;
        }
    }


    /// <summary>
    /// Gets the value that indicates if a new site SKU should be created.
    /// </summary>
    private bool CreateNewSiteSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.CREATE_NEW;
        }
    }


    /// <summary>
    /// Gets the value that indicates if a new global SKU should be created.
    /// </summary>
    private bool CreateNewGlobalSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.CREATE_NEW_GLOBAL;
        }
    }


    /// <summary>
    /// Gets the value that indicates if a new site or global SKU should be created.
    /// </summary>
    private bool CreateNewSKU
    {
        get
        {
            return (CreateNewSiteSKU || CreateNewGlobalSKU);
        }
    }


    /// <summary>
    /// Gets the value that indicates if an existing SKU should be used.
    /// </summary>
    private bool UseExistingSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.USE_EXISTING;
        }
    }


    /// <summary>
    /// Gets the value that indicates if an SKU does not need to be created for the product.
    /// </summary>
    public bool AllowDoNoCreateSKU
    {
        get
        {
            return IsInContent;
        }
    }


    /// <summary>
    /// Gets the value that indicates if the SKU should not be created.
    /// </summary>
    private bool DoNotCreateSKU
    {
        get
        {
            return selectSkuBindingElem.GetValueFromRequest() == CMSModules_Ecommerce_FormControls_SelectSKUBinding.DO_NOT_CREATE;
        }
    }


    /// <summary>
    /// Indicates if apply workflow header action is visible.
    /// </summary>
    public bool ShowApplyWorkflow
    {
        get
        {
            return editMenuElem.ShowApplyWorkflow;
        }
        set
        {
            editMenuElem.ShowApplyWorkflow = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs after the product was saved successfully.
    /// </summary>
    public event EventHandler ProductSaved;


    /// <summary>
    /// Raises the ProductSaved event.
    /// </summary>
    protected void RaiseProductSaved()
    {
        ProductSaved?.Invoke(this, EventArgs.Empty);

        // For product options of type other than Products, header refresh is not needed
        if (ProductIsProductOption)
        {
            int optionCategoryId = Product.GetIntegerValue("SKUOptionCategoryID", 0);
            OptionCategoryInfo categoryInfo = OptionCategoryInfo.Provider.Get(optionCategoryId);
            if (categoryInfo.CategoryType != OptionCategoryTypeEnum.Products)
            {
                return;
            }
        }

        // Refresh breadcrumbs after edit
        ScriptHelper.RefreshTabHeader(Page, Product?.Generalized.ObjectDisplayName);
    }


    /// <summary>
    /// Gets a value that indicates if a new product should be created after the currently edited product is saved.
    /// </summary>
    public bool ProductSavedCreateAnother
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets a value that indicates if the SKU binding was removed from the product on save.
    /// </summary>
    public bool ProductSavedSkuBindingRemoved
    {
        get;
        private set;
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (StopProcessing)
        {
            return;
        }

        IsInContent = QueryHelper.GetBoolean("content", false);
        IsInCompare = QueryHelper.GetBoolean("compare", false);
        IsInDialog = QueryHelper.GetBoolean("dialog", false);

        string action = QueryHelper.GetString("action", string.Empty).ToLowerInvariant();
        switch (action)
        {
            case "newculture":
                FormMode = FormModeEnum.InsertNewCultureVersion;
                break;

            case "bindsku":
                IsBindSKUAction = true;
                break;
        }

        int productId = QueryHelper.GetInteger("productId", 0);
        Product = SKUInfo.Provider.Get(productId);

        int newProductOptionCategoryId = QueryHelper.GetInteger("categoryId", 0);
        NewProductOptionCategory = OptionCategoryInfo.Provider.Get(newProductOptionCategoryId);

        selectSkuBindingElem.Changed += selectSkuBindingElem_Changed;

        InitDocumentManager();
        InitProduct();


        if (DocumentManager.Mode == FormModeEnum.Insert)
        {
            int classId = QueryHelper.GetInteger("classId", 0);
            var ci = DataClassInfoProvider.GetDataClassInfo(classId);

            if (ci != null && ci.ClassUsesPageBuilder)
            {
                var templateIdentifier = QueryHelper.GetString("templateidentifier", string.Empty);
                var noTemplateFlag = QueryHelper.GetBoolean("noTemplate", false);

                if (!noTemplateFlag && string.IsNullOrEmpty(templateIdentifier))
                {
                    URLHelper.Redirect("~/CMSModules/Content/CMSDesk/MVC/TemplateSelection.aspx" + RequestContext.CurrentQueryString);
                }
            }
        }

        if (Product != null)
        {
            InitSkuForms();
            InitDocumentForm();

            InitHeaderActions();
            InitCreateSkuBinding();
            InitRemoveSkuBinding();

            InitPage();

            if (ProductIsProductOption)
            {
                anchorDropup.Visible = false;
                pnlForms.RemoveCssClass("ProductEditForms");
                pnlForms.AddCssClass("ProductEditFormsOptionCategory");
            }
        }
        else
        {
            // Stop further processing and hide navigation
            StopProcessing = true;
            anchorDropup.Visible = false;

            ShowError(GetString("com.products.notaproducttype"));
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        SetSelectorVisibility("SKUBrandID");
        SetSelectorVisibility("SKUCollectionID");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            return;
        }

        // Set product defaults
        var defaultsSet = ValidationHelper.GetBoolean(ViewState["SKUDefaultsSet"], false);
        var previousRepresenting = ValidationHelper.GetString(ViewState["SKURepresenting"], "");

        if (!skuGeneralForm.StopProcessing)
        {
            object value = skuGeneralForm.GetFieldValue("SKUProductType");
            string actualSKURepresenting = ValidationHelper.GetString(value, "");
            // default values are set if product is new and defaults has not been set, or representing has changed
            if (ProductIsNew && (!defaultsSet || !previousRepresenting.Equals(actualSKURepresenting, StringComparison.InvariantCulture)))
            {
                SetFieldDefaults();
                ViewState["SKUDefaultsSet"] = true;
                ViewState["SKURepresenting"] = actualSKURepresenting;
            }
        }

        // Set forms visibility
        SKURepresentingForms.ForEach(f => f.Visible = f == SKURepresentingForm);
        CurrentSKUForms.ForEach(f => f.Visible = CreateNewSKU);

        // Disable other forms when editing other category types
        if (NewProductOptionCategory != null)
        {
            switch (NewProductOptionCategory.CategoryType)
            {
                case OptionCategoryTypeEnum.Products:
                    skuOtherForm.Visible = true;
                    break;

                case OptionCategoryTypeEnum.Attribute:
                    skuOtherForm.Visible = false;
                    break;

                case OptionCategoryTypeEnum.Text:
                    skuOtherForm.Visible = false;
                    break;
            }
        }

        InitCustomProperties();

        if (Product is TreeNode)
        {
            Enabled = DocumentManager.AllowSave;
            pnlFormsInner.CssClass = ProductClass.ClassName.ToLowerInvariant().Replace('.', '_');
            btnRemoveSkuBinding.OnClientClick = Enabled ? $"return confirm({ScriptHelper.GetLocalizedString("com.skubinding.unbindconfirm")});" : null;
        }

        EnsureProductAvailableItemsCount();

        // Set enabled value for the product type field
        ActOnField(SKUForms, "SKUProductType", f => f.Enabled &= !(ProductIsOrdered || IsInCompare || ProductHasVariants));

        ApplyCompare();

        // Show warning if the main currency is not defined
        string currencyWarning = EcommerceUIHelper.CheckMainCurrency(ProductSiteID);
        if (!string.IsNullOrEmpty(currencyWarning))
        {
            ShowWarning(currencyWarning);
        }
    }

    #endregion


    #region "Initialization"

    /// <summary>
    /// Initializes the document manager.
    /// </summary>
    private void InitDocumentManager()
    {
        int parentNodeId = QueryHelper.GetInteger("parentNodeId", 0);
        string parentCulture = QueryHelper.GetString("parentCulture", null);
        int classId = QueryHelper.GetInteger("classId", 0);
        int sourceDocumentId = QueryHelper.GetInteger("sourceDocumentId", 0);

        DocumentManager.ParentNodeID = parentNodeId;
        DocumentManager.NewNodeCultureCode = parentCulture;
        DocumentManager.NewNodeClassID = classId;
        DocumentManager.LocalMessagesPlaceHolder = MessagesPlaceHolder;
        DocumentManager.Mode = FormMode;

        // Copy content from another language
        if (sourceDocumentId > 0)
        {
            DocumentManager.SourceDocumentID = sourceDocumentId;
        }

        if ((FormMode == FormModeEnum.Insert) && ((parentNodeId <= 0) || (classId <= 0)))
        {
            DocumentManager.Mode = FormModeEnum.Update;
        }

        DocumentManager.OnValidateData += (sender, args) =>
        {
            args.IsValid = (Product != null) && IsValid();
        };

        DocumentManager.OnSaveData += (sender, args) =>
        {
            if (CreateNewSKU)
            {
                CurrentSKUForms.ForEach(f => f.RaiseOnUploadFile(this, EventArgs.Empty));
            }

            ProductSavedCreateAnother = DocumentManager.CreateAnother;
            RaiseProductSaved();
        };

        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        DocumentManager.OnBeforeAction += TemplateSelectioUtils.SetTemplateForNewPage;
    }


    /// <summary>
    /// Handles the OnAfterAction event of the DocumentManager control.
    /// </summary>
    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        if (e.ActionName == DocumentComponentEvents.UNDO_CHECKOUT)
        {
            // Reload values in the forms
            foreach (var form in SKUForms)
            {
                form.LoadControlValues();
            }
            documentForm.LoadControlValues();
        }
    }


    /// <summary>
    /// Initializes the edited product object.
    /// </summary>
    private void InitProduct()
    {
        if (Product != null)
        {
            // Set edited object
            EditedObject = Product;

            return;
        }

        var newSku = new SKUInfo
        {
            SKUEnabled = true
        };

        // Set SKU option category
        if (NewProductOptionCategory != null)
        {
            newSku.SKUOptionCategoryID = NewProductOptionCategory.CategoryID;
        }

        bool isNewSku = false;

        if (DocumentManager.NodeID > 0)
        {
            // Get the existing node
            var node = DocumentManager.Node as SKUTreeNode;
            if ((node != null) && !node.HasSKU && !UseExistingSKU)
            {
                node.SKU = newSku;
                isNewSku = true;
            }

            Product = node;
        }
        else if ((NewProductParentNodeID > 0) && (NewProductDataClassID > 0))
        {
            // Create a new node if all required information are specified
            var node = DocumentManager.Node as SKUTreeNode;
            if ((node != null) && !DoNotCreateSKU && !UseExistingSKU)
            {
                node.SKU = newSku;
                isNewSku = true;
            }

            Product = node;
        }
        else
        {
            // Create a new SKU
            Product = newSku;
            isNewSku = true;
        }

        // Set edited object
        EditedObject = Product;

        if (isNewSku)
        {
            // Set SKU site ID
            if (NewProductOptionCategory != null)
            {
                if (NewProductOptionCategory.CategorySiteID > 0)
                {
                    Product.SetValue("SKUSiteID", NewProductOptionCategory.CategorySiteID);
                }
                else
                {
                    Product.SetValue("SKUSiteID", null);
                }
            }
            else if (CreateNewSiteSKU)
            {
                Product.SetValue("SKUSiteID", SiteContext.CurrentSiteID);
            }
            else if (CreateNewGlobalSKU)
            {
                Product.SetValue("SKUSiteID", null);
            }
        }
    }


    /// <summary>
    /// Sets the field defaults.
    /// </summary>
    private void SetFieldDefaults()
    {
        // Set default NeedsShipping flag value according to product representation
        var needsShipping = SKUInfoProvider.GetDefaultNeedsShippingFlag(SKURepresenting);
        SetFieldValue(SKUForms, "SKUNeedsShipping", needsShipping);
    }


    /// <summary>
    /// Initializes the "create SKU binding" panel.
    /// </summary>
    private void InitCreateSkuBinding()
    {
        // Check if user is authorized to create global products
        selectSkuBindingElem.AllowCreateNewGlobal = ECommerceContext.IsUserAuthorizedToModifySKU(true);
        selectSkuBindingElem.AllowUseExisting = Product is TreeNode;
        selectSkuBindingElem.AllowDoNotCreate = AllowDoNoCreateSKU;
        pnlCreateSkuBinding.Visible = ShowCreateSKUBindingPanel;
    }


    /// <summary>
    /// Initializes the "remove SKU binding" panel.
    /// </summary>
    private void InitRemoveSkuBinding()
    {
        pnlRemoveSkuBinding.Visible = ShowRemoveSKUBindingPanel;
        btnRemoveSkuBinding.OnClientClick = Enabled ? $"return confirm({ScriptHelper.GetLocalizedString("com.skubinding.unbindconfirm")});" : null;
        btnRemoveSkuBinding.Click += (sender, args) =>
        {
            var node = Product as SKUTreeNode;
            if (node != null)
            {
                node.SKU = null;
                node.Update();

                ProductSavedSkuBindingRemoved = true;
                RaiseProductSaved();
                ShowChangesSaved();
            }
        };
    }


    /// <summary>
    /// Initializes the SKU forms.
    /// </summary>
    private void InitSkuForms()
    {
        // Initialize SKU forms
        skuGeneralForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateGeneral", true);
        skuMembershipForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateMembership", true);
        skuEproductForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateEproduct", true);
        skuBundleForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateBundle", true);
        skuCustomForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateCustom", true);
        skuOtherForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateOther", true);

        // Save bundle binding after SKU is saved for new products
        if (ProductIsNew)
        {
            skuBundleForm.OnUploadFile += (sender, args) => skuBundleForm.SaveData(null);
        }

        // Initialize General SKU form
        if (NewProductOptionCategory != null)
        {
            switch (NewProductOptionCategory.CategoryType)
            {
                case OptionCategoryTypeEnum.Products:
                    skuGeneralForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateProductOptionGeneral", true);
                    skuOtherForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateProductOptionOther", true);
                    InitPriceField();
                    break;

                case OptionCategoryTypeEnum.Attribute:
                    skuGeneralForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateAttributeOptionGeneral", true);
                    InitPriceField();
                    break;

                case OptionCategoryTypeEnum.Text:
                    skuGeneralForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateTextOptionGeneral", true);
                    InitPriceField();
                    break;

                default:
                    skuGeneralForm.FormInformation = FormHelper.GetFormInfo(SKUInfo.OBJECT_TYPE_SKU + ".UpdateGeneral", true);
                    break;
            }
        }

        bool createEmptyDocument = (FormMode == FormModeEnum.InsertNewCultureVersion) && (DocumentManager.SourceDocumentID == 0);

        SKUForms.ForEach(f =>
        {
            if (UseExistingSKU || DoNotCreateSKU)
            {
                f.StopProcessing = true;

                return;
            }

            // Insert SKUInfo if form is SKU representing
            f.Data = SKURepresentingForms.Contains(f) && (SKU != null) ? SKU : Product;
            f.EditedObject = SKURepresentingForms.Contains(f) && (SKU != null) ? SKU : Product;
            f.Mode = FormMode;
            f.MarkRequiredFields = true;
            f.SubmitButton.Visible = false;
            f.MessagesPlaceHolder = MessagesPlaceHolder;

            // Set additional form data
            f.AdditionalData["DataClassID"] = NewProductDataClassID;
            f.AdditionalData["IsInCompare"] = IsInCompare;
            f.AdditionalData["SKUObjectType"] = ProductIsProductOption ? SKUInfo.OBJECT_TYPE_OPTIONSKU : SKUInfo.OBJECT_TYPE_SKU;

            // Set additional form macro data
            f.ContextResolver.SetNamedSourceData("ProductSiteID", ProductSiteID);

            if (Product is TreeNode)
            {
                f.OnAfterDataLoad += (sender, args) =>
                {
                    TreeNode productNode = Product as TreeNode;
                    BasicForm form = sender as BasicForm;

                    // Change image field label when product id under workflow and using metafiles for product images
                    if ((productNode != null) && (form != null) && (productNode.DocumentWorkflowStepID > 0) && form.FieldLabels.Contains("skuimagepath"))
                    {
                        form.FieldLabels["skuimagepath"].ResourceString = "com.sku.imagepathnotversioned";
                    }
                };
            }

            // Create empty document
            if (createEmptyDocument && !RequestHelper.IsPostBack())
            {
                f.OnAfterDataLoad += (sender, args) =>
                {
                    // Clear the paired document fields
                    SetFieldValue(CurrentSKUForms, "SKUName", null);
                    SetFieldValue(CurrentSKUForms, "SKUShortDescription", null);
                    SetFieldValue(CurrentSKUForms, "SKUDescription", null);
                };
            }

            f.OnItemValidation += (object sender, ref string errorMessage) =>
            {
                FormEngineUserControl control = sender as FormEngineUserControl;
                if (control != null)
                {
                    errorMessage = ValidateSkuField(control);
                }
            };
        });

        skuGeneralForm.OnBeforeSave += (sender, args) =>
        {
            // Assign department default tax class if '(automatic)' tax class is selected
            var taxID = ValidationHelper.GetInteger(skuGeneralForm.GetFieldValue("SKUTaxClassID"), 0);

            if (taxID == AUTOMATIC_TAX)
            {
                var departmentID = ValidationHelper.GetInteger(skuGeneralForm.GetFieldValue("SKUDepartmentID"), 0);
                var defaultTaxClassID = DepartmentInfo.Provider.Get(departmentID)?.DepartmentDefaultTaxClassID;

                if (defaultTaxClassID > 0)
                {
                    SKU.SKUTaxClassID = defaultTaxClassID.Value;
                }
            }
        };

        skuGeneralForm.OnGetControlValue += (sender, args) =>
        {
            if (args.ColumnName.Equals("SKUBrandID", StringComparison.OrdinalIgnoreCase) || args.ColumnName.Equals("SKUCollectionID", StringComparison.OrdinalIgnoreCase))
            {
                var value = ValidationHelper.GetInteger(args.Value, 0);
                if (value == 0)
                {
                    // Reset value if "(none)" option with value "0" was selected
                    args.Value = null;
                }
            }
        };
    }


    /// <summary>
    /// Applies compare mode to the forms.
    /// </summary>
    private void ApplyCompare()
    {
        if (IsInCompare)
        {
            // Disable non-text fields
            foreach (string field in skuGeneralForm.Fields)
            {
                // Skip text fields which can be localized
                switch (field.ToLowerInvariant())
                {
                    case "skuname":
                    case "skudescription":
                    case "skushortdescription":
                        continue;
                }

                // Disable the non-localized field
                FormEngineUserControl fc = skuGeneralForm.FieldControls[field];
                if (fc != null)
                {
                    fc.Enabled = false;
                }
            }

            SetFormsCompare(skuMembershipForm, skuEproductForm, skuBundleForm, skuCustomForm, skuOtherForm);
        }
    }


    /// <summary>
    /// Sets the compare mode on the given list of forms.
    /// </summary>
    protected void SetFormsCompare(params BasicForm[] forms)
    {
        // Apply to all forms
        foreach (BasicForm form in forms)
        {
            form.Enabled = false;
            form.ToolTip = GetString("compare.notlocalizable");
        }
    }


    /// <summary>
    /// Initializes the document form.
    /// </summary>
    private void InitDocumentForm()
    {
        if (!(Product is TreeNode))
        {
            documentForm.StopProcessing = true;
            return;
        }

        documentForm.OnBeforeDataLoad += (sender, args) => HideRedundantDocumentFields();

        documentForm.MessagesPlaceHolder = MessagesPlaceHolder;

        documentForm.OnBeforeValidate += (sender, args) =>
        {
            var isValid = true;
            // Validate the visible SKU forms that are on the page above the document form to ensure user friendly order of validation error messages
            foreach (var form in CurrentSKUForms.Except(new[] { skuOtherForm }).Where(f => f.Visible))
            {
                form.ShowValidationErrorMessage = isValid;
                isValid &= form.ValidateData();
            }

            validationErrorShown = !isValid;
            documentForm.ShowValidationErrorMessage = isValid;
        };

        documentForm.OnBeforeSave += (sender, args) =>
        {
            // Save the SKU data before saving to the database
            SaveSKUDataToProduct();
        };

        HideRedundantDocumentFields();
    }


    /// <summary>
    /// Initializes the header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        if (Product is TreeNode)
        {
            // Initialize edit menu
            editMenuElem.Visible = true;
            editMenuElem.StopProcessing = false;
        }
        else
        {
            // Initialize header actions
            pnlHeaderActions.Visible = true;
            headerActionsElem.StopProcessing = false;

            SaveAction saveAction = new SaveAction();
            if ((DocumentManager != null) && DocumentManager.ConfirmChanges)
            {
                saveAction.OnClientClick = DocumentManager.GetAllowSubmitScript();
            }
            headerActionsElem.AddAction(saveAction);

            if (FormMode == FormModeEnum.Insert)
            {
                headerActionsElem.AddAction(new HeaderAction
                {
                    Text = GetString("editmenu.iconsaveandanother"),
                    Tooltip = GetString("editmenu.iconsaveandanother"),
                    Enabled = Enabled,
                    CommandName = "savecreateanother"
                });
            }

            headerActionsElem.ActionPerformed += (sender, args) => HandlePostbackCommand(args.CommandName);
        }
    }


    /// <summary>
    /// Initializes the custom properties panel.
    /// </summary>
    private void InitCustomProperties()
    {
        skuCustomForm.Visible = HasAnyCustomSKUFields;

        if (!documentForm.StopProcessing)
        {
            documentForm.Visible = HasAnyDocumentFields;
        }

        pnlCustomProperties.Visible = HasAnyCustomPropertiesFields;

        string elementName = ProductIsProductOption ? "ProductOptions.Options.CustomFields" : "Products.CustomFields";
        pnlCustomProperties.Visible &= MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement(ModuleName.ECOMMERCE, elementName);
    }


    /// <summary>
    /// Initializes the parent page.
    /// </summary>
    private void InitPage()
    {
        CMSDeskPage page = Page as CMSDeskPage;
        if (page != null)
        {
            page.CurrentMaster.PanelHeader?.Controls.Add(plcHeaderActions);

            if (page.RequiresDialog)
            {
                editMenuElem.ShowSaveAndClose = true;

                // Add the product name to the dialog title
                var treeNode = Product as TreeNode;
                if (treeNode != null)
                {
                    string nodeName = HTMLHelper.HTMLEncode(treeNode.GetDocumentName());
                    string text = $"{GetString("content.edittitle")} \"{nodeName}\"";
                    page.SetTitle(text);
                }
            }
        }
    }


    /// <summary>
    /// Initializes the price field for editing product option.
    /// </summary>
    private void InitPriceField()
    {
        // Display empty field if price is 0 if form is loaded
        skuGeneralForm.OnAfterDataLoad += (sender, e) =>
        {
            var priceControl = skuGeneralForm.FieldControls["SKUPrice"];
            var price = ValidationHelper.GetDecimal(priceControl.Value, 0);

            if (price == 0)
            {
                priceControl.Value = "";
            }
        };

        // Save 0 to DB in case of empty value
        skuGeneralForm.OnBeforeValidate += (sender, e) =>
        {
            var priceControl = skuGeneralForm.FieldControls["SKUPrice"];
            if (priceControl.Value == null)
            {
                priceControl.Value = 0;
            }
        };

        // Display empty field if price is 0 after form is saved
        skuGeneralForm.OnAfterSave += (sender, e) => skuGeneralForm.ReloadData();
    }

    #endregion


    #region "Validation"

    /// <summary>
    /// Validates the forms.
    /// Returns true if the form data is valid, otherwise returns false.
    /// </summary>
    public bool IsValid()
    {
        bool isValid = !validationErrorShown;

        // Validate new SKU binding
        if (!selectSkuBindingElem.IsValid())
        {
            ShowError(selectSkuBindingElem.ValidationError);
            isValid = false;
        }

        // Validate general SKU forms
        if (CreateNewSKU)
        {
            skuGeneralForm.ShowValidationErrorMessage = isValid;
            isValid &= skuGeneralForm.ValidateData();
            if (SKURepresentingForm != null)
            {
                SKURepresentingForm.ShowValidationErrorMessage = isValid;
                isValid &= SKURepresentingForm.ValidateData();
            }
            skuCustomForm.ShowValidationErrorMessage = isValid;
            isValid &= skuCustomForm.ValidateData();
        }

        // Validate document form
        if (Product is TreeNode)
        {
            documentForm.ShowValidationErrorMessage = isValid;
            isValid &= documentForm.ValidateData();
        }

        // Validate other SKU form
        if (CreateNewSKU)
        {
            skuOtherForm.ShowValidationErrorMessage = isValid;
            isValid &= skuOtherForm.ValidateData();
        }

        return isValid;
    }


    /// <summary>
    /// Performs an additional SKU validation on the specified form control.
    /// Returns an error message if the field is not valid, otherwise returns null.
    /// </summary>
    private string ValidateSkuField(FormEngineUserControl formControl)
    {
        // General validation
        switch (formControl.FieldInfo.Name)
        {
            case "SKUImagePath":
                {
                    string value = formControl.Value as string;

                    // Validate meta files file system permissions
                    string siteName = SiteInfoProvider.GetSiteName(ProductSiteID);
                    var filesLocationType = FileHelper.FilesLocationType(siteName);

                    if (!string.IsNullOrEmpty(value) && (filesLocationType != FilesLocationTypeEnum.Database))
                    {
                        string path = MetaFileInfoProvider.GetFilesFolderPath(siteName);
                        if (!DirectoryHelper.CheckPermissions(path))
                        {
                            return GetString("com.newproduct.accessdeniedtopath");
                        }
                    }
                }
                break;

            case "SKUMembershipGUID":
                {
                    Guid value = ValidationHelper.GetGuid(formControl.Value, Guid.Empty);

                    // Validate membership is selected
                    if (value == Guid.Empty)
                    {
                        return GetString("com.membership.nomembershipselectederror");
                    }
                }
                break;

            case "SKUWeight":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate weight is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packageweightinvalid");
                    }
                }
                break;

            case "SKUHeight":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate height is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packageheightinvalid");
                    }
                }
                break;

            case "SKUWidth":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate width is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packagewidthinvalid");
                    }
                }
                break;

            case "SKUDepth":
                {
                    double? value = GetNullableDouble(formControl.Value);

                    // Validate depth is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.packagedepthinvalid");
                    }
                }
                break;

            case "SKUMinItemsInOrder":
                {
                    int? value = GetNullableInteger(formControl.Value);

                    // Validate minimum items in one order is > 0
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.minorderitemsinvalid");
                    }

                    // Validate minimum items in one order is lower than maximum
                    int? maxItems = GetNullableInteger(GetFieldValue(CurrentSKUForms, "SKUMaxItemsInOrder"));
                    if (value.HasValue && maxItems.HasValue && (value.Value > maxItems.Value))
                    {
                        return GetString("com.productedit.minorderitemsexceedsmax");
                    }
                }
                break;

            case "SKUMaxItemsInOrder":
                {
                    int? value = GetNullableInteger(formControl.Value);

                    // Validate maximum items in one order
                    if (value.HasValue && value.Value <= 0)
                    {
                        return GetString("com.productedit.maxorderitemsinvalid");
                    }
                }
                break;
        }

        return null;
    }


    private void ClearHiddenFieldValues()
    {
        if (skuOtherForm.StopProcessing)
        {
            return;
        }

        // Set default values for hidden fields (used in inventory fields)
        foreach (DictionaryEntry item in skuOtherForm.FieldControls)
        {
            FormFieldInfo field = skuOtherForm.FormInformation.GetFormField(item.Key.ToString());

            if (field == null)
            {
                continue;
            }

            FormEngineUserControl control = item.Value as FormEngineUserControl;

            if (control != null && !control.Visible)
            {
                control.Value = field.GetPropertyValue(FormFieldPropertyEnum.DefaultValue);
            }
        }
    }

    #endregion


    #region "Save"

    /// <summary>
    /// Saves SKU inventory of SKU object. Value of SKUAvailableItems and SKUTrackInventory is in NonVersionedCoupledColumns collection 
    /// which means that versioned value is not saved to SKU object automatically and must be saved manually.
    /// </summary>
    private void SaveInventory()
    {
        if ((Product is SKUTreeNode) && ((SKUTreeNode)Product).DocumentCheckedOutVersionHistoryID > 0)
        {
            if (SKU == null)
            {
                return;
            }

            var sku = SKUInfo.Provider.Get(SKU.SKUID);

            if ((sku != null) && ((sku.SKUAvailableItems != SKU.SKUAvailableItems) || (sku.SKUTrackInventory != SKU.SKUTrackInventory)))
            {
                sku.SKUAvailableItems = SKU.SKUAvailableItems;
                sku.SKUTrackInventory = SKU.SKUTrackInventory;
                SKUInfo.Provider.Set(sku);
            }
        }
    }


    /// <summary>
    /// Saves the SKU data to the edited product object.
    /// </summary>
    private void SaveSKUDataToProduct()
    {
        ClearHiddenFieldValues();

        if (UseExistingSKU)
        {
            // Set existing SKU
            ((SKUTreeNode)Product).NodeSKUID = selectSkuBindingElem.SelectedProduct;

            CheckProductPermissions();

            return;
        }

        CheckProductPermissions();

        // Clear the product 'representing' properties
        Product.SetValue("SKUMembershipGUID", null);
        Product.SetValue("SKUValidity", null);
        Product.SetValue("SKUValidFor", null);
        Product.SetValue("SKUValidUntil", null);
        Product.SetValue("SKUBundleInventoryType", null);

        if ((NewProductOptionCategory != null) && ProductIsNew)
        {
            Product.SetValue("SKUOrder", Product.Generalized.GetLastObjectOrder());
        }

        // Save the form data
        foreach (BasicForm form in CurrentSKUForms.Where(f => f.Visible))
        {
            // For new product bundle binding is saved after SKU data
            if (ProductIsNew && form.Equals(skuBundleForm))
            {
                continue;
            }
            form.SaveData(null);
        }

        SaveInventory();

        // Fix valid until value
        var validUntil = ValidationHelper.GetDateTime(Product.GetValue("SKUValidUntil"), DateTimeHelper.ZERO_TIME);
        if (validUntil == DateTimeHelper.ZERO_TIME)
        {
            Product.SetValue("SKUValidUntil", null);
        }
    }


    /// <summary>
    /// Saves the edited product object to the database as a standalone SKU.
    /// </summary>
    private void SaveStandaloneSKU()
    {
        var sku = Product as SKUInfo;

        validationErrorShown = false;
        if ((sku == null) || !IsValid())
        {
            return;
        }

        SaveSKUDataToProduct();

        SKUInfo.Provider.Set(sku);

        if (sku.SKUID == 0)
        {
            return;
        }

        CurrentSKUForms.ForEach(f => f.RaiseOnUploadFile(this, EventArgs.Empty));
        RaiseProductSaved();
        ShowChangesSaved();
    }

    #endregion


    #region "IPostBackEventHandler members"

    /// <summary>
    /// Handles the postback event.
    /// </summary>
    /// <param name="argument">An optional postback event argument</param>
    public void RaisePostBackEvent(string argument)
    {
        HandlePostbackCommand(argument);
    }

    #endregion


    #region "SKU fields mapping"

    /// <summary>
    /// Hides redundant document fields from CMS form.
    /// </summary>
    private void HideRedundantDocumentFields()
    {
        // Do not process
        if (Product is SKUInfo)
        {
            return;
        }

        HideDocumentNameField();
    }


    /// <summary>
    /// Hides 'Document name' field if SKU name field is already visible.
    /// </summary>
    private void HideDocumentNameField()
    {
        if (IsSKUFieldVisible("SKUName"))
        {
            var node = DocumentManager.Node;
            if (node != null)
            {
                var fi = FormHelper.GetFormInfo(node.NodeClassName, false);
                if ((fi != null) && fi.FieldExists("DocumentName"))
                {
                    return;
                }
            }

            HideDocumentField("DocumentName");
        }
    }


    /// <summary>
    /// Indicates if the given SKU field is visible in some of the SKU forms.
    /// </summary>
    /// <param name="field">SKU field name to check</param>
    private bool IsSKUFieldVisible(string field)
    {
        if (UseExistingSKU)
        {
            // All SKU form are hidden but SKU has properties already specified
            return true;
        }

        if (DoNotCreateSKU)
        {
            // All SKU forms are hidden
            return false;
        }

        // Some SKU forms should be visible
        return CurrentSKUForms.Any(form => form.IsFieldVisible(field));
    }


    /// <summary>
    /// Hides the given document field from CMS form.
    /// </summary>
    /// <param name="field">Document field name</param>
    private void HideDocumentField(string field)
    {
        documentForm.FieldsToHide.Add(field);
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Check if current user is authorized to modify product
    /// </summary>
    private void CheckProductPermissions()
    {
        // Check module permissions
        bool global = (ProductSiteID <= 0);
        if (!ECommerceContext.IsUserAuthorizedToModifySKU(global))
        {
            RedirectToAccessDenied(ModuleName.ECOMMERCE, global ? EcommercePermissions.ECOMMERCE_MODIFYGLOBAL : "EcommerceModify OR ModifyProducts");
        }
    }


    protected void selectSkuBindingElem_Changed(object sender, EventArgs e)
    {
        // Reload selectors to ensure correct site/global data
        ReloadSiteSeparatedSelector("SKUDepartmentID");
        ReloadSiteSeparatedSelector("SKUManufacturerID");
        ReloadSiteSeparatedSelector("SKUSupplierID");
        ReloadSiteSeparatedSelector("SKUPublicStatusID");
        ReloadSiteSeparatedSelector("SKUInternalStatusID");

        // Reload bundle selector
        var bundleSelector = skuBundleForm.FieldControls?["skubundleitemscount"];
        if (bundleSelector != null)
        {
            bundleSelector.Value = null;
            skuBundleForm.ReloadData();
        }

        // Reload tax selector
        var taxFieldControl = skuGeneralForm.FieldControls?["skutaxclassid"];
        var selector = taxFieldControl as UniSelector;
        selector?.Reload(true);
    }


    /// <summary>
    /// Reloads the site separated selector for given field.
    /// </summary>
    /// <param name="fieldName">Name of the field.</param>
    private void ReloadSiteSeparatedSelector(string fieldName)
    {
        SiteSeparatedObjectSelector selector = FindFieldFormControl(CurrentSKUForms, fieldName) as SiteSeparatedObjectSelector;
        if (selector != null)
        {
            selector.EnsureSelectedItem = false;
            selector.Reload();
        }
    }


    /// <summary>
    /// Sets selector visibility for given <paramref name="fieldName"/> when global product is being edited.
    /// </summary>
    private void SetSelectorVisibility(string fieldName)
    {
        if (!IsGlobalProductSelected)
        {
            return;
        }

        skuGeneralForm.FieldsToHide.Add(fieldName);
    }


    /// <summary>
    /// Ensures the product available items count. If track inventory type is set from by variants to by product, product available count is set to sum of all available items from variants.
    /// </summary>
    private void EnsureProductAvailableItemsCount()
    {
        TrackInventoryTypeEnum? actualTrackInventoryType = GetNullableTrackInventoryType(GetFieldValue(CurrentSKUForms, "SKUTrackInventory"));

        SKUInfo sku;
        SKUTreeNode productTreeNode = Product as SKUTreeNode;
        // Stand-alone SKUs Product property is SKUInfo, classic SKUs Product property is SKUTreeNode with SKUInfo in SKU property
        if (productTreeNode != null)
        {
            sku = productTreeNode.SKU;
        }
        else
        {
            sku = Product as SKUInfo;
        }

        // Null reference check
        if (actualTrackInventoryType.HasValue && (sku != null))
        {
            TrackInventoryTypeEnum oldTrackInventoryType = sku.SKUTrackInventory;
            // Track type was changed from by variants to by product
            if ((actualTrackInventoryType.Value == TrackInventoryTypeEnum.ByProduct) && (oldTrackInventoryType == TrackInventoryTypeEnum.ByVariants))
            {
                DataSet dsVariants = VariantHelper.GetVariants(sku.SKUID);

                if (!DataHelper.DataSourceIsEmpty(dsVariants))
                {
                    int variantAvailableItemsSum = 0;

                    // Sum available items through all variants
                    foreach (DataRow dr in dsVariants.Tables[0].Rows)
                    {
                        variantAvailableItemsSum += ValidationHelper.GetInteger(dr["SKUAvailableItems"], 0);
                    }

                    // Set sum to product if it is non-zero
                    if (variantAvailableItemsSum > 0)
                    {
                        SetFieldValue(CurrentSKUForms, "SKUAvailableItems", variantAvailableItemsSum);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Handles the postback command.
    /// </summary>
    private void HandlePostbackCommand(string command)
    {
        switch (command.ToLowerInvariant())
        {
            case "save":
            case "savecreateanother":
                ProductSavedCreateAnother = command.Equals("savecreateanother", StringComparison.OrdinalIgnoreCase);
                SaveStandaloneSKU();
                break;
        }
    }


    /// <summary>
    /// Finds and returns all the form controls for the specified form field name across all the specified forms.
    /// </summary>
    private IEnumerable<FormEngineUserControl> FindFieldFormControls(IEnumerable<BasicForm> forms, string fieldName)
    {
        return forms.Where(f => (f.FieldControls != null) && f.FieldControls.ContainsKey(fieldName)).Select(f => f.FieldControls[fieldName]);
    }


    /// <summary>
    /// Finds and returns the first form control for the specified form field name across all the specified forms.
    /// Returns null if the form control is not found.
    /// </summary>
    private FormEngineUserControl FindFieldFormControl(IEnumerable<BasicForm> forms, string fieldName)
    {
        return FindFieldFormControls(forms, fieldName).FirstOrDefault();
    }


    /// <summary>
    /// Invokes the specified action for the first specified field found across all the specified forms.
    /// Returns true if any fields were found and acted upon.
    /// </summary>
    private void ActOnField(IEnumerable<BasicForm> forms, string fieldName, Action<FormEngineUserControl> action)
    {
        var formControl = FindFieldFormControl(forms, fieldName);
        if (formControl != null)
        {
            action.Invoke(formControl);
        }
    }


    /// <summary>
    /// Sets the field value of the specified fields found across all the specified forms.
    /// </summary>
    private void SetFieldValue(IEnumerable<BasicForm> forms, string fieldName, object value)
    {
        ActOnField(forms, fieldName, f => f.Value = value);
    }


    /// <summary>
    /// Gets the value of the first field found across all the specified forms.
    /// </summary>
    private object GetFieldValue(IEnumerable<BasicForm> forms, string fieldName)
    {
        object value = null;
        ActOnField(forms, fieldName, f => value = f.Value);
        return value;
    }


    /// <summary>
    /// Gets the nullable integer representation of the value.
    /// </summary>
    private int? GetNullableInteger(object value)
    {
        if (value is int)
        {
            return (int)value;
        }

        if (!string.IsNullOrEmpty(value as string))
        {
            return ValidationHelper.GetInteger(value, 0);
        }

        return null;
    }


    /// <summary>
    /// Gets the nullable double representation of the value.
    /// </summary>
    private double? GetNullableDouble(object value)
    {
        if (value is double)
        {
            return (double)value;
        }

        if (!string.IsNullOrEmpty(value as string))
        {
            return ValidationHelper.GetDouble(value, 0);
        }

        return null;
    }


    /// <summary>
    /// Gets the nullable TrackInventoryTypeEnum representation of the value.
    /// </summary>
    private TrackInventoryTypeEnum? GetNullableTrackInventoryType(object value)
    {
        if (value is TrackInventoryTypeEnum)
        {
            return (TrackInventoryTypeEnum)value;
        }

        if (!string.IsNullOrEmpty(value as string))
        {
            return EnumStringRepresentationExtensions.ToEnum<TrackInventoryTypeEnum>(value.ToString());
        }

        return null;
    }

    #endregion
}