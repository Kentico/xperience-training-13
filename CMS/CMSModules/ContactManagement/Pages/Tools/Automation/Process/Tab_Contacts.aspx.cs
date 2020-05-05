using System;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;


[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditProcess;EditProcessContacts")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_Tab_Contacts : CMSAutomationPage
{
    #region "Variables"

    private int mProcessID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current workflow ID
    /// </summary>
    public int ProcessID
    {
        get
        {
            if (mProcessID <= 0)
            {
                mProcessID = QueryHelper.GetInteger("processid", 0);
            }
            return mProcessID;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set process identifier
        listContacts.ProcessID = ProcessID;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!AuthorizedForContacts)
        {
            // User has no permissions
            RedirectToAccessDenied(ModuleName.CONTACTMANAGEMENT, "Read");
        }

        // Use local Header actions
        HeaderActions = headerActions;

        // Add Refresh action button
        AddHeaderAction(new HeaderAction()
        {
            Text = GetString("general.Refresh"),
            RedirectUrl = "Tab_Contacts.aspx?processid=" + listContacts.ProcessID
        });

        ucSelector.UniSelector.DialogButton.ResourceString = "ma.automationprocess.select";
        
        InitContactSelector();
    }


    void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        try
        {
            int contactId = ValidationHelper.GetInteger(ucSelector.Value, 0);
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
            var infoObj = ContactInfo.Provider.Get(contactId);
            if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, CurrentSiteName))
            {
                using (CMSActionContext context = new CMSActionContext())
                {
                    context.AllowAsyncActions = false;

                    manager.StartProcess(infoObj, ProcessID);
                }
            }
        }
        catch (ProcessRecurrenceException ex)
        {
            ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            LogAndShowError("Automation", "STARTPROCESS", ex);
        }

        listContacts.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes contact selector.
    /// </summary>
    private void InitContactSelector()
    {
        // Initialize contact selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleButton;

        WorkflowInfo process = WorkflowInfoProvider.GetWorkflowInfo(ProcessID);

        if (process == null)
        {
            RedirectToInformation("editedobject.notexists");            
        }

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, CurrentSiteName) && ((process != null) && process.WorkflowEnabled))
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.IsLiveSite = false;
            ucSelector.Enabled = true;
            ucSelector.UniSelector.DialogButton.ToolTipResourceString = "automenu.startstatedesc";
        }
        else
        {
            ucSelector.Enabled = false;
            ucSelector.UniSelector.DialogButton.ToolTipResourceString = process.WorkflowEnabled ? "general.nopermission" : "autoMenu.DisabledStateDesc";
        }
    }

    #endregion
}