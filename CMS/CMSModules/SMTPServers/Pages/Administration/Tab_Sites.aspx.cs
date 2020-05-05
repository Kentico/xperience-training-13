using System;
using System.Data;

using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_SMTPServers_Pages_Administration_Tab_Sites : CMSSMTPServersPage
{
    #region "Variables"

    private SMTPServerInfo smtpServer;
    private int smtpServerId;
    private string currentValues;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        smtpServerId = QueryHelper.GetInteger("objectid", 0);

        if (smtpServerId <= 0)
        {
            pnlAvailability.Visible = false;
            return;
        }

        EditedObject = smtpServer = SMTPServerInfoProvider.GetSMTPServerInfo(smtpServerId);

        if (!RequestHelper.IsPostBack())
        {
            if (smtpServer.ServerIsGlobal)
            {
                rblSites.SelectedIndex = 0;
                usSites.Enabled = false;
            }
            else
            {
                rblSites.SelectedIndex = 1;
                usSites.Enabled = true;
                LoadSiteBindings();
            }
        }

        usSites.OnSelectionChanged += usSites_OnSelectionChanged;
        rblSites.SelectedIndexChanged += rblSites_SelectedIndexChanged;
    }

    #endregion


    #region "Control event handlers"

    /// <summary>
    /// Radiobutton event handler.
    /// </summary>
    protected void rblSites_SelectedIndexChanged(object sender, EventArgs e)
    {
        usSites.Enabled = rblSites.SelectedValue == "site";

        // Add/remove global availability
        if (usSites.Enabled)
        {
            SMTPServerInfoProvider.DemoteSMTPServer(smtpServer);
        }
        else
        {
            SMTPServerInfoProvider.PromoteSMTPServer(smtpServer);
        }

        LoadSiteBindings();
    }


    /// <summary>
    /// Uniselector event handler.
    /// </summary>
    protected void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveSiteBindings();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Load control.
    /// </summary>
    private void LoadSiteBindings()
    {
        GetCurrentSites();
        usSites.Value = currentValues;
        usSites.Reload(true);
    }


    /// <summary>
    /// Loads current sites from DB.
    /// </summary>
    private void GetCurrentSites()
    {
        DataSet serverSites = SMTPServerSiteInfoProvider.GetSMTPServerSites().WhereEquals("ServerID", smtpServer.ServerID).OrderBy("SiteID");

        if (!DataHelper.DataSourceIsEmpty(serverSites))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(serverSites.Tables[0], "SiteID"));
        }
        else
        {
            currentValues = string.Empty;
        }
    }


    /// <summary>
    /// Save changes.
    /// </summary>
    private void SaveSiteBindings()
    {
        if (RequestHelper.IsPostBack())
        {
            GetCurrentSites();
        }

        string newValues = ValidationHelper.GetString(usSites.Value, null);
        RemoveOldRecords(newValues, currentValues);
        AddNewRecords(newValues, currentValues);
        currentValues = newValues;
    }


    /// <summary>
    /// Remove sites from SMTP server.
    /// </summary>
    private void RemoveOldRecords(string newValues, string currentValues)
    {
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] modifiedItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (modifiedItems != null)
            {
                foreach (string item in modifiedItems)
                {
                    SMTPServerSiteInfoProvider.RemoveSMTPServerFromSite(smtpServerId, ValidationHelper.GetInteger(item, 0));
                }
            }
        }
    }


    /// <summary>
    /// Add sites to SMTP server.
    /// </summary>
    private void AddNewRecords(string newValues, string currentValues)
    {
        string items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] modifiedItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (modifiedItems != null)
            {
                foreach (string item in modifiedItems)
                {
                    SMTPServerSiteInfoProvider.AddSMTPServerToSite(smtpServerId, ValidationHelper.GetInteger(item, 0));
                }
            }
        }
    }

    #endregion
}