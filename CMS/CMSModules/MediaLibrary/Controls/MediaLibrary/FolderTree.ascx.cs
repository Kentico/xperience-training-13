using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderTree : CMSAdminControl
{
    /// <summary>
    /// Occurs when selected folder changed.
    /// </summary>
    public delegate void OnFolderSelectedHandler();


    #region "Variables"

    private string mRootFolderPath;
    private string mSelectedPath;
    private string mExpandedPath;
    private string mImageFolderPath;
    private string mMediaLibraryFolder;
    private string mMediaLibraryPath;
    private bool mDisplayFilesCount;
    private bool mDisplayFolderIcon = true;
    private bool mIgnoreAccessDenied;
    private bool mCloseListing;
    private bool mRootHasMore;
    private string mCustomSelectFunction;
    private string mCustomClickForMoreFunction;
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

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the value which determines whether to display icon of folder in tree.
    /// </summary>
    public bool DisplayFolderIcon
    {
        get
        {
            return mDisplayFolderIcon;
        }
        set
        {
            mDisplayFolderIcon = value;
        }
    }


    /// <summary>
    /// Indicates whether the control should try to bind as much folders as available ignoring access denied exceptions.
    /// </summary>
    public bool IgnoreAccessDenied
    {
        get
        {
            return mIgnoreAccessDenied;
        }
        set
        {
            mIgnoreAccessDenied = value;
        }
    }


    /// <summary>
    /// Indicates whether the control propagates information when max number of root child nodes is exceeded.
    /// </summary>
    public bool CloseListing
    {
        get
        {
            return mCloseListing;
        }
        set
        {
            mCloseListing = value;
        }
    }


    /// <summary>
    /// Root folder physical path.
    /// </summary>
    public string RootFolderPath
    {
        get
        {
            return mRootFolderPath;
        }
        set
        {
            mRootFolderPath = value;
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
                if (CultureHelper.IsUICultureRTL())
                {
                    mImageFolderPath = GetImageUrl("RTL/Design/Controls/Tree", true);
                }
                else
                {
                    mImageFolderPath = GetImageUrl("Design/Controls/Tree", true);
                }
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
        get
        {
            if (treeElem.SelectedNode != null)
            {
                // If not selected root node
                if (treeElem.SelectedNode != treeElem.Nodes[0])
                {
                    // Return path without library folder and \
                    mSelectedPath = treeElem.SelectedNode.ValuePath.Substring(treeElem.Nodes[0].Value.Length + 1);
                }
                else
                {
                    mSelectedPath = "";
                }
            }
            return mSelectedPath;
        }
        set
        {
            mSelectedPath = value;
        }
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
    /// Media library folder in root of treeview.
    /// </summary>
    public string MediaLibraryFolder
    {
        get
        {
            return mMediaLibraryFolder;
        }
        set
        {
            mMediaLibraryFolder = value;
        }
    }


    /// <summary>
    /// Media library path for root of tree within library.
    /// </summary>
    public string MediaLibraryPath
    {
        get
        {
            return mMediaLibraryPath;
        }
        set
        {
            mMediaLibraryPath = value;
        }
    }


    /// <summary>
    /// Indicates if file count should be displayed in folder tree.
    /// </summary>
    public bool DisplayFilesCount
    {
        get
        {
            return mDisplayFilesCount;
        }
        set
        {
            mDisplayFilesCount = value;
        }
    }


    /// <summary>
    /// Javascript function for custom select handling.
    /// </summary>
    public string CustomSelectFunction
    {
        get
        {
            return mCustomSelectFunction;
        }
        set
        {
            mCustomSelectFunction = value;
        }
    }


    /// <summary>
    /// Javascript function for custom "click here for more" item handling.
    /// </summary>
    public string CustomClickForMoreFunction
    {
        get
        {
            return mCustomClickForMoreFunction;
        }
        set
        {
            mCustomClickForMoreFunction = value;
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
        get
        {
            return mRootHasMore;
        }
        set
        {
            mRootHasMore = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            treeElem.LineImagesFolder = ImageFolderPath;
            treeElem.ImageSet = TreeViewImageSet.Custom;
            treeElem.ExpandDepth = 1;
            treeElem.PathSeparator = '\\';
            treeElem.SelectedNodeChanged += treeElem_SelectedNodeChanged;

            if (!RequestHelper.IsPostBack())
            {
                ReloadData();
            }
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
            rootDir = DirectoryHelper.CombinePath(rootDir, !String.IsNullOrEmpty(MediaLibraryPath) ? MediaLibraryPath : MediaLibraryFolder);

            // Get the file and directories count
            int dirCount = 0;
            int fileCount = 0;

            string[] files = null;
            string[] directories = null;
            if ((rootDir != null) && (Directory.Exists(rootDir)))
            {
                files = Directory.GetFiles(rootDir);
                directories = Directory.GetDirectories(rootDir);
            }

            if (files != null)
            {
                fileCount = files.Length;
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
        catch (Exception ex)
        {
            if (!IgnoreAccessDenied)
            {
                lblError.Text = GetString("FolderTree.FailedLoad") + ": " + ex.Message;
                lblError.ToolTip = EventLogProvider.GetExceptionLogMessage(ex);
            }
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
        if (path != null)
        {
            TreeNode node = treeElem.FindNode(path.ToLowerCSafe());
            if (node != null)
            {
                ExpandParent(node);
                node.Select();
                if ((folderSelect) && (OnFolderSelected != null))
                {
                    OnFolderSelected();
                }
            }
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

    #endregion


    #region "Private methods"

    /// <summary>
    /// Bind tree view.
    /// </summary>
    /// <param name="dirPath">Directory path</param>
    /// <param name="parentNode">Parent node</param>
    /// <param name="isRoot">Indicates if root node</param>
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
            catch (Exception ex)
            {
                if (!IgnoreAccessDenied)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            if (dirs != null)
            {
                int index = 1;
                foreach (string dir in dirs)
                {
                    if (!dir.EndsWithCSafe(hidenFolder, true))
                    {
                        // Add node
                        string text = dir.Substring(dir.LastIndexOfCSafe('\\')).Trim('\\');
                        string[] files = null;
                        int dirCount = 0;

                        // Get the files and directories
                        try
                        {
                            files = Directory.GetFiles(dir);

                            string[] directories = Directory.GetDirectories(dir);
                            if (directories != null)
                            {
                                dirCount = directories.Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!IgnoreAccessDenied)
                            {
                                throw new Exception(ex.Message, ex);
                            }
                        }

                        TreeNode node;
                        if (index <= MaxSubFolders)
                        {
                            if (DisplayFilesCount && (files != null))
                            {
                                node = CreateNode("<span class=\"Name\">" + text + " (" + files.Length + ")</span>", text, parentNode, files.Length + dirCount, index);
                            }
                            else
                            {
                                node = CreateNode("<span class=\"Name\">" + text + "</span>", text, parentNode, dirCount, index);
                            }
                            parentNode.ChildNodes.Add(node);

                            // Recursive bind
                            BindTreeView(dir, node);
                        }
                        else if (!IsLiveSite)
                        {
                            // Render 'more' node only if not LiveSite
                            node = CreateNode("<span class=\"Name\">" + GetString("general.seelisting") + "</span>", "", parentNode, 0, index);
                            parentNode.ChildNodes.Add(node);

                            RootHasMore = (isRoot && !CloseListing);
                        }

                        if (index <= MaxSubFolders)
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


    private TreeNode CreateNode(string nodeText, string nodeValue, TreeNode parentNode, int childCount, int index)
    {
        TreeNode node = new TreeNode();

        if (!String.IsNullOrEmpty(CustomSelectFunction))
        {
            string output = "<span ";
            string val = nodeValue;
            if (parentNode != null)
            {
                val = parentNode.ValuePath.ToLowerCSafe() + treeElem.PathSeparator + nodeValue.ToLowerCSafe();
            }
            node.SelectAction = TreeNodeSelectAction.None;
            if (index <= MaxSubFolders)
            {
                output += "onClick=\" document.getElementById('" + hdnPath.ClientID + "').value = '" + val.Replace("\\", "\\\\") + "'; ";
                if ((childCount > MaxSubFolders) && !String.IsNullOrEmpty(CustomClickForMoreFunction))
                {
                    output += CustomClickForMoreFunction.Replace("##NODEVALUE##", val.Replace('\\', '|')) + " return false;\" ";
                }
                else
                {
                    output += CustomSelectFunction.Replace("##NODEVALUE##", val.Replace('\\', '|')) + " return false;\" ";
                }
            }
            else if (!string.IsNullOrEmpty(CustomClickForMoreFunction) && (parentNode != null))
            {
                output += "onClick=\" document.getElementById('" + hdnPath.ClientID + "').value = '" + parentNode.ValuePath.Replace("\\", "\\\\") + "'; ";
                output += CustomClickForMoreFunction.Replace("##NODEVALUE##", parentNode.ValuePath.Replace('\\', '|')) + " return false;\" ";
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
        node.Value = nodeValue.ToLowerCSafe();

        return node;
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