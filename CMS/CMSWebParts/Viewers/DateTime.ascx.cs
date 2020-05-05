using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_DateTime : CMSAbstractWebPart
{
    #region "Javascript properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to use server time or not.
    /// </summary>
    public bool JsUseServerTime
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("JsUseServerTime"), false);
        }
        set
        {
            SetValue("JsUseServerTime", value);
        }
    }


    /// <summary>
    /// Gets or sets the date time format (ie. "dd.mm.yy").
    /// </summary>
    public string JsFormat
    {
        get
        {
            return ValidationHelper.GetString(GetValue("JsFormat"), "dd.m.yy");
        }
        set
        {
            SetValue("JsFormat", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        // If Content Slider is in design mode do not start scripts (IE z-index problem)
        if (PortalContext.ViewMode != ViewModeEnum.Design)
        {
            ltlDateTime.Text = "<div id=\"time_" + ClientID + "\" ></div>";

            // Register DateTime.js script file
            ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Viewers/DateTime_files/DateTime.js");

            string jScript = @"
function dt_attachLoadEvent(callBack) {
	if (window.addEventListener) {                    
        window.addEventListener('load', callBack);
    } else if (window.attachEvent) {                  
        window.attachEvent('onload', callBack);
    }
}

dt_attachLoadEvent(function() {
	var now = new Date();
	var diff = 0;
";
            if (JsUseServerTime)
            {
                jScript += @"
var serverTime = " + Math.Round((DateTime.Now.ToUniversalTime() - DateTimeHelper.UNIX_TIME_START).TotalMilliseconds, 0, MidpointRounding.AwayFromZero) + @";
diff = serverTime - now.getTime();
";
            }

            jScript += @"
startTimer('" + ClientID + @"','" + JsFormat + @"',diff);
});
";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ("timerScript" + ClientID), ScriptHelper.GetScript(jScript));
        }
        else
        {
            ltlDateTime.Text = "<div id=\"time_" + ClientID + "\" >Timer</div>";
        }
    }

    #endregion
}
