using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Categories_Controls_Categories : CMSAdminEditControl
{
    #region "Variables and constants"

    private int mSelectedCategoryId = -1;
    private int mSelectedParentId;
    private CategoryInfo mSelectedCategory;
    private const int CATEGORIES_ROOT_PARENT_ID = -1;
    private const int PERSONAL_CATEGORIES_ROOT_PARENT_ID = -2;
    private int mUserId;
    private bool mDisplayPersonalCategories = true;
    private bool mDisplaySiteCategories = true;
    private bool mDisplaySiteSelector = true;
    private bool canModifySite;
    private bool canModifyGlobal;
    private bool? mAllowGlobalCategories;
    private string mDocumentGridBaseWhere;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the user to manage categories for. Default value is ID of the current user.
    /// </summary>
    public int UserID
    {
        get
        {
            if (mUserId > 0)
            {
                return mUserId;
            }

            if (MembershipContext.AuthenticatedUser != null)
            {
                return MembershipContext.AuthenticatedUser.UserID;
            }

            return 0;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Get ID of the selected site.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (DisplaySiteSelector)
            {
                int siteId = SelectSite.SiteID;

                return (siteId < 0) ? 0 : siteId;
            }
            else
            {
                return SiteContext.CurrentSiteID;
            }
        }
    }


    /// <summary>
    /// Indicates whether personal categories are to be displayed.
    /// </summary>
    public bool DisplayPersonalCategories
    {
        get
        {
            return mDisplayPersonalCategories;
        }
        set
        {
            mDisplayPersonalCategories = value;
        }
    }


    /// <summary>
    /// Indicates whether general categories are to be displayed.
    /// </summary>
    public bool DisplaySiteCategories
    {
        get
        {
            return mDisplaySiteCategories;
        }
        set
        {
            mDisplaySiteCategories = value;
        }
    }


    /// <summary>
    /// Indicates whether site selector will be displayed.
    /// </summary>
    public bool DisplaySiteSelector
    {
        get
        {
            return mDisplaySiteSelector;
        }
        set
        {
            mDisplaySiteSelector = value;
        }
    }


    /// <summary>
    /// Allows to make control start with 'Create new category' form opened.
    /// </summary>
    public bool StartInCreatingMode
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            EnsureChildControls();
            base.StopProcessing = value;

            // Set inner controls StopProcessing property 
            if (treeElemG != null)
            {
                treeElemG.StopProcessing = value;
            }
            if (treeElemP != null)
            {
                treeElemP.StopProcessing = value;
            }
            if (gridDocuments != null)
            {
                gridDocuments.UniGrid.StopProcessing = value;
            }
            if (gridSubCategories != null)
            {
                gridSubCategories.StopProcessing = value;
            }
            if (catEdit != null)
            {
                catEdit.StopProcessing = value;
            }
            if (catNew != null)
            {
                catNew.StopProcessing = value;
            }
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            if (catEdit != null)
            {
                catEdit.IsLiveSite = value;
            }
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// ID of currently selected category.
    /// </summary>
    private int SelectedCategoryID
    {
        get
        {
            if (mSelectedCategoryId == -1)
            {
                string[] splits = hidSelectedElem.Value.Split('|');

                if (splits.Length > 0)
                {
                    return ValidationHelper.GetInteger(splits[0], 0);
                }
            }

            return mSelectedCategoryId;
        }
        set
        {
            mSelectedCategoryId = value;
            mSelectedCategory = null;
        }
    }


    /// <summary>
    /// Currently selected category object.
    /// </summary>
    private CategoryInfo SelectedCategory
    {
        get
        {
            if ((mSelectedCategory == null) && (SelectedCategoryID > 0))
            {
                mSelectedCategory = CategoryInfoProvider.GetCategoryInfo(SelectedCategoryID);
            }

            return mSelectedCategory;
        }
        set
        {
            mSelectedCategory = value;
        }
    }


    /// <summary>
    /// ID of the parent category of selected category.
    /// </summary>
    private int SelectedCategoryParentID
    {
        get
        {
            if (mSelectedParentId == 0)
            {
                string[] splits = hidSelectedElem.Value.Split('|');

                if (splits.Length > 1)
                {
                    return ValidationHelper.GetInteger(splits[1], CATEGORIES_ROOT_PARENT_ID);
                }
                else
                {
                    return DisplaySiteCategories ? CATEGORIES_ROOT_PARENT_ID : PERSONAL_CATEGORIES_ROOT_PARENT_ID;
                }
            }

            return mSelectedParentId;
        }
        set
        {
            mSelectedParentId = value;
        }
    }


    /// <summary>
    /// Indicates whether root category of personal categories is selected.
    /// </summary>
    private bool CustomCategoriesRootSelected
    {
        get
        {
            return SelectedCategoryParentID == PERSONAL_CATEGORIES_ROOT_PARENT_ID;
        }
    }


    /// <summary>
    /// Returns true when new category was created using dialog.
    /// </summary>
    private bool NewFromDialog
    {
        get
        {
            string[] splits = hidSelectedElem.Value.Split('|');

            if (splits.Length > 2)
            {
                return ValidationHelper.GetInteger(splits[2], 0) == 1;
            }

            return false;
        }
    }


    /// <summary>
    /// Indicates whether control is in editing mode.
    /// </summary>
    private bool IsEditing
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsEditing"], false);
        }
        set
        {
            ViewState["IsEditing"] = value;
        }
    }


    /// <summary>
    /// Indicates whether control is in mode of creating a new category.
    /// </summary>
    private bool IsCreating
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsCreating"], false);
        }
        set
        {
            ViewState["IsCreating"] = value;
        }
    }


    /// <summary>
    /// Indicates whether global categories are allowed for selected site.
    /// </summary>
    private bool AllowGlobalCategories
    {
        get
        {
            if (!mAllowGlobalCategories.HasValue)
            {
                string siteName = SiteInfoProvider.GetSiteName(SiteID);
                mAllowGlobalCategories = SettingsKeyInfoProvider.GetBoolValue(siteName + ".CMSAllowGlobalCategories");
            }

            return (bool)mAllowGlobalCategories;
        }
    }

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Prepare actions texts
        btnNew.ToolTip = GetString("categories.new");
        btnDelete.ToolTip = GetString("categories.delete");
        btnUp.ToolTip = GetString("general.up");
        btnDown.ToolTip = GetString("general.down");

        // Init grids
        gridDocuments.UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        gridDocuments.UniGrid.OnAfterDataReload += UniGrid_OnAfterDataReload;

        gridSubCategories.OnBeforeDataReload += gridSubCategories_OnBeforeDataReload;
        gridSubCategories.OnExternalDataBound += gridSubCategories_OnExternalDataBound;
        gridSubCategories.OnAction += gridSubCategories_OnAction;

        gridDocuments.IsLiveSite = IsLiveSite;
        gridSubCategories.IsLiveSite = IsLiveSite;
        gridSubCategories.ShowObjectMenu = !IsLiveSite;

        titleElem.HideBreadcrumbs = !IsLiveSite;

        // Prepare tabs headings
        tabGeneral.HeaderText = GetString("general.general");
        tabDocuments.HeaderText = GetString("Category_Edit.Documents");
        tabCategories.HeaderText = GetString("Development.Categories");

        // Init editing controls
        catNew.EditingForm.OnAfterSave += NewEditingForm_OnAfterSave;
        catEdit.EditingForm.OnAfterSave += EditingForm_OnAfterSave;

        catNew.IsLiveSite = IsLiveSite;
        catEdit.IsLiveSite = IsLiveSite;

        // Plant some trees
        treeElemG.OnNodeCreated += treeElem_OnNodeCreated;
        treeElemP.OnNodeCreated += treeElem_OnNodeCreated;

        // Get and store permissions
        canModifyGlobal = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", "GlobalModify");
        canModifySite = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", "Modify");

        // Init site selector
        SelectSite.DropDownSingleSelect.AutoPostBack = true;
        SelectSite.UniSelector.OnSelectionChanged += Selector_SelectedIndexChanged;
        if (!RequestHelper.IsPostBack())
        {
            SelectSite.SiteID = SiteContext.CurrentSiteID;
        }
    }


    private void NewEditingForm_OnAfterSave(object sender, EventArgs e)
    {
        // Set created category as selected
        SelectedCategoryID = catNew.Category.CategoryID;

        if (catNew.Category != null)
        {
            catEdit.UserID = catNew.Category.CategoryUserID;
            catEdit.CategoryID = catNew.Category.CategoryID;

            SwitchToEdit(true);
        }

        PreselectCategory(catNew.Category, false);

        // Open general tab after the new category is created
        pnlTabs.SelectedTab = tabGeneral;

        pnlUpdateTree.Update();
    }


    protected void EditingForm_OnAfterSave(object sender, EventArgs e)
    {
        PreselectCategory(catEdit.Category, false);

        // Refresh selected category info after update
        SelectedCategory = null;

        pnlUpdateTree.Update();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JQuery
        ScriptHelper.RegisterJQuery(Page);

        SelectSite.StopProcessing = !DisplaySiteSelector;
        plcSelectSite.Visible = DisplaySiteSelector;

        bool hasSelected = SelectedCategory != null;

        // Ensure correct object type for cloning
        if (hasSelected)
        {
            gridSubCategories.ObjectType = SelectedCategory.CategoryIsPersonal ? CategoryInfo.OBJECT_TYPE_USERCATEGORY : CategoryInfo.OBJECT_TYPE;
        }

        // Check if selection is valid
        CheckSelection();

        // Stop processing grids, when no category selected
        gridDocuments.UniGrid.StopProcessing = !hasSelected;
        gridDocuments.UniGrid.FilterForm.StopProcessing = !hasSelected;
        gridDocuments.UniGrid.Visible = hasSelected;

        gridSubCategories.StopProcessing = !hasSelected;
        gridSubCategories.FilterForm.StopProcessing = !hasSelected;
        gridSubCategories.Visible = hasSelected;

        if (!StopProcessing)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Start in mode of creating new category when requested
                if (StartInCreatingMode)
                {
                    SwitchToNew();
                }
                else
                {
                    SwitchToInfo();
                }
            }

            // Use images according to culture
            var imageUrl = (CultureHelper.IsUICultureRTL()) ? GetImageUrl("RTL/Design/Controls/Tree") : GetImageUrl("Design/Controls/Tree");
            treeElemG.LineImagesFolder = imageUrl;
            treeElemP.LineImagesFolder = imageUrl;

            treeElemG.StopProcessing = !DisplaySiteCategories;
            treeElemP.StopProcessing = !DisplayPersonalCategories;

            // Prepare node templates
            treeElemP.SelectedNodeTemplate = treeElemG.SelectedNodeTemplate = "<span id=\"node_##NODECODENAME####NODEID##\" class=\"ContentTreeItem ContentTreeSelectedItem\" onclick=\"SelectNode('##NODECODENAME####NODEID##'); if (NodeSelected) { NodeSelected(##NODEID##, ##PARENTID##); return false;}\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";
            treeElemP.NodeTemplate = treeElemG.NodeTemplate = "<span id=\"node_##NODECODENAME####NODEID##\" class=\"ContentTreeItem\" onclick=\"SelectNode('##NODECODENAME####NODEID##'); if (NodeSelected) { NodeSelected(##NODEID##, ##PARENTID##); return false;}\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";

            // Init tree provider objects
            treeElemG.ProviderObject = CreateTreeProvider(SiteID, 0);
            treeElemP.ProviderObject = CreateTreeProvider(0, UserID);

            // Expand first level by default
            treeElemP.ExpandPath = treeElemG.ExpandPath = "/";

            if (SelectedCategory != null)
            {
                catEdit.UserID = SelectedCategory.CategoryUserID;
                catEdit.Category = SelectedCategory;

                catNew.UserID = SelectedCategory.CategoryUserID;
                catNew.ParentCategoryID = SelectedCategory.CategoryID;
                catNew.SiteID = SiteID;
                catNew.AllowCreateOnlyGlobal = SiteID == 0;
                gridDocuments.SiteName = filterDocuments.SelectedSite;

                PreselectCategory(SelectedCategory, false);
            }
            else
            {
                catNew.UserID = CustomCategoriesRootSelected ? UserID : 0;
                catNew.SiteID = CustomCategoriesRootSelected ? 0 : SiteID;
                catNew.ParentCategoryID = 0;
                catNew.AllowCreateOnlyGlobal = SiteID == 0;
            }

            // Create root node for global and site categories
            string rootName = "<span class=\"TreeRoot\">" + GetString("categories.rootcategory") + "</span>";
            string rootText = treeElemG.ReplaceMacros(treeElemG.NodeTemplate, 0, 6, rootName, "", 0, null, null);

            rootText = rootText.Replace("##NODECUSTOMNAME##", rootName);
            rootText = rootText.Replace("##NODECODENAME##", "CategoriesRoot");
            rootText = rootText.Replace("##PARENTID##", CATEGORIES_ROOT_PARENT_ID.ToString());

            treeElemG.SetRoot(rootText, "NULL", null, null, null);

            // Create root node for personal categories
            rootName = "<span class=\"TreeRoot\">" + GetString("categories.rootpersonalcategory") + "</span>";
            rootText = treeElemP.ReplaceMacros(treeElemP.NodeTemplate, 0, 6, rootName, "", 0, null, null);

            rootText = rootText.Replace("##NODECUSTOMNAME##", rootName);
            rootText = rootText.Replace("##NODECODENAME##", "PersonalCategoriesRoot");
            rootText = rootText.Replace("##PARENTID##", PERSONAL_CATEGORIES_ROOT_PARENT_ID.ToString());
            treeElemP.SetRoot(rootText, "NULL", null, null, null);

            // Prepare script for selecting category
            string script = @"
