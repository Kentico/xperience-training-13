using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Community.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Groups_Tools_Polls_Polls_Edit_Answer_Edit : CMSGroupPollsPage
{
    protected int pollId = 0;
    protected int answerId = 0;
    protected int groupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get AnswerID and PollID from querystring
        pollId = QueryHelper.GetInteger("pollId", 0);
        answerId = QueryHelper.GetInteger("answerId", 0);
        groupId = QueryHelper.GetInteger("groupId", 0);

        string currentPollAnswer = GetString("Polls_Answer_Edit.NewItemCaption");

        // Initialize AnswerEdit control
        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            AnswerEdit.Saved = true;
        }
        AnswerEdit.ItemID = answerId;
        AnswerEdit.PollId = pollId;
        AnswerEdit.OnSaved += AnswerEdit_OnSaved;
        AnswerEdit.OnCheckPermissions += AnswerEdit_OnCheckPermissions;

        if (answerId > 0)
        {
            PollAnswerInfo pollAnswerObj = PollAnswerInfoProvider.GetPollAnswerInfo(answerId);
            EditedObject = pollAnswerObj;
            if (pollAnswerObj != null)
            {
                // Check that poll belongs to the specified group
                if ((pollAnswerObj.AnswerPollID > 0) && (groupId > 0))
                {
                    PollInfo poll = PollInfoProvider.GetPollInfo(pollAnswerObj.AnswerPollID);

                    // Answer not found or doesn't belong to specified group
                    if ((poll == null) || (poll.PollGroupID != groupId))
                    {
                        RedirectToAccessDenied(GetString("community.group.pollnotassigned"));
                    }
                }

                // Set control
                currentPollAnswer = GetString("Polls_Answer_Edit.AnswerLabel") + " " + pollAnswerObj.AnswerOrder.ToString();
                pollId = pollAnswerObj.AnswerPollID;
            }
        }

        // Validate
        EditedObject = PollInfoProvider.GetPollInfo(pollId);

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Polls_Answer_Edit.ItemListLink"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Polls/Polls_Edit_Answer_List.aspx?pollId=" + pollId + "&groupId=" + groupId),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = currentPollAnswer,
        });

        // New item link
        HeaderAction add = new HeaderAction
        {
            Text = GetString("Polls_Answer_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Polls_Edit_Answer_Edit.aspx?pollId=" + pollId.ToString() + "&groupId=" + groupId)
        };
        CurrentMaster.HeaderActions.AddAction(add);
    }


    private void AnswerEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check 'Manage' permission
        PollInfo pi = PollInfoProvider.GetPollInfo(AnswerEdit.PollId);
        int groupId = 0;

        if (pi != null)
        {
            groupId = pi.PollGroupID;
        }

        // Check permissions
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }


    /// <summary>
    /// AnswerEdit event handler.
    /// </summary>
    private void AnswerEdit_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("Polls_Edit_Answer_Edit.aspx?answerId=" + AnswerEdit.ItemID.ToString() + "&groupId=" + groupId + "&saved=1"));
    }
}