using System;
using System.Web;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Community;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;

public partial class CMSWebParts_Community_Profile_GroupSecurityMessage : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the text which should be displayed if group content is available only to site members.
    /// </summary>
    public string SiteMembersOnlyText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteMembersOnlyText"), GetString("group.SiteMembersOnlyText"));
        }
        set
        {
            SetValue("SiteMembersOnlyText", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed if group content is available only to its members.
    /// </summary>
    public string GroupMembersOnlyText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("GroupMembersOnlyText"), GetString("group.GroupMembersOnlyText"));
        }
        set
        {
            SetValue("GroupMembersOnlyText", value);
        }
    }


    /// <summary>
    /// Gets or sets the text which should be displayed if group content is available only to its members and current user is site member.
    /// </summary>
    public string GroupMembersOnlyAuthenticatedText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("GroupMembersOnlyAuthenticatedText"), GetString("group.GroupMembersOnlyAuthenticatedText"));
        }
        set
        {
            SetValue("GroupMembersOnlyAuthenticatedText", value);
        }
    }


    /// <summary>
    /// URL where public user will be redirected.
    /// </summary>
    public string LoginURL
    {
        get
        {
            return ResolveUrl(DataHelper.GetNotEmpty(GetValue("LoginURL"), AuthenticationHelper.DEFAULT_LOGON_PAGE));
        }
        set
        {
            SetValue("LoginURL", value);
        }
    }


    /// <summary>
    /// Path to join group page.
    /// </summary>
    public string JoinGroupPath
    {
        get
        {
            // Get path from path selector
            return ValidationHelper.GetString(GetValue("JoinGroupPath"), "");
        }
        set
        {
            SetValue("JoinGroupPath", value);
        }
    }

    #endregion


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
            GroupInfo group = null;

            // Try to get group information from query string
            int groupId = QueryHelper.GetInteger("groupid", 0);
            group = (groupId > 0) ? GroupInfoProvider.GetGroupInfo(groupId) : CommunityContext.CurrentGroup;

            if (group != null)
            {
                // Check whether group is approved
                if (!group.GroupApproved)
                {
                    lblAccessInfo.Text = GetString("group.groupnotavailable");
                }
                else
                {
                    // Check permissions for current user
                    switch (group.GroupAccess)
                    {
                            // Anybody can view the content
                        default:
                        case SecurityAccessEnum.AllUsers:
                            break;

                            // Site members can view the content
                        case SecurityAccessEnum.AuthenticatedUsers:
                            if (!AuthenticationHelper.IsAuthenticated())
                            {
                                lblAccessInfo.Text = String.Format(SiteMembersOnlyText, "<a title=\"Sign in\" href=\"" + URLHelper.UpdateParameterInUrl(LoginURL, "returnurl", HTMLHelper.EncodeForHtmlAttribute(RequestContext.CurrentURL)) + "\">", "</a>");
                            }
                            break;

                            // Only group members can view the content
                        case SecurityAccessEnum.GroupMembers:
                            if (!MembershipContext.AuthenticatedUser.IsGroupMember(group.GroupID))
                            {
                                // If current user is public show text with sign in link
                                if (!AuthenticationHelper.IsAuthenticated())
                                {
                                    lblAccessInfo.Text = String.Format(GroupMembersOnlyText, "<a title=\"Sign in\" href=\"" + URLHelper.UpdateParameterInUrl(LoginURL, "returnurl", HTMLHelper.EncodeForHtmlAttribute(RequestContext.CurrentURL)) + "\">", "</a>");
                                }
                                else
                                {
                                    // For authenticate users display text with join the group text
                                    string link = string.Empty;

                                    // If join group path is defined use it, otherwise display join the group dialog
                                    if (String.IsNullOrEmpty(JoinGroupPath))
                                    {
                                        // Register the dialog script
                                        ScriptHelper.RegisterDialogScript(Page);

                                        string script = ScriptHelper.GetScript("function JoinToGroupAccessRequest() {\n" +
                                                                               "modalDialog('" + ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/Groups/CMSPages/JoinTheGroup.aspx") + "?groupid=" + group.GroupID + "','requestJoinToGroup', 500, 180); \n" +
                                                                               " } \n");
                                        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "JoinToGroupAccessRequest", script);

                                        link = "<a title=\"" + GetString("group.joingroup") + "\" href=\"" + HTMLHelper.EncodeForHtmlAttribute(RequestContext.CurrentURL) + "\" onclick=\"JoinToGroupAccessRequest(); return false;\">";
                                    }
                                    else
                                    {
                                        link = "<a title=\"" + GetString("group.joingroup") + "\" href=\"" + JoinGroupPath + "\">";
                                    }

                                    lblAccessInfo.Text = String.Format(GroupMembersOnlyAuthenticatedText, link, "</a>");
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                Visible = false;
            }
        }
    }
}
