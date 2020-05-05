using System;
using System.Collections;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_FileSystemPathProperties : ItemProperties
{
    #region "Variables"

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets if selected item is file or folder.
    /// </summary>
    public bool IsFile
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsFile"], true);
        }
        set
        {
            ViewState["IsFile"] = value;
        }
    }


    /// <summary>
    /// File system dialog configuration.
    /// </summary>
    public FileSystemDialogConfiguration DialogConfig
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get refference causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

        txtPathText.Attributes["onkeydown"] = postBackKeyDownRef;
    }


    /// <summary>
    /// Page prerender.
    /// </summary>
    protected void Page_Prerender(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(txtPathText.Text))
        {
            if (lblError.Text == String.Empty)
            {
                lblEmpty.Text = DialogConfig.ShowFolders ? GetString("dialogs.filesystem.nofolderselected") : NoSelectionText;
                lblEmpty.Visible = true;
                plnPathUpdate.Visible = false;
            }
            else
            {
                plnPathUpdate.Visible = true;
                lblEmpty.Visible = false;
            }
        }
        else
        {
            plnPathUpdate.Visible = true;
            lblEmpty.Visible = false;
        }
    }


    /// <summary>
    /// Click action on hidden button.
    /// </summary>
    protected void hdnButton_Click(object sender, EventArgs e)
    {
        if (Validate())
        {
            // Get selected item information
            var props = GetItemProperties();

            // Get JavaScript for inserting the item
            var script = CMSDialogHelper.GetFileSystemItem(props);
            if (!string.IsNullOrEmpty(script))
            {
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "insertItemScript", script, true);
            }
        }
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Loads the properties into control.
    /// </summary>
    /// <param name="properties">Properties to be loaded</param>
    public override void LoadItemProperties(Hashtable properties)
    {
        if (properties != null)
        {
            string filePath = ValidationHelper.GetString(properties[DialogParameters.ITEM_PATH], "");
            string fileSize = ValidationHelper.GetString(properties[DialogParameters.ITEM_SIZE], "");
            bool showSize = ValidationHelper.GetBoolean(properties[DialogParameters.ITEM_ISFILE], true);
            bool isPathRelative = ValidationHelper.GetBoolean(properties[DialogParameters.ITEM_RELATIVEPATH], true);

            IsFile = showSize;
            if (isPathRelative)
            {
                filePath = URLHelper.UnMapPath(filePath);
                if (filePath == "~")
                {
                    filePath += "/";
                }
            }
            txtPathText.Text = filePath;

            filePath = FileHelper.GetLiveFilePath(filePath);

            lblFileUrl.Text = URLHelper.GetAbsoluteUrl(filePath);

            if (showSize)
            {
                plcFileSize.Visible = true;
                lblFileSizeText.Text = fileSize;
            }
            else
            {
                plcFileSize.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();
        
        string path = txtPathText.Text.Trim();
        retval[DialogParameters.ITEM_PATH] = path;
        retval[DialogParameters.ITEM_RESOLVED_PATH] = UrlResolver.ResolveUrl(path);
        retval[DialogParameters.ITEM_SIZE] = lblFileSizeText.Text.Trim();
        retval[DialogParameters.ITEM_ISFILE] = ValidationHelper.IsFileName(txtPathText.Text.Trim());
        retval[DialogParameters.EDITOR_CLIENTID] = QueryHelper.GetString(DialogParameters.EDITOR_CLIENTID, "");

        return retval;
    }


    /// <summary>
    /// Validates From, Cc and Bcc e-mails.
    /// </summary>
    public override bool Validate()
    {
        string errorMessage = "";
        try
        {
            if (string.IsNullOrEmpty(txtPathText.Text))
            {
                errorMessage += GetString("dialogs.filesystem.requirepath");
            }
            if (IsFile)
            {
                string ext = (txtPathText.Text.Contains(".") ? txtPathText.Text.Substring(txtPathText.Text.LastIndexOfCSafe(".")) : String.Empty);
                if (!(File.Exists(txtPathText.Text.Trim()) || FileHelper.FileExists(txtPathText.Text.Trim())))
                {
                    errorMessage += GetString("general.filedoesntexist");
                }
                else if (!IsAllowedExtension(ext) || IsExcludedExtension(ext))
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
                        errorMessage += GetString("dialogs.filesystem.NotAllowedExtension").Replace("%%extensions%%", FormatExtensions(allowedExt));
                    }
                    else
                    {
                        errorMessage += GetString("dialogs.filesystem.ExcludedExtension").Replace("%%extensions%%", FormatExtensions(DialogConfig.ExcludedExtensions));
                    }
                }
            }
            else
            {
                if (!((Directory.Exists(txtPathText.Text.Trim())) || (FileHelper.DirectoryExists(txtPathText.Text.Trim()))) && (!IsFile))
                {
                    errorMessage += GetString("general.folderdoesntexist");
                }
                else if (!IsAllowedAndNotExcludedFolder(txtPathText.Text))
                {
                    errorMessage += GetString("dialogs.filesystem.NotAllowedFolder");
                }
            }
        }
        catch (Exception ex)
        {
            errorMessage += ex.Message;
        }

        errorMessage = errorMessage.Trim();

        if (errorMessage != "")
        {
            lblError.Text = errorMessage;
            lblError.Visible = true;
            plcFileSize.Visible = false;
            plnPathUpdate.Update();
            return false;
        }

        return true;
    }


    /// <summary>
    /// Clears the properties form.
    /// </summary>
    public override void ClearProperties(bool hideProperties)
    {
        txtPathText.Text = "";
        lblFileSizeText.Text = "";
    }

    #endregion


    #region "Helper methods"

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
        else
        {
            string extensions = ";" + DialogConfig.AllowedExtensions.ToLowerCSafe() + ";";
            if (extensions.Contains(";" + extension.ToLowerCSafe().TrimStart('.') + ";"))
            {
                return true;
            }
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
        else
        {
            string extensions = ";" + DialogConfig.ExcludedExtensions.ToLowerCSafe() + ";";
            if (extensions.Contains(";" + extension.ToLowerCSafe().TrimStart('.') + ";"))
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Check if folder is allowed and not excluded.
    /// </summary>
    /// <param name="folder">Folder to check</param>
    /// <returns>True if folder isallowed and not excluded otherwise false</returns>
    private bool IsAllowedAndNotExcludedFolder(string folder)
    {
        bool isAllowed = false;
        bool isExcluded = false;

        string startPath = DialogConfig.StartingPath;

        // Resolve relative URL with ~
        if (startPath.StartsWithCSafe("~"))
        {
            startPath = ResolveUrl(startPath);
        }

        // Map to server if not network path
        if (!startPath.Contains("\\\\"))
        {
            startPath = Server.MapPath(startPath);
        }

        startPath = Path.EnsureEndBackslash(startPath.ToLowerCSafe());
        string folderName = Path.EnsureEndBackslash(folder.ToLowerCSafe());
        try
        {
            folderName = Server.MapPath(folderName);
        }
        catch
        {
        }
        folderName = folderName.ToLowerCSafe();

        // Check if folder is allowed
        if (String.IsNullOrEmpty(DialogConfig.AllowedFolders))
        {
            isAllowed = true;
        }
        else
        {
            foreach (string path in DialogConfig.AllowedFolders.ToLowerCSafe().Split(';'))
            {
                if (folderName.StartsWithCSafe(startPath + path))
                {
                    isAllowed = true;
                }
            }
        }

        // Check if folder isn't excluded
        if (!String.IsNullOrEmpty(DialogConfig.ExcludedFolders))
        {
            foreach (string path in DialogConfig.ExcludedFolders.ToLowerCSafe().Split(';'))
            {
                if (folderName.StartsWithCSafe(startPath + path))
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
    private static string FormatExtensions(string extensions)
    {
        string ext = ";" + extensions.Trim(';');
        return ext.Replace(";", ";.").TrimStart(';').Replace(";", ", ");
    }

    #endregion
}