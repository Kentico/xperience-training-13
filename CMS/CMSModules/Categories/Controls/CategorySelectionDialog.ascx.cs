using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;

public partial class CMSModules_Categories_Controls_CategorySelectionDialog : CMSUserControl, ICallbackEventHandler
{
    #region "Variables and constants"

    private SelectionModeEnum mSelectionMode = SelectionModeEnum.SingleButton;
    private string mValuesSeparator = ";";
    private string mSecurityPurpose;

    private bool mAllowMultiple;
    private bool? mAllowGlobalCategories;
    private string whereCondition;
    private string callbackValues;
    private string callbackMethod;
    private bool allowEditTextBox;
    private bool fireOnChanged;
    private string returnColumnName = "CategoryID";
    private string disabledItems = "";
    private Hashtable parameters;
    private int mSelectedCategoryId = -1;
    private CategoryInfo mSelectedCategory;
    private int mSelectedParentId;
    private const int CATEGORIES_ROOT_PARENT_ID = -1;
    private const int PERSONAL_CATEGORIES_ROOT_PARENT_ID = -2;
    private bool canModifySite;
    private bool canModifyGlobal;

    // Actions
    private HeaderAction upAction;
    private HeaderAction downAction;
    private HeaderAction deleteAction;
    private HeaderAction editAction;
    private HeaderAction newAction;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the user to manage categories for. Default value is ID of the current user.
    /// </summary>
    public int UserID
    {
        get
        {
            if (MembershipContext.AuthenticatedUser != null)
            {
                return MembershipContext.AuthenticatedUser.UserID;
            }

            return 0;
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
            treeElemG.IsLiveSite = value;
            treeElemP.IsLiveSite = value;

            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// ID of the selected category.
    /// </summary>
    public int SelectedCategoryID
    {
        get
        {
            if (mSelectedCategoryId == -1)
            {
                string[] splits = hidSelectedElem.Value.Split('|');

                if (splits.Length == 2)
                {
                    return ValidationHelper.GetInteger(splits[0], -1);
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
    /// Selected category object.
    /// </summary>
    public CategoryInfo SelectedCategory
    {
        get
        {
            return mSelectedCategory ?? (mSelectedCategory = CategoryInfo.Provider.Get(SelectedCategoryID));
        }
    }


    /// <summary>
    /// ID of the parent category of selected category.
    /// </summary>
    public int SelectedCategoryParentID
    {
        get
        {
            if (mSelectedParentId == 0)
            {
                string[] splits = hidSelectedElem.Value.Split('|');

                if (splits.Length == 2)
                {
                    return ValidationHelper.GetInteger(splits[1], CATEGORIES_ROOT_PARENT_ID);
                }

                return CATEGORIES_ROOT_PARENT_ID;
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
    public bool PersonalCategoriesRootSelected
    {
        get
        {
            return SelectedCategoryParentID == PERSONAL_CATEGORIES_ROOT_PARENT_ID;
        }
    }


    /// <summary>
    /// Indicates whether root category of site/global categories is selected.
    /// </summary>
    public bool CategoriesRootSelected
    {
        get
        {
            return SelectedCategoryParentID == CATEGORIES_ROOT_PARENT_ID;
        }
    }


    /// <summary>
    /// Indicates if current user can modify global categories.
    /// </summary>
    public bool CanModifyGlobalCategories
    {
        get
        {
            return canModifyGlobal;
        }
    }


    /// <summary>
    /// Indicates if current user can modify site categories.
    /// </summary>
    public bool CanModifySiteCategories
    {
        get
        {
            return canModifySite;
        }
    }


    /// <summary>
    /// Indicates whether global categories are allowed for selected site.
    /// </summary>
    public bool AllowGlobalCategories
    {
        get
        {
            if (!mAllowGlobalCategories.HasValue)
            {
                mAllowGlobalCategories = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSAllowGlobalCategories");
            }

            return (bool)mAllowGlobalCategories;
        }
    }


    /// <summary>
    /// Dialog selection mode.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            return mSelectionMode;
        }
    }


    /// <summary>
    /// Selected values separator.
    /// </summary>
    public string ValuesSeparator
    {
        get
        {
            return mValuesSeparator;
        }
    }


    /// <summary>
    /// Indicates whether multiple selection is allowed
    /// </summary>
    public bool AllowMultipleSelection
    {
        get
        {
            return mAllowMultiple;
        }
    }


    /// <summary>
    /// Allows to specify where to place actions.
    /// </summary>
    public HeaderActions Actions
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init trees handlers
        treeElemG.OnNodeCreated += treeElem_OnNodeCreated;
        treeElemP.OnNodeCreated += treeElem_OnNodeCreated;

        // Set enabled state of actions before rendering actions
        Page.PreRender += (sender, args) => HandleEnabledActions();

        // Load parameters
        LoadParameters();

        StopDisabledTrees();

        GetAndStorePermissions();

        // Expand and preselect selected category when in single select mode
        if (!RequestHelper.IsPostBack() && !AllowMultipleSelection)
        {
            SetSelectedCategory();
        }
    }


    private void SetSelectedCategory()
    {
        string value = hidItem.Value;
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        string[] values = value.Split(new[] { ValuesSeparator }, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != 1)
        {
            return;
        }

        int catId;

        if (returnColumnName == "CategoryID")
        {
            catId = ValidationHelper.GetInteger(values[0], 0);
        }
        else
        {
            CategoryInfo cat = CategoryInfo.Provider.Get(values[0], SiteContext.CurrentSiteID);
            catId = (cat != null) ? cat.CategoryID : 0;
        }

        if (catId > 0)
        {
            // Select category
            SelectedCategoryID = catId;
        }
    }


    private void GetAndStorePermissions()
    {
        canModifyGlobal = AllowGlobalCategories && CheckPermissionForCategoriesResource("GlobalModify");
        canModifySite = CheckPermissionForCategoriesResource("Modify");
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register JQuery
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);

        // Initialize actions
        InitActions();

        // Check if selection is valid
        CheckSelection();

        // Prepare node templates
        treeElemP.SelectedNodeTemplate = treeElemG.SelectedNodeTemplate = "<span id=\"node_##NODECODENAME####NODEID##\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\" onclick=\"SelectNode('##NODECODENAME####NODEID##'); if (NodeSelected) { NodeSelected(##NODEID##, ##PARENTID##); ##ONCLICK## return false;}\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";
        treeElemP.NodeTemplate = treeElemG.NodeTemplate = "<span id=\"node_##NODECODENAME####NODEID##\" name=\"treeNode\" class=\"ContentTreeItem\" onclick=\"SelectNode('##NODECODENAME####NODEID##'); if (NodeSelected) { NodeSelected(##NODEID##, ##PARENTID##); ##ONCLICK## return false;}\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span>";

        treeElemP.UsePostBack = treeElemG.UsePostBack = false;
        treeElemG.ProviderObject = CreateTreeProvider(SiteContext.CurrentSiteID, 0);
        treeElemP.ProviderObject = CreateTreeProvider(0, UserID);

        treeElemP.ExpandPath = treeElemG.ExpandPath = "/";
        CategoryInfo categoryObj = SelectedCategory;
        if (categoryObj != null)
        {
            PreselectCategory(categoryObj);
        }

        AddRootNode();

        var script = GetSelectionScript();
        ltlScript.Text = ScriptHelper.GetScript(script);

        var treeScript = GetTreeScript();
        ltlTreeScript.Text = ScriptHelper.GetScript(treeScript);
    }


    private void AddRootNode()
    {
        // Create root node for global and site categories
        string rootName = "<span class=\"TreeRoot\">" + GetString("categories.rootcategory") + "</span>";
        string rootText = treeElemG.ReplaceMacros(treeElemG.NodeTemplate, 0, 6, rootName, null, 0, null, null);

        rootText = rootText.Replace("##NODECUSTOMNAME##", rootName);
        rootText = rootText.Replace("##NODECODENAME##", "CategoriesRoot");
        rootText = rootText.Replace("##PARENTID##", CATEGORIES_ROOT_PARENT_ID.ToString());
        rootText = rootText.Replace("##ONCLICK##", "");

        treeElemG.SetRoot(rootText, "NULL", null, RequestContext.URL + "#", null);

        // Create root node for personal categories
        rootName = "<span class=\"TreeRoot\">" + GetString("categories.rootpersonalcategory") + "</span>";
        rootText = treeElemP.ReplaceMacros(treeElemP.NodeTemplate, 0, 6, rootName, null, 0, null, null);

        rootText = rootText.Replace("##NODECUSTOMNAME##", rootName);
        rootText = rootText.Replace("##NODECODENAME##", "PersonalCategoriesRoot");
        rootText = rootText.Replace("##PARENTID##", PERSONAL_CATEGORIES_ROOT_PARENT_ID.ToString());
        rootText = rootText.Replace("##ONCLICK##", "");

        treeElemP.SetRoot(rootText, "NULL", null, RequestContext.URL + "#", null);
    }


    private string GetTreeScript()
    {
        StringBuilder script = new StringBuilder();
        script.Append(@"
var menuHiddenId = '", hidSelectedElem.ClientID , @"';
var paramElemId = '", hidParam.ClientID, @"';
function deleteConfirm() { 
    return confirm(", ScriptHelper.GetLocalizedString("general.confirmdelete"), @"); 
}

function RaiseHiddenPostBack() {", ControlsHelper.GetPostBackEventReference(hdnButton, ""), @"; }
");

        return script.ToString();
    }


    private string GetSelectionScript()
    {
        var parentClientId = QueryHelper.GetControlClientId("clientId", string.Empty);

        StringBuilder script = new StringBuilder();

        switch (SelectionMode)
        {
                // Button modes
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
            {
                // Register javascript code
                if (callbackMethod == null)
                {
                    script.AppendFormat("function SelectItems(items,hash) {{ wopener.US_SelectItems_{0}(items,hash); CloseDialog(); }}", parentClientId);
                }
                else
                {
                    script.AppendFormat("function SelectItems(items,hash) {{ wopener.{0}(items.replace(/^;+|;+$/g, ''),hash); CloseDialog(); }}", callbackMethod);
                }
            }
                break;

                // Selector modes
            default:
            {
                // Register javascript code
                script.Append(@"
function SelectItems(items, names, hiddenFieldId, txtClientId, hashClientId, hash) {
    if(items.length > 0) {
        wopener.US_SetItems(items, names, hiddenFieldId, txtClientId, null, hashClientId, hash);
    } else {
        wopener.US_SetItems('','', hiddenFieldId, txtClientId);
    }", (fireOnChanged ? "wopener.US_SelectionChanged_" + parentClientId + "();" : ""), @"
    return CloseDialog(); 
}

function SelectItemsReload(items, names, hiddenFieldId, txtClientId, hidValue, hashClientId, hash) {
    if (items.length > 0) {
        wopener.US_SetItems(items, names, hiddenFieldId, txtClientId, hidValue, hashClientId, hash);
    } else {
        wopener.US_SetItems('','', hiddenFieldId, txtClientId);
    }
    wopener.US_ReloadPage_", parentClientId, @"();
    return CloseDialog();
}");
            }
                break;
        }

        script.Append(@"
var nameElem = document.getElementById('", hidName.ClientID, @"');
            
function ItemsElem() {
    return document.getElementById('", hidItem.ClientID, @"');
}

function HashElem() {
    return document.getElementById('", hidHash.ClientID, @"');
}

function SetHash(hashvalue) {
    var hashElem = HashElem();
    if (hashElem != null) {
        hashElem.value = hashvalue;
    }
}

function Refresh(param) {
    var parameElem = $cmsj('#' + paramElemId);
    if (parameElem.length) {
        parameElem[0].value = param;
    }
    RaiseHiddenPostBack();
}

function disableParents(id, disable) {
    while (id > 0) {
        var chkbox = $cmsj('#chk' + id);
        id = 0;
        if (chkbox.length) {
            var continueToParent = true;
            var parentId = 0;
            var nameSplits = chkbox[0].name.split('_');
            if (nameSplits.length == 2) {
                parentId = nameSplits[1];
                if (!disable) {
                    var siblings = $cmsj('input[name$=\'_' + parentId + '\']:checked');
                    continueToParent = (siblings.length === 0);
                }
            }
            if (continueToParent) {
                var parentChkbox = $cmsj('#chk' + parentId);
                if (parentChkbox.length) {
                    if (disable) {
                        parentChkbox.attr('disabled', 'disabled');
                    } else {
                        parentChkbox.removeAttr('disabled');
                    }

                    if (ItemsElem().value.toLowerCase().indexOf('", ValuesSeparator, @"' + parentId + '", ValuesSeparator, @"') < 0) {
                        parentChkbox[0].checked = disable;
                    } else {
                        parentChkbox[0].checked = true;
                    }

                    id = parentId;
                }
            }
        }
    }
}

function ProcessItem(chkbox, hash, changeChecked, getHash) {
    var itemsElem = ItemsElem();
    var items = itemsElem.value;
    var checkHash = '';
    if (chkbox != null) {
        var item = chkbox.id.substr(3);
        if (changeChecked)
        {
            chkbox.checked = !chkbox.checked;
        }
        if (chkbox.checked)
        {
            if (items == '') {
                itemsElem.value = '", ValuesSeparator, @"' + item + '", ValuesSeparator, @"';
            }
            else if (items.toLowerCase().indexOf('", ValuesSeparator, @"' + item.toLowerCase() + '", ValuesSeparator, @"') < 0)
            {
                itemsElem.value += item + '", ValuesSeparator, @"';
            }
        }
        else
        {
            var re = new RegExp('", ValuesSeparator, "' + item + '", ValuesSeparator, @"', 'i');
            itemsElem.value = items.replace(re, '", ValuesSeparator, @"');
        }
        checkHash = '|' + item + '#' + hash;
        disableParents(item, chkbox.checked);
    }
    else
    {
        checkHash = '|' + items.replace('", ValuesSeparator, @"',';') + '#' + hash;
    }
    if (getHash) {
        ", Page.ClientScript.GetCallbackEventReference(this, "itemsElem.value + checkHash", "SetHash", null), @";
    }
}
            
function Cancel() { 
    wopener.US_RefreshPage_", parentClientId, @"(); CloseDialog(); 
}

function SelectAllItems(checkbox, hash) {
    var itemsElem = ItemsElem();
    itemsElem.value = '';
    SetHash('');
    var checkboxes = document.getElementsByTagName('input');
    for (var i = 0; i < checkboxes.length; i++) {
        var chkbox = checkboxes[i];
        if (chkbox.className == 'chckbox') {
            if (checkbox.checked) { chkbox.checked = true; }
            else { chkbox.checked = false; }

            ProcessItem(chkbox, null, false, false);
        }
    }
    ProcessItem(null, hash, false, true);
}
");
        script.Append(GetButtonsScript());

        return script.ToString();
    }


    private string GetButtonsScript()
    {
        var txtClientId = ScriptHelper.GetString(QueryHelper.GetString("txtElem", string.Empty));
        var hdnClientId = ScriptHelper.GetString(QueryHelper.GetString("hidElem", string.Empty));
        var hdnDrpClientId = ScriptHelper.GetString(QueryHelper.GetString("selectElem", string.Empty));
        var hashElementClientId = ScriptHelper.GetString(QueryHelper.GetString("hashElem", string.Empty));

        StringBuilder script = new StringBuilder();
        script.AppendLine("function EncodeValue(value){ return encodeURIComponent(value).replace(/'/g, '%27'); }");
        script.AppendLine("function US_Cancel(){ Cancel(); return false; }");

        switch (SelectionMode)
        {
            // Button modes
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
                script.AppendLine("function US_Submit(){ SelectItems(EncodeValue(ItemsElem().value),HashElem().value); return false; }");
                break;

            // Textbox modes
            case SelectionModeEnum.SingleTextBox:
            case SelectionModeEnum.MultipleTextBox:
                if (allowEditTextBox)
                {
                    script.AppendFormat("function US_Submit(){{ SelectItems(EncodeValue(ItemsElem().value), EncodeValue(ItemsElem().value.replace(/^{0}+|{0}+$/g, '')), {1}, {2}, {3}, HashElem().value); return false; }}", ValuesSeparator, hdnClientId, txtClientId, hashElementClientId);
                }
                else
                {
                    script.AppendFormat("function US_Submit(){{ SelectItemsReload(EncodeValue(ItemsElem().value), EncodeValue(nameElem.value), {0}, {1}, {2}, {3}, HashElem().value); return false; }}", hdnClientId, txtClientId, hdnDrpClientId, hashElementClientId);
                }
                break;

            // Other modes
            default:
                script.AppendFormat("function US_Submit(){{ SelectItemsReload(EncodeValue(ItemsElem().value), EncodeValue(nameElem.value), {0}, {1}, {2}, {3}, HashElem().value); return false; }}", hdnClientId, txtClientId, hdnDrpClientId, hashElementClientId);
                break;
        }

        return script.ToString();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        CategoryInfo categoryObj = SelectedCategory;

        string categoryName;
        int categoryId = 0;
        int categoryParentId;
        // Mark selected category
        if (categoryObj != null)
        {
            categoryName = categoryObj.CategoryName;
            categoryId = categoryObj.CategoryID;
            categoryParentId = categoryObj.CategoryParentID;
        }
        else
        {
            // Mark root when no category selected
            categoryParentId = SelectedCategoryParentID;
            categoryName = PersonalCategoriesRootSelected ? "PersonalCategoriesRoot" : "CategoriesRoot";
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CategorySelectionScript", ScriptHelper.GetScript("SelectNode(" + ScriptHelper.GetString(categoryName + categoryId) + ");"));
        hidSelectedElem.Value = categoryId.ToString() + '|' + categoryParentId;

        // Reload trees
        treeElemG.ReloadData();
        treeElemP.ReloadData();
    }

    #endregion


    #region "Event handlers"

    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        defaultNode.Selected = false;
        if (itemData != null)
        {
            CategoryInfo category = new CategoryInfo(itemData);

            string caption = category.CategoryDisplayName;
            if (String.IsNullOrEmpty(caption))
            {
                caption = category.CategoryName;
            }

            caption = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(caption));

            if (category.IsGlobal && !category.CategoryIsPersonal)
            {
                caption += " <sup>" + GetString("general.global") + "</sup>";
            }

            // Set caption
            defaultNode.Text = defaultNode.Text.Replace("##NODECUSTOMNAME##", caption);
            defaultNode.Text = defaultNode.Text.Replace("##NODECODENAME##", HTMLHelper.HTMLEncode(category.CategoryName));
            defaultNode.Text = defaultNode.Text.Replace("##PARENTID##", category.CategoryParentID.ToString());

            string onclick = "";
            string checkBox = "";
            bool catHasCheckedChildren = ValidationHelper.GetInteger(itemData["ChildChecked"], 0) > 0;

            if (AllowMultipleSelection)
            {
                // Prepare checbox when in multiple selection mode
                checkBox = string.Format("<span class=\"checkbox tree-checkbox\"><input id=\"chk{0}\" type=\"checkbox\" onclick=\"ProcessItem(this,'{1}',false,true);\" class=\"chckbox\" ", category.CategoryID, ValidationHelper.GetHashString(category.CategoryID.ToString(), new HashSettings(mSecurityPurpose)));
                if (catHasCheckedChildren || (hidItem.Value.IndexOfCSafe(ValuesSeparator + category.CategoryID + ValuesSeparator, true) >= 0))
                {
                    checkBox += "checked=\"checked\" ";
                }
                if (catHasCheckedChildren)
                {
                    checkBox += "disabled=\"disabled\" ";
                }
                checkBox += "name=\"" + category.CategoryID + "_" + category.CategoryParentID + "\"";
                checkBox += string.Format("/><label for=\"chk{0}\">&nbsp;</label>", category.CategoryID);
            }
            else
            {
                if (returnColumnName == "CategoryID")
                {
                    onclick = "ItemsElem().value = '" + ValuesSeparator + category.CategoryID + ValuesSeparator + "';";
                }
                else
                {
                    onclick = "ItemsElem().value = '" + ValuesSeparator + ScriptHelper.GetString(category.CategoryName, false) + ValuesSeparator + "';";
                }
            }

            defaultNode.Text = defaultNode.Text.Replace("##ONCLICK##", onclick);
            defaultNode.Text = checkBox + defaultNode.Text + "</span>";

            // Expand selected categories
            if (catHasCheckedChildren && !RequestHelper.IsPostBack())
            {
                defaultNode.Expand();
            }

            return defaultNode;
        }

        return null;
    }


    /// <summary>
    /// Handles actions risen from JavaScript.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        string param = hidParam.Value;

        // Check if action was saving of existing category
        if (param.StartsWithCSafe("edit"))
        {
            // Check if category was disabled during editing
            if ((SelectedCategory != null) && !SelectedCategory.CategoryEnabled)
            {
                // Select coresponding root element
                bool personal = SelectedCategory.CategoryIsPersonal;
                SelectedCategoryID = 0;
                SelectedCategoryParentID = personal ? PERSONAL_CATEGORIES_ROOT_PARENT_ID : CATEGORIES_ROOT_PARENT_ID;
            }

            pnlUpdateTrees.Update();
        }
        // Check if action was creation of new category
        else if (param.StartsWithCSafe("new"))
        {
            string[] splits = param.Split('|');
            if (splits.Length == 2)
            {
                int id = ValidationHelper.GetInteger(splits[1], 0);
                if (id > 0)
                {
                    // Select created category
                    CategoryInfo category = CategoryInfo.Provider.Get(id);
                    if (category != null)
                    {
                        SelectedCategoryID = category.CategoryID;
                        PreselectCategory(category);

                        if (!AllowMultipleSelection)
                        {
                            if (returnColumnName == "CategoryID")
                            {
                                hidItem.Value = ValuesSeparator + id + ValuesSeparator;
                            }
                            else
                            {
                                hidItem.Value = ValuesSeparator + ScriptHelper.GetString(category.CategoryName, false) + ValuesSeparator;
                            }
                            pnlHidden.Update();
                        }
                    }
                }
            }

            pnlUpdateTrees.Update();
        }

        // Remove parameter
        hidParam.Value = "";
    }


    /// <summary>
    /// Actions event handler.
    /// </summary>
    protected void Actions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "delete":
                Delete();
                break;

            case "up":
                Up();
                break;

            case "down":
                Down();
                break;

            case "expandall":
                ExpandAll();
                break;

            case "collapseall":
                CollapseAll();
                break;
        }
    }

    #endregion


    #region "Actions"

    /// <summary>
    /// Handles request for deleting category.
    /// </summary>
    public void Delete()
    {
        CategoryInfo categoryObj = SelectedCategory;

        if ((categoryObj != null) && CanModifySelectedCategory)
        {
            // Remove deleted category from selection
            if (!string.IsNullOrEmpty(hidItem.Value))
            {
                hidItem.Value = hidItem.Value.Replace(ValuesSeparator + categoryObj.CategoryID + ValuesSeparator, ValuesSeparator);
                hidHash.Value = ValidationHelper.GetHashString(hidItem.Value, new HashSettings(mSecurityPurpose));
                pnlHidden.Update();
            }

            // Preselect parent category
            CategoryInfo parentCategory = CategoryInfo.Provider.Get(categoryObj.CategoryParentID);
            if (parentCategory != null)
            {
                SelectedCategoryID = parentCategory.CategoryID;
                PreselectCategory(parentCategory, true);
            }
            else
            {
                SelectedCategoryID = 0;
                SelectedCategoryParentID = categoryObj.CategoryIsPersonal ? PERSONAL_CATEGORIES_ROOT_PARENT_ID : CATEGORIES_ROOT_PARENT_ID;
            }

            // Delete category
            CategoryInfo.Provider.Delete(categoryObj);

            pnlUpdateTrees.Update();
        }
    }


    /// <summary>
    /// Handles request for moving category up.
    /// </summary>
    public void Up()
    {
        // Move selected category up
        int catId = SelectedCategoryID;
        if ((catId > 0) && CanModifySelectedCategory)
        {
            CategoryInfoProvider.MoveCategoryUp(catId);
        }

        pnlUpdateTrees.Update();
    }


    /// <summary>
    /// Handles request for moving category down.
    /// </summary>
    public void Down()
    {
        // Move selected category down
        int catId = SelectedCategoryID;
        if ((catId > 0) && CanModifySelectedCategory)
        {
            CategoryInfoProvider.MoveCategoryDown(catId);
        }

        pnlUpdateTrees.Update();
    }


    /// <summary>
    /// Handles request for expanding trees.
    /// </summary>
    public void ExpandAll()
    {
        // Expand trees
        treeElemG.ExpandAll = true;
        treeElemP.ExpandAll = true;
        pnlUpdateTrees.Update();
    }


    /// <summary>
    /// Handles request for collapsing trees.
    /// </summary>
    public void CollapseAll()
    {
        // Collapse trees
        treeElemG.ExpandAll = false;
        treeElemP.ExpandAll = false;
        pnlUpdateTrees.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Preselects category allowing to expand its children.
    /// </summary>
    /// <param name="categoryObj">Category to be selected.</param>
    /// <param name="expandLast">When true, children of selected category will be expanded.</param>
    private void PreselectCategory(CategoryInfo categoryObj, bool expandLast = false)
    {
        if (categoryObj != null)
        {
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
    /// Loads control parameters.
    /// </summary>
    private void LoadParameters()
    {
        string identifier = QueryHelper.GetString("params", null);
        parameters = (Hashtable)WindowHelper.GetItem(identifier);

        if (parameters != null)
        {
            // Load values from session
            mSelectionMode = (SelectionModeEnum)parameters["SelectionMode"];
            mValuesSeparator = ValidationHelper.GetString(parameters["ValuesSeparator"], ";");
            whereCondition = ValidationHelper.GetString(parameters["WhereCondition"], null);
            callbackMethod = ValidationHelper.GetString(parameters["CallbackMethod"], null);
            allowEditTextBox = ValidationHelper.GetBoolean(parameters["AllowEditTextBox"], false);
            fireOnChanged = ValidationHelper.GetBoolean(parameters["FireOnChanged"], false);
            returnColumnName = ValidationHelper.GetString(parameters["ReturnColumnName"], null);
            disabledItems = ValidationHelper.GetString(parameters["DisabledItems"], null);
            mSecurityPurpose = ValidationHelper.GetString(parameters["SecurityPurpose"], String.Empty);

            switch (SelectionMode)
            {
                case SelectionModeEnum.Multiple:
                case SelectionModeEnum.MultipleButton:
                case SelectionModeEnum.MultipleTextBox:
                    mAllowMultiple = true;
                    break;

                case SelectionModeEnum.SingleButton:
                case SelectionModeEnum.SingleDropDownList:
                case SelectionModeEnum.SingleTextBox:
                    mAllowMultiple = false;
                    break;
            }

            // Pre-select unigrid values passed from parent window
            if (!RequestHelper.IsPostBack())
            {
                string values = (string)parameters["Values"];
                if (!String.IsNullOrEmpty(values))
                {
                    hidItem.Value = values;
                    hidHash.Value = ValidationHelper.GetHashString(hidItem.Value, new HashSettings(mSecurityPurpose));
                    parameters["Values"] = null;
                }
            }
        }
    }


    /// <summary>
    /// Stops trees which are disabled by dialog parameters.
    /// </summary>
    protected void StopDisabledTrees()
    {
        if (!string.IsNullOrEmpty(disabledItems))
        {
            // Stop personal categories tree when disabled
            if (disabledItems.ToLowerCSafe().Contains("personal"))
            {
                treeElemP.StopProcessing = true;
            }

            // Stop global and site categories tree when disabled
            if (disabledItems.ToLowerCSafe().Contains("globalandsite"))
            {
                treeElemG.StopProcessing = true;
            }
        }
    }


    /// <summary>
    /// Creates tree provider.
    /// </summary>
    /// <param name="siteId">ID of the site to create provider for.</param>
    /// <param name="userId">ID of the user to create provider for.</param>
    private UniTreeProvider CreateTreeProvider(int siteId, int userId)
    {
        // Create and set category provider
        UniTreeProvider provider = new UniTreeProvider();
        provider.UseCustomRoots = true;
        provider.RootLevelOffset = -1;
        provider.ObjectType = "cms.category";
        provider.DisplayNameColumn = "CategoryDisplayName";
        provider.IDColumn = "CategoryID";
        provider.LevelColumn = "CategoryLevel";
        provider.OrderColumn = "CategoryOrder";
        provider.ParentIDColumn = "CategoryParentID";
        provider.PathColumn = "CategoryIDPath";
        provider.ValueColumn = "CategoryID";
        provider.ChildCountColumn = "CategoryChildCount";

        // Prepare the parameters
        provider.Parameters = new QueryDataParameters();
        provider.Parameters.Add("SiteID", siteId);
        provider.Parameters.Add("IncludeGlobal", AllowGlobalCategories);
        provider.Parameters.Add("UserID", userId);

        string selected = "";
        string colName = "CategoryID";
        if (!string.IsNullOrEmpty(hidItem.Value))
        {
            // Selecting by ID
            if (returnColumnName == "CategoryID")
            {
                int[] valIds =
                    ValidationHelper.GetIntegers(
                            hidItem.Value.Split(new[] { ValuesSeparator }, StringSplitOptions.RemoveEmptyEntries),
                            0);
                selected = "0";
                foreach (int i in valIds)
                {
                    selected += "," + i;
                }
            }
            else
            {
                // Selecting by code name
                colName = "CategoryName";
                string[] valNames = hidItem.Value.Split(new[] { ValuesSeparator }, StringSplitOptions.RemoveEmptyEntries);
                selected = "N''";
                foreach (string name in valNames)
                {
                    selected += ", N'" + SqlHelper.EscapeQuotes(name) + "'";
                }
            }
        }

        if (string.IsNullOrEmpty(selected))
        {
            selected = (returnColumnName == "CategoryID") ? "0" : "N''";
        }

        // Subquery to obtain count of enabled child categories for specified user, site and 'use global categories' setting
        string ChildCountColumn = "(SELECT COUNT(C.CategoryID) FROM CMS_Category AS C WHERE (C.CategoryEnabled = 1) AND (C.CategoryParentID = CMS_Category.CategoryID) AND (ISNULL(C.CategorySiteID, 0) = @SiteID OR (C.CategorySiteID IS NULL AND @IncludeGlobal = 1)) AND (ISNULL(C.CategoryUserID, 0) = @UserID)) AS CategoryChildCount";

        // Subquery to obtain count of selected enabled child categories with no disabled parent.
        string CheckedChildCountColumn = "(SELECT COUNT(CategoryID) FROM CMS_Category AS cc WHERE (cc.CategoryEnabled = 1) AND (cc." + colName + " IN (" + selected + ")) AND (cc.CategoryIDPath LIKE CMS_Category.CategoryIDPath + '/%')  AND (NOT EXISTS(SELECT CategoryID FROM CMS_Category AS pc WHERE (pc.CategoryEnabled = 0) AND (cc.CategoryIDPath like pc.CategoryIDPath+'/%')))) AS ChildChecked";

        // Prepare 
        provider.Columns = string.Format("CategoryID, CategoryName, CategoryDisplayName, CategoryLevel, CategoryOrder, CategoryParentID, CategoryIDPath, CategoryUserID, CategorySiteID, {0}, {1}", ChildCountColumn, CheckedChildCountColumn);
        provider.OrderBy = "CategoryUserID, CategorySiteID, CategoryOrder";
        provider.WhereCondition = "ISNULL(CategoryUserID, 0) = " + userId + " AND (CategoryEnabled = 1) AND (ISNULL(CategorySiteID, 0) = " + siteId;
        if (AllowGlobalCategories && (siteId > 0))
        {
            provider.WhereCondition += " OR CategorySiteID IS NULL";
        }
        provider.WhereCondition += ")";

        // Append explicit where condition
        provider.WhereCondition = SqlHelper.AddWhereCondition(provider.WhereCondition, whereCondition);

        return provider;
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
                    valid = (SelectedCategory.CategorySiteID == SiteContext.CurrentSiteID);
                }
            }
        }

        // Select root when invalid
        if (!valid)
        {
            SelectedCategoryID = 0;
            SelectedCategoryParentID = CATEGORIES_ROOT_PARENT_ID;
        }
    }


    /// <summary>
    /// Returns true if current user can modify selected category.
    /// </summary>
    public bool CanModifySelectedCategory
    {
        get
        {
            CategoryInfo category = SelectedCategory;

            if (category != null)
            {
                if (!category.CategoryIsPersonal)
                {
                    return category.CategoryIsGlobal ? CanModifyGlobalCategories : CanModifySiteCategories;
                }

                // Personal categories can be modified.
                return true;
            }

            // No category selected
            return false;
        }
    }


    /// <summary>
    /// Initializes the actions.
    /// </summary>
    protected void InitActions()
    {
        if (Actions == null)
        {
            return;
        }

        // Append actions handler
        Actions.ActionPerformed += Actions_ActionPerformed;

        // Init actions images
        Actions.AddActions(
            newAction = new HeaderAction
            {
                Text = GetString("general.new"),
                CommandName = "New",
                ButtonStyle = ButtonStyle.Default
            },
            deleteAction = new HeaderAction
            {
                Text = GetString("general.delete"),
                CommandName = "Delete",
                OnClientClick = "return confirm(" + ScriptHelper.GetLocalizedString("General.ConfirmDelete") + ")",
                ButtonStyle = ButtonStyle.Default
            },
            editAction = new HeaderAction
            {
                Text = GetString("general.edit"),
                CommandName = "Edit",
                ButtonStyle = ButtonStyle.Default
            },
            upAction = new HeaderAction
            {
                Text = GetString("general.up"),
                CommandName = "Up",
                ButtonStyle = ButtonStyle.Default
            },
            downAction = new HeaderAction
            {
                Text = GetString("general.down"),
                CommandName = "Down",
                ButtonStyle = ButtonStyle.Default
            },
            new HeaderAction
            {
                Text = GetString("general.expandall"),
                CommandName = "ExpandAll",
                ButtonStyle = ButtonStyle.Default
            },
            new HeaderAction
            {
                Text = GetString("general.collapseall"),
                CommandName = "CollapseAll",
                ButtonStyle = ButtonStyle.Default
            }
        );
    }


    /// <summary>
    /// Enables/disables actions according to selected category.
    /// </summary>
    protected void HandleEnabledActions()
    {
        if (Actions == null)
        {
            return;
        }

        int categoryId = 0;

        bool newEnabled = true;
        bool upDownDeleteEnabled = true;
        bool editEnabled = true;

        // Get selected category and its parent IDs
        CategoryInfo category = SelectedCategory;
        if (category != null)
        {
            categoryId = category.CategoryID;

            // Check if user can manage selected category
            upDownDeleteEnabled = CanModifySelectedCategory;

            if (!category.CategoryIsPersonal)
            {
                // Display New button when authorized to modify site categories
                newEnabled = CanModifySiteCategories;

                // Additionally check GlobalModify under global categories 
                if (category.CategoryIsGlobal)
                {
                    newEnabled |= CanModifyGlobalCategories;
                }
            }
        }
        else
        {
            newEnabled = !CategoriesRootSelected || (CanModifyGlobalCategories || CanModifySiteCategories);
        }

        // Enable/disable actions
        if (categoryId == 0 || !HasUserReadAccess(SelectedCategory))
        {
            upDownDeleteEnabled = false;
            editEnabled = false;
        }

        string createPersonal = "";
        if ((CategoriesRootSelected) || ((category != null) && !category.CategoryIsPersonal))
        {
            createPersonal = "&personal=0";
        }

        newAction.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/Categories/Dialogs/CategoryEdit.aspx?parentId=" + categoryId + createPersonal) + "', 'CategoryEdit', 700, 530); return false;";
        newAction.Enabled = newEnabled;

        editAction.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/Categories/Dialogs/CategoryEdit.aspx?categoryId=" + categoryId) + "', 'CategoryEdit', 700, 530); return false;";
        editAction.Enabled = editEnabled;

        upAction.Enabled = upDownDeleteEnabled;
        downAction.Enabled = upDownDeleteEnabled;
        deleteAction.Enabled = upDownDeleteEnabled;
    }


    private bool HasUserReadAccess(CategoryInfo selectedCategory)
    {
        return IsCurrentUserCategory(selectedCategory) || CheckPermissionForCategoriesResource("Read");
    }


    private bool CheckPermissionForCategoriesResource(string permissionType)
    {
        return MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", permissionType);
    }


    private bool IsCurrentUserCategory(CategoryInfo selectedCategory)
    {
        return (selectedCategory != null) && (selectedCategory.CategoryUserID == UserID);
    }

    #endregion


    #region "ICallbackEventHandler Members"

    string ICallbackEventHandler.GetCallbackResult()
    {
        // Prepare the parameters for dialog
        string result = string.Empty;
        if (!string.IsNullOrEmpty(callbackValues))
        {
            bool isValid = false;

            string[] values = callbackValues.Split('|');
            if (values.Length == 2)
            {
                // Check hash of the selected item
                string[] checkValues = values[1].Split('#');

                var settings = new HashSettings(mSecurityPurpose)
                {
                    Redirect = false
                };

                isValid = (checkValues.Length == 2) && ValidationHelper.ValidateHash(checkValues[0].Trim(';'), checkValues[1], settings);
            }

            if (isValid)
            {
                // Get new hash for currently selected items
                result = ValidationHelper.GetHashString(values[0], new HashSettings(mSecurityPurpose));
            }
        }

        return result;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        callbackValues = eventArgument;
    }

    #endregion
}
