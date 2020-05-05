using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_Controls_TreeContextMenu : CMSContextMenuControl
{
    #region "Properties"

    /// <summary>
    /// Allows to restrict selection of document types in New... submenu.
    /// </summary>
    public string DocumentTypeWhere
    {
        get;
        set;
    }


    /// <summary>
    /// Allows to explicitly order document types in New... submenu.
    /// </summary>
    public string DocumentTypeOrderBy
    {
        get;
        set;
    } = "ClassDisplayName";


    /// <summary>
    /// Allows to hide Properties menu item.
    /// </summary>
    public bool HidePropertiesItem
    {
        get;
        set;
    }


    /// <summary>
    /// Allows to add additional menu items (ContextMenuItem controls).
    /// </summary>
    public PlaceHolder AdditionalMenuItems
    {
        get
        {
            return plcAdditionalMenuItems;
        }
    }


    /// <summary>
    /// UI elements resource name. UI elements are not evaluated if resource name is empty.
    /// </summary>
    public string ResourceName
    {
        get;
        set;
    } = ModuleName.CONTENT;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        if (ContextMenu != null)
        {
            ContextMenu.Dynamic = true;
            ContextMenu.OnReloadData += ContextMenu_OnReloadData;
        }

        // Check UI permissions if resource name specified
        plcProperties.Visible = (String.IsNullOrEmpty(ResourceName) || CurrentUser.IsAuthorizedPerUIElement(ResourceName, "Properties")) && !HidePropertiesItem;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string loadingContent = new ContextMenuItem { ResourceString = "ContextMenu.Loading" }.GetRenderedHTML();

        menuNew.LoadingContent = loadingContent;
        menuProperties.LoadingContent = loadingContent;

        menuNew.OnReloadData += menuNew_OnReloadData;
        repNew.ItemDataBound += repNew_ItemDataBound;

        // Main menu
        iNew.Attributes.Add("onclick", "NewItem(GetContextMenuParameter('nodeMenu'), true);");

        iDelete.Attributes.Add("onclick", "DeleteItem(GetContextMenuParameter('nodeMenu'), true);");

        iCopy.Attributes.Add("onclick", "CopyRef(GetContextMenuParameter('nodeMenu'));");

        iMove.Attributes.Add("onclick", "MoveRef(GetContextMenuParameter('nodeMenu'));");

        iUp.Attributes.Add("onclick", "MoveUp(GetContextMenuParameter('nodeMenu'));");

        iDown.Attributes.Add("onclick", "MoveDown(GetContextMenuParameter('nodeMenu'));");

        // Refresh subsection
        iRefresh.Attributes.Add("onclick", "RefreshTree(GetContextMenuParameter('nodeMenu'), null);");

        // Sort menu
        iAlphaAsc.Attributes.Add("onclick", "SortAlphaAsc(GetContextMenuParameter('nodeMenu'));");
        iAlphaDesc.Attributes.Add("onclick", "SortAlphaDesc(GetContextMenuParameter('nodeMenu'));");
        iDateAsc.Attributes.Add("onclick", "SortDateAsc(GetContextMenuParameter('nodeMenu'));");
        iDateDesc.Attributes.Add("onclick", "SortDateDesc(GetContextMenuParameter('nodeMenu'));");

        // Up menu
        iTop.Text = GetString("UpMenu.IconTop");
        iTop.Attributes.Add("onclick", "MoveTop(GetContextMenuParameter('nodeMenu'));");

        // Down menu
        iBottom.Text = GetString("DownMenu.IconBottom");
        iBottom.Attributes.Add("onclick", "MoveBottom(GetContextMenuParameter('nodeMenu'));");

        // New menu
        iNewLink.Attributes.Add("onclick", "LinkRef(GetContextMenuParameter('nodeMenu'));");
    }


    protected void repNew_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Panel pnlItem = (Panel)e.Item.FindControl("pnlItem");
        if (pnlItem != null)
        {
            int count = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)["Count"], 0) - 1;
            if (e.Item.ItemIndex == count)
            {
                pnlItem.CssClass = "item-last";
            }

            pnlItem.Attributes.Add("onclick", "NewItem(GetContextMenuParameter('nodeMenu'), " + ((DataRowView)e.Item.DataItem)["ClassID"] + ", true);");
        }
    }

    #endregion


    #region "Events of context menus"

    protected void ContextMenu_OnReloadData(object sender, EventArgs e)
    {
        int nodeId = ValidationHelper.GetInteger(ContextMenu.Parameter, 0);

        // Get the node
        var tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        var node = tree.SelectSingleNode(nodeId);

        if (node != null)
        {
            if (node.IsLink)
            {
                cmcNew.Visible = false;
            }

            if (plcProperties.Visible)
            {
                // Properties menu
                var elements = UIElementInfoProvider.GetChildUIElements("CMS.Content", "Properties");
                if (!DataHelper.DataSourceIsEmpty(elements))
                {
                    var index = 0;
                    UserInfo user = MembershipContext.AuthenticatedUser;

                    foreach (var element in elements)
                    {
                        // Skip elements not relevant for given node
                        if (DocumentUIHelper.IsElementHiddenForNode(element, node))
                        {
                            continue;
                        }

                        var elementName = element.ElementName.ToLowerInvariant();

                        // If UI element is available and user has permission to show it then add it
                        if (UIContextHelper.CheckElementAvailabilityInUI(element) && user.IsAuthorizedPerUIElement(element.ElementResourceID, elementName))
                        {
                            var item = new ContextMenuItem();
                            item.ID = "p" + index;
                            item.Attributes.Add("onclick", String.Format("Properties(GetContextMenuParameter('nodeMenu'), '{0}');", elementName));

                            item.Text = ResHelper.LocalizeString(element.ElementDisplayName);

                            pnlPropertiesMenu.Controls.Add(item);

                            index++;
                        }
                    }

                    if (index == 0)
                    {
                        // Hide 'Properties' menu if user has no permission for at least one properties section
                        plcProperties.Visible = false;
                    }
                }
            }
        }
        else
        {
            iNoNode.Visible = true;
            plcFirstLevelContainer.Visible = false;
        }
    }


    protected void menuNew_OnReloadData(object sender, EventArgs e)
    {
        int nodeId = ValidationHelper.GetInteger(menuNew.Parameter, 0);

        // Get the node
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode node = tree.SelectSingleNode(nodeId);

        if (node != null)
        {
            if (CurrentUser.IsAuthorizedToCreateNewDocument(node, null))
            {
                DocumentTypeScopeInfo scope = DocumentTypeScopeInfoProvider.GetScopeInfo(node);
                if (scope != null)
                {
                    plcNewLink.Visible = scope.ScopeAllowLinks;
                }

                pnlSepNewLinkVariant.Visible = plcNewLink.Visible;

                string where = "ClassID IN (SELECT ChildClassID FROM CMS_AllowedChildClasses WHERE ParentClassID=" + ValidationHelper.GetInteger(node.GetValue("NodeClassID"), 0) + ") " +
                               "AND ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + SiteContext.CurrentSiteID + ")";

                if (!string.IsNullOrEmpty(DocumentTypeWhere))
                {
                    where = SqlHelper.AddWhereCondition(where, DocumentTypeWhere);
                }

                if (scope != null)
                {
                    // Apply document type scope
                    where = SqlHelper.AddWhereCondition(where, DocumentTypeScopeInfoProvider.GetScopeClassWhereCondition(scope).ToString(true));
                }

                // Get the allowed child classes
                DataSet ds = DocumentTypeHelper.GetDocumentTypeClasses()
                    .Where(where)
                    .OrderBy(DocumentTypeOrderBy)
                    .TopN(50)
                    .Columns("ClassID", "ClassName", "ClassDisplayName");

                var rows = new List<DataRow>();

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Check user permissions for "Create" permission
                    bool hasNodeAllowCreate = (CurrentUser.IsAuthorizedPerTreeNode(node, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed);
                    bool isAuthorizedToCreateInContent = CurrentUser.IsAuthorizedPerResource("CMS.Content", "Create");

                    DataTable resultTable = ds.Tables[0].DefaultView.ToTable();

                    for (int i = 0; i < resultTable.Rows.Count; ++i)
                    {
                        DataRow dr = resultTable.Rows[i];
                        string doc = DataHelper.GetStringValue(dr, "ClassName");

                        // Document type is not allowed, remove it from the data set
                        if (!isAuthorizedToCreateInContent && !CurrentUser.IsAuthorizedPerClassName(doc, "Create") && (!CurrentUser.IsAuthorizedPerClassName(doc, "CreateSpecific") || !hasNodeAllowCreate))
                        {
                            rows.Add(dr);
                        }
                    }

                    // Remove the document types
                    foreach (DataRow dr in rows)
                    {
                        resultTable.Rows.Remove(dr);
                    }

                    bool classesRemoved = false;

                    // Leave only first 15 rows
                    while (resultTable.Rows.Count > 15)
                    {
                        resultTable.Rows.RemoveAt(resultTable.Rows.Count - 1);
                        classesRemoved = true;
                    }

                    if (!DataHelper.DataSourceIsEmpty(resultTable))
                    {
                        // Add show more item
                        if (classesRemoved)
                        {
                            DataRow dr = resultTable.NewRow();
                            dr["ClassID"] = 0;
                            dr["ClassName"] = "more";
                            dr["ClassDisplayName"] = ResHelper.GetString("class.showmore");
                            resultTable.Rows.InsertAt(dr, resultTable.Rows.Count);
                        }

                        // Create temp column
                        int rowCount = resultTable.Rows.Count;
                        DataColumn tmpColumn = new DataColumn("Count");
                        tmpColumn.DefaultValue = rowCount;
                        resultTable.Columns.Add(tmpColumn);

                        repNew.DataSource = resultTable;
                        repNew.DataBind();
                    }
                    else
                    {
                        DisplayErrorMessage(scope != null ? "Content.ScopeApplied" : "Content.NoPermissions");
                    }
                }
                else
                {
                    DisplayErrorMessage(scope != null ? "Content.ScopeApplied" : "NewMenu.NoChildAllowed");
                }
            }
            else
            {
                DisplayErrorMessage("Content.NoPermissions");
            }
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Displays error message (if any permission is not present)
    /// </summary>
    /// <param name="message">Message to display</param>
    private void DisplayErrorMessage(String message)
    {
        plcNewLinkVariant.Visible = false;
        iNoChild.Visible = true;

        iNoChild.ResourceString = message;
    }

    #endregion
}