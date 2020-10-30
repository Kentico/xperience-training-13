using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_CopyMoveFolder : CMSModalPage
{
    #region "Variables"

    private Hashtable mParameters;

    private MediaLibraryInfo mLibraryInfo;
    private SiteInfo mLibrarySiteInfo;
    private string mLibraryRootFolder;
    private string mLibraryPath = "";
    private bool mAllFiles;

    #endregion


    #region "Private properties"

    /// <summary>
    /// ID of the media library.
    /// </summary>
    private int MediaLibraryID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets current library info.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if ((mLibraryInfo == null) && (MediaLibraryID > 0))
            {
                mLibraryInfo = MediaLibraryInfo.Provider.Get(MediaLibraryID);
            }
            return mLibraryInfo;
        }
    }


    /// <summary>
    /// Gets info on site library belongs to.
    /// </summary>
    private SiteInfo LibrarySiteInfo
    {
        get
        {
            if ((mLibrarySiteInfo == null) && (LibraryInfo != null))
            {
                mLibrarySiteInfo = SiteInfo.Provider.Get(LibraryInfo.LibrarySiteID);
            }
            return mLibrarySiteInfo;
        }
    }


    /// <summary>
    /// Type of the action.
    /// </summary>
    private string CopyMoveAction
    {
        get;
        set;
    }


    /// <summary>
    /// Media library Folder path.
    /// </summary>
    private string FolderPath
    {
        get;
        set;
    }


    /// <summary>
    /// Media library root folder path.
    /// </summary>
    private string RootFolder
    {
        get;
        set;
    }


    /// <summary>
    /// Path where the item(s) should be copied/moved.
    /// </summary>
    private string AsyncNewPath
    {
        get
        {
            return ctlAsyncLog.ProcessData.Data as string ?? "";
        }
        set
        {
            ctlAsyncLog.ProcessData.Data = value;
            ctlAsyncLog.ProcessData.AllowUpdateThroughPersistentMedium = true;
        }
    }


    /// <summary>
    /// Path where the item(s) should be copied/moved.
    /// </summary>
    private string NewPath
    {
        get;
        set;
    }


    /// <summary>
    /// List of files to copy/move.
    /// </summary>
    private string Files
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether all files should be copied.
    /// </summary>
    private bool AllFiles
    {
        get
        {
            return mAllFiles;
        }
        set
        {
            mAllFiles = value;
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }


    /// <summary>
    /// Current Info.
    /// </summary>
    private string CurrentInfo
    {
        get
        {
            return ctlAsyncLog.ProcessData.Information;
        }
        set
        {
            ctlAsyncLog.ProcessData.Information = value;
        }
    }


    /// <summary>
    /// Indicates whether the properties are just loaded - no folder was previously selected.
    /// </summary>
    private bool IsLoad
    {
        get
        {
            return ValidationHelper.GetBoolean(Parameters["load"], false);
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Returns media library root folder path.
    /// </summary>
    public string LibraryRootFolder
    {
        get
        {
            if ((LibrarySiteInfo != null) && (mLibraryRootFolder == null))
            {
                mLibraryRootFolder = MediaLibraryHelper.GetMediaRootFolderPath(LibrarySiteInfo.SiteName);
            }
            return mLibraryRootFolder;
        }
    }


    /// <summary>
    /// Gets library relative url path.
    /// </summary>
    public string LibraryPath
    {
        get
        {
            if (String.IsNullOrEmpty(mLibraryPath))
            {
                if (LibraryInfo != null)
                {
                    mLibraryPath = LibraryRootFolder + LibraryInfo.LibraryFolder;
                }
            }
            return mLibraryPath;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        SetRTL();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        // Check if hashtable containing dialog parameters is not empty
        if ((Parameters == null) || (Parameters.Count == 0))
        {
            return;
        }

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsyncLog_OnFinished;
        ctlAsyncLog.OnError += ctlAsyncLog_OnError;
        ctlAsyncLog.OnCancel += ctlAsyncLog_OnCancel;

        // Get the source node
        MediaLibraryID = ValidationHelper.GetInteger(Parameters["libraryid"], 0);
        CopyMoveAction = ValidationHelper.GetString(Parameters["action"], string.Empty);
        FolderPath = Path.EnsureSlashes(ValidationHelper.GetString(Parameters["path"], ""));
        Files = ValidationHelper.GetString(Parameters["files"], "").Trim('|');
        RootFolder = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName);
        AllFiles = ValidationHelper.GetBoolean(Parameters["allFiles"], false);
        NewPath = Path.EnsureSlashes(ValidationHelper.GetString(Parameters["newpath"], ""));

        // Target folder
        string tarFolder = NewPath;
        if (string.IsNullOrEmpty(tarFolder) && (LibraryInfo != null))
        {
            tarFolder = LibraryInfo.LibraryFolder + " (root)";
        }
        lblFolder.Text = tarFolder;

        if (!IsLoad)
        {
            if (AllFiles || String.IsNullOrEmpty(Files))
            {
                if (AllFiles)
                {
                    lblFilesToCopy.ResourceString = "media.folder.filestoall" + CopyMoveAction.ToLowerCSafe();
                }
                else
                {
                    lblFilesToCopy.ResourceString = "media.folder.folderto" + CopyMoveAction.ToLowerCSafe();
                }

                // Source folder
                string srcFolder = FolderPath;
                if (string.IsNullOrEmpty(srcFolder) && (LibraryInfo != null))
                {
                    srcFolder = LibraryInfo.LibraryFolder + "&nbsp;(root)";
                }
                lblFileList.Text = HTMLHelper.HTMLEncode(srcFolder);
            }
            else
            {
                lblFilesToCopy.ResourceString = "media.folder.filesto" + CopyMoveAction.ToLowerCSafe();
                string[] fileList = Files.Split('|');
                foreach (string file in fileList)
                {
                    lblFileList.Text += HTMLHelper.HTMLEncode(DirectoryHelper.CombinePath(FolderPath.TrimEnd('\\'), file)) + "<br />";
                }
            }

            if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
            {
                bool performAction = ValidationHelper.GetBoolean(Parameters["performaction"], false);
                if (performAction)
                {
                    // Perform Move or Copy
                    PerformAction();
                }
            }

            pnlInfo.Visible = true;
            pnlEmpty.Visible = false;
        }
        else
        {
            pnlInfo.Visible = false;
            pnlEmpty.Visible = true;
            lblEmpty.Text = GetString("media.copymove.select");

            // Disable New folder button
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "DisableNewFolderOnLoad", ScriptHelper.GetScript("if ((window.parent != null) && window.parent.DisableNewFolderBtn) { window.parent.DisableNewFolderBtn(); }"));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Moves document.
    /// </summary>
    private void PerformAction(object parameter)
    {
        AddLog(GetString(CopyMoveAction.ToLowerCSafe() == "copy" ? "media.copy.startcopy" : "media.move.startmove"));

        if (LibraryInfo != null)
        {
            // Library path (used in recursive copy process)
            string libPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(SiteContext.CurrentSiteName, LibraryInfo.LibraryFolder);

            // Ensure libPath is in original path type
            libPath = Path.GetFullPath(libPath);

            // Original path on disk from query
            string origPath = Path.GetFullPath(DirectoryHelper.CombinePath(libPath, FolderPath));

            // Original path in DB
            string origDBPath = Path.EnsureForwardSlashes(FolderPath);

            // New path in DB
            string newDBPath;

            AddLog(NewPath);

            // Check if requested folder is in library root folder
            if (!origPath.StartsWithCSafe(libPath, true))
            {
                CurrentError = GetString("media.folder.nolibrary");
                AddLog(CurrentError);
                return;
            }

            string origFolderName = Path.GetFileName(origPath);

            if ((String.IsNullOrEmpty(Files) && !mAllFiles) && string.IsNullOrEmpty(origFolderName))
            {
                NewPath = NewPath + "\\" + LibraryInfo.LibraryFolder;
                NewPath = NewPath.Trim('\\');
            }

            // New path on disk
            string newPath = NewPath;

            // Process current folder copy/move action
            if (String.IsNullOrEmpty(Files) && !AllFiles)
            {
                newPath = Path.EnsureEndSlash(newPath) + origFolderName;
                newPath = newPath.Trim('\\');

                // Check if moving into same folder
                if ((CopyMoveAction.ToLowerCSafe() == "move") && (newPath == FolderPath))
                {
                    CurrentError = GetString("media.move.foldermove");
                    AddLog(CurrentError);
                    return;
                }

                // Error if moving folder into itself
                string newRootPath = Path.GetDirectoryName(newPath).Trim();
                string newSubRootFolder = Path.GetFileName(newPath).ToLowerCSafe().Trim();
                string originalSubRootFolder = Path.GetFileName(FolderPath).ToLowerCSafe().Trim();
                if (String.IsNullOrEmpty(Files) && (CopyMoveAction.ToLowerCSafe() == "move") && newPath.StartsWithCSafe(Path.EnsureEndSlash(FolderPath))
                    && (originalSubRootFolder == newSubRootFolder) && (newRootPath == FolderPath))
                {
                    CurrentError = GetString("media.move.movetoitself");
                    AddLog(CurrentError);
                    return;
                }

                try
                {
                    // Get unique path for copy or move
                    string path = Path.GetFullPath(DirectoryHelper.CombinePath(libPath, newPath));
                    path = MediaLibraryHelper.EnsureUniqueDirectory(path);
                    newPath = path.Remove(0, (libPath.Length + 1));

                    // Get new DB path
                    newDBPath = Path.EnsureForwardSlashes(newPath.Replace(Path.EnsureEndSlash(libPath), ""));
                }
                catch (Exception ex)
                {
                    CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                    Service.Resolve<IEventLogService>().LogException("MediaFolder", CopyMoveAction, ex);
                    AddLog(CurrentError);
                    return;
                }
            }
            else
            {
                origDBPath = Path.EnsureForwardSlashes(FolderPath);
                newDBPath = Path.EnsureForwardSlashes(newPath.Replace(libPath, "")).Trim('/');
            }

            // Error if moving folder into its subfolder
            if ((String.IsNullOrEmpty(Files) && !AllFiles) && (CopyMoveAction.ToLowerCSafe() == "move") && newPath.StartsWithCSafe(Path.EnsureEndSlash(FolderPath)))
            {
                CurrentError = GetString("media.move.parenttochild");
                AddLog(CurrentError);
                return;
            }

            // Error if moving files into same directory
            if ((!String.IsNullOrEmpty(Files) || AllFiles) && (CopyMoveAction.ToLowerCSafe() == "move") && (newPath.TrimEnd('\\') == FolderPath.TrimEnd('\\')))
            {
                CurrentError = GetString("media.move.fileserror");
                AddLog(CurrentError);
                return;
            }

            NewPath = newPath;
            AsyncNewPath = newPath;

            // If mFiles is empty handle directory copy/move
            if (String.IsNullOrEmpty(Files) && !mAllFiles)
            {
                try
                {
                    switch (CopyMoveAction.ToLowerCSafe())
                    {
                        case "move":
                            MediaLibraryInfoProvider.MoveMediaLibraryFolder(SiteContext.CurrentSiteName, MediaLibraryID, origDBPath, newDBPath);
                            break;

                        case "copy":
                            MediaLibraryInfoProvider.CopyMediaLibraryFolder(SiteContext.CurrentSiteName, MediaLibraryID, origDBPath, newDBPath, CurrentUser.UserID);
                            break;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    CurrentError = GetString("general.erroroccurred") + " " + GetString("media.security.accessdenied");
                    Service.Resolve<IEventLogService>().LogException("MediaFolder", CopyMoveAction, ex);
                    AddLog(CurrentError);
                }
                catch (ThreadAbortException ex)
                {
                    if (CMSThread.Stopped(ex))
                    {
                        // When canceled
                        CurrentInfo = GetString("general.actioncanceled");
                        AddLog(CurrentInfo);
                    }
                    else
                    {
                        // Log error
                        CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                        Service.Resolve<IEventLogService>().LogException("MediaFolder", CopyMoveAction, ex);
                        AddLog(CurrentError);
                    }
                }
                catch (Exception ex)
                {
                    CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                    Service.Resolve<IEventLogService>().LogException("MediaFolder", CopyMoveAction, ex);
                    AddLog(CurrentError);
                }
            }
            else
            {
                string origDBFilePath;
                string newDBFilePath;

                if (!mAllFiles)
                {
                    try
                    {
                        string[] files = Files.Split('|');
                        foreach (string filename in files)
                        {
                            origDBFilePath = (string.IsNullOrEmpty(origDBPath)) ? filename : origDBPath + "/" + filename;
                            newDBFilePath = (string.IsNullOrEmpty(newDBPath)) ? filename : newDBPath + "/" + filename;
                            AddLog(filename);
                            CopyMove(origDBFilePath, newDBFilePath);
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        CurrentError = GetString("general.erroroccurred") + " " + GetString("media.security.accessdenied");
                        Service.Resolve<IEventLogService>().LogException("MediaFile", CopyMoveAction, ex);
                        AddLog(CurrentError);
                    }
                    catch (ThreadAbortException ex)
                    {
                        if (CMSThread.Stopped(ex))
                        {
                            // When canceled
                            CurrentInfo = GetString("general.actioncanceled");
                            AddLog(CurrentInfo);
                        }
                        else
                        {
                            // Log error
                            CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                            Service.Resolve<IEventLogService>().LogException("MediaFile", CopyMoveAction, ex);
                            AddLog(CurrentError);
                        }
                    }
                    catch (Exception ex)
                    {
                        CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                        Service.Resolve<IEventLogService>().LogException("MediaFile", CopyMoveAction, ex);
                        AddLog(CurrentError);
                    }
                }
                else
                {
                    var fileNames = GetFileNames().ToList();
                    if (!fileNames.Any())
                    {
                        return;
                    }

                    foreach (string fileName in fileNames)
                    {
                        AddLog(fileName);

                        origDBFilePath = (string.IsNullOrEmpty(origDBPath)) ? fileName : origDBPath + "/" + fileName;
                        newDBFilePath = (string.IsNullOrEmpty(newDBPath)) ? fileName : newDBPath + "/" + fileName;

                        try
                        {
                            CopyMove(origDBFilePath, newDBFilePath);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            CurrentError = GetString("general.erroroccurred") + " " + GetString("media.security.accessdenied");
                            Service.Resolve<IEventLogService>().LogException("MediaFile", CopyMoveAction, ex);
                            AddLog(CurrentError);
                            return;
                        }
                        catch (ThreadAbortException ex)
                        {
                            if (CMSThread.Stopped(ex))
                            {
                                // When canceled
                                CurrentInfo = GetString("general.actioncanceled");
                                AddLog(CurrentInfo);
                            }
                            else
                            {
                                // Log error
                                CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                                Service.Resolve<IEventLogService>().LogException("MediaFile", CopyMoveAction, ex);
                                AddLog(CurrentError);
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            CurrentError = GetString("general.erroroccurred") + " " + ex.Message;
                            Service.Resolve<IEventLogService>().LogException("MediaFile", CopyMoveAction, ex);
                            AddLog(CurrentError);
                            return;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Performs the Move of Copy action.
    /// </summary>
    public void PerformAction()
    {
        if (!IsLoad)
        {
            if (CheckPermissions())
            {
                pnlInfo.Visible = true;
                pnlEmpty.Visible = false;

                if (CopyMoveAction.ToLowerCSafe() == "copy")
                {
                    ctlAsyncLog.TitleText = GetString("media.copy.startcopy");
                }
                else
                {
                    ctlAsyncLog.TitleText = GetString("media.move.startmove");
                }
                RunAsync(PerformAction);
            }
        }
        else
        {
            pnlInfo.Visible = false;
            pnlEmpty.Visible = true;
            lblEmpty.Text = GetString("media.copymove.noselect");
        }
    }


    /// <summary>
    /// Performs action itself.
    /// </summary>
    /// <param name="origDBFilePath">Path of the file specified in DB</param>
    /// <param name="newDBFilePath">New path of the file being inserted into DB</param>
    private void CopyMove(string origDBFilePath, string newDBFilePath)
    {
        switch (CopyMoveAction.ToLowerCSafe())
        {
            case "move":
                MediaFileInfoProvider.MoveMediaFile(SiteContext.CurrentSiteName, MediaLibraryID, origDBFilePath, newDBFilePath);
                break;

            case "copy":
                MediaFileInfoProvider.CopyMediaFile(SiteContext.CurrentSiteName, MediaLibraryID, origDBFilePath, newDBFilePath, false, CurrentUser.UserID);
                break;
        }
    }

    #endregion


    #region "Help methods"

    /// <summary>
    /// Returns list of filenames in the file system.
    /// </summary>
    private IEnumerable<string> GetFileNames()
    {
        var path = LibraryPath + "/" + Path.EnsureForwardSlashes(FolderPath) + "/";
        path = Path.EnsureSlashes(path).Replace("|", "\\");

        IEnumerable<FileInfo> files = GetFileInfos(path, "*.*");
        var filenames = new List<string>();

        if (files != null)
        {
            filenames = files.Where(f => f.Exists)
                .OrderBy(f => f.FullName)
                .Select(x => x.Name)
                .ToList();
        }

        return filenames;
    }


    private static IEnumerable<FileInfo> GetFileInfos(string path, string filter)
    {
        DirectoryInfo di = DirectoryInfo.New(path);

        FileInfo[] files = di.GetFiles(filter, SearchOption.TopDirectoryOnly);
        return files;
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Check permissions for selected library.
    /// </summary>
    private bool CheckPermissions()
    {
        // If mFiles is empty handle directory copy/move
        if (String.IsNullOrEmpty(Files) && !mAllFiles)
        {
            if (CopyMoveAction.ToLowerCSafe().Trim() == "copy")
            {
                // Check 'Folder create' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "foldercreate"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("foldercreate"));
                    return false;
                }
            }
            else
            {
                // Check 'Folder modify' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "foldermodify"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("foldermodify"));
                    return false;
                }
            }
        }
        else
        {
            if (CopyMoveAction.ToLowerCSafe().Trim() == "copy")
            {
                // Check 'File create' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filecreate"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filecreate"));
                    return false;
                }
            }
            else
            {
                // Check 'File modify' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "filemodify"))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage("filemodify"));
                    return false;
                }
            }
        }
        return true;
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsyncLog_OnCancel(object sender, EventArgs e)
    {
        CurrentInfo = (CopyMoveAction.ToLowerCSafe() == "copy") ? GetString("media.copy.canceled") : GetString("media.move.canceled");
        AddLog(CurrentInfo);

        pnlLog.Visible = false;
        pnlInfo.Visible = true;

        AddScript("var __pendingCallbacks = new Array();DestroyLog();");

        ShowConfirmation(CurrentInfo);
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnError(object sender, EventArgs e)
    {
        pnlLog.Visible = false;
        pnlInfo.Visible = true;

        AddScript("DestroyLog();");
        HandlePossibleErrors();
    }


    private void ctlAsyncLog_OnFinished(object sender, EventArgs e)
    {
        Refresh();
    }


    private void Refresh()
    {
        var newPath = AsyncNewPath;

        var refreshScript = @"
var topWin = GetTop();
if (topWin) {
    if ((topWin.opener) && (typeof(topWin.opener.RefreshLibrary) != 'undefined')) {
        topWin.opener.RefreshLibrary(" + ScriptHelper.GetString(newPath.Replace('\\', '|')) + @");
    } 
    else if ((topWin.wopener) && (typeof(topWin.wopener.RefreshLibrary) != 'undefined')) { 
        topWin.wopener.RefreshLibrary(" + ScriptHelper.GetString(newPath.Replace('\\', '|')) + @"); 
    } 
    CloseDialog();
}";

        AddScript(!HandlePossibleErrors() ? refreshScript : "DestroyLog();");
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Runs async thread.
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        pnlLog.Visible = true;
        pnlInfo.Visible = false;

        CurrentError = string.Empty;
        CurrentInfo = string.Empty;

        AddScript("InitializeLog();");

        ctlAsyncLog.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Ensures any error or info is displayed to user.
    /// </summary>
    /// <returns>True if error occurred.</returns>
    protected bool HandlePossibleErrors()
    {
        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
            AddScript("var __pendingCallbacks = new Array();DestroyLog();");

            pnlLog.Visible = false;
            pnlInfo.Visible = true;

            return true;
        }

        return false;
    }

    #endregion
}
