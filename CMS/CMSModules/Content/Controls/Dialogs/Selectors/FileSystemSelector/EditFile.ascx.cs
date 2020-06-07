using System;
using System.Collections;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_EditFile : CMSAdminControl
{
    #region "Variables"

    private string filePath;
    private string fileName;
    private string extension;
    private bool newFile;
    Hashtable prop;
    String identifier = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init
        ICMSMasterPage currentMaster = Page.Master as ICMSMasterPage;
        InitHeaderActions();
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "performAction", ScriptHelper.GetScript("function actionPerformed(action) { " + Page.ClientScript.GetPostBackEventReference(btnAction, "#").Replace("'#'", "action") + ";}"));

        txtContent.ShowBookmarks = true;
        txtContent.FullScreenParentElementID = "divContent";

        if (QueryHelper.ValidateHash("hash"))
        {
            identifier = QueryHelper.GetString("identifier", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                prop = WindowHelper.GetItem(identifier) as Hashtable;
                if (prop != null)
                {
                    lblName.Text = GetString("general.filename");

                    filePath = Server.MapPath(ValidationHelper.GetString(prop["filepath"], ""));
                    extension = ValidationHelper.GetString(prop["newfileextension"], "");

                    if (!String.IsNullOrEmpty(extension))
                    {
                        // New file
                        newFile = true;

                        if (!extension.StartsWith(".", StringComparison.Ordinal))
                        {
                            extension = "." + extension;
                        }

                        filePath = ValidationHelper.GetString(prop["targetpath"], "");

                        if (currentMaster != null)
                        {
                            currentMaster.Title.TitleText = GetString("filemanagernew.header.file");
                        }
                    }
                    else
                    {
                        // Edit file
                        if (!File.Exists(filePath))
                        {
                            // Redirect to error page
                            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("Error.Header", "general.filedoesntexist", true));
                        }

                        fileName = Path.GetFileNameWithoutExtension(filePath);

                        // Setup the controls
                        if (!RequestHelper.IsPostBack())
                        {
                            txtName.Text = fileName;
                            txtContent.Text = File.ReadAllText(filePath);

                            if (QueryHelper.GetBoolean("saved", false))
                            {
                                ShowChangesSaved();
                                String script = "wopener.SetRefreshAction();";
                                ScriptHelper.RegisterStartupScript(Page, typeof(String), "refreshScript", ScriptHelper.GetScript(script));
                            }
                        }

                        extension = Path.GetExtension(filePath);

                        if (currentMaster != null)
                        {
                            currentMaster.Title.TitleText = GetString("filemanageredit.header.file");
                        }
                    }

                    // Setup the syntax highlighting
                    switch (extension.TrimStart('.').ToLowerCSafe())
                    {
                        case "css":
                            txtContent.Language = LanguageEnum.CSS;
                            break;

                        case "skin":
                            txtContent.Language = LanguageEnum.ASPNET;
                            break;

                        case "xml":
                            txtContent.Language = LanguageEnum.XML;
                            break;

                        case "html":
                            txtContent.Language = LanguageEnum.HTMLMixed;
                            break;

                        case "cs":
                            txtContent.Language = LanguageEnum.CSharp;
                            break;

                        case "js":
                            txtContent.Language = LanguageEnum.JavaScript;
                            break;
                    }

                    // Setup the labels
                    lblExt.Text = extension;
                }
            }
        }
    }


    /// <summary>
    /// Init menu
    /// </summary>
    private void InitHeaderActions()
    {
        // Save action
        SaveAction save = new SaveAction();
        headerActions.ActionsList.Add(save);

        headerActions.ActionPerformed += (sender, e) =>
        {
            if (e.CommandName == ComponentEvents.SAVE)
            {
                Save(false);
            }
        };
    }


    protected void btnAction_Click(object sender, EventArgs e)
    {
        String arg = Request[Page.postEventArgumentID];
        Save(arg == "saveandclose");
    }


    /// <summary>
    /// Saves the item
    /// </summary>
    /// <param name="saveAndClose">Close dialog after save</param>
    private void Save(bool saveAndClose)
    {
        //Check valid input
        string errMsg = new Validator().
            NotEmpty(txtName.Text, GetString("img.errors.filename")).
            IsFolderName(txtName.Text, GetString("img.errors.filename")).
            Result;

        if (!String.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
            return;
        }

        // Prepare the path
        string path = filePath;

        if (!newFile)
        {
            path = Path.GetDirectoryName(path);
        }
        path += "\\" + txtName.Text + extension;

        // Check the file name for existence
        if (!txtName.Text.EqualsCSafe(fileName, true))
        {
            if (File.Exists(path))
            {
                errMsg = GetString("general.fileexists");
            }
        }

        if (!String.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
            return;
        }

        bool success = true;
        bool fileNameChanged = false;
        try
        {
            // Move the file to the new location
            if (!newFile && !path.EqualsCSafe(filePath, true))
            {
                File.WriteAllText(filePath, txtContent.Text);
                File.Move(filePath, path);
                fileNameChanged = true;
            }
            // Create the file or write into it
            else
            {
                File.WriteAllText(path, txtContent.Text);
            }
        }
        catch (Exception ex)
        {
            success = false;
            ShowError(GetString("general.saveerror"), ex.Message, null);
            Service.Resolve<IEventLogService>().LogException("FileSystemSelector", "SAVEFILE", ex);
        }

        if (success)
        {
            ShowChangesSaved();

            // Redirect for new items
            if (newFile && !saveAndClose)
            {
                string fileIdentifier = Guid.NewGuid().ToString("N") + path.GetHashCode();
                Hashtable pp = new Hashtable();
                pp.Add("filepath", URLHelper.UnMapPath(path));
                pp.Add("newfileextension", String.Empty);
                WindowHelper.Add(fileIdentifier, pp);

                string parameters = String.Format("?identifier={0}&saved=1", fileIdentifier);
                string validationHash = QueryHelper.GetHash(parameters);
                string url = UrlResolver.ResolveUrl("~/CMSModules/Content/Controls/Dialogs/Selectors/FileSystemSelector/EditTextFile.aspx") + parameters + "&hash=" + validationHash;
                URLHelper.Redirect(url);
            }

            // Update file name path stored in session in case of changing file name
            if (fileNameChanged)
            {
                prop["filepath"] = URLHelper.UnMapPath(path);
                WindowHelper.Add(identifier, prop);
            }

            String script = "wopener.SetRefreshAction();";
            if (saveAndClose)
            {
                script += "CloseDialog()";
            }
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "closescript", ScriptHelper.GetScript(script));
        }
    }

    #endregion
}

