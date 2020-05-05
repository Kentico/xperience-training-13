using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Forums_ForumBreadcrumbs : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the breadcrumbs separator.
    /// </summary>
    public string BreadcrumbsSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbsSeparator"), ctrlForumBreadcrumbs.BreadcrumbSeparator);
        }
        set
        {
            SetValue("BreadcrumbsSeparator", value);
            ctrlForumBreadcrumbs.BreadcrumbSeparator = value;
        }
    }


    /// <summary>
    /// Gets or sets the breadcrumbs prefix.
    /// </summary>
    public string BreadcrumbPrefix
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbPrefix"), ctrlForumBreadcrumbs.BreadcrumbPrefix);
        }
        set
        {
            SetValue("BreadcrumbPrefix", value);
            ctrlForumBreadcrumbs.BreadcrumbPrefix = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether current item should be rendered as link.
    /// </summary>
    public bool UseLinkForCurrentItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseLinkForCurrentItem"), ctrlForumBreadcrumbs.UseLinkForCurrentItem);
        }
        set
        {
            SetValue("UseLinkForCurrentItem", value);
            ctrlForumBreadcrumbs.UseLinkForCurrentItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether forum group should be displayed in breadcrumbs.
    /// </summary>
    public bool DisplayGroup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayGroup"), ctrlForumBreadcrumbs.DisplayGroup);
        }
        set
        {
            SetValue("DisplayGroup", value);
            ctrlForumBreadcrumbs.DisplayGroup = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether thread name should be displayed in breadcrumbs.
    /// </summary>
    public bool DisplayThread
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayThread"), ctrlForumBreadcrumbs.DisplayThread);
        }
        set
        {
            SetValue("DisplayThread", value);
            ctrlForumBreadcrumbs.DisplayThread = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether threads names should be displayed in breadcrumbs.
    /// </summary>
    public bool DisplayThreads
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayThreads"), ctrlForumBreadcrumbs.DisplayThreads);
        }
        set
        {
            SetValue("DisplayThreads", value);
            ctrlForumBreadcrumbs.DisplayThreads = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether breadcrumbs should be hidden on forum group page
    /// This option hides only forum breadcrumbs, breadcrumbs prefix is allways visible if is defined
    /// </summary>
    public bool HideBreadcrumbsOnForumGroupPage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideBreadcrumbsOnForumGroupPage"), ctrlForumBreadcrumbs.HideBreadcrumbsOnForumGroupPage);
        }
        set
        {
            SetValue("HideBreadcrumbsOnForumGroupPage", value);
            ctrlForumBreadcrumbs.HideBreadcrumbsOnForumGroupPage = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether friendly URL should be used.
    /// </summary>
    public bool UseFriendlyURL
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseFriendlyURL"), ctrlForumBreadcrumbs.UseFriendlyURL);
        }
        set
        {
            SetValue("UseFriendlyURL", value);
            ctrlForumBreadcrumbs.UseFriendlyURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the forum base URL without extension.
    /// </summary>
    public string FriendlyBaseURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FriendlyBaseURLs"), ctrlForumBreadcrumbs.FriendlyBaseURL);
        }
        set
        {
            SetValue("FriendlyBaseURLs", value);
            ctrlForumBreadcrumbs.FriendlyBaseURL = value;
        }
    }


    /// <summary>
    /// Gets or sets the friendly URL extension. For extension less URLs sets it to empty string.
    /// </summary>
    public string FriendlyURLExtension
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FriendlyURLExtension"), ctrlForumBreadcrumbs.FriendlyURLExtension);
        }
        set
        {
            SetValue("FriendlyURLExtension", value);
            ctrlForumBreadcrumbs.FriendlyURLExtension = value;
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
            // Do nothing
        }
        else
        {
            ctrlForumBreadcrumbs.BreadcrumbSeparator = BreadcrumbsSeparator;
            ctrlForumBreadcrumbs.BreadcrumbPrefix = BreadcrumbPrefix;
            ctrlForumBreadcrumbs.UseLinkForCurrentItem = UseLinkForCurrentItem;
            ctrlForumBreadcrumbs.HideBreadcrumbsOnForumGroupPage = HideBreadcrumbsOnForumGroupPage;

            ctrlForumBreadcrumbs.DisplayGroup = DisplayGroup;
            ctrlForumBreadcrumbs.DisplayThread = DisplayThread;
            ctrlForumBreadcrumbs.DisplayThreads = DisplayThreads;

            ctrlForumBreadcrumbs.UseFriendlyURL = UseFriendlyURL;
            ctrlForumBreadcrumbs.FriendlyBaseURL = FriendlyBaseURL;
            ctrlForumBreadcrumbs.FriendlyURLExtension = FriendlyURLExtension;
        }
    }

    #endregion
}