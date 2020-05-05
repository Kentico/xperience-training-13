using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Boards_BoardSubscriptions : CMSAdminListControl
{
    // Current board ID
    private int mBoardId = 0;
    private BoardInfo mBoard = null;
    private int mGroupId = 0;
    private bool mChangeMaster = false;


    #region "Public properties"

    /// <summary>
    /// ID of the current message board.
    /// </summary>
    public int BoardID
    {
        get
        {
            if (mBoard != null)
            {
                return mBoard.BoardID;
            }

            return mBoardId;
        }
        set
        {
            mBoardId = value;

            mBoard = null;
        }
    }


    /// <summary>
    /// Current message board info object.
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

            mBoardId = 0;
        }
    }


    /// <summary>
    /// ID of the current group.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// Gets or sets to change master according to tab level.
    /// </summary>
    public bool ChangeMaster
    {
        get
        {
            return mChangeMaster;
        }
        set
        {
            mChangeMaster = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize controls
        SetupControl();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        // If data grid
        if (boardSubscriptions.GridView.Rows.Count <= 0)
        {
            pnlSendConfirmationEmail.Visible = false;
        }
    }


    /// <summary>
    /// Reloads the control content.
    /// </summary>
    public override void ReloadData()
    {
        boardSubscriptions.ReloadData();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes controls on the page.
    /// </summary>
    private void SetupControl()
    {
        boardSubscriptions.OnAction += new OnActionEventHandler(boardSubscriptions_OnAction);
        boardSubscriptions.OnExternalDataBound += new OnExternalDataBoundEventHandler(boardSubscriptions_OnExternalDataBound);
        boardSubscriptions.IsLiveSite = IsLiveSite;
        boardSubscriptions.ZeroRowsText = GetString("general.nodatafound");

        // Get current board ID
        if (BoardID > 0)
        {
            boardSubscriptions.WhereCondition = "SubscriptionBoardID = " + BoardID;
            boardSubscriptions.Visible = true;
        }
        else
        {
            boardSubscriptions.Visible = false;
        }
    }

    #endregion


    #region "UniGrid events handling"

    protected object boardSubscriptions_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // If parameter for binding supplied
        switch (sourceName.ToLowerCSafe())
        {
            case "formattedusername":
                string userName = ValidationHelper.GetString(parameter, null);
                if (userName == null)
                {
                    return "-";
                }
                else
                {
                    return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, IsLiveSite));
                }

            case "approved":
                return UniGridFunctions.ColoredSpanYesNo(parameter, true);

            case "approve":
                CMSGridActionButton button = ((CMSGridActionButton)sender);
                if (button != null)
                {
                    bool isApproved = ValidationHelper.GetBoolean(((DataRowView)((GridViewRow)parameter).DataItem).Row["SubscriptionApproved"], true);

                    if (isApproved)
                    {
                        button.Visible = false;
                    }
                }
                break;
        }

        return HTMLHelper.HTMLEncode(Convert.ToString(parameter));
    }


    protected void boardSubscriptions_OnAction(string actionName, object actionArgument)
    {
        BoardSubscriptionInfo bsi = null;

        // Get currently processed subscription ID
        int subscriptionId = ValidationHelper.GetInteger(actionArgument, 0);
        if (subscriptionId > 0)
        {
            switch (actionName.ToLowerCSafe())
            {
                case "delete":
                    if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
                    {
                        return;
                    }

                    // Get subscription according current ID
                    bsi = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(subscriptionId);
                    if (bsi != null)
                    {

                        if (chkSendConfirmationEmail.Checked && bsi.SubscriptionApproved)
                        {
                            BoardSubscriptionInfoProvider.SendConfirmationEmail(bsi, false);
                        }
                        BoardSubscriptionInfoProvider.DeleteBoardSubscriptionInfo(bsi);
                    }
                    break;

                case "approve":
                    if (!CheckPermissions("cms.messageboards", PERMISSION_MODIFY))
                    {
                        return;
                    }

                    // Approve ForumSubscriptionInfo object
                    bsi = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(subscriptionId);
                    if ((bsi != null) && !bsi.SubscriptionApproved)
                    {
                        bsi.SubscriptionApproved = true;
                        BoardSubscriptionInfoProvider.SetBoardSubscriptionInfo(bsi);

                        if (chkSendConfirmationEmail.Checked)
                        {
                            BoardSubscriptionInfoProvider.SendConfirmationEmail(bsi, true);
                        }
                    }
                    break;

                default:
                    break;
            }

            RaiseOnAction(actionName, actionArgument);
        }
    }

    #endregion
}