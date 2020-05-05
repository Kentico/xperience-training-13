using System;
using System.Data;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.Search;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("SiteEditPageExtender", typeof(SiteEditPageExtender))]

/// <summary>
/// Site edit page extender.
/// </summary>
public class SiteEditPageExtender : ControlExtender<UIForm>
{
    #region "Variables"

    private SiteInfo siteInfo = null;
    private bool runSite;

    #endregion


    #region "Methods"

    /// <summary>
    /// Extender initialization.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;
        Control.OnAfterSave += Control_OnAfterSave;
        Control.OnBeforeSave += Control_OnBeforeSave;
        Control.Page.PreRenderComplete += Page_PreRenderComplete;
        Control.OnAfterValidate += Control_OnAfterValidate;
        ComponentEvents.RequestEvents.RegisterForComponentEvent("SiteEditCheckCollision", "StopSite", StopSiteAndSave);
        ComponentEvents.RequestEvents.RegisterForComponentEvent("SiteEditRebuildIndex", "RebuildSiteIndex", RebuildSiteIndex);
    }


    /// <summary>
    /// AfterDataLoad event handler of UIForm.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    private void Control_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Store edited site object in local variable
        siteInfo = Control.EditedObject as SiteInfo;

        // Set SiteID of SiteCultureSelector in order to offer only cultures assigned to the edited site.
        FormEngineUserControl visitorCultureControl = Control.FieldControls["SiteDefaultVisitorCulture"];
        if (visitorCultureControl != null)
        {
            visitorCultureControl.SetValue("SiteID", siteInfo.SiteID);
        }
    }


    /// <summary>
    /// Handles OnAfterValidate event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnAfterValidate(object sender, EventArgs e)
    {
        // Check for domain name collision
        if (siteInfo.Status == SiteStatusEnum.Running)
        {
            string domainName = ValidationHelper.GetString(Control.GetFieldValue("SiteDomainName"), String.Empty);
            DataSet ds = SiteInfoProvider.CheckDomainNameForCollision(domainName, siteInfo.SiteID);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                int collisionSiteID = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["SiteID"], 0);
                SiteInfo collisionSite = SiteInfoProvider.GetSiteInfo(collisionSiteID);

                if (collisionSite != null)
                { 
                    Control.StopProcessing = true;

                    // Hide error message because user gets informed via confirmation
                    Control.MessagesPlaceHolder.Visible = false;
                    
                    string postBackRef = ControlsHelper.GetPostBackEventReference(Control.ObjectManager.HeaderActions, "StopSite;");
                    string script = "if (confirm('" + ScriptHelper.GetString(ResHelper.GetStringFormat("sitedomain.proceedwithcollision", collisionSite.DisplayName), false) + "')) { " + postBackRef + "; } ";
                    ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "DomainCollisionMessage", ScriptHelper.GetScript(script));
                }
            }
        }

        // Remember the state of the site
        runSite = (siteInfo.Status == SiteStatusEnum.Running);
    }


    /// <summary>
    /// Handle PreRenderComplete event of the page.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Page_PreRenderComplete(object sender, EventArgs e)
    {
        if ((siteInfo != null) && !RequestHelper.IsPostBack())
        {
            // Check version limitations
            if (!CultureSiteInfoProvider.LicenseVersionCheck(siteInfo.DomainName, FeatureEnum.Multilingual, ObjectActionEnum.Edit))
            {
                Control.ShowError(ResHelper.GetString("licenselimitation.siteculturesexceeded"));
                
                // Disable culture selector
                FormEngineUserControl cultureSelector = Control.FieldControls["SiteDefaultVisitorCulture"];
                if (cultureSelector != null)
                {
                    cultureSelector.Enabled = false;
                }

                // Disable Save button
                HeaderActions actions = Control.ObjectManager.HeaderActions;
                SaveAction sa = actions.ActionsList.Where<HeaderAction>(a => (a is SaveAction)).First() as SaveAction;
                if (sa != null)
                {
                    sa.Enabled = false;
                    sa.BaseButton.Visible = true;
                    actions.ReloadData();
                    Control.SubmitButton.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Handles OnBeforeSave event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnBeforeSave(object sender, EventArgs e)
    {
        string newSiteCodeName = ValidationHelper.GetString(Control.GetFieldValue("SiteName"), String.Empty);
        SiteInfo existingSiteInfo = SiteInfoProvider.GetSiteInfo(newSiteCodeName);

        // Ensure uniqueness of the site code name
        if ((existingSiteInfo != null) && (existingSiteInfo.SiteID != siteInfo.SiteID))
        {
            Control.StopProcessing = true;
            Control.AddError(ResHelper.GetString("Administration-Site_Edit.SiteExists"));
        }

        // Correct domain name
        string domainName = ValidationHelper.GetString(Control.Data["SiteDomainName"], String.Empty);
        if (!String.IsNullOrEmpty(domainName))
        {
            Control.Data["SiteDomainName"] = URLHelper.RemoveProtocol(domainName);
        }

        // Stop the site
        siteInfo.Status = SiteStatusEnum.Stopped;
    }


    /// <summary>
    /// Handles OnAfterSave event of the UI Form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnAfterSave(object sender, EventArgs e)
    {
        string newSiteCodeName = ValidationHelper.GetString(Control.GetFieldValue("SiteName"), String.Empty);

        if (newSiteCodeName.ToLowerCSafe() != siteInfo.SiteName.ToLowerCSafe())
        {
            // Clear settings if sitename changes
            ProviderHelper.ClearHashtables(SettingsKeyInfo.OBJECT_TYPE, true);

            if (SearchIndexInfoProvider.SearchEnabled)
            {
                Control.ShowInformation(String.Format(ResHelper.GetString("general.changessaved") + " " + ResHelper.GetString("srch.indexrequiresrebuild"), "<a href=\"javascript:" + ControlsHelper.GetPostBackEventReference(Control.ObjectManager.HeaderActions, "RebuildSiteIndex;") + "\">" + ResHelper.GetString("General.clickhere") + "</a>"));
            }
        }

        // Remove cached cultures for site
        CultureSiteInfoProvider.ClearSiteCultures(true);

        // Run the site if it was running previously
        try
        {
            if (runSite)
            {
                DataSet ds = SiteInfoProvider.CheckDomainNameForCollision(siteInfo.DomainName, siteInfo.SiteID);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    SiteInfo runningSiteInfo = SiteInfoProvider.GetSiteInfo(ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["SiteID"], 0));
                    if (runningSiteInfo != null)
                    {
                        string collisionSite = HTMLHelper.HTMLEncode(runningSiteInfo.DisplayName);
                        string collisionDomain = HTMLHelper.HTMLEncode(ValidationHelper.GetString(ds.Tables[0].Rows[0]["SiteDomainAliasName"], ""));

                        Control.ShowWarning(String.Format(ResHelper.GetString("SiteDomain.RunError"), collisionSite, collisionDomain, HTMLHelper.HTMLEncode(siteInfo.DisplayName)));
                    }
                }
                else
                {
                    // Run current site
                    SiteInfoProvider.RunSite(siteInfo.SiteName);
                }
            }
        }
        catch (RunningSiteException ex)
        {
            Control.ShowError(ex.Message);
        }
    }


    /// <summary>
    /// Handles the event that occurs when user confirms that the site object should be saved despite another site is using the same domain name.
    /// </summary>
    /// <param name="sender">Object sender</param>
    /// <param name="e">Event arguments</param>
    private void StopSiteAndSave(object sender, EventArgs e)
    {
        // Stop the site in order to finish saving the site
        siteInfo.Status = SiteStatusEnum.Stopped;
        Control.SaveData(null);
    }


    /// <summary>
    /// Handles the event that occurs when link button "click here" is clicked.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    private void RebuildSiteIndex(object sender, EventArgs e)
    {
        // Rebuild search index
        if (SearchIndexInfoProvider.SearchEnabled)
        {
            SearchIndexInfoProvider.RebuildSiteIndexes(siteInfo.SiteID);
            Control.ShowInformation(ResHelper.GetString("srch.index.rebuildstarted"));
        }
    }

    #endregion
}