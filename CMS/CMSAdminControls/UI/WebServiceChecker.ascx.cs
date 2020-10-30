using System;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Helpers;
using CMS.Base.Web.UI;
using CMS.UIControls;

public partial class CMSAdminControls_UI_WebServiceChecker : CMSUserControl, IPostBackEventHandler, IServiceChecker
{
    #region "Constants"

    /// <summary>
    /// Short link to help topic page regarding WCF configuration.
    /// </summary>
    private const string HELP_TOPIC_CONFIGURING_WCF_LINK = "wcf_config";


    private const string HELP_TOPIC_CONFIGURING_WCF_HTTPS_LINK = HELP_TOPIC_CONFIGURING_WCF_LINK;

    #endregion


    #region "Variables"

    // Default messages
    private string mNotAuthorizedText = "webservices.checker.unauthorized";
    private string mNotFoundText = "webservices.checker.unavailable";
    private string mDisabledText = "webservices.checker.disabled";
    private string mServerErrorText = "webservices.checker.servererror";

    #endregion


    #region "Properties"

    /// <summary>
    /// URL of service to be checked.
    /// </summary>
    public string ServiceUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Text displayed when user is not authorized to use the service.
    /// Might be caused by incorrect server configuration.
    /// </summary>
    public string NotAuthorizedText
    {
        get
        {
            string link = String.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_CONFIGURING_WCF_LINK), GetString("general.ourdocumentation"));
            return String.Format(ResHelper.GetString(mNotAuthorizedText), link);
        }
        set
        {
            mNotAuthorizedText = value;
        }
    }


    /// <summary>
    /// Text displayed when service is not available.
    /// </summary>
    public string NotFoundText
    {
        get
        {
            string link = String.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_CONFIGURING_WCF_LINK), GetString("general.ourdocumentation"));
            return String.Format(ResHelper.GetString(mNotFoundText), link);
        }
        set
        {
            mNotFoundText = value;
        }
    }


    /// <summary>
    /// Text displayed when service is disabled.
    /// </summary>
    public string DisabledText
    {
        get
        {
            return ResHelper.GetString(mDisabledText);
        }
        set
        {
            mDisabledText = value;
        }
    }


    /// <summary>
    /// Text displayed when server error occurs - most likely incorrect http/https configuration.
    /// </summary>
    public string ServerErrorText
    {
        get
        {
            string link = String.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_CONFIGURING_WCF_HTTPS_LINK), GetString("general.ourdocumentation"));
            return String.Format(ResHelper.GetString(mServerErrorText), link);
        }
        set
        {
            mServerErrorText = value;
        }
    }


    /// <summary>
    /// Placeholder for messages
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event raised when javascript service check detects problem with WCF.
    /// </summary>
    public event EventHandler OnCheckFailed;

    #endregion


    #region "Methods"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing || String.IsNullOrEmpty(ServiceUrl))
        {
            // Do nothing
            StopProcessing = true;
        }
        else if (!RequestHelper.IsPostBack())
        {
            // This message is hidden by javascript as soon as possible
            ShowWarning(ResHelper.GetString("webservices.checker.initialmessage"));

            string url = URLHelper.ResolveUrl(ServiceUrl.EndsWithCSafe("/js") ? ServiceUrl : ServiceUrl.TrimEnd('/') + "/js");

            // Prepare the script
            StringBuilder actionScript = new StringBuilder();
            actionScript.Append(@"
function CheckService(){
    var xmlHttp = new XMLHttpRequest();
    xmlHttp.open('GET', ", ScriptHelper.GetString(url), @", false);
    xmlHttp.send();
    return xmlHttp.status;
}

function CheckFailed(label, message, status){
    if (", (OnCheckFailed != null).ToString().ToLowerCSafe(), @") {
        ", ControlsHelper.GetPostBackEventReference(this, "##PARAM##").Replace("'##PARAM##'", "status"), @";
    } else if ((label != null) && (label.length > 0)) {
        label.empty();
        label.append(message);
        label.show();
    }
}

function PerformServiceCheck(){
    var label = $cmsj('#", MessagesPlaceHolder.ClientID, ClientIDSeparator, MessagesPlaceHolder.WarningLabel.ID, @"');
    label.hide();

    var status = CheckService();
    switch (status) {
        case 404:
            CheckFailed(label, ", ScriptHelper.GetString(NotFoundText), @", status);
            break;

        case 401:
            CheckFailed(label, ", ScriptHelper.GetString(NotAuthorizedText), @", status);
            break;

        case 403:
            CheckFailed(label, ", ScriptHelper.GetString(DisabledText), @", status);
            break;

        case 500:
            CheckFailed(label, ", ScriptHelper.GetString(ServerErrorText), @", status);
            break;

        default:
            if (", MessagesPlaceHolder.UseRelativePlaceHolder.ToString().ToLowerCSafe(), @") {
                label.parent().hide();
            }
    }
}");
            ScriptHelper.RegisterJQuery(Page);
            ScriptHelper.RegisterClientScriptBlock(Page, typeof (string), "WebServiceChecker_" + ClientID, ScriptHelper.GetScript(actionScript.ToString()));

            ScriptHelper.RegisterStartupScript(Page, typeof (string), "PerformServiceCheck_" + ClientID, "PerformServiceCheck();", true);
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Raise post-back event
    /// </summary>
    /// <param name="eventArgument">Argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (OnCheckFailed != null)
        {
            OnCheckFailed(this, null);
        }

        switch (eventArgument)
        {
            case "401":
                ShowWarning(NotAuthorizedText);
                break;

            case "403":
                ShowWarning(DisabledText);
                break;

            case "404":
                ShowWarning(NotFoundText);
                break;

            case "500":
                ShowWarning(ServerErrorText);
                break;
        }
    }

    #endregion
}
