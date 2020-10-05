using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;

using Newtonsoft.Json;

public partial class CMSAdminControls_Validation_HTMLValidator : DocumentValidator
{
    #region "Constants"

    private const string DEFAULT_VALIDATOR_URL = "https://validator.w3.org/nu/?out=json";

    #endregion


    #region "Nested classes"

    private enum MessageTypeEnum
    {
        [EnumStringRepresentation("error")]
        Error,

        [EnumStringRepresentation("info")]
        Info
    }


    /// <summary>
    /// Represents the strong-typed validation results.
    /// </summary>
    private class HtmlValidationResult
    {
        /// <summary>
        /// A collection of validation messages.
        /// </summary>
        public List<HtmlValidationMessage> Messages { get; set; }
    }


    /// <summary>
    /// Represents a single validation message that contains structured information.
    /// </summary>
    private class HtmlValidationMessage
    {
        /// <summary>
        /// Type of message. Possible values are: <c>error</c>, <c>info</c> and <c>non-document-error</c>.
        /// This item is mandatory and should always be present in the results.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Indicates the first line onto which the source range associated with the message falls. If the attribute is missing, it is assumed to have the same value as <see cref="LastLine"/>
        /// </summary>
        public int FirstLine { get; set; }

        /// <summary>
        /// Indicates the last line (inclusive) onto which the source range associated with the message falls.
        /// </summary>
        public int LastLine { get; set; }

        /// <summary>
        /// Indicates the last column (inclusive) onto which the source range associated with the message falls on the last line onto which is falls.
        /// </summary>
        public int LastColumn { get; set; }

        /// <summary>
        /// Indicates the first column onto which the source range associated with the message falls on the first line onto which is falls.
        /// </summary>
        public int FirstColumn { get; set; }

        /// <summary>
        /// Represents a paragraph of text (suitable for rendering to the user as plain text without further processing) that is the message stated succinctly in natural language.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Represents an extract of the document source from around the point in source designated for the message by the "line" and "column" numbers.
        /// </summary>
        public string Extract { get; set; }

        /// <summary>
        /// Designates the starting point of highlighting at the <see cref="Extract"/>.
        /// </summary>
        public int HiliteStart { get; set; }

        /// <summary>
        /// Indicates the range of highlighting in <see cref="Extract"/>.
        /// </summary>
        public int HiliteLength { get; set; }
    }

    #endregion


    #region "Variables"

    private string mValidatorURL;
    private string mErrorText;

    #endregion


    #region "Properties"

    /// <summary>
    /// URL to which validator requests will be sent
    /// </summary>
    public string ValidatorURL
    {
        get
        {
            return mValidatorURL ?? (mValidatorURL = DEFAULT_VALIDATOR_URL);
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
    /// Key to store validation result
    /// </summary>
    protected override string ResultKey
    {
        get
        {
            return "validation|html|" + CultureCode + "|" + Url;
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
    }


    /// <summary>
    /// Page PreRender
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        IsLiveSite = false;

        InitializeScripts();

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

        HeaderActions.AddAction(validate);
        HeaderActions.AddAction(viewCode);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        chkErrorsOnly.ResourceString = "validation.html.showerrorsonly";
        chkErrorsOnly.AddCssClass("dont-check-changes");

        // Set sorting and add events
        gridValidationResult.OrderBy = "line ASC";
        gridValidationResult.IsLiveSite = IsLiveSite;
        gridValidationResult.OnExternalDataBound += GridExternalDataBound;
        gridValidationResult.OnExternalDataBound += gridValidationResult_OnExternalDataBound;
        gridValidationResult.OnDataReload += gridValidationResult_OnDataReload;
        gridValidationResult.GridView.RowDataBound += GridView_RowDataBound;
        gridValidationResult.ZeroRowsText = GetString("validation.html.notvalidated");
        gridValidationResult.ShowActionsMenu = true;
        gridValidationResult.AllColumns = "line, column, message, source";
        gridValidationResult.DelayedReload = true;
        gridValidationResult.RememberState = false;

        // Set custom validating text
        up.ProgressText = GetString("validation.validating");
    }


    private void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var dataRow = UniGridFunctions.GetDataRowView(e.Row);
            string messageType = ValidationHelper.GetString(dataRow["type"], String.Empty);

            if (EnumStringRepresentationExtensions.ToEnum<MessageTypeEnum>(messageType) == MessageTypeEnum.Error)
            {
                e.Row.CssClass = "error";
            }
        }
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


    /// <summary>
    /// Actions handler.
    /// </summary>
    private void Validate()
    {
        DataSource = null;
        DataSource = ValidateHtml();
    }


    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData()
    {
        gridValidationResult.ReloadData();
        UpdateControlsVisibility();
    }


    protected DataSet gridValidationResult_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        return GetDataSource();
    }


