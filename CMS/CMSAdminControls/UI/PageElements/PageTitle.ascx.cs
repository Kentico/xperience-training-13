using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.UIControls;
using CMS.Helpers;

public partial class CMSAdminControls_UI_PageElements_PageTitle : PageTitle
{
    #region "Public properties"

    /// <summary>
    /// Breadcrumbs control
    /// </summary>
    public override Breadcrumbs Breadcrumbs
    {
        get
        {
            return breadcrumbs;
        }
    }


    /// <summary>
    /// Title CSS class.
    /// </summary>
    public override string TitleCssClass
    {
        get
        {
            return pnlTitle.CssClass;
        }
        set
        {
            pnlTitle.CssClass = value;
        }
    }


    /// <summary>
    /// Topic name can be either a perma link, or absolute URL.
    /// </summary>
    public override string HelpTopicName
    {
        get
        {
            return helpElem.TopicName;
        }
        set
        {
            helpElem.TopicName = value;
            breadcrumbs.Help.TopicName = value;
        }
    }


    /// <summary>
    /// Help name to identify the help within the javascript.
    /// </summary>
    public override string HelpName
    {
        get
        {
            return helpElem.HelpName;
        }
        set
        {
            helpElem.HelpName = value;
            breadcrumbs.Help.HelpName = value;
        }
    }


    /// <summary>
    /// Help icon name for title.
    /// </summary>
    public override string HelpIconName
    {
        get
        {
            return helpElem.IconName;
        }
        set
        {
            helpElem.IconName = value;
        }
    }


    /// <summary>
    /// Placeholder after image and title text.
    /// </summary>
    public override PlaceHolder RightPlaceHolder
    {
        get
        {
            return plcMisc;
        }
        set
        {
            plcMisc = value;
        }
    }

    #endregion


    #region "Dialog properties"

    /// <summary>
    /// Indicates if the control should use the string from resource file.
    /// </summary>
    public bool UseFileStrings
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Register jQuery
        ScriptHelper.RegisterJQuery(Page);

        // Use dark help icon for dialogs
        helpElem.IsDialog = IsDialog;

        // Set breadcrumbs visibility
        breadcrumbs.HideBreadcrumbs = HideBreadcrumbs;

        // Set level of h element
        headTitle.Level = HeadingLevel;

        // Set the title text if set
        if (!string.IsNullOrEmpty(TitleText))
        {
            if (!HideTitle)
            {
                pnlTitle.Visible = true;
                headTitle.Text = TitleText;
            }
        }
        else
        {
            pnlTitle.Visible = false;
            breadcrumbs.Help.Visible = true;
        }

        // Set the title info if set
        if (!string.IsNullOrEmpty(TitleInformation))
        {
            lblTitleInfo.Text = TitleInformation;
            lblTitleInfo.Visible = true;
        }

        // Register scripts only when needed
        if (pnlTitle.Visible)
        {
            EnsureFullScreenButton();

            EnsureCloseButton();

            EnsureDraggable();
        }

        if (!Wrap)
        {
            headTitle.Style.Add("white-space", "nowrap");
        }
    }

    #endregion


    #region "Methods"

    private void EnsureDraggable()
    {
        // Load draggable iframe script
        ScriptHelper.RegisterScriptFile(Page, "DragAndDrop/dragiframe.js");

        ScriptHelper.RegisterGetTopScript(Page);

        // Initialize draggable behavior
        ScriptHelper.RegisterStartupScript(this, typeof (string), "draggableFrame", ScriptHelper.GetScript(
@"$cmsj(window).load(function(){
var topFrame = GetTop();
if(window.wopener)
{
    if((top.isTitleWindow) && top.isTitleWindow(topFrame, window))
    {
        addHandle(document.getElementById('" + pnlTitle.ClientID + @"'), window);
    }
}
});"));
    }


    private void EnsureFullScreenButton()
    {
        if (ShowFullScreenButton && IsDialog)
        {
            pnlMaximize.Visible = true;
            btnMaximize.Attributes.Add("onclick", "return fs_" + ClientID + "($cmsj(this));");
            btnMaximize.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            string fsToolTip = GetString("general.fullscreen");
            btnMaximize.Attributes.Add("title", fsToolTip);

            string fullScreenScript = @"
function titleFullScreen(sender)
{
    if(top.toggleFullScreen)
    {
        top.toggleFullScreen();
        toggleFullScreenIcon(sender);
    }
    return false;
}

function toggleFullScreenIcon(sender)
{
    var btn = sender;
    btn.toggleClass('icon-modal-maximize');
    btn.toggleClass('icon-modal-minimize');
    if(btn.hasClass('icon-modal-minimize'))
    {
        sender.attr('title', " + ScriptHelper.GetString(GetString("general.restore")) + @");
    }
    else
    {
        sender.attr('title', '" + fsToolTip + @"');
    }
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "fullScreenScript", ScriptHelper.GetScript(fullScreenScript));

            // Register fullscreen button for new dialogs
            string fsVar = @"
function fs_" + ClientID + @"(sender)
{
    return titleFullScreen(sender);
}";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "fsVar_" + ClientID, ScriptHelper.GetScript(fsVar));

            string fsInit = @"
$cmsj(document).ready(function(){
    if(window.wopener && (top != null) && top.$visiblePopup)
    {  
        var topFrame = GetTop();
        if(top.isTitleWindow(topFrame, window) && !topFrame.fullScreenButtonAvailable)
        {
            var fsButton = $cmsj('#" + btnMaximize.ClientID + @"');
            if(top.isFullScreen())
            {
                toggleFullScreenIcon(fsButton);
            }
            fsButton.show(); 
            $cmsj('#" + pnlBody.ClientID + @"').dblclick(function(){return fs_" + ClientID + @"(fsButton);});
            topFrame.setToFullScreen = function () { return !top.isFullScreen() ? fs_" + ClientID + @"(fsButton) : false; };
            topFrame.fullScreenButtonAvailable = true;
            $cmsj(window).unload(function() {
                topFrame.fullScreenButtonAvailable = false;
            });
        }
    }
});";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "fsInit_" + ClientID, ScriptHelper.GetScript(fsInit));
        }
    }


    private void EnsureCloseButton()
    {
        if (ShowCloseButton && IsDialog)
        {
            pnlClose.Visible = true;
            btnClose.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");
            btnClose.Attributes.Add("title", GetString("general.close"));

            // Always close the window
            btnClose.Attributes["onclick"] += ";return CloseDialog();";
        }
    }


    /// <summary>
    /// Returns localized string.
    /// </summary>
    /// <param name="stringName">String to localize</param>
    /// <param name="culture">Culture</param>
    public override string GetString(string stringName, string culture = null)
    {
        if (UseFileStrings || !ConnectionHelper.ConnectionAvailable)
        {
            return ResHelper.GetFileString(stringName, culture);
        }

        return base.GetString(stringName, culture);
    }

    #endregion


    public void SetCloseJavaScript(string javaScript)
    {
        btnClose.Attributes.Add("onclick", javaScript);
    }
}
