using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


[UIElement("CMS.Polls", "EditPoll.Answers")]
public partial class CMSModules_Polls_Tools_Polls_Answer_List : CMSPollsPage
{
    #region "Private variables"

    protected int pollId = 0;
    protected PollInfo pi = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get poll id from querystring		
        pollId = QueryHelper.GetInteger("pollId", 0);
        pi = PollInfoProvider.GetPollInfo(pollId);
        EditedObject = pi;

        // Check global and site read permmision
        CheckPollsReadPermission(pi.PollSiteID);

        if (CheckPollsModifyPermission(pi.PollSiteID, false))
        {
            HeaderAction newItem = new HeaderAction();
            newItem.Text = GetString("Polls_Answer_List.NewItemCaption");
            newItem.RedirectUrl = ResolveUrl("Polls_Answer_Edit.aspx?pollId=" + pollId);
            CurrentMaster.HeaderActions.AddAction(newItem);

            HeaderAction reset = new HeaderAction();
            reset.Text = GetString("Polls_Answer_List.ResetButton");
            reset.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("Polls_Answer_List.ResetConfirmation")) + ");";
            reset.CommandName = "btnReset_Click";
            CurrentMaster.HeaderActions.AddAction(reset);
            CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

            AnswerList.AllowEdit = true;
        }

        AnswerList.OnEdit += new EventHandler(AnswerList_OnEdit);
        AnswerList.PollId = pollId;
        AnswerList.IsLiveSite = false;
        AnswerList.AllowEdit = CheckPollsModifyPermission(pi.PollSiteID, false);
    }


    /// <summary>
    /// AnswerList edit action handler.
    /// </summary>
    private void AnswerList_OnEdit(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("Polls_Answer_Edit.aspx?answerId=" + AnswerList.SelectedItemID));
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

                // Check 'Modify' permission
                CheckPollsModifyPermission(pi.PollSiteID);

                if (pollId > 0)
                {
                    PollAnswerInfoProvider.ResetAnswers(pollId);
                    AnswerList.ReloadData();
                }
                break;
        }
    }
}