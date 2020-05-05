using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.PortalEngine;

public partial class CMSWebParts_General_AdminActions : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Indicates whether on-site editing is preferred for document editing if is available
    /// </summary>
    public bool PreferOnSiteEdit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PreferOnSiteEdit"), true);
        }
        set
        {
            SetValue("PreferOnSiteEdit", value);
        }
    }


    /// <summary>
    /// Display only to global administrator.
    /// </summary>
    public bool DisplayOnlyToGlobalAdministrator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyToGlobalAdministrator"), false);
        }
        set
        {
            SetValue("DisplayOnlyToGlobalAdministrator", value);
        }
    }


    /// <summary>
    /// Check permissions.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Separator.
    /// </summary>
    public string Separator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Separator"), null);
        }
        set
        {
            SetValue("Separator", value);
        }
    }


    /// <summary>
    /// Show cms desk link.
    /// </summary>
    public bool ShowCMSDeskLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCMSDeskLink"), true);
        }
        set
        {
            SetValue("ShowCMSDeskLink", value);
        }
    }


    /// <summary>
    /// CMS Desk link text.
    /// </summary>
    public string CMSDeskLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CMSDeskLinkText"), "Administration");
        }
        set
        {
            SetValue("CMSDeskLinkText", value);
        }
    }


    /// <summary>
    /// CMS Desk text.
    /// </summary>
    public string CMSDeskText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CMSDeskText"), "{0}");
        }
        set
        {
            SetValue("CMSDeskText", value);
        }
    }


    /// <summary>
    /// Show edit document link.
    /// </summary>
    public bool ShowEditDocumentLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditDocumentLink"), false);
        }
        set
        {
            SetValue("ShowEditDocumentLink", value);
        }
    }


    /// <summary>
    /// Edit document link text.
    /// </summary>
    public string EditDocumentLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EditDocumentLinkText"), "Edit page");
        }
        set
        {
            SetValue("EditDocumentLinkText", value);
        }
    }


    /// <summary>
    /// Edit document text.
    /// </summary>
    public string EditDocumentText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EditDocumentText"), "{0}");
        }
        set
        {
            SetValue("EditDocumentText", value);
        }
    }


    /// <summary>
    /// Default user name for logon page.
    /// </summary>
    public string DefaultUserName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultUserName"), null);
        }
        set
        {
            SetValue("DefaultUserName", value);
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
            // Do not process
        }
        else
        {
            var uinfo = MembershipContext.AuthenticatedUser;

            if (uinfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) || !DisplayOnlyToGlobalAdministrator)
            {
                // Create new string builder for links
                StringBuilder sb = new StringBuilder();

                // Store current site name
                string curSiteName = SiteContext.CurrentSiteName;
                // Get default user name
                string queryStringKey = (string.IsNullOrEmpty(DefaultUserName)) ? null : "?username=" + DefaultUserName;
                bool separatorNeeded = false;

                // If cms desk link is shown
                if (ShowCMSDeskLink && (!CheckPermissions || uinfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, curSiteName)))
                {
                    string url = ResolveUrl("~/Admin/");
                    if (!string.IsNullOrEmpty(DefaultUserName))
                    {
                        url = URLHelper.AddParameterToUrl(url, "username", DefaultUserName);
                    }

                    sb.AppendFormat(CMSDeskText, string.Concat("<a href=\"", HTMLHelper.EncodeForHtmlAttribute(url), "\">", CMSDeskLinkText, "</a>"));

                    separatorNeeded = true;
                }

                // If edit document link is shown
                if (ShowEditDocumentLink && (!CheckPermissions || (uinfo.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, curSiteName) && CMSPage.IsUserAuthorizedPerContent() && (uinfo.IsAuthorizedPerDocument(DocumentContext.CurrentDocument, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed))))
                {
                    // Check if separator needed
                    if (separatorNeeded)
                    {
                        sb.Append(" " + Separator + " ");
                    }

                    string url = HTMLHelper.EncodeForHtmlAttribute(DocumentUIHelper.GetDocumentEditUrl(DocumentContext.CurrentDocument.NodeID, DocumentContext.CurrentDocumentCulture.CultureCode));
                    if (PreferOnSiteEdit && PortalHelper.IsOnSiteEditingEnabled(CurrentSiteName))
                    {
                         url = UrlResolver.ResolveUrl(PortalHelper.OnSiteEditRelativeURL);

                        string retUrl = RequestContext.CurrentURL;
                        url = URLHelper.UpdateParameterInUrl(url, "editurl", HttpUtility.UrlEncode(retUrl));
                    }

                    sb.AppendFormat(EditDocumentText, string.Concat("<a href=\"", HTMLHelper.EncodeForHtmlAttribute(url), "\">", EditDocumentLinkText, "</a>"));
                }

                ltlAdminActions.Text = sb.ToString();
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}