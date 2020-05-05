using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Widgets_Dialogs_WidgetProperties : CMSWidgetPropertiesPage
{
    #region "Methods"

    /// <summary>
    /// OnInit event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        RequireSite = true;

        IsDialog = true;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterModalPageScripts();

        // Set page title 
        Page.Title = GetString(isNewWidget ? "widgets.propertiespage.titlenew" : "widgets.propertiespage.title");

        // Resize the header (enlarge) to make a space for the tabs header when displaying a widget variant
        var headerHeight = TitleOnlyHeight;
        if (variantId > 0)
        {
            headerHeight = TabsFrameHeight;
        }

        rowsFrameset.Attributes.Add("rows", string.Format("{0}, *", headerHeight));

        // Ensure correct view mode
        if (String.IsNullOrEmpty(aliasPath))
        {
            // Ensure the dashboard mode for the dialog
            if (QueryHelper.Contains("dashboard"))
            {
                PortalContext.SetRequestViewMode(ViewModeEnum.DashboardWidgets);
                PortalContext.DashboardName = QueryHelper.GetString("dashboard", String.Empty);
                PortalContext.DashboardSiteName = QueryHelper.GetString("sitename", String.Empty);
            }
            // Ensure the design mode for the dialog
            else
            {
                PortalContext.SetRequestViewMode(ViewModeEnum.Design);
            }
        }

        if ((widgetId != "") && (PageInfo != null))
        {
            // Get template instance
            PageTemplateInstance templateInstance = CMSPortalManager.GetTemplateInstanceForEditing(PageInfo);

            // Get widget from instance
            WidgetInfo wi;
            if (!isNewWidget)
            {
                // Get the instance of widget
                WebPartInstance widgetInstance = templateInstance.GetWebPart(instanceGuid, widgetId);
                if (widgetInstance == null)
                {
                    return;
                }

                // Get widget info by widget name(widget type)
                wi = WidgetInfoProvider.GetWidgetInfo(widgetInstance.WebPartType);
            }
            // Widget instance hasn't created yet
            else
            {
                wi = WidgetInfoProvider.GetWidgetInfo(ValidationHelper.GetInteger(widgetId, 0));
            }

            if (wi != null)
            {
                WebPartZoneInstance zone = templateInstance.GetZone(zoneId);
                if (zone != null)
                {
                    var currentUser = MembershipContext.AuthenticatedUser;

                    if ((zone.WidgetZoneType != WidgetZoneTypeEnum.Group) && !WidgetRoleInfoProvider.IsWidgetAllowed(wi, currentUser.UserID, AuthenticationHelper.IsAuthenticated()))
                    {
                        RedirectToAccessDenied(GetString("widgets.security.notallowed"));
                    }
                }

                // If all ok, set up frames
                frameHeader.Attributes.Add("src", "widgetproperties_header.aspx" + RequestContext.CurrentQueryString);
                frameContent.Attributes.Add("src", "widgetproperties_properties_frameset.aspx" + RequestContext.CurrentQueryString);
            }
        }

        frameHeader.Attributes.Add("src", "widgetproperties_header.aspx" + RequestContext.CurrentQueryString);
        if (inline && !isNewWidget)
        {
            frameContent.Attributes.Add("src", ResolveUrl("~/CMSPages/Blank.htm"));
        }
        else
        {
            frameContent.Attributes.Add("src", "widgetproperties_properties_frameset.aspx" + RequestContext.CurrentQueryString);
        }
    }

    #endregion
}