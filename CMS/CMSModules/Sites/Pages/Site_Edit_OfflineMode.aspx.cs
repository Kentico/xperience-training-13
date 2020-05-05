using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Sites_Pages_Site_Edit_OfflineMode : GlobalAdminPage
{
    #region "Private variables"

    private int siteId = 0;
    private SiteInfo si = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        siteId = QueryHelper.GetInteger("siteid", 0);

        si = SiteInfoProvider.GetSiteInfo(siteId);
        EditedObject = si;

        if (!RequestHelper.IsPostBack())
        {
            // Load input fields for the first time
            txtMessage.Value = si.SiteOfflineMessage;
            txtURL.Text = si.SiteOfflineRedirectURL;

            radURL.Checked = !String.IsNullOrEmpty(txtURL.Text);
            radMessage.Checked = !String.IsNullOrEmpty(txtMessage.Value);
            if (!radURL.Checked && !radMessage.Checked)
            {
                radMessage.Checked = true;
            }
        }

        txtMessage.MediaDialogConfig.HideContent = true;

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "EnableDisableTextBox", ScriptHelper.GetScript(
            "function EnableDisableBox(elementId, state) \n" +
            "{ var elem = document.getElementById(elementId);  \n" +
            " if (elem) { \n" +
            "   if (state == 0) { elem.disabled = 'disabled'; elem.readonly = 'readonly'; } \n" +
            "   if (state == 1) { elem.disabled = ''; elem.readonly = ''; } \n" +
            " } \n" +
            "} \n"));

        radMessage.Attributes.Add("onclick", "EnableDisableBox('" + txtURL.ClientID + "', 0); EnableDisableBox('" + txtMessage.ClientID + "', 1);");
        radURL.Attributes.Add("onclick", "EnableDisableBox('" + txtURL.ClientID + "', 1); EnableDisableBox('" + txtMessage.ClientID + "', 0);");
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        CurrentMaster.DisplayControlsPanel = true;
        btnOK.Text = GetString("General.Save");

        // Init info message according to site mode (on-line/off-line)
        if (si.SiteIsOffline)
        {
            ShowInformation(GetString("sm.offlinemode.siteisoffline"));
            btnSubmit.Text = GetString("sm.offlinemode.btntakeonline");
            btnSubmit.ButtonStyle = ButtonStyle.Primary;
        }
        else
        {
            ShowInformation(GetString("sm.offlinemode.siteisonline"));
            btnSubmit.Text = GetString("sm.offlinemode.btntakeoffline");
            btnSubmit.ButtonStyle = ButtonStyle.Default;
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Save the data
        Save(false);
    }


    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        // Save the data and change offline status
        Save(true);
    }


    /// <summary>
    /// Saves the data and optionally changes site's offline status.
    /// </summary>
    /// <param name="changeOffline">Indicates whether to change offline status</param>
    protected void Save(bool changeOffline)
    {
        string message = txtMessage.Value;
        string url = txtURL.Text.Trim();

        // Check emptiness when site is going to off-line state
        if (changeOffline && !si.SiteIsOffline || !changeOffline && si.SiteIsOffline)
        {
            if (radURL.Checked && String.IsNullOrEmpty(url))
            {
                ShowError(GetString("sm.offlinemode.urlempty"));
                return;
            }
        }

        // Update site info
        if (radMessage.Checked)
        {
            si.SiteOfflineMessage = message;
            si.SiteOfflineRedirectURL = null;
        }
        else
        {
            si.SiteOfflineMessage = null;
            si.SiteOfflineRedirectURL = url;
        }

        if (changeOffline)
        {
            // Change the offline status
            si.SiteIsOffline = !si.SiteIsOffline;
        }

        SiteInfoProvider.SetSiteInfo(si);

        ShowChangesSaved();
    }
}
