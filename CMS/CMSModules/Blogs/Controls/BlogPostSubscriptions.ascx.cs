using System;
using System.Data;

using CMS.Base;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.Blogs;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Blogs_Controls_BlogPostSubscriptions : CMSAdminControl
{
    #region "Variables"

    private int mUserId = 0;
    private int mSiteId = 0;
    private int mDisplayNameLength = 50;
    private string mSiteName;

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
    /// Maximum length of the displayname (whole display name is displayed in tooltip).
    /// </summary>
    public int DisplayNameLength
    {
        get
        {
            return mDisplayNameLength;
        }
        set
        {
            mDisplayNameLength = value;
        }
    }


    /// <summary>
    /// Gets or sets user ID.
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


    /// <summary>
    /// Gets or sets site ID.
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
    /// If true, control does not process the data.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["StopProcessing"], false);
        }
        set
        {
            ViewState["StopProcessing"] = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControl();
    }


    protected void SetupControl()
    {
        // In design mode is processing of control stopped
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (MembershipContext.AuthenticatedUser.UserID == UserID)
            {
                gridElem.ZeroRowsText = GetString("blogsubscripitons.userhasnosubscriptions");
            }
            else
            {
                gridElem.ZeroRowsText = GetString("blogsubscripitons.NoDataUser");
            }
            gridElem.IsLiveSite = IsLiveSite;
            gridElem.OnAction += gridElem_OnAction;
            gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
            gridElem.OnDataReload += gridElem_OnDataReload;
            gridElem.ShowActionsMenu = true;
            gridElem.Columns = "SubscriptionID, SubscriptionEmail, DocumentName, NodeAliasPath, DocumentCulture, SubscriptionApproved";

            // Get all possible columns to retrieve
            gridElem.AllColumns = SqlHelper.JoinColumnList(ObjectTypeManager.GetColumnNames(BlogPostSubscriptionInfo.OBJECT_TYPE, PredefinedObjectType.NODE, PredefinedObjectType.DOCUMENTLOCALIZATION));

            mSiteName = SiteInfoProvider.GetSiteName(SiteID);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (DataHelper.DataSourceIsEmpty(gridElem.GridView.DataSource))
        {
            lblMessage.Visible = false;
        }
    }


    /// <summary>
    /// Overridden SetValue - because of subscriptions control.
    /// </summary>
    /// <param name="propertyName">Name of the property to set</param>
    /// <param name="value">Value to set</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "userid":
                UserID = ValidationHelper.GetInteger(value, 0);
                break;
            case "siteid":
                SiteID = ValidationHelper.GetInteger(value, 0);
                break;
            case "islivesite":
                IsLiveSite = ValidationHelper.GetBoolean(value, true);
                break;
        }

        return true;
    }


    /// <summary>
    /// Reloads data for unigrid.
    /// </summary>
    public override void ReloadData()
    {
        gridElem.ReloadData();
    }


    #region "UniGrid events"

    protected DataSet gridElem_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        return BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptions(UserID, SiteID, completeWhere, currentTopN, currentOrder, columns, currentOffset, currentPageSize, ref totalRecords);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int subscriptionId = ValidationHelper.GetInteger(actionArgument, 0);

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
                    // Try to delete notification subscription
                    BlogPostSubscriptionInfoProvider.DeleteBlogPostSubscriptionInfo(subscriptionId);
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

                // Approve BlogPostSubscriptionInfo object
                BlogPostSubscriptionInfo bsi = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo(subscriptionId);
                if ((bsi != null) && !bsi.SubscriptionApproved)
                {
                    bsi.SubscriptionApproved = true;
                    BlogPostSubscriptionInfoProvider.SetBlogPostSubscriptionInfo(bsi);

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
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "documentname":
                DataRowView dr = (DataRowView)parameter;

                if (SiteContext.CurrentSite != null)
                {
                    string url = "";
                    string lang = ValidationHelper.GetString(dr["DocumentCulture"], "");
                    if (!String.IsNullOrEmpty(lang))
                    {
                        url += "?" + URLHelper.LanguageParameterName + "=" + lang;
                    }

                    return "<a target=\"_blank\" href=\"" + url + "\">" + HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["DocumentName"], "")) + "</a>";
                }
                else
                {
                    return HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["DocumentName"], ""));
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

        return parameter;
    }

    #endregion
}