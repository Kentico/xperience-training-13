using System;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Forums_Controls_Subscriptions_SubscriptionEdit : CMSAdminEditControl
{
    #region "Variables"

    private int mForumId;
    private int mSubscriptionId;
    private ForumSubscriptionInfo mSubscriptionObj;

    #endregion


    #region "Public properties"

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
    /// Gets or sets the ID of the forum to edit.
    /// </summary>
    public int ForumID
    {
        get
        {
            return mForumId;
        }
        set
        {
            mForumId = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the subscription to edit.
    /// </summary>
    public int SubscriptionID
    {
        get
        {
            return mSubscriptionId;
        }
        set
        {
            mSubscriptionId = value;
        }
    }


    /// <summary>
    /// Returns if checkbox "Send confirmation email" is checked or not.
    /// </summary>
    public bool SendEmailConfirmation
    {
        get
        {
            return chkSendConfirmationEmail.Checked;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the subscription obj.
    /// </summary>
    protected ForumSubscriptionInfo SubscriptionObj
    {
        get
        {
            return mSubscriptionObj ?? (mSubscriptionObj = SubscriptionID > 0 ? ForumSubscriptionInfoProvider.GetForumSubscriptionInfo(SubscriptionID) : new ForumSubscriptionInfo());
        }
        set
        {
            mSubscriptionObj = value;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query keys
        bool saved = QueryHelper.GetBoolean("saved", false);
        bool chkChecked = QueryHelper.GetBoolean("checked", true);

        // Get resource strings        
        rfvSubscriptionEmail.ErrorMessage = GetString("ForumSubscription_Edit.EnterSomeEmail");
        btnOk.Text = GetString("General.OK");

        // Check whether the forum still exists
        if ((ForumID > 0) && (ForumInfoProvider.GetForumInfo(ForumID) == null))
        {
            RedirectToInformation("editedobject.notexists");
        }

        EditedObject = SubscriptionObj;

        // Set edited object
        if (SubscriptionID > 0)
        {
            pnlSendConfirmationEmail.Visible = false;
        }

        bool process = true;
        if (!Visible || StopProcessing)
        {
            EnableViewState = false;
            process = false;
        }

        if (!IsLiveSite && !RequestHelper.IsPostBack() && process)
        {
            ReloadData();
        }

        if (!RequestHelper.IsPostBack())
        {
            chkSendConfirmationEmail.Checked = true;
        }

        if (!chkChecked)
        {
            chkSendConfirmationEmail.Checked = false;
        }

        if (saved && !RequestHelper.IsPostBack())
        {
            ShowChangesSaved();
        }

        txtSubscriptionEmail
            .EnableClientSideEmailFormatValidation(errorMessageResourceString: "ForumSubscription_Edit.emailErrorMsg")
            .RegisterCustomValidator(rfvSubscriptionEmail);
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        if (!String.IsNullOrWhiteSpace(txtSubscriptionEmail.Text))
        {
            // Check that e-mail is not already subscribed
            if (ForumSubscriptionInfoProvider.IsSubscribed(txtSubscriptionEmail.Text.Trim(), mForumId, 0))
            {
                ShowError(GetString("ForumSubscibe.SubscriptionExists"));
                return;
            }

            if (SubscriptionObj.SubscriptionID <= 0)
            {
                SubscriptionObj.SubscriptionForumID = mForumId;
                SubscriptionObj.SubscriptionGUID = Guid.NewGuid();
            }

            if (!txtSubscriptionEmail.IsValid())
            {
                ShowError(GetString("ForumSubscription_Edit.EmailIsNotValid"));
                return;
            }

            SubscriptionObj.SubscriptionEmail = txtSubscriptionEmail.Text.Trim();
            ForumSubscriptionInfoProvider.SetForumSubscriptionInfo(SubscriptionObj, chkSendConfirmationEmail.Checked, null, null);
            SubscriptionID = SubscriptionObj.SubscriptionID;
            ShowChangesSaved();
            RaiseOnSaved();
        }
        else
        {
            ShowError(GetString("ForumSubscription_Edit.EnterSomeEmail"));
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        ClearForm();
        if (SubscriptionObj.SubscriptionID > 0)
        {
            txtSubscriptionEmail.Text = HTMLHelper.HTMLEncode(SubscriptionObj.SubscriptionEmail);
        }
    }


    /// <summary>
    /// Clears the form fields to default values.
    /// </summary>
    public override void ClearForm()
    {
        txtSubscriptionEmail.Text = "";
    }

    #endregion
}