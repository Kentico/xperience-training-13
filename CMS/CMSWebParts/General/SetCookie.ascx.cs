using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_General_SetCookie : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Name of the cookie.
    /// </summary>
    public string Name
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Name"), "");
        }
        set
        {
            SetValue("Name", value);
        }
    }


    /// <summary>
    /// Value of the cookie.
    /// </summary>
    public string CookieValue
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CookieValue"), "");
        }
        set
        {
            SetValue("CookieValue", value);
        }
    }


    /// <summary>
    /// Expiration time of the cookie.
    /// </summary>
    public DateTime Expiration
    {
        get
        {
            return ValidationHelper.GetDateTimeSystem(GetValue("Expiration"), DateTime.MaxValue);
        }
        set
        {
            SetValue("Expiration", value);
        }
    }


    /// <summary>
    /// The virtual path to transmit with the cookie.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), "");
        }
        set
        {
            SetValue("Path", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Loads the web part content.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Sets up the control.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (PortalContext.ViewMode.IsLiveSite())
            {
                // Length of the string in the cookie, because size of the cookie can't be more than 4KB
                int length = Name.Length + CookieValue.Length + Path.Length + Expiration.ToString().Length;

                if ((Name != "") && (length < 4000))
                {
                    CookieHelper.SetValue(Name, CookieValue, Path, Expiration);
                }
            }
        }
    }

    #endregion
}