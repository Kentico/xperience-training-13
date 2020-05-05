using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_LiveControls_Subscription : CMSAdminItemsControl
{
    #region "Variables"

    private int mForumId = 0;
    private bool displayControlPerformed = false;
    private bool listVisible = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the Forum ID.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
        }

        listVisible = plcList.Visible;
        plcList.Visible = true;
        subscriptionList.Visible = true;


        #region "Security"

        subscriptionList.OnCheckPermissions += subscriptionList_OnCheckPermissions;
        subscriptionEdit.OnCheckPermissions += subscriptionEdit_OnCheckPermissions;
        subscriptionNew.OnCheckPermissions += subscriptionNew_OnCheckPermissions;

        #endregion


        subscriptionNew.ForumID = mForumId;
        subscriptionEdit.ForumID = mForumId;
        subscriptionList.ForumID = mForumId;

        subscriptionList.OnAction += subscriptionList_OnAction;

        int subscriptionId = ValidationHelper.GetInteger(ViewState["SubscriptionID"], 0);
        subscriptionEdit.SubscriptionID = subscriptionId;

        HeaderAction action = new HeaderAction();
        action.Text = GetString("ForumSubscription_List.NewItemCaption");
        action.CommandName = "newsubscription";
        actionsElem.AddAction(action);
        actionsElem.ActionPerformed += actionsElem_ActionPerformed;
        subscriptionNew.OnSaved += subscriptionNew_OnSaved;
        subscriptionEdit.OnSaved += subscriptionEdit_OnSaved;

        InitializeBreadcrumbs(subscriptionId);

        // Default show listing
        if (!RequestHelper.IsPostBack())
        {
            DisplayControl("list");
        }
    }

    
    private void subscriptionEdit_OnSaved(object sender, EventArgs e)
    {
        int subscriptionId = ValidationHelper.GetInteger(ViewState["SubscriptionID"], 0);
        if (subscriptionId > 0)
        {
            ForumSubscriptionInfo fsi = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
            if (fsi != null)
            {
                SetBreadcrumbsItem(fsi.SubscriptionEmail);
            }
        }
    }


    #region "Security handlers"

    private void subscriptionList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void subscriptionNew_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }


    private void subscriptionEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        RaiseOnCheckPermissions(permissionType, sender);
    }

    #endregion


    protected void subscriptionNew_OnSaved(object sender, EventArgs e)
    {
        int subscriptionId = subscriptionNew.SubscriptionID;
        ViewState["SubscriptionID"] = subscriptionId;

        ForumSubscriptionInfo fsi = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
        if (fsi != null)
        {
            SetBreadcrumbsItem(fsi.SubscriptionEmail);
        }

        subscriptionEdit.SubscriptionID = subscriptionId;

        DisplayControl("edit");
    }


    /// <summary>
    /// New subscription link handler.
    /// </summary>
    protected void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "newsubscription":
                DisplayControl("new");
                SetBreadcrumbsItem(GetString("forum_list.subscription.newsubscription"));
                break;
        }
    }


    /// <summary>
    /// Edit subscription handler.
    /// </summary>
    protected void subscriptionList_OnAction(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "edit":

                int subscriptionId = ValidationHelper.GetInteger(e.CommandArgument, 0);
                subscriptionEdit.SubscriptionID = subscriptionId;
                ViewState["SubscriptionID"] = subscriptionId;

                ForumSubscriptionInfo fsi = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
                if (fsi != null)
                {
                    SetBreadcrumbsItem(fsi.SubscriptionEmail);
                }

                DisplayControl("edit");

                break;
        }
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        subscriptionNew.ForumID = mForumId;
        subscriptionEdit.ForumID = mForumId;
        subscriptionList.ForumID = mForumId;
        DisplayControl("list");
    }


    /// <summary>
    /// Initializes the breadcrumbs.
    /// </summary>
    private void InitializeBreadcrumbs(int subscriptionId)
    {
        lnkBackHidden.Click += lnkBackHidden_Click;

        ucBreadcrumbs.Items.Clear();

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = GetString("forum_list.subscription.headeractions"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(lnkBackHidden) + "; return false;"
        });

        ucBreadcrumbs.AddBreadcrumb(new BreadcrumbItem {
            Text = (subscriptionId <= 0) ? GetString("forum_list.subscription.newsubscription") : "",
        });
    }


    protected void lnkBackHidden_Click(object sender, EventArgs e)
    {
        DisplayControl("list");
    }


    private void DisplayControl(string selectedControl)
    {
        // Hide all controls
        plcList.Visible = false;
        plcEdit.Visible = false;
        plcNew.Visible = false;

        displayControlPerformed = true;

        switch (selectedControl.ToLowerCSafe())
        {
            // Show edit control
            case "edit":
                plcEdit.Visible = true;
                subscriptionEdit.Visible = true;
                subscriptionEdit.ReloadData();
                plcBreadcrumbs.Visible = true;
                break;

            // Show new control
            case "new":
                plcNew.Visible = true;
                subscriptionNew.Visible = true;
                subscriptionNew.ReloadData();
                plcBreadcrumbs.Visible = true;
                break;

            // Show list control
            default:
                plcList.Visible = true;
                subscriptionList.Visible = true;
                subscriptionList.ReloadData();
                plcBreadcrumbs.Visible = false;
                break;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!displayControlPerformed)
        {
            plcList.Visible = listVisible;
            subscriptionList.Visible = listVisible;
            if (listVisible)
            {
                subscriptionList.ReloadData();
            }
        }

        // Ensure breadcrumbs items
        int subscriptionId = ValidationHelper.GetInteger(ViewState["SubscriptionID"], 0);
        if ((subscriptionId > 0) && String.IsNullOrEmpty(ucBreadcrumbs.Items[1].Text))
        {
            ForumSubscriptionInfo fsi = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
            if (fsi != null)
            {
                SetBreadcrumbsItem(fsi.SubscriptionEmail);
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Set breadcrumbs item representing current subscription text.
    /// </summary>
    /// <param name="itemText">Item text</param>
    private void SetBreadcrumbsItem(string itemText)
    {
        ucBreadcrumbs.Items[1].Text = itemText;
    }
}