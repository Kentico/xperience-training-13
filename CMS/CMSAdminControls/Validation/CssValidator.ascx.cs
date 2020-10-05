using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSAdminControls_Validation_CssValidator : DocumentValidator
{
    #region "Data class"

    [Serializable]
    private class ValidationData
    {
        public DataSet DataSource;

        public bool PostProcessingRequired;
    }

    #endregion


    #region "Constants"

    private const string DEFAULT_VALIDATOR_URL = "http://jigsaw.w3.org/css-validator/validator";
    private const int VALIDATION_DELAY = 1500;
    private const string EXCLUDED_CSS = ";designmode.css;bootstrap.css;";

    #endregion


    #region "Variables"

    private string mValidatorURL;
    private int mValidationDelay;
    private string mErrorText;
    private string mInfoText;
    private Regex mInlineStylesRegex;
    private Regex mLinkedStylesRegex;
    private CurrentUserInfo mCurrentUser;
    private string currentCulture = CultureHelper.DefaultUICultureCode;

    #endregion


    #region "Properties"

    /// <summary>
    /// URL to which validator requests will be sent
    /// </summary>
    public string ValidatorURL
    {
        get
        {
            return mValidatorURL ?? (mValidatorURL = DataHelper.GetNotEmpty(Service.Resolve<IAppSettingsService>()["CMSValidationCSSValidatorURL"], DEFAULT_VALIDATOR_URL));
        }
        set
        {
            mValidatorURL = value;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridValidationResult.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if server request  will be used rather than javascript request to obtain HTML
    /// </summary>
    public bool UseServerRequestType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets source of the data for unigrid control
    /// </summary>
    public override DataSet DataSource
    {
        get
        {
            var s = Data;
            if (s.DataSource == null)
            {
                s.DataSource = base.DataSource;
            }

            base.DataSource = s.DataSource;

            return s.DataSource;
        }
        set
        {
            var s = Data;

            s.DataSource = value;

            base.DataSource = s.DataSource;
        }
    }


    /// <summary>
    /// Current Error
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ctlAsyncLog.ProcessData.Error;
        }
        set
        {
            ctlAsyncLog.ProcessData.Error = value;
        }
    }


    /// <summary>
    /// Regular expression to get inline css styles
    /// </summary>
    private Regex InlineStylesRegex
    {
        get
        {
            return mInlineStylesRegex ?? (mInlineStylesRegex = RegexHelper.GetRegex("<style[^>]*>(?<comment><!--)?(?<css>[^<]*)(?(comment)-->)</style>", RegexOptions.Singleline));
        }
    }


    /// <summary>
    /// Regular expression to get linked css styles
    /// </summary>
    private Regex LinkedStylesRegex
    {
        get
        {
            return mLinkedStylesRegex ?? (mLinkedStylesRegex = RegexHelper.GetRegex("(?<link><link)?(?(link)[^>]*(?<type>type\\s*=\\s*(?<qc1>[\"']?)text/css(?(qc1)\\k<qc1>))?[^>]*href\\s*=\\s*(?<qc2>[\"'])|@import\\s*url\\s*(?<bracket>\\()?(?<qc3>[\"'])?(?=([^<])*</style))(?<url>(?(link)[^\"'>\\s]*|[^\"']*))(?(link)(?(qc2)\\k<qc2>)[^>]*(?(type)|(\\s*type\\s*=\\s*(?<qc1>[\"']?)text/css(?(qc1)\\k<qc1>))))", RegexOptions.Singleline));
        }
    }


    /// <summary>
    /// Key to store validation result
    /// </summary>
    protected override string ResultKey
    {
        get
        {
            return "validation|css|" + CultureCode + "|" + Url;
        }
    }


    /// <summary>
    /// Delay between validation requests to server
    /// </summary>
    private int ValidationDelay
    {
        get
        {
            if (mValidationDelay == 0)
            {
                mValidationDelay = ValidationHelper.GetInteger(Service.Resolve<IAppSettingsService>()["CMSValidationCSSValidatorDelay"], VALIDATION_DELAY);
            }
            return mValidationDelay;
        }
    }


    /// <summary>
    /// Validation data
    /// </summary>
    private ValidationData Data
    {
        get
        {
            var data = ctlAsyncLog.ProcessData.Data as ValidationData;
            if (data == null)
            {
                data = new ValidationData();
                ctlAsyncLog.ProcessData.Data = data;
                ctlAsyncLog.ProcessData.AllowUpdateThroughPersistentMedium = true;
            }

            return data;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Control methods"

    /// <summary>
    /// Page load 
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            DataSource = null;
        }

        // Configure controls
        SetupControls();

        if (RequestHelper.IsPostBack())
        {
            ProcessResult(DataSource);
        }
    }


    /// <summary>
    /// Page PreRender 
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(mErrorText))
        {
            ShowError(mErrorText);
        }

        if (!string.IsNullOrEmpty(mInfoText))
        {
            ShowInformation(mInfoText);
        }
    }


    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        IsLiveSite = false;

        InitializeScripts();

        // Set current UI culture
        currentCulture = CultureHelper.PreferredUICultureCode;

        // Initialize current user
        mCurrentUser = MembershipContext.AuthenticatedUser;

        // Initialize events
        ctlAsyncLog.OnFinished += ctlAsync_OnFinished;
        ctlAsyncLog.OnError += ctlAsync_OnError;
        ctlAsyncLog.OnCancel += ctlAsync_OnCancel;
        ctlAsyncLog.PostbackOnError = true;

        ctlAsyncLog.TitleText = GetString("validation.css.checkingcss");
        HeaderActions.ActionsList.Clear();

        // Validate action
        HeaderAction validate = new HeaderAction();
        validate.OnClientClick = "LoadHTMLToElement('" + hdnHTML.ClientID + "'," + ScriptHelper.GetString(Url) + ");";
        validate.Text = GetString("general.validate");
        validate.Tooltip = validate.Text;
        validate.CommandName = "validate";

        // View HTML code
        string click = GetViewSourceActionClick();

        HeaderAction viewCode = new HeaderAction();
        viewCode.OnClientClick = click;
        viewCode.Text = GetString("validation.viewcode");
        viewCode.Tooltip = viewCode.Text;
        viewCode.ButtonStyle = ButtonStyle.Default;

        // Show results in new window
        HeaderAction newWindow = new HeaderAction();
        newWindow.OnClientClick = click;
        newWindow.Text = GetString("validation.showresultsnewwindow");
        newWindow.Tooltip = newWindow.Text;
        newWindow.ButtonStyle = ButtonStyle.Default;

        if (DataHelper.DataSourceIsEmpty(DataSource))
        {
            newWindow.Enabled = false;
            newWindow.OnClientClick = null;
        }
        else
        {
            string encodedKey = ScriptHelper.GetString(HttpUtility.UrlEncode(ResultKey), false);
            newWindow.OnClientClick = String.Format("modalDialog('" + ResolveUrl("~/CMSModules/Content/CMSDesk/Validation/ValidationResults.aspx") + "?datakey={0}&docid={1}&hash={2}', 'ViewValidationResult', 800, 600);return false;", encodedKey, Node.DocumentID, QueryHelper.GetHash(String.Format("?datakey={0}&docid={1}", encodedKey, Node.DocumentID)));
        }

        // Add actions and set help topic
        HeaderActions.AddAction(validate);
        HeaderActions.AddAction(viewCode);
        HeaderActions.AddAction(newWindow);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Set sorting and add events
        gridValidationResult.OrderBy = "line";
        gridValidationResult.IsLiveSite = IsLiveSite;
        gridValidationResult.ZeroRowsText = GetString("validation.css.notvalidated");
        gridValidationResult.OnExternalDataBound += gridValidationResult_OnExternalDataBound;
        gridValidationResult.OnDataReload += gridValidationResult_OnDataReload;
        gridValidationResult.ShowActionsMenu = true;
        gridValidationResult.AllColumns = "line, context, message, source";
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "validate":
                Validate();
                break;
        }
    }


    protected DataSet gridValidationResult_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        DataSet ds = null;
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            if (Data.PostProcessingRequired)
            {
                ds = DocumentValidationHelper.PostProcessValidationData(DataSource, DocumentValidationEnum.CSS, null);
                Data.PostProcessingRequired = false;
            }
            else
            {
                ds = DataSource;
            }
        }

        return ds;
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    private void Validate()
    {
        pnlLog.Visible = true;
        DataSource = null;
        pnlGrid.Visible = false;

        CurrentError = string.Empty;

        // Get the full domain
        ctlAsyncLog.EnsureLog();
        var presentationUrl = new PresentationUrlRetriever().RetrieveForAdministration(CurrentSite.SiteID, CultureCode);
        ctlAsyncLog.Parameter = new Uri(presentationUrl).GetLeftPart(UriPartial.Authority) + ";" + presentationUrl + ";" + URLHelper.RemoveProtocolAndDomain(Url);
        ctlAsyncLog.RunAsync(CheckCss, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// On external databound event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Action what is called</param>
    /// <param name="parameter">Parameter</param>
    /// <returns>Result object</returns>
    protected object gridValidationResult_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        return GridExternalDataBound(sender, sourceName, parameter);
    }

    #endregion


    #region "Validation methods"

    /// <summary>
    /// Prepare Dictionary with requests for CSS validation
    /// </summary>
    /// <param name="parameter">Asynchronous parameter containing current url data to resolve absolute URL </param>
    private Dictionary<string, string> GetValidationRequests(string parameter)
    {
        string html = GetHtml(Url);
        Dictionary<string, string> cssRequests = null;
        string[] urlParams = parameter.Split(';');

        if (!String.IsNullOrEmpty(html))
        {
            cssRequests = new Dictionary<string, string>();

            // Get inline CSS
            AddLog(GetString("validation.css.preparinginline"));
            StringBuilder sbInline = new StringBuilder();
            foreach (Match m in InlineStylesRegex.Matches(html))
            {
                string captured = m.Groups["css"].Value;
                sbInline.AppendLine(captured);
            }

            cssRequests.Add(DocumentValidationHelper.InlineCSSSource, sbInline.ToString());

            // Get linked styles URLs
            foreach (Match m in LinkedStylesRegex.Matches(html))
            {
                string url = m.Groups["url"].Value;
                url = Server.HtmlDecode(url);

                if (!String.IsNullOrEmpty(url))
                {
                    bool processCss = true;
                    string[] excludedCsss = EXCLUDED_CSS.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    // Check if CSS is not excluded (CMS stylesheets)
                    foreach (string excludedCss in excludedCsss)
                    {
                        if (url.EndsWithCSafe(excludedCss, true))
                        {
                            processCss = false;
                            break;
                        }
                    }

                    if (processCss && !cssRequests.ContainsKey(url))
                    {
                        AddLog(String.Format(GetString("validation.css.preparinglinkedstyles"), url));

                        try
                        {
                            // Get CSS data from URL
                            string readUrl = DocumentValidationHelper.DisableMinificationOnUrl(URLHelper.GetAbsoluteUrl(url, urlParams[0], urlParams[1], urlParams[2]));

                            var request = WebRequest.CreateHttp(readUrl);
                            EnsureCertificateValidation(request);

                            using (var stream = request.GetResponse().GetResponseStream())
                            using (var reader = StreamReader.New(stream))
                            {
                                string css = reader.ReadToEnd();
                                if (!String.IsNullOrEmpty(css))
                                {
                                    cssRequests.Add(url, css.Trim(new[] { '\r', '\n' }));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Service.Resolve<IEventLogService>().LogException("CSSValidator", "GetValidationRequests", ex);
                        }
                    }
                }
            }
        }

        return cssRequests;
    }


    /// <summary>
    /// Get HTML code using server or client method
    /// </summary>
    /// <param name="url">URL to obtain HTML from</param>
    private string GetHtml(string url)
    {
        string html;
        if (UseServerRequestType)
        {
            // Create web client and try to obtain HTML using it
            using (WebClient client = new WebClient())
            {
                try
                {
                    using (var reader = StreamReader.New(client.OpenRead(url)))
                    {
                        html = reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    mErrorText = String.Format(ResHelper.GetString("validation.exception"), e.Message);
                    return null;
                }
            }
        }
        else
        {
            // Get HTML stored using javascript
            html = ValidationHelper.Base64Decode(hdnHTML.Value);
        }

        return RemoveVirtualContextDataFromUrls(html, CultureCode);
    }


    /// <summary>
    /// Send validation request to validator and obtain result 
    /// </summary>
    /// <param name="validationData">Validator parameters</param>
    /// <param name="parameter">Parameter</param>
    /// <returns>DataSet containing validator response</returns>
    private void GetValidationResults(Dictionary<string, string> validationData, string parameter)
    {
        DataSet dsResponse = null;
        List<string> validatedUrls = validationData.Keys.ToList();
        Random randGen = new Random();
        DataSet dsResult = DataSource = ((validationData.Count == 1) && string.IsNullOrEmpty(validationData[validatedUrls[0]])) ? new DataSet() : null;

        string source = null;
        int counter = 0;

        while (validatedUrls.Count > 0)
        {
            // Check if source is processed repeatedly
            if (source == validatedUrls[0])
            {
                counter++;
            }
            else
            {
                counter = 0;
            }

            // Set current source to validate
            source = validatedUrls[0];
            string cssData = validationData[source];
            validatedUrls.RemoveAt(0);

            if (!String.IsNullOrEmpty(cssData))
            {
                // Create web request
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(ValidatorURL);
                req.Method = "POST";
                // 1 second for timeout
                req.ReadWriteTimeout = req.Timeout = 10000;

                string boundary = "---------------------------" + randGen.Next(1000000, 9999999) + randGen.Next(1000000, 9999999);
                req.ContentType = "multipart/form-data; boundary=" + boundary;

                // Set data to web request for validation           
                byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(GetRequestData(GetRequestDictionary(cssData), boundary));
                req.ContentLength = data.Length;

                try
                {
                    AddLog(String.Format(GetString("validation.css.validatingcss"), source));

                    using (var stream = req.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    // Process server answer
                    using (var webResponse = (HttpWebResponse)req.GetResponse())
                    {
                        using (var response = webResponse.GetResponseStream())
                        {
                            if (response != null)
                            {
                                if (dsResult == null)
                                {
                                    dsResult = DataSource = new DataSet();
                                }

                                dsResponse = new DataSet();
                                dsResponse.ReadXml(response);
                                response.Close();
                            }
                        }
                        webResponse.Close();
                    }

                    string[] currentUrlValues = parameter.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["source"] = source;                    

                    DataTable dtResponse = DocumentValidationHelper.ProcessValidationResult(dsResponse, DocumentValidationEnum.CSS, parameters);

                    // Check if response contain any relevant data
                    if (!DataHelper.DataSourceIsEmpty(dtResponse))
                    {
                        // Add response data to validation DataSet
                        if (DataHelper.DataSourceIsEmpty(dsResult))
                        {
                            dsResult.Tables.Add(dtResponse);
                        }
                        else
                        {
                            dsResult.Tables[0].Merge(dtResponse);
                        }
                    }
                }
                catch (WebException)
                {
                    AddError(string.Format(GetString("validation.css.cssnotvalidated"), source));
                }
                catch
                {
                    if (counter < 5)
                    {
                        validatedUrls.Insert(0, source);
                    }
                    else
                    {
                        AddError(string.Format(GetString("validation.css.cssnotvalidated"), source));
                    }
                }
                finally
                {
                    req.Abort();
                    Thread.Sleep(ValidationDelay);
                }
            }
        }
    }


    /// <summary>
    /// Get dictionary with request parameters
    /// </summary>
    /// <param name="data">CSS data to be checked</param>
    private Dictionary<string, string> GetRequestDictionary(string data)
    {
        Dictionary<string, string> reqData = new Dictionary<string, string>();
        reqData.Add("text", data);
        reqData.Add("usermedium", "all");
        reqData.Add("type", "none");
        reqData.Add("warning", "1");
        reqData.Add("output", "soap12");
        reqData.Add("profile", "css3");
        return reqData;
    }


    /// <summary>
    /// Get request data which will be sent using HTTP request to validator
    /// </summary>
    /// <param name="data">Data to create </param>
    /// <param name="boundary">HTTP boundary string</param>
    private string GetRequestData(Dictionary<string, string> data, string boundary)
    {
        boundary = "--" + boundary;

        // Prepare beginning of the request data
        StringBuilder sbRequest = new StringBuilder();
        sbRequest.AppendLine(boundary);

        // Process request form data
        foreach (string key in data.Keys)
        {
            sbRequest.AppendFormat("Content-Disposition: form-data; name=\"{0}\"", key);
            sbRequest.AppendLine().AppendLine();
            sbRequest.AppendLine(data[key]);
            sbRequest.AppendLine(boundary);
        }
        string request = sbRequest.ToString();

        // Add final boundary dashes
        request = request.Insert(request.Length - 2, "--");
        return request;
    }


    /// <summary>
    /// Process validation results
    /// </summary>
    /// <param name="validationResult">DataSet with result of validation</param>
    public void ProcessResult(DataSet validationResult)
    {
        if (validationResult != null)
        {
            // Check if result is not empty
            if (!DataHelper.DataSourceIsEmpty(validationResult))
            {
                // Show validation errors
                ShowError(GetString("validation.css.resultinvalid"));
                lblResults.Visible = true;
                lblResults.Text = ResHelper.GetString("validation.validationresults");
                gridValidationResult.Visible = true;
            }
            else
            {
                // Show validation is valid
                ShowConfirmation(GetString("validation.css.resultvalid"));
                lblResults.Visible = false;
                gridValidationResult.Visible = false;
            }
        }
        else
        {
            // No results obtained during validation, show error
            lblResults.Visible = false;
            gridValidationResult.Visible = false;
            if (string.IsNullOrEmpty(mErrorText))
            {
                mErrorText = GetString("validation.errorinitialization");
            }
        }
    }


    /// <summary>
    /// Check document CSS
    /// </summary>
    /// <param name="parameter">Parameter containing data to resolve relative links to absolute</param>
    private void CheckCss(object parameter)
    {
        try
        {
            AddLog(ResHelper.GetString("validation.css.checkingcss", currentCulture));
            Dictionary<string, string> requests = GetValidationRequests(ValidationHelper.GetString(parameter, null));

            // Ensure thread doesn't finish to early in special situations 
            if ((requests == null) || (requests.Count <= 1))
            {
                Thread.Sleep(200);
            }

            if (requests != null)
            {
                GetValidationResults(requests, ValidationHelper.GetString(parameter, null));
                Data.PostProcessingRequired = true;
            }
            else
            {
                CurrentError = GetString("validation.diffdomainorprotocol");
            }
            pnlLog.Visible = false;
        }
        catch (ThreadAbortException ex)
        {
            if (CMSThread.Stopped(ex))
            {
                // When canceled
                AddLog(ResHelper.GetString("validation.css.abort", currentCulture));
                ctlAsyncLog.RaiseError(null, null);
            }
            else
            {
                mErrorText = ex.Message;
            }
        }
        catch (Exception ex)
        {
            mErrorText = ex.Message;
        }
    }

    #endregion


    #region "Handling async thread"

    /// <summary>
    /// On cancel event
    /// </summary>
    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsyncLog.Parameter = null;
        AddError(ResHelper.GetString("validation.validationcanceled"));

        ScriptHelper.RegisterStartupScript(this, typeof(string), "CancelLog", ScriptHelper.GetScript("var __pendingCallbacks = new Array();"));

        const string SEPARATOR = "<br />";
        int error = CurrentError.IndexOf(SEPARATOR, StringComparison.OrdinalIgnoreCase);

        mInfoText = CurrentError.Substring(0, error);
        mErrorText = CurrentError.Substring(error + SEPARATOR.Length);

        pnlLog.Visible = false;
        pnlGrid.Visible = true;

        Data.PostProcessingRequired = true;
    }


    /// <summary>
    /// On error event
    /// </summary>
    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsyncLog.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsyncLog.Stop();
        }

        ctlAsyncLog.Parameter = null;

        if (!string.IsNullOrEmpty(CurrentError))
        {
            mErrorText = CurrentError;
        }

        pnlLog.Visible = false;
        pnlGrid.Visible = true;
    }


    /// <summary>
    /// On finished event
    /// </summary>
    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(CurrentError))
        {
            mErrorText = CurrentError;
        }

        pnlLog.Visible = false;
        pnlGrid.Visible = true;
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        ctlAsyncLog.AddLog(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}
