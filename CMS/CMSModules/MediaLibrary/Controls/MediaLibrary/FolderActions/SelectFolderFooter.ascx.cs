using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_FolderActions_SelectFolderFooter : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            // Setup controls
            SetupControls();
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        CMSDialogHelper.RegisterDialogHelper(Page);

        // Register for events
        btnInsert.Click += btnInsert_Click;
        btnCancel.Click += btnCancel_Click;

        switch (QueryHelper.GetString("action", "").ToLowerCSafe().Trim())
        {
            case "copy":
                btnInsert.ResourceString = "general.copy";
                break;

            case "move":
                btnInsert.ResourceString = "general.move";
                break;

            default:
                btnInsert.ResourceString = "general.select";
                break;
        }
    }

    #endregion


    #region "Event handlers"

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
    }


    protected void btnInsert_Click(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("if ((window.parent.frames['selectFolderContent'])&&(window.parent.frames['selectFolderContent'].RaiseAction)) {window.parent.frames['selectFolderContent'].RaiseAction();}");
    }

    #endregion
}
