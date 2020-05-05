using System;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement("CMS.Polls", "Polls")]
public partial class CMSModules_Polls_Tools_Polls_List : CMSPollsPage
{
    private bool globAndSite = false;


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        InitMaster();

        // Set poll list control
        PollsList.OnEdit += new EventHandler(PollsList_OnEdit);
        PollsList.WhereCondition = fltSite.GetWhereCondition();
        PollsList.IsLiveSite = false;
        PollsList.DeleteEnabled = CheckPollsModifyPermission(SiteContext.CurrentSiteID, false);
        PollsList.DeleteGlobalEnabled = CheckPollsModifyPermission(UniSelector.US_GLOBAL_RECORD, false);

        // Disables creating of new poll when "global and sites objects" is selected
        globAndSite = (fltSite.SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD);
        PollsList.DisplayGlobalColumn = globAndSite;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Disables creating of new poll when "global and sites objects" is selected
        hdrActions.Enabled = !globAndSite && CheckPollsModifyPermission(fltSite.SiteID, false);
        lblWarnNew.Visible = globAndSite;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Edit poll click handler.
    /// </summary>
    private void PollsList_OnEdit(object sender, EventArgs e)
    {
        // Propagate selected item from ddlist to breadcrumbs on edit page
        string editActionUrl = URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("CMS.Polls", "EditPoll"), "objectid", PollsList.SelectedItemID.ToString()), "siteid", fltSite.SiteID.ToString()), "displaytitle", "false");
        URLHelper.Redirect(editActionUrl);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes master page and header actions.
    /// </summary>
    private void InitMaster()
    {
        // Init filter for the first time according to user permissions
        if (!RequestHelper.IsPostBack())
        {
            if (AuthorizedForGlobalPolls && AuthorizedForSitePolls)
            {
                fltSite.SiteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
            }
            else if (AuthorizedForSitePolls)
            {
                // User is authorized for site polls => select site polls
                fltSite.SiteID = SiteContext.CurrentSiteID;
            }
            else
            {
                // User is authorized for global polls => select global polls only
                fltSite.SiteID = UniSelector.US_GLOBAL_RECORD;
            }
        }

        HeaderAction action = new HeaderAction();
        action.Text = GetString("Polls_List.NewItemCaption");
        action.RedirectUrl = ResolveUrl("Polls_New.aspx?siteid=" + fltSite.SiteID);
        hdrActions.AddAction(action);

        // Set the page title
        PageTitle.TitleText = GetString("Polls_List.HeaderCaption");
        CurrentMaster.DisplaySiteSelectorPanel = AuthorizedForSitePolls && AuthorizedForGlobalPolls;
    }

    #endregion
}