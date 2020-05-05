using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_WebParts : GlobalAdminPage
{
    #region "Variables"

    /// <summary>
    /// PageTemplateID.
    /// </summary>
    protected int templateId = 0;

    private PageTemplateInfo pti;
    private bool dialog;

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        dialog = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));

        // Initialize controls.        
        ShowWarning(GetString("PageTemplate_WebParts.Warning"));
        lblWPConfig.Text = GetString("PageTemplate_WebParts.WebPartsConfiguration");

        // Get 'templateid' from querystring.
        templateId = QueryHelper.GetInteger("templateId", 0);

        pnlObjectLocking.ObjectManager.OnSaveData += ObjectManager_OnSaveData;
        pnlObjectLocking.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;

        pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
        EditedObject = pti;

        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }
    }


    private void LoadData()
    {
        if (pti != null)
        {
            txtWebParts.Text = HTMLHelper.ReformatHTML(pti.WebParts, "  ");
        }
    }


    /// <summary>
    /// Reloads data after save.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName != ComponentEvents.SAVE)
        {
            LoadData();
        }

        if (dialog)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
        }
    }


    /// <summary>
    /// Handle btnOK's OnClick event.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        string errorMessage = "";

        // Get PageTemplateInfo.
        pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
        if (pti != null)
        {
            // Update WebParts configuration in PageTemplate.
            try
            {
                pti.WebParts = txtWebParts.Text;
                PageTemplateInfoProvider.SetPageTemplateInfo(pti);
                ShowChangesSaved();

                // Update textbox value
                txtWebParts.Text = HTMLHelper.ReformatHTML(pti.WebParts, "  ");
            }
            catch (UnauthorizedAccessException ex)
            {
                errorMessage = ResHelper.GetStringFormat("general.sourcecontrolerror", ex.Message);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            // Show error message
            if (!String.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
        }
    }
}
