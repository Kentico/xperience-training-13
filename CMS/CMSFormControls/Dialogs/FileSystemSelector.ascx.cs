using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;

public partial class CMSFormControls_Dialogs_FileSystemSelector : FormEngineUserControl, ICallbackEventHandler
{
    #region "Variables"

    private FileSystemDialogConfiguration mDialogConfig;
    private bool mEnabled = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selector value: path of the file or folder.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtPath.Text;
        }
        set
        {
            txtPath.Text = value != null ? value.ToString() : String.Empty;
        }
    }


    /// <summary>
    /// Configuration of the dialog for inserting Images.
    /// </summary>
    public FileSystemDialogConfiguration DialogConfig
    {
        get
        {
            return mDialogConfig ?? (mDialogConfig = new FileSystemDialogConfiguration());
        }
        set
        {
            mDialogConfig = value;
        }
    }


    /// <summary>
    /// Determines what types of files will be offered in the selector. Specified by a list of allowed file extensions, separated by a semicolon.
    /// </summary>
    public string AllowedExtensions
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AllowedExtensions"), string.Empty);
        }
        set
        {
            SetValue("AllowedExtensions", value);
        }
    }


    /// <summary>
    /// Determines what types of files will not be offered in the selector. Specified by a list of excluded file extensions, separated by a semicolon.
    /// </summary>
    public string ExcludedExtensions
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExcludedExtensions"), string.Empty);
        }
        set
        {
            SetValue("ExcludedExtensions", value);
        }
    }


    /// <summary>
    /// Gets or sets the string containing list of allowed folders.
    /// </summary>
    public string AllowedFolders
    {
        get
        {
            return ContextResolver.ResolveMacros(ValidationHelper.GetString(GetValue("AllowedFolders"), string.Empty));
        }
        set
        {
            SetValue("AllowedFolders", value);
        }
    }


    /// <summary>
    /// Gets or sets the string containing list of excluded folders.
    /// </summary>
    public string ExcludedFolders
    {
        get
        {
            return ContextResolver.ResolveMacros(ValidationHelper.GetString(GetValue("ExcludedFolders"), string.Empty));
        }
        set
        {
            SetValue("ExcludedFolders", value);
        }
    }


    /// <summary>
    /// Gets or sets if value of form control could be empty.
    /// </summary>
    public bool AllowEmptyValue
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmptyValue"), true);
        }
        set
        {
            SetValue("AllowEmptyValue", value);
        }
    }


    /// <summary>
    /// Indicates if starting path can be located out of application directory.
    /// </summary>
    public bool AllowNonApplicationPath
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowNonApplicationPath"), true);
        }
        set
        {
            SetValue("AllowNonApplicationPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the starting path of the root in the file system tree shown in the selection dialog.
    /// </summary>
    public string StartingPath
    {
        get
        {
            return ContextResolver.ResolveMacros(ValidationHelper.GetString(GetValue("StartingPath"), string.Empty));
        }
        set
        {
            SetValue("StartingPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the selected path in the file system tree shown in the selection dialog.
    /// </summary>
    public string SelectedPath
    {
        get
        {
            return ContextResolver.ResolveMacros(ValidationHelper.GetString(GetValue("SelectedPath"), string.Empty));
        }
        set
        {
            SetValue("SelectedPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the path in the file system tree selected by default.
    /// </summary>
    public string DefaultPath
    {
        get
        {
            return ContextResolver.ResolveMacros(ValidationHelper.GetString(GetValue("DefaultPath"), string.Empty));
        }
        set
        {
            SetValue("DefaultPath", value);
        }
    }


    /// <summary>
    /// Gets or sets prefix for paths with preselected source folder(web parts, form controls ...) The prefix will be trimmed from file path.
    /// </summary>
    public string SelectedPathPrefix
    {
        get
        {
            return ContextResolver.ResolveMacros(ValidationHelper.GetString(GetValue("SelectedPathPrefix"), string.Empty));
        }
        set
        {
            SetValue("SelectedPathPrefix", value);
        }
    }


    /// <summary>
    /// Gets or sets the file extension used when creating new files in the selection dialog.
    /// </summary>
    public string NewTextFileExtension
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NewTextFileExtension"), string.Empty);
        }
        set
        {
            SetValue("NewTextFileExtension", value);
        }
    }


    /// <summary>
    /// Indicates whether it will be possible to manage (upload, create, remove, edit) the files in the selection dialog.
    /// </summary>
    public bool AllowManage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowManage"), false);
        }
        set
        {
            SetValue("AllowManage", value);
        }
    }


    /// <summary>
    /// Indicates whether folder mode is turned on or if file should be selected.
    /// </summary>
    public bool ShowFolders
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFolders"), false);
        }
        set
        {
            SetValue("ShowFolders", value);
        }
    }


    /// <summary>
    /// Indicates whether the dialog allows selection from zip files as folders
    /// </summary>
    public bool AllowZipFolders
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowZipFolders"), false);
        }
        set
        {
            SetValue("AllowZipFolders", value);
        }
    }


    /// <summary>
    /// Gets or sets width of the dialog.
    /// </summary>
    public int DialogWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DialogWidth"), 0);
        }
        set
        {
            SetValue("DialogWidth", value);
        }
    }


    /// <summary>
    /// Gets or sets height of the dialog.
    /// </summary>
    public int DialogHeight
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("DialogHeight"), 0);
        }
        set
        {
            SetValue("DialogHeight", value);
        }
    }


    /// <summary>
    /// Indicates if dialog width/height are set as relative to the total width/height of the screen.
    /// </summary>
    public bool UseRelativeDimensions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseRelativeDimensions"), true);
        }
        set
        {
            SetValue("UseRelativeDimensions", value);
        }
    }


    /// <summary>
    /// Validates the return value of form control.
    /// </summary>
    public override bool IsValid()
    {
        String value = txtPath.Text.Trim();
        if (AllowEmptyValue && String.IsNullOrEmpty(value))
        {
            return true;
        }

        if (!AllowEmptyValue)
        {
            if (String.IsNullOrEmpty(value))
            {
                ValidationError = ResHelper.GetString(DialogConfig.ShowFolders ? "UserControlSelector.RequireFolderName" : "UserControlSelector.RequireFileName");
                return false;
            }
        }
        
        var fullPath = (value[0] == '~' || Path.IsPathRooted(value)) ? value : Path.Combine(SelectedPathPrefix, value);
        if (DialogConfig.ShowFolders)
        {
            if (!FileHelper.DirectoryExists(fullPath))
            {
                ValidationError = ResHelper.GetString("general.folderdoesntexist");
                return false;
            }

            if (!IsAllowedAndNotExcludedFolder(value))
            {
                ValidationError = ResHelper.GetString("dialogs.filesystem.NotAllowedFolder");
                return false;
            }
        }
        else
        {
            if (!FileHelper.FileExists(fullPath))
            {
                ValidationError = ResHelper.GetString("general.filedoesntexist");
                return false;
            }

            string ext = value.Contains(".") ? value.Substring(value.LastIndexOf(".", StringComparison.Ordinal)) : String.Empty;
            if (!IsAllowedExtension(ext) || IsExcludedExtension(ext))
            {
                if (!String.IsNullOrEmpty(DialogConfig.AllowedExtensions))
                {
                    string allowedExt = ";" + DialogConfig.AllowedExtensions + ";";

                    if (!String.IsNullOrEmpty(DialogConfig.ExcludedExtensions))
                    {
                        foreach (string excludedExt in DialogConfig.ExcludedExtensions.Split(';'))
                        {
                            allowedExt = allowedExt.Replace(";" + excludedExt + ";", ";");
                        }
                    }

                    ValidationError = ResHelper.GetString("dialogs.filesystem.NotAllowedExtension").Replace("%%extensions%%", FormatExtensions(allowedExt));
                }
                else
                {
                    ValidationError = ResHelper.GetString("dialogs.filesystem.ExcludedExtension").Replace("%%extensions%%", FormatExtensions(DialogConfig.ExcludedExtensions));
                }

                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Gets or sets if value can be changed.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            txtPath.Enabled = value;
            btnSelect.Enabled = value;
            btnClear.Enabled = value;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Init event.
    /// </summary>
    /// <param name="sender">Sender parameter</param>
    /// <param name="e">Arguments</param>
    protected void Page_Init(object sender, EventArgs e)
    {
        CreateChildControls();
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
    }


    /// <summary>
    /// Setup all contained controls.
    /// </summary>
    private void SetupControls()
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterDialogScript(Page);

        btnSelect.Text = ResHelper.GetString("General.select");
        btnClear.Text = ResHelper.GetString("General.clear");

        if (Enabled)
        {
            ApplyProperties();

            var config = DialogConfig;

            // Configure FileSystem dialog
            string width = config.DialogWidth.ToString();
            string height = config.DialogHeight.ToString();

            if (config.UseRelativeDimensions)
            {
                width += "%";
                height += "%";
            }

            config.EditorClientID = txtPath.ClientID;
            config.SelectedPath = txtPath.Text;

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(
@"
function FSS_SelectionDialogReady_{0}(url, context) {{
    modalDialog(url, 'SelectFile', '{1}', '{2}', null)
}}

function FSS_ValueUpdated_{0}() {{
    if (window.Changed) {{
        Changed();
    }}

    var item = document.getElementById('{4}');
    var newValue = item.value;
    var prefix = '{3}';
    if (prefix != '') {{
        if (newValue.indexOf(prefix) == 0) {{
            item.value = newValue.substring(prefix.length); 
            // Trim start '/'
            if (item.value[0] == '/') {{
                item.value = item.value.substring(1);
            }}
        }}
    }}
}}

function FSS_Clear_{0}(selectorId) {{ 
    document.getElementById('{4}').value = '';
    FSS_ValueUpdated_{0}();
}}
",
                ClientID,
                width,
                height,
                SelectedPathPrefix,
                txtPath.ClientID
            );

            txtPath.Attributes.Add("onchange", "FSS_ValueUpdated_" + ClientID + "();");

            // Register the Path related javascript functions
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "FileSystemSelector_" + ClientID, ScriptHelper.GetScript(sb.ToString()));

            // Setup the buttons
            btnSelect.Attributes.Add("onclick", Page.ClientScript.GetCallbackEventReference(this, "document.getElementById('" + txtPath.ClientID + "').value", "FSS_SelectionDialogReady_" + ClientID, null) + "; return false;");

            btnClear.Attributes.Add("onclick", String.Format(
                "FSS_Clear_{0}(); return false;",
                ClientID
            ));
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Returns query string which will be passed to the CMS dialogs (Insert image or media/Insert link).
    /// </summary>
    /// <param name="config">Dialog configuration</param>  
    /// <param name="selectedPathPrefix">Path prefix of selected value</param>
    public string GetDialogURL(FileSystemDialogConfiguration config, string selectedPathPrefix)
    {
        StringBuilder builder = new StringBuilder();

        var p = new Hashtable();

        if (!String.IsNullOrEmpty(config.AllowedExtensions))
        {
            p["allowed_extensions"] = config.AllowedExtensions;
        }

        // New text file extension
        if (!String.IsNullOrEmpty(config.NewTextFileExtension))
        {
            p["newfile_extension"] = config.NewTextFileExtension;
        }

        // Excluded extensions
        if (!String.IsNullOrEmpty(config.ExcludedExtensions))
        {
            p["excluded_extensions"] = config.ExcludedExtensions;
        }

        // Allowed folders 
        if (!String.IsNullOrEmpty(config.AllowedFolders))
        {
            p["allowed_folders"] = config.AllowedFolders;
        }

        // Excluded folders
        if (!String.IsNullOrEmpty(config.ExcludedFolders))
        {
            p["excluded_folders"] = config.ExcludedFolders;
        }

        // Default path-preselected path in filesystem tree
        if (!String.IsNullOrEmpty(config.DefaultPath))
        {
            p["default_path"] = config.DefaultPath;
        }

        // Deny non-application starting path
        if (!config.AllowNonApplicationPath)
        {
            p["allow_nonapp_path"] = "0";
        }

        // Allow manage
        if (config.AllowManage)
        {
            p["allow_manage"] = "1";
        }

        // SelectedPath - actual value of textbox
        if (!String.IsNullOrEmpty(config.SelectedPath))
        {
            string selectedPath = config.SelectedPath;
            if (!(selectedPath.StartsWith("~", StringComparison.Ordinal) || selectedPath.StartsWith("/", StringComparison.Ordinal) || selectedPath.StartsWith(".", StringComparison.Ordinal) || selectedPath.StartsWith("\\", StringComparison.Ordinal))
                && (String.IsNullOrEmpty(config.StartingPath) || config.StartingPath.StartsWith("~", StringComparison.Ordinal)) && !String.IsNullOrEmpty(selectedPathPrefix))
            {
                selectedPath = selectedPathPrefix.TrimEnd('/') + "/" + selectedPath.TrimStart('/');
            }
            p["selected_path"] = selectedPath;
        }

        // Starting path in filesystem
        if (!String.IsNullOrEmpty(config.StartingPath))
        {
            p["starting_path"] = config.StartingPath;
        }

        // Show only folders|files
        p["show_folders"] = config.ShowFolders.ToString();
        p["allow_zip_folders"] = config.AllowZipFolders.ToString();

        // Editor client id
        if (!String.IsNullOrEmpty(config.EditorClientID))
        {
            builder.Append("&editor_clientid=" + Server.UrlEncode(config.EditorClientID));
        }

        // Register parameters within Window helper
        var paramsGuid = Guid.NewGuid().ToString();
        WindowHelper.Add(paramsGuid, p, true);

        builder.Append("&params=", Server.UrlEncode(paramsGuid));

        // Get the query
        string query = HttpUtility.UrlPathEncode("?" + builder.ToString().TrimStart('&'));

        // Get complete query string with attached hash
        query = URLHelper.AddParameterToUrl(query, "hash", QueryHelper.GetHash(query));

        const string baseUrl = "~/CMSFormControls/Selectors/";

        // Get complete URL
        return ResolveUrl(baseUrl + "SelectFileOrFolder/Default.aspx" + query);
    }


    /// <summary>
    /// Check if manually typed extension is allowed.
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>True if allowed, false otherwise</returns>
    private bool IsAllowedExtension(string extension)
    {
        if (String.IsNullOrEmpty(DialogConfig.AllowedExtensions))
        {
            return true;
        }

        string extensions = ";" + DialogConfig.AllowedExtensions + ";";
        if (extensions.IndexOf(";" + extension.TrimStart('.') + ";", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Check if manually typed extension is excluded.
    /// </summary>
    /// <param name="extension">File extension</param>
    /// <returns>True if excluded, false otherwise</returns>
    private bool IsExcludedExtension(string extension)
    {
        if (String.IsNullOrEmpty(DialogConfig.ExcludedExtensions))
        {
            return false;
        }

        string extensions = ";" + DialogConfig.ExcludedExtensions + ";";
        if (extensions.IndexOf(";" + extension.TrimStart('.') + ";", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        return false;
    }


    /// <summary>
    /// Check if folder is allowed and not excluded.
    /// </summary>
    /// <param name="folder">DiretoryInfo to check</param>
    /// <returns>True if folder isallowed and not excluded otherwise false</returns>
    private bool IsAllowedAndNotExcludedFolder(string folder)
    {
        bool isAllowed = false;
        bool isExcluded = false;

        string startPath = DialogConfig.StartingPath;

        // Resolve relative URL with ~
        if (startPath.StartsWith("~", StringComparison.Ordinal))
        {
            startPath = ResolveUrl(startPath);
        }

        // Map to server if not network path
        if (!startPath.Contains("\\\\"))
        {
            startPath = Server.MapPath(startPath);
        }

        startPath = Path.EnsureEndBackslash(startPath);
        string folderName = Path.EnsureEndBackslash(folder);
        try
        {
            folderName = Server.MapPath(folderName);
        }
        catch
        {
        }

        // Check if folder is allowed
        if (String.IsNullOrEmpty(DialogConfig.AllowedFolders))
        {
            isAllowed = true;
        }
        else
        {
            foreach (string path in DialogConfig.AllowedFolders.Split(';'))
            {
                if (folderName.StartsWith(startPath + path, StringComparison.OrdinalIgnoreCase))
                {
                    isAllowed = true;
                }
            }
        }

        // Check if folder isn't excluded
        if (!String.IsNullOrEmpty(DialogConfig.ExcludedFolders))
        {
            foreach (string path in DialogConfig.ExcludedFolders.Split(';'))
            {
                if (folderName.StartsWith(startPath + path, StringComparison.OrdinalIgnoreCase))
                {
                    isExcluded = true;
                }
            }
        }
        return (isAllowed) && (!isExcluded);
    }


    /// <summary>
    /// Format extensions.
    /// </summary>
    /// <param name="extensions">Extensions string to be displayed</param>
    private string FormatExtensions(string extensions)
    {
        string ext = ";" + extensions.Trim(';');
        return ext.Replace(";", ";.").TrimStart(';').Replace(";", ", ");
    }

    #endregion


    #region "Callback handling"

    private string callbackResult;

    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        // Configure dialog
        var config = DialogConfig;
        config.SelectedPath = !String.IsNullOrWhiteSpace(eventArgument) ? eventArgument : DefaultPath;

        ApplyProperties();

        string url = GetDialogURL(config, SelectedPathPrefix);

        callbackResult = url;
    }


    /// <summary>
    /// Applies properties to the dialog configuration
    /// </summary>
    private void ApplyProperties()
    {
        var config = DialogConfig;

        // Apply selected path
        if (!String.IsNullOrEmpty(SelectedPath))
        {
            config.SelectedPath = SelectedPath;
        }

        // Apply starting path
        if (!String.IsNullOrEmpty(StartingPath))
        {
            config.StartingPath = StartingPath;
        }

        // Apply default path
        if (!String.IsNullOrEmpty(DefaultPath))
        {
            config.DefaultPath = DefaultPath;
        }

        // Apply allowed extensions
        if (!String.IsNullOrEmpty(AllowedExtensions))
        {
            config.AllowedExtensions = AllowedExtensions;
        }

        // Apply excluded extensions
        if (!String.IsNullOrEmpty(ExcludedExtensions))
        {
            config.ExcludedExtensions = ExcludedExtensions;
        }

        // Apply allowed folders
        if (!String.IsNullOrEmpty(AllowedFolders))
        {
            config.AllowedFolders = AllowedFolders;
        }

        // Apply excluded folders
        if (!String.IsNullOrEmpty(ExcludedFolders))
        {
            config.ExcludedFolders = ExcludedFolders;
        }

        // Apply new text files extensions
        if (!String.IsNullOrEmpty(NewTextFileExtension))
        {
            config.NewTextFileExtension = NewTextFileExtension;
        }

        // Apply dialog width
        if (DialogWidth > 0)
        {
            config.DialogWidth = DialogWidth;
        }

        // Apply dialog height
        if (DialogHeight > 0)
        {
            config.DialogHeight = DialogHeight;
        }

        // Apply Use relative dimensions
        if (GetValue("UseRelativeDimensions") != null)
        {
            config.UseRelativeDimensions = UseRelativeDimensions;
        }

        // Apply starting path
        if (GetValue("AllowManage") != null)
        {
            config.AllowManage = AllowManage;
        }

        // Apply starting path
        if (GetValue("ShowFolders") != null)
        {
            config.ShowFolders = ShowFolders;
        }

        // Apply show zip folders
        if (GetValue("AllowZipFolders") != null)
        {
            config.AllowZipFolders = AllowZipFolders;
        }
    }


    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return callbackResult;
    }

    #endregion
}