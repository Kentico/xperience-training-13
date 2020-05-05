using System;

using CMS.PortalEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;

public partial class CMSWebParts_MessageBoards_MessageBoardSubscriptionApproval : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Information text
    /// </summary>
    public string ConfirmationInfoText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConfirmationInfoText"), "");
        }
        set
        {
            SetValue("ConfirmationInfoText", value);
        }
    }


    /// <summary>
    /// Confirmation text CSS class
    /// </summary>
    public string ConfirmationTextCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConfirmationTextCssClass"), "");
        }
        set
        {
            SetValue("ConfirmationTextCssClass", value);
        }
    }


    /// <summary>
    /// Confirmation button text
    /// </summary>
    public string ConfirmationButtonText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConfirmationButtonText"), "");
        }
        set
        {
            SetValue("ConfirmationButtonText", value);
        }
    }


    /// <summary>
    /// Confirmation button CSS class
    /// </summary>
    public string ConfirmationButtonCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConfirmationButtonCssClass"), "");
        }
        set
        {
            SetValue("ConfirmationButtonCssClass", value);
        }
    }


    /// <summary>
    /// Successful confirmation text
    /// </summary>
    public string SuccessfulConfirmationText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SuccessfulConfirmationText"), "Your subscription was confirmed successfully.");
        }
        set
        {
            SetValue("SuccessfulConfirmationText", value);
        }
    }


    /// <summary>
    /// Unsuccessful confirmation text
    /// </summary>
    public string UnsuccessfulConfirmationText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UnsuccessfulConfirmationText"), "Subscription confirmation was unsuccessful.");
        }
        set
        {
            SetValue("UnsuccessfulConfirmationText", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        subscriptionApproval.StopProcessing = (ViewMode != ViewModeEnum.LiveSite);
        subscriptionApproval.StopProcessing = string.IsNullOrEmpty(QueryHelper.GetString("hash", string.Empty));
    }


    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
            subscriptionApproval.StopProcessing = true;
        }
        else
        {
            string subscription = QueryHelper.GetString("boardsubscriptionhash", string.Empty);

            if (!string.IsNullOrEmpty(subscription))
            {
                subscriptionApproval.SuccessfulConfirmationText = SuccessfulConfirmationText;
                subscriptionApproval.UnsuccessfulConfirmationText = UnsuccessfulConfirmationText;
                subscriptionApproval.ConfirmationInfoText = ConfirmationInfoText;
                subscriptionApproval.ConfirmationTextCssClass = ConfirmationTextCssClass;
                subscriptionApproval.ConfirmationButtonText = ConfirmationButtonText;
                subscriptionApproval.ConfirmationButtonCssClass = ConfirmationButtonCssClass;
            }
            else
            {
                Visible = false;
            }
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}