using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Base;
using CMS.Membership;
using CMS.Core;

public partial class CMSModules_EmailQueue_EmailQueue_Details : CMSModalPage
{
    #region "Protected variables"

    protected int mEmailId;

    protected int mPrevId;

    protected int mNextId;

    protected Hashtable mParameters;

    private bool? mUserIsAdmin;

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


    /// <summary>
    /// Indicates whether user is admin.
    /// </summary>
    private bool UserIsAdmin
    {
        get
        {
            if (!mUserIsAdmin.HasValue)
            {
                mUserIsAdmin = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin);
            }

            return mUserIsAdmin.Value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!QueryHelper.ValidateHash("hash", "emailid") || Parameters == null)
        {
            return;
        }

        if (!CMSActionContext.CurrentUser.IsAuthorizedPerResource(ModuleName.EMAILENGINE, EmailQueuePage.READ_PERMISSION, SiteContext.CurrentSiteName, false))
        {
            RedirectToAccessDenied(ModuleName.EMAILENGINE, EmailQueuePage.READ_PERMISSION);
        }

        PageTitle.TitleText = GetString("emailqueue.details.title");
        // Get the ORDER BY column and starting event ID
        string orderBy = DataHelper.GetNotEmpty(Parameters["orderby"], "EmailID DESC");
        if (orderBy.IndexOf(";", StringComparison.Ordinal) >= 0)
        {
            orderBy = "EmailID DESC"; // ORDER BY with semicolon is considered to be dangerous
        }
        string whereCondition = ValidationHelper.GetString(Parameters["where"], string.Empty);

        // Get e-mail ID from query string
        mEmailId = QueryHelper.GetInteger("emailid", 0);

        if (!RequestHelper.IsPostBack())
        {
            LoadData();
            HandleFieldsVisibility();
        }
        
