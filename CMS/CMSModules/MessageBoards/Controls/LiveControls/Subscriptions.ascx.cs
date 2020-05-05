using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.MessageBoards;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Controls_LiveControls_Subscriptions : CMSAdminItemsControl
{
    #region "Selected control enumeration"

    private enum SelectedControlEnum
    {
        Listing,
        Edit
    };

    #endregion


    #region "Private fields"

    private int mGroupID = 0;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns currently selected control.
    /// </summary>
    private SelectedControlEnum SelectedControl
    {
        get
        {
            if (plcEditSubscriptions.Visible)
            {
                return SelectedControlEnum.Edit;
            }
            else
            {
                return SelectedControlEnum.Listing;
            }
        }
        set
        {
            switch (value)
            {
                case SelectedControlEnum.Listing:
                    boardSubscriptionList.StopProcessing = false;
                    boardSubscriptionEdit.StopProcessing = true;
                    plcEditSubscriptions.Visible = false;
                    plcSubscriptionList.Visible = true;
                    break;

                case SelectedControlEnum.Edit:
                    boardSubscriptionList.StopProcessing = true;
                    boardSubscriptionEdit.StopProcessing = false;
                    plcEditSubscriptions.Visible = true;
                    plcSubscriptionList.Visible = false;
                    break;
            }
        }
    }


    /// <summary>
    /// Current subscription ID for internal use.
    /// </summary>
    private int SubscriptionID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SubscriptionID"], 0);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Current board ID.
    /// </summary>
    public int BoardID
    {
        get
        {
            return boardSubscriptionEdit.BoardID;
        }
        set
        {
            boardSubscriptionEdit.BoardID = value;
            boardSubscriptionList.BoardID = value;
        }
    }


    /// <summary>
    /// Current group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            if (mGroupID == 0)
            {
                mGroupID = ValidationHelper.GetInteger(GetValue("GroupID"), 0);
            }

            if (mGroupID == 0)
            {
                mGroupID = QueryHelper.GetInteger("groupid", 0);
            }
            return mGroupID;
        }
        set
        {
            mGroupID = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
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
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        boardSubscriptionEdit.OnCheckPermissions += boardSubscriptionEdit_OnCheckPermissions;
        boardSubscriptionList.OnCheckPermissions += boardSubscriptionList_OnCheckPermissions;
        boardSubscriptionList.IsLiveSite = IsLiveSite;
        newSubscription.IsLiveSite = IsLiveSite;
        boardSubscriptionList.OnAction += boardSubscriptionList_OnAction;
        boardSubscriptionEdit.GroupID = GroupID;
        boardSubscriptionEdit.SubscriptionID = SubscriptionID;
        boardSubscriptionEdit.OnSaved += boardSubscriptionEdit_OnSaved;

        lnkBackHidden.Click += lnkBackHidden_Click;

        HeaderAction action = new HeaderAction();
        action.Text = GetString("Group_General.Boards.Boards.NewSubscription");
        action.CommandName = "newsubscription";

        newSubscription.ActionsList.Clear();
        newSubscription.ActionPerformed += newSubscription_ActionPerformed;
        newSubscription.AddAction(action);

        InitBreadCrumbs();
    }


    /// <summary>
    /// Displays specified control.
    /// </summary>
    /// <param name="selectedControl">Control to be displayed</param>
    /// <param name="reload">If True, ReloadData on child control is called</param>
    private void DisplayControl(SelectedControlEnum selectedControl, bool reload)
    {
        // Set correct tab
        SelectedControl = selectedControl;

        // Enable currently selected element
        switch (selectedControl)
        {
            case SelectedControlEnum.Listing:
                if (reload)
                {
                    boardSubscriptionList.BoardID = BoardID;
                    boardSubscriptionList.ReloadData();
                }

                break;

            case SelectedControlEnum.Edit:
                if (reload)
                {
                    boardSubscriptionEdit.GroupID = GroupID;
                    boardSubscriptionEdit.SubscriptionID = SubscriptionID;
                    boardSubscriptionEdit.ReloadData();
                }

                break;
        }
    }


    /// <summary>
    /// Reloads the contol data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControls();

        DisplayControl(SelectedControlEnum.Listing, true);

        newSubscription.ReloadData();
    }


    private void InitBreadCrumbs()
    {
        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("Group_General.Boards.Boards.BackToSubscriptions"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem());

        // Initialize subscription breadcrumbs
        if (SubscriptionID > 0)
        {
            BoardSubscriptionInfo subscription = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(SubscriptionID);
            if (subscription != null)
            {
                ucBreadcrumbs.Items[1].Text = subscription.SubscriptionEmail;
            }
        }
        else
        {
            ucBreadcrumbs.Items[1].Text = GetString("Group_General.Boards.Boards.NewSubscription");
        }
    }


    #region "Event handlers"

    protected void newSubscription_ActionPerformed(object sender, CommandEventArgs e)
    {
        ViewState.Add("SubscriptionID", 0);
        DisplayControl(SelectedControlEnum.Edit, true);
        InitBreadCrumbs();
    }


    protected void boardSubscriptionList_OnAction(object sender, CommandEventArgs e)
    {
        // Check if command is edit
        if ((e.CommandName.ToLowerCSafe() == "edit") && boardSubscriptionList.Visible)
        {
            ViewState.Add("SubscriptionID", e.CommandArgument);
            DisplayControl(SelectedControlEnum.Edit, true);
            InitBreadCrumbs();
        }
    }


    protected void boardSubscriptionEdit_OnSaved(object sender, EventArgs e)
    {
        ViewState.Add("SubscriptionID", boardSubscriptionEdit.SubscriptionID);
        DisplayControl(SelectedControlEnum.Edit, true);
        InitBreadCrumbs();
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        DisplayControl(SelectedControlEnum.Listing, true);
    }

    #endregion


    #region "Security event handlers"

    protected void boardSubscriptionList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }


    protected void boardSubscriptionEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Raise event
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion
}