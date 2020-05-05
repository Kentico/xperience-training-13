using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Rating control used for web part and transformations. 
/// </summary>
public partial class CMSAdminControls_ContentRating_RatingControl : CMSUserControl
{
    #region "Private variables"

    private AbstractRatingControl usrControl;
    private double mExternalValue = -1.0;
    private bool loaded = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets max value of scale.
    /// </summary>
    public int MaxRatingValue { get; set; } = 5;


    /// <summary>
    /// Gets or sets value that indicates whether unrated value is allowed.
    /// </summary>
    public bool AllowZeroValue { get; set; } = true; 


    /// <summary>
    /// Gets or sets current value. If value is negative number then document
    /// rating is used.
    /// </summary>
    public string ExternalValue
    {
        get
        {
            return mExternalValue.ToString();
        }
        set
        {
            double extValue = ValidationHelper.GetDouble(value, -1.0);
            if (mExternalValue != extValue)
            {
                // Set external value and reload control
                mExternalValue = extValue;

                if (loaded)
                {
                    ReloadData(true);
                }
            }
        }
    }


    /// <summary>
    /// Code name of form control that manages rating scale.
    /// </summary>
    public string RatingType { get; set; } = "Stars";


    /// <summary>
    /// If true the brief result info is shown.
    /// </summary>
    public bool ShowResultMessage { get; set; }


    /// <summary>
    /// Gets or sets result info message that is displayed after rating.
    /// </summary>
    public string ResultMessage { get; set; }


    /// <summary>
    /// Gets or sets message that is displayed after rating.
    /// </summary>
    public string MessageAfterRating { get; set; }


    /// <summary>
    /// Gets or sets message that is displayed when user forgot to rate.
    /// </summary>
    public string ErrorMessage { get; set; }


    /// <summary>
    /// Gets or sets value that indicates whether rating is allowed for public users.
    /// </summary>
    public bool AllowForPublic { get; set; } = true;


    /// <summary>
    /// Enables/disables checking if user voted.
    /// </summary>
    public bool CheckIfUserRated { get; set; } = true;


    /// <summary>
    /// If true, the control hides when user is not authorized.
    /// </summary>
    public bool HideToUnauthorizedUsers { get; set; }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
    /// </summary>
    public bool CheckPermissions { get; set; } = true;


    /// <summary>
    /// Enables/disables rating control 
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData(false);
    }


    /// <summary>
    /// Occurs on rating event.
    /// </summary>
    protected void usrControl_RatingEvent(AbstractRatingControl sender)
    {
        // Check if control is enabled
        if (!(Enabled && HasPermissions() && !(CheckIfUserRated && TreeProvider.HasRated(DocumentContext.CurrentDocument))))
        {
            return;
        }

        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            pnlError.Visible = true;
            lblError.Text = GetString("general.bannedip");
            return;
        }

        // Check null value
        if (!AllowZeroValue && usrControl.CurrentRating <= 0)
        {
            pnlError.Visible = true;
            lblError.Text = ErrorMessage;
            return;
        }

        if (DocumentContext.CurrentDocument != null)
        {
            // Check whether user has already rated
            if (CheckIfUserRated && TreeProvider.HasRated(DocumentContext.CurrentDocument))
            {
                return;
            }

            // Update document rating, remember rating in cookie if required
            TreeProvider.AddRating(DocumentContext.CurrentDocument, usrControl.CurrentRating, CheckIfUserRated);

            // Get absolute rating value of the current rating
            double currRating = usrControl.MaxRating * usrControl.CurrentRating;
            // Reload rating control
            ReloadData(true);
            // Show message after rating if enabled or set
            if (!string.IsNullOrEmpty(MessageAfterRating))
            {
                pnlMessage.Visible = true;
                // Merge message text with rating values
                lblMessage.Text = String.Format(MessageAfterRating,
                                                Convert.ToInt32(currRating), usrControl.CurrentRating * usrControl.MaxRating, DocumentContext.CurrentDocument.DocumentRatings);
            }
            else
            {
                pnlMessage.Visible = false;
            }
        }
    }


    /// <summary>
    /// Reload all values.
    /// </summary>
    public void ReloadData()
    {
        ReloadData(false);
    }


    /// <summary>
    /// Reload all values.
    /// </summary>
    /// <param name="forceReload">Forces reload</param>
    public void ReloadData(bool forceReload)
    {
        if (StopProcessing || (loaded && !forceReload))
        {
            return;
        }

        // Check permissions
        if (HideToUnauthorizedUsers && !HasPermissions())
        {
            Visible = false;
            return;
        }

        if (DocumentContext.CurrentDocument != null)
        {
            try
            {
                // Insert rating control to page
                usrControl = (AbstractRatingControl)(Page.LoadUserControl(AbstractRatingControl.GetRatingControlUrl(RatingType + ".ascx")));
            }
            catch (Exception e)
            {
                Controls.Add(new LiteralControl(e.Message));
                return;
            }

            double rating = 0.0f;

            // Use current document rating if external value is not used
            if (mExternalValue < 0)
            {
                if (DocumentContext.CurrentDocument.DocumentRatings > 0)
                {
                    rating = DocumentContext.CurrentDocument.DocumentRatingValue / DocumentContext.CurrentDocument.DocumentRatings;
                }
            }
            else
            {
                rating = mExternalValue;
            }

            if ((rating < 0.0) || (rating > 1.0))
            {
                rating = 0.0;
            }

            usrControl.ID = "RatingControl";
            usrControl.MaxRating = MaxRatingValue;
            usrControl.CurrentRating = rating;
            usrControl.Visible = true;
            usrControl.Enabled = Enabled && HasPermissions() && !(CheckIfUserRated && TreeProvider.HasRated(DocumentContext.CurrentDocument));

            RefreshResultMessage();

            usrControl.RatingEvent += new AbstractRatingControl.OnRatingEventHandler(usrControl_RatingEvent);
            pnlRating.Controls.Clear();
            pnlRating.Controls.Add(usrControl);

            loaded = true;
        }
    }


    /// <summary>
    /// Refreshes result info message.
    /// </summary>
    private void RefreshResultMessage()
    {
        if (ShowResultMessage && (!string.IsNullOrEmpty(ResultMessage)))
        {
            pnlResult.Visible = true;
            try
            {
                // Merge result text with rating values
                lblResult.Text = String.Format(ResultMessage, usrControl.CurrentRating * usrControl.MaxRating, DocumentContext.CurrentDocument.DocumentRatings);
            }
            catch (Exception ex)
            {
                Service.Resolve<IEventLogService>().LogException("Rating control", "SHOWRESULT", ex);
                pnlResult.Visible = false;
            }
        }
        else
        {
            pnlResult.Visible = false;
        }
    }


    /// <summary>
    /// Returns true if user has permissions to access to the rating control.
    /// </summary>
    private bool HasPermissions()
    {
        if (!CheckPermissions || MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            return true;
        }

        if (AllowForPublic && MembershipContext.AuthenticatedUser.IsPublic())
        {
            return true;
        }

        if (!MembershipContext.AuthenticatedUser.IsPublic())
        {
            return true;
        }

        return false;
    }

    #endregion
}