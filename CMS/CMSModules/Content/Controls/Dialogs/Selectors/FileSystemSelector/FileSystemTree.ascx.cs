using System;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_FileSystemTree : CMSUserControl
{
    #region "Variables"

    private string mStartingPath = "";
    private string mDefaultPath = "";
    private string mAllowedFolders = "";
    private string mExcludedFolders = "";

    private string mNodeTextTemplate = "##ICON####NODENAME##";
    private string mSelectedNodeTextTemplate = "##ICON####NODENAME##";
    private string mNodeValue = "";
    private string mBasePath;
    private int mMaxTreeNodes;
    private string mMaxTreeNodeText;
    private bool mDeniedNodePostback = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, zip folders are shown
    /// </summary>
    public bool AllowZipFolders
    {
        get;
        set;
    }


    /// <summary>
    /// Starting path of the tree.
    /// </summary>
    public string StartingPath
    {
        get
        {
            return mStartingPath;
        }
        set
        {
            mStartingPath = value;
        }
    }


    /// <summary>
    /// Path to selected tree node in tree.
    /// </summary>
    public string DefaultPath
    {
        get
        {
            mDefaultPath = ValidationHelper.GetString(ViewState["TreeDefaultPath"], "");
            return mDefaultPath;
        }
        set
        {
            mDefaultPath = Path.EnsureSlashes(value);
            ViewState["TreeDefaultPath"] = mDefaultPath;
        }
    }


    /// <summary>
    /// Determines if default path node should be expanded.
    /// </summary>
    public bool ExpandDefaultPath
    {
        get;
        set;
    }


    /// <summary>
    /// List of folders which should be displayed, separated with semicolon.
    /// </summary>
    public string AllowedFolders
    {
        get
        {
            return mAllowedFolders.ToLowerCSafe();
        }
        set
        {
            mAllowedFolders = value;
        }
    }


    /// <summary>
    /// List of files excluded from tree, separated with semicolon.
    /// </summary>
    public string ExcludedFolders
    {
        get
        {
            return mExcludedFolders.ToLowerCSafe();
        }
        set
        {
            mExcludedFolders = value;
        }
    }


    /// <summary>
    /// Maximum number of tree nodes displayed within the tree.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            if (mMaxTreeNodes <= 0)
            {
                mMaxTreeNodes = 100; //SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSMaxTreeNodes");
            }
            return mMaxTreeNodes;
        }
        set
        {
            mMaxTreeNodes = value;
        }
    }


    /// <summary>
    /// Text to appear within the latest node when max tree nodes applied.
    /// </summary>
    public string MaxTreeNodeText
    {
        get
        {
            return mMaxTreeNodeText ?? (mMaxTreeNodeText = GetString("general.SeeListing"));
        }
        set
        {
            mMaxTreeNodeText = value;
        }
    }


    /// <summary>
    /// Gets or sets the current node value.
    /// </summary>
    public string NodeValue
    {
        get
        {
            return mNodeValue;
        }
        set
        {
            mNodeValue = value;
        }
    }


    /// <summary>
    /// Template of the node text, use {0} to insert the original node text, {1} to insert the Node ID.
    /// </summary>
    public string NodeTextTemplate
    {
        get
        {
            return mNodeTextTemplate;
        }
        set
        {
            mNodeTextTemplate = value;
        }
    }


    /// <summary>
    /// Template of the SelectedNode text, use {0} to insert the original SelectedNode text, {1} to insert the SelectedNode ID.
    /// </summary>
    public string SelectedNodeTextTemplate
    {
        get
        {
            return mSelectedNodeTextTemplate;
        }
        set
        {
            mSelectedNodeTextTemplate = value;
        }
    }


    /// <summary>
    /// Indicates whether access denied node causes postback.
    /// </summary>
    public bool DeniedNodePostback
    {
        get
        {
            return mDeniedNodePostback;
        }
        set
        {
            mDeniedNodePostback = value;
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Gets number of children under specified item.
    /// </summary>
    /// <param name="dirInfo">Directory info of processed folder</param>
    private int GetAllowedChildNumber(DirectoryInfo dirInfo)
    {
        int counter = 0;
        try
        {
            // Add directories
            var dirs = dirInfo.GetDirectories();

            foreach (DirectoryInfo dir in dirs)
            {
                string lowerFullName = dir.FullName.ToLowerCSafe();
                if (IsAllowed(lowerFullName) && !IsExcluded(lowerFullName))
                {
                    counter++;
                }
            }

            // Zip files acts the same way as directories
            if (AllowZipFolders)
            {
                var childZips = dirInfo.GetFiles("*.zip");
                counter += childZips.Length;
            }

            return counter;
        }
        catch (Exception)
        {
            return 0;
        }
    }


    /// <summary>
    /// Gets full starting path inspecting possible relative path specification.
    /// </summary>
    private string FullStartingPath
    {
        get
        {
            if (UsingRelativeURL())
            {
                return Server.MapPath(StartingPath).TrimEnd('\\');
            }
            else
            {
                if (StartingPath.EndsWithCSafe(":\\"))
                {
                    return StartingPath;
                }
            }
            return StartingPath.TrimEnd('\\');
        }
    }


    /// <summary>
    /// Check if relative paths are used.
    /// </summary>
    /// <returns>True if relative paths are used</returns>
    public bool UsingRelativeURL()
    {
        if (StartingPath.StartsWithCSafe("~"))
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Determines if specified folder under root is allowed or not.
    /// </summary>
    /// <param name="folderName">Path to folder</param>
    /// <returns>True if folder is allowed</returns>
    private bool IsAllowed(string folderName)
    {
        if (String.IsNullOrEmpty(AllowedFolders) || (folderName.ToLowerCSafe() == FullStartingPath.ToLowerCSafe()))
        {
            return true;
        }

        foreach (string folder in AllowedFolders.Split(';'))
        {
            if (folderName.StartsWithCSafe(DirectoryHelper.CombinePath(FullStartingPath.ToLowerCSafe(), folder)))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Determines if specified folder under root is excluded or not.
    /// </summary>
    /// <param name="folderName">Path to folder</param>
    /// <returns>True if folder is excluded</returns>
    private bool IsExcluded(string folderName)
    {
        if (String.IsNullOrEmpty(ExcludedFolders))
        {
            return false;
        }

        foreach (string folder in ExcludedFolders.Split(';'))
        {
            if (folderName.ToLowerCSafe().EqualsCSafe(Path.Combine(FullStartingPath.ToLowerCSafe(), folder), true))
            {
                return true;
            }
        }

        return false;
    }

    #endregion


    #region "Control events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mBasePath = RequestContext.URL.LocalPath;

        var imagesPath = CultureHelper.IsUICultureRTL() ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree";

        treeFileSystem.LineImagesFolder = GetImageUrl(imagesPath, true);

        treeFileSystem.ImageSet = TreeViewImageSet.Custom;
        treeFileSystem.ExpandImageToolTip = GetString("ContentTree.Expand");
        treeFileSystem.CollapseImageToolTip = GetString("ContentTree.Collapse");
        treeFileSystem.ShowLines = true;


        if (!RequestHelper.IsCallback())
        {
            ScriptHelper.RegisterTreeProgress(Page);
        }
    }


    /// <summary>
    /// Pre render.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if ((!RequestHelper.IsCallback() && !RequestHelper.IsPostBack()))
        {
            ReloadData();
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Reload control data.
    /// </summary>
    public void ReloadData()
    {
        try
        {
            treeFileSystem.Nodes.Clear();
            InitializeTree();

            // Expand current node parent
            if (!String.IsNullOrEmpty(DefaultPath))
            {
                if (!DefaultPath.ToLowerCSafe().StartsWithCSafe(FullStartingPath.ToLowerCSafe().TrimEnd('\\')))
                {
                    DefaultPath = DirectoryHelper.CombinePath(FullStartingPath, DefaultPath);
                }

                if (!String.IsNullOrEmpty(ExcludedFolders))
                {
                    foreach (string excludedFolder in ExcludedFolders.Split(';'))
                    {
                        if (DefaultPath.ToLowerCSafe().StartsWithCSafe((DirectoryHelper.CombinePath(FullStartingPath, excludedFolder)).ToLowerCSafe()))
                        {
                            DefaultPath = FullStartingPath;
                            break;
                        }
                    }
                }

                string preselectedPath = DefaultPath;
                string rootPath = treeFileSystem.Nodes[0].Value;

                if (preselectedPath.ToLowerCSafe().StartsWithCSafe(rootPath.ToLowerCSafe()))
                {
                    TreeNode parent = treeFileSystem.Nodes[0];

                    string[] folders = preselectedPath.ToLowerCSafe().Substring(rootPath.Length).Split('\\');
                    int index = 0;
                    string path = rootPath.ToLowerCSafe() + folders[index];


                    foreach (string folder in folders)
                    {
                        foreach (TreeNode node in parent.ChildNodes)
                        {
                            if (node.Value.ToLowerCSafe() == path)
                            {
                                parent = node;
                                break;
                            }
                        }

                        if (index < folders.Length - 1)
                        {
                            parent.Expand();
                            path += '\\' + folders[index + 1];
                        }
                        else
                        {
                            if (ExpandDefaultPath)
                            {
                                parent.Expand();
                            }
                        }

                        index++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = GetString("ContentTree.FailedLoad");

            Service.Resolve<IEventLogService>().LogException("ContentTree", "LOAD", ex, SiteContext.CurrentSiteID);
        }
    }


    /// <summary>
    /// Tree initialization.
    /// </summary>
    private void InitializeTree()
    {
        // Create root element
        DirectoryInfo di;
        if (Directory.Exists(FullStartingPath))
        {
            di = DirectoryInfo.New(FullStartingPath);
        }
        else
        {
            try
            {
                // Ensure not existing starting path
                DirectoryHelper.EnsureDiskPath(FullStartingPath + "\\", SystemContext.WebApplicationPhysicalPath);
                di = DirectoryInfo.New(FullStartingPath);
            }
            catch
            {
                throw new Exception("Directory " + FullStartingPath + " not found and could not be created.");
            }
        }

        TreeNode root = CreateNode(di, 0);
        root.Expand();
        root.PopulateOnDemand = false;

        treeFileSystem.Nodes.Add(root);
    }


    /// <summary>
    /// Creation of new tree folder node.
    /// </summary>
    /// <param name="dirInfo">Folder information</param>
    /// <param name="index">Index in tree to check if max number of item isn't exceeded</param>
    /// <returns>Created node</returns>
    protected TreeNode CreateNode(DirectoryInfo dirInfo, int index)
    {
        if (dirInfo == null)
        {
            return null;
        }

        string fullName = dirInfo.FullName;
        string lowerFullName = fullName.ToLowerCSafe();

        TreeNode newNode = null;

        if (IsAllowed(lowerFullName) && !IsExcluded(lowerFullName))
        {
            newNode = new TreeNode();

            string name = dirInfo.Name;
            DirectoryInfo parent = dirInfo.Parent;

            // Check if node is part of preselected path
            string preselectedPath = DefaultPath;
            if (!DefaultPath.ToLowerCSafe().StartsWithCSafe(FullStartingPath.ToLowerCSafe().TrimEnd('\\')))
            {
                preselectedPath = DirectoryHelper.CombinePath(FullStartingPath, DefaultPath);
            }

            if (index == MaxTreeNodes)
            {
                newNode.Value = "";
                newNode.Text = MaxTreeNodeText.Replace("##PARENTNODEID##", ((parent == null) ? "" : parent.FullName.Replace("\\", "\\\\").Replace("'", "\\'")));
                newNode.NavigateUrl = mBasePath + "#";
            }
            else if ((index < MaxTreeNodes) || preselectedPath.ToLowerCSafe().StartsWithCSafe(lowerFullName))
            {
                newNode.Value = fullName;
                newNode.NavigateUrl = mBasePath + "#";

                string nodeName = HttpUtility.HtmlEncode(name);
                string nodeNameJava = ScriptHelper.GetString(nodeName);

                string preSel = FullStartingPath.TrimEnd('\\').ToLowerCSafe();
                if (DefaultPath.ToLowerCSafe().StartsWithCSafe(FullStartingPath.ToLowerCSafe().TrimEnd('\\')))
                {
                    preSel = DefaultPath.ToLowerCSafe();
                }
                else if (!String.IsNullOrEmpty(DefaultPath))
                {
                    preSel = DirectoryHelper.CombinePath(preSel, DefaultPath.ToLowerCSafe());
                }


                if ((preSel != "") && (newNode.Value.ToLowerCSafe() == preSel))
                {
                    newNode.Text = SelectedNodeTextTemplate.Replace("##NODENAMEJAVA##", nodeNameJava).Replace("##NODENAME##", nodeName).Replace("##ICON##", "").Replace("##NODEID##", newNode.Value.Replace("\\", "\\\\").Replace("'", "\\'"));
                }
                else
                {
                    newNode.Text = NodeTextTemplate.Replace("##NODENAMEJAVA##", nodeNameJava).Replace("##NODENAME##", nodeName).Replace("##ICON##", "").Replace("##NODEID##", newNode.Value.Replace("\\", "\\\\").Replace("'", "\\'"));
                }

                int childNodesCount = 0;
                try
                {
                    childNodesCount = ValidationHelper.GetInteger(GetAllowedChildNumber(dirInfo), 0);
                    if (childNodesCount == 0)
                    {
                        newNode.PopulateOnDemand = false;
                        newNode.Expanded = true;
                    }
                    else
                    {
                        newNode.PopulateOnDemand = true;
                        newNode.Expanded = false;
                    }
                }
                catch
                {
                    // Access error
                    newNode.PopulateOnDemand = false;
                    newNode.Expanded = true;
                }
                finally
                {
                    newNode.Text = newNode.Text.Replace("##NODECHILDNODESCOUNT##", childNodesCount.ToString());
                }
            }
        }

        return newNode;
    }


    /// <summary>
    /// Node populating.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Particular node arguments</param>
    protected void treeFileSystem_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        e.Node.ChildNodes.Clear();
        e.Node.PopulateOnDemand = false;

        try
        {
            var path = e.Node.Value;

            DirectoryInfo dirInfo = DirectoryInfo.New(path);
            if (dirInfo.Exists)
            {
                // Get the child directories
                DirectoryInfo[] childDirs = dirInfo.GetDirectories();

                for (int i = 0, index = 0; i < childDirs.Length; i++)
                {
                    TreeNode newNode = CreateNode(childDirs[i], index);
                    if (newNode != null)
                    {
                        e.Node.ChildNodes.Add(newNode);

                        // More content node was inserted
                        if (newNode.Value == "")
                        {
                            return;
                        }

                        index++;
                    }
                }

                // Get the zip directories
                if (AllowZipFolders)
                {
                    var childZips = dirInfo.GetFiles("*.zip");

                    for (int i = 0, index = 0; i < childZips.Length; i++)
                    {
                        // Convert zip file to directory
                        FileInfo zipFile = childZips[i];
                        string fileName = ZipStorageProvider.GetZipFileName(zipFile.Name);
                        string parentDir = zipFile.Directory.FullName;

                        DirectoryInfo zipDir = DirectoryInfo.New(parentDir + "\\" + fileName);

                        TreeNode newNode = CreateNode(zipDir, index);
                        if (newNode != null)
                        {
                            e.Node.ChildNodes.Add(newNode);

                            // More content node was inserted
                            if (newNode.Value == "")
                            {
                                return;
                            }

                            index++;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error
            Service.Resolve<IEventLogService>().LogException("FileSystemTree", "POPULATE", ex);
        }
    }

    #endregion
}
