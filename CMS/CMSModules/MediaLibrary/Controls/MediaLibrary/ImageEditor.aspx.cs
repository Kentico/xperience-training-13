using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_ImageEditor : CMSModalPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            if (QueryHelper.GetBoolean("refresh", false))
            {
                var script = String.Empty;
                if (!imageEditor.IsUndoRedoPossible())
                {
                    script = @"
$cmsj(window).unload(function () {
    " + GetRefresh() + @"
})";
                }

                // Register update script
                ScriptHelper.RegisterJQuery(Page);

                script += @"
$cmsj(window).on('beforeunload', function () {
    if (wopener.EditDialogStateUpdate) { 
        wopener.EditDialogStateUpdate('false'); 
    }
})
$cmsj(window).load(function () {
   if (wopener.EditDialogStateUpdate) { 
        wopener.EditDialogStateUpdate('true'); 
    }
})";
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "DialogStateUpdate", script, true);
            }

            PageTitle.TitleText = GetString("general.editimage");
            // Register postback
            ControlsHelper.RegisterPostbackControl(btnRedo);
            ControlsHelper.RegisterPostbackControl(btnUndo);
            ControlsHelper.RegisterPostbackControl(btnSave);
            ControlsHelper.RegisterPostbackControl(btnClose);

            btnSave.Click += btnSave_Click;
            btnUndo.Click += btnUndo_Click;
            btnRedo.Click += btnRedo_Click;
            btnClose.Click += btnClose_Click;

            btnUndo.Visible = imageEditor.IsUndoRedoPossible();
            btnRedo.Visible = imageEditor.IsUndoRedoPossible();
            btnSave.Visible = imageEditor.IsUndoRedoPossible();

            AddNoCacheTag();
        }
        else
        {
            // Hide all controls
            imageEditor.Visible = false;
            btnUndo.Visible = false;
            btnRedo.Visible = false;
            btnSave.Visible = false;
            btnClose.Visible = false;

            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript(String.Format("window.location = '{0}';", url));
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            btnUndo.Enabled = imageEditor.IsUndoEnabled();
            btnRedo.Enabled = imageEditor.IsRedoEnabled();

            if (btnUndo.Enabled || btnRedo.Enabled)
            {
                string confirmScript = String.Format("function discardchanges() {{ return confirm({0}); }}", ScriptHelper.GetString(GetString("imageeditor.discardchangesconfirmation")));
                ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "DiscardChanges", ScriptHelper.GetScript(confirmScript));

                btnClose.OnClientClick = "return discardchanges();";
            }
        }

        // Enable or disable save button
        btnSave.Enabled = imageEditor.Enabled;
    }

    #endregion


    #region "Buttons handling"

    protected void btnRedo_Click(object sender, EventArgs e)
    {
        imageEditor.ProcessRedo();
    }


    protected void btnUndo_Click(object sender, EventArgs e)
    {
        imageEditor.ProcessUndo();
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        imageEditor.SaveCurrentVersion();
        if (!imageEditor.SavingFailed)
        {
            string script = "setTimeout(function() { window.skipCloseConfirm = true; CloseDialog(); },0);";
            if (imageEditor.IsUndoRedoPossible())
            {
                script += GetRefresh();
            }

            ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseImageEditor", script, true);
        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        TempFileInfoProvider.DeleteTempFiles(TempFileInfoProvider.IMAGE_EDITOR_FOLDER, imageEditor.InstanceGUID);

        string script = "setTimeout(function() { window.skipCloseConfirm = true; CloseDialog(); },0);";

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseImageEditor", script, true);
    }

    #endregion


    #region "Private methods"

    private static string GetRefresh()
    {
        if (QueryHelper.GetBoolean("refresh", false))
        {
            string guid = QueryHelper.GetString("MediaFileGUID", "");
            string siteName = QueryHelper.GetString("siteName", "");
            bool isPreview = QueryHelper.GetBoolean("isPreview", false);

            return String.Format("if (wopener.imageEdit_Refresh) {{ wopener.imageEdit_Refresh('{0}|{1}|{2}');}}", ScriptHelper.GetString(guid, false), ScriptHelper.GetString(siteName, false), isPreview);
        }

        return String.Empty;
    }

    #endregion
}
