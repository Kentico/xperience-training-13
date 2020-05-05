using System;
using System.Collections;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;

using System.Text;

using CMS.FormEngine.Web.UI;
using CMS.IO;


public partial class CMSFormControls_Images_IconSelector : FormEngineUserControl
{
    #region "Constants"

    /// <summary>
    /// Value for 'Do not display any icon'.
    /// </summary>
    private const string NOT_DISPLAY_ICON = "##NONE##";

    #endregion


    #region "Private variables"

    private string mIconsFolder = "Design/Controls/IconSelector/RSS";
    private string mAllowedIconExtensions = "png";

    // Default value for RSS
    private string mFolderPreviewImageName = "24.png";
    private string mFullIconFolderPath = String.Empty;
    private string mMainPanelResourceName = "general.color";
    private string mChildPanelResourceName = "general.size";

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns full path to file system.
    /// </summary>
    private string FullIconFolderPath
    {
        get
        {
            if (mFullIconFolderPath == String.Empty)
            {
                mFullIconFolderPath = GetImagePath(IconsFolder);
                if (mFullIconFolderPath.StartsWithCSafe("~"))
                {
                    mFullIconFolderPath = Server.MapPath(mFullIconFolderPath);
                }
            }
            return mFullIconFolderPath;
        }
    }


    /// <summary>
    /// Gets current action name.
    /// </summary>
    private string CurrentAction
    {
        get
        {
            return hdnAction.Value.ToLowerCSafe().Trim();
        }
        set
        {
            hdnAction.Value = value;
        }
    }


    /// <summary>
    /// Gets current action argument value.
    /// </summary>
    private string CurrentArgument
    {
        get
        {
            return hdnArgument.Value;
        }
    }


