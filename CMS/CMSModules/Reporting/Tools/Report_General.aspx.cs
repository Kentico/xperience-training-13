using System;
using System.Threading;
using System.Web.Services;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Reporting;
using CMS.Reporting.Web.UI;
using CMS.UIControls;


[UIElement("CMS.Reporting", "General")]
public partial class CMSModules_Reporting_Tools_Report_General : CMSReportingPage
{
    #region "Variables"

    protected int reportId;

    private ReportInfo ri;

    #endregion


    #region "Properties"

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


    /// <summary>
    /// Returns the item name from id
    /// </summary>
    /// <param name="type">Item type</param>
    /// <param name="id">Item ID</param>
    [WebMethod]
    public static string GetReportItemName(string type, int id)
    {
        // Switch by type
        switch (ReportInfoProvider.StringToReportItemType(type))
        {
            // Graph
            case ReportItemType.Graph:
            case ReportItemType.HtmlGraph:
                ReportGraphInfo rgi = ReportGraphInfoProvider.GetReportGraphInfo(id);
                if (rgi != null)
                {
                    return rgi.GraphName;
                }
                break;

            // Table
            case ReportItemType.Table:
                ReportTableInfo rti = ReportTableInfoProvider.GetReportTableInfo(id);
                if (rti != null)
                {
                    return rti.TableName;
                }
                break;

            // Value
            case ReportItemType.Value:
                ReportValueInfo rvi = ReportValueInfoProvider.GetReportValueInfo(id);
                if (rvi != null)
                {
                    return rvi.ValueName;
                }
                break;
        }

        return String.Empty;
    }

    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucSelectString.Scope = ReportInfo.OBJECT_TYPE;
        pnlConnectionString.Visible = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "SetConnectionString");
        Title = "Report General";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReloadPage", ScriptHelper.GetScript("function ReloadPage() { \n" + Page.ClientScript.GetPostBackEventReference(btnHdnReload, null) + "}"));

        reportId = QueryHelper.GetInteger("reportId", 0);

        // control initializations				
        rfvReportDisplayName.ErrorMessage = GetString("Report_New.EmptyDisplayName");
        rfvReportName.ErrorMessage = GetString("Report_New.EmptyCodeName");

        lblReportDisplayName.Text = GetString("Report_New.DisplayNameLabel");
        lblReportName.Text = GetString("Report_New.NameLabel");
        lblReportCategory.Text = GetString("Report_General.CategoryLabel");
        lblLayout.Text = GetString("Report_General.LayoutLabel");
        lblGraphs.Text = GetString("Report_General.GraphsLabel") + ":";
        lblHtmlGraphs.Text = GetString("Report_General.HtmlGraphsLabel") + ":";
        lblTables.Text = GetString("Report_General.TablesLabel") + ":";
        lblValues.Text = GetString("Report_General.TablesValues") + ":";

        actionsElem.ActionsList.Add(new SaveAction());
        actionsElem.ActionPerformed += actionsElem_ActionPerformed;

        AttachmentTitle.TitleText = GetString("general.attachments");

        attachmentList.AllowPasteAttachments = true;
        attachmentList.ObjectID = reportId;
        attachmentList.ObjectType = ReportInfo.OBJECT_TYPE;
        attachmentList.Category = ObjectAttachmentsCategories.LAYOUT;

        // Get report info
        ri = ReportInfoProvider.GetReportInfo(reportId);

        if (ri == null)
        {
            URLHelper.SeeOther(AdministrationUrlHelper.GetInformationUrl("editedobject.notexists"));
        }

        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }

        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlTemplateBody.EditorAreaCSS = "";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReportingHTML", ScriptHelper.GetScript(" var reporting_htmlTemplateBody = '" + htmlTemplateBody.ClientID + "'"));

        // initialize item list controls
        ilGraphs.Report = ri;
        ilTables.Report = ri;
        ilValues.Report = ri;
        ilHtmlGraphs.Report = ri;

        ilGraphs.EditUrl = "ReportGraph_Edit.aspx";
        ilTables.EditUrl = "ReportTable_Edit.aspx";
        ilValues.EditUrl = "ReportValue_Edit.aspx";
        ilHtmlGraphs.EditUrl = "ReportHtmlGraph_Edit.aspx";

        ilGraphs.ItemType = ReportItemType.Graph;
        ilTables.ItemType = ReportItemType.Table;
        ilValues.ItemType = ReportItemType.Value;
        ilHtmlGraphs.ItemType = ReportItemType.HtmlGraph;

        // Refresh script
        string script = "function RefreshWOpener(w) { if (w.refreshPageOnClose){ " + ControlsHelper.GetPostBackEventReference(this, "arg") + " }}";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ReportingRefresh", ScriptHelper.GetScript(script));

    }


    /// <summary>
    /// Load data.
    /// </summary>
    protected void LoadData()
    {
        if (ri == null)
        {
            return;
        }
        txtReportDisplayName.Text = ri.ReportDisplayName;
        txtReportName.Text = ri.ReportName;
        htmlTemplateBody.ResolvedValue = ri.ReportLayout;
        chkEnableSubscription.Checked = ri.ReportEnableSubscription;
        ucSelectString.Value = ri.ReportConnectionString;
        selectCategory.Value = ri.ReportCategoryID;
    }


    /// <summary>
    /// Handles actions from HeaderAction control
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                SaveReport();
                break;
        }
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void SaveReport()
    {
        // Check 'Modify' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.reporting", "Modify"))
        {
            RedirectToAccessDenied("cms.reporting", "Modify");
        }
        string errorMessage = new Validator().NotEmpty(txtReportDisplayName.Text.Trim(), rfvReportDisplayName.ErrorMessage).NotEmpty(txtReportName.Text.Trim(), rfvReportName.ErrorMessage).Result;

        if (String.IsNullOrEmpty(errorMessage) && (!ValidationHelper.IsCodeName(txtReportName.Text.Trim())))
        {
            errorMessage = GetString("general.invalidcodename");
        }        

        if (String.IsNullOrEmpty(errorMessage))
        {
            ReportInfo reportInfo = ReportInfoProvider.GetReportInfo(reportId);
            ReportInfo nri = ReportInfoProvider.GetReportInfo(txtReportName.Text.Trim());

            // If report with given name already exists show error message
            if ((nri != null) && (nri.ReportID != reportInfo.ReportID))
            {
                ShowError(GetString("Report_New.ReportAlreadyExists"));
                return;
            }

            if (reportInfo != null)
            {
                reportInfo.ReportLayout = htmlTemplateBody.ResolvedValue;

                // If there was a change in report code name change codenames in layout
                if (reportInfo.ReportName != txtReportName.Text.Trim())
                {
                    // part of old macro
                    string oldValue = "?" + reportInfo.ReportName + ".";
                    string newValue = "?" + txtReportName.Text.Trim() + ".";

                    reportInfo.ReportLayout = reportInfo.ReportLayout.Replace(oldValue, newValue);

                    // Set updated text back to HTML editor
                    htmlTemplateBody.ResolvedValue = reportInfo.ReportLayout;
                }
                int categoryID = ValidationHelper.GetInteger(selectCategory.Value, reportInfo.ReportCategoryID);

                // If there was a change in display name refresh category tree 
                if ((reportInfo.ReportDisplayName != txtReportDisplayName.Text.Trim()) || (reportInfo.ReportCategoryID != categoryID))
                {
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshParent", ScriptHelper.GetScript("if (parent.refreshPage) {parent.refreshPage()} else parent.location = parent.location;"));
                }

                reportInfo.ReportDisplayName = txtReportDisplayName.Text.Trim();
                reportInfo.ReportName = txtReportName.Text.Trim();
                reportInfo.ReportAccess = ReportAccessEnum.All;
                reportInfo.ReportCategoryID = categoryID;
                reportInfo.ReportEnableSubscription = chkEnableSubscription.Checked;
                reportInfo.ReportConnectionString = ValidationHelper.GetString(ucSelectString.Value, String.Empty);

                ReportInfoProvider.SetReportInfo(reportInfo);

                ShowChangesSaved();

                // Reload header if changes were saved
                ScriptHelper.RefreshTabHeader(Page, reportInfo.ReportDisplayName);
            }
        }
        else
        {
            ShowError(errorMessage);
        }
    }

    #endregion
}
