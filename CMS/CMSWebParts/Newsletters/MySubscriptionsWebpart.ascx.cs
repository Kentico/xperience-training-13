using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Newsletters_MySubscriptionsWebpart : CMSAbstractWebPart
{
    #region "Public Properties"

    /// <summary>
    /// Indicates whether send emails when (un)subscribed.
    /// </summary>
    public bool SendConfirmationEmails
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendConfirmationEmails"), true);
            ;
        }
        set
        {
            SetValue("SendConfirmationEmails", value);
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
            // Stop processing
            ucMySubsriptions.StopProcessing = true;
        }
        else
        {
            ucMySubsriptions.ControlContext = ControlContext;

            ucMySubsriptions.CacheMinutes = CacheMinutes;
            ucMySubsriptions.ExternalUse = true;
            ucMySubsriptions.SendConfirmationEmail = SendConfirmationEmails;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
        ucMySubsriptions.ExternalUse = false;
        ucMySubsriptions.LoadData();
    }

    #endregion
}