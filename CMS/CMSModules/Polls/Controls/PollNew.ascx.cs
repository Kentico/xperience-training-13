using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_PollNew : CMSAdminEditControl
{
    #region "Variables"

    private int mGroupID;
    private int mSiteID;
    private Guid mGroupGUID = Guid.Empty;
    private string mLicenseError;
    private bool error;
    private PollInfo mPoll;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site ID for which the poll should be created.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteID <= 0)
            {
                mSiteID = SiteContext.CurrentSiteID;
            }

            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Gets or sets the group ID for which the poll should be created.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }


    /// <summary>
    /// Gets or sets the group GUID for which the poll should be created.
    /// </summary>
    public Guid GroupGUID
    {
        get
        {
            return mGroupGUID;
        }
        set
        {
            mGroupGUID = value;
        }
    }


    /// <summary>
    /// Gets or sets license error message.
    /// </summary>
    public string LicenseError
    {
        get
        {
            return mLicenseError;
        }
        set
        {
            mLicenseError = value;
        }
    }


    /// <summary>
    /// Indicates if a poll should be created as "global poll".
    /// </summary>
    public bool CreateGlobal
    {
        get;
        set;
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Poll
    /// </summary>
    private PollInfo Poll
    {
        get
        {
            return mPoll ?? (mPoll = new PollInfo());
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Hide code name editing in simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        // Set edited object
        EditedObject = Poll;

        // Init the labels
        rfvCodeName.ErrorMessage = GetString("general.requirescodename");
        rfvDisplayName.ErrorMessage = GetString("general.requiresdisplayname");
        rfvQuestion.ErrorMessage = GetString("Polls_New.QuestionError");
        rfvMaxLength.ErrorMessage = GetString("general.errortexttoolong");

        lblTitle.Text = GetString("Polls_New.TitleLabel");
        lblQuestion.Text = GetString("Polls_New.QuestionLabel");

        // Set if it is live site
        txtDisplayName.IsLiveSite = txtTitle.IsLiveSite = txtQuestion.IsLiveSite = IsLiveSite;

        if (!RequestHelper.IsPostBack())
        {
            ClearForm();
        }
    }


    /// <summary>
    /// Resets all boxes.
    /// </summary>
    public override void ClearForm()
    {
        txtCodeName.Text = null;
        txtDisplayName.Text = null;
        txtTitle.Text = null;
        txtQuestion.Text = null;
    }


    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (CreateGlobal)
        {
            if (!CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY))
            {
                return;
            }
        }
        else
        {
            if (!CheckPermissions("cms.polls", PERMISSION_MODIFY))
            {
                return;
            }
        }

        // Generate code name in simple mode
        string codeName = txtCodeName.Text.Trim();
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            codeName = ValidationHelper.GetCodeName(txtDisplayName.Text.Trim(), null, null);
        }

        // Perform validation
        string errorMessage = new Validator().NotEmpty(codeName, rfvCodeName.ErrorMessage)
            .NotEmpty(txtDisplayName.Text, rfvDisplayName.ErrorMessage).NotEmpty(txtQuestion.Text.Trim(), rfvQuestion.ErrorMessage).Result;

        // Check CodeName for identifier format
        if (!ValidationHelper.IsCodeName(codeName))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (string.IsNullOrEmpty(errorMessage))
        {
            // Create new 
            Poll.PollAllowMultipleAnswers = false;
            Poll.PollAccess = SecurityAccessEnum.AllUsers;

            // Check if codename already exists on a group or is a global
            PollInfo pi;
            if (CreateGlobal)
            {
                pi = PollInfoProvider.GetPollInfo("." + codeName, 0);
                if ((pi != null) && (pi.PollSiteID <= 0))
                {
                    errorMessage = GetString("polls.codenameexists");
                }
            }
            else
            {
                pi = PollInfoProvider.GetPollInfo(codeName, SiteID, GroupID);
                if ((pi != null) && (pi.PollSiteID > 0))
                {
                    errorMessage = GetString("polls.codenameexists");
                }
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                // Set the fields
                Poll.PollCodeName = codeName;
                Poll.PollDisplayName = txtDisplayName.Text.Trim();
                Poll.PollTitle = txtTitle.Text.Trim();
                Poll.PollQuestion = txtQuestion.Text.Trim();
                Poll.PollLogActivity = true;
                if (GroupID > 0)
                {
                    if (SiteID <= 0)
                    {
                        ShowError(GetString("polls.nositeid"));
                        return;
                    }

                    Poll.PollGroupID = GroupID;
                    Poll.PollSiteID = SiteID;
                }
                else
                {
                    // Assigned poll to particular site if it is not global poll
                    if (!CreateGlobal)
                    {
                        if (!PollInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Polls, ObjectActionEnum.Insert))
                        {
                            LicenseError = GetString("LicenseVersion.Polls");
                        }
                        else
                        {
                            Poll.PollSiteID = SiteID;
                        }
                    }
                }

                // Save the object
                PollInfoProvider.SetPollInfo(Poll);
                ItemID = Poll.PollID;

                // Add global poll to current site
                if ((SiteContext.CurrentSite != null) && CreateGlobal)
                {
                    if (PollInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Polls, ObjectActionEnum.Insert))
                    {
                        if ((Poll.PollGroupID == 0) && (Poll.PollSiteID == 0))
                        {
                            // Bind only global polls to current site
                            PollInfoProvider.AddPollToSite(Poll.PollID, SiteContext.CurrentSiteID);
                        }
                    }
                    else
                    {
                        LicenseError = GetString("LicenseVersion.Polls");
                    }
                }

                // Redirect to edit mode
                if (!error)
                {
                    RaiseOnSaved();
                }
            }
            else
            {
                // Error message - code name already exists
                ShowError(GetString("polls.codenameexists"));
            }
        }
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            // Error message - validation
            ShowError(errorMessage);
        }
    }
}