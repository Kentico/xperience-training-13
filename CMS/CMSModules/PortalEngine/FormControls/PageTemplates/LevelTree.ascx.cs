using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_PortalEngine_FormControls_PageTemplates_LevelTree : FormEngineUserControl
{
    #region "Private variables"

    private string mValue = string.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets tree path - if set is created from TreePath.
    /// </summary>
    public string TreePath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Level, levels are rendered only if TreePath is not set. 
    /// </summary>
    public int Level
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (treeElem.Nodes.Count > 0)
            {
                mValue = "";

                TreeNode currentNode = treeElem.Nodes[0];
                while (currentNode != null)
                {
                    if (currentNode.Checked)
                    {
                        mValue += "/{" + currentNode.Value + "}";
                    }

                    if (currentNode.ChildNodes.Count == 0)
                    {
                        break;
                    }
                    currentNode = currentNode.ChildNodes[0];
                }
            }

            return mValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, string.Empty);
        }
    }

    #endregion


    #region "Methods and events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check culture
        if (CultureHelper.IsUICultureRTL())
        {
            treeElem.LineImagesFolder = GetImageUrl("RTL/Design/Controls/Tree", true);
        }
        else
        {
            treeElem.LineImagesFolder = GetImageUrl("Design/Controls/Tree", true);
        }

        // Set tree elements
        treeElem.ToolTip = string.Empty;
        treeElem.ExpandImageToolTip = string.Empty;
        treeElem.ImageSet = TreeViewImageSet.Custom;
        treeElem.ExpandImageToolTip = GetString("ContentTree.Expand");
        treeElem.CollapseImageToolTip = GetString("ContentTree.Collapse");

        // Fill tree and select data
        if (!RequestHelper.IsPostBack())
        {
            FillTree();
            Select();
        }

        RegisterCheckBoxStyleFixScript();
    }


    /// <summary>
    /// Creates tree content.
    /// </summary>
    public void FillTree()
    {
        treeElem.Nodes.Clear();

        if (string.IsNullOrEmpty(ValidationHelper.GetString(TreePath, string.Empty)) && (Level == 0))
        {
            return;
        }
        // Create tree with Level0 to LevelN
        else if (string.IsNullOrEmpty(TreePath))
        {
            CreateLevelTree();
        }

        // Split leafs in tree path
        string[] levels = TreePath.Split('/');

        if (levels.Length > 0)
        {
            string rootName = GetString("InheritLevels.RootName");

            // Own user ID
            if (levels.Length > 1)
            {
                // Begin from index 1, first item is empty string
                if (!string.IsNullOrEmpty(levels[1]))
                {
                    rootName = levels[1];
                }
            }

            TreeNode root = new TreeNode("<span onclick=\"return false;\" class=\"ContentTreeItem\"><span class=\"Name\"><strong>" + HTMLHelper.HTMLEncode(rootName) + "</strong></span></span>", "0");
            root.ToolTip = rootName;
            root.ImageToolTip = string.Empty;
            TreeNode LastNode = root;

            // Insert other nodes
            for (int i = 2; i < levels.Length; i++)
            {
                string currentLevel = levels[i];
                if (ValidationHelper.GetString(currentLevel, "") != "")
                {
                    TreeNode currentNode = new TreeNode("<span onclick=\"return false;\" class=\"ContentTreeItem\"><span class=\"Name\">" + HTMLHelper.HTMLEncode(currentLevel) + "</span></span>", (i - 1).ToString());
                    currentNode.ToolTip = currentLevel;
                    currentNode.ImageToolTip = "";
                    LastNode.ChildNodes.Add(currentNode);
                    LastNode = currentNode;
                }
            }

            treeElem.Nodes.Add(root);
            treeElem.ExpandAll();
        }
    }


    /// <summary>
    /// Select specified documents.
    /// </summary>
    public void Select()
    {
        // Get value
        string selected = ValidationHelper.GetString(mValue, string.Empty);
        if (string.IsNullOrEmpty(selected))
        {
            return;
        }

        // Split value
        string[] levels = selected.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (levels.Length == 0)
        {
            return;
        }

        foreach (string currentLevel in levels)
        {
            int numberLevel = ValidationHelper.GetInteger(currentLevel.Replace("{", "").Replace("}", ""), -1);
            if (numberLevel >= 0)
            {
                TreeNode currentNode = treeElem.Nodes[0];
                while (currentNode != null)
                {
                    if (currentNode.Value == numberLevel.ToString())
                    {
                        currentNode.Checked = true;
                        break;
                    }
                    else
                    {
                        if (currentNode.ChildNodes.Count == 0)
                        {
                            break;
                        }
                        currentNode = currentNode.ChildNodes[0];
                    }
                }
            }
        }
    }


    /// <summary>
    /// Creates path to level tree.
    /// </summary>
    public void CreateLevelTree()
    {
        string levelName = GetString("inheritlevels.levelname");
        string tmpTree = "/" + levelName + "0";

        for (int i = 1; i < Level; i++)
        {
            tmpTree += "/" + levelName + i.ToString();
        }

        TreePath = tmpTree;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        FillTree();
        Select();
    }


    /// <summary>
    /// Register script to fix checkboxes style.
    /// </summary>
    private void RegisterCheckBoxStyleFixScript()
    {
        const string code = @"
var checkboxes = $cmsj('.level-tree-view table input[type=""checkbox""]');
checkboxes.each(function (index) {
    // Get checkbox input and its id
    var checkbox = $cmsj(this),
        checkboxId = checkbox.attr('id'),

        // Get link element and hide it
        td = checkbox.parent(),
        text = td.children('a, span');
    text.addClass('hidden');

    // Create label and add it after checkbox
    var label = '<label for=' + checkboxId + '>' + text.attr('title') + '</label>';
    $cmsj(label).insertAfter(checkbox);

    // Propagate click from label to link (in case of postback, not used currently)
    td.children('label').click(function () { text.click(); });
});";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "leveltreecheckboxstylescript", ScriptHelper.GetScript(code));
    }

    #endregion
}