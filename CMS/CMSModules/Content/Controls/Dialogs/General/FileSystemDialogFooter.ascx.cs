using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_General_FileSystemDialogFooter : CMSUserControl
{
    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
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

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Register for events
        btnInsert.Click += new EventHandler(btnInsert_Click);
        btnCancel.Click += new EventHandler(btnCancel_Click);

        // Register scripts for current code editor
        CMSDialogHelper.RegisterDialogHelper(Page);
        ScriptHelper.RegisterScriptFile(Page, "Dialogs/FileSystem.js");

        btnInsert.ResourceString = GetString("general.select");
        btnCancel.ResourceString = GetString("general.cancel");
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Button cancel click.
    /// </summary>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
    }


    /// <summary>
    /// Button insert click.
    /// </summary>
    protected void btnInsert_Click(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("if (window.parent.frames['insertContent'] && window.parent.frames['insertContent'].insertItem) { window.parent.frames['insertContent'].insertItem(); }");
    }


    /// <summary>
    /// Callback hidden button click.
    /// </summary>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        SessionHelper.SetValue("DialogParameters", null);
        SessionHelper.SetValue("DialogSelectedParameters", null);
        string selected = hdnSelected.Value;
    }

    #endregion
}
