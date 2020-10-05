using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;

using IOExceptions = System.IO;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaLibraryTree : CMSAdminControl
{
    /// <summary>
    /// Occurs when selected folder changed.
    /// </summary>
    public delegate void OnFolderSelectedHandler();


    #region "Variables"

    private string mExpandedPath;
    private string mImageFolderPath;
    private int mMaxSubFolders = -1;

    public event OnFolderSelectedHandler OnFolderSelected;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns maximal number of subfolders displayed per parent object 
    /// (if more objects are present, then "cick here for more" is displayed).
    /// </summary>
    private int MaxSubFolders
    {
        get
        {
            if (mMaxSubFolders < 0)
            {
                mMaxSubFolders = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSMediaLibraryMaxSubFolders");

                // Handle negative value
                if (mMaxSubFolders < 0)
                {
                    mMaxSubFolders = 0;
                }
            }
            return mMaxSubFolders;
        }
    }


    /// <summary>
    /// Indicates whether some folder was selected since tree was rendered.
    /// </summary>
    public bool FolderSelected
    {
        get
        {
            return ValidationHelper.GetBoolean(hdnFolderSelected.Value, false);
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value which determines whether the IDs for the folder nodes are generated.
    /// </summary>
    public bool GenerateIDs
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["GenerateIDs"], true);
        }
        set
        {
            ViewState["GenerateIDs"] = value;
        }
    }


    /// <summary>
    /// Indicates whether the control propagates information when max number of root child nodes is exceeded.
    /// </summary>
    public bool CloseListing
    {
        get;
        set;
    }


    /// <summary>
    /// Root folder physical path.
    /// </summary>
    public string RootFolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["RootFolderPath"], null);
        }
        set
        {
            ViewState["RootFolderPath"] = value;
        }
    }


    /// <summary>
    /// Path to the trees images.
    /// </summary>
    public string ImageFolderPath
    {
        get
        {
            if (mImageFolderPath == null)
            {
                bool isRTL = CultureHelper.IsUICultureRTL();
                if (IsLiveSite)
                {
                    isRTL = CultureHelper.IsPreferredCultureRTL();
                }

                mImageFolderPath = GetImageUrl(isRTL ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree", false);
            }
            return mImageFolderPath;
        }
        set
        {
            mImageFolderPath = value;
        }
    }


    /// <summary>
    /// Selected path in treeview.
    /// </summary>
    public string SelectedPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets path to be selected during on load.
    /// </summary>
    public string PathToSelect
    {
        get;
        set;
    }


    /// <summary>
    /// Expand path in treeview.
    /// </summary>
    public string ExpandedPath
    {
        get
        {
            return mExpandedPath;
        }
        set
        {
            ExpandPath(value);
            mExpandedPath = value;
        }
    }


    /// <summary>
    /// ID of the library tree is displayed for.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["MediaLibraryID"], 0);
        }
        set
        {
            ViewState["MediaLibraryID"] = value;
        }
    }


    /// <summary>
    /// Media library folder in root of treeview.
    /// </summary>
    public string MediaLibraryFolder
    {
        get
        {
            return ValidationHelper.GetString(ViewState["MediaLibraryFolder"], null);
        }
        set
        {
            ViewState["MediaLibraryFolder"] = value;
        }
    }


    /// <summary>
    /// Media library path for root of tree within library.
    /// </summary>
    public string MediaLibraryPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["MediaLibraryPath"], null);
        }
        set
        {
            ViewState["MediaLibraryPath"] = value;
        }
    }


    /// <summary>
    /// Indicates if file count should be displayed in folder tree.
    /// </summary>
    public bool DisplayFilesCount
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayFilesCount"], false);
        }
        set
        {
            ViewState["DisplayFilesCount"] = value;
        }
    }


    /// <summary>
    /// Javascript function for custom select handling.
    /// </summary>
    public string CustomSelectFunction
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CustomSelectFunction"], null);
        }
        set
        {
            ViewState["CustomSelectFunction"] = value;
        }
    }


    /// <summary>
    /// Javascript function for custom "click here for more" item handling.
    /// </summary>
    public string CustomClickForMoreFunction
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CustomClickForMoreFunction"], null);
        }
        set
        {
            ViewState["CustomClickForMoreFunction"] = value;
        }
    }


    /// <summary>
    /// Path separator.
    /// </summary>
    public char PathSeparator
    {
        get
        {
            return treeElem.PathSeparator;
        }
    }


    /// <summary>
    /// Indicates whether there is more items than MaxSubFolders under the root node.
    /// </summary>
    public bool RootHasMore
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if root node was loaded succesfully.
    /// </summary>
    public bool RootNodeLoaded
    {
        get;
        private set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeControlScripts();
        InitializeTree();

        if (!StopProcessing && !RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
    }


    protected void treeElem_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        e.Node.ChildNodes.Clear();
        e.Node.PopulateOnDemand = false;

        string nodePath = MediaLibraryHelper.EnsurePhysicalPath(e.Node.ValuePath);

        int rootEnd = nodePath.IndexOfCSafe('\\');
        if (rootEnd > -1)
        {
            nodePath = nodePath.Substring(rootEnd + 1);
        }

        // Bind tree nodes
        if (String.IsNullOrEmpty(MediaLibraryPath))
        {
            BindTreeView(RootFolderPath + DirectoryHelper.CombinePath(MediaLibraryFolder, nodePath), e.Node);
        }
        else
        {
            BindTreeView(DirectoryHelper.CombinePath(RootFolderPath, MediaLibraryPath, nodePath), e.Node);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        try
        {
            treeElem.Nodes.Clear();

            // Get root files
            string rootDir = RootFolderPath;
            if (rootDir != null)
            {
                rootDir = rootDir.TrimEnd('\\');
            }
            if (!String.IsNullOrEmpty(MediaLibraryPath))
            {
                rootDir = DirectoryHelper.CombinePath(rootDir, MediaLibraryPath);
            }
            else if (!String.IsNullOrEmpty(MediaLibraryFolder))
            {
                rootDir = DirectoryHelper.CombinePath(rootDir, MediaLibraryFolder);
            }

            // Get the file and directories count
            int dirCount = 0;

            string[] directories = null;
            if (rootDir != null)
            {
                if (!Directory.Exists(rootDir))
                {
                    DirectoryHelper.EnsureDiskPath(Path.EnsureEndSlash(rootDir), Server.MapPath("~"));
                }
                directories = Directory.GetDirectories(rootDir);
            }

            if (directories != null)
            {
                // Each directory contains directory for thumbnails
                dirCount = directories.Length - 1;
            }

            // Create root tree node
            TreeNode root;
            if (DisplayFilesCount)
            {
                int fileCount = 0;

                string[] files = Directory.GetFiles(rootDir);
                if (files != null)
                {
                    fileCount = files.Length;
                }

                root = CreateNode("<span class=\"Name\">" + MediaLibraryFolder + " (" + fileCount + ")</span>", MediaLibraryFolder, null, dirCount, 0);
            }
            else
            {
                root = CreateNode("<span class=\"Name\">" + MediaLibraryFolder + "</span>", MediaLibraryFolder, null, dirCount, 0);
            }

            // Keep root expanded
            root.Expand();

            // Add root node
            treeElem.Nodes.Add(root);

            if (!StopProcessing)
            {
                // Bind tree nodes
                if (String.IsNullOrEmpty(MediaLibraryPath))
                {
                    BindTreeView(RootFolderPath + MediaLibraryFolder, root, true);
                }
                else
                {
                    BindTreeView(DirectoryHelper.CombinePath(RootFolderPath, MediaLibraryPath), root, true);
                }
            }

            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "RemoveMultipleNodes_" + ClientID, ScriptHelper.GetScript("if (typeof(ResolveDuplicateNodes) != 'undefined') { ResolveDuplicateNodes('" + treeElem.ClientID + "') }"));
        }
        catch (Exception ex)
        {
            // Set error message
            lblError.Text = GetString("FolderTree.FailedLoad") + ": " + ex.Message;
            lblError.ToolTip = EventLogProvider.GetExceptionLogMessage(ex);
        }
        finally
        {
            // Check for root node
            RootNodeLoaded = (treeElem.Nodes.Count > 0);
        }
    }


    /// <summary>
    /// Clears selection of the currently selected node.
    /// </summary>
    public void UnselectNode()
    {
        if (treeElem.SelectedNode != null)
        {
            treeElem.SelectedNode.Selected = false;
        }
    }


    /// <summary>
    /// Selects given path in tree view.
    /// </summary>
    /// <param name="path">Path to select</param>
    public void SelectPath(string path)
    {
        SelectPath(path, true);
    }


    /// <summary>
    /// Selects given path in tree view.
    /// </summary>
    /// <param name="path">Path to select</param>
    /// <param name="folderSelect">Indicates if OnFolderSelect event should be executed</param>
    public void SelectPath(string path, bool folderSelect)
    {
        if (!string.IsNullOrEmpty(path))
        {
            path = EnsurePath(path.TrimEnd().TrimEnd('.'));

            string selectScript = ScriptHelper.GetScript("if (typeof(SelectPath) != 'undefined') { SelectPath('" + path + "'); }");

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "MediaLibraryTree_" + path, selectScript);
        }

        if (folderSelect && (OnFolderSelected != null))
        {
            OnFolderSelected();
        }
    }


    /// <summary>
    /// Expand given path in tree view.
    /// </summary>
    /// <param name="path">Path to expand</param>
    public void ExpandPath(string path)
    {
        TreeNode node = treeElem.FindNode(path);
        if (node != null)
        {
            ExpandParent(node);
        }
    }


    /// <summary>
    /// Checks if the folder structure contains specified folder.
    /// </summary>
    /// <param name="path">Path of the folder to check</param>
    public bool FolderExists(string path)
    {
        string libFolderPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(MediaLibraryID);
        libFolderPath = DirectoryHelper.CombinePath(libFolderPath, path);

        return Directory.Exists(libFolderPath);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets tree control properties.
    /// </summary>
    private void InitializeTree()
    {
        treeElem.SelectedNodeChanged += treeElem_SelectedNodeChanged;
        treeElem.ExpandImageToolTip = GetString("foldertree.expand");
        treeElem.CollapseImageToolTip = GetString("foldertree.collapse");
        treeElem.LineImagesFolder = ImageFolderPath;
        treeElem.ImageSet = TreeViewImageSet.Custom;
        treeElem.PathSeparator = '\\';
    }


    /// <summary>
    /// Ensures all scripts are present.
    /// </summary>
    private void InitializeControlScripts()
    {
        // Make sure jQuery is registered
        ScriptHelper.RegisterJQuery(Page);

        StringBuilder sb = new StringBuilder();

        sb.Append(
            @"
function GetCurrentFolderID() {
    if (currentFolderId != 0) {
        return currentFolderId;
    } else {
        return $cmsj('#", hdnPath.ClientID, @"').val();
    }
}

function FolderSelected() {
    var hdn = document.getElementById('", hdnFolderSelected.ClientID, @"');
    if(hdn != null) {
        hdn.value = 'true';
    }
}
");

        ltlScript.Text = ScriptHelper.GetScript(sb.ToString());

        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/MediaLibrary/Controls/MediaLibrary/MediaLibraryTree.js");
    }


    /// <summary>
    /// Bind tree view.
    /// </summary>
    /// <param name="dirPath">Directory path</param>
    /// <param name="parentNode">Parent node</param>
    /// <param name="isRoot">Indicates if current parentNode is root</param>
    private void BindTreeView(string dirPath, TreeNode parentNode, bool isRoot = false)
    {
        if (Directory.Exists(dirPath))
        {
            string hidenFolder = "\\" + MediaLibraryHelper.GetMediaFileHiddenFolder(SiteContext.CurrentSiteName);

            // Get directories
            string[] dirs = null;
            try
            {
                dirs = Directory.GetDirectories(dirPath);
            }
            catch (Exception exception)
            {
                // Ignore directories that are not accessible
                if (!(exception is UnauthorizedAccessException || exception is IOExceptions.PathTooLongException))
                {
                    Service.Resolve<IEventLogService>().LogException("Media library", "READOBJ", exception);
                }
            }
            if (dirs != null)
            {
                int index = 1;
                foreach (string dir in dirs)
                {
                    if (!dir.EndsWithCSafe(hidenFolder, true))
                    {
                        int dirCount = 0;
                        string[] directories = null;
                        string[] files = null;

                        string text = dir.Substring(dir.LastIndexOfCSafe('\\')).Trim('\\');

                        // Get the files and directories
                        try
                        {
                            if (DisplayFilesCount)
                            {
                                files = Directory.GetFiles(dir);
                            }

                            directories = Directory.GetDirectories(dir);
                            if (directories != null)
                            {
                                dirCount = directories.Length;
                            }
                        }
                        catch (Exception exception)
                        {
                            // Ignore files and directories that are not accessible
                            if (!(exception is UnauthorizedAccessException || exception is IOExceptions.PathTooLongException))
                            {
                                Service.Resolve<IEventLogService>().LogException("Media library", "READOBJ", exception);
                            }
                        }

                        TreeNode node;
                        if (ShowFolder(index))
                        {
                            if (DisplayFilesCount && (files != null))
                            {
                                node = CreateNode("<span class=\"Name\">" + text + " (" + files.Length + ")</span>", text, parentNode, dirCount, index);
                            }
                            else
                            {
                                node = CreateNode("<span class=\"Name\">" + text + "</span>", text, parentNode, dirCount, index);
                            }

                            if (dirCount == 1)
                            {
                                node.PopulateOnDemand = !directories[0].EndsWithCSafe(hidenFolder);
                            }
                            else
                            {
                                node.PopulateOnDemand = (dirCount > 0);
                            }

                            // Check if there is node within the current path to be selected
                            if (!string.IsNullOrEmpty(PathToSelect))
                            {
                                EnsureNodeExpand(parentNode, node);
                            }

                            parentNode.ChildNodes.Add(node);
                        }
                        else
                        {
                            node = CreateNode("<span class=\"Name\">" + GetString("general.seelisting") + "</span>", "", parentNode, 0, index);
                            parentNode.ChildNodes.Add(node);

                            RootHasMore = (isRoot && !CloseListing);
                        }

                        if (ShowFolder(index))
                        {
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Ensures that necessary path is expanded when selecting specific node.
    /// </summary>
    /// <param name="parentNode">Parent of the node which is supposed to be a part of the selected path</param>
    /// <param name="node">Node which should be a part of the slected path</param>
    private void EnsureNodeExpand(TreeNode parentNode, TreeNode node)
    {
        // Get normalized path of the current node
        string nodePathLowered = (parentNode.ValuePath + '/' + node.Value).Replace(treeElem.PathSeparator, '/').ToLowerCSafe();
        string pathToSelectLowered = PathToSelect.Replace(treeElem.PathSeparator, '/').ToLowerCSafe();

        // If current node is part of the selected path
        if (pathToSelectLowered != nodePathLowered)
        {
            // Ensure path endings for compare
            string nodePath = nodePathLowered + '/';
            string pathToSelect = pathToSelectLowered + '/';
            if (pathToSelect.StartsWithCSafe(nodePath))
            {
                // Get the folder path of the current node and last folder in its path
                string nodeFolder = Path.GetDirectoryName(nodePathLowered);
                string nodeLastFolder = Path.GetFileName(nodePathLowered);

                // Get the folder path of the folder to select and destination folder name
                string pathFolder = Path.GetDirectoryName(pathToSelectLowered);
                string pathLastFolder = Path.GetFileName(pathToSelectLowered);

                // Check if the current node is in the same directory as selected folder while differs just in name
                bool isDestDir = (nodeFolder == pathFolder);
                bool isDestFolder = (nodeLastFolder == pathLastFolder);
                bool isNamedSame = (isDestDir && !isDestFolder);

                // Do not expand node that has same name as selected folder (while its name differs only in suffix)
                node.Expanded = !isNamedSame;
            }
        }
    }


    /// <summary>
    /// Expand parent node.
    /// </summary>
    /// <param name="node">Tree node</param>
    private void ExpandParent(TreeNode node)
    {
        if (node.Parent != null)
        {
            node.Parent.Expand();
            ExpandParent(node.Parent);
        }
        else
        {
            node.Expand();
        }
    }


    /// <summary>
    /// Creates a HTML node used in the tree.
    /// </summary>
    /// <param name="nodeText">Text opf the node</param>
    /// <param name="nodeValue">Path of the node in the tree</param>
    /// <param name="parrentNode">Parent node element</param>
    /// <param name="childCount">Number of child elements (folders)</param>
    /// <param name="index">Index of the currently processed node</param>
    private TreeNode CreateNode(string nodeText, string nodeValue, TreeNode parrentNode, int childCount, int index)
    {
        TreeNode node = new TreeNode();

        if (!String.IsNullOrEmpty(CustomSelectFunction))
        {
            string output = "<span ";

            string val = nodeValue;
            if (parrentNode != null)
            {
                val = parrentNode.ValuePath + treeElem.PathSeparator + nodeValue;
            }

            // IDs
            string folderId = "";
            if (GenerateIDs)
            {
                folderId = EnsurePath(val);

                output += "id=\"" + folderId + "\" ";
            }

            node.SelectAction = TreeNodeSelectAction.None;
            if (ShowFolder(index))
            {
                output += "onClick=\" document.getElementById('" + hdnPath.ClientID + "').value = '" + folderId + "'; ";
                if (!ShowFolder(childCount) && !String.IsNullOrEmpty(CustomClickForMoreFunction))
                {
                    output += CustomClickForMoreFunction.Replace("##NODEVALUE##", val.Replace('\\', '|').Replace("\'", "\\\'")).Replace("##FOLDERID##", folderId).Replace("##TYPE##", "folder") + " return false;\" ";
                }
                else
                {
                    output += CustomSelectFunction.Replace("##NODEVALUE##", val.Replace('\\', '|').Replace("\'", "\\\'")).Replace("##FOLDERID##", folderId) + " return false;\" ";
                }
            }
            else if (!string.IsNullOrEmpty(CustomClickForMoreFunction))
            {
                string parentFolderId = "";
                if (GenerateIDs)
                {
                    parentFolderId = EnsurePath(parrentNode.ValuePath);
                }

                output += "onClick=\" document.getElementById('" + hdnPath.ClientID + "').value = '" + parentFolderId.Replace("\\", "\\\\").Replace("\'", "\\\'") + "'; ";
                output += CustomClickForMoreFunction.Replace("##NODEVALUE##", parrentNode.ValuePath.Replace('\\', '|').Replace("\'", "\\\'")).Replace("##FOLDERID##", parentFolderId).Replace("##TYPE##", "link") + " return false;\" ";
            }

            output += ">";
            output += nodeText;
            output += "</span>";
            node.Text = output;
        }
        else
        {
            node.Text = nodeText;
        }
        node.Value = nodeValue;

        return node;
    }


    /// <summary>
    /// Indicates if folder should be shown.
    /// </summary>
    /// <param name="index">Position of folder</param>
    private bool ShowFolder(int index)
    {
        return (index <= MaxSubFolders) || (MaxSubFolders <= 0);
    }


    /// <summary>
    /// Ensures that path contains only characters acceptable by jQuery selector.
    /// </summary>
    /// <param name="path">Path</param>
    private string EnsureFolderId(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            int hash = path.GetHashCode();
            char[] specialChars = "#;&,.+*~':\"!^$[]()=>|/\\-%@`{}".ToCharArray();
            foreach (char specialChar in specialChars)
            {
                path = path.Replace(specialChar, '_');
            }
            return path.Replace(" ", "") + "_" + hash;
        }
        return path;
    }


    /// <summary>
    /// Ensures that the path is ready to be used as an node ID.
    /// </summary>
    /// <param name="path">Path to use</param>
    private string EnsurePath(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            path = Path.EnsureForwardSlashes(path);
            path = EnsureFolderId(path.ToLowerCSafe());
        }
        return path;
    }

    #endregion


    #region "TreeView events"

    protected void treeElem_SelectedNodeChanged(object sender, EventArgs e)
    {
        if (OnFolderSelected != null)
        {
            OnFolderSelected();
        }
    }

    #endregion
}
