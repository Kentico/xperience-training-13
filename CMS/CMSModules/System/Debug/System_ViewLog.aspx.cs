using System;

using CMS.Base;
using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_ViewLog : CMSDebugPage
{
    private CMSThread thread;
    private Guid threadGuid = Guid.Empty;


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.IsDialog = true;

        threadGuid = QueryHelper.GetGuid("threadGuid", Guid.Empty);
        string requestedServerName = QueryHelper.GetString("serverName", "");

        if (!RequestHelper.IsCallback())
        {
            // Set the title
            PageTitle.TitleText = GetString("ViewLog.Title");
            Page.Title = GetString("ViewLog.Title");

            if (WebFarmHelper.WebFarmEnabled && !String.IsNullOrEmpty(requestedServerName))
            {
                if (WebFarmHelper.ServerName.ToLowerCSafe() != requestedServerName.ToLowerCSafe())
                {
                    string loadMsg = ScriptHelper.GetLoaderInlineHtml(GetString("debug.contactrequestedserver"));
                    litMessage.Text = loadMsg;
                    litMessage.Visible = true;

                    RegisterRefreshScript();
                    return;
                }
            }
        }

        thread = CMSThread.GetThread(threadGuid);
        ctlAsync.ProcessGUID = threadGuid;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
        {
            if ((thread != null) && (thread.Log != null))
            {
                pnlLog.Visible = pnlCancel.Visible = true;
                
                btnCancel.Text = GetString("general.cancel");
                btnCancel.OnClientClick = "if(confirm(" + ScriptHelper.GetLocalizedString("ViewLog.CancelPrompt") + ")) { " + ctlAsync.GetCancelScript(true) + "} " + "return false;";

                ctlAsync.AttachToThread(thread);
            }
            else
            {
                pnlLog.Visible = false;
                
                ShowError(GetString("ViewLog.ThreadNotRunning"));
            }

            ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
        }
    }


    /// <summary>
    /// On finished event
    /// </summary>
    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        ShowConfirmation(GetString("ViewLog.ThreadFinished"), true);
        btnCancel.Visible = false;
    }


    /// <summary>
    /// On cancel event
    /// </summary>
    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ShowConfirmation(GetString("ViewLog.ThreadCanceled"), true);
        btnCancel.Visible = false;
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        if ((thread != null) && (thread.Log != null))
        {
            ctlAsync.LogContext = thread.Log;
        }
    }


    private void RegisterRefreshScript()
    {
        ScriptHelper.RegisterJQuery(Page);

        StringBuilder script = new StringBuilder();

        script.Append(@"
var refreshTimeout_", ClientID, @";
$cmsj(function () 
{ 
    if (refreshTimeout_", ClientID, @") 
    { 
        clearTimeout(refreshTimeout_", ClientID, @"); 
    } 

    refreshTimeout_", ClientID, @" = setTimeout(function ()
    {
        ", ClientScript.GetPostBackEventReference(btnHiddenRefresh, ""), @";
    },
        1000
    );
});"
        );

        ScriptHelper.RegisterStartupScript(this, typeof(string), "ViewLog_" + ClientID, ScriptHelper.GetScript(script.ToString()));
    }

    #endregion
}
