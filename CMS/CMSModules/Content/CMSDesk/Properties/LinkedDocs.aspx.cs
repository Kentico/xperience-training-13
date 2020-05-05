using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_CMSDesk_Properties_LinkedDocs : CMSPropertiesPage
{
    #region "Protected variables"

    protected string currentSiteName = null;

    private CurrentUserInfo currentUser;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentUser = MembershipContext.AuthenticatedUser;
        if (!currentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.LinkedDocs"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.LinkedDocs");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Node != null)
        {
            // Check modify permissions
            if (!DocumentUIHelper.CheckDocumentPermissions(Node, PermissionsEnum.Modify))
            {
                pnlContainer.Enabled = false;
            }
        }

        currentSiteName = SiteContext.CurrentSiteName.ToLowerCSafe();

        gridDocs.OnExternalDataBound += gridDocuments_OnExternalDataBound;
        gridDocs.OnAction += gridDocs_OnAction;
        gridDocs.OnDataReload += gridDocs_OnDataReload;
        gridDocs.ShowActionsMenu = true;
        gridDocs.Columns = "NodeAliasPath, NodeSiteID, NodeParentID, DocumentName, ClassDisplayName";

        // Get all possible columns to retrieve
        gridDocs.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(PredefinedObjectType.NODE, PredefinedObjectType.DOCUMENTLOCALIZATION));

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Display document information
        DocumentManager.ShowDocumentInfo(false);

        base.OnPreRender(e);
    }


    protected DataSet gridDocs_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        if (Node != null)
        {
            int linkedNodeId = Node.NodeID;

            if (Node.IsLink)
            {
                linkedNodeId = ValidationHelper.GetInteger(Node.GetValue("NodeLinkedNodeID"), 0);
            }

            // Get the documents
            columns = SqlHelper.MergeColumns(DocumentColumnLists.SELECTNODES_REQUIRED_COLUMNS, columns);
            DataSet nodes = DocumentManager.Tree.SelectNodes(TreeProvider.ALL_SITES, "/%", TreeProvider.ALL_CULTURES, true, null, "(NodeID = " + linkedNodeId + " AND NodeLinkedNodeID IS NULL) OR NodeLinkedNodeID = " + linkedNodeId, "NodeAliasPath ASC", -1, false, gridDocs.TopN, columns);
            if (!DataHelper.DataSourceIsEmpty(nodes) && (nodes.Tables[0].Rows.Count > 1))
            {
                return nodes;
            }

            headElem.Visible = false;
            gridDocs.Visible = false;
            lblNoData.Visible = true;
        }

        return null;
    }


    protected void gridDocs_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int deleteNodeId = ValidationHelper.GetInteger(actionArgument, 0);
                if (deleteNodeId > 0)
                {
                    TreeNode deleteNode = DocumentManager.Tree.SelectSingleNode(deleteNodeId, TreeProvider.ALL_CULTURES);
                    if ((deleteNode != null) && (Node != null))
                    {
                        try
                        {
                            // Check user permissions
                            if (IsUserAuthorizedToDeleteDocument(deleteNode))
                            {
                                // Delete the document
                                DocumentHelper.DeleteDocument(deleteNode, DocumentManager.Tree);
                                ShowConfirmation(GetString("LinkedDocs.LinkDeleted"));

                                if ((deleteNode.NodeSiteID == Node.NodeSiteID) && (deleteNode.NodeID == NodeID))
                                {
                                    // When deleting itself, select parent
                                    ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshScript", ScriptHelper.GetScript("SelectItem(" + deleteNode.NodeParentID + ", " + deleteNode.NodeParentID + ");"));
                                }
                                else
                                {
                                    // When deleting from somewhere else, refresh tree
                                    gridDocs.ReloadData();

                                    ScriptHelper.RefreshTree(this, Node.NodeID, deleteNode.NodeParentID);
                                }
                            }
                            else
                            {
                                ShowError(String.Format(GetString("cmsdesk.notauthorizedtodeletedocument"), HTMLHelper.HTMLEncode(deleteNode.GetDocumentName())));
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowError(GetString("ContentRequest.DeleteFailed"), ex.Message);
                        }
                    }
                }
                break;
        }
    }


    /// <summary>
    /// External data binding handler.
    /// </summary>
    protected object gridDocuments_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView data;
        switch (sourceName.ToLowerCSafe())
        {
            case "documentname":
                {
                    data = (DataRowView)parameter;

                    string name = ValidationHelper.GetString(data["NodeAliasPath"], "");
                    string siteName = SiteInfoProvider.GetSiteName(ValidationHelper.GetInteger(data["NodeSiteID"], 0));
                    int currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                    int currentNodeParentId = ValidationHelper.GetInteger(data["NodeParentID"], 0);

                    string result;
                    if (currentSiteName == siteName.ToLowerCSafe())
                    {
                        result = "<a href=\"javascript: SelectItem(" + currentNodeId + ", " + currentNodeParentId + ");\">" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 50)) + "</a>";
                    }
                    else
                    {
                        result = "<span>" + HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 50)) + "</span>";
                    }

                    // Show document mark only if method is not called from grid export
                    bool isLink = (data["NodeLinkedNodeID"] != DBNull.Value);
                    if ((sender != null) && isLink)
                    {
                        result += DocumentUIHelper.GetDocumentMarkImage(this, DocumentMarkEnum.Link);
                    }

                    return result;
                }

            case "documentnametooltip":
                data = (DataRowView)parameter;
                return UniGridFunctions.DocumentNameTooltip(data);

            case "deleteaction":
                {
                    GridViewRow container = (GridViewRow)parameter;
                    int currentNodeId = ValidationHelper.GetInteger(((DataRowView)container.DataItem)["NodeID"], 0);

                    bool current = (Node.NodeID == currentNodeId);
                    ((Control)sender).Visible = ((((DataRowView)container.DataItem)["NodeLinkedNodeID"] != DBNull.Value) && !current);
                    ((CMSGridActionButton)sender).CommandArgument = currentNodeId.ToString();
                    break;
                }
        }
        return parameter;
    }


    /// <summary>
    /// Checks whether the user is authorized to delete document.
    /// </summary>
    /// <param name="node">Document node</param>
    protected bool IsUserAuthorizedToDeleteDocument(TreeNode node)
    {
        // Check delete permission for document
        return (currentUser.IsAuthorizedPerDocument(node, new[] { NodePermissionsEnum.Delete, NodePermissionsEnum.Read }) == AuthorizationResultEnum.Allowed);
    }

    #endregion
}
