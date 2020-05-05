using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;

public partial class CMSWebParts_General_Timer : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Refresh interval.
    /// </summary>
    public int Interval
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Interval"), 1000);
        }
        set
        {
            SetValue("Interval", value);
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
            if (PortalContext.ViewMode.IsOneOf(ViewModeEnum.LiveSite, ViewModeEnum.Preview))
            {
                if (timElem.Interval > 0)
                {
                    timElem.Interval = Interval;
                }
                else
                {
                    timElem.Enabled = false;
                }
            }
        }
    }

    #endregion


    #region "Control events"

    protected void timElem_Tick(object sender, EventArgs e)
    {
        // Do nothing
    }

    #endregion
}