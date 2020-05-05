using System;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Boards_BoardEdit : CMSAdminEditControl
{
    #region "Private fields"

    private int mBoardID = 0;
    private BoardInfo mBoard = null;
    private bool mExternalParent = false;

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
    /// Current board ID.
    /// </summary>
    public int BoardID
    {
        get
        {
            if (mBoardID == 0)
            {
                if (mBoard != null)
                {
                    return mBoard.BoardID;
                }

                mBoardID = QueryHelper.GetInteger("boardid", 0);
            }

            return mBoardID;
        }
        set
        {
            mBoardID = value;

            mBoard = null;
        }
    }


    /// <summary>
    /// Current board object.
    /// </summary>
    public BoardInfo Board
    {
        get
        {
            return mBoard ?? (mBoard = BoardInfoProvider.GetBoardInfo(BoardID));
        }
        set
        {
            mBoard = value;

            mBoardID = 0;
        }
    }


    /// <summary>
    /// Indicates whether the control has external parent.
    /// </summary>
    public bool ExternalParent
    {
        get
        {
            return mExternalParent;
        }
        set
        {
            mExternalParent = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        txtBoardDisplayName.IsLiveSite = IsLiveSite;
        txtBoardDescription.IsLiveSite = IsLiveSite;
        txtOptInURL.IsLiveSite = IsLiveSite;

        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
            return;
        }

        // If control should be hidden save view state memory
        if (!Visible)
        {
            EnableViewState = false;
        }

        // Initializes the controls
        SetupControls();

        // Reload data if necessary
        if (!RequestHelper.IsPostBack() && !IsLiveSite)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControls()
    {
        // Hide code name editing for simple mode
        if (DisplayMode == ControlDisplayModeEnum.Simple)
        {
            plcCodeName.Visible = false;
        }

        // Register scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowDateFields",
                                               ScriptHelper.GetScript(
                                                   "function ShowDateFields(){ \n" +
                                                   "    document.getElementById('" + lblBoardOpenFrom.ClientID + "').style.display = 'block'; \n" +
                                                   "    document.getElementById('" + dtpBoardOpenFrom.ClientID + "').style.display = 'block'; \n" +
                                                   "    document.getElementById('" + lblBoardOpenTo.ClientID + "').style.display = 'block'; \n" +
                                                   "    document.getElementById('" + dtpBoardOpenTo.ClientID + "').style.display = 'block'; } \n" +
                                                   "function HideDateFields(){ \n " +
                                                   "    document.getElementById('" + lblBoardOpenFrom.ClientID + "').style.display = 'none'; \n " +
                                                   "    document.getElementById('" + dtpBoardOpenFrom.ClientID + "').style.display = 'none'; \n" +
                                                   "    document.getElementById('" + lblBoardOpenTo.ClientID + "').style.display = 'none'; \n" +
                                                   "    document.getElementById('" + dtpBoardOpenTo.ClientID + "').style.display = 'none'; }"
                                                   ));

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CheckBoxes",
                                               ScriptHelper.GetScript(@"
                function check(txtId,chk,inhV)  
                {
                    txt = document.getElementById(txtId);
                    if ((txt != null)&&(chk != null))
                    {
                        if (chk.checked)
                        {
                            txt.disabled = 'disabled';
                            txt.value = inhV;
                        }
                        else
                        {
                            txt.disabled = '';
                        }
                    }
                }"
                                                   ));

        // Set the labels
        lblBoardCodeName.Text = GetString("general.codename") + ResHelper.Colon;
        lblBoardOwner.Text = GetString("board.owner.title") + ResHelper.Colon;
        lblBoardDescription.Text = GetString("general.description") + ResHelper.Colon;
        lblBoardDisplayName.Text = GetString("general.displayname") + ResHelper.Colon;
        lblBoardEnable.Text = GetString("general.enable") + ResHelper.Colon;
        lblBoardOpen.Text = GetString("general.open") + ResHelper.Colon;
        lblBoardOpenFrom.Text = GetString("general.openfrom") + ResHelper.Colon;
        lblBoardOpenTo.Text = GetString("general.opento") + ResHelper.Colon;
        lblBoardRequireEmail.Text = GetString("board.edit.requireemail") + ResHelper.Colon;
        lblUnsubscriptionUrl.Text = GetString("general.unsubscriptionurl") + ResHelper.Colon;
        lblBaseUrl.Text = GetString("general.baseurl") + ResHelper.Colon;
        btnOk.Text = GetString("general.ok");

        chkBoardOpen.Attributes.Add("onclick", "if(this.checked){ ShowDateFields() }else{ HideDateFields() }");

        // Set the error messages for validators
        rfvBoardCodeName.ErrorMessage = GetString("board.edit.errcodename");
        rfvBoardDisplayName.ErrorMessage = GetString("board.edit.errdisplayname");

        if (IsLiveSite)
        {
            plcUnsubscription.Visible = false;
        }

        chkInheritBaseUrl.Attributes.Add("onclick", "check('" + txtBaseUrl.ClientID + "', this,'" + ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardBaseUrl"), "") + "')");
        chkInheritUnsubUrl.Attributes.Add("onclick", "check('" + txtUnsubscriptionUrl.ClientID + "', this,'" + ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardUnsubsriptionURL"), "") + "')");
        chkInheritOptInURL.Attributes.Add("onclick", "check('" + txtOptInURL.PathTextBox.ClientID + "', this,'" + ValidationHelper.GetString(SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSBoardOptInApprovalPath"), "") + "');ChangeState_" + txtOptInURL.ClientID + "(!this.checked);");

        chkEnableOptIn.NotSetChoice.Text = chkSendOptInConfirmation.NotSetChoice.Text = GetString("general.sitesettings") + " (##DEFAULT##)";
        chkEnableOptIn.SetDefaultValue(BoardInfoProvider.EnableDoubleOptIn(SiteContext.CurrentSiteName));
        chkSendOptInConfirmation.SetDefaultValue(BoardInfoProvider.SendOptInConfirmation(SiteContext.CurrentSiteName));

        if (ActivitySettingsHelper.IsModuleLoaded())
        {
            plcOnline.Visible = true;
        }
    }


    /// <summary>
    /// Reloads the data in the form.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControls();

        EditedObject = Board;

        if (Board != null)
        {
            txtBoardCodeName.Text = Board.BoardName;
            txtBoardDisplayName.Text = Board.BoardDisplayName;
            txtBoardDescription.Text = Board.BoardDescription;
            txtUnsubscriptionUrl.Text = Board.BoardUnsubscriptionURL;
            txtBaseUrl.Text = Board.BoardBaseURL;

            chkBoardEnable.Checked = Board.BoardEnabled;
            chkBoardOpen.Checked = Board.BoardOpened;
            chkBoardRequireEmail.Checked = Board.BoardRequireEmails;
            chkSubscriptionsEnable.Checked = Board.BoardEnableSubscriptions;

            dtpBoardOpenFrom.SelectedDateTime = Board.BoardOpenedFrom;
            dtpBoardOpenTo.SelectedDateTime = Board.BoardOpenedTo;

            // Load the owner info
            string owner = "";
            if (Board.BoardGroupID > 0)
            {
                owner = GetString("board.owner.group");
            }
            else if (Board.BoardUserID > 0)
            {
                owner = GetString("general.user");
            }
            else
            {
                owner = GetString("board.owner.document");
            }

            lblBoardOwnerText.Text = owner;

            // Set base/unsubscription URL inheritance
            chkInheritBaseUrl.Checked = (Board.GetValue("BoardBaseUrl") == null);
            chkInheritUnsubUrl.Checked = (Board.GetValue("BoardUnsubscriptionUrl") == null);


            if (!chkInheritBaseUrl.Checked)
            {
                txtBaseUrl.Attributes.Remove("disabled");
            }
            else
            {
                txtBaseUrl.Attributes.Add("disabled", "disabled");
            }

            if (!chkInheritUnsubUrl.Checked)
            {
                txtUnsubscriptionUrl.Attributes.Remove("disabled");
            }
            else
            {
                txtUnsubscriptionUrl.Attributes.Add("disabled", "disabled");
            }

            if (!chkInheritOptInURL.Checked)
            {
                txtOptInURL.Attributes.Remove("disabled");
            }
            else
            {
                txtOptInURL.Attributes.Add("disabled", "disabled");
            }

            // Double opt-in settings
            chkEnableOptIn.InitFromThreeStateValue(Board, "BoardEnableOptIn");
            chkSendOptInConfirmation.InitFromThreeStateValue(Board, "BoardSendOptInConfirmation");
            txtOptInURL.Text = Board.BoardOptInApprovalURL;
            chkInheritOptInURL.Checked = (Board.GetValue("BoardOptInApprovalURL") == null);
            txtOptInURL.Enabled = !chkInheritOptInURL.Checked;

            // If the open date-time details should be displayed
            bool isChecked = chkBoardOpen.Checked;
            lblBoardOpenFrom.Attributes.Add("style", (isChecked) ? "display: block;" : "display: none;");
            dtpBoardOpenFrom.Attributes.Add("style", (isChecked) ? "display: block;" : "display: none;");
            lblBoardOpenTo.Attributes.Add("style", (isChecked) ? "display: block;" : "display: none;");
            dtpBoardOpenTo.Attributes.Add("style", (isChecked) ? "display: block;" : "display: none;");

            if (plcOnline.Visible)
            {
                chkLogActivity.Checked = Board.BoardLogActivity;
            }
        }
    }


    #region "Event handlers"

    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
        {
            return;
        }

        if (Board != null)
        {
            string errMsg = ValidateForm();

            // If the entries were valid
            if (string.IsNullOrEmpty(errMsg))
            {
                // Get info on existing board

                try
                {
                    // Update board information
                    if (plcCodeName.Visible)
                    {
                        Board.BoardName = txtBoardCodeName.Text;
                    }

                    Board.BoardDisplayName = txtBoardDisplayName.Text;
                    Board.BoardDescription = txtBoardDescription.Text;
                    Board.BoardEnabled = chkBoardEnable.Checked;
                    Board.BoardOpened = chkBoardOpen.Checked;
                    Board.BoardOpenedFrom = dtpBoardOpenFrom.SelectedDateTime;
                    Board.BoardOpenedTo = dtpBoardOpenTo.SelectedDateTime;
                    if (!IsLiveSite)
                    {
                        Board.BoardUnsubscriptionURL = chkInheritUnsubUrl.Checked ? null : txtUnsubscriptionUrl.Text.Trim();
                        Board.BoardBaseURL = chkInheritBaseUrl.Checked ? null : txtBaseUrl.Text.Trim();
                    }
                    Board.BoardRequireEmails = chkBoardRequireEmail.Checked;
                    Board.BoardEnableSubscriptions = chkSubscriptionsEnable.Checked;

                    if (plcOnline.Visible)
                    {
                        Board.BoardLogActivity = chkLogActivity.Checked;
                    }

                    // Double opt-in
                    Board.BoardOptInApprovalURL = chkInheritOptInURL.Checked ? null : txtOptInURL.Text.Trim();
                    chkEnableOptIn.SetThreeStateValue(Board, "BoardEnableOptIn");
                    chkSendOptInConfirmation.SetThreeStateValue(Board, "BoardSendOptInConfirmation");

                    // Save changes
                    BoardInfoProvider.SetBoardInfo(Board);

                    // Inform user on success
                    ShowChangesSaved();

                    // Refresh tree if external parent
                    if (ExternalParent)
                    {
                        ltlScript.Text = ScriptHelper.GetScript("window.parent.parent.frames['tree'].RefreshNode('" + Board.BoardDisplayName + "', '" + BoardID + "')");
                    }

                    // Refresh the fields initialized with JavaScript
                    ReloadData();

                    ScriptHelper.RefreshTabHeader(Page, txtBoardDisplayName.Text);
                }
                catch (Exception ex)
                {
                    LogAndShowError("MessageBoard", "UPDATEBOARDPROPERTIES", ex);
                }
            }
            else
            {
                // Inform user on error
                ShowError(errMsg);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Validates form entries.
    /// </summary>    
    private string ValidateForm()
    {
        string errMsg = new Validator().NotEmpty(txtBoardDisplayName.Text.Trim(), rfvBoardDisplayName.ErrorMessage).Result;

        if (txtBoardCodeName.Visible && String.IsNullOrEmpty(errMsg))
        {
            errMsg = new Validator().NotEmpty(txtBoardCodeName.Text.Trim(), rfvBoardCodeName.ErrorMessage).Result;

            if (!ValidationHelper.IsCodeName(txtBoardCodeName.Text.Trim()))
            {
                errMsg = GetString("general.errorcodenameinidentifierformat");
            }
        }

        if (!dtpBoardOpenFrom.IsValidRange() || !dtpBoardOpenTo.IsValidRange())
        {
            errMsg = GetString("general.errorinvaliddatetimerange");
        }

        if (string.IsNullOrEmpty(errMsg))
        {
            // Check if the board with given name doesn't exist for particular document
            BoardInfo bi = BoardInfoProvider.GetBoardInfo(txtBoardCodeName.Text.Trim(), Board.BoardDocumentID);
            if ((bi != null) && (bi.BoardID != BoardID))
            {
                errMsg = GetString("general.codenameexists");
            }

            if (errMsg == "")
            {
                // If the board is open check date-time settings
                if (chkBoardOpen.Checked)
                {
                    //// Initialize default values
                    DateTime from = DateTimeHelper.ZERO_TIME;
                    DateTime to = DateTimeHelper.ZERO_TIME;
                    bool wasWrongDateTime = true;


                    //// Check if the date-time value is in valid format
                    bool isValidDateTime = ((DateTime.TryParse(dtpBoardOpenFrom.DateTimeTextBox.Text, out from) || string.IsNullOrEmpty(dtpBoardOpenFrom.DateTimeTextBox.Text)) &&
                                            (DateTime.TryParse(dtpBoardOpenTo.DateTimeTextBox.Text, out to) || string.IsNullOrEmpty(dtpBoardOpenTo.DateTimeTextBox.Text)));

                    // Check if the date-time doesn't overleap
                    if (isValidDateTime)
                    {
                        // If the date-time values are valid
                        if ((from <= to) || ((from == DateTimeHelper.ZERO_TIME) || (to == DateTimeHelper.ZERO_TIME)))
                        {
                            wasWrongDateTime = false;
                        }
                    }

                    if (wasWrongDateTime)
                    {
                        errMsg = GetString("board.edit.wrongtime");
                    }
                }
            }
        }

        return errMsg;
    }

    #endregion
}
