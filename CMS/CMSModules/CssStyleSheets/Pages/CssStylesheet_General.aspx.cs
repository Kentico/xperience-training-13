using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_CssStylesheets_Pages_CssStylesheet_General : CMSDeskPage
{
    #region "Variables"

    protected int cssStylesheetId;
    private CssStylesheetInfo si;
    private SiteInfo mSite;
    private bool isDialog;

    #endregion


    #region "Properties"

    /// <summary>
    /// Site info object, used for test whether css sheet belongs to site
    /// </summary>
    private SiteInfo CurrentSite
    {
        get
        {
            if (mSite == null)
            {
                int siteId = QueryHelper.GetInteger("siteid", 0);
                if (siteId > 0)
                {
                    mSite = SiteInfoProvider.GetSiteInfo(siteId);
                }
                if (mSite == null)
                {
                    mSite = SiteContext.CurrentSite;
                }
            }
            return mSite;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;

        if (!currentUser.IsAuthorizedPerResource("CMS.Design", "ReadCMSCSSStylesheet"))
        {
            RedirectToAccessDenied("CMS.Design", "ReadCMSCSSStylesheet");
        }

        // Page has been opened in CMSDesk and only stylesheet style editing is allowed
        isDialog = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));

        // Check for UI permissions
        bool isUserAuthorizedPagesApp = currentUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "Properties", "Properties.General", "General.Design", "Design.EditCSSStylesheets" }, SiteContext.CurrentSiteName);
        bool isUserAuthorizedCssStylesheetsApp = currentUser.IsAuthorizedPerUIElement("CMS.Design", new string[] { "EditStylesheet", "StylesheetGeneral" }, SiteContext.CurrentSiteName);
        if (!isUserAuthorizedPagesApp && !isUserAuthorizedCssStylesheetsApp)
        {
            var uiElement = isDialog ? "EditStylesheet;StylesheetGeneral" : "Properties;Properties.General;General.Design;Design.EditCSSStylesheets";
            RedirectToUIElementAccessDenied("CMS.Content", uiElement);
        }

        // Prevent replacing of the master page with dialog master page
        RequiresDialog = false;

        if (isDialog)
        {
            // Check hash
            if (!QueryHelper.ValidateHash("hash", "objectid", null, true))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
            }

            // Check 'Modify CSS stylesheets' permission 
            if (!currentUser.IsAuthorizedPerResource("CMS.Design", "ModifyCMSCSSStylesheet"))
            {
                RedirectToAccessDenied("CMS.Design", "ModifyCMSCSSStylesheet");
            }
        }

        string stylesheet = QueryHelper.GetString("objectid", "0");

        // If default stylesheet defined and selected, choose it
        if (stylesheet == "default")
        {
            si = PortalContext.CurrentSiteStylesheet;
        }

        // Default stylesheet not selected try to find stylesheet selected
        if (si != null)
        {
            cssStylesheetId = si.StylesheetID;
        }
        else
        {
            cssStylesheetId = ValidationHelper.GetInteger(stylesheet, 0);
            if (cssStylesheetId > 0)
            {
                // Get the stylesheet
                si = CssStylesheetInfoProvider.GetCssStylesheetInfo(cssStylesheetId);
            }
        }

        SetEditedObject(si, null);

        // Check site association in case that user is not admin
        var checkSiteAssociation = !currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);

        if ((si != null) && isDialog && checkSiteAssociation)
        {
            // Check if stylesheet is under current site
            int siteId = (CurrentSite != null) ? CurrentSite.SiteID : 0;
            DataSet ds = CssStylesheetSiteInfoProvider.GetCssStylesheetSites()
                .WhereEquals("SiteID", siteId)
                .WhereEquals("StylesheetID", si.StylesheetID)
                .TopN(1);

            if (DataHelper.DataSourceIsEmpty(ds))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("cssstylesheet.errorediting", "cssstylesheet.notallowedtoedit"));
            }
        }

        base.OnPreInit(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register client ID for autoresize of codemirror
        ucHierarchy.RegisterEnvelopeClientID();
    }


    protected override void CreateChildControls()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        if (si != null)
        {
            UIContext.EditedObject = si;
            ucHierarchy.PreviewObjectName = si.StylesheetName;
        }

        ucHierarchy.IgnoreSessionValues = isDialog;
        ucHierarchy.ShowPanelSeparator = true;
        ucHierarchy.StorePreviewScrollPosition = true;
        ucHierarchy.DefaultPreviewPath = "/";

        // Prevent displaying footer in dialog mode
        ucHierarchy.DialogMode = isDialog;

        base.CreateChildControls();
    }

    #endregion
}
