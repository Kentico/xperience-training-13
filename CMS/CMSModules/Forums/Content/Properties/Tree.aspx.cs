using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

using TreeElemNode = System.Web.UI.WebControls.TreeNode;


public partial class CMSModules_Forums_Content_Properties_Tree : CMSForumsPage
{
    #region "Private variables"

    private string navUrl;
    private int docId;
    private bool selectedSet;
    private string selectedNodeName = String.Empty;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize page
        navUrl = UIContextHelper.GetElementUrl("CMS.Forums", "EditForum");
        navUrl = URLHelper.AddParameterToUrl(navUrl, "changemaster", "1");
        navUrl = URLHelper.AddParameterToUrl(navUrl, "showbreadcrumbs", "false");

        btnDelete.Visible = CurrentUser.IsAuthorizedPerResource("cms.forums", CMSAdminControl.PERMISSION_MODIFY, CurrentSiteName);
        btnDelete.ToolTip = GetString("general.delete");
        btnDelete.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ");";
        btnDelete.Click += btnDelete_Click;

        // Ensure RTL
        if (CultureHelper.IsUICultureRTL())
        {
            treeElem.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree");
        }
        else
        {
            treeElem.LineImagesFolder = GetImageUrl("Design/Controls/Tree");
        }

        selectedNodeName = QueryHelper.GetString("selectednodename", string.Empty);

        RegisterScripts();
        PopulateTree();
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    void btnDelete_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", CMSAdminControl.PERMISSION_MODIFY))
        {
            return;
        }

        ForumInfoProvider.DeleteForumInfo(ValidationHelper.GetInteger(hdnForumId.Value, 0));
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
        rootNode.Text = "<span class=\"ContentTreeItem\" \"><span class=\"Name\">" + GetString("forum.header.forum") + "</span></span>";
        rootNode.Expanded = true;
        treeElem.Nodes.Add(rootNode);

        // Populate the tree
        docId = QueryHelper.GetInteger("documentid", 0);
        if (docId > 0)
        {
            DataSet ds = ForumInfoProvider.GetForums().WhereEquals("ForumDocumentID", docId);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    AddNode(Convert.ToString(dr["ForumDisplayName"]), ValidationHelper.GetInteger(dr["ForumID"], -1));
                }
            }
        }
    }


    /// <summary>
    /// Registers all necessary scripts.
    /// </summary>
    private void RegisterScripts()
    {
        ltlScript.Text += ScriptHelper.GetScript(
            "var hiddenField = document.getElementById('" + hdnForumId.ClientID + "');" +
            "var currentNode = document.getElementById('treeSelectedNode');" +
            "var currentNodeName = \"\";" +
            "" +
            "treeUrl = '" + ResolveUrl("~/CMSModules/Content/CMSDesk/Properties/Advanced/Forums/tree.aspx") + "';" +
            "function SelectForumNode(nodeName, nodeElem, forumId)" +
            "{" +
            "    if ((currentNode != null) && (nodeElem != null))" +
            "    {" +
            "        currentNode.className = 'ContentTreeItem';" +
            "    }" +
            "    " +
            "    parent.frames['main'].location.href = '" + navUrl + "&forumid=' + forumId;" +
            "    currentNodeName = nodeName;" +
            "    " +
            "    if (nodeElem != null)" +
            "    {" +
            "        currentNode = nodeElem;" +
            "        if (currentNode != null)" +
            "        {" +
            "            currentNode.className = 'ContentTreeSelectedItem';" +
            "        }" +
            "    }" +
            "    if (hiddenField != null) {" +
            "        hiddenField.value = forumId;" +
            "    }" +
            "}");
    }


    /// <summary>
    /// Adds node to the root node.
    /// </summary>
    /// <param name="nodeName">Name of node</param>
    /// <param name="forumId">ID of forum</param>
    private void AddNode(string nodeName, int forumId)
    {
        TreeElemNode newNode = new TreeElemNode();
        string cssClass = "ContentTreeItem";
        string elemId = string.Empty;

        // Select proper node
        if ((!selectedSet) && String.IsNullOrEmpty(selectedNodeName))
        {
            if (!RequestHelper.IsPostBack())
            {
                hdnForumId.Value = forumId.ToString();
            }
            ltlScript.Text += ScriptHelper.GetScript("parent.frames['main'].location.href = '" + navUrl + "&forumid=" + forumId + "'");
            selectedSet = true;
            cssClass = "ContentTreeSelectedItem";
            elemId = "id=\"treeSelectedNode\"";
        }
        if (selectedNodeName == nodeName)
        {
            cssClass = "ContentTreeSelectedItem";
            elemId = "id=\"treeSelectedNode\"";
        }
        newNode.Text = "<span class=\"" + cssClass + "\" " + elemId + " onclick=\"SelectForumNode(" + ScriptHelper.GetString(nodeName) + ", this, " + forumId + ");\"><span class=\"Name\">" + HTMLHelper.HTMLEncode(nodeName) + "</span></span>";
        newNode.NavigateUrl = "#";

        treeElem.Nodes[0].ChildNodes.Add(newNode);
    }

    #endregion
}