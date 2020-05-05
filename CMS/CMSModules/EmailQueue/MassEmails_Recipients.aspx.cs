using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[Title(ResourceString = "emailqueue.sentdetails.title")]
public partial class CMSModules_EmailQueue_MassEmails_Recipients : CMSModalGlobalAdminPage
{
    #region "Protected variables"

    private int emailId;
    private bool isArchive;
    private string mCreatedText;
    private string mWaitingText;
    private string mSendingText;
    private string mArchivedText;

    #endregion


    #region "Properties"

    private string CreatedText
    {
        get
        {
            return mCreatedText ?? (mCreatedText = GetString("emailstatus.created"));
        }
    }


    private string WaitingText
    {
        get
        {
            return mWaitingText ?? (mWaitingText = GetString("emailstatus.waiting"));
        }
    }


    private string SendingText
    {
        get
        {
            return mSendingText ?? (mSendingText = GetString("emailstatus.sending"));
        }
    }


    private string ArchivedText
    {
        get
        {
            return mArchivedText ?? (mArchivedText = GetString("general.archived"));
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        emailId = QueryHelper.GetInteger("emailid", 0);
        isArchive = QueryHelper.GetBoolean("archive", false);

        gridElem.WhereCondition = "EmailID=" + emailId;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeDataReload += gridElem_OnBeforeDataReload;
        gridElem.OnAction += gridElem_OnAction;

        // Header action initialization
        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("emailqueue.queue.deleteselected"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("EmailQueue.DeleteSelectedRecipientConfirmation")) + ")) return false;",
            CommandName = "delete"
        });
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Remove selected recipients from mass e-mail.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName.EqualsCSafe("delete", true))
        {
            // Get list of selected users
            var list = gridElem.SelectedItems;
            if (list.Count > 0)
            {
                foreach (string userId in list)
                {
                    // Remove specific recipient
                    EmailUserInfoProvider.DeleteEmailUserInfo(emailId, ValidationHelper.GetInteger(userId, 0));
                }
                gridElem.ResetSelection();
                gridElem.Pager.UniPager.CurrentPage = 1;
                gridElem.ReloadData();
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Enable header action and show grid if the grid is not empty
        HeaderAction action = HeaderActions.ActionsList[0];
        if (action != null)
        {
            action.Enabled = gridElem.GridView.Rows.Count > 0;
        }
    }

    #endregion


    #region "Grid events"

    protected void gridElem_OnBeforeDataReload()
    {
        // Hide status and last result columns in archive
        gridElem.NamedColumns["result"].Visible = gridElem.NamedColumns["status"].Visible = !isArchive;
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "userid":
                // Get user friendly name instead of id
                UserInfo ui = UserInfoProvider.GetUserInfo(ValidationHelper.GetInteger(parameter, 0));
                if (ui != null)
                {
                    return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(ui.UserName) + " (" + ui.Email + ")");
                }
                
                return GetString("general.na");
            case "status":
                return GetEmailStatus(parameter);
        }

        return null;
    }


    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                int userId = ValidationHelper.GetInteger(actionArgument, 0);
                if (userId > 0)
                {
                    EmailUserInfoProvider.DeleteEmailUserInfo(emailId, userId);
                }
                break;
        }
    }


    /// <summary>
    /// Gets the e-mail status.
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>E-mail status text</returns>
    private string GetEmailStatus(object parameter)
    {
        switch ((EmailStatusEnum)parameter)
        {
            case EmailStatusEnum.Created:
                return CreatedText;

            case EmailStatusEnum.Waiting:
                return WaitingText;

            case EmailStatusEnum.Sending:
                return SendingText;

            case EmailStatusEnum.Archived:
                return ArchivedText;

            default:
                return string.Empty;
        }
    }

    #endregion
}