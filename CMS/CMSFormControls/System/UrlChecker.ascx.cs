using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSFormControls_System_UrlChecker : FormEngineUserControl, ICallbackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Identifies error detail panel
    /// </summary>
    private const string STATUS_DETAIL_SUFFIX = "_detail";

    #endregion


    #region "Variables"

    private AsyncWorker mWorker;
    protected static Hashtable mResults = new Hashtable();

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets whole URL with suffix.
    /// </summary>
    public override object Value
    {
        get
        {
            return (!String.IsNullOrEmpty(txtDomain.Text) ? txtDomain.Text.Trim().TrimEnd('/') + (IncludeUrlSuffixInValue ? UrlSuffix : string.Empty) : String.Empty);
        }
        set
        {
            string domain = ValidationHelper.GetString(value, String.Empty);
            txtDomain.Text = (String.IsNullOrEmpty(UrlSuffix)) ? domain : domain.Replace(UrlSuffix, String.Empty);
        }
    }


    /// <summary>
    /// Gets ID of input control.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtDomain.ID;
        }
    }


    /// <summary>
    /// Indicates whether a detailed error report (i.e. exception message) should be displayed.
    /// </summary>
    public bool ShowDetailedError
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowDetailedError"), true);
        }
        set
        {
            SetValue("ShowDetailedError", value);
        }
    }


    /// <summary>
    /// Indicates whether the URL must contain HTTP or HTTPS protocol.
    /// </summary>
    public bool ProtocolIsRequired
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ProtocolIsRequired"), false);
        }
        set
        {
            SetValue("ProtocolIsRequired", value);
        }
    }


    /// <summary>
    /// Custom status error message.
    /// </summary>
    public string StatusErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StatusErrorMessage"), string.Empty);
        }
        set
        {
            SetValue("StatusErrorMessage", value);
        }
    }


    /// <summary>
    /// Indicates whether the control will append URL suffix as a part of the returned value in the Value property.
    /// </summary>
    public bool IncludeUrlSuffixInValue
    {
        get
        {
            return GetValue("IncludeUrlSuffixInValue", true);
        }
        set
        {
            SetValue("IncludeUrlSuffixInValue", value);
        }
    }


    /// <summary>
    /// Indicates if redirected responses are allowed from server
    /// </summary>
    public bool AllowAutoRedirect
    {
        get
        {
            return GetValue("AllowAutoRedirect", true);
        }
        set
        {
            SetValue("AllowAutoRedirect", value);
        }
    }


    /// <summary>
    /// URL suffix for complete server URL.
    /// </summary>
    public string UrlSuffix
    {
        get
        {
            return GetValue("UrlSuffix", String.Empty);
        }
        set
        {
            SetValue("UrlSuffix", value);
        }
    }


    /// <summary>
    /// Path to page under server which will checked.
    /// </summary>
    public string PagePath
    {
        get
        {
            return GetValue("PagePath", String.Empty);
        }
        set
        {
            SetValue("PagePath", value);
        }
    }


    /// <summary>
    /// Async worker.
    /// </summary>
    private AsyncWorker Worker
    {
        get
        {
            return mWorker ?? (mWorker = new AsyncWorker());
        }
    }


    /// <summary>
    /// Guid of currently processed process.
    /// </summary>
    private Guid CurrentProcessGUID
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlError.Visible = ShowDetailedError;
    }


    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void SetupControl()
    {
        #region ServerChecker

        // Register javascripts
        if (!RequestHelper.IsCallback())
        {
            // Ensure required scripts
            ScriptHelper.RegisterJQuery(Page);
            ScriptHelper.RegisterTooltip(Page);

            if (txtDomain != null)
            {
                string suffix = !String.IsNullOrEmpty(UrlSuffix) ? " + '" + UrlSuffix + "'" : String.Empty;

                StringBuilder script = new StringBuilder();
                script.Append(@"
var processGuid_", ClientID, @"='';

function UpdateStatusLabel_", ClientID, @"(value, context)
{
    var args = value.split('|'); 
    if(args[0] == 'true') 
    {
        var item = $cmsj('#", pnlStatus.ClientID, @"');
        var errorPanel = $cmsj('#", pnlError.ClientID, @"');

        if(item != null) 
        {
            item.replaceWith(args[1]); 
        }

        
        if (errorPanel != null)        
        {            
            if(args[2] && (args[2] != '')) 
            {
                errorPanel.html(args[2]); 
            }
            else
            {
                errorPanel.html('');
            }
        }
    }    
    else 
    { 
        if(args[1] != '') 
        { 
            processGuid_", ClientID, @"= value; 
            window.setTimeout(function() {", Page.ClientScript.GetCallbackEventReference(this, "processGuid_" + ClientID, "UpdateStatusLabel_" + ClientID, "null"), @"},1000);
        }
    }
}

function CheckServer_", ClientID, @"()
{ 
    var item = $cmsj('#", pnlStatus.ClientID, @"');
    item.text(", ScriptHelper.GetLocalizedString("urlchecker.processing"), @");
    $cmsj('#", pnlStatus.ClientID, STATUS_DETAIL_SUFFIX, @"').remove(); 
    var control = $cmsj('#", txtDomain.ClientID, @"'); 
    var textboxValue = control.val(); 

    if ((control.TextBoxWrapper != null) && control.TextBoxWrapper._isWatermarked) 
    { 
        textboxValue = '';
    } 

    var value='true|' + textboxValue ", suffix, @";
    ", Page.ClientScript.GetCallbackEventReference(this, "value", "UpdateStatusLabel_" + ClientID, "null"), @";
}");
                // Register custom scripts to page
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ServerChecker_" + ClientID, ScriptHelper.GetScript(script.ToString()));
            }
        }

        // Initialize controls
        btnCheckServer.Text = btnCheckServer.ToolTip = GetString("urlchecker.checkserver");
        btnCheckServer.Attributes.Add("onclick", "CheckServer_" + ClientID + "();return false;");

        #endregion

        if (!String.IsNullOrEmpty(UrlSuffix))
        {
            lblSuffix.Text = UrlSuffix;
            lblSuffix.Visible = true;
        }
        else
        {
            lblSuffix.Visible = false;
        }

        txtDomain.WatermarkText = GetString("urlchecker.specifyapproot");

        CheckMinMaxLength = true;
        CheckRegularExpression = true;

        // Set max length for textbox
        if (FieldInfo != null)
        {
            if (FieldInfo.DataType == FieldDataType.Text)
            {
                txtDomain.MaxLength = FieldInfo.Size - UrlSuffix.Length;
            }
        }
    }


    /// <summary>
    /// Check server specified with URL availability.
    /// </summary>
    /// <param name="parameter">Async worker parameter</param>
    protected void CheckServer(object parameter)
    {
        CheckServer(parameter.ToString());
    }


    /// <summary>
    /// Check server specified with URL availability.
    /// </summary>
    /// <param name="parameters">URL to be checked and result identifier</param>
    protected void CheckServer(string parameters)
    {
        string[] param = parameters.Split('|');
        string result = null;
        
        try
        {
            string url = param[0];

            // Resolve relative URL to full URL
            if (!String.IsNullOrEmpty(url) && url.StartsWithCSafe("~/"))
            {
                url = URLHelper.GetAbsoluteUrl(url, RequestContext.FullDomain, null, null);
            }

            // Check if protocol is contained in checked URL and if not complete it
            if (!ProtocolIsRequired && !URLHelper.ContainsProtocol(url))
            {
                // Check if running on secured layer
                if (!RequestContext.IsSSL)
                {
                    url = "http://" + url;
                }
                else
                {
                    url = "https://" + url;
                }
            }

            if (ProtocolIsRequired && !URLHelper.ContainsProtocol(url))
            {
                // Process result
                result = SetStatusText(GetString("urlchecker.protocolisrequired.error"));
            }
            else
            {
                SecurityHelper.EnsureCertificateSecurity();

                // Send HTTP request
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                if (!AllowAutoRedirect)
                {
                    request.AllowAutoRedirect = false;
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // Process result
                result = SetStatusText(response);

                response.Close();
            }
        }
        catch (Exception ex)
        {
            // Handling exceptions
            result = SetStatusText(ex);
        }
        finally
        {
            SetCurrentResult(param[1], result);
        }
    }


    /// <summary>
    /// Gets result of process with specified ID.
    /// </summary>
    /// <param name="id">ID of process</param>
    /// <returns>Result of process</returns>
    private string GetCurrentResult(string id)
    {
        return ValidationHelper.GetString(mResults["ServerCheckerResult_" + id], string.Empty);
    }


    /// <summary>
    /// Sets result of process with specified ID.
    /// </summary>
    /// <param name="id">ID of process</param>
    /// <param name="value">Value of result</param>
    private void SetCurrentResult(string id, string value)
    {
        mResults["ServerCheckerResult_" + id] = value;
    }


    /// <summary>
    /// Set status of URL check.
    /// </summary>
    /// <param name="response">Web response to obtain data about URL</param>
    /// <returns>URL status text</returns>
    private string SetStatusText(HttpWebResponse response)
    {
        string errorText;
        if ((response == null) || (response.StatusCode == HttpStatusCode.OK))
        {
            return SetStatusText();
        }
        else if (response.StatusCode == HttpStatusCode.Redirect)
        {
            errorText = GetErrorText("Response status code: Redirect found", ResHelper.GetString("stagingeditserver.serverurl.redirectfound"));
            return SetStatusText(errorText);
        }

        // If web response is available, obtain status from that
        errorText = GetErrorText("Response status code: " + response.StatusCode, response.StatusDescription);
        return SetStatusText(errorText);
    }


    /// <summary>
    /// Set status of URL check.
    /// </summary>
    /// <param name="exception">Exception thrown during obtaining URL status</param>
    /// <returns>URL status text</returns>
    private string SetStatusText(Exception exception)
    {
        if (exception == null)
        {
            return SetStatusText();
        }

        string errorText = GetErrorText(exception.Message, null);
        return SetStatusText(errorText);
    }


    /// <summary>
    /// Set status of URL check.
    /// </summary>
    /// <param name="error">Error obtained during checking URL status</param>
    /// <returns>URL status text</returns>
    private string SetStatusText(string error = null)
    {
        // Get rendered HTML
        var sb = new StringBuilder();
        var statusControls = GetStatusControls(error);
        foreach (WebControl ctrl in statusControls.Item1)
        {
            sb.Append(ctrl.GetRenderedHTML());
        }

        if (statusControls.Item2 != null)
        {
            sb.Append('|', statusControls.Item2.GetRenderedHTML());
        }

        return sb.ToString();
    }


    /// <summary>
    /// Get text in appropriate format usable for tooltip framework.
    /// </summary>
    /// <param name="error">Main error message</param>
    /// <param name="errorDescription">Error description giving more details about the source of the error</param>
    /// <returns>Resulting tooltip text in desired format</returns>
    private string GetErrorText(string error, string errorDescription)
    {
        const string errorTemplate = "<strong>{0}</strong><p>{1}</p>";
        return string.Format(errorTemplate, ScriptHelper.FormatTooltipString(error), ScriptHelper.FormatTooltipString(errorDescription));
    }


    /// <summary>
    /// Gets basic controls to render status HTML
    /// </summary>
    /// <param name="statusIconClass">Status icon CSS class</param>
    /// <param name="statusTextClass">Status text CSS class</param>
    /// <param name="statusText">Status text</param>
    private IEnumerable<WebControl> GetBasicStatusControls(string statusIconClass, string statusTextClass, string statusText)
    {
        List<WebControl> statusControls = new List<WebControl>();

        // Status icon for available URL
        CMSIcon statusIcon = new CMSIcon
        {
            CssClass = statusIconClass,
        };
        statusControls.Add(statusIcon);

        // Status text for available URL
        Label lblStatus = new Label
        {
            Text = statusText,
            CssClass = statusTextClass
        };
        statusControls.Add(lblStatus);

        return statusControls;
    }


    /// <summary>
    /// Gets controls to render status HTML
    /// </summary>
    /// <param name="error">Error which occurred during accessing the URL</param>
    /// <returns>Collection of controls used to render URL status HTML and control with error message if error occurs</returns>
    private Tuple<IEnumerable<WebControl>, WebControl> GetStatusControls(string error = null)
    {
        List<WebControl> statusControls = new List<WebControl>();
        IEnumerable<WebControl> basicControls;
        Panel pnlErrorDetail = null;

        // Get wrapping panel simulating status panel control
        Panel statusPanel = new Panel
        {
            CssClass = "status form-control-text",
            ID = pnlStatus.ClientID,
            ClientIDMode = ClientIDMode.Static
        };
        statusControls.Add(statusPanel);

        // Add controls for available URL
        if (string.IsNullOrEmpty(error))
        {
            // Get basic status controls for available URL
            basicControls = GetBasicStatusControls("status-icon-ok icon-check-circle", "status-text ok", GetString("general.ok"));
        }
        else
        {
            string errorMessage = String.IsNullOrWhiteSpace(StatusErrorMessage) ? GetString("general.error") : StatusErrorMessage;

            // Get basic status controls for not-available URL
            basicControls = GetBasicStatusControls("status-icon-error icon-times-circle", "status-text error", errorMessage);

            if (ShowDetailedError)
            {
                // Status detail text for not-available URL
                Literal ltlErrorDetail = new Literal
                {
                    Text = error,
                };

                // Status detail panel 
                pnlErrorDetail = new Panel
                {
                    CssClass = "status-error-detail form-control-text",
                    ID = pnlError.ClientID + STATUS_DETAIL_SUFFIX,
                    ClientIDMode = ClientIDMode.Static
                };
                pnlErrorDetail.Controls.Add(ltlErrorDetail);
            }
        }

        // Add basic status controls to status panel
        foreach (var ctrl in basicControls)
        {
            statusPanel.Controls.Add(ctrl);
        }

        return new Tuple<IEnumerable<WebControl>, WebControl>(statusControls, pnlErrorDetail);
    }


    /// <summary>
    /// Checks if specified process is currently running.
    /// </summary>
    /// <param name="process">Async process to check its status</param>
    private bool ProcessIsRunning(AsyncWorker process)
    {
        switch (process.Status)
        {
            // Process not running statuses
            case AsyncWorkerStatusEnum.Unknown:
            case AsyncWorkerStatusEnum.Stopped:
            case AsyncWorkerStatusEnum.Finished:
            case AsyncWorkerStatusEnum.Error:
                return false;

            default:
                return true;
        }
    }

    #endregion


    #region "Callback methods"

    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string result = GetCurrentResult(CurrentProcessGUID.ToString());

        // If worker finished return result
        if (!String.IsNullOrEmpty(result))
        {
            mResults.Remove("ServerCheckerResult_" + CurrentProcessGUID);
            return "true|" + result;
        }

        return "false|" + CurrentProcessGUID;
    }


    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        string[] args = eventArgument.Split('|');

        if (args.Length == 2)
        {
            // If not checking callback, run new request
            if (args[0].ToLowerCSafe() == "true")
            {
                if (!ProcessIsRunning(Worker))
                {
                    CurrentProcessGUID = Guid.NewGuid();
                    if (!String.IsNullOrEmpty(args[1]) && (args[1].ToLowerCSafe() != UrlSuffix.ToLowerCSafe()))
                    {
                        // Prepare URL
                        string parameter = args[1];
                        if (!String.IsNullOrEmpty(PagePath))
                        {
                            parameter = parameter.TrimEnd('/') + "//" + PagePath;
                        }

                        // Call async request to server
                        Worker.Parameter = parameter + "|" + CurrentProcessGUID;
                        Worker.RunAsync(CheckServer, WindowsIdentity.GetCurrent());
                    }
                    else
                    {
                        SetCurrentResult(CurrentProcessGUID.ToString(), GetString("urlchecker.urlnotavailable"));
                    }
                }
            }
            else
            {
                CurrentProcessGUID = new Guid(args[1]);
            }
        }
    }

    #endregion
}