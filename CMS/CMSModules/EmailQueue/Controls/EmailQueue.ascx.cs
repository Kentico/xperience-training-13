using System;
using System.Collections;
using System.Data;

using CMS.Base;

using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.SiteProvider;
using CMS.Core;

public partial class CMSModules_EmailQueue_Controls_EmailQueue : CMSAdminControl, ICallbackEventHandler
{
    #region "Variables"

    private Hashtable mParameters;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the UniGrid.
    /// </summary>
    public UniGrid EmailGrid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Gets or sets the email id.
    /// </summary>
    private int EmailID
    {
        get;
        set;
    }


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
            gridElem.StopProcessing = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check permissions
        RaiseOnCheckPermissions("READ", this);

        if (StopProcessing)
        {
            return;
        }

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Register script for modal dialog with mass email recipients
        // and for opening modal dialog displaying email detail
        string script = string.Format(
@"var emailDialogParams_{0} = '';

function DisplayRecipients(emailId) {{
    if ( emailId != 0 ) {{
        modalDialog({1} + '?emailid=' + emailId, 'emailrecipients', 920, 700);
    }}
}}

function OpenEmailDetail(queryParameters) {{
    modalDialog({2} + queryParameters, 'emaildetails', 1000, 730);
}}", ClientID, ScriptHelper.GetString(ResolveUrl("~/CMSModules/EmailQueue/MassEmails_Recipients.aspx")), ScriptHelper.GetString(ResolveUrl("~/CMSModules/EmailQueue/EmailQueue_Details.aspx")));

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Email_" + ClientID, script, true);

        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
    }

    #endregion


    #region "Public Methods"

    public override void ReloadData()
    {
        if (!StopProcessing)
        {
            gridElem.ReloadData();
        }
        base.ReloadData();
    }


    /// <summary>
    /// Returns the IDs of e-mail messages that were selected.
    /// </summary>
    /// <returns>An array of email IDs</returns>
    public int[] GetSelectedEmailIDs()
    {
        return EmailGrid.SelectedItems.Select(v => ValidationHelper.GetInteger(v, 0)).ToArray();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets the dialog identifier used for sharing data between windows.
    /// </summary>
    /// <returns>Dialog identifier</returns>
    private Guid GetDialogIdentifier()
    {
        Guid identifier;

        // Try parse the identifier as a Guid value
        if (!Guid.TryParse(hdnIdentifier.Value, out identifier))
        {
            // If the identifier value is not a valid Guid value, generates a new Guid
            identifier = Guid.NewGuid();
            hdnIdentifier.Value = identifier.ToString();
        }

        return identifier;
    }

    #endregion


    #region "Unigrid events"

    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        sourceName = sourceName.ToLowerCSafe();

        switch (sourceName)
        {
            case "priority":
                return GetEmailPriority(parameter);

            case "status":
                return GetEmailStatus(parameter);

            case "subject":
                return HTMLHelper.HTMLEncode(TextHelper.LimitLength(parameter.ToString(), 40));

            case "result":
                string result = parameter.ToString();
                int newLineIndex = result.IndexOfCSafe("\r\n", true);
                return HTMLHelper.HTMLEncode(TextHelper.LimitLength((newLineIndex > 0) ? result.Remove(newLineIndex) : result, 45));

            case "resend":
            case "delete":
                CMSGridActionButton imageButton = sender as CMSGridActionButton;
                if (imageButton != null)
                {
                    DisableActionButtons(imageButton, parameter);
                }
                break;

            case "emailto":
                return GetEmailRecipients(parameter);

            case "edit":
                CMSGridActionButton viewBtn = (CMSGridActionButton)sender;
                viewBtn.OnClientClick = string.Format("emailDialogParams_{0} = '{1}';{2};return false;",
                                                          ClientID,
                                                          viewBtn.CommandArgument, Page.ClientScript.GetCallbackEventReference(this, "emailDialogParams_" + ClientID, "OpenEmailDetail", null));
                break;
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        if (StopProcessing)
        {
            return;
        }

        if (!CMSActionContext.CurrentUser.IsAuthorizedPerResource(ModuleName.EMAILENGINE, EmailQueuePage.MODIFY_PERMISSION, SiteContext.CurrentSiteName, false))
        {
            RedirectToAccessDenied(ModuleName.EMAILENGINE, EmailQueuePage.MODIFY_PERMISSION);
        }

        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                // Delete an email
                int deleteEmailId = ValidationHelper.GetInteger(actionArgument, 0);
                EmailHelper.Queue.Delete(deleteEmailId);
                break;

            case "resend":
                // Resend email info object from queue
                int sendEmailId = ValidationHelper.GetInteger(actionArgument, -1);
                if (sendEmailId > 0)
                {
                    EmailHelper.Queue.Send(sendEmailId);
                    ShowInformation(GetString("emailqueue.sendingemails"));
                }
                break;
        }
    }