    /// <summary>
    /// On external data-bound event handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Action what is called</param>
    /// <param name="parameter">Parameter</param>
    /// <returns>Result object</returns>
    protected object gridValidationResult_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLower(CultureHelper.EnglishCulture))
        {
            case "source":
                if (parameter is DBNull)
                {
                    return parameter;
                }

                const string LINE_BREAK_SIGN = "\u21A9";
                string source = (string)parameter;
                var drv = UniGridFunctions.GetDataRowView(sender as DataControlFieldCell);
                int highlightStartIndex = Convert.ToInt32(drv["highlightStart"]);
                int highlightLength = Convert.ToInt32(drv["highlightLength"]);
                var messageType = EnumStringRepresentationExtensions.ToEnum<MessageTypeEnum>(Convert.ToString(drv["type"]));

                // Get parts to be able to highlight code in the code extract
                IEnumerable<string> parts = new List<string>(new[]
                {
                    source.Substring(0, highlightStartIndex),
                    source.Substring(highlightStartIndex, highlightLength),
                    source.Substring(highlightStartIndex + highlightLength)
                });

                // HTML encode each part
                parts = parts.Select(HttpUtility.HtmlEncode).ToList();

                // Update the second (~ highlighted) part
                var partList = parts as List<string>;
                partList[1] = $"<strong class=\"{messageType.ToStringRepresentation()}\">{partList[1]}</strong>";

                source = String.Concat(parts);
                source = source.Replace("\n", LINE_BREAK_SIGN);

                return $@"<div class=""Source"">{source}</div>";

            case "message":
                return HttpUtility.HtmlEncode(parameter);

            case "type":
                var typeEnum = EnumStringRepresentationExtensions.ToEnum<MessageTypeEnum>((string)parameter);
                return typeEnum.ToLocalizedString("validation.html.messagetype");
        }

        return parameter;
    }

    #endregion


    #region "Validation request methods"

    /// <summary>
    /// Send validation request to validator and obtain result
    /// </summary>
    /// <param name="documentHtml">Validator parameters</param>
    /// <returns>DataSet containing validator response</returns>
    private DataSet GetValidationResult(string documentHtml)
    {
        try
        {
            var dsValResult = new DataSet();

            // Create web request
            var req = WebRequest.CreateHttp(ValidatorURL);
            req.Method = WebRequestMethods.Http.Post;
            req.UserAgent = HttpContext.Current.Request.UserAgent;
            req.ContentType = "text/html; charset=utf-8";

            byte[] data = Encoding.UTF8.GetBytes(documentHtml);
            req.ContentLength = data.Length;

            EnsureCertificateValidation(req);            

            using (var requestStream = req.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }


            // Process server answer
            using (var answer = req.GetResponse().GetResponseStream())
            using (var reader = StreamReader.New(answer, Encoding.UTF8))
            {
                string responseString = reader.ReadToEnd();
                var js = JsonConvert.DeserializeObject<HtmlValidationResult>(responseString);
                dsValResult = GetDataSetFromResult(js);
            }

            return dsValResult;
        }
        catch (WebException ex) when (ex.Status == WebExceptionStatus.TrustFailure)
        {
            Service.Resolve<IEventLogService>().LogException("HTMLValidator", "GetValidationResult", ex);
            mErrorText = GetString("validation.servercertificateerror");
        }
        catch (InvalidOperationException exception)
        {
            Service.Resolve<IEventLogService>().LogException("HTMLValidator", "GetValidationResult", exception);
            mErrorText = GetString("validation.invalidresponse");
        }
        catch
        {
            mErrorText = GetString("validation.servererror");
        }

        return null;
    }


    /// <summary>
    /// General method to process validation and return validation results
    /// </summary>
    private DataSet ValidateHtml()
    {
        if (!String.IsNullOrEmpty(Url))
        {
            string docHtml = GetHtml(Url);

            if (!String.IsNullOrEmpty(docHtml))
            {
                return GetValidationResult(docHtml);
            }

            mErrorText = GetString("validation.diffdomainorprotocol");
        }

        return null;
    }


    /// <summary>
    /// Converts the <see cref="HtmlValidationResult"/> object to the <see cref="DataSet"/> in order to display it in the UniGrid.
    /// </summary>
    /// <param name="result">Results object</param>
    private DataSet GetDataSetFromResult(HtmlValidationResult result)
    {
        var ds = new DataSet();
        ds.CaseSensitive = false;
        var messages = GetMessages(result);

        if (messages.Any())
        {
            var table = new DataTable();

            table.Columns.AddRange(new[]
            {
                new DataColumn("type"),
                new DataColumn("line", typeof(Int32)),
                new DataColumn("column", typeof(Int32)),
                new DataColumn("message"),
                new DataColumn("source"),
                new DataColumn("highlightStart", typeof(Int32)),
                new DataColumn("highlightLength", typeof(Int32))
            });

            foreach (var message in messages)
            {
                var row = table.NewRow();

                row.ItemArray = new object[]
                {
                    message.Type,
                    (message.FirstLine == 0) ? message.LastLine : message.FirstLine,
                    (message.FirstColumn == 0) ? message.LastColumn : message.FirstColumn,
                    message.Message,
                    message.Extract,
                    message.HiliteStart,
                    message.HiliteLength
                };

                table.Rows.Add(row);
            }

            ds.Tables.Add(table);
        }

        return ds;
    }


    /// <summary>
    /// Extracts a collection of messages from the <see cref="HtmlValidationResult"/> object given by <paramref name="result"/> parameter.
    /// </summary>
    /// <param name="result">Results object</param>
    private IEnumerable<HtmlValidationMessage> GetMessages(HtmlValidationResult result)
    {
        if ((result == null) || (result.Messages == null))
        {
            return Enumerable.Empty<HtmlValidationMessage>();
        }

        return result.Messages;
    }


    /// <summary>
    /// Process validation results
    /// </summary>
    public void UpdateControlsVisibility()
    {
        if (DataSource != null)
        {
            mErrorText = null;

            // Check if result is not empty
            if (DataSourceIsNotEmpty())
            {
                if (DataSourceContainsOnlyWarnings())
                {
                    lblResults.Visible = false;
                    gridValidationResult.Visible = !chkErrorsOnly.Checked;
                    chkErrorsOnly.Visible = true;
                    ShowConfirmation(GetString("validation.html.validbutwithwarnings"), true);
                }
                else
                {
                    // Show validation errors
                    lblResults.Text = GetString("validation.validationresults");
                    lblResults.Visible = true;
                    gridValidationResult.Visible = true;
                    chkErrorsOnly.Visible = true;
                    ShowError(GetString("validation.html.resultinvalid"));
                }
            }
            else
            {
                // Show validation is valid
                lblResults.Visible = false;
                gridValidationResult.Visible = false;
                chkErrorsOnly.Visible = false;
                ShowConfirmation(GetString("validation.html.resultvalid"));
            }
        }
        else
        {
            // No results obtained from validator, show error
            lblResults.Visible = false;
            gridValidationResult.Visible = false;
            chkErrorsOnly.Visible = false;

            if (string.IsNullOrEmpty(mErrorText))
            {
                mErrorText = GetString("validation.errorinitialization");
            }
        }
    }


    private string GetHtml(string url)
    {
        string html;
        if (UseServerRequestType)
        {
            // Create web client and try to obtaining HTML using it
            using (WebClient client = new WebClient())
            {
                try
                {
                    using (var reader = StreamReader.New(client.OpenRead(url)))
                    { 
                        html = reader.ReadToEnd();
                    }
                }
                catch
                {
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
    /// Returns a <see cref="DataSet"/> reduced just to error messages if required. Otherwise return a complete results.
    /// </summary>
    private DataSet GetDataSource()
    {
        if (chkErrorsOnly.Checked)
        {
            return GetOnlyErrorsDataSet(DataSource);
        }

        return DataSource;
    }


    private bool DataSourceIsNotEmpty()
    {
        return !DataHelper.DataSourceIsEmpty(DataSource);
    }


    /// <summary>
    /// Returns <see cref="DataSet"/> only containing errors which is generated by filtering data source given by <paramref name="ds"/>.
    /// </summary>
    /// <param name="ds">Data source to filter</param>
    private DataSet GetOnlyErrorsDataSet(DataSet ds)
    {
        if (ds == null)
        {
            return ds;
        }

        var dataView = ds.Tables[0].DefaultView;
        dataView.RowFilter = "type = 'error'";
        var table = dataView.ToTable();
        var dataSet = new DataSet();
        dataSet.Tables.Add(table);

        return dataSet;
    }


    /// <summary>
    /// Indicates whether a data source with results contains only warnings.
    /// </summary>
    private bool DataSourceContainsOnlyWarnings()
    {
        return DataSourceIsNotEmpty() && DataHelper.DataSourceIsEmpty(GetOnlyErrorsDataSet(DataSource));
    }

    #endregion
}