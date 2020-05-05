using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_WebAnalytics_Controls_ReportHeader : CMSAdminControl
{
    private string mPrintPageURL = "~/CMSModules/Reporting/Tools/Analytics_Print.aspx";
    private HitsIntervalEnum mSelectedInterval = HitsIntervalEnum.None;
    private string mPanelCssClass = "cms-edit-menu";
    private bool mPrintEnabled = true;
    private HeaderAction mPrintAction;
    private HeaderAction mSubscriptionAction;
    
    
    /// <summary>
    /// Header actions
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }


    /// <summary>
    /// CSS class for header actions container. Default class is cms-edit-menu.
    /// </summary>
    public string PanelCssClass
    {
        get
        {
            return mPanelCssClass;
        }
        set
        {
            mPanelCssClass = value;
        }
    }


    /// <summary>
    /// Datarow with report's parameters.
    /// </summary>
    public DataRow ReportParameters
    {
        get;
        set;
    }


    /// <summary>
    /// Report's name.
    /// </summary>
    public String ReportName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates report's interval (hour,day,week,month,year,none).
    /// </summary>
    public HitsIntervalEnum SelectedInterval
    {
        get
        {
            return mSelectedInterval;
        }
        set
        {
            mSelectedInterval = value;
        }
    }


    /// <summary>
    /// Gets or sets the print page URL for the print action.
    /// </summary>
    public string PrintPageURL
    {
        get
        {
            return mPrintPageURL;
        }
        set
        {
            mPrintPageURL = value;
        }
    }


    /// <summary>
    /// If false, button for print will be disabled.
    /// </summary>
    public bool PrintEnabled
    {
        get
        {
            return mPrintEnabled;
        }
        set
        {
            mPrintEnabled = value;
        }
    }
    

    public event CommandEventHandler ActionPerformed;
    

    protected void Page_Load(object sender, EventArgs e)
    {
        headerActions.ActionPerformed += OnActionPerformed;
        headerActions.PanelCssClass = PanelCssClass;

        // Create header actions
        var save = new SaveAction();
        headerActions.AddAction(save);

        // Print
        mPrintAction = new HeaderAction
        {
            Text = GetString("Analytics_Report.Print"),
            Enabled = PrintEnabled,
            ButtonStyle = ButtonStyle.Default,
        };
        headerActions.AddAction(mPrintAction);

        var cui = MembershipContext.AuthenticatedUser;

        // Report subscription enabled test
        var ri = ProviderHelper.GetInfoByName(PredefinedObjectType.REPORT, ReportName);
        if (ri != null)
        {
            var enableSubscription = ValidationHelper.GetBoolean(ri.GetValue("ReportEnableSubscription"), true);

            // Show enable subscription only for users with subscribe or modify.            
            enableSubscription &= (cui.IsAuthorizedPerResource("cms.reporting", "subscribe") || cui.IsAuthorizedPerResource("cms.reporting", "modify"));

            if (enableSubscription)
            {
                // Subscription
                mSubscriptionAction = new HeaderAction
                {
                    Text = GetString("notifications.subscribe"),
                    ButtonStyle = ButtonStyle.Default,
                };
                headerActions.AddAction(mSubscriptionAction);
            }
        }        
    }


    protected void OnActionPerformed(object sender, CommandEventArgs e)
    {
        if (ActionPerformed != null)
        {
            ActionPerformed(sender, e);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        var printDialogUrl = String.Format("{0}?reportname={1}&parameters={2}",
            ResolveUrl(PrintPageURL),
            ReportName,
            AnalyticsHelper.GetQueryStringParameters(ReportParameters));

        var printScript = String.Format("myModalDialog('{0}&UILang={1}&hash={2}','PrintReport {3}',800,700);return false",
            printDialogUrl,
            CultureInfo.CurrentUICulture.Name,
            QueryHelper.GetHash(printDialogUrl),
            ReportName);

        var subscriptionScript = String.Format("modalDialog('{0}?reportname={1}&parameters={2}&interval={3}','Subscription',{4},{5});return false",
            ResolveUrl("~/CMSModules/Reporting/Dialogs/EditSubscription.aspx"),
            ReportName,
            AnalyticsHelper.GetQueryStringParameters(ReportParameters),
            HitsIntervalEnumFunctions.HitsConversionToString(SelectedInterval),
            AnalyticsHelper.SUBSCRIPTION_WINDOW_WIDTH,
            AnalyticsHelper.SUBSCRIPTION_WINDOW_HEIGHT);

        var refreshScript = "function RefreshPage() {" + ControlsHelper.GetPostBackEventReference(this, "") + "};";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshScript", ScriptHelper.GetScript(refreshScript));

        // Register special script for print window
        ScriptHelper.RegisterPrintDialogScript(Page);
        ScriptHelper.RegisterDialogScript(Page);

        // Scripts have to be assigned when ReportName and ReportParameters are available!
        AssignClientScriptToAction(mPrintAction, printScript);
        AssignClientScriptToAction(mSubscriptionAction, subscriptionScript);
    }


    private static void AssignClientScriptToAction(HeaderAction action, string clientScript)
    {
        if (action != null)
        {
            action.OnClientClick = clientScript;
        }
    }
}
