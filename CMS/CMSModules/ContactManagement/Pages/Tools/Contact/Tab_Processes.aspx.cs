using System;

using CMS.Automation;
using CMS.Base;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


[EditedObject("om.contact", "objectid")]
[UIElement(ModuleName.ONLINEMARKETING, "ContactProcesses", false, true)]
public partial class CMSModules_ContactManagement_Pages_Tools_Contact_Tab_Processes : CMSContactManagementPage
{
    #region "Properties"

    /// <summary>
    /// Currently edited contact.
    /// </summary>
    public ContactInfo Contact 
    { 
        get
        {
            return (ContactInfo)EditedObject;
        }
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = false;
        base.OnPreInit(e);
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (Contact == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        AuthorizationHelper.AuthorizedReadContact(true);

        // Initialize process selector
        ucSelector.UniSelector.SelectionMode = SelectionModeEnum.SingleButton;

        // Check permissions
        if (WorkflowStepInfoProvider.CanUserStartAutomationProcess(CurrentUser, CurrentSiteName))
        {
            ucSelector.UniSelector.OnItemsSelected += UniSelector_OnItemsSelected;
            ucSelector.UniSelector.SetValue("IsLiveSite", false);
            ucSelector.Enabled = true;
        }
        else
        {
            ucSelector.Enabled = false;
        }


        listElem.ObjectID = Contact.ContactID;
        listElem.ObjectType = ContactInfo.OBJECT_TYPE;
        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}";
    }


    void UniSelector_OnItemsSelected(object sender, EventArgs e)
    {
        try
        {
            int processId = ValidationHelper.GetInteger(ucSelector.Value, 0);
            AutomationManager manager = AutomationManager.GetInstance(CurrentUser);
            var infoObj = ProviderHelper.GetInfoById(listElem.ObjectType, listElem.ObjectID);
            using (CMSActionContext context = new CMSActionContext())
            {
                context.AllowAsyncActions = false;

                manager.StartProcess(infoObj, processId);
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

        listElem.UniGrid.ReloadData();
        pnlUpdate.Update();
    }

    #endregion
}
