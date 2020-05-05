using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.UIControls;


[Security(Resource = ModuleName.NEWSLETTER, Permission = "authorissues")]
[UIElement(ModuleName.NEWSLETTER, "Newsletter.Send")]
[EditedObject(NewsletterInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Send : CMSNewsletterPage
{
    private int newsletterId;


    protected void Page_Load(object sender, EventArgs e)
    {
        NewsletterInfo newsletterObj = EditedObject as NewsletterInfo;
        if (newsletterObj == null)
        {
            RedirectToAccessDenied(GetString("general.invalidparameters"));
        }

        if (!newsletterObj.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(newsletterObj.TypeInfo.ModuleName, "Read");
        }

        newsletterId = newsletterObj.NewsletterID;
        sendElem.NewsletterID = newsletterId;
        btnSend.Enabled = true;

        if (btnSend.Action != null)
        {
            btnSend.Action.Text = GetString("general.send");
        }
    }


    protected void btnSend_Click(object sender, EventArgs e)
    {
        // Generate new issue
        try
        {
            int issueId = EmailQueueManager.GenerateDynamicIssue(newsletterId);
            if (issueId <= 0)
            {
                return;
            }

            // Set ID of generated issue
            sendElem.IssueID = issueId;

            // Send the issue according to selected options
            if (sendElem.SendIssue())
            {
                ShowConfirmation(GetString("Newsletter_Send.SuccessfullySent"));
            }
            else
            {
                ShowError(sendElem.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ShowError(GetString("Newsletter_Send.ErrorSent") + ex.Message);
        }
    }
}