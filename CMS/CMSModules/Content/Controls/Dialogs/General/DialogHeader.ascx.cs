using System;

using AngleSharp.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_General_DialogHeader : CMSUserControl
{
    #region "Variables"

    private CurrentUserInfo currentUser;
    private string mSelectedTab = "";
    private int mSelectedTabIndex;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Tabs header master.
    /// </summary>
    public ICMSMasterPage CurrentMaster
    {
        get;
        set;
    }


    /// <summary>
    /// Selected tab: attachments, content, libraries, web, anchor, email.
    /// </summary>
    public string SelectedTab
    {
        get
        {
            return mSelectedTab;
        }
        set
        {
            if (value != null)
            {
                mSelectedTab = value.ToLowerCSafe();
            }
        }
    }


    /// <summary>
    /// Custom output format (used when OuptupFormat is set to Custom).
    /// </summary>
    public string CustomOutputFormat { get; set; } = String.Empty;


    /// <summary>
    /// CMS dialog output format which determines dialog title, image and visible tabs.
    /// </summary>
    public OutputFormatEnum OutputFormat { get; set; }


    /// <summary>
    /// Type of content which could be selected from the dialog.
    /// </summary>
    public SelectableContentEnum SelectableContent { get; set; } = SelectableContentEnum.AllContent;


    /// <summary>
    /// Indicates if 'Attachments' tab should be hidden.
    /// </summary>
    public bool HideAttachments
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Content' tab should be hidden.
    /// </summary>
    public bool HideContent
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Media libraries' tab should be hidden.
    /// </summary>
    public bool HideMediaLibraries
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Web' tab should be hidden.
    /// </summary>
    public bool HideWeb
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'Anchor' tab should be hidden.
    /// </summary>
    public bool HideAnchor
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if 'E-mail' tab should be hidden.
    /// </summary>
    public bool HideEmail
    {
        get;
        set;
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns if query string contains data for identification of the object the metafile attachments are related to.
    /// </summary>
    private bool HasMetaFileObjectInfo
    {
        get
        {
            return QueryHelper.GetInteger("objectid", 0) > 0
                && !String.IsNullOrEmpty(QueryHelper.GetString("objecttype", String.Empty))
                && !String.IsNullOrEmpty(QueryHelper.GetString("objectcategory", String.Empty));
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
        }
        else
        {
            currentUser = MembershipContext.AuthenticatedUser;

            ReloadHeader();
        }
    }

    #endregion


    #region "Public methods"

    public void InitFromQueryString()
    {
        // Get format definition from URL
        string output = QueryHelper.GetString("output", "html");
        bool link = QueryHelper.GetBoolean("link", false);
        OutputFormat = CMSDialogHelper.GetOutputFormat(output, link);
        if (OutputFormat == OutputFormatEnum.Custom)
        {
            CustomOutputFormat = output;
        }

        // Get selectable content
        string content = QueryHelper.GetString("content", "all");
        SelectableContent = CMSDialogHelper.GetSelectableContent(content);

        // Get user dialog configuration
        XmlData userConfig = MembershipContext.AuthenticatedUser.UserSettings.UserDialogsConfiguration;

        // Get selected tab from URL
        SelectedTab = QueryHelper.GetString("tab", (string)userConfig["selectedtab"]);

        // Get hidden tabs from URL
        bool hasFormGuid = (QueryHelper.GetGuid("formguid", Guid.Empty) != Guid.Empty);
        bool hasDocumentId = (QueryHelper.GetInteger("documentid", 0) > 0);
        bool hasParentId = (QueryHelper.GetInteger("parentid", 0) > 0);

        HideAttachments = QueryHelper.GetBoolean("attachments_hide", false) || !((hasFormGuid && hasParentId) || hasDocumentId || HasMetaFileObjectInfo);

        HideContent = QueryHelper.GetBoolean("content_hide", false) || SelectableContent != SelectableContentEnum.AllContent;
        if (!HideContent)
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Content", SiteContext.CurrentSiteName))
            {
                HideContent = true;
            }
        }

        HideMediaLibraries = QueryHelper.GetBoolean("libraries_hide", false);
        if (!HideMediaLibraries)
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", SiteContext.CurrentSiteName))
            {
                HideMediaLibraries = true;
            }
        }

        HideWeb = QueryHelper.GetBoolean("web_hide", false);
        HideAnchor = QueryHelper.GetBoolean("anchor_hide", false);
        HideEmail = QueryHelper.GetBoolean("email_hide", false);
    }


    public void ReloadHeader()
    {
        if (CurrentMaster != null)
        {
            var pt = CurrentMaster.Title;

            pt.TitleText = GetTitleText();

            CurrentMaster.PanelLeft.CssClass = "FullTabsLeft";

            GetTabs();

            var tabs = CurrentMaster.Tabs;

            tabs.OpenTabContentAfterLoad = false;
            tabs.SelectedTab = mSelectedTabIndex;
            tabs.UrlTarget = "insertContent";
        }
    }

    #endregion


    #region "Private methods"

    private string GetTitleText()
    {
        string result = "";

        // Insert link
        if ((OutputFormat == OutputFormatEnum.BBLink) ||
            (OutputFormat == OutputFormatEnum.HTMLLink))
        {
            result = GetString("dialogs.header.title.link");
        }

        // Insert image or media
        else if (OutputFormat == OutputFormatEnum.HTMLMedia)
        {
            result = GetString("dialogs.header.title.imagemedia");
        }
        else if (OutputFormat == OutputFormatEnum.URL)
        {
            if (SelectableContent == SelectableContentEnum.OnlyImages)
            {
                result = GetString("dialogs.header.title.selectimage");
            }
            else if (SelectableContent == SelectableContentEnum.AllFiles)
            {
                result = GetString("dialogs.header.title.selectimagemedia");
            }
            else
            {
                result = GetString("dialogs.header.title.selectlink");
            }
        }

        // Insert image
        else if (OutputFormat == OutputFormatEnum.BBMedia)
        {
            result = GetString("dialogs.header.title.image");
        }
        else if (OutputFormat == OutputFormatEnum.Custom)
        {
            switch (CustomOutputFormat.ToLowerCSafe())
            {
                case "copy":
                    result = GetString("dialogs.header.title.copydoc");
                    break;

                case "move":
                    result = GetString("dialogs.header.title.movedoc");
                    break;

                case "link":
                    result = GetString("dialogs.header.title.linkdoc");
                    break;

                case "linkdoc":
                    result = GetString("dialogs.header.title.linkdoc");
                    break;

                case "relationship":
                    result = GetString("selectlinkdialog.title");
                    break;

                case "selectpath":
                    result = GetString("dialogs.header.title.selectpath");
                    break;
            }
        }

        return result;
    }


    /// <summary>
    /// Returns path to the specified tab page.
    /// </summary>
    /// <param name="fileName">File name of the tab page</param>
    /// <param name="parameterName">Additional parameter name</param>
    /// <param name="parameterValue">Additional parameter value</param>
    private string GetFilePath(string fileName, string parameterName = null, string parameterValue = null)
    {
        string path = "~/CMSFormControls/Selectors/InsertImageOrMedia/";

        if ((!String.IsNullOrEmpty(parameterName)) && (!String.IsNullOrEmpty(parameterValue)))
        {
            string query = URLHelper.RemoveUrlParameter(RequestContext.CurrentQueryString, "hash");
            query = URLHelper.AddUrlParameter(query, parameterName, parameterValue);
            query = URLHelper.AddUrlParameter(query, "hash", QueryHelper.GetHash(query));
            return UrlResolver.ResolveUrl(path + fileName) + query.Replace("'", "%27");
        }
        else
        {
            return UrlResolver.ResolveUrl(path + fileName) + RequestContext.CurrentQueryString;
        }
    }


    /// <summary>
    /// Returns path to the Media libraries tab page.
    /// </summary>
    private string GetMediaLibrariesPath()
    {
        const string MEDIA_FORMCONTROLS_FOLDER = "~/CMSModules/MediaLibrary/FormControls/";

        var path = MEDIA_FORMCONTROLS_FOLDER + "Selectors/InsertImageOrMedia/Tabs_Media.aspx";

        return UrlResolver.ResolveUrl(path) + RequestContext.CurrentQueryString;
    }


    /// <summary>
    /// Creates collection of tabs which should be displayed to the user.
    /// </summary>
    private void GetTabs()
    {
        UITabs tabControl = CurrentMaster.Tabs;

        // Disable personalization for none-HTML editors
        var checkUI = !new[] { "copy", "move", "link", "relationship", "selectpath" }.Contains(CustomOutputFormat);

        if (checkUI)
        {
            if ((OutputFormat == OutputFormatEnum.HTMLMedia) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertImageOrMedia"))
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "frameLoad", ScriptHelper.GetScript("if (window.parent.frames['insertContent']) { window.parent.frames['insertContent'].location= '" + UrlResolver.ResolveUrl(AdministrationUrlHelper.GetAccessDeniedUrl("CMS.WYSIWYGEditor", null, "InsertImageOrMedia")) + "';} "));
                return;
            }
            if ((OutputFormat == OutputFormatEnum.HTMLLink) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertLink"))
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "frameLoad", ScriptHelper.GetScript("if (window.parent.frames['insertContent']) { window.parent.frames['insertContent'].location= '" + UrlResolver.ResolveUrl(AdministrationUrlHelper.GetAccessDeniedUrl("CMS.WYSIWYGEditor", null, "InsertLink")) + "';} "));
                return;
            }
            if ((CustomOutputFormat == "linkdoc") && !(MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New.LinkExistingDocument") && MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New")))
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "frameLoad", ScriptHelper.GetScript("if (window.parent.frames['insertContent']) { window.parent.frames['insertContent'].location= '" + UrlResolver.ResolveUrl(AdministrationUrlHelper.GetAccessDeniedUrl("CMS.Content", null, "New.LinkExistingDocument")) + "';} "));
                return;
            }
        }

        // Attachments
        if (String.IsNullOrEmpty(CustomOutputFormat) && !HasMetaFileObjectInfo && !HideAttachments &&
            (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "AttachmentsTab")))
        {
            tabControl.AddTab(new UITabItem()
            {
                Text = GetString("general.attachments"),
                RedirectUrl = GetFilePath("Tabs_Media.aspx", "source", CMSDialogHelper.GetMediaSource(MediaSourceEnum.DocumentAttachments))
            });

            if (SelectedTab == "attachments")
            {
                mSelectedTabIndex = tabControl.TabItems.Count - 1;
            }
        }
        else if (String.IsNullOrEmpty(CustomOutputFormat) && HasMetaFileObjectInfo && !HideAttachments &&
            (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "MetafilesTab")))
        {
            // Metafile attachments
            tabControl.AddTab(new UITabItem()
            {
                Text = GetString("general.attachments"),
                RedirectUrl = GetFilePath("Tabs_Media.aspx", "source", CMSDialogHelper.GetMediaSource(MediaSourceEnum.MetaFile))
            });


            if (SelectedTab == "attachments")
            {
                mSelectedTabIndex = tabControl.TabItems.Count - 1;
            }
        }
        else if (SelectedTab == "attachments")
        {
            SelectedTab = "web";
        }

        // Content
        if (!HideContent && (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "ContentTab")))
        {
            tabControl.AddTab(new UITabItem()
            {
                Text = GetString("general.content"),
                RedirectUrl = GetFilePath("Tabs_Media.aspx", "source", CMSDialogHelper.GetMediaSource(MediaSourceEnum.Content))
            });

            if (SelectedTab == "content")
            {
                mSelectedTabIndex = tabControl.TabItems.Count - 1;
            }
        }
        else if (SelectedTab == "content")
        {
            SelectedTab = "web";
        }

        // Media libraries
        if ((CustomOutputFormat == "") && !HideMediaLibraries &&
            ModuleManager.IsModuleLoaded(ModuleName.MEDIALIBRARY) &&
            (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "MediaLibrariesTab")))
        {
            tabControl.AddTab(new UITabItem()
            {
                Text = GetString("dialogs.header.libraries"),
                RedirectUrl = GetMediaLibrariesPath()
            });


            if (SelectedTab == "libraries")
            {
                mSelectedTabIndex = tabControl.TabItems.Count - 1;
            }
        }
        else if (SelectedTab == "libraries")
        {
            SelectedTab = "web";
        }

        // Web
        if (String.IsNullOrEmpty(CustomOutputFormat) && !HideWeb &&
            (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "WebTab")))
        {
            tabControl.AddTab(new UITabItem()
            {
                Text = GetString("dialogs.header.web"),
                RedirectUrl = (OutputFormat == OutputFormatEnum.BBLink) || (OutputFormat == OutputFormatEnum.HTMLLink) ? GetFilePath("Tabs_WebLink.aspx") : GetFilePath("Tabs_Web.aspx")
            });

            if (SelectedTab == "web")
            {
                mSelectedTabIndex = tabControl.TabItems.Count - 1;
            }
        }

        // Anchor & E-mail
        if (String.IsNullOrEmpty(CustomOutputFormat) && (OutputFormat == OutputFormatEnum.BBLink ||
                                           OutputFormat == OutputFormatEnum.HTMLLink ||
                                           OutputFormat == OutputFormatEnum.Custom))
        {
            // Anchor
            if (!HideAnchor && (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "AnchorTab")))
            {
                tabControl.AddTab(new UITabItem()
                {
                    Text = GetString("dialogs.header.anchor"),
                    RedirectUrl = GetFilePath("Tabs_Anchor.aspx")
                });

                if (SelectedTab == "anchor")
                {
                    mSelectedTabIndex = tabControl.TabItems.Count - 1;
                }
            }
            else if (SelectedTab == "anchor")
            {
                SelectedTab = "web";
            }

            // E-mail
            if (!HideEmail && (!checkUI || currentUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "EmailTab")))
            {
                tabControl.AddTab(new UITabItem()
                {
                    Text = GetString("general.email"),
                    RedirectUrl = GetFilePath("Tabs_Email.aspx")
                });

                if (SelectedTab == "email")
                {
                    mSelectedTabIndex = tabControl.TabItems.Count - 1;
                }
            }
            else if (SelectedTab == "email")
            {
                SelectedTab = "web";
            }
        }

        string selectedUrl = mSelectedTabIndex > 0 ? tabControl.TabItems[mSelectedTabIndex].RedirectUrl : (tabControl.TabItems.Count > 0 ? tabControl.TabItems[0].RedirectUrl : String.Empty);
        if (!String.IsNullOrEmpty(selectedUrl))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "frameLoad", ScriptHelper.GetScript("if (window.parent.frames['insertContent']) { window.parent.frames['insertContent'].location= '" + selectedUrl.Replace("&amp;", "&").Replace("'", "%27") + "';} "));
        }

        // No tab is displayed -> load UI Not available
        if (tabControl.TabItems.Count == 0)
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "frameLoad", ScriptHelper.GetScript("if (window.parent.frames['insertContent']) { window.parent.frames['insertContent'].location= '" + UrlResolver.ResolveUrl(AdministrationUrlHelper.GetInformationUrl("uiprofile.uinotavailable")) + "';} "));
        }
        else if (tabControl.TabItems.Count == 1)
        {
            tabControl.Visible = false;

            // Hide empty space because of hidden tab control
            ScriptHelper.RegisterStartupScript(this, typeof(string), "headerFrameHide", ScriptHelper.GetScript("parent.$cmsj('#rowsFrameset').attr('rows','" + CMSPage.TitleOnlyHeight + ", *, " + CMSPage.FooterFrameHeight + "')"));
        }
    }

    #endregion
}
