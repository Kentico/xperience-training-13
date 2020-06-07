using System;
using System.Security.Principal;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Delete")]
public partial class CMSModules_Sites_Pages_Site_Delete : GlobalAdminPage, ICallbackEventHandler
{
    #region "Variables"
    
    private SiteInfo mSiteInfo;
    private string mBackToSiteListUrl;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Application instance GUID.
    /// </summary>
    public Guid ApplicationInstanceGUID
    {
        get
        {
            if (ViewState["ApplicationInstanceGUID"] == null)
            {
                ViewState["ApplicationInstanceGUID"] = SystemHelper.ApplicationInstanceGUID;
            }

            return ValidationHelper.GetGuid(ViewState["ApplicationInstanceGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Import process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Persistent settings key.
    /// </summary>
    private string PersistentLogKey
    {
        get
        {
            return "SiteDeletion_" + ProcessGUID + "_Log";
        }
    }


    /// <summary>
    /// Deletion info.
    /// </summary>
    private SiteDeletionLog SiteDeletionLog
    {
        get
        {
            SiteDeletionLog delLog = (SiteDeletionLog)PersistentStorageHelper.GetValue(PersistentLogKey);
            if (delLog == null)
            {
                throw new InvalidOperationException("Site deletion log has been lost.");
            }

            CheckApplicationState(delLog);

            return delLog;
        }
        set
        {
            PersistentStorageHelper.SetValue(PersistentLogKey, value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        InitAlertLabels();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        if (RequestHelper.IsCallback())
        {
            return;
        }

        if (!RequestHelper.IsPostBack())
        {
            SiteDeletionLog = new SiteDeletionLog();
            SiteDeletionLog.PersistentLogKey = PersistentLogKey;
        }

        // Register the script to perform get flags for showing buttons retrieval callback
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "GetState", ScriptHelper.GetScript("function GetState(cancel){ return " + Page.ClientScript.GetCallbackEventReference(this, "cancel", "SetStateMssg", null) + " } \n"));

        // Setup page title text and image
        PageTitle.TitleText = GetString("Site_Edit.DeleteSite");
        mBackToSiteListUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "Sites", false);

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("general.sites"),
            RedirectUrl = mBackToSiteListUrl,
            Target = "cmsdesktop",
        });

        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("Site_Edit.DeleteSite"),
        });

        mSiteInfo = SiteInfo.Provider.Get(QueryHelper.GetInteger("siteId", 0));
        if (mSiteInfo != null)
        {
            var siteDisplayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(mSiteInfo.DisplayName));

            ucHeader.Header = String.Format(GetString("Site_Delete.Header"), siteDisplayName);
            ucHeaderConfirm.Header = GetString("Site_Delete.HeaderConfirm");

            headConfirmation.Text = String.Format(GetString("Site_Edit.Confirmation"), siteDisplayName);
        }

        RegisterScripts();
    }
    

    private void RegisterScripts()
    {
        // Javascript functions
        string script = @"
var timerId = 0;
var i = 0;

function GetStateAction() {
    if (window.Activity) {
        window.Activity();
    }
    if (i++ >= 10) {
        i = 0;
        try {
            GetState(false);
        }
        catch (ex) {
        }
    }
}

// End timer function
function StopStateTimer() {
    if (timerId) {
        clearInterval(timerId);
        timerId = 0;
    }
}

// Start timer function
function StartStateTimer() {
    timerId = setInterval('GetStateAction()', 100);
}

function SetStateMssg(rValue, context) 
{
    var values = rValue.split('<#>');
    if((values[0]=='E') || (values[0]=='F') || values=='')
    {
        StopStateTimer();
        var actDiv = document.getElementById('actDiv');
        if (actDiv != null) { actDiv.style.display = 'none'; }
        BTN_Enable('" + btnOk.ClientID + @"');
    }
    if((values[0]=='E') && values[2] && (values[2].length > 0))
    {
        document.getElementById('" + lblError.ClientID + @"').innerHTML = values[2];
        document.getElementById('" + pnlError.ClientID + @"').style.removeProperty('display');
    }
    else if(values[0]=='I')
    {
        document.getElementById('" + lblLog.ClientID + @"').innerHTML = values[1];
    }
    else if((values=='') || (values[0]=='F'))
    {
        document.getElementById('" + lblLog.ClientID + @"').innerHTML = values[1];
    }
    if (values[3] && (values[3].length > 0))
    {
        document.getElementById('" + lblWarning.ClientID + @"').innerHTML = values[3];
        document.getElementById('" + pnlWarning.ClientID + @"').style.removeProperty('display');
    }
}";
        
        // Register the script to perform get flags for showing buttons retrieval callback
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "SiteDeletionScripts", ScriptHelper.GetScript(script));
    }

    #endregion


    #region "Control event handlers"

    protected void btnClick_BackToSiteList(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl(mBackToSiteListUrl));
    }


    protected void btnYes_Click(object sender, EventArgs e)
    {
        var worker = new AsyncWorker();
        if (worker.Status == AsyncWorkerStatusEnum.Running)
        {
            // Preventing parallel deletion of 2 and more sites because of the database transaction deadlocks
            ShowInformation(GetString("site_delete.alreadyrunning"));
            return;
        }

        SiteDeletionLog.LogDeletionState(LogStatusEnum.Info, String.Format(ResHelper.GetAPIString("Site_Delete.DeletingSite", "Initializing deletion of the site"), mSiteInfo.SiteName));

        pnlConfirmation.Visible = false;
        pnlDeleteSite.Visible = true;

        // Start the timer for the callbacks
        ltlScript.Text = ScriptHelper.GetScript("StartStateTimer();");

        var deletionSettings = new SiteDeletionSettings
        {
            DeleteAttachments = chkDeleteDocumentAttachments.Checked,
            DeleteMediaFiles = chkDeleteMediaFiles.Checked,
            DeleteMetaFiles = chkDeleteMetaFiles.Checked,
            Site = mSiteInfo
        };
        
        worker.RunAsync(_ => SiteInfoProvider.DeleteSiteInfo(deletionSettings, SiteDeletionLog), WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "ICallBackEventHandler methods"

    /// <summary>
    /// Callback event handler.
    /// </summary>
    /// <param name="argument">Callback argument</param>
    public void RaiseCallbackEvent(string argument)
    {
        hdnLog.Value = SiteDeletionLog.DeletionLog;
    }


    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        return hdnLog.Value;
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes (hides) alert labels
    /// </summary>
    private void InitAlertLabels()
    {
        // Do not use Visible property to hide this elements. They are used in JS.
        pnlError.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblError.Text) ? "none" : "block");
        pnlWarning.Attributes.CssStyle.Add(HtmlTextWriterStyle.Display, String.IsNullOrEmpty(lblWarning.Text) ? "none" : "block");
    }


    private void CheckApplicationState(SiteDeletionLog delLog)
    {
        if (ApplicationInstanceGUID == SystemHelper.ApplicationInstanceGUID)
        {
            return;
        }

        // Restart of the application
        LogStatusEnum progressLog = delLog.GetProgressState();
        if (progressLog != LogStatusEnum.Finish)
        {
            delLog.LogDeletionState(LogStatusEnum.UnexpectedFinish, ResHelper.GetAPIString("Site_Delete.Applicationrestarted", "<strong>Application has been restarted and the logging of the site delete process has been terminated. Please make sure that the site is deleted. If it is not, please repeate the deletion process.</strong><br />"));
        }
    }

    #endregion
}