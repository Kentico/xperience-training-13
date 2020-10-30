using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.WorkflowEngine;


public partial class CMSModules_ContactManagement_Pages_Tools_Automation_List : CMSAutomationPage
{
    private const string HOWTO_VIDEO_URL = "https://youtu.be/IIDizgW8xI4";
    private const string HOWTO_VIDEO_LENGTH = "3:55";

    private bool? mCanManageProcesses;


    private bool CanManageProcesses
    {
        get
        {
            if (!mCanManageProcesses.HasValue)
            {
                mCanManageProcesses = WorkflowStepInfoProvider.CanUserManageAutomationProcesses(CurrentUser, CurrentSiteName);
            }
            return mCanManageProcesses.Value;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set master page elements
        InitializeMasterPage();

        // Initialize the smart tip
        InitSmartTip();

        // Check manage permission for object menu
        gridProcesses.ShowObjectMenu = CanManageProcesses;

        // Control initialization
        gridProcesses.RememberStateByParam = "";
    }


    /// <summary>
    ///  Initializes master page elements.
    /// </summary>
    private void InitializeMasterPage()
    {
        if (WorkflowInfoProvider.IsMarketingAutomationAllowed())
        {
            var newProcess = new HeaderAction
            {
                // New process link
                Text = GetString("ma.newprocess"),
                RedirectUrl = "Process/New.aspx",
                Enabled = CanManageProcesses
            };

            AddHeaderAction(newProcess);
        }
    }


    private void InitSmartTip()
    {
        tipHowMAWorks.ExpandedHeader = GetString("ma.process.listing.howto.title");
        tipHowMAWorks.Content = $@"
<div class=""smarttip-video"">
    <a href=""{HOWTO_VIDEO_URL}"" target=""_blank"">
        <img src=""{UIHelper.GetImageUrl("CMSModules/CMS_Automation/ma_howto_video_thumbnail.png")}"" class=""smarttip-video-thumbnail"">
        <span class=""smarttip-video-title"">{GetString("ma.process.listing.howto.content")}</span>
        <span class=""smarttip-video-length"">{HOWTO_VIDEO_LENGTH}</span>
    </a>
</div>";
    }


    protected object gridProcesses_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "recurrencetype":
                var val = (ProcessRecurrenceTypeEnum)ValidationHelper.GetInteger(parameter, (int)ProcessRecurrenceTypeEnum.Recurring);
                return val.ToLocalizedString(null);

            case "delete":
                if (!CanManageProcesses)
                {
                    var btn = (CMSGridActionButton)sender;
                    btn.Enabled = false;
                }
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridProcesses_OnAction(string actionName, object actionArgument)
    {
        int processId = Convert.ToInt32(actionArgument);

        switch (actionName)
        {
            case "edit":
                var url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditProcess");
                url = URLHelper.AddParameterToUrl(url, "objectId", processId.ToString());
                url = URLHelper.AddParameterToUrl(url, "tabname", "EditProcessSteps");
                URLHelper.Redirect(url);
                break;

            case "delete":
                if (!CanManageProcesses)
                {
                    RedirectToAccessDenied(ModuleName.ONLINEMARKETING, "ManageProcesses");
                }

                // Delete the workflow with all the dependencies
                WorkflowInfo.Provider.Get(processId)?.Delete();

                ShowConfirmation(GetString("ma.process.delete.confirmation"));
                break;
        }
    }
}
