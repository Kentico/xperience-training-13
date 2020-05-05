using System;
using System.Text;
using System.Web;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.DocumentEngine;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.PortalEngine;

public partial class CMSWebParts_General_EditDocumentLink : CMSAbstractWebPart
{
    #region "Web part properties"

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
    /// URL to the image
    /// </summary>
    public string ImageURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageURL"), String.Empty);
        }
        set
        {
            SetValue("ImageURL", value);
        }
    }


    /// <summary>
    /// Text of the link.
    /// </summary>
    public string LinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinkText"), String.Empty);
        }
        set
        {
            SetValue("LinkText", value);
        }
    }


    /// <summary>
    /// Indicates if the link should be available only for users who have the access to the CMS Desk and rights to edit the current document.
    /// </summary>
    public bool ShowOnlyWhenAuthorized
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOnlyWhenAuthorized"), false);
        }
        set
        {
            SetValue("ShowOnlyWhenAuthorized", value);
        }
    }

    #endregion


    #region "Web part methods"

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
        if (!StopProcessing)
        {
            bool show = true;
            TreeNode curDoc = DocumentContext.CurrentDocument;

            // Check if permissions should be checked
            if (ShowOnlyWhenAuthorized)
            {
                // Check permissions
                if (!((MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Editor, SiteContext.CurrentSiteName)) && CMSPage.IsUserAuthorizedPerContent() && (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(curDoc, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)))
                {
                    show = false;
                    Visible = false;
                }
            }

            if (show)
            {
                // Create edit link
                StringBuilder sb = new StringBuilder("<a class=\"EditDocumentLink\" href=\"");
                // On-Site edit
                if (PreferOnSiteEdit && PortalHelper.IsOnSiteEditingEnabled(CurrentSiteName))
                {
                    string onsiteEditUrl = UrlResolver.ResolveUrl(PortalHelper.OnSiteEditRelativeURL);

                    string retUrl = RequestContext.CurrentURL;
                    onsiteEditUrl = URLHelper.UpdateParameterInUrl(onsiteEditUrl, "editurl", HttpUtility.UrlEncode(retUrl));
                    sb.Append(HTMLHelper.EncodeForHtmlAttribute(onsiteEditUrl));
                }
                // Administration edit
                else
                {
                    sb.Append("~/Admin/cmsadministration.aspx?action=edit&amp;nodeid=");
                    sb.Append(curDoc.NodeID);
                    sb.Append("&amp;culture=");
                    sb.Append(curDoc.DocumentCulture);
                    sb.Append(ApplicationUrlHelper.GetApplicationHash("cms.content", "content"));
                }
                sb.Append("\">");
                // Text link
                if (String.IsNullOrEmpty(ImageURL))
                {
                    sb.Append(LinkText);
                }
                // Image link
                else
                {
                    sb.Append("<img src=\"");
                    sb.Append(UrlResolver.ResolveUrl(ImageURL));
                    sb.Append("\" alt=\"");
                    sb.Append(HTMLHelper.HTMLEncode(LinkText));
                    sb.Append("\" title=\"");
                    sb.Append(HTMLHelper.HTMLEncode(LinkText));
                    sb.Append("\" />");
                }
                sb.Append("</a>");
                ltlEditLink.Text = sb.ToString();
            }
        }
    }

    #endregion
}