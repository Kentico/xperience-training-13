using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_DashBoard_EmailQueue : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Site name (empty string for all sites).
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), "").Replace("##currentsite##", SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Order by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "EmailLastSendAttempt");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Sorting.
    /// </summary>
    public string Sorting
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Sorting"), "desc");
        }
        set
        {
            SetValue("Sorting", value);
        }
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemsPerPage"), "25");
        }
        set
        {
            SetValue("ItemsPerPage", value);
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            emailQueue.StopProcessing = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Register the dialog script
            ScriptHelper.RegisterDialogScript(Page);

            // Register script for modal dialog
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "displayModal", ScriptHelper.GetScript(
                "function DisplayRecipients(emailId) { \n" +
                "    if ( emailId != 0 ) { \n" +
                "       modalDialog('" + UrlResolver.ResolveUrl("~/CMSModules/EmailQueue/MassEmails_Recipients.aspx") + "?emailid=' + emailId, 'emailrecipients', 920, 700); \n" +
                "    } } \n "));

            if ((!RequestHelper.IsPostBack()) && (!string.IsNullOrEmpty(ItemsPerPage)))
            {
                emailQueue.EmailGrid.Pager.DefaultPageSize = ValidationHelper.GetInteger(ItemsPerPage, -1);
            }

            emailQueue.EmailGrid.OrderBy = OrderBy + " " + Sorting;
            emailQueue.EmailGrid.WhereCondition = GenerateWhereCondition();
            emailQueue.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(emailQueue_OnCheckPermissions);
        }
    }


    /// <summary>
    /// OnLoad handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        emailQueue.ReloadData();
        base.OnLoad(e);
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        emailQueue.ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Remove selection checkboxes
        emailQueue.EmailGrid.GridView.Columns[0].Visible = false;
    }


    /// <summary>
    /// Check permission.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void emailQueue_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            sender.StopProcessing = true;
            emailQueue.Visible = false;
            messageElem.Visible = true;
            messageElem.ErrorMessage = GetString("general.nopermission");
        }
    }


    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = "";

        // Append site condition if siteid given
        SiteInfo siteObj = SiteInfo.Provider.Get(SiteName);
        int siteId = -1;

        if (siteObj != null)
        {
            siteId = siteObj.SiteID;
        }
        // create where condition for SiteID
        if (siteId > 0)
        {
            whereCond += " (EmailSiteID=" + siteId + ")";
        }

        return whereCond;
    }

    #endregion
}