    /// <summary>
    /// Gets or set value of selected predefined icon folder.
    /// </summary>
    private string CurrentIconFolder
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CurrentIconFolder"], String.Empty);
        }
        set
        {
            ViewState["CurrentIconFolder"] = value;
        }
    }


    /// <summary>
    /// Gets or sets value of selected predefined icon.
    /// </summary>
    private string CurrentIcon
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CurrentIcon"], String.Empty);
        }
        set
        {
            ViewState["CurrentIcon"] = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets folder name from which icons will be taken.
    /// </summary>
    public string IconsFolder
    {
        get
        {
            return mIconsFolder;
        }
        set
        {
            mIconsFolder = value;
        }
    }


    /// <summary>
    /// Gets or sets the files pattern for the icons files.
    /// </summary>
    public string AllowedIconExtensions
    {
        get
        {
            return mAllowedIconExtensions;
        }
        set
        {
            mAllowedIconExtensions = value;
        }
    }


    /// <summary>
    /// Gets or sets default image displayed as preview of icon image set.
    /// </summary>
    public string FolderPreviewImageName
    {
        get
        {
            return mFolderPreviewImageName;
        }
        set
        {
            mFolderPreviewImageName = value;
        }
    }


    /// <summary>
    /// Gets or set resource name for main panel.
    /// </summary>
    public string MainPanelResourceName
    {
        get
        {
            return mMainPanelResourceName;
        }
        set
        {
            mMainPanelResourceName = value;
        }
    }


    /// <summary>
    /// Gets or set resource name for main panel.
    /// </summary>
    public string ChildPanelResourceName
    {
        get
        {
            return mChildPanelResourceName;
        }
        set
        {
            mChildPanelResourceName = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Predefined icon
            if (UsingPredefinedIcon)
            {
                return IconsFolder + "/" + CurrentIconFolder + "/" + CurrentIcon;
            }

            // Custom icon
            string url = mediaSelector.Value;
            if (radCustomIcon.Checked && !String.IsNullOrEmpty(url))
            {
                return url.StartsWithCSafe("/")
                    ? "~" + URLHelper.RemoveApplicationPath(url) 
                    : url;
            }

            // Return value for 'Do not display icon'
            return NOT_DISPLAY_ICON;
        }
        set
        {
            // Initialize only for regular post-back
            if (RequestHelper.IsAsyncPostback())
            {
                return;
            }

            // First load (when the web-part is added to the design tab)
            if (value == null)
            {
                SetIconOptionButtonChecked(radPredefinedIcon);
                return;
            }

            string stringValue = ValidationHelper.GetString(value, String.Empty);
            
            // Empty value or 'None icon'
            // Check for String.Empty is because of backward compatibility(String.Empty was previous value for not displaying any icon)
            if (String.IsNullOrEmpty(stringValue) || stringValue.Equals(NOT_DISPLAY_ICON, StringComparison.Ordinal))
            {
                SetIconOptionButtonChecked(radDoNotDisplay);
                return;
            }

            string virtualPath = URLHelper.GetVirtualPath(GetImagePath(stringValue));
            string folder = GetImagePath(IconsFolder);
            
            // Check if same with starting path for local icon set, being null in this stage means custom icon was selected
            if ((virtualPath != null) && virtualPath.StartsWithCSafe(folder))
            {
                SetCurrentIconAndFolder(virtualPath, folder);
                SetIconOptionButtonChecked(radPredefinedIcon);
            }
            else
            {
                SetIconOptionButtonChecked(radCustomIcon);
                mediaSelector.Value = stringValue;
            }
        }
    }


    /// <summary>
    /// Indicates if predefined icons are used instead of custom icon.
    /// </summary>
    public bool UsingPredefinedIcon
    {
        get
        {
            return radPredefinedIcon.Checked;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            InitializeControlScripts();
            SetupControls();
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!String.IsNullOrEmpty(lblError.Text))
        {
            lblError.Visible = true;
            pnlUpdate.Visible = false;

            pnlUpdateContent.Update();
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Setup all contained controls.
    /// </summary>
    private void SetupControls()
    {
        // Reset error label
        lblError.Text = String.Empty;
        lblError.Visible = false;

        // Setup main radio button controls
        radCustomIcon.Text = GetString("iconselector.custom");
        radCustomIcon.Attributes.Add("onclick", "SetAction_" + ClientID + "('switch','');RaiseHiddenPostBack_" + ClientID + "();");
        radPredefinedIcon.Text = GetString("iconselector.predefined");
        radPredefinedIcon.Attributes.Add("onclick", "SetAction_" + ClientID + "('switch','');RaiseHiddenPostBack_" + ClientID + "();");
        radDoNotDisplay.Text = GetString("iconselector.donotdisplay");
        radDoNotDisplay.Attributes.Add("onclick", "SetAction_" + ClientID + "('switch','');RaiseHiddenPostBack_" + ClientID + "();");

        // Setup panels
        lblColor.ResourceString = MainPanelResourceName;
        lblSize.ResourceString = ChildPanelResourceName;

        // Configuration of media dialog
        DialogConfiguration config = new DialogConfiguration();
        config.SelectableContent = SelectableContentEnum.OnlyImages;
        config.OutputFormat = OutputFormatEnum.URL;
        config.HideWeb = false;
        config.ContentSites = AvailableSitesEnum.All;
        config.DialogWidth = 90;
        config.DialogHeight = 80;
        config.UseRelativeDimensions = true;
        config.LibSites = AvailableSitesEnum.All;


        mediaSelector.UseCustomDialogConfig = true;
        mediaSelector.DialogConfig = config;
        mediaSelector.ShowPreview = false;
        mediaSelector.IsLiveSite = IsLiveSite;

        if (!RequestHelper.IsAsyncPostback())
        {
            // Load initial data and ensure something is selected
            if ((!radCustomIcon.Checked) && (!radDoNotDisplay.Checked) && (!radPredefinedIcon.Checked))
            {
                radPredefinedIcon.Checked = true;
            }
            HandleSwitchAction();
        }
    }


    /// <summary>
    /// Initializes all the script required for communication between controls.
    /// </summary>
    private void InitializeControlScripts()
    {
        ScriptHelper.RegisterJQuery(Page);

        // SetAction function setting action name and passed argument
        StringBuilder script = new StringBuilder();
        script.Append(@"
function SetAction_", ClientID, @"(action, argument) {
  var hdnAction = $cmsj('#", hdnAction.ClientID, @"');
  var hdnArgument = $cmsj('#", hdnArgument.ClientID, @"');
  if ((hdnAction != null) && (hdnArgument != null)) {                             
     if (action != null) {                                                       
        hdnAction.val(action);                                               
     }                                                                           
     
     if (argument != null) {                                                     
        hdnArgument.val(argument);                                           
     }                                                                           
  }                                                                               
}

function SelectItem_", ClientID, @"(item) {
  $cmsj(item).parent().find("".iconItem"").removeClass(""selected"");
  $cmsj(item).addClass(""selected"");
}");

        // Get reference causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, String.Empty);
        script.Append(@"
function RaiseHiddenPostBack_", ClientID, @"() { 
", postBackRef, @";
}");

        // Render the script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "IconSelector_" + ClientID, script.ToString(), true);
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Check if file extension is allowed.
    /// </summary>
    /// <param name="info">FileInfo to check</param>
    /// <returns>True if extension isallowed otherwise false</returns>
    private bool IsAllowedFileExtension(FileInfo info)
    {
        string allowedExt = ";" + AllowedIconExtensions.Trim(';').ToLowerCSafe() + ";";
        try
        {
            return allowedExt.Contains(";" + info.Extension.ToLowerCSafe().TrimStart('.') + ";");
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Gets Arraylist filled with folder icons data.
    /// </summary>
    /// <returns>ArrayList with data for icon sets</returns>
    private ArrayList GetPredefinedIconFoldersSet()
    {
        ArrayList directories = new ArrayList();

        try
        {
            if (!String.IsNullOrEmpty(FullIconFolderPath))
            {
                DirectoryInfo di = DirectoryInfo.New(FullIconFolderPath);

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    bool containIcons = false;
                    string previewIconName = FolderPreviewImageName.ToLowerCSafe();
                    string firstIconName = String.Empty;

                    // Get files array and filter it
                    FileInfo[] files = dir.GetFiles();
                    files = Array.FindAll(files, IsAllowedFileExtension);

                    foreach (FileInfo fi in files)
                    {
                        firstIconName = String.Empty;

                        // Store first icon name to be used if default preview icon is missing
                        if (firstIconName == String.Empty)
                        {
                            firstIconName = fi.Name;
                            containIcons = true;
                        }

                        // Check for default icon
                        if (fi.Name.ToLowerCSafe() == previewIconName)
                        {
                            firstIconName = fi.Name;
                            break;
                        }
                    }

                    if (containIcons)
                    {
                        directories.Add(new string[] { dir.Name, firstIconName });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text += "[IconSelector.GetPredefinedIconFoldersSet]: Error loading predefined icon set. Original exception: " + ex.Message;
        }

        return directories;
    }


    /// <summary>
    /// Render Folder icon preview HTML.
    /// </summary>
    /// <param name="defaultValue">Determine default value which should be checked</param>
    private void GetFolderIconPreview(string defaultValue)
    {
        foreach (string[] folderInfo in GetPredefinedIconFoldersSet())
        {
            string dirName = folderInfo[0];
            string iconName = folderInfo[1];
            string path = GetImagePath(IconsFolder + "/" + dirName + "/" + iconName);

            // Icon image
            CMSImage imgFolder = new CMSImage
            {
                ImageUrl = AdministrationUrlHelper.ResolveImageUrl(path),
                AlternateText = iconName,
                EnableViewState = false
            };

            // Icon panel
            CMSPanel pnlFolder = new CMSPanel
            {
                EnableViewState = false
            };
            pnlFolder.AddCssClass("iconItem");
            pnlFolder.Controls.Add(imgFolder);
            pnlFolder.Attributes.Add("onclick", string.Format("SelectItem_{0}(this);SetAction_{0}('changefolder','{1}');RaiseHiddenPostBack_{0}();", ClientID, dirName));

            // Check for selected value
            if ((defaultValue == String.Empty) || (dirName.ToLowerCSafe() == defaultValue.ToLowerCSafe()))
            {
                defaultValue = dirName;
                pnlFolder.AddCssClass("selected");
            }

            // Add controls
            pnlMain.Controls.Add(pnlFolder);
        }

        // Actualize value of current icon folder
        CurrentIconFolder = defaultValue;
    }


    /// <summary>
    /// Gets Array List with icons located in specified directory.
    /// </summary>
    /// <param name="di">DirectoryInfo of particular directory</param>
    /// <returns>ArrayList with contained icons</returns>
    private ArrayList GetIconsInFolder(DirectoryInfo di)
    {
        ArrayList icons = new ArrayList();
        try
        {
            // Get files array and filter it
            FileInfo[] files = di.GetFiles();
            files = Array.FindAll(files, IsAllowedFileExtension);
            foreach (FileInfo fi in files)
            {
                icons.Add(fi.Name);
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "[IconSelector.GetIconsInFolder]: Error getting icons in source icon folder. Original exception: " + ex.Message;
        }
        return icons;
    }


    /// <summary>
    /// Sets given radio button to checked
    /// </summary>
    /// <param name="button"></param>
    private void SetIconOptionButtonChecked(CMSRadioButton button)
    {
        if (button == null)
        {
            // Ensure controls are available
            pnlUpdateContent.LoadContainer();
            pnlUpdate.LoadContainer();
        }

        if (button != null)
        {
            button.Checked = true;
        }
    }


    /// <summary>
    /// Sets current (custom) icon's folder and file name
    /// </summary>
    /// <param name="virtualPath">Virtual path to the selected icon file (including icons folder)</param>
    /// <param name="iconsFolder">Virtual path to the folder with icons</param>
    private void SetCurrentIconAndFolder(string virtualPath, string iconsFolder)
    {
        string iconSubfolder = Path.GetDirectoryName(virtualPath);
        
        // virtual path is supposed to start with iconFolder (it should continue with classifying sub-folder and ending with sizing file name)
        if (String.Equals(iconsFolder, Path.GetDirectoryName(iconSubfolder), StringComparison.InvariantCultureIgnoreCase))
        {
            try
            {
                if (File.Exists(Server.MapPath(virtualPath)))
                {
                    // Gets last folder name
                    CurrentIconFolder = Path.GetFileName(iconSubfolder);
                    
                    // Gets icon file name
                    CurrentIcon = Path.GetFileName(virtualPath);
                }
            }
            catch (Exception ex)
            {
                lblError.Text += "[IconSelector.SetValue]: Error accessing selected icon. Original exception: " + ex.Message;
            }
        }
    }

    #endregion


    #region "Handler methods"

    /// <summary>
    /// Generate apropriate icons if source folder is changed.
    /// </summary>
    /// <param name="folderName">Name of selected folder</param>
    private void HandleChangeFolderAction(string folderName)
    {
        HandleChangeFolderAction(folderName, CurrentIcon);
    }


    /// <summary>
    /// Generate apropriate icons if source folder is changed.
    /// </summary>
    /// <param name="folderName">Name of selected folder</param>
    /// <param name="defaultValue">Determine default value which should be checked</param>
    private void HandleChangeFolderAction(string folderName, string defaultValue)
    {
        try
        {
            DirectoryInfo di = DirectoryInfo.New(DirectoryHelper.CombinePath(FullIconFolderPath, folderName));
            string directoryName = di.Name;
            ArrayList iconList = GetIconsInFolder(di);
            string defaultIcon = (iconList.Contains(defaultValue)) ? defaultValue : String.Empty;

            foreach (string fileInfo in iconList)
            {
                string path = GetImagePath(IconsFolder + "/" + directoryName + "/" + fileInfo);

                // Size caption
                LocalizedLabel lblIcon = new LocalizedLabel
                {
                    ResourceString = "iconcaption." + fileInfo.Remove(fileInfo.LastIndexOfCSafe('.')),
                    EnableViewState = false
                };

                // Icon image
                CMSImage imgIcon = new CMSImage
                {
                    ImageUrl = AdministrationUrlHelper.ResolveImageUrl(path),
                    AlternateText = fileInfo,
                    EnableViewState = false
                };

                // Icon panel
                CMSPanel pnlIcon = new CMSPanel
                {
                    CssClass = "iconItem",
                    EnableViewState = false
                };
                pnlIcon.Attributes.Add("onclick", string.Format("SelectItem_{0}(this);SetAction_{0}('select','{1}');RaiseHiddenPostBack_{0}();", ClientID, fileInfo));
                pnlIcon.Controls.Add(imgIcon);
                pnlIcon.Controls.Add(lblIcon);


                // Check for selected value
                if ((defaultIcon == String.Empty) || (fileInfo.ToLowerCSafe() == defaultValue.ToLowerCSafe()))
                {
                    defaultIcon = fileInfo;
                    pnlIcon.AddCssClass("selected");
                }

                // Add controls
                pnlChild.Controls.Add(pnlIcon);
            }
            CurrentIcon = defaultIcon;
        }
        catch (Exception ex)
        {
            lblError.Text += "[IconSelector.HandleChangeFolderAction]: Error getting icons in selected icon folder. Original exception: " + ex.Message;
        }
        CurrentIconFolder = folderName;
        pnlUpdateIcons.Update();
    }


    /// <summary>
    /// Handle situation when type of choosing is changed.
    /// </summary>
    private void HandleSwitchAction()
    {
        pnlPredefined.Visible = radPredefinedIcon.Checked;
        pnlCustom.Visible = radCustomIcon.Checked;

        if (pnlPredefined.Visible)
        {
            GetFolderIconPreview(CurrentIconFolder);
            HandleChangeFolderAction(CurrentIconFolder, CurrentIcon);
        }

        pnlUpdate.Update();
    }


    /// <summary>
    /// Behaves as mediator in communication line between control taking action and the rest of the same level controls.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        switch (CurrentAction)
        {
            // Switch from predefined icon to custom
            case "switch":
                HandleSwitchAction();
                break;

            // Change predefined icon folder
            case "changefolder":
                //this.CurrentIcon = String.Empty;
                HandleChangeFolderAction(CurrentArgument);
                break;

            // Select icon
            case "select":
                CurrentIcon = CurrentArgument;
                ClearActionElems();
                break;

            // By default do nothing
            default:
                break;
        }
    }


    /// <summary>
    /// Clears hidden control elements fo future use.
    /// </summary>
    private void ClearActionElems()
    {
        CurrentAction = String.Empty;
        hdnArgument.Value = String.Empty;
    }

    #endregion
}