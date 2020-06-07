using System;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;

public partial class CMSModules_ContactManagement_Controls_UI_Automation_Contacts : CMSAdminControl
{
    /// <summary>
    /// Gets or sets current identifier.
    /// </summary>
    public int ProcessID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the current unigrid.
    /// </summary>
    public UniGrid UniGrid => listElem;


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing)
        {
            SetupControl();
        }
        else
        {
            listElem.StopProcessing = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!SqlInstallationHelper.DatabaseIsSeparated() && !StopProcessing)
        {
            var filterLabel = listElem.FilterForm.FieldLabels["ContactFirstName"];
            if (filterLabel != null)
            {
                filterLabel.ResourceString = "filter.searchbyname";
            }
        }
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    private void SetupControl()
    {
        if (ProcessID > 0)
        {
            listElem.WhereCondition = "(StateWorkflowID = " + ProcessID + ") ";
        }

        // Register scripts for contact details dialog
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewContactDetails", ScriptHelper.GetScript(
            "function Refresh() {" +
            "__doPostBack('" + ClientID + @"', '');" +
            "}"));

        // Hide filtered fields that are on separated database, since the query that the filter
        // returns couldn't be executed anyways
        if (SqlInstallationHelper.DatabaseIsSeparated())
        {
            listElem.FilterForm.FieldsToHide.Add("ContactFirstName");
        }
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerInvariant())
        {
            // Delete action
            case "delete":
                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(String.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerCSafe()))) + ")) { return false; }" + btn.OnClientClick;
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteContext.CurrentSiteName))
                {
                    if (btn != null)
                    {
                        btn.Enabled = false;
                    }
                }
                break;

            // Process status column
            case "statestatus":
                return AutomationHelper.GetProcessStatus((ProcessStatusEnum)ValidationHelper.GetInteger(parameter, 0));
        }

        return null;
    }


    /// <summary>
    /// Reloads data in listing.
    /// </summary>
    /// <param name="forceReload">Whether to force complete reload</param>
    public override void ReloadData(bool forceReload)
    {
        listElem.ReloadData();
    }
}
