using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_AnswerEdit : CMSAdminEditControl
{
    #region "Variables"

    private PollAnswerInfo mPollAnswer;
    private FormEngineUserControl bizFormElem;

    #endregion


    #region "Properties"

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
    /// Gets or sets the answer ID.
    /// </summary>
    public int PollId
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["pollid"], 0);
        }
        set
        {
            ViewState["pollid"] = value;
        }
    }


    /// <summary>
    /// Gets or sets saved property.
    /// </summary>
    public bool Saved
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["saved"], false);
        }
        set
        {
            ViewState["saved"] = value;
        }
    }

    #endregion


    #region "Private properties"

    private PollAnswerInfo PollAnswer
    {
        get
        {
            return mPollAnswer ?? (mPollAnswer = (ItemID > 0) ? PollAnswerInfoProvider.GetPollAnswerInfo(ItemID) : new PollAnswerInfo());
        }
        set
        {
            mPollAnswer = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Required field validator error messages initialization
        rfvAnswerText.ErrorMessage = GetString("Polls_Answer_Edit.AnswerTextError");

        // Set if it is live site
        txtAnswerText.IsLiveSite = IsLiveSite;

        // Set edited object
        EditedObject = PollAnswer;

        // Load BizForm selector if BizForms module is available
        if (ModuleEntryManager.IsModuleLoaded(ModuleName.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleName.BIZFORM, SiteContext.CurrentSiteName))
        {
            bizFormElem = (FormEngineUserControl)Page.LoadUserControl("~/CMSModules/BizForms/FormControls/SelectBizForm.ascx");
            bizFormElem.ShortID = "bizFormElem";
            bizFormElem.SetValue("ShowSiteFilter", false);
            plcBizFormSelector.Controls.Add(bizFormElem);
            bizFormElem.Visible = true;

            UniSelector uniSelector = bizFormElem.GetValue("UniSelector") as UniSelector;
            if (uniSelector != null)
            {
                uniSelector.OnSelectionChanged += BizFormSelector_OnSelectionChanged;
            }
        }

        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            LoadData();
        }
    }


    /// <summary>
    /// Loads new data for this control.
    /// </summary>
    public void LoadData()
    {
        // If working with existing record
        if (ItemID > 0)
        {
            if ((PollAnswer) != null && (PollAnswer.AnswerPollID > 0))
            {
                // Fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    ReloadData();
                }

                // When saved, display info message
                if (Saved)
                {
                    ShowChangesSaved();
                    Saved = false;
                }

                PollId = PollAnswer.AnswerPollID;
            }
        }
        // If creating new record
        else
        {
            plcVotes.Visible = false;
            txtVotes.Text = "0";
        }
    }


    /// <summary>
    /// Clears data.
    /// </summary>
    public override void ClearForm()
    {
        base.ClearForm();
        txtAnswerText.Text = null;
        txtVotes.Text = null;
    }


    /// <summary>
    /// Reloads answer data.
    /// </summary>
    public override void ReloadData()
    {
        ClearForm();
        // Force reloading of PollAnswer info object if ID changed. The getter does the actual load.
        if ((PollAnswer != null) && (ItemID != PollAnswer.AnswerPollID))
        {
            PollAnswer = null;
            EditedObject = PollAnswer;
        }

        if ((PollAnswer != null) && (PollAnswer.AnswerPollID > 0))
        {
            // Load the fields
            txtAnswerText.Text = PollAnswer.AnswerText;
            chkAnswerEnabled.Checked = PollAnswer.AnswerEnabled;
            txtVotes.Text = PollAnswer.AnswerCount.ToString();
            plcVotes.Visible = true;
            pnlGeneralHeading.Visible = true;

            // Check if bizform module is available (for open-ended answers).
            if (ModuleEntryManager.IsModuleLoaded(ModuleName.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleName.BIZFORM, SiteContext.CurrentSiteName))
            {
                // Show open-ended answer settings only for site poll
                PollInfo pi = PollInfoProvider.GetPollInfo(PollAnswer.AnswerPollID);
                plcOpenAnswer.Visible = (pi != null) && (pi.PollSiteID > 0) && (pi.PollGroupID == 0);

                chkAnswerIsOpenEnded.Checked = updPanelForm.Visible = plcOpenAnswerSettings.Visible = PollAnswer.AnswerIsOpenEnded;
                bizFormElem.Value = PollAnswer.AnswerForm;
                alternativeFormElem.ClassName = "BizForm." + bizFormElem.Text;
                alternativeFormElem.Value = PollAnswer.AnswerAlternativeForm;
                chkAnswerHideForm.Checked = PollAnswer.AnswerHideForm;
            }
        }
        else
        {
            txtAnswerText.Text = String.Empty;
            plcVotes.Visible = false;
        }
    }

    
    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check permission for answer object (global/site poll)
        if (!CheckModifyPermission(PollId))
        {
            return;
        }

        string errorMessage;
        // Validate the input
        if (txtVotes.Visible)
        {
            errorMessage = new Validator().NotEmpty(txtAnswerText.Text, rfvAnswerText.ErrorMessage)
                .IsPositiveNumber(txtVotes.Text, GetString("Polls_Answer_Edit.VotesNotNumber"), true)
                .IsInteger(txtVotes.Text, GetString("Polls_Answer_Edit.VotesNotNumber")).Result;
        }
        else
        {
            errorMessage = new Validator().NotEmpty(txtAnswerText.Text, rfvAnswerText.ErrorMessage).Result;
        }

        if (String.IsNullOrEmpty(errorMessage))
        {
            // If pollAnswer doesn't already exist, create new one
            if (PollAnswer.AnswerPollID <= 0)
            {
                PollAnswer.AnswerOrder = PollAnswerInfoProvider.GetLastAnswerOrder(PollId);
                PollAnswer.AnswerCount = 0;
                PollAnswer.AnswerPollID = PollId;
            }

            // Set the fields
            PollAnswer.AnswerEnabled = chkAnswerEnabled.Checked;
            PollAnswer.AnswerText = txtAnswerText.Text.Trim();
            PollAnswer.AnswerCount = ValidationHelper.GetInteger(txtVotes.Text, 0);

            if (plcOpenAnswer.Visible)
            {
                string answerForm = ValidationHelper.GetString(bizFormElem.Value, string.Empty);
                if (chkAnswerIsOpenEnded.Checked && string.IsNullOrEmpty(answerForm))
                {
                    ShowError(GetString("Polls_Answer_Edit.SelectForm"));
                    return;
                }
                PollAnswer.AnswerForm = answerForm;
                PollAnswer.AnswerAlternativeForm = ValidationHelper.GetString(alternativeFormElem.Value, string.Empty);
                PollAnswer.AnswerHideForm = chkAnswerHideForm.Checked;
            }

            // Save the data
            PollAnswerInfoProvider.SetPollAnswerInfo(PollAnswer);
            Saved = true;
            ItemID = PollAnswer.AnswerID;

            // Raise event;
            RaiseOnSaved();
        }
        else
        {
            // Error message - Validation
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Checks modify permission. Returns false if checking failed.
    /// </summary>
    /// <param name="pollId">Poll ID</param>
    private bool CheckModifyPermission(int pollId)
    {
        // Get parent of answer object and see if it is global poll or site poll
        PollInfo pi = null;
        if (pollId > 0) // non-zero value when creating new poll
        {
            pi = PollInfoProvider.GetPollInfo(pollId);
        }
        else if (PollAnswer.AnswerPollID > 0) // not null when modifying existing answer
        {
            pi = PollInfoProvider.GetPollInfo(PollAnswer.AnswerPollID);
        }
        if (pi != null)
        {
            return (pi.PollSiteID > 0) && CheckPermissions("cms.polls", PERMISSION_MODIFY) ||
                   (pi.PollSiteID <= 0) && CheckPermissions("cms.polls", PERMISSION_GLOBALMODIFY);
        }
        return false;
    }


    /// <summary>
    /// Show/hide open-ended answer settings
    /// </summary>
    protected void chkAnswerIsOpenEnded_CheckedChanged(object sender, EventArgs e)
    {
        updPanelForm.Visible = plcOpenAnswerSettings.Visible = chkAnswerIsOpenEnded.Checked;
        if (!chkAnswerIsOpenEnded.Checked)
        {
            bizFormElem.Value = string.Empty;
            alternativeFormElem.Value = string.Empty;
        }
    }

    /// <summary>
    /// Fired when BizForm is selected
    /// </summary>
    protected void BizFormSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        alternativeFormElem.ClassName = "BizForm." + bizFormElem.Text;
    }
}