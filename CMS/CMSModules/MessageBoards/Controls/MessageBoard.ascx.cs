using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_MessageBoards_Controls_MessageBoard : CMSUserControl
{
    #region "Private fields"

    private BoardProperties mBoardProperties = new BoardProperties();

    // Currently processed board info
    private BoardInfo bi;

    private bool userVerified;

    private string mAliasPath;
    private string mCulture;
    private string mSiteName;
    private TreeNode mBoardNode;
    #endregion


    #region "Public properties"

    /// <summary>
    /// Returns current board properties.
    /// </summary>
    public BoardProperties BoardProperties
    {
        get
        {
            return mBoardProperties;
        }
        set
        {
            mBoardProperties = value;
        }
    }


    /// <summary>
    /// Prefix for the resource strings which are used for the strings on the message form. 
    /// </summary>
    public string FormResourcePrefix
    {
        get;
        set;
    }


    /// <summary>
    /// Transformation to be used for displaying the board message text.
    /// </summary>
    public string MessageTransformation
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the currently processed message board.
    /// </summary>
    public int MessageBoardID
    {
        get;
        set;
    }


    /// <summary>
    /// No messages text.
    /// </summary>
    public string NoMessagesText
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the page should be reloaded after action took place.
    /// </summary>
    public bool ReloadPageAfterAction
    {
        get;
        set;
    }


    /// <summary>
    /// OrderBy clause used for sorting messages.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return rptBoardMessages.OrderBy;
        }
        set
        {
            rptBoardMessages.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public string CacheItemName
    {
        get
        {
            return rptBoardMessages.CacheItemName;
        }
        set
        {
            rptBoardMessages.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public string CacheDependencies
    {
        get
        {
            return rptBoardMessages.CacheDependencies;
        }
        set
        {
            rptBoardMessages.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public int CacheMinutes
    {
        get
        {
            return rptBoardMessages.CacheMinutes;
        }
        set
        {
            rptBoardMessages.CacheMinutes = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Post alias path.
    /// </summary>
    private string AliasPath
    {
        get
        {
            return mAliasPath ?? (mAliasPath = DocumentContext.CurrentPageInfo.NodeAliasPath);
        }
    }


    /// <summary>
    /// Post culture.
    /// </summary>
    private string Culture
    {
        get
        {
            return mCulture ?? (mCulture = LocalizationContext.PreferredCultureCode);
        }
    }


    /// <summary>
    /// Post SiteName.
    /// </summary>
    private string SiteName
    {
        get
        {
            return mSiteName ?? (mSiteName = SiteContext.CurrentSiteName);
        }
    }


    /// <summary>
    /// Board document node.
    /// </summary>
    private TreeNode BoardNode
    {
        get
        {
            if (mBoardNode == null)
            {
                SetContext();

                // Get the document
                TreeProvider tree = new TreeProvider();
                mBoardNode = tree.SelectSingleNode(SiteName, AliasPath, Culture, false);
                if ((mBoardNode != null) && (PortalContext.ViewMode != ViewModeEnum.LiveSite))
                {
                    mBoardNode = DocumentHelper.GetDocument(mBoardNode, tree);
                }

                ReleaseContext();
            }
            return mBoardNode;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions for board
        if (BoardProperties.CheckPermissions)
        {
            if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(BoardNode, NodePermissionsEnum.Read) != AuthorizationResultEnum.Allowed)
            {
                Visible = false;
            }
        }

        // Hide the refresh button for JS
        btnRefresh.Style.Add("display", "none");

        // Initializes the control elements
        SetupControl();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        string postBackRefference = ControlsHelper.GetPostBackEventReference(btnRefresh);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshBoardList", ScriptHelper.GetScript("function RefreshBoardList(filterParams){" +
                                                                                                                postBackRefference + "}"));
    }


    #region "Event handlers"

    private void rptBoardMessages_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.DataItem != null)
        {
            if (e.Item.Controls.Count > 0)
            {
                // Load 'MessageActions.ascx' control
                BoardMessageActions boardMsgActions = (BoardMessageActions)e.Item.Controls[0].FindControl("messageActions");

                Control pnlRating = e.Item.Controls[0].FindControl("pnlRating");

                if ((boardMsgActions != null) || (pnlRating != null))
                {
                    DataRow drvRow = ((DataRowView)e.Item.DataItem).Row;

                    // Create new comment info object
                    BoardMessageInfo bmi = new BoardMessageInfo(drvRow);

                    if (boardMsgActions != null)
                    {
                        // Initialize control                    
                        boardMsgActions.MessageID = bmi.MessageID;
                        boardMsgActions.MessageBoardID = MessageBoardID;

                        // Register for OnAction event
                        boardMsgActions.OnMessageAction += boardMsgActions_OnMessageAction;

                        // Handle buttons displaying
                        boardMsgActions.ShowApprove = ((BoardProperties.ShowApproveButton) && (!bmi.MessageApproved) && userVerified);
                        boardMsgActions.ShowReject = ((BoardProperties.ShowRejectButton) && (bmi.MessageApproved) && userVerified);
                        boardMsgActions.ShowDelete = ((BoardProperties.ShowDeleteButton) && userVerified);
                        boardMsgActions.ShowEdit = ((BoardProperties.ShowEditButton) && userVerified);
                    }

                    // Init content rating control if enabled and rating is greater than zero
                    if (pnlRating != null)
                    {
                        if ((bmi.MessageRatingValue > 0) && (BoardProperties.EnableContentRating))
                        {
                            pnlRating.Visible = true;
                            if (DocumentContext.CurrentDocument != null)
                            {
                                AbstractRatingControl usrControl;
                                try
                                {
                                    // Insert rating control to page
                                    usrControl = (AbstractRatingControl)(Page.LoadUserControl(AbstractRatingControl.GetRatingControlUrl(BoardProperties.RatingType + ".ascx")));
                                }
                                catch (Exception ex)
                                {
                                    Controls.Add(new LiteralControl(ex.Message));
                                    return;
                                }

                                // Init values
                                usrControl.ID = "messageRating";
                                usrControl.MaxRating = BoardProperties.MaxRatingValue;
                                usrControl.CurrentRating = bmi.MessageRatingValue;
                                usrControl.Visible = true;
                                usrControl.Enabled = false;

                                pnlRating.Controls.Clear();
                                pnlRating.Controls.Add(usrControl);
                            }
                        }
                        else
                        {
                            pnlRating.Visible = false;
                        }
                    }
                }
            }
        }
    }


    private void boardMsgActions_OnMessageAction(string actionName, object argument)
    {
        // Get current board message ID
        int boardMessageId = ValidationHelper.GetInteger(argument, 0);
        BoardMessageInfo message = BoardMessageInfoProvider.GetBoardMessageInfo(boardMessageId);

        // Handle not existing message
        if (message == null)
        {
            return;
        }

        if ((bi != null) && BoardInfoProvider.IsUserAuthorizedToManageMessages(bi))
        {
            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    // Delete message
                    BoardMessageInfoProvider.DeleteBoardMessageInfo(message);

                    rptBoardMessages.ClearCache();
                    ReloadData();
                    break;

                case "approve":
                    // Approve board message
                    if (MembershipContext.AuthenticatedUser != null)
                    {
                        message.MessageApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
                        message.MessageApproved = true;
                        BoardMessageInfoProvider.SetBoardMessageInfo(message);
                    }

                    rptBoardMessages.ClearCache();
                    ReloadData();
                    break;

                case "reject":
                    // Reject board message
                    if (MembershipContext.AuthenticatedUser != null)
                    {
                        message.MessageApprovedByUserID = 0;
                        message.MessageApproved = false;
                        BoardMessageInfoProvider.SetBoardMessageInfo(message);
                    }

                    rptBoardMessages.ClearCache();
                    ReloadData();
                    break;
            }
        }
    }


    private void msgEdit_OnAfterMessageSaved(BoardMessageInfo message)
    {
        if ((bi == null) && (message != null) && (message.MessageBoardID > 0))
        {
            MessageBoardID = message.MessageBoardID;

            // Get updated board information
            bi = BoardInfoProvider.GetBoardInfo(message.MessageBoardID);

            userVerified = BoardInfoProvider.IsUserAuthorizedToManageMessages(bi);
        }

        rptBoardMessages.ClearCache();

        ReloadData();
    }


    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        rptBoardMessages.ClearCache();
        ReloadData();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the control elements.
    /// </summary>
    private void SetupControl()
    {
        // If the control shouldn't proceed further
        if (BoardProperties.StopProcessing)
        {
            Visible = false;
            return;
        }

        btnLeaveMessage.Attributes.Add("onclick", "ShowSubscription(0, '" + hdnSelSubsTab.ClientID + "','" + pnlMsgEdit.ClientID + "','" +
                                                  pnlMsgSubscription.ClientID + "'); return false; ");
        btnSubscribe.Attributes.Add("onclick", " ShowSubscription(1, '" + hdnSelSubsTab.ClientID + "','" + pnlMsgEdit.ClientID + "','" +
                                               pnlMsgSubscription.ClientID + "'); return false; ");

        // Show/hide appropriate control based on current selection form hidden field
        if (ValidationHelper.GetInteger(hdnSelSubsTab.Value, 0) == 0)
        {
            pnlMsgEdit.Style.Remove("display");
            pnlMsgEdit.Style.Add("display", "block");
            pnlMsgSubscription.Style.Remove("display");
            pnlMsgSubscription.Style.Add("display", "none");
        }
        else
        {
            pnlMsgSubscription.Style.Remove("display");
            pnlMsgSubscription.Style.Add("display", "block");
            pnlMsgEdit.Style.Remove("display");
            pnlMsgEdit.Style.Add("display", "none");
        }

        // Set the repeater
        rptBoardMessages.QueryName = "board.message.selectall";
        rptBoardMessages.ZeroRowsText = HTMLHelper.HTMLEncode(NoMessagesText) + "<br /><br />";
        rptBoardMessages.ItemDataBound += rptBoardMessages_ItemDataBound;
        rptBoardMessages.TransformationName = MessageTransformation;

        // Set the labels
        msgEdit.ResourcePrefix = FormResourcePrefix;
        lblLeaveMessage.ResourceString = "board.messageboard.leavemessage";
        lblNewSubscription.ResourceString = "board.newsubscription";
        btnSubscribe.Text = GetString("board.messageboard.subscribe");
        btnLeaveMessage.Text = GetString("board.messageboard.leavemessage");

        // Pass the properties down to the message edit control
        msgEdit.BoardProperties = BoardProperties;
        msgEdit.MessageBoardID = MessageBoardID;
        msgEdit.OnAfterMessageSaved += msgEdit_OnAfterMessageSaved;
        msgSubscription.BoardProperties = BoardProperties;
        plcBtnSubscribe.Visible = BoardProperties.BoardEnableSubscriptions;
        pnlMsgSubscription.Visible = BoardProperties.BoardEnableSubscriptions;

        // If the message board exist and is enabled
        bi = BoardInfoProvider.GetBoardInfo(MessageBoardID);
        if (bi != null)
        {
            // Get basic info on users permissions            
            userVerified = BoardInfoProvider.IsUserAuthorizedToManageMessages(bi);

            if (bi.BoardEnabled)
            {
                // If the board is moderated remember it
                if (bi.BoardModerated)
                {
                    BoardProperties.BoardModerated = true;
                }

                // Reload messages
                ReloadBoardMessages();

                // If the message board is opened users can add the messages
                bool displayAddMessageForm = BoardInfoProvider.IsUserAuthorizedToAddMessages(bi);

                // Hide 'add message' form when anonymous read disabled and user is not authenticated
                displayAddMessageForm &= (BoardProperties.BoardEnableAnonymousRead || AuthenticationHelper.IsAuthenticated());

                if (displayAddMessageForm)
                {
                    // Display the 'add the message' control
                    DisplayAddMessageForm();
                }
                else
                {
                    HideAddMessageForm();
                }

                msgSubscription.BoardID = bi.BoardID;
            }
            else
            {
                // Hide the control
                Visible = false;
            }
        }
        else
        {
            // The repeater is not rendered, but the NoMessageText has to be visible.
            HideMessages();
            zeroRowsText.Visible = true;
            zeroRowsText.Text = rptBoardMessages.ZeroRowsText;

            // Decide whether the 'Leave message' dialog should be displayed
            if (BoardInfoProvider.IsUserAuthorizedToAddMessages(BoardProperties))
            {
                DisplayAddMessageForm();
            }
            else
            {
                // Hide the dialog, but set message board ID in case that board closed just while entering
                HideAddMessageForm();
            }
        }
    }


    /// <summary>
    /// Hides the messages.
    /// </summary>
    private void HideMessages()
    {
        rptBoardMessages.StopProcessing = true;
        rptBoardMessages.Visible = false;
    }


    /// <summary>
    /// Hides the form used to add a message.
    /// </summary>
    private void HideAddMessageForm()
    {
        pnlMsgEdit.Visible = false;
        msgEdit.StopProcessing = true;
    }


    /// <summary>
    /// Displays form dialog used to leave a new message.
    /// </summary>
    private void DisplayAddMessageForm()
    {
        msgEdit.MessageID = 0; 
        pnlMsgEdit.Visible = true;
        msgEdit.StopProcessing = false;
    }


    /// <summary>
    /// Reloads the board messages related to the currently processed message board.
    /// </summary>
    private void ReloadBoardMessages()
    {
        SetContext();

        // If user isn't allowed to read comments
        if (!(AuthenticationHelper.IsAuthenticated() || BoardProperties.BoardEnableAnonymousRead))
        {
            // Do not display existing messages to anonymous user, but inform on situation
            lblNoMessages.Visible = true;
            lblNoMessages.Text = GetString("board.messagelist.anonymousreadnotallowed");
            HideMessages();
        }
        else
        {
            // If the message board ID was specified
            if (MessageBoardID > 0)
            {
                string where = "(MessageBoardID = " + MessageBoardID.ToString() + ")";

                // If the user should be displayed with all messages not just approved ones
                if (!BoardInfoProvider.IsUserAuthorizedToManageMessages(bi))
                {
                    where += " AND (MessageApproved = 1) AND ((MessageIsSpam IS NULL) OR (MessageIsSpam = 0))";
                }

                // Get board messages
                zeroRowsText.Visible = false;
                rptBoardMessages.StopProcessing = false;
                rptBoardMessages.Visible = true;
                rptBoardMessages.WhereCondition = where;
                rptBoardMessages.ReloadData(true);
            }
        }

        // Update update panel if needed
        if (ControlsHelper.IsInUpdatePanel(this))
        {
            ControlsHelper.UpdateCurrentPanel(this);
        }

        ReleaseContext();
    }


    /// <summary>
    /// Reloads the data on the page.
    /// </summary>
    protected void ReloadData()
    {
        // Reload whole page
        if (ReloadPageAfterAction)
        {
            URLHelper.Redirect(RequestContext.CurrentURL);
        }
        // Reload comment list only
        else
        {
            ReloadBoardMessages();
        }
    }

    #endregion
}
