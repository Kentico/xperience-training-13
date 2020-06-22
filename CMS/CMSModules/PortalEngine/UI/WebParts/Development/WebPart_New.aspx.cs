using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_New : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Disable "Generate new files" option on azure / precompiled environment
        if (SystemContext.IsPrecompiledWebsite || SystemContext.IsRunningOnAzure)
        {
            radNewFile.Enabled = false;
            radNewFile.Checked = false;
            radNewFile.ToolTipResourceString = "webpart.edit.precompiledsite";

            radExistingFile.Checked = true;
            plcSelectFile.Visible = true;
            plcFileName.Visible = false;
        }

        // Setup page title text and image
        PageTitle.TitleText = GetString("Development-WebPart_Edit.TitleNew");
        // Initialize
        btnOk.Text = GetString("general.ok");
        rfvWebPartDisplayName.ErrorMessage = GetString("Development-WebPart_Edit.ErrorDisplayName");
        rfvWebPartName.ErrorMessage = GetString("Development-WebPart_Edit.ErrorWebPartName");
        rfvCodeFileName.ErrorMessage = GetString("webpart.codefilenamerequired");
        webpartSelector.ShowInheritedWebparts = false;

        lblWebpartList.Text = GetString("DevelopmentWebPartEdit.InheritedWebPart");

        // Set breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Development-WebPart_Edit.WebParts"),
            RedirectUrl = UIContextHelper.GetElementUrl("CMS.Design", "Development.WebParts", false),
            Target = "_parent"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Development-WebPart_Edit.New"),
        });

        FileSystemDialogConfiguration config = new FileSystemDialogConfiguration();
        config.AllowedExtensions = "ascx";
        config.ShowFolders = false;

        FileSystemSelector.DialogConfig = config;
        FileSystemSelector.AllowEmptyValue = false;
        FileSystemSelector.SelectedPathPrefix = "~/CMSWebParts/";
        FileSystemSelector.DefaultPath = "CMSWebParts";
    }


    /// <summary>
    /// Handles radio buttons change.
    /// </summary>
    protected void radNewWebPart_CheckedChanged(object sender, EventArgs e)
    {
        plcWebparts.Visible = radInherited.Checked;
        plcCodeFiles.Visible = radNewWebPart.Checked;
    }


    /// <summary>
    /// Handles 'Code files' radio buttons change.
    /// </summary>
    protected void radNewFile_CheckedChanged(object sender, EventArgs e)
    {
        plcFileName.Visible = radNewFile.Checked;
        plcSelectFile.Visible = radExistingFile.Checked;
    }


    /// <summary>
    /// Creates new web part.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate the text box fields
        string errorMessage = new Validator().IsCodeName(txtWebPartName.Text, GetString("general.invalidcodename")).Result;

        // Validate and trim file name textbox only if it's visible
        if (radNewWebPart.Checked && radNewFile.Checked)
        {
            if (String.IsNullOrEmpty(errorMessage))
            {
                errorMessage = new Validator().IsFileName(Path.GetFileName(txtCodeFileName.Text), GetString("WebPart_Clone.InvalidFileName")).Result;
            }
        }

        // Check file name
        if (radExistingFile.Checked && radNewWebPart.Checked)
        {
            if (String.IsNullOrEmpty(errorMessage))
            {
                string webpartPath = WebPartInfoProvider.GetWebPartPhysicalPath(FileSystemSelector.Value.ToString());
                errorMessage = new Validator().IsFileName(Path.GetFileName(webpartPath), GetString("WebPart_Clone.InvalidFileName")).Result;
            }
        }

        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(HTMLHelper.HTMLEncode(errorMessage));
            return;
        }

        // Run in transaction
        using (var tr = new CMSTransactionScope())
        {
            WebPartInfo wi = new WebPartInfo();

            // Check if new name is unique
            WebPartInfo webpart = WebPartInfoProvider.GetWebPartInfo(txtWebPartName.Text);
            if (webpart != null)
            {
                ShowError(GetString("Development.WebParts.WebPartNameAlreadyExist").Replace("%%name%%", txtWebPartName.Text));
                return;
            }

            string filename = String.Empty;

            if (radNewWebPart.Checked)
            {
                if (radExistingFile.Checked)
                {
                    filename = FileSystemSelector.Value.ToString().Trim();

                    if (filename.ToLowerCSafe().StartsWithCSafe("~/cmswebparts/"))
                    {
                        filename = filename.Substring("~/cmswebparts/".Length);
                    }
                }
                else
                {
                    filename = txtCodeFileName.Text.Trim();

                    if (!filename.EndsWithCSafe(".ascx"))
                    {
                        filename += ".ascx";
                    }
                }
            }

            wi.WebPartDisplayName = txtWebPartDisplayName.Text.Trim();
            wi.WebPartFileName = filename;
            wi.WebPartName = txtWebPartName.Text.Trim();
            wi.WebPartCategoryID = QueryHelper.GetInteger("parentobjectid", 0);
            wi.WebPartDescription = "";
            wi.WebPartDefaultValues = "<form></form>";
            // Initialize WebPartType - fill it with the default value
            wi.WebPartType = wi.WebPartType;
            wi.WebPartIconClass = PortalHelper.DefaultWebPartIconClass;

            // Inherited web part
            if (radInherited.Checked)
            {
                // Check if is selected webpart and isn't category item
                if (ValidationHelper.GetInteger(webpartSelector.Value, 0) <= 0)
                {
                    ShowError(GetString("WebPartNew.InheritedCategory"));
                    return;
                }

                int parentId = ValidationHelper.GetInteger(webpartSelector.Value, 0);
                var parent = WebPartInfoProvider.GetWebPartInfo(parentId);
                if (parent != null)
                {
                    wi.WebPartType = parent.WebPartType;
                    wi.WebPartResourceID = parent.WebPartResourceID;
                    wi.WebPartSkipInsertProperties = parent.WebPartSkipInsertProperties;
                    wi.WebPartIconClass = parent.WebPartIconClass;
                }

                wi.WebPartParentID = parentId;

                // Create empty default values definition
                wi.WebPartProperties = "<defaultvalues></defaultvalues>";
            }
            else
            {
                // Check if filename was added
                if (FileSystemSelector.Visible && !FileSystemSelector.IsValid())
                {
                    ShowError(FileSystemSelector.ValidationError);
                    return;
                }

                wi.WebPartProperties = "<form></form>";
                wi.WebPartParentID = 0;
            }

            // Save the web part
            WebPartInfoProvider.SetWebPartInfo(wi);

            if (radNewFile.Checked && radNewWebPart.Checked)
            {
                string physicalFile = WebPartInfoProvider.GetFullPhysicalPath(wi);
                if (!File.Exists(physicalFile))
                {
                    // Write the files
                    try
                    {
                        var generator = new WebPartCodeGenerator(SystemContext.ApplicationPath);
                        var result = generator.GenerateWebPartCode(wi);

                        string folder = Path.GetDirectoryName(physicalFile);

                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }

                        File.WriteAllText(physicalFile, result.Markup);
                        File.WriteAllText(physicalFile + ".cs", result.Code);

                        // Designer file
                        if (!String.IsNullOrEmpty(result.Designer))
                        {
                            File.WriteAllText(physicalFile + ".designer.cs", result.Designer);
                        }

                    }
                    catch (Exception ex)
                    {
                        LogAndShowError("WebParts", "GENERATEFILES", ex, true);
                        return;
                    }
                }
                else
                {
                    ShowError(String.Format(GetString("General.FileExistsPath"), physicalFile));
                    return;
                }
            }

            // Refresh web part tree
            ScriptHelper.RegisterStartupScript(this, typeof(string), "reloadframee", ScriptHelper.GetScript(
                "parent.location = '" + UIContextHelper.GetElementUrl("cms.design", "Development.Webparts", false, wi.WebPartID) + "';"));

            PageBreadcrumbs.Items[1].Text = HTMLHelper.HTMLEncode(wi.WebPartDisplayName);
            ShowChangesSaved();
            plcTable.Visible = false;

            tr.Commit();
        }
    }
}