var menuHiddenId = '" + hidSelectedElem.ClientID + @"';

function deleteConfirm() {
    return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + @"); 
}

function Refresh(catId, parentId) {
        // This method is used in Category cloning action
        NodeSelected(catId, parentId, true);
    }

function NodeSelected(elementId, parentId, isEdit) {
    // Set menu actions value
    var menuElem = $cmsj('#' + menuHiddenId);
    if (menuElem.length = 1) {
        menuElem[0].value = elementId + '|' + parentId;
    }

    if (isEdit) {
        " + ControlsHelper.GetPostBackEventReference(hdnButton, "edit") + @"
    } else {
        " + ControlsHelper.GetPostBackEventReference(hdnButton, "tree") + @"
    }
}";

            ltlScript.Text = ScriptHelper.GetScript(script);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (!StopProcessing)
        {
            // Prepare values for selection script
            string categoryName;
            int categoryId = 0;
            int categoryParentId;
            if (SelectedCategory != null)
            {
                categoryName = SelectedCategory.CategoryName;
                categoryId = SelectedCategory.CategoryID;
                categoryParentId = SelectedCategory.CategoryParentID;

                // Check if user can manage selected category
                bool canModify = CanModifySelectedCategory();

                // Set enabled state of actions
                btnDelete.Enabled = canModify;
                btnUp.Enabled = canModify;
                btnDown.Enabled = canModify;

                if (!SelectedCategory.CategoryIsPersonal)
                {
                    // Display New button when authorized to modify site categories
                    bool canCreate = canModifySite;

                    // Additionally check GlobalModify under global categories GlobalModify
                    if (SelectedCategory.CategoryIsGlobal)
                    {
                        canCreate |= canModifyGlobal;
                    }

                    btnNew.Enabled = canCreate;
                }
            }
            else
            {
                categoryParentId = SelectedCategoryParentID;
                categoryName = CustomCategoriesRootSelected ? "PersonalCategoriesRoot" : "CategoriesRoot";

                // Set enabled state of new category button
                btnNew.Enabled = CustomCategoriesRootSelected || canModifyGlobal || canModifySite;
            }

            ShowForms();

            // Enable/disable actions
            if (categoryId == 0)
            {
                btnDelete.Enabled = false;
                btnUp.Enabled = false;
                btnDown.Enabled = false;
            }

            pnlUpdateActions.Update();

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "CategorySelectionScript", ScriptHelper.GetScript("SelectNode(" + ScriptHelper.GetString(categoryName + categoryId) + ");"));
            hidSelectedElem.Value = categoryId.ToString() + '|' + categoryParentId;

            // Use correct CSS classes for edit/create mode
            pnlHeader.CssClass = IsEditing ? "PageHeader" : "PageHeader SimpleHeader";

            treeElemG.Visible = DisplaySiteCategories;
            treeElemP.Visible = DisplayPersonalCategories;

            // Reload trees
            treeElemG.ReloadData();
            treeElemP.ReloadData();
        }
    }

    #endregion


    #region "Event handlers"

    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        defaultNode.Selected = false;
        defaultNode.SelectAction = TreeNodeSelectAction.None;
        defaultNode.NavigateUrl = "";

        if (itemData != null)
        {
            CategoryInfo category = new CategoryInfo(itemData);
            // Ensure name
            string catName = category.CategoryName;

            // Ensure caption
            string caption = category.CategoryDisplayName;

            // Ensure parent category ID
            int catParentId = category.CategoryParentID;

            if (String.IsNullOrEmpty(caption))
            {
                caption = catName;
            }

            caption = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(caption));

            if (category.IsGlobal && !category.CategoryIsPersonal)
            {
                caption += " <sup>" + GetString("general.global") + "</sup>";
            }

            // Set caption
            defaultNode.Text = defaultNode.Text.Replace("##NODECUSTOMNAME##", caption)
                                          .Replace("##NODECODENAME##", HTMLHelper.HTMLEncode(catName))
                                          .Replace("##PARENTID##", catParentId.ToString());

            return defaultNode;
        }

        return null;
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    private void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Check if any category selected
        if (SelectedCategory != null)
        {
            bool selectRoot;

            // Preselect root category when site category is selected
            if (!SelectedCategory.CategoryIsGlobal)
            {
                selectRoot = true;
            }
            else
            {
                // Select root when global categories are not allowed
                selectRoot = !SelectedCategory.CategoryIsPersonal && !AllowGlobalCategories;
            }

            // Decide whether to select root
            if (selectRoot)
            {
                SelectedCategoryID = 0;
                SelectedCategoryParentID = CATEGORIES_ROOT_PARENT_ID;
                SwitchToInfo();
            }
            else
            {
                SwitchToEdit(true);
            }
        }
        else
        {
            // Switch to info message when root is selected
            SwitchToInfo();
        }

        // Update trees and content
        pnlUpdateContent.Update();
        pnlUpdateTree.Update();
    }


    /// <summary>
    /// Ensures filtering documents assigned to the selected category.
    /// </summary>
    protected void UniGrid_OnBeforeDataReload()
    {
        string where = "(DocumentID IN (SELECT CMS_DocumentCategory.DocumentID FROM CMS_DocumentCategory WHERE CategoryID = " + SelectedCategoryID + "))";

        var documentGrid = gridDocuments.UniGrid;
        if (!String.IsNullOrEmpty(mDocumentGridBaseWhere))
        {
            // Replace part of the where condition from the first load
            documentGrid.WhereCondition = documentGrid.WhereCondition.Replace(mDocumentGridBaseWhere, where);
        }
        else
        {
            // Add new part to the where condition
            documentGrid.WhereCondition = SqlHelper.AddWhereCondition(documentGrid.WhereCondition, where);
        }

        // Store current where condition for possible reload after a postback action (e.g. delete)
        mDocumentGridBaseWhere = where;

        // Add where condition from filter
        documentGrid.WhereCondition = SqlHelper.AddWhereCondition(documentGrid.WhereCondition, filterDocuments.WhereCondition);
    }


    /// <summary>
    /// Ensures filtering ancestor categories.
    /// </summary>
    protected void gridSubCategories_OnBeforeDataReload()
    {
        if (SelectedCategory != null)
        {
            gridSubCategories.WhereCondition = "CategoryParentID = " + SelectedCategory.CategoryID + " AND (ISNULL(CategorySiteID, 0) = " + SelectedCategory.CategorySiteID + " OR ISNULL(CategorySiteID, 0) = " + SiteID + ") AND ISNULL(CategoryUserID, 0) = " + SelectedCategory.CategoryUserID;
        }
    }


    protected object gridSubCategories_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
            case "delete":
                var button = sender as CMSGridActionButton;
                if (button != null)
                {
                    DataRowView data = UniGridFunctions.GetDataRowView(button.Parent as DataControlFieldCell);

                    int userId = ValidationHelper.GetInteger(data["CategoryUserID"], 0);
                    int siteId = ValidationHelper.GetInteger(data["CategorySiteID"], 0);

                    // Hide action when can not modify
                    button.Visible = CanModifyCategory(userId > 0, siteId == 0);
                }
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Ensures hiding of document filer.
    /// </summary>
    protected void UniGrid_OnAfterDataReload()
    {
        plcFilter.Visible = gridDocuments.UniGrid.DisplayExternalFilter(filterDocuments.FilterIsSet);
    }


    /// <summary>
    /// Handles sub categories grid actions.
    /// </summary>
    /// <param name="actionName">Action name</param>
    /// <param name="actionArgument">Parameter</param>
    protected void gridSubCategories_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int categoryId = ValidationHelper.GetInteger(actionArgument, 0);

                // Get category
                CategoryInfo categoryObj = CategoryInfoProvider.GetCategoryInfo(categoryId);
                if (categoryObj != null)
                {
                    // Delete the category
                    DeleteCategory(categoryObj);
                }
                break;
        }
    }


    #region "Actions"

    /// <summary>
    /// Handles category selection.
    /// </summary>
    protected void hdnButton_OnClick(object sender, EventArgs e)
    {
        if (SelectedCategory != null)
        {
            SwitchToEdit();
        }
        else
        {
            SwitchToInfo();
        }

        pnlUpdateContent.Update();

        string eventArgument = Page.Request.Params.Get("__EVENTARGUMENT");
        if (eventArgument == "edit")
        {
            PreselectCategory(SelectedCategory, false);
            pnlUpdateTree.Update();
            pnlTabs.SelectedTab = tabGeneral;
        }

        // Update also tree if category was created in dialog
        if (NewFromDialog)
        {
            pnlUpdateTree.Update();
        }
    }


    /// <summary>
    /// Handles New category button click.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Arguments</param>
    protected void btnNewElem_Click(object sender, EventArgs e)
    {
        SwitchToNew();

        pnlUpdateContent.Update();
    }


    /// <summary>
    /// Handles Delete category button click.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Arguments</param>
    protected void btnDeleteElem_Click(object sender, EventArgs e)
    {
        DeleteCategory(SelectedCategory, true);
    }


    /// <summary>
    /// Handles Move category up button click.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Arguments</param>
    protected void btnUpElem_Click(object sender, EventArgs e)
    {
        var category = SelectedCategory;
        if (category != null && CanModifyOrder(category))
        {
            CategoryInfoProvider.MoveCategoryUp(category.CategoryID);
        }

        pnlUpdateTree.Update();
    }


    /// <summary>
    /// Handles Move category down button click.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Arguments</param>
    protected void btnDownElem_Click(object sender, EventArgs e)
    {
        var category = SelectedCategory;
        if (category != null && CanModifyOrder(category))
        {
            CategoryInfoProvider.MoveCategoryDown(category.CategoryID);
        }

        pnlUpdateTree.Update();
    }

    #endregion


    #endregion


    #region "Methods"

    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);
        switch (propertyName.ToLowerCSafe())
        {
            case "displaypersonalcategories":
                DisplayPersonalCategories = ValidationHelper.GetBoolean(value, true);
                break;
            case "displaysitecategories":
                DisplaySiteCategories = ValidationHelper.GetBoolean(value, false);
                break;
            case "displaysiteselector":
                DisplaySiteSelector = ValidationHelper.GetBoolean(value, false);
                break;
        }

        return true;
    }


    /// <summary>
    /// Preselects category in the tree.
    /// </summary>
    /// <param name="categoryObj">Category to be selected.</param>
    /// <param name="expandLast">Indicates, if selected category is to be expanded.</param>
    private void PreselectCategory(CategoryInfo categoryObj, bool expandLast)
    {
        if (categoryObj != null)
        {
            // Decide which tree will be affected
            if (categoryObj.CategoryIsPersonal)
            {
                treeElemP.SelectPath = categoryObj.CategoryIDPath;
                treeElemP.SelectedItem = categoryObj.CategoryName;
                treeElemP.ExpandPath = categoryObj.CategoryIDPath + (expandLast ? "/" : "");
            }
            else
            {
                treeElemG.SelectPath = categoryObj.CategoryIDPath;
                treeElemG.SelectedItem = categoryObj.CategoryName;
                treeElemG.ExpandPath = categoryObj.CategoryIDPath + (expandLast ? "/" : "");
            }
        }
    }


    /// <summary>
    /// Creates tree provider object for categories assigned to specified site or user.
    /// </summary>
    /// <param name="siteId">ID of the site.</param>
    /// <param name="userId">ID of the user.</param>
    private UniTreeProvider CreateTreeProvider(int siteId, int userId)
    {
        // Create and set category provider
        UniTreeProvider provider = new UniTreeProvider
        {
            UseCustomRoots = true,
            RootLevelOffset = -1,
            ObjectType = "cms.category",
            DisplayNameColumn = "CategoryDisplayName",
            IDColumn = "CategoryID",
            LevelColumn = "CategoryLevel",
            OrderColumn = "CategoryOrder",
            ParentIDColumn = "CategoryParentID",
            PathColumn = "CategoryIDPath",
            ValueColumn = "CategoryID",
            ChildCountColumn = "CategoryChildCount"
        };

        // Prepare the parameters
        provider.Parameters = new QueryDataParameters();
        provider.Parameters.Add("SiteID", siteId);
        provider.Parameters.Add("IncludeGlobal", AllowGlobalCategories);
        provider.Parameters.Add("UserID", userId);

        provider.Columns = "CategoryID, CategoryName, CategoryDisplayName, CategoryLevel, CategoryOrder, CategoryParentID, CategoryIDPath, CategoryUserID, CategorySiteID, (SELECT COUNT(C.CategoryID) FROM CMS_Category AS C WHERE (C.CategoryParentID = CMS_Category.CategoryID) AND (ISNULL(C.CategorySiteID, 0) = @SiteID OR (C.CategorySiteID IS NULL AND @IncludeGlobal = 1)) AND (ISNULL(C.CategoryUserID, 0) = @UserID)) AS CategoryChildCount";
        provider.OrderBy = "CategoryUserID, CategorySiteID, CategoryOrder";
        provider.WhereCondition = "ISNULL(CategoryUserID, 0) = " + userId + " AND (ISNULL(CategorySiteID, 0) = " + siteId;
        if (AllowGlobalCategories && (siteId > 0))
        {
            provider.WhereCondition += " OR CategorySiteID IS NULL";
        }
        provider.WhereCondition += ")";

        return provider;
    }


    /// <summary>
    /// Switches control to editing mode.
    /// </summary>
    /// <param name="reloadGrids">Indicates if grids on Categories and Pages tabs should be reloaded</param>
    private void SwitchToEdit(bool reloadGrids = false)
    {
        IsEditing = true;
        IsCreating = false;

        plcEdit.Visible = true;

        catNew.StopProcessing = true;

        catEdit.StopProcessing = false;
        catEdit.ReloadData();

        gridDocuments.UniGrid.StopProcessing = false;
        gridDocuments.UniGrid.FilterForm.StopProcessing = false;
        gridDocuments.UniGrid.Visible = true;

        gridSubCategories.StopProcessing = false;
        gridSubCategories.FilterForm.StopProcessing = false;
        gridSubCategories.Visible = true;

        if (reloadGrids)
        {
            gridDocuments.UniGrid.ReloadData();
            gridSubCategories.ReloadData();
        }

        pnlUpdateContent.Update();
    }


    /// <summary>
    /// Switches control to creating mode.
    /// </summary>
    private void SwitchToNew()
    {
        IsCreating = true;
        IsEditing = false;

        plcNew.Visible = true;

        catEdit.StopProcessing = true;

        catNew.StopProcessing = false;
        catNew.ReloadData();
    }


    /// <summary>
    /// Switches control to show information.
    /// </summary>
    private void SwitchToInfo()
    {
        IsCreating = false;
        IsEditing = false;

        catEdit.StopProcessing = true;
        catNew.StopProcessing = true;
    }


    /// <summary>
    /// Shows forms according to mode of control and initializes breadcrumbs.
    /// </summary>
    private void ShowForms()
    {
        plcNew.Visible = IsCreating;
        plcEdit.Visible = IsEditing && SelectedCategory != null;
        plcInfo.Visible = !plcNew.Visible && !plcEdit.Visible;

        if (plcNew.Visible || plcEdit.Visible)
        {
            // Create breadcrumbs
            CreateBreadcrumbs();
        }

        // Display title when creating a new category
        if (plcNew.Visible)
        {
            titleElem.TitleText = GetString("categories.new");
        }
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        // Init breadcrumbs data
        string text;
        string script;

        string[] idSplits = { };

        // Split category path
        if (SelectedCategory != null)
        {
            idSplits = SelectedCategory.CategoryIDPath.Trim('/').Split('/');
        }

        // Prepare root item
        if (CustomCategoriesRootSelected || ((SelectedCategory != null) && (SelectedCategory.CategoryUserID > 0)))
        {
            text = GetString("categories.rootpersonalcategory");
            script = "SelectNode('PersonalCategoriesRoot'); if (NodeSelected) { NodeSelected(0, -2);} return false;";
        }
        else
        {
            text = GetString("categories.rootcategory");
            script = "SelectNode('CategoriesRoot'); if (NodeSelected) { NodeSelected(0, -1);} return false;";
        }

        titleElem.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = text,
            OnClientClick = script
        });

        if (SelectedCategory != null)
        {
            if (IsLiveSite)
            {
                // Show full breadcrumbs path on live site
                int[] ids = ValidationHelper.GetIntegers(idSplits, 0);

                foreach (int id in ids)
                {
                    CategoryInfo currentCategory = CategoryInfoProvider.GetCategoryInfo(id);
                    if (currentCategory != null)
                    {
                        titleElem.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem
                        {
                            Text = ResHelper.LocalizeString(currentCategory.CategoryDisplayName),
                            RedirectUrl = " ",
                            OnClientClick = GetCategorySelectionScript(currentCategory)
                        });
                    }
                }
            }
            else if (!plcNew.Visible)
            {
                // Set edited object
                EditedObject = SelectedCategory;

                string displayName = SelectedCategory.CategoryDisplayName + (SelectedCategory.CategoryIsGlobal && !SelectedCategory.CategoryIsPersonal ? " " + GetString("general.global") : String.Empty);

                // Show only selected category display name in breadcrumb in UI
                titleElem.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem
                {
                    Text = ResHelper.LocalizeString(displayName),
                });

                UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.cms_category"));
            }
        }

        // Add new category item
        if (plcNew.Visible)
        {
            titleElem.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("categories.new")
            });
        }
    }


    private string GetCategorySelectionScript(CategoryInfo category)
    {
        string script = string.Empty;
        if (category != null)
        {
            script = string.Format("SelectNode('{0}{1}'); if (NodeSelected) {{ NodeSelected({1}, {2});}} return false;", category.CategoryName, category.CategoryID, category.CategoryParentID);
        }

        return script;
    }


    /// <summary>
    /// Returns true if current user can modify selected category.
    /// </summary>
    private bool CanModifySelectedCategory()
    {
        if (SelectedCategory != null)
        {
            return CanModifyCategory(SelectedCategory.CategoryIsPersonal, SelectedCategory.CategoryIsGlobal);
        }

        return false;
    }


    /// <summary>
    /// Returns true if current user can modify given category.
    /// </summary>
    private bool CanModifyCategory(bool personal, bool global)
    {
        if (!personal)
        {
            return global ? canModifyGlobal : canModifySite;
        }

        // Personal categories can be modified.
        return true;
    }


    /// <summary>
    /// Checks whether category belongs to current site, current user and whether it is allowed (in case of global category).
    /// </summary>
    private void CheckSelection()
    {
        if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return;
        }

        bool valid = true;

        if (SelectedCategory != null)
        {
            if (SelectedCategory.CategoryIsPersonal)
            {
                // Can not access personal categories from another user
                valid = (SelectedCategory.CategoryUserID == UserID);
            }
            else
            {
                // Global categories have to be allowed
                if (SelectedCategory.CategoryIsGlobal)
                {
                    valid = AllowGlobalCategories;
                }
                else
                {
                    // Site categories have to belong to selected site
                    valid = (SelectedCategory.CategorySiteID == SiteID);
                }
            }
        }

        // Select root when invalid
        if (!valid)
        {
            SelectedCategoryID = 0;
            SelectedCategoryParentID = DisplaySiteCategories ? CATEGORIES_ROOT_PARENT_ID : PERSONAL_CATEGORIES_ROOT_PARENT_ID;
            SwitchToInfo();
        }
    }


    private void DeleteCategory(CategoryInfo categoryObj, bool reload = false)
    {
        // Check if category
        if ((categoryObj != null) && CanModifyCategory(categoryObj.CategoryIsPersonal, categoryObj.CategoryIsGlobal))
        {
            var parentCategory = CategoryInfoProvider.GetCategoryInfo(categoryObj.CategoryParentID);
            var isPersonal = categoryObj.CategoryIsPersonal;

            // Delete category
            CategoryInfoProvider.DeleteCategoryInfo(categoryObj);

            // Check if deleted category has parent
            if (parentCategory != null)
            {
                SelectedCategoryID = parentCategory.CategoryID;

                // Switch to editing of parent category
                catEdit.UserID = parentCategory.CategoryUserID;
                catEdit.Category = parentCategory;

                SwitchToEdit(reload);
                PreselectCategory(parentCategory, false);
            }
            else
            {
                SelectedCategoryID = 0;
                SelectedCategoryParentID = isPersonal ? PERSONAL_CATEGORIES_ROOT_PARENT_ID : CATEGORIES_ROOT_PARENT_ID;
                SwitchToInfo();
            }

            pnlUpdateTree.Update();
            pnlUpdateContent.Update();
        }
    }


    private bool CanModifyOrder(CategoryInfo category)
    {
        if (category.CategoryIsPersonal)
        {
            return true;
        }

        return category.CategoryIsGlobal ? canModifyGlobal : canModifySite;
    }

    #endregion
}
