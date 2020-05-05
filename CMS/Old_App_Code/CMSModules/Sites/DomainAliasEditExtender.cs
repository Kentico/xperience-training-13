using System;
using System.Data;

using CMS;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

[assembly: RegisterCustomClass("DomainAliasEditExtender", typeof(DomainAliasEditExtender))]

/// <summary>
/// Domain alias edit extender.
/// </summary>
public class DomainAliasEditExtender : ControlExtender<UIForm>
{
    #region "Variables"

    private SiteInfo siteInfo = null;
    private SiteDomainAliasInfo domainInfo = null;
    private bool runAfterSave;

    #endregion


    #region "Methods"

    /// <summary>
    /// Init event of the UI form.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;
        Control.OnBeforeSave += Control_OnBeforeSave;
        Control.OnAfterSave += Control_OnAfterSave;
        Control.OnAfterValidate += Control_OnAfterValidate;

        ComponentEvents.RequestEvents.RegisterForComponentEvent("DomainAliasEditCheckCollision", "StopSite", StopSiteAndSave);
    }


    /// <summary>
    /// AfterDataLoad event handler of UIForm.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    private void Control_OnAfterDataLoad(object sender, EventArgs e)
    {
        domainInfo = Control.EditedObject as SiteDomainAliasInfo;
        siteInfo = Control.UIContext.EditedObjectParent as SiteInfo;

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
        if (siteInfo != null)
        {
            // Check for domain name collision
            if (siteInfo.Status == SiteStatusEnum.Running)
            {
                string domainName = ValidationHelper.GetString(Control.GetFieldValue("SiteDomainAliasName"), String.Empty);
                SiteInfo runningSite = SiteInfoProvider.GetRunningSiteInfo(domainName, null);
                if ((runningSite != null) && (siteInfo.SiteID != runningSite.SiteID))
                {
                    Control.StopProcessing = true;

                    // Hide error message because user gets informed via confirmation
                    Control.MessagesPlaceHolder.Visible = false;

                    string postBackRef = ControlsHelper.GetPostBackEventReference(Control.ObjectManager.HeaderActions, "StopSite;");
                    string script = "if (confirm('" + ScriptHelper.GetString(ResHelper.GetStringFormat("sitedomain.proceedwithcollision", runningSite.DisplayName), false) + "')) { " + postBackRef + "; }";
                    ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "DomainCollisionMessage", ScriptHelper.GetScript(script));
                }
            }

            // Remember the state of the site
            runAfterSave = (siteInfo.Status == SiteStatusEnum.Running);
        }
    }


    /// <summary>
    /// Handle OnAfterSave event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnAfterSave(object sender, EventArgs e)
    {
        // Run site again if it was previously running
        if ((siteInfo != null) && runAfterSave)
        {
            string domainName = ValidationHelper.GetString(Control.GetFieldValue("SiteDomainAliasName"), String.Empty);
            DataSet ds = SiteInfoProvider.CheckDomainNameForCollision(domainName, siteInfo.SiteID);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                SiteInfo runningSite = SiteInfoProvider.GetSiteInfo(ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["SiteID"], 0));
                if (runningSite != null)
                {
                    string collisionSite = runningSite.DisplayName;
                    string collisionDomain = ValidationHelper.GetString(ds.Tables[0].Rows[0]["SiteDomainAliasName"], "");
                    Control.ShowError(String.Format(ResHelper.GetString("SiteDomain.RunError"), collisionSite, collisionDomain, siteInfo.DisplayName));
                }
            }
            else
            {
                // Try to re-run the site
                try
                {
                    SiteInfoProvider.RunSite(siteInfo.SiteName);
                }
                catch (RunningSiteException ex)
                {
                    Control.ShowError(ex.Message);
                }
            }
        }
    }


    /// <summary>
    /// Handle OnBeforeSave event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void Control_OnBeforeSave(object sender, EventArgs e)
    {
        if (siteInfo == null)
        {
            return;
        }

        // Remove protocol from the domain alias
        string newDomainName = ValidationHelper.GetString(Control.Data["SiteDomainAliasName"], String.Empty);
        newDomainName = URLHelper.RemoveProtocol(newDomainName);
        Control.Data["SiteDomainAliasName"] = newDomainName.Trim();

        // Ensure uniqueness of the domain name
        if (Control.IsInsertMode)
        {
            // Check duplicity
            if (SiteDomainAliasInfoProvider.DomainAliasExists(newDomainName, siteInfo.SiteID))
            {
                Control.StopProcessing = true;
                Control.ShowError(ResHelper.GetString("Site_Edit.AliasExists"));
            }
        }
        else
        {
            // Check duplicity
            SiteDomainAliasInfo existing = SiteDomainAliasInfoProvider.GetSiteDomainAliasInfo(newDomainName, siteInfo.SiteID);
            if ((existing != null) && (existing.SiteDomainAliasID != domainInfo.SiteDomainAliasID))
            {
                Control.StopProcessing = true;
                Control.ShowError(ResHelper.GetString("Site_Edit.AliasExists"));
                return;
            }

            // Stop the site before saving the domain alias
            if (siteInfo.Status == SiteStatusEnum.Running)
            {
                SiteInfoProvider.StopSite(siteInfo.SiteName);
                siteInfo.Status = SiteStatusEnum.Stopped;
                runAfterSave = true;
            }
        }
    }


    /// <summary>
    /// Handles the event that occurs when user confirms that the site domain alias object should be saved despite another site is using the same domain name.
    /// </summary>
    /// <param name="sender">Object sender</param>
    /// <param name="e">Event arguments</param>
    private void StopSiteAndSave(object sender, EventArgs e)
    {
        if (siteInfo != null)
        {
            // Stop the site in order to finish saving the domain alias
            siteInfo.Status = SiteStatusEnum.Stopped;
            siteInfo.Update();
            Control.SaveData(null);
        }
    }

    #endregion
}