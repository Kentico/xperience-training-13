using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

using CMS.Activities;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Polls;
using CMS.PortalEngine;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_View_PollView : CMSUserControl
{
    #region "Events"

    public event EventHandler OnAfterVoted;

    #endregion


    #region "Variables"

    protected bool mShowGraph = true;
    protected CountTypeEnum mCodeType = CountTypeEnum.Absolute;
    protected bool mShowResultsAfterVote = true;
    protected bool mCheckVoted = true;
    protected bool mCheckPermissions = true;
    protected bool mCheckOpen = true;
    protected bool mHideWhenNotAuthorized = false;
    protected bool mHideWhenNotOpened = false;
    protected string mButtonText = null;
    protected int mCacheMinutes = 0;
    protected string errMessage = null;
    protected bool hasPermission = false;
    protected bool isOpened = false;
    protected DataSet answers = null;
    private PollInfo pi = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the code name of the poll, which should be displayed.
    /// </summary>
    public string PollCodeName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PollCodeName"], null);
        }
        set
        {
            ViewState["PollCodeName"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID of the poll (optional).
    /// </summary>
    public int PollSiteID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PollSiteID"], 0);
        }
        set
        {
            ViewState["PollSiteID"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the group ID of the poll (optional).
    /// </summary>
    public int PollGroupID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PollGroupID"], 0);
        }
        set
        {
            ViewState["PollGroupID"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the graph of the poll is displayed.
    /// </summary>
    public bool ShowGraph
    {
        get
        {
            return mShowGraph;
        }
        set
        {
            mShowGraph = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of the representation of the answers' count in the graph.
    /// </summary>
    public CountTypeEnum CountType
    {
        get
        {
            return mCodeType;
        }
        set
        {
            mCodeType = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether graph is displayed after answering the poll.
    /// </summary>
    public bool ShowResultsAfterVote
    {
        get
        {
            return mShowResultsAfterVote;
        }
        set
        {
            mShowResultsAfterVote = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current user has voted is checked.
    /// </summary>
    public bool CheckVoted
    {
        get
        {
            return mCheckVoted;
        }
        set
        {
            mCheckVoted = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return mCheckPermissions;
        }
        set
        {
            mCheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or stes the value that indicates whether open from/to time is checked
    /// </summary>
    public bool CheckOpen
    {
        get
        {
            return mCheckOpen;
        }
        set
        {
            mCheckOpen = value;
        }
    }


    /// <summary>
    /// If true, the control hides when not authorized, 
    /// otherwise the control displays the message and does not allow to vote.
    /// </summary>
    public bool HideWhenNotAuthorized
    {
        get
        {
            return mHideWhenNotAuthorized;
        }
        set
        {
            mHideWhenNotAuthorized = value;
        }
    }


    /// <summary>
    /// If true, the control hides when not opened, 
    /// otherwise the control does not allow to vote.
    /// </summary>
    public bool HideWhenNotOpened
    {
        get
        {
            return mHideWhenNotOpened;
        }
        set
        {
            mHideWhenNotOpened = value;
        }
    }


    /// <summary>
    /// Gets or sets the text of the vote button.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return ValidationHelper.GetString(mButtonText, GetString("Polls.Vote"));
        }
        set
        {
            mButtonText = value;
        }
    }


    /// <summary>
    /// Vote button.
    /// </summary>
    public LocalizedButton VoteButton
    {
        get
        {
            return btnVote;
        }
    }


    /// <summary>
    /// Gets or sets the WebPart cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get
        {
            return mCacheMinutes;
        }
        set
        {
            mCacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the poll answers dataset.
    /// </summary>
    public DataSet Answers
    {
        get
        {
            if (DataHelper.DataSourceIsEmpty(answers))
            {
                // Try to get data from cache
                using (var cs = new CachedSection<DataSet>(ref answers, CacheMinutes, true, null, "pollanswers", PollCodeName))
                {
                    if (cs.LoadData)
                    {
                        // Get from database
                        if (pi != null)
                        {
                            answers = PollAnswerInfoProvider.GetPollAnswers()
                                                            .Columns("AnswerID, AnswerText, AnswerCount, AnswerEnabled, AnswerForm, AnswerAlternativeForm, AnswerHideForm")
                                                            .WhereEquals("AnswerPollID", pi.PollID);
                        }

                        if (cs.Cached)
                        {
                            // Prepare cache dependency
                            cs.CacheDependency = CacheHelper.GetCacheDependency("polls.pollanswer|all");
                        }

                        // Add to the cache
                        cs.Data = answers;
                    }
                }
            }

            return answers;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Visible)
        {
            ReloadData(false);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Display info messages if not empty
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text);
        lblResult.Visible = !string.IsNullOrEmpty(lblResult.Text);
    }


    /// <summary>
    /// Loads data.
    /// </summary>
    public void ReloadData(bool forceReload)
    {
        if (!StopProcessing)
        {
            SetContext();

            lblInfo.Text = string.Empty;
            lblInfo.Visible = false;

            if (pi == null)
            {
                pi = PollInfoProvider.GetPollInfo(PollCodeName, PollSiteID, PollGroupID);
                hasPermission = HasPermission();
                isOpened = IsOpened();
            }

            // Show poll if current user has permission or if poll should be displayed even if user is not authorized
            // and if poll is opened or if poll should be opened even if it is not opened
            // ... and show group poll if it is poll of current group
            bool showPoll = (pi != null) && (hasPermission || !HideWhenNotAuthorized) && (isOpened || !HideWhenNotOpened);
            // Show site poll only if it is poll of current site
            if (showPoll && (pi.PollSiteID > 0) && (pi.PollSiteID != SiteContext.CurrentSiteID))
            {
                showPoll = false;
            }

            // Show global poll only if it is allowed for current site
            if (showPoll && (pi.PollSiteID == 0))
            {
                showPoll = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSPollsAllowGlobal");
            }

            if (showPoll)
            {
                Visible = true;

                // Load title
                lblTitle.Text = HTMLHelper.HTMLEncode(pi.PollTitle);
                // Load question
                lblQuestion.Text = HTMLHelper.HTMLEncode(pi.PollQuestion);

                if ((!forceReload) || ((forceReload) && (ShowResultsAfterVote)))
                {
                    // Add answer section
                    CreateAnswerSection(forceReload, CheckVoted && PollInfoProvider.HasVoted(pi.PollID));
                }
                else
                {
                    // Hide answer panel
                    pnlAnswer.Visible = false;
                }

                if ((forceReload) && (isOpened))
                {
                    // Hide footer with vote button
                    pnlFooter.Visible = false;

                    // Add poll response after voting
                    if ((errMessage != null) && (errMessage.Trim() != ""))
                    {
                        // Display message if error occurs
                        lblInfo.Text = errMessage;
                        lblInfo.CssClass = "ErrorMessage";
                    }
                    else
                    {
                        // Display poll response message
                        lblResult.Text = HTMLHelper.HTMLEncode(pi.PollResponseMessage);
                    }
                }
                else if (isOpened)
                {
                    if (hasPermission && !(CheckVoted && (PollInfoProvider.HasVoted(pi.PollID))))
                    {
                        // Display footer wiht vote button
                        pnlFooter.Visible = true;
                        btnVote.Text = ButtonText;
                    }
                    else
                    {
                        pnlFooter.Visible = false;
                    }
                }
                else
                {
                    pnlFooter.Visible = false;
                    lblInfo.Text = GetString("Polls.Closed");
                }
            }
            else
            {
                Visible = false;
            }

            ReleaseContext();
        }
    }


    /// <summary>
    /// Creates poll answer section.
    /// </summary>
    /// <param name="reload">Indicates postback</param>
    /// <param name="hasVoted">Indicates if user has voted</param>
    protected void CreateAnswerSection(bool reload, bool hasVoted)
    {
        pnlAnswer.Controls.Clear();

        if (pi != null)
        {
            // Get poll's answers
            DataSet ds = Answers;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                int maxCount = 0;
                long sumCount = 0;
                bool hideSomeForm = false;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    // Sum answer counts and get highest
                    if (ValidationHelper.GetBoolean(row["AnswerEnabled"], true))
                    {
                        int count = ValidationHelper.GetInteger(row["AnswerCount"], 0);
                        sumCount += count;
                        if (count > maxCount)
                        {
                            maxCount = count;
                        }
                    }

                    // Check if any open-ended answer form should be hidden
                    if (ValidationHelper.GetBoolean(row["AnswerHideForm"], false))
                    {
                        hideSomeForm = true;
                    }
                }

                int index = 0;

                pnlAnswer.Controls.Add(new LiteralControl("<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">"));

                // Create the answers
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    bool enabled = ValidationHelper.GetBoolean(row["AnswerEnabled"], true);
                    if (enabled)
                    {
                        pnlAnswer.Controls.Add(new LiteralControl("<tr><td class=\"PollAnswer\" colspan=\"2\">"));

                        if (((reload) && (ShowResultsAfterVote)) || (!hasPermission && !HideWhenNotAuthorized)
                            || (!isOpened && !HideWhenNotOpened) || (CheckVoted && PollInfoProvider.HasVoted(pi.PollID)))
                        {
                            // Add label
                            LocalizedLabel lblItem = new LocalizedLabel();
                            lblItem.ID = "lbl" + ValidationHelper.GetInteger(row["AnswerID"], 0);
                            lblItem.EnableViewState = false;
                            lblItem.Text = HTMLHelper.HTMLEncode(ValidationHelper.GetString(row["AnswerText"], string.Empty));
                            lblItem.CssClass = "PollAnswerText";

                            pnlAnswer.Controls.Add(lblItem);
                        }
                        else
                        {
                            if (pi.PollAllowMultipleAnswers)
                            {
                                // Add checkboxes for multiple answers
                                CMSCheckBox chkItem = new CMSCheckBox();
                                chkItem.ID = "chk" + ValidationHelper.GetInteger(row["AnswerID"], 0);
                                chkItem.AutoPostBack = false;
                                chkItem.Text = HTMLHelper.HTMLEncode(ValidationHelper.GetString(row["AnswerText"], string.Empty));
                                chkItem.Checked = false;
                                chkItem.CssClass = "PollAnswerCheck";

                                if (hideSomeForm)
                                {
                                    chkItem.AutoPostBack = true;
                                }
                                pnlAnswer.Controls.Add(chkItem);
                            }
                            else
                            {
                                // Add radiobuttons
                                CMSRadioButton radItem = new CMSRadioButton();
                                radItem.ID = "rad" + ValidationHelper.GetInteger(row["AnswerID"], 0);
                                radItem.AutoPostBack = false;
                                radItem.GroupName = pi.PollCodeName + "Group";
                                radItem.Text = HTMLHelper.HTMLEncode(ValidationHelper.GetString(row["AnswerText"], string.Empty));
                                radItem.Checked = false;
                                radItem.CssClass = "PollAnswerRadio";

                                if (hideSomeForm)
                                {
                                    radItem.AutoPostBack = true;
                                }

                                pnlAnswer.Controls.Add(radItem);
                            }
                        }

                        pnlAnswer.Controls.Add(new LiteralControl("</td></tr>"));

                        if (ShowGraph || (hasVoted || reload) && ShowResultsAfterVote)
                        {
                            // Create graph under the answer
                            CreateGraph(maxCount, ValidationHelper.GetInteger(row["AnswerCount"], 0), sumCount, index);
                        }

                        index++;
                    }
                }

                pnlAnswer.Controls.Add(new LiteralControl("</table>"));
            }
        }
    }


    /// <summary>
    /// Creates graph bar for the answer.
    /// </summary>
    /// <param name="maxValue">Max answers' count</param>
    /// <param name="currentValue">Current answer count</param>
    /// <param name="countSummary">Count summary of all answers</param>
    /// <param name="index">Index</param>
    protected void CreateGraph(int maxValue, int currentValue, long countSummary, int index)
    {
        long ratio = 0;
        if (maxValue != 0)
        {
            ratio = Math.BigMul(100, currentValue) / (long)maxValue;
        }
        // Begin PollAnswerGraph
        pnlAnswer.Controls.Add(new LiteralControl("<tr><td style=\"width: 100%;\"><div class=\"PollGraph\">"));
        if (ratio != 0)
        {
            // PollAnswerItemGraph
            pnlAnswer.Controls.Add(new LiteralControl("<div class=\"PollGraph" + index +
                                                      "\" style=\"width:" + ratio + "%\">&nbsp;</div>"));
        }
        else
        {
            pnlAnswer.Controls.Add(new LiteralControl("&nbsp;"));
        }

        // End PollAnswerGraph
        pnlAnswer.Controls.Add(new LiteralControl("</div></td><td style=\"white-space: nowrap;\" class=\"PollCount\">"));

        // Create lable with answer count
        if (CountType == CountTypeEnum.Absolute)
        {
            // Absolute count
            pnlAnswer.Controls.Add(new LiteralControl(currentValue.ToString()));
        }
        else if (CountType == CountTypeEnum.Percentage)
        {
            // Percentage count
            long percent = 0;
            if (countSummary != 0)
            {
                percent = Math.BigMul(100, currentValue) / countSummary;
            }
            pnlAnswer.Controls.Add(new LiteralControl(percent.ToString() + "%"));
        }

        // End PollAnswerGraph
        pnlAnswer.Controls.Add(new LiteralControl("</td></tr>"));
    }


    /// <summary>
    /// On btnVote click event handler.
    /// </summary>
    protected void btnVote_OnClick(object sender, EventArgs e)
    {
        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblInfo.CssClass = "ErrorMessage";
            lblInfo.Text = GetString("General.BannedIP");
            return;
        }

        if (pi != null)
        {
            // Indicates whether user voted or not
            bool voted = false;

            // Indicates wheter all forms of all open-ended answers can be saved
            List<int> selectedAnswers = new List<int>();

            // Check if user has already voted
            if ((CheckVoted) && (PollInfoProvider.HasVoted(pi.PollID)))
            {
                errMessage = GetString("Polls.UserHasVoted");
                voted = true;
            }
            else if (isOpened)
            {
                // Get poll answers
                DataSet ds = Answers;
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    DataRowCollection rows = ds.Tables[0].Rows;

                    foreach (DataRow row in rows)
                    {
                        PollAnswerInfo pai = new PollAnswerInfo(row);

                        if (pai.AnswerEnabled)
                        {
                            bool selected = false;

                            // Find specific controls and update pollanswerinfo if controls are checked
                            if (pi.PollAllowMultipleAnswers)
                            {
                                // Find checkbox
                                CMSCheckBox chkItem = (CMSCheckBox)pnlAnswer.FindControl("chk" + pai.AnswerID);

                                if (chkItem != null)
                                {
                                    selected = chkItem.Checked;
                                }
                            }
                            else
                            {
                                // Find radiobutton
                                CMSRadioButton radItem = (CMSRadioButton)pnlAnswer.FindControl("rad" + pai.AnswerID);

                                if (radItem != null)
                                {
                                    selected = radItem.Checked;
                                }
                            }

                            if ((selected) && (pai.AnswerCount < Int32.MaxValue))
                            {
                                selectedAnswers.Add(pai.AnswerID);
                            }
                        }
                    }

                    if (selectedAnswers.Count > 0)
                    {
                        foreach (int aid in selectedAnswers)
                        {
                            // Set the vote
                            PollAnswerInfoProvider.Vote(aid);
                        }
                        voted = true;
                    }
                    else
                    {
                        // Set error message if no answer selected
                        lblInfo.CssClass = "ErrorMessage";
                        lblInfo.Text = GetString("Polls.DidNotVoted");
                    }

                    if ((CheckVoted) && (voted))
                    {
                        // Create cookie about user's voting
                        PollInfoProvider.SetVoted(pi.PollID);
                    }
                }
            }

            if (voted)
            {
                // Clear cache if it's used
                ClearAnswersCache();
                // Reload poll control
                ReloadData(true);

                if (OnAfterVoted != null)
                {
                    OnAfterVoted(this, EventArgs.Empty);
                }
            }
        }
    }


    /// <summary>
    /// Returns true if user has permissions.
    /// </summary>
    protected bool HasPermission()
    {
        if ((!CheckPermissions) || (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin)))
        {
            return true;
        }

        bool toReturn = false;

        if (pi != null)
        {
            // Access to all users
            if (pi.PollAccess == SecurityAccessEnum.AllUsers)
            {
                toReturn = true;
            }

            // Access to authenticated user
            if ((pi.PollAccess == SecurityAccessEnum.AuthenticatedUsers) && (AuthenticationHelper.IsAuthenticated()))
            {
                toReturn = true;
            }

            // Access to group members
            if ((pi.PollAccess == SecurityAccessEnum.GroupMembers) && (MembershipContext.AuthenticatedUser.IsGroupMember(PollGroupID)))
            {
                toReturn = true;
            }

            // Access to roles
            if ((pi.PollAccess == SecurityAccessEnum.AuthorizedRoles) && (MembershipContext.AuthenticatedUser != null))
            {
                foreach (String role in pi.AllowedRoles.Keys)
                {
                    if (MembershipContext.AuthenticatedUser.IsInRole(role, SiteContext.CurrentSiteName))
                    {
                        toReturn = true;
                        break;
                    }
                }
            }
        }

        return toReturn;
    }


    /// <summary>
    /// Return true if actual time is inside the poll's opening time or the poll's open from/to are not set.
    /// </summary>
    protected bool IsOpened()
    {
        if (!CheckOpen)
        {
            return true;
        }

        bool toReturn = false;

        if (pi != null)
        {
            DateTime now = DateTime.Now;
            if ((pi.PollOpenFrom == DateTimeHelper.ZERO_TIME) && (pi.PollOpenTo == DateTimeHelper.ZERO_TIME))
            {
                toReturn = true;
            }
            else if ((pi.PollOpenFrom != DateTimeHelper.ZERO_TIME) && (pi.PollOpenTo == DateTimeHelper.ZERO_TIME) && (pi.PollOpenFrom < now))
            {
                toReturn = true;
            }
            else if ((pi.PollOpenFrom == DateTimeHelper.ZERO_TIME) && (pi.PollOpenTo != DateTimeHelper.ZERO_TIME) && (pi.PollOpenTo > now))
            {
                toReturn = true;
            }
            else if ((pi.PollOpenFrom != DateTimeHelper.ZERO_TIME) && (pi.PollOpenTo != DateTimeHelper.ZERO_TIME) && (pi.PollOpenFrom < now) && (pi.PollOpenTo > now))
            {
                toReturn = true;
            }
        }

        return toReturn;
    }


    private void ClearAnswersCache()
    {
        // Remove the data from cache
        if (CacheMinutes > 0)
        {
            string useCacheItemName = CacheHelper.GetCacheItemName(null, "pollanswers", PollCodeName);
            CacheHelper.Remove(useCacheItemName);
        }

        answers = null;
    }

    #endregion
}