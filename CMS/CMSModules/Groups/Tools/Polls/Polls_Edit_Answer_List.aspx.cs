using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_Answer_List : CMSGroupPollsPage
{
    protected int pollId = 0;
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get poll id from querystring		
        pollId = QueryHelper.GetInteger("pollId", 0);
        groupId = QueryHelper.GetInteger("groupId", 0);

        if (CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE, false))
        {
            HeaderAction newItem = new HeaderAction();
            newItem.Text = GetString("Polls_Answer_List.NewItemCaption");
            newItem.RedirectUrl = ResolveUrl("Polls_Edit_Answer_Edit.aspx?pollId=" + pollId.ToString() + "&groupId=" + groupId);
            CurrentMaster.HeaderActions.AddAction(newItem);

            HeaderAction reset = new HeaderAction();
            reset.Text = GetString("Polls_Answer_List.ResetButton");
            reset.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Polls_Answer_List.ResetConfirmation")) + ");";
            reset.CommandName = "btnReset_Click";
            CurrentMaster.HeaderActions.AddAction(reset);

            CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

            AnswerList.AllowEdit = true;
        }

        AnswerList.OnEdit += new EventHandler(AnswerList_OnEdit);
        AnswerList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(AnswerList_OnCheckPermissions);
        AnswerList.PollId = pollId;
        AnswerList.GroupId = groupId;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            AnswerList.ReloadData();
        }
    }


    private void AnswerList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        int groupId = 0;

        PollInfo pi = PollInfoProvider.GetPollInfo(AnswerList.PollId);
        if (pi != null)
        {
            groupId = pi.PollGroupID;
        }

        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// AnswerList edit action handler.
    /// </summary>
    private void AnswerList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("Polls_Edit_Answer_Edit.aspx?answerId=" + AnswerList.SelectedItemID.ToString() + "&groupId=" + groupId));
    }


    /// <summary>
    /// Header action handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "btnreset_click": // Reset all answer counts
                // Check 'Manage' permission
                PollInfo pi = PollInfoProvider.GetPollInfo(AnswerList.PollId);
                int groupId = 0;

                if (pi != null)
                {
                    groupId = pi.PollGroupID;
                }

                // Check permissions
                CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);

                if (pollId > 0)
                {
                    PollAnswerInfoProvider.ResetAnswers(pollId);
                    AnswerList.ReloadData();
                }
                break;
        }
    }
}