using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Activities;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.MessageBoards;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_Boards_BoardUserSubscriptions : CMSAdminControl
{
    private int mSiteId = 0;
    private int mUserId = 0;
    private string mSiteName;


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
    /// Site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// User ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || !Visible)
        {
            EnableViewState = false;
            boardSubscriptions.StopProcessing = true;
            return;
        }

        // If control should be hidden save view state memory
        if (!Visible)
        {
            EnableViewState = false;
        }

        // Initialize controls
        SetupControls();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (DataHelper.DataSourceIsEmpty(boardSubscriptions.GridView.DataSource))
        {
            lblMessage.Visible = false;
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes controls on the page.
    /// </summary>
    private void SetupControls()
    {
        if (UserID > 0)
        {
            boardSubscriptions.Visible = true;

            if (MembershipContext.AuthenticatedUser.UserID == UserID)
            {
                boardSubscriptions.ZeroRowsText = GetString("boardsubscripitons.userhasnosubscriptions");
            }
            else
            {
                boardSubscriptions.ZeroRowsText = GetString("boardsubscripitons.NoDataUser");
            }

            // Setup UniGrid control     
            boardSubscriptions.IsLiveSite = IsLiveSite;
            boardSubscriptions.Pager.DefaultPageSize = 10;
            boardSubscriptions.OnAction += boardSubscriptions_OnAction;
            boardSubscriptions.OnExternalDataBound += boardSubscriptions_OnExternalDataBound;
            boardSubscriptions.OnDataReload += boardSubscriptions_OnDataReload;
            boardSubscriptions.ShowActionsMenu = true;

            BoardSubscriptionInfo bsi = new BoardSubscriptionInfo();
            boardSubscriptions.AllColumns = SqlHelper.AddColumns(SqlHelper.JoinColumnList(bsi.ColumnNames), "BoardID, BoardDisplayName, BoardSiteID, NodeAliasPath, DocumentCulture");

            mSiteName = SiteInfoProvider.GetSiteName(SiteID);
        }
        else
        {
            boardSubscriptions.Visible = false;
        }
    }

    #endregion


    #region "UniGrid events handling"

    protected DataSet boardSubscriptions_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        DataSet ds = BoardSubscriptionInfoProvider.GetSubscriptions(UserID, SiteID, currentTopN);
        totalRecords = DataHelper.GetItemsCount(ds);
        return ds;
    }


    /// <summary>
    /// On action event handling.
    /// </summary>
    /// <param name="actionName">Name of the action.</param>
    /// <param name="actionArgument">Parameter for the action.</param>
    protected void boardSubscriptions_OnAction(string actionName, object actionArgument)
    {
        int boardSubscriptionId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                if (RaiseOnCheckPermissions(PERMISSION_MANAGE, this))
                {
                    if (StopProcessing)
                    {
                        return;
                    }
                }

                try
                {
                    BoardSubscriptionInfoProvider.DeleteBoardSubscriptionInfo(boardSubscriptionId);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }

                break;

            case "approve":
                if (RaiseOnCheckPermissions(PERMISSION_MANAGE, this))
                {
                    if (StopProcessing)
                    {
                        return;
                    }
                }

                // Approve BoardSubscriptionInfo object
                BoardSubscriptionInfo bsi = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(boardSubscriptionId);
                if ((bsi != null) && !bsi.SubscriptionApproved)
                {
                    bsi.SubscriptionApproved = true;
                    BoardSubscriptionInfoProvider.SetBoardSubscriptionInfo(bsi);

                    // Send confirmation mail
                    BoardInfo bi = BoardInfoProvider.GetBoardInfo(bsi.SubscriptionBoardID);
                    if ((bi != null) && bi.BoardSendOptInConfirmation)
                    {
                        BoardSubscriptionInfoProvider.SendConfirmationEmail(bsi, true);
                    }

                    // Log activity
                    if (MembershipContext.AuthenticatedUser.UserID == UserID)
                    {
                        Service.Resolve<ICurrentContactMergeService>().UpdateCurrentContactEmail(bsi.SubscriptionEmail, MembershipContext.AuthenticatedUser);
                    }
                }
                break;

        }
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    private object boardSubscriptions_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "displayname":
                DataRowView dr = (DataRowView)parameter;
                string url = "";
                string lang = ValidationHelper.GetString(dr["DocumentCulture"], "");
                if (!String.IsNullOrEmpty(lang))
                {
                    url += "?" + URLHelper.LanguageParameterName + "=" + lang;
                }

                return "<a target=\"_blank\" href=\"" + url + "\">" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["BoardDisplayName"], "")) + "</a>";

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

        return parameter;
    }


    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        if (propertyName != null)
        {
            switch (propertyName.ToLowerCSafe())
            {
                case "siteid":
                    SiteID = ValidationHelper.GetInteger(value, 0);
                    break;
                case "userid":
                    UserID = ValidationHelper.GetInteger(value, 0);
                    break;
                case "islivesite":
                    IsLiveSite = ValidationHelper.GetBoolean(value, true);
                    break;
            }
        }

        return true;
    }

    #endregion
}