using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_LiveSelectors_PublicImageEditor : CMSLiveModalPage
{
    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            if (!imageEditor.IsUndoRedoPossible())
            {
                CurrentMaster.Body.Attributes["onunload"] = GetRefresh();
            }

            string title = GetString("general.editimage");
            Page.Title = title;
            PageTitle.TitleText = title;
            // Register postback
            ControlsHelper.RegisterPostbackControl(btnRedo);
            ControlsHelper.RegisterPostbackControl(btnUndo);
            ControlsHelper.RegisterPostbackControl(btnSave);
            ControlsHelper.RegisterPostbackControl(btnClose);

            btnSave.Click += btnSave_Click;
            btnClose.Click += btnClose_Click;
            btnUndo.Click += btnUndo_Click;
            btnRedo.Click += btnRedo_Click;

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
            ltlScript.Text = ScriptHelper.GetScript("setTimeout('window.skipCloseConfirm = true;Close()',0);");
            if (imageEditor.IsUndoRedoPossible())
            {
                ltlScript.Text += ScriptHelper.GetScript(GetRefresh());
            }
        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        TempFileInfoProvider.DeleteTempFiles(TempFileInfoProvider.IMAGE_EDITOR_FOLDER, imageEditor.InstanceGUID);
        ltlScript.Text = ScriptHelper.GetScript("setTimeout('window.skipCloseConfirm = true;Close()',0);");
    }

    #endregion


    #region "Private methods"

    private static string GetRefresh()
    {
        if (QueryHelper.GetBoolean("refresh", false))
        {
            string attachmentGuid = QueryHelper.GetString("attachmentguid", "");
            string filepath = QueryHelper.GetString("filepath", "");
            int nodeId = QueryHelper.GetInteger("nodeId", 0);

            if (!String.IsNullOrEmpty(filepath))
            {
                return String.Format("if (wopener.imageEdit_FileSystemRefresh) {{ wopener.imageEdit_FileSystemRefresh({0}); }}", ScriptHelper.GetString(String.Format("filepath|{0}", filepath)));
            }
            else if (nodeId > 0)
            {
                return String.Format("if (wopener.imageEdit_ContentRefresh) {{ wopener.imageEdit_ContentRefresh({0}); }}", ScriptHelper.GetString(String.Format("attachmentguid|{0}|nodeId|{1}", attachmentGuid, nodeId)));
            }
            else
            {
                return String.Format("if (wopener.imageEdit_AttachmentRefresh) {{ wopener.imageEdit_AttachmentRefresh('attachmentguid|{0}'); }}", ScriptHelper.GetString(attachmentGuid, false));
            }
        }

        return String.Empty;
    }

    #endregion
}
