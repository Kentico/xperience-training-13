using System;
using System.Data;
using System.Web;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;

using TreeElemNode = System.Web.UI.WebControls.TreeNode;

public partial class CMSModules_MessageBoards_Content_Properties_Tree : CMSContentMessageBoardsPage
{
    #region "Private variables"

    private string navUrl;
    private string groupNavUrl;
    private int docId;
    private bool selectedSet;
    private string selectedNodeName = String.Empty;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize page
        navUrl = AddParameters(UIContextHelper.GetElementUrl("CMS.MessageBoards", "EditBoards"));
        groupNavUrl = AddParameters(UIContextHelper.GetElementUrl("CMS.MessageBoards", "EditGroupBoards"));
        
        btnDelete.ToolTip = GetString("general.delete");
        btnDelete.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ")) { return false; }";
        btnDelete.Click += btnDelete_Click;

        // Ensure RTL
        treeElem.LineImagesFolder = GetImageUrl(CultureHelper.IsUICultureRTL() ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree", false, true);

        selectedNodeName = QueryHelper.GetString("selectednodename", string.Empty);

        RegisterScripts();
        PopulateTree();
    }


    private string AddParameters(string url)
    {
        url = URLHelper.AddParameterToUrl(url, "changemaster", "0");
        url = URLHelper.AddParameterToUrl(url, "displaytitle", "false");
        url = URLHelper.AddParameterToUrl(url, "showbreadcrumbs", "false");
        url = URLHelper.AddParameterToUrl(url, "tabslayout", "horizontal");

        return url;
    }


    /// <summary>
    /// Button handler.
    /// </summary>
    void btnDelete_Click(object sender, EventArgs e)
    {
        // Check 'Modify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.MessageBoards", "Modify"))
        {
            RedirectToAccessDenied("CMS.MessageBoards", "Modify");
        }

        BoardInfoProvider.DeleteBoardInfo(ValidationHelper.GetInteger(hdnBoardId.Value, 0));
        ltlScript.Text += ScriptHelper.GetScript("parent.frames['main'].location.href = '" + ResolveUrl("~/CMSPages/blank.htm") + "'");
        ltlScript.Text += ScriptHelper.GetScript("window.location.replace(window.location);");
    }


    #region "Private methods"

    /// <summary>
    /// Populates the tree with the data.
    /// </summary>
    private void PopulateTree()
    {
        // Create root node
        TreeElemNode rootNode = new TreeElemNode();
        rootNode.Text = "<span class=\"ContentTreeItem\" \"><span class=\"Name\">" + GetString("board.header.messageboards") + "</span></span>";
        rootNode.Expanded = true;
        treeElem.Nodes.Add(rootNode);

        // Populate the tree
        docId = QueryHelper.GetInteger("documentid", 0);
        if (docId > 0)
        {
            DataSet ds = BoardInfoProvider.GetMessageBoards("BoardDocumentID = " + docId, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    AddNode(Convert.ToString(dr["BoardDisplayName"]), ValidationHelper.GetInteger(dr["BoardId"], -1), ValidationHelper.GetInteger(dr["BoardGroupID"], 0) > 0);
                }
            }
        }
    }


    /// <summary>
    /// Registers all necessary scripts.
    /// </summary>
    private void RegisterScripts()
    {
        ltlScript.Text += ScriptHelper.GetScript(String.Format(
@"
var hiddenField = document.getElementById('{0}');
var currentNode = document.getElementById('treeSelectedNode');
var currentNodeName = '';
treeUrl = '{1}';

function SelectNode(nodeName, nodeElem, boardId, navUrl) {{
    if ((currentNode != null) && (nodeElem != null)) {{
        currentNode.className = 'ContentTreeItem';
    }}
    
    parent.frames['main'].location.href = navUrl + '&boardid=' + boardId;
    currentNodeName = nodeName;

    if (nodeElem != null) {{ 
        currentNode = nodeElem;
        
        if (currentNode != null) {{ 
            currentNode.className = 'ContentTreeSelectedItem';
        }}
    }}
            
    if (hiddenField != null) {{
        hiddenField.value = boardId;
    }}
}}
", 
            hdnBoardId.ClientID, 
            ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/Advanced/MessageBoards/tree.aspx")
        ));
    }


    /// <summary>
    /// Adds node to the root node.
    /// </summary>
    /// <param name="nodeName">Name of node</param>
    /// <param name="boardId">Message board identifier</param>
    /// <param name="group">Indicates if group board</param>
    private void AddNode(string nodeName, int boardId, bool group)
    {
        TreeElemNode newNode = new TreeElemNode();
        string cssClass = "ContentTreeItem";
        string elemId = string.Empty;

        string url = group ? groupNavUrl : navUrl;

        // Select proper node
        if ((!selectedSet) && String.IsNullOrEmpty(selectedNodeName))
        {
            if (!RequestHelper.IsPostBack())
            {
                hdnBoardId.Value = boardId.ToString();
            }
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['main'].location.href = '" + url + "&boardid=" + boardId + "'");
            selectedSet = true;
            cssClass = "ContentTreeSelectedItem";
            elemId = "id=\"treeSelectedNode\"";
        }
        if (selectedNodeName == nodeName)
        {
            cssClass = "ContentTreeSelectedItem";
            elemId = "id=\"treeSelectedNode\"";
        }

        newNode.Text = String.Format(
            "<span class=\"{0}\" {1} onclick=\"SelectNode({2}, this, {3}, '{4}');\"><span class=\"Name\">{5}</span></span>", 
            cssClass,
            elemId,
            ScriptHelper.GetString(HttpUtility.UrlEncode(nodeName)),
            boardId,
            url,
            HTMLHelper.HTMLEncode(nodeName)
        );

        newNode.NavigateUrl = "#";

        treeElem.Nodes[0].ChildNodes.Add(newNode);
    }

    #endregion
}