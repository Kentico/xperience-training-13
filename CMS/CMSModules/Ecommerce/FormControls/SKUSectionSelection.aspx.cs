using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("com.productsections.select")]
public partial class CMSModules_Ecommerce_FormControls_SKUSectionSelection : CMSModalPage, ICallbackEventHandler
{
    #region "Variables and constants"

    private const char NODE_SEPARATOR = '|';

    private string mValuesSeparator = ";";
    private string mWhereCondition;
    private string mCallbackValues;
    private string mReturnColumnName;
    private string mSecurityPurpose;
    private int mCreatedNodes;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        LoadParameters();

        SetSaveJavascript("return US_Submit();");
        SetSaveResourceString("general.select");
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register JQuery
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(Page);

        // Initialize actions
        InitActions();

        // Prepare tree menu
        InitTreeMenu();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
     
        // Reload tree
        treeElemG.ReloadData();

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "SKUProductSectionSelectionScript", GetSelectionScript(), true);      

        if (mCreatedNodes == 0)
        {
            pnlBody.Controls.Add(new Literal { Text = GetString("com.productsections.none") });
        }
    }

    #endregion


    #region "Event handlers"

    protected TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        defaultNode.Selected = false;
        if (itemData != null)
        {
            var treeNode = CMS.DocumentEngine.TreeNode.New(itemData);

            string caption = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(treeNode.NodeName));

            // Set caption
            defaultNode.Text = defaultNode.Text.Replace("##NODECUSTOMNAME##", caption);
            defaultNode.Text = defaultNode.Text.Replace("##NODECODENAME##", HTMLHelper.HTMLEncode(treeNode.NodeName));
            defaultNode.Text = defaultNode.Text.Replace("##PARENTID##", treeNode.NodeParentID.ToString());

            string onclick = $"ProcessItem(this,'{ValidationHelper.GetHashString(treeNode.NodeID.ToString(), new HashSettings(mSecurityPurpose))}',false,true);";
            bool catHasCheckedChildren = ValidationHelper.GetInteger(itemData["ChildChecked"], 0) > 0;

            string value = mReturnColumnName == "NodeGUID" ? treeNode.NodeGUID.ToString() : treeNode.NodeID.ToString();

            // Prepare checkbox when in multiple selection mode
            string checkBox = $"<span class=\"checkbox tree-checkbox\"><input id=\"chk{treeNode.NodeID}\" data-value=\"{value}\" type=\"checkbox\" onclick=\"{onclick}\" class=\"chckbox\" ";
            if (catHasCheckedChildren || (hidItem.Value.IndexOf(mValuesSeparator + value + mValuesSeparator, StringComparison.Ordinal) >= 0))
            {
                checkBox += "checked=\"checked\" ";
            }
            if (catHasCheckedChildren)
            {
                checkBox += "disabled=\"disabled\" ";
            }
            checkBox += $"name=\"{treeNode.NodeID}_{treeNode.NodeParentID}\" /><label for=\"chk{treeNode.NodeID}\">&nbsp;";

            defaultNode.Text = defaultNode.Text.Replace("##ONCLICK##", onclick);
            defaultNode.Text = checkBox + defaultNode.Text + "</span>";

            // Expand selected categories
            if (catHasCheckedChildren && !RequestHelper.IsPostBack())
            {
                defaultNode.Expand();
            }

            mCreatedNodes++;

            return defaultNode;
        }

        return null;
    }


    /// <summary>
    /// Actions event handler.
    /// </summary>
    protected void Actions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            case "expandall":
                ExpandAll();
                break;

            case "collapseall":
                CollapseAll();
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles request for expanding trees.
    /// </summary>
    private void ExpandAll()
    {
        // Expand tree
        treeElemG.ExpandAll = true;
        pnlUpdate.Update();
    }


    /// <summary>
    /// Handles request for collapsing trees.
    /// </summary>
    private void CollapseAll()
    {
        // Collapse tree
        treeElemG.ExpandAll = false;
        pnlUpdate.Update();
    }


    /// <summary>
    /// Loads control parameters.
    /// </summary>
    private void LoadParameters()
    {
        string identifier = QueryHelper.GetString("params", null);
        var parameters = (Hashtable)WindowHelper.GetItem(identifier);

        if (parameters != null)
        {
            // Load values from session
            mValuesSeparator = ValidationHelper.GetString(parameters["ValuesSeparator"], ";");
            mWhereCondition = ValidationHelper.GetString(parameters["WhereCondition"], null);
            mReturnColumnName = ValidationHelper.GetString(parameters["ReturnColumnName"], "NodeID");
            mSecurityPurpose = ValidationHelper.GetString(parameters["SecurityPurpose"], String.Empty);

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
    /// Creates tree provider.
    /// </summary>
    private UniTreeProvider CreateTreeProvider(int siteId)
    {
        // Get root node level
        var rootPath = ECommerceSettings.ProductsStartingPath(siteId);
        var rootLevel = rootPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length;

        // Create and set sku category provider
        var provider = new UniTreeProvider
        {
            RootLevelOffset = rootLevel,
            ObjectType = "cms.tree",
            DisplayNameColumn = "NodeName",
            IDColumn = "NodeID",
            LevelColumn = "NodeLevel",
            OrderColumn = "NodeOrder",
            ParentIDColumn = "NodeParentID",
            PathColumn = "NodeAliasPath",
            ValueColumn = "NodeID",
            ChildCountColumn = "NodeChildCount",
            Parameters = new QueryDataParameters
            {
                { "SiteID", siteId }
            }
        };

        string selected = null;
        if (!string.IsNullOrEmpty(hidItem.Value))
        {
            var splittedValues = hidItem.Value.Split(new[]
            {
                mValuesSeparator
            }, StringSplitOptions.RemoveEmptyEntries);

            selected = mReturnColumnName == "NodeGUID"
                ? string.Join(",", splittedValues.Select(x => $"'{ValidationHelper.GetGuid(x, Guid.Empty)}'"))
                : string.Join(",", splittedValues.Select(x => ValidationHelper.GetInteger(x, 0)));
        }

        // Subquery to obtain count of enabled child sku categories for specified user, site
        string childCountColumn = "(SELECT COUNT(T.NodeID) FROM CMS_Tree AS T INNER JOIN CMS_Class AS C ON T.NodeClassID = C.ClassID AND C.ClassIsProductSection = 1 WHERE (T.NodeParentID = CMS_Tree.NodeID) AND (T.NodeSiteID = @SiteID)) AS NodeChildCount";

        // Subquery to obtain count of selected enabled child categories with no disabled parent.
        string checkedChildCountColumn = string.IsNullOrEmpty(selected) ? "(0) AS ChildChecked" : $"(SELECT COUNT(TT.NodeID) FROM CMS_Tree AS TT WHERE (TT.{mReturnColumnName} IN ({selected})) AND (TT.NodeAliasPath LIKE CMS_Tree.NodeAliasPath + '/%')) AS ChildChecked";

        // Prepare 
        provider.Columns = $"NodeID, NodeGUID, NodeName, NodeLevel, NodeOrder, NodeParentID, NodeAliasPath, {childCountColumn}, {checkedChildCountColumn}";
        provider.OrderBy = "NodeAliasPath, NodeOrder";
        provider.WhereCondition = "NodeSiteID = @SiteID AND NodeAliasPath LIKE N'" + rootPath + "%' AND NodeClassID IN (SELECT ClassID FROM CMS_Class WHERE ClassIsProductSection = 1)";

        // Append explicit where condition
        provider.WhereCondition = SqlHelper.AddWhereCondition(provider.WhereCondition, mWhereCondition);

        return provider;
    }


    /// <summary>
    /// Initializes the actions.
    /// </summary>
    private void InitActions()
    {
        // Append actions handler
        actionsElem.ActionPerformed += Actions_ActionPerformed;

        // Init actions images
        actionsElem.AddActions(
            new HeaderAction
            {
                Text = GetString("general.expandall"),
                CommandName = "ExpandAll"
            },
            new HeaderAction
            {
                Text = GetString("general.collapseall"),
                CommandName = "CollapseAll"
            }
        );
    }


    private void InitTreeMenu()
    {
        treeElemG.NodeTemplate = "<span id=\"node_##NODECODENAME####NODEID##\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODECUSTOMNAME##</span></span></label>";

        treeElemG.UsePostBack = false;
        treeElemG.ProviderObject = CreateTreeProvider(SiteContext.CurrentSiteID);
        treeElemG.ExpandPath = "/";
    }


    private string GetSelectionScript()
    {
        var parentClientId = QueryHelper.GetControlClientId("clientId", string.Empty);

        var script = $@"

function SelectItemsReload(items, hiddenFieldId, txtClientId, hidValue, hashClientId, hash) {{
    if (items.length > 0) {{
        wopener.US_SetItems(items, '', hiddenFieldId, txtClientId, hidValue, hashClientId, hash);
    }} else {{
        wopener.US_SetItems('','', hiddenFieldId, txtClientId);
    }}
    wopener.US_ReloadPage_{parentClientId}();
    return CloseDialog();
}}
            
function ItemsElem() {{
    return document.getElementById('{hidItem.ClientID}');
}}

function HashElem() {{
    return document.getElementById('{hidHash.ClientID}');
}}

function SetHash(hashvalue) {{
    var hashElem = HashElem();
    if (hashElem != null) {{
        hashElem.value = hashvalue;
    }}
}}

function disableParents(id, disable) {{
    while (id > 0) {{
        var chkbox = $cmsj('#chk' + id);
        id = 0;
        if (chkbox.length) {{
            var continueToParent = true;
            var parentId = 0;
            var nameSplits = chkbox[0].name.split('_');
            if (nameSplits.length == 2) {{
                parentId = nameSplits[1];
                if (!disable) {{
                    var siblings = $cmsj('input[name$=\'_' + parentId + '\']:checked');
                    continueToParent = (siblings.length === 0);
                }}
            }}
            if (continueToParent) {{
                var parentChkbox = $cmsj('#chk' + parentId);
                if (parentChkbox.length) {{
                    if (disable) {{
                        parentChkbox.attr('disabled', 'disabled');
                    }} else {{
                        parentChkbox.removeAttr('disabled');
                    }}

                    if (ItemsElem().value.toLowerCase().indexOf('{mValuesSeparator}' + parentId + '{mValuesSeparator}') < 0) {{
                        parentChkbox[0].checked = disable;
                    }} else {{
                        parentChkbox[0].checked = true;
                    }}

                    id = parentId;
                }}
            }}
        }}
    }}
}}

function unselectParents(id) {{
    while (id > 0) {{
        var chkbox = $cmsj('#chk' + id);
        id = 0;
        if (chkbox.length) {{
            var nameSplits = chkbox[0].name.split('_');
            if (nameSplits.length == 2) {{
                var parentId = nameSplits[1];
                var parentChkbox = $cmsj('#chk' + parentId);
                if (parentChkbox != null) {{
                    var parentValue = parentChkbox.attr('data-value');
                    unselectItem(parentValue);
                    id = parentId;
                }}
            }}
        }}
    }}
}}

function unselectItem(itemValue) {{
    var itemsElem = ItemsElem();
    var re = new RegExp('{mValuesSeparator}' + itemValue + '{mValuesSeparator}', 'i');
    itemsElem.value = itemsElem.value.replace(re, '{mValuesSeparator}');
}}

function ProcessItem(chkbox, hash, changeChecked, getHash) {{
    var itemsElem = ItemsElem();
    var items = itemsElem.value;
    var checkHash = '';
    if (chkbox != null) {{
        var itemValue = chkbox.getAttribute('data-value');
        var itemId = chkbox.id.substr(3);
        if (changeChecked) {{
            chkbox.checked = !chkbox.checked;
        }}
        if (chkbox.checked) {{
            if (items == '') {{
                itemsElem.value = '{mValuesSeparator}' + itemValue + '{mValuesSeparator}';
            }}
            else if (items.toLowerCase().indexOf('{mValuesSeparator}' + itemValue.toLowerCase() + '{mValuesSeparator}') < 0) {{
                itemsElem.value += itemValue + '{mValuesSeparator}';
            }}
        }}
        else {{
            unselectItem(itemValue);
        }}

        checkHash = '|' + itemId + '#' + hash;
        disableParents(itemId, chkbox.checked);
        unselectParents(itemId);
    }}
    else {{
        checkHash = '|' + items.replace('{mValuesSeparator}',';') + '#' + hash;
    }}
    if (getHash) {{
        {Page.ClientScript.GetCallbackEventReference(this, "itemsElem.value + checkHash", "SetHash", null)};
    }}
}}
            
function Cancel() {{
    wopener.US_RefreshPage_{parentClientId}(); CloseDialog();
}}

{GetButtonsScript()}";

        return script;
    }


    private string GetButtonsScript()
    {
        var txtClientId = ScriptHelper.GetString(QueryHelper.GetString("txtElem", string.Empty));
        var hdnClientId = ScriptHelper.GetString(QueryHelper.GetString("hidElem", string.Empty));
        var hdnDrpClientId = ScriptHelper.GetString(QueryHelper.GetString("selectElem", string.Empty));
        var hashElementClientId = ScriptHelper.GetString(QueryHelper.GetString("hashElem", string.Empty));

        return $@"
function US_Cancel(){{ Cancel(); return false; }}
function US_Submit(){{ SelectItemsReload(encodeURIComponent(ItemsElem().value).replace(/'/g, '%27'), {hdnClientId}, {txtClientId}, {hdnDrpClientId}, {hashElementClientId}, HashElem().value); return false; }}";
    }

    #endregion


    #region "ICallbackEventHandler Members"

    string ICallbackEventHandler.GetCallbackResult()
    {
        // Prepare the parameters for dialog
        string result = string.Empty;
        if (!string.IsNullOrEmpty(mCallbackValues))
        {
            bool isValid = false;

            string[] values = mCallbackValues.Split(NODE_SEPARATOR);
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
        mCallbackValues = eventArgument;
    }

    #endregion
}