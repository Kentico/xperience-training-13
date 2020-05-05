using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_NewFolder : CMSModalPage
{
    #region "Variables"

    private string targetPath;

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            String identifier = QueryHelper.GetString("identifier", null);
            if (!String.IsNullOrEmpty(identifier))
            {
                Hashtable properties = WindowHelper.GetItem(identifier) as Hashtable;
                if (properties != null)
                {
                    lblName.Text = GetString("folder_edit.foldername");

                    targetPath = ValidationHelper.GetString(properties["targetpath"], string.Empty);

                    Page.Header.Title = GetString("dialogs.newfoldertitle");

                    PageTitle.TitleText = GetString("media.folder.new");
                }
            }
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check valid input
        string errMsg = new Validator().
            NotEmpty(txtName.Text, GetString("media.folder.foldernameempty")).
            IsFolderName(txtName.Text, GetString("media.folder.foldernameerror")).
            Result;

        string path = String.Format("{0}\\{1}", targetPath, txtName.Text);

        // Check existing
        if (String.IsNullOrEmpty(errMsg) && Directory.Exists(path))
        {
            errMsg = GetString("media.folder.folderexist");
        }

        if (!String.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
        }
        else
        {
            try
            {
                // Create the folder
                Directory.CreateDirectory(path);

                ltlScript.Text = ScriptHelper.GetScript(String.Format("wopener.SetTreeRefreshAction({0}); CloseDialog()", ScriptHelper.GetString(path)));
            }
            catch (Exception ex)
            {
                // Display an error to the user
                ShowError(String.Format("{0} {1}", GetString("general.erroroccurred"), ex.Message));
            }
        }
    }

    #endregion
}
