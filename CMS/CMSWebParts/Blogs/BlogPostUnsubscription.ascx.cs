using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Blogs;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Blogs_BlogPostUnsubscription : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Unsubscription info text
    /// </summary>
    public string UnsubscriptionInfoText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionInfoText"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionInfoText", value);
        }
    }


    /// <summary>
    /// Unsubscription CSS class
    /// </summary>
    public string UnsubscriptionInfoCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionInfoCssClass"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionInfoCssClass", value);
        }
    }


    /// <summary>
    /// Unsubscription button text
    /// </summary>
    public string UnsubscriptionButtonText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionButtonText"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionButtonText", value);
        }
    }


    /// <summary>
    /// Unsubscription button CSS class
    /// </summary>
    public string UnsubscriptionButtonCssClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionButtonCssClass"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionButtonCssClass", value);
        }
    }


    /// <summary>
    /// Unsubscription text
    /// </summary>
    public string UnsubscriptionText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsubscriptionText"), "");
        }
        set
        {
            this.SetValue("UnsubscriptionText", value);
        }
    }


    /// <summary>
    /// Unsuccessful unsubscription text
    /// </summary>
    public string UnsuccessfulUnsubscriptionText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("UnsuccessfulUnsubscriptionText"), "");
        }
        set
        {
            this.SetValue("UnsuccessfulUnsubscriptionText", value);
        }
    }

    #endregion


    #region "Methods"

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
        if (this.StopProcessing)
        {
            unsubscription.StopProcessing = true;
        }
        else
        {
            unsubscription.SuccessfulUnsubscriptionText = UnsubscriptionText;
            unsubscription.UnsuccessfulUnsubscriptionText = UnsuccessfulUnsubscriptionText;
            unsubscription.UnsubscriptionInfoText = UnsubscriptionInfoText;
            unsubscription.UnsubscriptionInfoCssClass = UnsubscriptionInfoCssClass;
            unsubscription.UnsubscriptionButtonText = UnsubscriptionButtonText;
            unsubscription.UnsubscriptionButtonCssClass = UnsubscriptionButtonCssClass;
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