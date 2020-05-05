using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_ContentRating_ContentRating : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets max value of scale.
    /// </summary>
    public int MaxRatingValue
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRatingValue"), elemRating.MaxRatingValue);
        }
        set
        {
            SetValue("MaxRatingValue", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether unrated value is allowed.
    /// </summary>
    public bool AllowZeroValue
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowZeroValue"), elemRating.AllowZeroValue);
        }
        set
        {
            SetValue("AllowZeroValue", value);
        }
    }


    /// <summary>
    /// Gets or sets current value. If value is negative number then document
    /// rating is used.
    /// </summary>
    public string ExternalValue
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExternalValue"), elemRating.ExternalValue);
        }
        set
        {
            SetValue("ExternalValue", value);
        }
    }


    /// <summary>
    /// Code name of control that manages rating scale.
    /// </summary>
    public string RatingType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RatingType"), elemRating.RatingType);
        }
        set
        {
            SetValue("RatingType", value);
        }
    }


    /// <summary>
    /// If true the brief result info is shown.
    /// </summary>
    public bool ShowResultMessage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowResultMessage"), elemRating.ShowResultMessage);
        }
        set
        {
            SetValue("ShowResultMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets result info message.
    /// </summary>
    public string ResultMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ResultMessage"), elemRating.ResultMessage);
        }
        set
        {
            SetValue("ResultMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets message that is displayed after rating.
    /// </summary>
    public string MessageAfterRating
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MessageAfterRating"), elemRating.MessageAfterRating);
        }
        set
        {
            SetValue("MessageAfterRating", value);
        }
    }


    /// <summary>
    /// Gets or sets message that is displayed when user forgot to rate.
    /// </summary>
    public string ErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ErrorMessage"), elemRating.ErrorMessage);
        }
        set
        {
            SetValue("ErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether rating is allowed for public users.
    /// </summary>
    public bool AllowForPublic
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowForPublic"), elemRating.AllowForPublic);
        }
        set
        {
            SetValue("AllowForPublic", value);
        }
    }


    /// <summary>
    /// Enables/disables checking if user rated.
    /// </summary>
    public bool CheckIfUserRated
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckIfUserRated"), elemRating.CheckIfUserRated);
        }
        set
        {
            SetValue("CheckIfUserRated", value);
        }
    }


    /// <summary>
    /// If true, the control is hidden when user is not authorized.
    /// </summary>
    public bool HideToUnauthorizedUsers
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideToUnauthorizedUsers"), elemRating.HideToUnauthorizedUsers);
        }
        set
        {
            SetValue("HideToUnauthorizedUsers", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that determines whether permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), elemRating.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            elemRating.CheckPermissions = value;
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
            elemRating.StopProcessing = true;
        }
        else
        {
            elemRating.MaxRatingValue = MaxRatingValue;
            elemRating.RatingType = RatingType;
            elemRating.ShowResultMessage = ShowResultMessage;
            elemRating.ResultMessage = ResultMessage;
            elemRating.MessageAfterRating = MessageAfterRating;
            elemRating.AllowForPublic = AllowForPublic;
            elemRating.CheckIfUserRated = CheckIfUserRated;
            elemRating.HideToUnauthorizedUsers = HideToUnauthorizedUsers;
            elemRating.CheckPermissions = CheckPermissions;
            elemRating.ExternalValue = ExternalValue;
            elemRating.AllowZeroValue = AllowZeroValue;
            elemRating.ErrorMessage = ErrorMessage;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    #endregion
}