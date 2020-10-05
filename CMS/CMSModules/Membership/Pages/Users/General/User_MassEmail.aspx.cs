using System;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Membership_Pages_Users_General_User_MassEmail : CMSUsersPage
{
    #region "Variables"

    private int siteId;

    #endregion


    #region "Control methods"

    /// <summary>
    /// Page load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Don't show generic roles in role selector except role 'Everyone'
        roles.UseFriendlyMode = true;
        roles.DisplayEveryone = true;

        // Get current user
        var currentUser = MembershipContext.AuthenticatedUser;
        if (currentUser == null)
        {
            return;
        }

        // Try to get site ID from query string
        siteId = SiteID;

        // Disable sites dropdown for non-global admins
        if (siteId > 0)
        {
            // Hide site selector
            CurrentMaster.DisplaySiteSelectorPanel = false;
            siteSelector.StopProcessing = true;
        }
        else
        {
            // Show content placeholder where site selector can be shown
            CurrentMaster.DisplaySiteSelectorPanel = true;

            siteSelector.UniSelector.OnSelectionChanged += Site_Changed;
            siteSelector.DropDownSingleSelect.AutoPostBack = true;

            if (!RequestHelper.IsPostBack())
            {
                siteSelector.Value = UniSelector.US_ALL_RECORDS;
            }

            // Load selected site from site selector
            if (RequestHelper.IsPostBack())
            {
                siteId = ValidationHelper.GetInteger(siteSelector.Value, 0);
            }
            // Set site ID to other selectors
            roles.GlobalRoles = (siteSelector.SiteID == UniSelector.US_ALL_RECORDS);
            roles.SiteRoles = siteSelector.SiteID != UniSelector.US_ALL_RECORDS;
        }

        roles.SiteID = siteId;
        roles.CurrentSelector.SelectionMode = SelectionModeEnum.Multiple;
        roles.CurrentSelector.ResourcePrefix = "addroles";
        roles.UseCodeNameForSelection = false;
        roles.ShowSiteFilter = false;

        users.SiteID = siteId;
        users.IsLiveSite = false;
        users.UniSelector.ReturnColumnName = "UserID";
        users.ShowSiteFilter = false;

        // Display/hide specific selectors according to selected site
        EnsureSelectorPanels();

        // Add current user email at first load        
        if (!RequestHelper.IsPostBack())
        {
            emailSender.Text = currentUser.Email;
        }

        // Initialize master page and other controls
        uploader.AddButtonIconClass = "icon-paperclip";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "IsSubjectEmpty", ScriptHelper.GetScript(@"
function IsSubjectEmpty()
{
    var obj = document.getElementById('" + txtSubject.ClientID + @"');
    if (obj != null)
    {
        if ((obj.value == null) || obj.value == '' || obj.value.replace(/ /g,'') == '')
        {
            if (!confirm(" + ScriptHelper.GetString(GetString("massemail.emptyemailsubject")) + @"))
            {
                return false;
            }
        }
    }
    return true;
}"));

        // Hides HTML or text area according to e-mail format in settings
        EnsureEmailFormatRegions();

        // Initialize the e-mail text editor
        InitHTMLEditor();

        // Initialize header actions
        InitHeaderActions();
    }


    /// <summary>
    /// Handles header actions.
    /// </summary>
    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            case "send":
                Send();
                break;

            case "clear":
                Clear();
                break;
        }
    }


    /// <summary>
    /// Handles change in site selection.
    /// </summary>
    protected void Site_Changed(object sender, EventArgs e)
    {
        // Delete users
        users.Value = null;
        users.ReloadData();

        // Delete roles
        roles.Value = null;
        roles.Reload(true);

        // Hide HTML or text area according to e-mail format in settings
        EnsureEmailFormatRegions();

        pnlUpdate.Update();
    }


    /// <summary>
    /// Clear page.
    /// </summary>
    protected void Clear()
    {
        txtSubject.Text = String.Empty;
        txtPlainText.Text = String.Empty;
        htmlText.ResolvedValue = String.Empty;
        emailSender.Text = MembershipContext.AuthenticatedUser.Email;

        users.Value = String.Empty;
        users.ReloadData();

        roles.Value = String.Empty;
        roles.Reload(true);
    }


    /// <summary>
    /// Sends the email.
    /// </summary>
    protected void Send()
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        var sender = emailSender.Text;

        // Validate first
        if (!emailSender.IsValid() || string.IsNullOrEmpty(sender))
        {
            ShowError(GetString("general.correctemailformat"));
            return;
        }

        // Get recipients

        string userIDs = Convert.ToString(users.Value);
        string roleIDs = Convert.ToString(roles.Value);

        if (string.IsNullOrEmpty(userIDs) && string.IsNullOrEmpty(roleIDs))
        {
            ShowError(GetString("massemail.norecipients"));
            return;
        }

        // Get resolver to resolve context macros
        MacroResolver resolver = MacroResolver.GetInstance();

        // Create the message
        EmailMessage message = new EmailMessage();
        message.Subject = resolver.ResolveMacros(txtSubject.Text);
        message.From = sender;
        if (plcText.Visible)
        {
            message.Body = resolver.ResolveMacros(htmlText.ResolvedValue);
        }
        if (plcPlainText.Visible)
        {
            message.PlainTextBody = resolver.ResolveMacros(txtPlainText.Text);
        }

        // Get the attachments
        HttpPostedFile[] attachments = uploader.PostedFiles;
        foreach (HttpPostedFile att in attachments)
        {
            message.Attachments.Add(new EmailAttachment(att.InputStream, Path.GetFileName(att.FileName), Guid.NewGuid(), DateTime.Now, siteId));
        }

        // Check if list of roleIds contains generic role 'Everyone'
        bool containsEveryone = false;

        if (!String.IsNullOrEmpty(roleIDs))
        {
            RoleInfo roleEveryone = RoleInfoProvider.GetRoleInfo(RoleName.EVERYONE, siteId);
            if ((roleEveryone != null) && (";" + roleIDs + ";").Contains(";" + roleEveryone.RoleID + ";"))
            {
                containsEveryone = true;
            }
        }

        // Send messages using email engine
        EmailSender.SendMassEmails(message, userIDs, roleIDs, siteId, containsEveryone);
        
        // Clear the form if email was sent successfully
        Clear();
        ShowConfirmation(GetString("system_email.emailsent"));
    }


    /// <summary>
    /// Initializes HTML editor's settings.
    /// </summary>
    protected void InitHTMLEditor()
    {
        htmlText.AutoDetectLanguage = false;
        htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlText.ToolbarSet = "SimpleEdit";
        htmlText.MediaDialogConfig.UseFullURL = true;
        htmlText.LinkDialogConfig.UseFullURL = true;
        htmlText.QuickInsertConfig.UseFullURL = true;
    }


    /// <summary>
    /// Initializes header actions.
    /// </summary>
    private void InitHeaderActions()
    {
        HeaderAction sendAction = new HeaderAction
        {
            Text = GetString("general.send"),
            ButtonStyle = ButtonStyle.Primary,
            CommandName = "send",
            OnClientClick = "return IsSubjectEmpty()",
            ResourceName = "CMS.Users",
            Permission = "Modify"
        };

        HeaderAction clearAction = new HeaderAction
        {
            Text = GetString("general.clear"),
            ButtonStyle = ButtonStyle.Default,
            CommandName = "clear",
        };

        HeaderActions.AddAction(sendAction);
        HeaderActions.AddAction(clearAction);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Hides HTML or text area according to e-mail format in settings.
    /// </summary>
    protected void EnsureEmailFormatRegions()
    {
        string siteName = null;
        plcPlainText.Visible = true;
        plcText.Visible = true;

        if (siteId > 0)
        {
            // Get site name
            siteName = SiteInfoProvider.GetSiteName(siteId);
        }

        // Get e-mail format from settings
        EmailFormatEnum emailFormat = EmailHelper.GetEmailFormat(siteName);
        switch (emailFormat)
        {
            case EmailFormatEnum.Html:
                plcPlainText.Visible = false;
                break;

            case EmailFormatEnum.PlainText:
                plcText.Visible = false;
                break;
        }
    }


    /// <summary>
    /// Displays or hides specific selectors according to selected site.
    /// </summary>
    protected void EnsureSelectorPanels()
    {
        // Check group availability and try to load group selector
        pnlGroups.Visible = false;
    }

    #endregion
}