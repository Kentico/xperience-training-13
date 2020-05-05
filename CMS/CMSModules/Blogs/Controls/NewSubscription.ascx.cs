using System;

using CMS.Blogs;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_Blogs_Controls_NewSubscription : CMSUserControl
{
    #region "Public properties"

    /// <summary>
    /// Document ID.
    /// </summary>
    public int DocumentID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets document node ID.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets document culture.
    /// </summary>
    public string Culture
    {
        get;
        set;
    }


    /// <summary>
    /// Properties passed from the upper control.
    /// </summary>
    public BlogProperties BlogProperties
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string valGroup = UniqueID;

        lblEmail.ResourceString = "blog.subscription.email";
        btnOk.ResourceString = "blog.subscription.subscribe";
        btnOk.ValidationGroup = valGroup;

        rfvEmailRequired.ErrorMessage = GetString("blog.subscription.noemail");
        rfvEmailRequired.ValidationGroup = valGroup;

        // Enable client side validation for emails
        txtEmail
            .EnableClientSideEmailFormatValidation(valGroup, "general.correctemailformat")
            .RegisterCustomValidator(rfvEmailRequired);
    }


    /// <summary>
    /// Pre-fill user e-mail.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsPostBack())
        {
            // Pre-fill user e-mail address to empty textbox for the first time
            if ((txtEmail.Text.Trim() == "") && (MembershipContext.AuthenticatedUser != null))
            {
                txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
            }
        }
    }


    /// <summary>
    /// OK click handler.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        // Check input fields
        string email = txtEmail.Text.Trim();
        string result = new Validator()
            .NotEmpty(email, rfvEmailRequired.ErrorMessage)
            .MatchesCondition(txtEmail, input => input.IsValid(), GetString("general.correctemailformat"))
            .Result;

        // Try to subscribe new subscriber
        if (result == String.Empty)
        {
            if (DocumentID > 0)
            {
                BlogPostSubscriptionInfo bpsi = BlogPostSubscriptionInfoProvider.GetBlogPostSubscriptionInfo(email, DocumentID);

                // Check for duplicity of subscriptions
                if ((bpsi == null) || !bpsi.SubscriptionApproved)
                {
                    bpsi = new BlogPostSubscriptionInfo();
                    bpsi.SubscriptionPostDocumentID = DocumentID;
                    bpsi.SubscriptionEmail = email;

                    // Update user id for logged users (except the public users)
                    if ((MembershipContext.AuthenticatedUser != null) && (!MembershipContext.AuthenticatedUser.IsPublic()))
                    {
                        bpsi.SubscriptionUserID = MembershipContext.AuthenticatedUser.UserID;
                    }

                    BlogPostSubscriptionInfoProvider.Subscribe(bpsi, DateTime.Now, true, true);

                    lblInfo.Visible = true;
                    if (bpsi.SubscriptionApproved)
                    {
                        lblInfo.Text = GetString("blog.subscription.beensubscribed");
                        Service.Resolve<ICurrentContactMergeService>().UpdateCurrentContactEmail(bpsi.SubscriptionEmail, MembershipContext.AuthenticatedUser);                        
                    }
                    else
                    {
                        lblInfo.Text = GetString("general.subscribed.doubleoptin");
                        int optInInterval = BlogHelper.GetBlogDoubleOptInInterval(SiteContext.CurrentSiteName);
                        if (optInInterval > 0)
                        {
                            lblInfo.Text += "<br />" + string.Format(GetString("general.subscription_timeintervalwarning"), optInInterval);
                        }

                    }
                    
                    // Clear form after successful subscription
                    txtEmail.Text = "";
                }
                else
                {
                    result = GetString("blog.subscription.emailexists");
                }
            }
            else
            {
                result = GetString("general.invalidid");
            }
        }

        if (result == String.Empty)
        {
            return;
        }

        lblError.Visible = true;
        lblError.Text = result;
    }

    #endregion
}