        // Initialize next/previous buttons
        int[] prevNext = EmailInfoProvider.GetPreviousNext(mEmailId, whereCondition, orderBy);
        if (prevNext != null)
        {
            mPrevId = prevNext[0];
            mNextId = prevNext[1];

            btnPrevious.Enabled = (mPrevId != 0);
            btnNext.Enabled = (mNextId != 0);

            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
        }
    }

    #endregion


    #region "Button handling"

    protected void btnPrevious_Click(object sender, EventArgs e)
    {
        // Redirect to previous
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "emailId", mPrevId.ToString()));
    }


    protected void btnNext_Click(object sender, EventArgs e)
    {
        // Redirect to next
        URLHelper.Redirect(URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "emailId", mNextId.ToString()));
    }

    #endregion


    #region "Protected methods"

    /// <summary>
    /// Loads data of specific e-mail from DB.
    /// </summary>
    protected void LoadData()
    {
        if (mEmailId <= 0)
        {
            return;
        }

        // Get specific e-mail
        EmailInfo ei = EmailInfoProvider.GetEmailInfo(mEmailId);
        if (ei == null)
        {
            plcDetails.Visible = false;
            ShowInformation(GetString("emailqueue.details.emailalreadysent"));
            return;
        }

        EditedObject = ei;

        lblFromValue.Text = HTMLHelper.HTMLEncode(ei.EmailFrom);
        lblToValue.Text = !ei.EmailIsMass ? HTMLHelper.HTMLEncode(ei.EmailTo) : GetString("emailqueue.detail.multiplerecipients");
        lblCcValue.Text = HTMLHelper.HTMLEncode(ei.EmailCc);
        lblBccValue.Text = HTMLHelper.HTMLEncode(ei.EmailBcc);
        lblReplyToValue.Text = HTMLHelper.HTMLEncode(ei.EmailReplyTo);
        lblSubjectValue.Text = HTMLHelper.HTMLEncode(ei.EmailSubject);
        lblErrorMessageValue.Text = HTMLHelper.HTMLEncodeLineBreaks(ei.EmailLastSendResult);

        if (UserIsAdmin)
        {
            LoadHTMLBody(ei);
            LoadPlainTextBody(ei);
            GetAttachments();
        }
    }


    /// <summary>
    /// Prepare the HTML body of the e-mail message for display.
    /// </summary>
    /// <param name="ei">The e-mail message object</param>
    private void LoadHTMLBody(EmailInfo ei)
    {
        string body = ei.EmailBody;

        if (string.IsNullOrEmpty(body))
        {
            lblBodyValue.Visible = true;
            lblBodyValue.Text = GetString("emailqueue.detail.valuenotentered");
            return;
        }

        // Regular expression to search the tracking image in HTML code
        Regex regExp = RegexHelper.GetRegex("(src=\"[^\"]+Track.ashx)\\?[^\"]*", true);
        Match matchTrack = regExp.Match(body);
        if (matchTrack.Success && (matchTrack.Groups.Count > 0))
        {
            // Remove parameters from tracking image URL so the statistics are not influenced by e-mail previews
            body = regExp.Replace(body, matchTrack.Groups[1].Value);
        }

        // Transform inline attachments back to metafile attachments
        regExp = RegexHelper.GetRegex("src=\"cid:(?<cidCode>[a-z0-9]{32})\"", true);
        body = regExp.Replace(body, TransformSrc);

        htmlTemplateBody.Visible = true;
        htmlTemplateBody.ResolvedValue = body;
        htmlTemplateBody.AutoDetectLanguage = false;
        htmlTemplateBody.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
    }


    /// <summary>
    /// Prepare the plain text body of the e-mail message for display.
    /// </summary>
    /// <param name="ei">The e-mail message object</param>
    private void LoadPlainTextBody(EmailInfo ei)
    {
        if (string.IsNullOrEmpty(ei.EmailPlainTextBody))
        {
            lblPlainTextValue.Text = GetString("emailqueue.detail.valuenotentered");
            return;
        }

        DiscussionMacroResolver dmh = new DiscussionMacroResolver { ResolveToPlainText = true };
        string body = dmh.ResolveMacros(ei.EmailPlainTextBody);

        body = HTMLHelper.HTMLEncode(body);

        // Replace line breaks with br tags and modify discussion macros
        lblPlainTextValue.Text = DiscussionMacroResolver.RemoveTags(HTMLHelper.HTMLEncodeLineBreaks(body));
    }


    /// <summary>
    /// Gets the attachments for the specified e-mail message.
    /// </summary>
    private void GetAttachments()
    {
        // Get basic info about all attachments attached to current e-mail
        DataSet ds = EmailAttachmentInfoProvider.GetEmailAttachmentInfos(mEmailId, null, -1, "AttachmentID, AttachmentName, AttachmentSize");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            plcAttachments.Visible = true;
            if (ds.Tables.Count > 0)
            {
                int i = 0;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (i > 0)
                    {
                        pnlAttachmentsList.Controls.Add(new LiteralControl("<br />"));
                    }
                    var eai = new EmailAttachmentInfo(dr);
                    pnlAttachmentsList.Controls.Add(new LiteralControl(HTMLHelper.HTMLEncode(eai.AttachmentName) + "&nbsp;(" + DataHelper.GetSizeString(eai.AttachmentSize) + ")"));
                    i++;
                }
            }
        }
        else
        {
            plcAttachments.Visible = false;
        }
    }


    /// <summary>
    /// Transforms inline attachment source back to metafile attachment source.
    /// </summary>
    /// <param name="match">Regex match result for inline attachment source</param>
    /// <returns>MetaFile attachment source</returns>
    private static string TransformSrc(Match match)
    {
        if (match.Groups.Count > 0)
        {
            // Get content ID (metafile GUID without '-') and make GUID of it
            string cidCode = match.Groups["cidCode"].Value;
            Guid mfGuid = (cidCode.Length == 32) ? ValidationHelper.GetGuid(cidCode.Insert(20, "-").Insert(16, "-").Insert(12, "-").Insert(8, "-"), Guid.Empty) : Guid.Empty;

            if (mfGuid != Guid.Empty)
            {
                // Get metafile by GUID
                MetaFileInfo mfi = MetaFileInfoProvider.GetMetaFileInfo(mfGuid, null, false);
                if (mfi != null)
                {
                    SiteInfo site = SiteInfoProvider.GetSiteInfo((mfi.MetaFileSiteID > 0) ? mfi.MetaFileSiteID : SiteContext.CurrentSiteID);
                    if (site !=null)
                    {
                        // return metafile source
                        return "src=\"" + URLHelper.GetAbsoluteUrl("~/CMSPages/GetMetaFile.aspx?fileguid=" + mfGuid, site.DomainName) + "\"";
                    }
                }
            }
        }

        return match.Value;
    }


    /// <summary>
    /// Handles visibility for fields <see cref="EmailInfo.EmailCc"/>, <see cref="EmailInfo.EmailBcc"/> and <see cref="EmailInfo.EmailReplyTo"/>
    /// If field value is empty label and value become hidden
    /// </summary>
    private void HandleFieldsVisibility()
    {
        if (!plcDetails.Visible || EditedObject == null)
        {
            return;
        }

        if (!UserIsAdmin)
        {
            pnlBody.Visible = false;
        }

        var emailInfo = (EmailInfo)EditedObject;

        SetPlaceholderVisibility(emailInfo.EmailCc, plcCc);
        SetPlaceholderVisibility(emailInfo.EmailBcc, plcBcc);
        SetPlaceholderVisibility(emailInfo.EmailReplyTo, plcReplyTo);
        SetPlaceholderVisibility(emailInfo.EmailLastSendResult, plcErrorMessage);
    }


    private static void SetPlaceholderVisibility(string emailValue, PlaceHolder placeHolder)
    {
        placeHolder.Visible = !string.IsNullOrEmpty(emailValue);
    }

    #endregion
}