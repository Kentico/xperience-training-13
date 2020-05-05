using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_PageElements_Help : HelpControl
{
    #region "Constants"

    /// <summary>
    /// Default icon.
    /// </summary>
    public const string ICON_DEFAULT = "icon-question-circle";


    /// <summary>
    /// Icon for modal dialogs.
    /// </summary>
    public const string ICON_MODAL = "icon-modal-question";

    #endregion


    #region "Variables"

    protected string mTopicName = "";
    protected string mHelpName = null;
    protected string mIconName = null;
    protected string mIconCssClass = "";

    #endregion


    #region "Public properties"


    /// <summary>
    /// Topic name can be either a perma link, or absolute URL.
    /// </summary>
    public override string TopicName
    {
        get
        {
            return mTopicName;
        }
        set
        {
            mTopicName = value;
        }
    }


    /// <summary>
    /// Help name to identify the help within the javascript.
    /// </summary>
    public override string HelpName
    {
        get
        {
            return mHelpName ?? ID;
        }
        set
        {
            mHelpName = value;
        }
    }


    /// <summary>
    /// Tooltip.
    /// </summary>
    public override string Tooltip
    {
        get
        {
            if (ViewState["Tooltip"] == null)
            {
                if (DatabaseHelper.IsDatabaseAvailable)
                {
                    return GetString("Help.Tooltip");
                }
                else
                {
                    return ResHelper.GetFileString("Help.Tooltip");
                }
            }
            return (string)ViewState["Tooltip"];
        }
        set
        {
            ViewState["Tooltip"] = value;
        }
    }


    /// <summary>
    /// Icon name. For other icon CSS classes use <see cref="IconCssClass"/>
    /// When not set, the icon name is chosen based on <see cref="HelpControl.IsDialog"/> property.
    /// </summary>
    /// <seealso cref="IconCssClass"/>
    public override string IconName
    {
        get
        {
            return mIconName ?? (IsDialog ? ICON_MODAL : ICON_DEFAULT);
        }
        set
        {
            mIconName = value;
        }
    }


    /// <summary>
    /// Gets or sets the CSS class assigned to the icon.
    /// </summary>
    /// <seealso cref="IconName"/>
    public override string IconCssClass
    {
        get
        {
            return mIconCssClass;
        }
        set
        {
            mIconCssClass = value;
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        iconHelp.Attributes["class"] = IconName + " " + IconCssClass;
        iconHelp.Attributes["title"] = Tooltip;

        if (!String.IsNullOrEmpty(TopicName))
        {
            lnkHelp.NavigateUrl = DocumentationHelper.GetDocumentationTopicUrl(TopicName);
        }

        // Render help name script
        if (!String.IsNullOrEmpty(HelpName))
        {
            object options = new
            {
                helpName = HelpName,
                helpLinkId = lnkHelp.ClientID,
                helpHidden = String.IsNullOrEmpty(TopicName)
            };
            ScriptHelper.RegisterModule(this, "CMS/DialogContextHelp", options);
        }
    }

}
