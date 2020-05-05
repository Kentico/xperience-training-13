using System;

using CMS.Helpers;
using CMS.MessageBoards;
using CMS.MessageBoards.Web.UI;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_Tools_Boards_Board_Edit_Subscription_Edit : CMSMessageBoardBoardsPage
{
    private int mSubscriptionId;
    private int mBoardId;

    private bool changeMaster;

    private BoardSubscriptionInfo mCurrentSubscription;


    protected override void OnPreInit(EventArgs e)
    {
        // External call
        changeMaster = QueryHelper.GetBoolean("changemaster", false);

        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize the controls
        SetupControl();
    }


    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
        // Get current subscription ID
        mSubscriptionId = QueryHelper.GetInteger("subscriptionid", 0);
        if (mSubscriptionId > 0)
        {
            mCurrentSubscription = BoardSubscriptionInfoProvider.GetBoardSubscriptionInfo(mSubscriptionId);
            EditedObject = mCurrentSubscription;
        }

        // Get current board ID
        mBoardId = QueryHelper.GetInteger("boardid", 0);

        boardSubscription.IsLiveSite = false;
        boardSubscription.BoardID = mBoardId;
        boardSubscription.SubscriptionID = mSubscriptionId;
        boardSubscription.OnSaved += boardSubscription_OnSaved;

        InitializeBreadcrumbs();
    }


    protected void boardSubscription_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("~/CMSModules/MessageBoards/Tools/Boards/Board_Edit_Subscription_Edit.aspx?subscriptionid=" + boardSubscription.SubscriptionID + "&boardid=" + mBoardId + "&saved=1" + "&changemaster=" + changeMaster);
    }


    /// <summary>
    /// Initializes the breadcrumbs on the page.
    /// </summary>
    private void InitializeBreadcrumbs()
    {
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("board.subscription.subscriptions"),
            RedirectUrl = "~/CMSModules/MessageBoards/Tools/Boards/Board_Edit_Subscriptions.aspx?boardid=" + mBoardId + "&changemaster=" + changeMaster,
            Target = "_self"
        });

        // Display current subscription e-mail
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = (mCurrentSubscription != null) ? mCurrentSubscription.SubscriptionEmail : GetString("board.subscriptions.newitem")
        });
    }
}