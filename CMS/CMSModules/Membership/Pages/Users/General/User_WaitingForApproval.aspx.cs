using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_General_User_WaitingForApproval : CMSUsersPage
{
    #region "Methods"


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        gridElem.HideFilterButton = true;
        gridElem.LoadGridDefinition();

        // Filter settings
        IUserFilter filter = (IUserFilter)gridElem.CustomFilter;

        filter.DisplayUserEnabled = false;

    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Pagetitle
        Title = "Users - Waiting for approval";

        // Set grid list properties
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.OnBeforeDataReload += new OnBeforeDataReload(gridElem_OnBeforeDataReload);
        gridElem.OnExternalDataBound += new OnExternalDataBoundEventHandler(gridElem_OnExternalDataBound);
        gridElem.ZeroRowsText = GetString("general.nodatafound");


        CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);
        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("administration.users.approveallselected"),
            OnClientClick = "if(confirm(" + ScriptHelper.GetString(GetString("administration.users.approveallselectedquestion")) + ")){}else{return false}",
            CommandName = "approveall"

        });

        CurrentMaster.HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("administration.users.rejectallselected"),
            OnClientClick = "OpenReject(); return false;",
            CommandName = "rejectall"
        });



        ltlScript.Text = "";

        // Register script for javascript postback
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UpdatePage", ScriptHelper.GetScript(
            "function UpdatePage(){ " + Page.ClientScript.GetPostBackEventReference(btnUpdate, "") + "; } \n"));

        // Register dialog script for modal window
        ScriptHelper.RegisterDialogScript(this);

        // Register opener for modal window
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "OpenReject",
                                               ScriptHelper.GetScript(
                                                   "function OpenReject(user)\n" +
                                                   "{\n" +
                                                   "document.getElementById('" + hdnUser.ClientID + "').value = user;  \n" +
                                                   "modalDialog('" + ResolveUrl("~/CMSModules/Membership/Pages/Users/General/User_Reject.aspx") + "', 'UserReject', 650, 550); \n" +
                                                   "return false;\n" +
                                                   "}\n"));


        // Register script for recieving data from modal window
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "SetRejectParam",
                                               ScriptHelper.GetScript(
                                                   "function SetRejectParam(reason, email, confirmDelete)\n" +
                                                   "{\n" +
                                                   "document.getElementById('" + hdnReason.ClientID + "').value = reason;\n" +
                                                   "document.getElementById('" + hdnSendEmail.ClientID + "').value  = email;\n" +
                                                   "document.getElementById('" + hdnConfirmDelete.ClientID + "').value  = confirmDelete;\n" +
                                                   "UpdatePage();\n" +
                                                   "}\n"));

    }


    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "approveall":
                ApproveAll();
                break;
            case "rejectall":
                RejectAll();
                break;
        }
    }


    /// <summary>
    /// Sets grid where condition before data binding.
    /// </summary>
    protected void gridElem_OnBeforeDataReload()
    {
        gridElem.WhereCondition = GetWhereCondition();
    }


    private string GetWhereCondition()
    {
        string whereCondition = "(UserWaitingForApproval = 1)";

        // Get site related condition
        if (SiteID != 0)
        {
            whereCondition += " AND (UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID=" + SiteID + "))";
        }

        return whereCondition;
    }


    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "userenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "formattedusername":
                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(Convert.ToString(parameter)));
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        // Approve action
        if (actionName == "approve")
        {
            ApproveUsers(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    /// <summary>
    /// Approves all selected users.
    /// </summary>
    private void ApproveAll()
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        if (gridElem.SelectedItems.Count > 0)
        {
            ApproveUsers(0);
        }
        else
        {
            ltlScript.Text = ScriptHelper.GetScript("alert('" + GetString("administration.users.nousers") + "');");
        }
    }


    /// <summary>
    /// Reject all selected users.
    /// </summary>
    private void RejectAll()
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        hdnUser.Value = "0";
    }


    /// <summary>
    /// Approve users.
    /// </summary>
    /// <param name="actionArgument">User ID</param>
    protected void ApproveUsers(int actionArgument)
    {
        // Process single user
        if (actionArgument > 0)
        {
            SetUserInfo(actionArgument);
        }
        // Process all selected users
        else
        {
            // Get selected users
            var users = gridElem.SelectedItems;

            // Set user activation properties
            foreach (string userID in users)
            {
                SetUserInfo(ValidationHelper.GetInteger(userID, 0));
            }
        }
        gridElem.ResetSelection();
        gridElem.ReloadData();
    }


    /// <summary>
    /// Reject users.
    /// </summary>
    /// <param name="actionArgument">User ID</param>
    private void RejectUsers(int actionArgument)
    {
        // Process single user
        if (actionArgument > 0)
        {
            DeleteUser(actionArgument);
        }
        // Process all selected users
        else
        {
            // Get selected users
            var users = gridElem.SelectedItems;

            // Set user activation properties
            foreach (string userID in users)
            {
                DeleteUser(ValidationHelper.GetInteger(userID, 0));
            }
        }
        // Reload data
        hdnReason.Value = "";
        hdnSendEmail.Value = "";
        hdnConfirmDelete.Value = "";
        hdnUser.Value = "";
        gridElem.ResetSelection();
        gridElem.ReloadData();
    }


    /// <summary>
    /// Sets new UserInfo for approved user.
    /// </summary>
    /// <param name="userID">User to be approved</param>
    protected void SetUserInfo(int userID)
    {
        UserInfo user = UserInfoProvider.GetFullUserInfo(userID);

        // Cancel waiting for approval attribute
        user.UserSettings.UserWaitingForApproval = false;
        // Set activation time to now
        user.UserSettings.UserActivationDate = DateTime.Now;
        // Set user who activated this account
        user.UserSettings.UserActivatedByUserID = MembershipContext.AuthenticatedUser.UserID;
        // Enable user
        user.Enabled = true;

        UserInfo.Provider.Set(user);

        // Send e-mail to user
        if (!String.IsNullOrEmpty(user.Email))
        {
            EmailTemplateInfo template = EmailTemplateInfo.Provider.Get("RegistrationUserApproved", SiteContext.CurrentSiteID);
            if (template != null)
            {
                string from = EmailHelper.GetSender(template, SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress"));
                if (!String.IsNullOrEmpty(from))
                {
                    EmailMessage email = new EmailMessage();
                    email.EmailFormat = EmailFormatEnum.Default;
                    // Get e-mail sender and subject from template, if used
                    email.From = from;
                    email.Recipients = user.Email;

                    MacroResolver resolver = MembershipResolvers.GetRegistrationApprovalResolver(user);

                    // Enable encoding of macro results for HTML mail body
                    resolver.Settings.EncodeResolvedValues = true;
                    email.Body = resolver.ResolveMacros(template.TemplateText);

                    // Disable encoding of macro results for plaintext body and subject
                    resolver.Settings.EncodeResolvedValues = false;
                    string emailSubject = EmailHelper.GetSubject(template, GetString("registrationform.registrationapprovalemailsubject"));
                    email.Subject = resolver.ResolveMacros(emailSubject);

                    email.PlainTextBody = resolver.ResolveMacros(template.TemplatePlainText);

                    email.CcRecipients = template.TemplateCc;
                    email.BccRecipients = template.TemplateBcc;

                    try
                    {
                        // Add attachments
                        EmailHelper.ResolveMetaFileImages(email, template.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                        EmailSender.SendEmail(SiteContext.CurrentSiteName, email);
                    }
                    catch
                    {
                        Service.Resolve<IEventLogService>().LogError("Membership", "WaitingForApprovalEmail");
                    }
                }
                else
                {
                    Service.Resolve<IEventLogService>().LogError("WaitingForApproval", "EmailSenderNotSpecified");
                }
            }
            else
            {
                // Log missing e-mail template
                try
                {
                    var logData = new EventLogData(EventTypeEnum.Error, "RegistrationUserApproved", "GetEmailTemplate")
                    {
                        EventUrl = RequestContext.RawURL
                    };
                    
                    Service.Resolve<IEventLogService>().LogEvent(logData);
                }
                catch
                {
                }
            }
        }
    }


    /// <summary>
    /// Rejects and deletes user.
    /// </summary>
    /// <param name="userID">User to be rejected</param>
    protected void DeleteUser(int userID)
    {
        // Find user
        UserInfo user = UserInfoProvider.GetFullUserInfo(userID);

        // Send e-mail if requested
        if (hdnSendEmail.Value == "true")
        {
            EmailMessage em = new EmailMessage();

            string from = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");

            if (!String.IsNullOrEmpty(from))
            {
                // Set message content
                em.From = from;
                em.Recipients = user.Email;
                em.Subject = GetString("administration.users.rejected");
                em.EmailFormat = EmailFormatEnum.Default;

                string emailBody = hdnReason.Value.Trim();

                em.Body = HTMLHelper.HTMLEncode(emailBody);
                em.PlainTextBody = emailBody;

                // Send message
                EmailSender.SendEmail(em);
            }
            else
            {
                Service.Resolve<IEventLogService>().LogError("WaitingForApproval", "EmailSenderNotSpecified");
            }
        }

        // Delete user
        UserInfo.Provider.Delete(UserInfo.Provider.Get(userID));
    }


    /// <summary>
    /// Reject all selected users.
    /// </summary>
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        // Run reject function
        if ((hdnConfirmDelete.Value != null) && (hdnConfirmDelete.Value != ""))
        {
            RejectUsers(ValidationHelper.GetInteger(hdnUser.Value, 0));
        }
    }

    #endregion
}
