using System;
using System.Data;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls.Internal;


public partial class CMSAdminControls_Validation_AccessibilityValidator : AccessibilityValidator
{
    private CMSDropDownList mStandardList;
    private string mErrorText;

    
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
    /// Indicates if server request  will be used rather than JavaScript request to obtain HTML
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
            return "validation|access|" + CultureCode + "|" + Url;
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


    /// <summary>
    /// Page load 
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        FormEngineUserControl label = LoadUserControl("~/CMSFormControls/Basic/LabelControl.ascx") as FormEngineUserControl;
        if (label != null)
        {
            label.Value = GetString("validation.accessibility.standard");
        }

        // Add validation standard
        FormEngineUserControl standard = LoadUserControl("~/CMSFormControls/Basic/DropDownListControl.ascx") as FormEngineUserControl;
        if (standard != null)
        {
            mStandardList = standard.FindControl(standard.InputControlID) as CMSDropDownList;
            mStandardList.Attributes.Add("class", "form-control input-width-60");
        }
        ControlsHelper.FillListControlWithEnum<AccessibilityStandardEnum>(mStandardList, "validation.accessibility.standard");

        // Set default standard value
        if (!RequestHelper.IsPostBack() && (standard != null))
        {
            standard.Value = AccessibilityStandardCode.FromEnum(AccessibilityStandardEnum.WCAG2_0A);
        }

        HeaderActions.AdditionalControls.Add(label);
        HeaderActions.AdditionalControls.Add(standard);
        HeaderActions.AdditionalControlsCssClass = "HeaderActionsLabel control-group-inline";
        HeaderActions.ReloadAdditionalControls();
    }


    /// <summary>
    /// Page load 
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        bool isPostBack = RequestHelper.IsPostBack();

        if (!isPostBack)
        {
            DataSource = null;
        }

        ReloadData(isPostBack);
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
            newWindow.Enabled = true;
            string encodedKey = ScriptHelper.GetString(HttpUtility.UrlEncode(ResultKey), false);
            newWindow.OnClientClick = String.Format("modalDialog('" + ResolveUrl("~/CMSModules/Content/CMSDesk/Validation/ValidationResults.aspx") + "?datakey={0}&docid={1}&hash={2}', 'ViewValidationResult', 800, 600);return false;", encodedKey, Node.DocumentID, QueryHelper.GetHash(String.Format("?datakey={0}&docid={1}", encodedKey, Node.DocumentID)));
        }

        HeaderActions.AddAction(validate);
        HeaderActions.AddAction(viewCode);
        HeaderActions.AddAction(newWindow);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Set sorting and add events       
        gridValidationResult.OrderBy = "line";
        gridValidationResult.IsLiveSite = IsLiveSite;
        gridValidationResult.OnExternalDataBound += gridValidationResult_OnExternalDataBound;
        gridValidationResult.OnDataReload += gridValidationResult_OnDataReload;
        gridValidationResult.ZeroRowsText = GetString("validation.access.notvalidated");
        gridValidationResult.ShowActionsMenu = true;
        gridValidationResult.AllColumns = "line, column, accessibilityrule, error, fixsuggestion, source";

        // Set custom validating text
        up.ProgressText = GetString("validation.validating");
        mStandardList.CssClass = "";
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "validate":
                Standard = AccessibilityStandardCode.ToEnum(ValidationHelper.GetInteger(((FormEngineUserControl)HeaderActions.AdditionalControls[1]).Value, 0));
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

        ReloadData(true);
    }


    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData(bool force)
    {
        SetupControls();

        gridValidationResult.ReloadData();

        if (force)
        {
            ProcessResult(DataSource);
        }

    }

    /// <summary>
    /// Data reload event
    /// </summary> 
    protected DataSet gridValidationResult_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            totalRecords = DataSource.Tables[0].Rows.Count;
        }
        return DataSource;
    }


    /// <summary>
    /// On external data bound event
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Action what is called</param>
    /// <param name="parameter">Parameter</param>
    /// <returns>Result object</returns>
    protected object gridValidationResult_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        return GridExternalDataBound(sender, sourceName, parameter);
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
            // Get HTML stored using JavaScript
            html = ValidationHelper.Base64Decode(hdnHTML.Value);
        }

        return RemoveVirtualContextDataFromUrls(html, CultureCode);
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
                DataSet dsValidationResult = GetValidationResult(docHtml, ref mErrorText);

                // Check if result contains error table
                if (!DataHelper.DataSourceIsEmpty(dsValidationResult) && !DataHelper.DataSourceIsEmpty(dsValidationResult.Tables["errors"]))
                {
                    DataTable tbError = DocumentValidationHelper.ProcessValidationResult(dsValidationResult, DocumentValidationEnum.Accessibility, null);
                    tbError.DefaultView.Sort = "line ASC";
                    DataSet result = new DataSet();
                    result.Tables.Add(tbError);
                    return result;
                }

                return dsValidationResult;
            }

            mErrorText = GetString("validation.diffdomainorprotocol");
        }

        return null;
    }


    /// <summary>
    /// Process validation results
    /// </summary>
    /// <param name="validationResult">DataSet with result of validation</param>
    public void ProcessResult(DataSet validationResult)
    {
        if (validationResult != null)
        {
            mErrorText = null;

            // Check if result is not empty
            if (!DataHelper.DataSourceIsEmpty(validationResult))
            {
                // Show validation errors
                ShowError(GetString("validation.access.resultinvalid"));
                lblResults.Visible = true;
                lblResults.Text = ResHelper.GetString("validation.validationresults");
                gridValidationResult.Visible = true;
            }
            else
            {
                // Show validation is valid
                ShowConfirmation(GetString("validation.access.resultvalid"));
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
}