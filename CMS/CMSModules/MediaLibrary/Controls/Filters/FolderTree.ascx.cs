using System;
using System.Web;
using System.Web.UI;

using CMS.Base;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.MediaLibrary.Web.UI;


public partial class CMSModules_MediaLibrary_Controls_Filters_FolderTree : FolderTree
{
    #region "Private properties"

    /// <summary>
    /// Currently selected path.
    /// </summary>
    override public string SelectedPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SelectedPath"], (FilterMethod == 0 ? QueryHelper.GetString(PathQueryStringKey, null) : null));
        }
        set
        {
            ViewState["SelectedPath"] = value;
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        string path = null;
        // If filter by query parameters
        if (FilterMethod == 0)
        {
            path = QueryHelper.GetString(PathQueryStringKey, "");
        }
        else
        {
            // Check if media file is set and try get file path
            int fileId = GetFileID();
            if (fileId > 0)
            {
                MediaFileInfo mfi = MediaFileInfoProvider.GetMediaFileInfo(fileId);
                if (mfi != null)
                {
                    // Get folder path from media file info object
                    path = Path.GetDirectoryName(mfi.FilePath);
                }
            }
            else
            {
                path = RemoveRoot(SelectedPath);
            }
        }

        if (String.IsNullOrEmpty(path))
        {
            // Select root in tree view
            folderTree.SelectPath(MediaLibraryFolder, false);
        }
        else
        {
            // Select folder in tree view
            folderTree.SelectPath(DirectoryHelper.CombinePath(MediaLibraryFolder, path), false);
        }

        base.OnPreRender(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (FilterMethod == 1)
        {
            // Root by default
            if (SelectedPath == null)
            {
                SelectedPath = MediaLibraryFolder;
            }
        }
        // If postback is from Folder tree
        if (ValidationHelper.GetString(Request.Params[Page.postEventSourceID], String.Empty).StartsWithCSafe(folderTree.UniqueID))
        {
            // Update information on currently selected path
            string selectedPath = ValidationHelper.GetString(Request.Params[Page.postEventArgumentID], String.Empty).ToLowerCSafe();
            if (selectedPath != "")
            {
                // Remove library root
                SelectedPath = selectedPath.Remove(0, 1);
            }
            if (GetFileID() > 0)
            {
                string url = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, FileIDQueryStringKey);
                if (FilterMethod == 0)
                {
                    url = URLHelper.UpdateParameterInUrl(url, PathQueryStringKey, GetPathForQuery(SelectedPath));
                }
                URLHelper.Redirect(url);
            }
        }

        SetupControls();
    }


    /// <summary>
    /// Setup controls.
    /// </summary>
    private void SetupControls()
    {
        if (StopProcessing)
        {
            folderTree.Visible = false;
            folderTree.StopProcessing = true;
            return;
        }

        if (SourceFilterControl != null)
        {
            SourceFilterControl.OnFilterChanged += FilterControl_OnFilterChanged;
        }
        SourceFilterName = SourceFilterName;
        folderTree.MediaLibraryFolder = MediaLibraryFolder;
        folderTree.MediaLibraryPath = MediaLibraryPath;
        folderTree.ImageFolderPath = ImageFolderPath;
        folderTree.RootFolderPath = RootFolderPath;
        folderTree.ExpandedPath = ExpandPath;
        folderTree.DisplayFilesCount = DisplayFileCount;
        folderTree.OnFolderSelected += folderTree_OnFolderSelected;

        if (!string.IsNullOrEmpty(SelectedPath))
        {
            folderTree.SelectPath(SelectedPath, true);
        }

        int fid = GetFileID();
        if (fid == 0)
        {
            SetFilter();
        }
        if (!RequestHelper.IsPostBack())
        {
            // Filter changed event
            RaiseOnFilterChanged();
        }
    }


    /// <summary>
    /// OnFilterChange handler.
    /// </summary>
    private void FilterControl_OnFilterChanged()
    {
        OrderBy = SourceFilterControl.OrderBy;
        WhereCondition = Where;
        // Raise change event
        RaiseOnFilterChanged();
    }


    /// <summary>
    /// Returns FileID from query string.
    /// </summary>
    private int GetFileID()
    {
        return QueryHelper.GetInteger(FileIDQueryStringKey, 0);
    }


    /// <summary>
    /// Sets filters where condition according to selected path in folder tree.
    /// </summary>
    private void SetFilter()
    {
        string path;
        if (FilterMethod != 0)
        {
            // Filter by postback
            path = RemoveRoot(SelectedPath);
        }
        else
        {
            // Filter by query parameter
            path = QueryHelper.GetString(PathQueryStringKey, "");
        }

        // If in library root
        if (String.IsNullOrEmpty(MediaLibraryPath))
        {
            if (String.IsNullOrEmpty(path))
            {
                // Select only files from root folder
                WhereCondition = "(Filepath LIKE N'%')";
                CurrentFolder = "";

                if (!ShowSubfoldersContent)
                {
                    // Select only files from root folder
                    WhereCondition += " AND (Filepath NOT LIKE N'%/%')";
                }
            }
            else
            {
                // Escape ' and [ (spacial character for LIKE condition)
                string wPath = Path.EnsureSlashes(path).Replace("'", "''").Replace("[", "[[]");
                // Get files from path
                WhereCondition = String.Format("(FilePath LIKE N'{0}/%')", wPath);
                CurrentFolder = Path.EnsureSlashes(path);

                if (!ShowSubfoldersContent)
                {
                    // But no from subfolders
                    WhereCondition += String.Format(" AND (FilePath NOT LIKE N'{0}/%/%')", wPath);
                }
            }
        }
        else
        {
            if (String.IsNullOrEmpty(path))
            {
                // Escape ' and [ (spacial character for LIKE condition)
                string wPath = Path.EnsureSlashes(MediaLibraryPath).Replace("'", "''").Replace("[", "[[]");
                // Select files from path folder
                WhereCondition = String.Format("(Filepath LIKE N'{0}/%')", wPath);
                CurrentFolder = Path.EnsureSlashes(MediaLibraryPath);

                if (!ShowSubfoldersContent)
                {
                    // Select only files from path folder
                    WhereCondition += String.Format(" AND (Filepath NOT LIKE N'{0}/%/%')", wPath);
                }
            }
            else
            {
                // Escape ' and [ (spacial character for LIKE condition)
                string wPath = Path.EnsureSlashes(String.Format("{0}/{1}", MediaLibraryPath, path)).Replace("'", "''").Replace("[", "[[]");
                // Get files from path
                WhereCondition = String.Format("(FilePath LIKE N'{0}/%')", wPath);
                CurrentFolder = String.Format("{0}/{1}", Path.EnsureSlashes(MediaLibraryPath), Path.EnsureSlashes(path));
                if (!ShowSubfoldersContent)
                {
                    // But no from subfolders
                    WhereCondition += String.Format(" AND (FilePath NOT LIKE N'{0}/%/%')", wPath);
                }
            }
        }
        Where = WhereCondition;
    }


    private void folderTree_OnFolderSelected()
    {
        if (FilterMethod == 0)
        {
            string url = URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, FileIDQueryStringKey);
            string path = GetPathForQuery(SelectedPath);
            url = URLHelper.UpdateParameterInUrl(url, PathQueryStringKey, path);

            URLHelper.Redirect(url);
        }
        else
        {
            SetFilter();
            RaiseOnFilterChanged();
        }
    }


    /// <summary>
    /// Gets path without root folder.
    /// </summary>
    /// <param name="path">Path to be unrooted</param>
    private static string RemoveRoot(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            int rootEnd = path.IndexOfCSafe('\\');
            return ((rootEnd > -1) ? path.Substring(rootEnd + 1) : "");
        }

        return path;
    }


    /// <summary>
    /// Returns folder path encoded for query string.
    /// </summary>
    /// <param name="path">Folder path</param>
    private string GetPathForQuery(string path)
    {
        string noRootPath = RemoveRoot(path);

        if (!String.IsNullOrEmpty(noRootPath))
        {
            noRootPath = HttpUtility.UrlEncode(noRootPath);

            // Escape special characters from path
            noRootPath = noRootPath.Replace("&", "%26").Replace("#", "%23");
        }
        return noRootPath;
    }
}