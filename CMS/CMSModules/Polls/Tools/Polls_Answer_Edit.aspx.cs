using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.Polls;
using CMS.UIControls;


public partial class CMSModules_Polls_Tools_Polls_Answer_Edit : CMSPollsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get AnswerID and PollID from querystring
        int pollId = QueryHelper.GetInteger("pollid", 0);

        string currentPollAnswer = GetString("Polls_Answer_Edit.NewItemCaption");

        int answerId = QueryHelper.GetInteger("answerId", 0);
        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            AnswerEdit.Saved = true;
        }
        AnswerEdit.ItemID = answerId;
        AnswerEdit.PollId = pollId;

        if (answerId > 0)
        {
            // Modifying existing answer
            PollAnswerInfo pollAnswerObj = PollAnswerInfoProvider.GetPollAnswerInfo(answerId);
            EditedObject = pollAnswerObj;

            if (pollAnswerObj != null)
            {
                currentPollAnswer = GetString("Polls_Answer_Edit.AnswerLabel") + " " + pollAnswerObj.AnswerOrder.ToString();
                pollId = pollAnswerObj.AnswerPollID;
            }
        }
        else
        {
            // Creating new answer - check if parent object exists
            EditedObject = PollInfoProvider.GetPollInfo(pollId);
        }

        var poll = EditedObject as PollInfo ?? PollInfoProvider.GetPollInfo(pollId);

        CheckPollsReadPermission(poll.PollSiteID);

        // Create breadcrumbs
        CreateBreadCrumbs(pollId, currentPollAnswer);

        HeaderAction add = new HeaderAction
        {
            Text = GetString("Polls_Answer_List.NewItemCaption"),
            RedirectUrl = ResolveUrl("Polls_Answer_Edit.aspx?pollId=" + pollId),
        };

        CurrentMaster.HeaderActions.AddAction(add);

        AnswerEdit.OnSaved += AnswerEdit_OnSaved;
        AnswerEdit.IsLiveSite = false;
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    /// <param name="text">Breadcrumb text</param>
    private void CreateBreadCrumbs(int pollId, string text)
    {
        // Initializes page title control	
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("Polls_Answer_Edit.ItemListLink"),
            RedirectUrl = "~/CMSModules/Polls/Tools/Polls_Answer_List.aspx?pollId=" + pollId
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = text
        });

        // Do not include type as breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix("");
    }


    /// <summary>
    /// AnswerEdit event handler.
    /// </summary>
    protected void AnswerEdit_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect(UrlResolver.ResolveUrl("Polls_Answer_Edit.aspx?answerId=" + AnswerEdit.ItemID.ToString() + "&saved=1"));
    }
}
