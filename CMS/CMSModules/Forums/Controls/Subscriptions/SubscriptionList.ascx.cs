using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Subscriptions_SubscriptionList : CMSAdminListControl
{
    #region "Variables"

    private int mForumId;
    private bool process = true;
    private bool reloadData = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the ID of the forum to edit.
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
        process = true;
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
            process = false;
        }

        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnBeforeSorting += new OnBeforeSorting(gridElem_OnBeforeSorting);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("general.nodatafound");
        SetupUniGrid();
    }


    /// <summary>
    /// Reloads the grid data.
    /// </summary>
    public override void ReloadData()
    {
        reloadData = true;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int subscriptionId = ValidationHelper.GetInteger(actionArgument, 0);

        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
                {
                    return;
                }

                // Delete ForumSubscriptionInfo object from database
                ForumSubscriptionInfoProvider.DeleteForumSubscriptionInfo(subscriptionId, chkSendConfirmationEmail.Checked);
                break;

            case "approve":
                if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
                {
                    return;
                }
                // Approve ForumSubscriptionInfo object
                ForumSubscriptionInfo fsi = ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(subscriptionId);
                if ((fsi != null) && !fsi.SubscriptionApproved)
                {
                    fsi.SubscriptionApproved = true;
                    ForumSubscriptionInfoProvider.SetForumSubscriptionInfo(fsi);
                    if (chkSendConfirmationEmail.Checked)
                    {
                        ForumSubscriptionInfoProvider.SendConfirmationEmail(fsi, true, null, null);
                    }
                }
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    protected void gridElem_OnBeforeSorting(object sender, EventArgs e)
    {
        SetupUniGrid();
        gridElem.ReloadData();
    }


    /// <summary>
    /// Pre render.
    /// </summary>    
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if ((!IsLiveSite && !RequestHelper.IsPostBack() && process) || reloadData)
        {
            if (ForumID > 0)
            {
                gridElem.ReloadData();
            }

            reloadData = false;
        }

        // If data grid
        if (gridElem.GridView.Rows.Count <= 0)
        {
            pnlSendConfirmationEmail.Visible = false;
        }
    }


    private void SetupUniGrid()
    {
        if (ForumID > 0)
        {
            gridElem.ObjectType = "forums.forumsubscriptionwithpost";
        }

        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ForumID", ForumID);

        gridElem.QueryParameters = parameters;
        gridElem.OrderBy = "SubscriptionEmail";
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
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
}