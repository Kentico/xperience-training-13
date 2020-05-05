using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.UIControls;

[UIElement(ModuleName.NEWSLETTER, "Newsletter.ABTestIssue.Send")]
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_ABTestIssue_Send : CMSNewsletterSendPage
{
    private bool CanIssueBeSent => (Issue.IssueStatus == IssueStatusEnum.Idle) && !Issue.IssueForAutomation;


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        MessagesPlaceHolder = plcMessages;

        ctrSmartTip.Visible = IssueEditingEnabled && !Issue.IssueForAutomation;

        InitSendVariantControl();

        InitHeaderActions();

        AddBrokenEmailUrlNotifier(Newsletter, lblUrlWarning);

        if (!Issue.IssueForAutomation)
        {
            DisplayMessage(ctrSendVariant.InfoMessage);
        }
    }


    private void InitSendVariantControl()
    {
        ctrSendVariant.StopProcessing = Issue.IssueID <= 0;
        ctrSendVariant.IssueID = Issue.IssueID;
        ctrSendVariant.Enabled = IssueEditingEnabled;

        if (!Issue.IssueForAutomation)
        {
            // Update information about email variants sending state
            ctrSendVariant.OnChanged += (sender, e) => DisplayMessage(ctrSendVariant.InfoMessage);
        }

        ctrSendVariant.ReloadData(!RequestHelper.IsPostBack());
    }


    private void InitHeaderActions()
    {
        var hdrActions = CurrentMaster.HeaderActions;
        hdrActions.ActionsList.Clear();

        if (IssueEditingEnabled && !IssueHelper.IsWinnerSelected(Issue))
        {
            hdrActions.ActionsList.Add(new SaveAction());
        }

        if (IssueEditingEnabled && CanIssueBeSent)
        {
            var variants = GetIssueVariants(Issue);

            var variantNamesWithUnfilledRequiredWidgetProperties = GetVariantNamesWithUnfilledRequiredWidgetProperties(variants);
            var variantNamesWithMissingWidgetDefinition = GetVariantNamesWithMissingWidgetDefinition(variants);

            var isValidDefinition = !variantNamesWithUnfilledRequiredWidgetProperties.Any()
                && !variantNamesWithMissingWidgetDefinition.Any();

            AddSendHeaderAction(hdrActions, isValidDefinition, ButtonStyle.Default);

            if (!isValidDefinition)
            {
                var invalidVariantNames = variantNamesWithUnfilledRequiredWidgetProperties.Union(variantNamesWithMissingWidgetDefinition);

                MessagesPlaceHolder.AddError(string.Format(GetString("newsletter.issue.send.variantwidgeterror"), string.Join(", ", invalidVariantNames)));
            }
        }

        if (IssueEditingEnabled)
        {
            hdrActions.ActionPerformed += hdrActions_OnActionPerformed;
        }
        
        hdrActions.ReloadData();

        CurrentMaster.DisplayActionsPanel = true;
    }


    protected void hdrActions_OnActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                SaveIssueSettings();
                break;
            case ComponentEvents.SUBMIT:
                SendIssue();
                break;
        }
    }


    /// <summary>
    /// Sends displayed issue.
    /// </summary>
    protected void SendIssue()
    {
        HandleMissingPermissionsForIssueEditing();

        var errorMessage = !ctrSendVariant.SendIssue() ? ctrSendVariant.ErrorMessage : String.Empty;

        HandleSendActionResult(errorMessage);
    }


    /// <summary>
    /// Saves displayed issue settings.
    /// </summary>
    protected void SaveIssueSettings()
    {
        if (ctrSendVariant.SaveIssue())
        {
            if (!String.IsNullOrEmpty(ctrSendVariant.InfoMessage) && (!Issue.IssueIsABTest || !Issue.IssueForAutomation))
            {
                DisplayMessage(ctrSendVariant.InfoMessage);
            }

            ShowChangesSaved();
        }
        else
        {
            ShowError(ctrSendVariant.ErrorMessage);
        }
    }


    private static IList<IssueInfo> GetIssueVariants(IssueInfo issue)
    {
        var originalIssue = IssueInfoProvider.GetOriginalIssue(issue.IssueID);

        return IssueInfo.Provider.Get()
                                .WhereEquals("IssueVariantOfIssueID", originalIssue.IssueID)
                                .ToList();
    }


    private IList<string> GetVariantNamesWithUnfilledRequiredWidgetProperties(IEnumerable<IssueInfo> variants)
    {
        return variants.Where(variant => variant.HasWidgetWithUnfilledRequiredProperty())
            .Select(v => v.GetVariantName())
            .ToList();
    }


    private IList<string> GetVariantNamesWithMissingWidgetDefinition(IEnumerable<IssueInfo> variants)
    {
        return variants.Where(variant => variant.HasWidgetWithMissingDefinition())
            .Select(v => v.GetVariantName())
            .ToList();
    }
}