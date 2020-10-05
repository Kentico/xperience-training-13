using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using CMS.Base;
using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.PortalEngine;
using CMS.SiteProvider;

using CultureInfo = System.Globalization.CultureInfo;
using TreeNode = CMS.DocumentEngine.TreeNode;

/// <summary>
/// Common methods.
/// </summary>
public static class Functions
{
    #region "Methods"

    /// <summary>
    /// Returns connection string used throughout the application.
    /// </summary>
    public static string GetConnectionString()
    {
        return ConnectionHelper.GetSqlConnectionString();
    }


    /// <summary>
    /// Creates and returns a new, initialized instance of TreeProvider with current user set.
    /// </summary>
    public static TreeProvider GetTreeProvider()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        return tree;
    }


    /// <summary>
    /// Returns User ID of the current user.
    /// </summary>
    public static int GetUserID()
    {
        return MembershipContext.AuthenticatedUser.UserID;
    }


    /// <summary>
    /// Returns true if the current user is authorized to access the given resource (module) with required permission.
    /// </summary>
    /// <param name="resourceName">Resource name</param>
    /// <param name="permissionName">Permission name</param>
    public static bool IsAuthorizedPerResource(string resourceName, string permissionName)
    {
        return MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(resourceName, permissionName);
    }


    /// <summary>
    /// Returns true if the current user is authorized to access the given class (document type) with required permission.
    /// </summary>
    /// <param name="className">Class name in format application.class</param>    
    /// <param name="permissionName">Name of the required permission</param>    
    public static bool IsAuthorizedPerClass(string className, string permissionName)
    {
        return MembershipContext.AuthenticatedUser.IsAuthorizedPerClassName(className, permissionName);
    }


    /// <summary>
    /// Redirects user to the "Access Denied" page.
    /// </summary>
    /// <param name="resourceName">Name of the resource that cannot be accessed</param>
    /// <param name="permissionName">Name of the permission that is not allowed</param>
    public static void RedirectToAccessDenied(string resourceName, string permissionName)
    {
        if (HttpContext.Current != null)
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetAccessDeniedUrl(resourceName, permissionName, null));
        }
    }


    /// <summary>
    /// Returns preferred UI culture of the current user.
    /// </summary>
    public static CultureInfo GetPreferredUICulture()
    {
        return CultureHelper.PreferredUICultureInfo;
    }


    /// <summary>
    /// Returns current alias path based on base alias path setting and "aliaspath" querystring parameter.
    /// </summary>
    public static string GetAliasPath()
    {
        return DocumentContext.CurrentPageInfo.NodeAliasPath;
    }


    /// <summary>
    /// Returns preferred culture code (as string). You can modify this function so that it determines the preferred culture using some other algorithm.
    /// </summary>
    public static string GetPreferredCulture()
    {
        return LocalizationContext.PreferredCultureCode;
    }


    /// <summary>
    /// Returns type (such as "cms.article") of the current document.
    /// </summary>
    public static string GetDocumentType()
    {
        return DocumentContext.CurrentPageInfo.ClassName;
    }


    /// <summary>
    /// Returns type (such as "cms.article") of the specified document.
    /// </summary>
    /// <param name="aliasPath">Alias path of the document</param>
    public static string GetDocumentType(string aliasPath)
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, aliasPath, LocalizationContext.PreferredCultureCode);
        if (node != null)
        {
            return node.NodeClassName;
        }
        else
        {
            return null;
        }
    }


    /// <summary>
    /// Returns node representing the current page.
    /// </summary>
    public static TreeNode GetCurrentPage()
    {
        return DocumentContext.CurrentDocument;
    }


    /// <summary>
    /// Returns node representing the current document.
    /// </summary>
    public static TreeNode GetCurrentDocument()
    {
        return DocumentContext.CurrentDocument;
    }


    /// <summary>
    /// Returns true if the current user is member of the given role.
    /// </summary>
    /// <param name="roleName">Role name</param>
    public static bool IsInRole(string roleName)
    {
        return MembershipContext.AuthenticatedUser.IsInRole(roleName, SiteContext.CurrentSiteName);
    }


    /// <summary>
    /// Writes event to the event log.
    /// </summary>
    /// <param name="eventType">Type of the event. I = information, E = error, W = warning</param>
    /// <param name="source">Source of the event (Content, Administration, etc.)</param>
    /// <param name="eventCode">Event code (Security, Update, Delete, etc.)</param>
    /// <param name="nodeId">ID value of the document</param>
    /// <param name="nodeNamePath">NamePath value of the document</param>
    /// <param name="eventDescription">Detailed description of the event</param>
    public static void LogEvent(string eventType, string source, string eventCode, int nodeId, string nodeNamePath, string eventDescription)
    {
        int siteId = 0;
        if (SiteContext.CurrentSite != null)
        {
            siteId = SiteContext.CurrentSite.SiteID;
        }

        var eventTypeEnum = EventType.ToEventTypeEnum(eventType);

        var logData = new EventLogData(eventTypeEnum, source, eventCode)
        {
            EventDescription = eventDescription,
            EventUrl = RequestContext.RawURL,
            UserID = MembershipContext.AuthenticatedUser.UserID,
            UserName = RequestContext.UserName,
            NodeID = nodeId,
            DocumentName = nodeNamePath,
            IPAddress = RequestContext.UserHostAddress,
            SiteID = siteId
        };

        Service.Resolve<IEventLogService>().LogEvent(logData);
    }


    /// <summary>
    /// Returns first N levels of the given alias path, N+1 if CMSWebSiteBaseAliasPath is set.
    /// </summary>
    /// <param name="aliasPath">Alias path</param>
    /// <param name="level">Number of levels to be returned</param>
    public static string GetPathLevel(string aliasPath, int level)
    {
        return TreePathUtils.GetPathLevel(aliasPath, level);
    }


    /// <summary>
    /// Encodes URL(just redirection for use with aspx code.
    /// </summary>
    /// <param name="url">URL to encode</param>
    public static string UrlPathEncode(object url)
    {
        string path = ValidationHelper.GetString(url, "");

        if (HttpContext.Current != null)
        {
            return HttpContext.Current.Server.UrlPathEncode(path);
        }

        return "";
    }


    /// <summary>
    /// Returns the text of the specified region.
    /// </summary>
    /// <param name="aliasPath">Alias path of the region MenuItem</param>
    /// <param name="regionID">Region ID to get the text from</param>
    public static string GetEditableRegionText(string aliasPath, string regionID)
    {
        try
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, aliasPath, LocalizationContext.PreferredCultureCode);
        
            if (node != null)
            {
                PageInfo pi = new PageInfo();
                pi.LoadContentXml(Convert.ToString(node.GetValue("DocumentContent")));
                return Convert.ToString(pi.EditableRegions[regionID.ToLowerCSafe()]);
            }
        }
        catch
        {
        }

        return null;
    }


    /// <summary>
    /// Returns the text of the specified region.
    /// </summary>
    /// <param name="aliasPath">Alias path of the region MenuItem</param>
    /// <param name="regionID">Region ID to get the text from</param>
    /// <param name="maxLength">Maximum text length</param>
    public static string GetEditableRegionText(string aliasPath, string regionID, int maxLength)
    {
        string text = GetEditableRegionText(aliasPath, regionID);
        if (!string.IsNullOrEmpty(text))
        {
            if (text.Length > maxLength)
            {
                int lastSpace = text.LastIndexOfCSafe(" ", maxLength - 4);
                if (lastSpace < maxLength / 2)
                {
                    lastSpace = maxLength - 1;
                }
                int lastStartTag = text.LastIndexOfCSafe("<", lastSpace);
                int lastEndTag = text.LastIndexOfCSafe(">", lastSpace);
                if (lastStartTag < lastSpace && lastEndTag < lastStartTag)
                {
                    lastSpace = lastStartTag;
                }
                text = text.Substring(0, lastSpace).Trim() + " ...";
            }
        }
        return text;
    }


    /// <summary>
    /// Resolves the dynamic control macros within the parent controls collection and loads the dynamic controls instead.
    /// </summary>
    /// <param name="parent">Parent control of the control tree to resolve</param>
    public static void ResolveDynamicControls(Control parent)
    {
        ControlsHelper.ResolveDynamicControls(parent);
    }


    /// <summary>
    /// Returns the virtual path to the Admin root directory.
    /// </summary>
    public static string GetCMSDeskPath()
    {
        return "~/Admin";
    }


    /// <summary>
    /// Returns formatted username in format: username. 
    /// Allows you to customize how the usernames will look like throughout the admin UI. 
    /// </summary>
    /// <param name="username">Source user name</param>   
    /// <param name="isLiveSite">Indicates if returned username should be displayed on live site</param>
    public static string GetFormattedUserName(string username, bool isLiveSite)
    {
        return GetFormattedUserName(username, null, null, isLiveSite);
    }

    
    /// <summary>
    /// Returns formatted username in format: fullname (username). 
    /// Allows you to customize how the usernames will look like throughout the admin UI. 
    /// </summary>
    /// <param name="username">Source user name</param>
    /// <param name="fullname">Source full name</param>
    /// <param name="isLiveSite">Indicates if returned username should be displayed on live site</param>
    public static string GetFormattedUserName(string username, string fullname = null, bool isLiveSite = false)
    {
        return GetFormattedUserName(username, fullname, null, isLiveSite);
    }


    /// <summary>
    /// Returns formatted username in format: fullname (nickname) if nickname specified otherwise fullname (username). 
    /// Allows you to customize how the usernames will look like throughout various modules. 
    /// </summary>
    /// <param name="username">Source user name</param>
    /// <param name="fullname">Source full name</param>
    /// <param name="nickname">Source nick name</param>
    /// <param name="isLiveSite">Indicates if returned username should be displayed on live site</param>
    public static string GetFormattedUserName(string username, string fullname, string nickname, bool isLiveSite = false)
    {
        return UserInfoProvider.GetFormattedUserName(username, fullname, nickname);
    }


    /// <summary>
    /// Clear all hashtables.
    /// </summary>
    public static void ClearHashtables()
    {
        ModuleManager.ClearHashtables();
    }

    #endregion
    

    #region "Macros"

    /// <summary>
    /// Builds and returns the list of object types that can contain macros.
    /// </summary>
    /// <param name="include">Object types to include in the list</param>
    /// <param name="exclude">Object types to exclude from the list</param>
    /// <remarks>
    /// Excludes the object types that cannot contain macros.
    /// </remarks>
    public static IEnumerable<string> GetObjectTypesWithMacros(IEnumerable<string> include = null, IEnumerable<string> exclude = null)
    {
        // Get the system object types
        var objectTypes = ObjectTypeManager.ObjectTypesWithMacros;

        // Include custom table object types
        objectTypes = objectTypes.Union(GetCustomTableObjectTypes());

        // Include biz form object types
        objectTypes = objectTypes.Union(GetFormObjectTypes());

        // Include object types
        if (include != null)
        {
            objectTypes = objectTypes.Union(include);
        }

        // Exclude object types
        if (exclude != null)
        {
            objectTypes = objectTypes.Except(exclude);
        }

        objectTypes = objectTypes.Where(t =>
            {
                try
                {
                    var typeInfo = ObjectTypeManager.GetTypeInfo(t);

                    return (!typeInfo.IsListingObjectTypeInfo && typeInfo.MacroSettings.ContainsMacros);
                }
                catch (Exception)
                {
                    return false;
                }
            });

        return objectTypes;
    }


    /// <summary>
    /// Gets all custom table object types
    /// </summary>
    public static IEnumerable<string> GetCustomTableObjectTypes()
    {
        return DataClassInfoProvider.GetClasses()
            .WhereTrue("ClassIsCustomTable")
            .Columns("ClassName")
            .Select(r => CustomTableItemProvider.GetObjectType(r["ClassName"].ToString()));
    }


    /// <summary>
    /// Gets all BizForms object types
    /// </summary>
    public static IEnumerable<string> GetFormObjectTypes()
    {
        return DataClassInfoProvider.GetClasses()
            .WhereTrue("ClassIsForm")
            .Columns("ClassName")
            .Select(r => BizFormItemProvider.GetObjectType(r["ClassName"].ToString()));
    }

    #endregion
}