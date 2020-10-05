using System;
using System.Collections;
using System.Linq;
using System.Text;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessContacts")]
[EditedObject(WorkflowInfo.OBJECT_TYPE, "processid")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts : CMSAutomationPage
{
    private readonly string DEMOGRAPHICS_IDENTIFIER = "automationProcess";


    private int mNumberOfUniqueContacts;
    private WorkflowInfo mProcess;
    private string mIdentifier;


    private bool RightPanelNeedsRefresh { get; set; } = true;


    private int NumberOfAllContacts => listContacts.UniGrid.PagerForceNumberOfResults;


    private int NumberOfUniqueContacts
    {
        get
        {
            if (NumberOfAllContacts > 0 && mNumberOfUniqueContacts <= 0)
            {
                mNumberOfUniqueContacts = GetNumberOfUniqueContacts();
            }

            return mNumberOfUniqueContacts;
        }
    }


    private WorkflowInfo Process
    {
        get
        {
            if (mProcess == null)
            {
                mProcess = EditedObject as WorkflowInfo;

                if (mProcess == null)
                {
                    RedirectToInformation("editedobject.notexists");
                }
            }

            return mProcess;
        }
    }


    private string Identifier
    {
        get
        {
            if (String.IsNullOrEmpty(mIdentifier))
            {
                mIdentifier = ValidationHelper.GetString(ViewState["identifier"], null);
                if (String.IsNullOrEmpty(mIdentifier))
                {
                    mIdentifier = Guid.NewGuid().ToString();
                    ViewState["identifier"] = mIdentifier;
                }
            }

            return mIdentifier;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        listContacts.ProcessID = UIContext.ObjectID;

        // Prevent right panel refresh on some unigrid actions
        listContacts.UniGrid.OnBeforeSorting += PreventRightPanelRefresh;
        listContacts.UniGrid.OnPageChanged += PreventRightPanelRefresh;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!AuthorizedForContacts)
        {
            // User has no permissions
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Read");
        }

        InitContactSelector();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack() || RightPanelNeedsRefresh)
        {
            InitRightPanel();
        }
    }


    private void PreventRightPanelRefresh(object sender, EventArgs e)
    {
        RightPanelNeedsRefresh = false;
    }


    private void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        var contacts = ValidationHelper.GetString(ucSelector.Value, null);
        if (String.IsNullOrEmpty(contacts) || !WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, CurrentSiteName))
        {
            return;
        }

        var contactIds = contacts.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(i => ValidationHelper.GetInteger(i, 0));
        var manager = AutomationManager.GetInstance(CurrentUser);
        var processId = UIContext.ObjectID;
        var warningBuilder = new StringBuilder();

        using (CMSActionContext context = new CMSActionContext { AllowAsyncActions = false })
        {
            try
            {
                foreach (var contactId in contactIds)
                {
                    var contact = ContactInfo.Provider.Get(contactId);
                    try
                    {
                        manager.StartProcess(contact, processId);
                    }
                    catch (ProcessRecurrenceException ex)
                    {
                        warningBuilder.AppendFormat("<div>{0}</div>", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LogAndShowError("Automation", "STARTPROCESS", ex);
                return;
            }
        }

        var warning = warningBuilder.ToString();
        if (!String.IsNullOrEmpty(warning))
        {
            ShowWarning(String.Format(GetString("sf.automation.error"), ""), warning);
        }
        else
        {
            ShowConfirmation(GetString("ma.process.startedselected"));
        }

        // Reload grid
        listContacts.ReloadData();
        pnlUpdate.Update();

        // Reset selector
        ucSelector.Value = null;
    }


    /// <summary>
    /// Initializes contact selector.
    /// </summary>
    private void InitContactSelector()
    {
        // Initialize contact selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.MultipleButton;
        ucSelector.UniSelector.DialogButton.ResourceString = "om.contact.general.additems";

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, CurrentSiteName) && Process.WorkflowEnabled)
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.Enabled = true;
            ucSelector.UniSelector.DialogButton.ToolTipResourceString = "ma.automationprocess.addcontacts.tooltip";
            ucSelector.UniSelector.DialogInformationMessage = GetString("ma.automationprocess.startprocessinfo");
        }
        else
        {
            ucSelector.Enabled = false;
            ucSelector.UniSelector.DialogButton.ToolTipResourceString = Process.WorkflowEnabled ? "general.nopermission" : "autoMenu.DisabledStateDesc";
        }
    }


    private void InitRightPanel()
    {
        InitDemographicsButton();
        InitChart();
    }


    private void InitDemographicsButton()
    {
        btnDemographics.Text = GetString("om.contact.demographics");

        if (NumberOfAllContacts == 0)
        {
            btnDemographics.Enabled = false;
            btnDemographics.ToolTip = GetString("ma.contact.demographics.tooltip.disabled");
        }
        else
        {
            btnDemographics.OnClientClick = $"modalDialog('{GetDemographicsUrl()}', 'ContactDemographics', '95%', '95%'); return false;";
        }
    }


    private void InitChart()
    {
        lblChartHeading.Text = String.Format(GetString("ma.automationprocess.analytics.chart.heading"), NumberOfAllContacts);

        if (Process.WorkflowRecurrenceType != ProcessRecurrenceTypeEnum.NonRecurring && NumberOfAllContacts > 0)
        {
            iconHelp.Visible = true;

            ScriptHelper.RegisterTooltip(this);
            ScriptHelper.AppendTooltip(iconHelp, String.Format(GetString("ma.automationprocess.analytics.chart.heading.info"), NumberOfUniqueContacts), null);
        }

        ScriptHelper.RegisterModule(Page, "CMS.OnlineMarketing/Process/ProcessContactsChart", new
        {
            chartElement = pnlChart.ClientID,
            data = GetChartData()
        });

        pnlUpdateChart.Update();
    }


    private IEnumerable GetChartData()
    {
        return AutomationStateInfo.Provider.Get()
            .Columns(
                new QueryColumn("StepDisplayName").As("Title"),
                new AggregatedColumn(AggregationType.Count, "StateStepID").As("Value")
            )
            .Source(s => s
                .Join("CMS_WorkflowStep", "StateStepID", "StepID")
            )
            .Where(listContacts.UniGrid.CompleteWhereCondition)
            .GroupBy("StateStepID", "StepDisplayName")
            .OrderBy("Value DESC")
            .Select(dr => new { Title = dr["Title"], Value = dr["Value"] })
            .ToList();
    }


    private int GetNumberOfUniqueContacts()
    {
        return AutomationStateInfo.Provider.Get()
            .Column(
                new AggregatedColumn(AggregationType.Count, "DISTINCT StateObjectID")
            )
            .Where(listContacts.UniGrid.CompleteWhereCondition)
            .GetScalarResult<int>();
    }


    private string GetDemographicsUrl()
    {
        WindowHelper.Add(Identifier, listContacts.UniGrid.CompleteWhereCondition);

        var additionalQuery = QueryHelper.BuildQuery(
            "retrieverIdentifier", DEMOGRAPHICS_IDENTIFIER,
            "persistent", "true",
            "overwriteBreadcrumbs", "false",
            "params", Identifier);

        return ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "ContactDemographics", 0, additionalQuery);
    }
}