    /// <summary>
    /// Gets the e-mail priority.
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>E-mail priority</returns>
    private string GetEmailPriority(object parameter)
    {
        switch ((EmailPriorityEnum)parameter)
        {
            case EmailPriorityEnum.Low:
                return GetString("emailpriority.low");

            case EmailPriorityEnum.Normal:
                return GetString("emailpriority.normal");

            case EmailPriorityEnum.High:
                return GetString("emailpriority.high");

            default:
                return string.Empty;
        }
    }


    /// <summary>
    /// Gets the e-mail status.
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>E-mail status</returns>
    private string GetEmailStatus(object parameter)
    {
        switch ((EmailStatusEnum)parameter)
        {
            case EmailStatusEnum.Created:
                return GetString("emailstatus.created");

            case EmailStatusEnum.Waiting:
                return GetString("emailstatus.waiting");

            case EmailStatusEnum.Sending:
                return GetString("emailstatus.sending");

            default:
                return string.Empty;
        }
    }


    /// <summary>
    /// Gets the e-mail recipient(s).
    /// </summary>
    /// <param name="parameter">The parameter</param>
    /// <returns>E-mail recipients</returns>
    private string GetEmailRecipients(object parameter)
    {
        DataRowView dr = (DataRowView)parameter;

        if (ValidationHelper.GetBoolean(dr["EmailIsMass"], false))
        {
            return string.Format("<a href=\"#\" onclick=\"javascript: DisplayRecipients({0}); return false; \">{1}</a>",
                                 ValidationHelper.GetInteger(dr["EmailID"], 0),
                                 GetString("emailqueue.queue.massdetails"));
        }
        
        return HTMLHelper.HTMLEncode(ValidationHelper.GetString(dr["EmailTo"], string.Empty));
    }


    /// <summary>
    /// Disables the action buttons.
    /// </summary>
    /// <param name="imageButton">The image button</param>
    /// <param name="parameter">The parameter</param>
    private void DisableActionButtons(CMSGridActionButton imageButton, object parameter)
    {
        int status = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem).Row["EmailStatus"], -1);
        bool sending = EmailHelper.Queue.SendingInProgess;

        // Disable action buttons (and image) if e-mail status is 'created' or 'sending'
        if (sending || (status == (int)EmailStatusEnum.Created) || (status == (int)EmailStatusEnum.Sending) ||
            !CMSActionContext.CurrentUser.IsAuthorizedPerResource(ModuleName.EMAILENGINE, EmailQueuePage.MODIFY_PERMISSION, SiteContext.CurrentSiteName, false))
        {
            imageButton.OnClientClick = null;
            imageButton.Enabled = false;
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        mParameters = new Hashtable();
        mParameters["where"] = gridElem.WhereCondition;
        mParameters["orderby"] = gridElem.SortDirect;

        // Get the dialog identifier
        Guid dialogIdentifier = GetDialogIdentifier();

        // Store the dialog identifier with appropriate data in the session
        WindowHelper.Add(dialogIdentifier.ToString(), mParameters);

        string queryString = "?params=" + dialogIdentifier;

        queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        queryString = URLHelper.AddParameterToUrl(queryString, "emailid", EmailID.ToString());

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        EmailID = ValidationHelper.GetInteger(eventArgument, 0);
    }

    #endregion
}
