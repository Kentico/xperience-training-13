using System;
using System.Collections;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("om.activity.details.title")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Pages_Tools_Activities_Activity_Activity_Details : CMSModalPage
{
    #region "Protected variables"

    protected int activityId;
    protected int prevId;
    protected int nextId;
    protected Hashtable mParameters;
    protected bool userIsAuthorized = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash", "activityid") || Parameters == null)
        {
            return;
        }

        // Get activity ID from query string
        activityId = QueryHelper.GetInteger("activityid", 0);

        ActivityInfo activity = ActivityInfo.Provider.Get(activityId);
        if (activity == null)
        {
            RedirectToInformation("editedobject.notexists");
        }

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ACTIVITIES, "ReadActivities", SiteInfoProvider.GetSiteName(activity.ActivitySiteID)))
        {
            RedirectToAccessDenied(ModuleName.ACTIVITIES, "ReadActivities");
        }

        LoadData();

        // Enable text boxes and show save button if user is autorized to change activities
        userIsAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ACTIVITIES, "ManageActivities", SiteInfoProvider.GetSiteName(activity.ActivitySiteID));
        if (userIsAuthorized)
        {
            Save += btnSave_Click;
            txtURL.ReadOnly = false;
            txtTitle.ReadOnly = false;
            txtURLRef.ReadOnly = false;
            txtComment.Enabled = true;
            btnStamp.Enabled = true;
            btnStamp.OnClientClick = "AddStamp('" + txtComment.ClientID + "'); return false;";
        }

        // Disable collapse of toolbar (IE7 bug)
        txtComment.ToolbarCanCollapse = false;

        RegisterScripts();
    }

    #endregion


    #region "Button handling"

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "activityid", prevId.ToString()));
    }


    protected void btnNext_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "activityid", nextId.ToString()));
    }


    private void btnSave_Click(object sender, EventArgs e)
    {
        if (userIsAuthorized && (activityId > 0))
        {
            ActivityInfo activity = ActivityInfo.Provider.Get(activityId);
            EditedObject = activity;
            activity.ActivityComment = txtComment.Value;
            activity.ActivityTitle = TextHelper.LimitLength(txtTitle.Text, 250, String.Empty);
            activity.ActivityURLReferrer = txtURLRef.Text;
            activity.ActivityURL = txtURL.Text;

            // Save activity info
            ActivityInfo.Provider.Set(activity);

            // Reload form (due to "view URL" button)
            LoadData();

            ltlScript.Text = ScriptHelper.GetScript("CloseDialog();");
        }
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Loads data of specific activity.
    /// </summary>
    protected void LoadData()
    {
        if (activityId <= 0)
        {
            return;
        }

        // Load and check if object exists
        ActivityInfo ai = ActivityInfo.Provider.Get(activityId);
        EditedObject = ai;

        ActivityTypeInfo ati = ActivityTypeInfo.Provider.Get(ai.ActivityType);
        plcActivityValue.Visible = (ati == null) || ati.ActivityTypeIsCustom || (ati.ActivityTypeName == PredefinedActivityType.PAGE_VISIT) && !String.IsNullOrEmpty(ai.ActivityValue);

        string dispName = (ati != null ? ati.ActivityTypeDisplayName : GetString("general.na"));

        lblTypeVal.Text = String.Format("{0}", HTMLHelper.HTMLEncode(dispName));
        lblContactVal.Text = HTMLHelper.HTMLEncode(ContactInfoProvider.GetContactFullName(ai.ActivityContactID));

        // Init contact detail link
        string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", ai.ActivityContactID);
        btnContact.Attributes.Add("onClick", ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail"));
        btnContact.ToolTip = GetString("general.edit");

        lblDateVal.Text = (ai.ActivityCreated == DateTimeHelper.ZERO_TIME ? GetString("general.na") : HTMLHelper.HTMLEncode(ai.ActivityCreated.ToString()));

        // Get site display name
        string siteName = SiteInfoProvider.GetSiteName(ai.ActivitySiteID);
        if (String.IsNullOrEmpty(siteName))
        {
            siteName = GetString("general.na");
        }
        else
        {
            // Retrieve site info and its display name
            SiteInfo si = SiteInfo.Provider.Get(siteName);
            if (si != null)
            {
                siteName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(si.DisplayName));
            }
            else
            {
                siteName = GetString("general.na");
            }
        }
        lblSiteVal.Text = siteName;

        string url = ai.ActivityURL;
        plcCampaign.Visible = !String.IsNullOrEmpty(ai.ActivityCampaign);
        lblCampaignVal.Text = HTMLHelper.HTMLEncode(ai.ActivityCampaign);
        lblValue.Text = HTMLHelper.HTMLEncode(String.IsNullOrEmpty(ai.ActivityValue) ? GetString("general.na") : ai.ActivityValue);

        // Init textboxes only for the first time
        if (!RequestHelper.IsPostBack())
        {
            txtComment.Value = ai.ActivityComment;
            txtTitle.Text = ai.ActivityTitle;
            txtURLRef.Text = ai.ActivityURLReferrer;
            if (ai.ActivityType != PredefinedActivityType.NEWSLETTER_CLICKTHROUGH)
            {
                txtURL.Text = url;
            }
        }

        cDetails.ActivityID = activityId;

        // Init link button URL
        if (ai.ActivitySiteID > 0)
        {
            SiteInfo si = SiteInfo.Provider.Get(ai.ActivitySiteID);
            if (si != null)
            {
                // Hide view button if URL is blank
                string activityUrl = ai.ActivityURL;
                if ((activityUrl != null) && !String.IsNullOrEmpty(activityUrl.Trim()))
                {
                    string appUrl = URLHelper.GetApplicationUrl(si.DomainName);
                    url = URLHelper.GetAbsoluteUrl(activityUrl, appUrl, appUrl, "");
                    url = URLHelper.AddParameterToUrl(url, URLHelper.SYSTEM_QUERY_PARAMETER, "1");
                    btnView.ToolTip = GetString("general.view");
                    btnView.NavigateUrl = url;
                    btnView.Visible = true;
                }
                else
                {
                    btnView.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Registers JavaScripts on page.
    /// </summary>
    protected void RegisterScripts()
    {
        var stamp = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSCMStamp");

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "AddStamp", ScriptHelper.GetScript(
            @"function InsertHTML(htmlString, ckClientID)
{
    // Get the editor instance that we want to interact with.
    var oEditor = oEditor = window.CKEDITOR.instances[ckClientID];
    // Check the active editing mode.
    if (oEditor != null) {
        // Check the active editing mode.
        if (oEditor.mode == 'wysiwyg') {
            // Insert the desired HTML.
            //oEditor.focus();
            oEditor.insertHtml(htmlString);        
        }
    }    
    return false;
}   

function AddStamp(ckClientID)
{
InsertHTML('<div>" + MacroResolver.Resolve(stamp).Replace("'", @"\'") + @"</div>', ckClientID);
}
"));
    }

    #endregion
}