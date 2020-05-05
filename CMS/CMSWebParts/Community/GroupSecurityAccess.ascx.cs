using System;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Community;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.SiteProvider;

public partial class CMSWebParts_Community_GroupSecurityAccess : CMSAbstractWebPart
{
    /// <summary>
    /// Gets or sets path where user will be redirected when he doesn't meet group security access requirements.
    /// </summary>
    public string GroupsSecurityAccessPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("GroupsSecurityAccessPath"), "");
        }
        set
        {
            SetValue("GroupsSecurityAccessPath", value);
        }
    }


    /// <summary>
    /// Gets or sets if query string should be used to transfer information about group.
    /// </summary>
    public bool UseQueryString
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseQueryString"), false);
        }
        set
        {
            SetValue("UseQueryString", value);
        }
    }


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
            SetContext();

            GroupInfo group = CommunityContext.CurrentGroup;
            if (group != null)
            {
                string path = (GroupsSecurityAccessPath == "") ? GroupInfoProvider.GetGroupSecurityAccessPath(group.GroupName, SiteContext.CurrentSiteName) : GroupsSecurityAccessPath;
                string url = "";
                if (UseQueryString)
                {
                    url = URLHelper.UpdateParameterInUrl(url, "groupid", group.GroupID.ToString());
                }

                // Check whether group is approved
                if (!group.GroupApproved)
                {
                    URLHelper.Redirect(url);
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
                                URLHelper.Redirect(url);
                            }
                            break;

                            // Only group members can view the content
                        case SecurityAccessEnum.GroupMembers:
                            if (!MembershipContext.AuthenticatedUser.IsGroupMember(group.GroupID))
                            {
                                URLHelper.Redirect(url);
                            }
                            break;
                    }
                }
            }
            else
            {
                Visible = false;
            }

            ReleaseContext();
        }
    }
}