using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartDocumentation : CMSAdminControl
{
    #region "Variables"

    private string plImg = "";
    private string minImg = "";
    private WidgetZoneTypeEnum zoneType = WidgetZoneTypeEnum.None;
    private WebPartInfo wpi = null;
    private WidgetInfo wi = null;
    protected int mPageTemplateID = 0;
    private string mAliasPath = String.Empty;
    private string mCultureCode = null;
    private bool mIsInline = false;
    private string mZoneID = String.Empty;
    protected int mWidgetID = 0;
    protected string mWebpartID = null;
    protected string mDashboardName = null;
    protected string mDashboardSiteName = null;
    protected Guid mInstanceGUID = Guid.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Page template ID.
    /// </summary>
    public int PageTemplateID
    {
        get
        {
            return mPageTemplateID;
        }
        set
        {
            mPageTemplateID = value;
        }
    }


    /// <summary>
    /// Page alias path of document where widget shoudld be placed.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return mAliasPath;
        }
        set
        {
            mAliasPath = value;
        }
    }


    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    public string CultureCode
    {
        get
        {
            if (string.IsNullOrEmpty(mCultureCode))
            {
                mCultureCode = LocalizationContext.PreferredCultureCode;
            }
            return mCultureCode;
        }
        set
        {
            mCultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets whether selector is loaded for inline widgets.
    /// </summary>
    public bool IsInline
    {
        get
        {
            return mIsInline;
        }
        set
        {
            mIsInline = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of webpart(widget) zone where selected widget shoudld be placed.
    /// </summary>
    public string ZoneID
    {
        get
        {
            return mZoneID;
        }
        set
        {
            mZoneID = value;
        }
    }


    /// <summary>
    /// Widget identifier.
    /// </summary>
    public int WidgetID
    {
        get
        {
            return mWidgetID;
        }
        set
        {
            mWidgetID = value;
        }
    }


    /// <summary>
    /// Web part identifier.
    /// </summary>
    public string WebpartID
    {
        get
        {
            return mWebpartID;
        }
        set
        {
            mWebpartID = value;
        }
    }


    /// <summary>
    /// Dashboard name.
    /// </summary>
    public string DashboardName
    {
        get
        {
            return mDashboardName;
        }
        set
        {
            mDashboardName = value;
        }
    }


    /// <summary>
    /// Dashboard site name.
    /// </summary>
    public string DashboardSiteName
    {
        get
        {
            return mDashboardSiteName;
        }
        set
        {
            mDashboardSiteName = value;
        }
    }


    /// <summary>
    /// Instance GUID.
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            return mInstanceGUID;
        }
        set
        {
            mInstanceGUID = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        plImg = GetImageUrl("CMSModules/CMS_PortalEngine/WebpartProperties/plus.png");
        minImg = GetImageUrl("CMSModules/CMS_PortalEngine/WebpartProperties/minus.png");

        if (WebpartID != String.Empty)
        {
            wpi = WebPartInfoProvider.GetWebPartInfo(WebpartID);
        }


        // Ensure correct view mode 
        if (String.IsNullOrEmpty(AliasPath))
        {
            // Ensure the dashboard mode for the dialog
            if (!string.IsNullOrEmpty(DashboardName))
            {
                PortalContext.SetRequestViewMode(ViewModeEnum.DashboardWidgets);
                PortalContext.DashboardName = DashboardName;
                PortalContext.DashboardSiteName = DashboardSiteName;
            }
            // Ensure the design mode for the dialog
            else
            {
                PortalContext.SetRequestViewMode(ViewModeEnum.Design);
            }
        }

        if (WidgetID != 0)
        {
            PageInfo pi = CMSWebPartPropertiesPage.GetPageInfo(AliasPath, PageTemplateID, CultureCode);

            if (pi != null)
            {
                PageTemplateInstance templateInstance = CMSPortalManager.GetTemplateInstanceForEditing(pi);

                if (templateInstance != null)
                {
                    // Get the instance of widget
                    WebPartInstance widgetInstance = templateInstance.GetWebPart(InstanceGUID);

                    // Info for zone type
                    WebPartZoneInstance zone = templateInstance.GetZone(ZoneID);

                    if (zone != null)
                    {
                        zoneType = zone.WidgetZoneType;
                    }

                    if (widgetInstance != null)
                    {
                        // Create widget from webpart instance
                        wi = WidgetInfoProvider.GetWidgetInfo(widgetInstance.WebPartType);
                    }
                }
            }

            // If inline widget display columns as in editor zone
            if (IsInline)
            {
                zoneType = WidgetZoneTypeEnum.Editor;
            }


            // If no zone set (only global admins allowed to continue)
            if (zoneType == WidgetZoneTypeEnum.None)
            {
                if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
                {
                    RedirectToAccessDenied(GetString("attach.actiondenied"));
                }
            }

            // If wi is still null (new item f.e.)
            if (wi == null)
            {
                // Try to get widget info directly by ID
                wi = WidgetInfoProvider.GetWidgetInfo(WidgetID);
            }
        }

        String itemDescription = String.Empty;
        String itemType = String.Empty;
        String itemDisplayName = String.Empty;
        String itemDocumentation = String.Empty;
        String itemIcon = String.Empty;
        int itemID = 0;

        // Check whether webpart was found
        if (wpi != null)
        {
            itemDescription = wpi.WebPartDescription;
            itemType = WebPartInfo.OBJECT_TYPE;
            itemID = wpi.WebPartID;
            itemDisplayName = wpi.WebPartDisplayName;
            itemDocumentation = wpi.WebPartDocumentation;

            itemIcon = PortalHelper.GetIconHtml(wpi.WebPartThumbnailGUID, wpi.WebPartIconClass ?? PortalHelper.DefaultWebPartIconClass);
        }
        // Or widget was found
        else if (wi != null)
        {
            itemDescription = wi.WidgetDescription;
            itemType = WidgetInfo.OBJECT_TYPE;
            itemID = wi.WidgetID;
            itemDisplayName = wi.WidgetDisplayName;
            itemDocumentation = wi.WidgetDocumentation;
            itemIcon = PortalHelper.GetIconHtml(wi.WidgetThumbnailGUID, wi.WidgetIconClass ?? PortalHelper.DefaultWidgetIconClass);
        }

        if ((wpi != null) || (wi != null))
        {
            // Get WebPart (widget) icon
            ltrImage.Text = itemIcon;

            // Set description of webpart
            ltlDescription.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(itemDescription));

            // Get description from parent webpart if webpart is inherited
            if ((wpi != null) && (string.IsNullOrEmpty(wpi.WebPartDescription) && (wpi.WebPartParentID > 0)))
            {
                WebPartInfo pwpi = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                if (pwpi != null)
                {
                    ltlDescription.Text = HTMLHelper.HTMLEncode(pwpi.WebPartDescription);
                }
            }

            FormInfo fi = null;

            // Generate properties
            if (wpi != null)
            {
                // Get form info from parent if webpart is inherited
                if (wpi.WebPartParentID != 0)
                {
                    WebPartInfo pwpi = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                    if (pwpi != null)
                    {
                        fi = GetWebPartProperties(pwpi);
                    }
                }
                else
                {
                    fi = GetWebPartProperties(wpi);
                }
            }
            else if (wi != null)
            {
                fi = GetWidgetProperties(wi);
            }

            // Generate properties
            if (fi != null)
            {
                GenerateProperties(fi);
            }

            // Generate documentation text
            if (itemDocumentation == null || itemDocumentation.Trim() == "")
            {
                if ((wpi != null) && (wpi.WebPartParentID != 0))
                {
                    WebPartInfo pwpi = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                    if (pwpi != null && pwpi.WebPartDocumentation.Trim() != "")
                    {
                        ltlContent.Text = HTMLHelper.ResolveUrls(pwpi.WebPartDocumentation, null);
                    }
                    else
                    {
                        ltlContent.Text = "<br /><div style=\"padding-left:5px; font-weight: bold;\">" + GetString("WebPartDocumentation.DocumentationText") + "</div><br />";
                    }
                }
                else
                {
                    ltlContent.Text = "<br /><div style=\"padding-left:5px; font-weight: bold;\">" + GetString("WebPartDocumentation.DocumentationText") + "</div><br />";
                }
            }
            else
            {
                ltlContent.Text = HTMLHelper.ResolveUrls(itemDocumentation, null);
            }
        }
        ScriptHelper.RegisterJQuery(Page);

        string script = @"
$cmsj(document.body).ready(initializeResize);
           
function initializeResize ()  { 
    resizeareainternal();
    $cmsj(window).resize(function() { resizeareainternal(); });
}

function resizeareainternal () {
    var height = document.body.clientHeight ; 
    var panel = document.getElementById ('" + divScrolable.ClientID + @"');
                
    // Get parent footer to count proper height (with padding included)
    var footer = $cmsj('#divFooter');                      
    panel.style.height = (height - footer.outerHeight() - panel.offsetTop) +'px';                  
}";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(Page), "mainScript", ScriptHelper.GetScript(script));

        // Init tabs
        tabControlElem.UsePostback = true;

        tabControlElem.AddTab(new UITabItem()
        {
            Text = GetString("webparts.documentation"),
        });

        tabControlElem.AddTab(new UITabItem()
        {
            Text = GetString("general.properties"),
        });

        // Disable caching
        Response.Cache.SetNoStore();

        base.OnLoad(e);
    }


    /// <summary>
    /// Returns form info with widget properties.
    /// </summary>
    /// <param name="wi">Widget</param>
    protected FormInfo GetWidgetProperties(WidgetInfo wi)
    {
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WidgetWebPartID);
        if (wpi != null)
        {
            string widgetProperties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WidgetProperties);

            return PortalFormHelper.GetWidgetFormInfo(wi.WidgetName, zoneType, widgetProperties, false);
        }
        return null;
    }


    /// <summary>
    /// Returns form info with webpart properties.
    /// </summary>
    /// <param name="wpi">Web part info</param>
    protected FormInfo GetWebPartProperties(WebPartInfo wpi)
    {
        if (wpi != null)
        {
            // Before form                
            string before = PortalFormHelper.GetWebPartProperties((WebPartTypeEnum)wpi.WebPartType, PropertiesPosition.Before);
            FormInfo bfi = new FormInfo(before);
            // After form
            string after = PortalFormHelper.GetWebPartProperties((WebPartTypeEnum)wpi.WebPartType, PropertiesPosition.After);
            FormInfo afi = new FormInfo(after);

            // Add general category to first items in webpart without category
            string properties = FormHelper.EnsureDefaultCategory(wpi.WebPartProperties, GetString("general.general"));

            return PortalFormHelper.GetWebPartFormInfo(wpi.WebPartName, properties, bfi, afi, true);
        }

        return null;
    }


    /// <summary>
    /// Generate properties.
    /// </summary>
    protected void GenerateProperties(FormInfo fi)
    {
        ltlProperties.Text = String.Empty;

        // Generate script to display and hide  properties groups
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ShowHideProperties", ScriptHelper.GetScript(@"
            function ShowHideProperties(id) {
                var obj = document.getElementById(id);
                var objlink = document.getElementById(id+'_link');
                if ((obj != null)&&(objlink != null)){
                   if (obj.style.display == 'none' ) { obj.style.display=''; objlink.innerHTML = '<img border=""0"" src=""" + minImg + @""" >'; } 
                   else {obj.style.display='none'; objlink.innerHTML = '<img border=""0"" src=""" + plImg + @""" >';}
                objlink.blur();}}"));

        // Get defintion elements
        var infos = fi.GetFormElements(true, false);

        bool isOpenSubTable = false;
        bool firstLoad = true;
        bool hasAnyProperties = false;

        string currentGuid = "";
        string newCategory = null;

        // Check all items in object array
        foreach (IDataDefinitionItem contrl in infos)
        {
            // Generate row for form category
            if (contrl is FormCategoryInfo)
            {
                // Load castegory info
                FormCategoryInfo fci = contrl as FormCategoryInfo;
                if (fci != null)
                {
                    if (isOpenSubTable)
                    {
                        ltlProperties.Text += "<tr class=\"PropertyBottom\"><td class=\"PropertyLeftBottom\">&nbsp;</td><td colspan=\"2\" class=\"Center\">&nbsp;</td><td class=\"PropertyRightBottom\">&nbsp;</td></tr></table>";
                        isOpenSubTable = false;
                    }

                    if (currentGuid == "")
                    {
                        currentGuid = Guid.NewGuid().ToString().Replace("-", "_");
                    }

                    // Generate properties content
                    newCategory = @"<br />
                        <table cellpadding=""0"" cellspacing=""0"" class=""CategoryTable"">
                          <tr>
                            <td class=""CategoryLeftBorder"">&nbsp;</td>
                            <td class=""CategoryTextCell"">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(fci.GetPropertyValue(FormCategoryPropertyEnum.Caption, MacroContext.CurrentResolver))) + @"</td>
                            <td class=""HiderCell"">
                              <a id=""" + currentGuid + "_link\" href=\"#\" onclick=\"ShowHideProperties('" + currentGuid + @"'); return false;"">
                              <img border=""0"" src=""" + minImg + @""" ></a>
                            </td>
                           <td class=""CategoryRightBorder"">&nbsp;</td>
                         </tr>
                       </table>";
                }
            }
            else
            {
                hasAnyProperties = true;
                // Get form field info
                FormFieldInfo ffi = contrl as FormFieldInfo;

                if (ffi != null)
                {
                    // If display only in dashboard and not dashbord zone (or none for admins) dont show
                    if ((ffi.DisplayIn == "dashboard") && ((zoneType != WidgetZoneTypeEnum.Dashboard) && (zoneType != WidgetZoneTypeEnum.None)))
                    {
                        continue;
                    }
                    if (newCategory != null)
                    {
                        ltlProperties.Text += newCategory;
                        newCategory = null;

                        firstLoad = false;
                    }
                    else if (firstLoad)
                    {
                        if (currentGuid == "")
                        {
                            currentGuid = Guid.NewGuid().ToString().Replace("-", "_");
                        }

                        // Generate properties content
                        firstLoad = false;
                        ltlProperties.Text += @"<br />
                        <table cellpadding=""0"" cellspacing=""0"" class=""CategoryTable"">
                          <tr>
                            <td class=""CategoryLeftBorder"">&nbsp;</td>
                            <td class=""CategoryTextCell"">Default</td>
                            <td class=""HiderCell"">
                               <a id=""" + currentGuid + "_link\" href=\"#\" onclick=\"ShowHideProperties('" + currentGuid + @"'); return false;"">
                               <img border=""0"" src=""" + minImg + @""" ></a>
                            </td>
                            <td class=""CategoryRightBorder"">&nbsp;</td>
                         </tr>
                       </table>";
                    }

                    if (!isOpenSubTable)
                    {
                        isOpenSubTable = true;
                        ltlProperties.Text += "" +
                                              "<table cellpadding=\"0\" cellspacing=\"0\" id=\"" + currentGuid + "\" class=\"PropertiesTable\" >";
                        currentGuid = "";
                    }

                    string fieldCaption = DataHelper.GetNotEmpty(
                        ResHelper.LocalizeString(ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, MacroContext.CurrentResolver)),
                        String.Empty
                    );
                    string fieldDescription = DataHelper.GetNotEmpty(
                        ResHelper.LocalizeString(ffi.GetPropertyValue(FormFieldPropertyEnum.FieldDescription, MacroContext.CurrentResolver)),
                        GetString("WebPartDocumentation.DescriptionNoneAvailable")
                    );

                    string doubleDot = "";
                    if (!String.IsNullOrEmpty(fieldCaption) && !fieldCaption.EndsWith(":", StringComparison.InvariantCulture))
                    {
                        doubleDot = ":";
                    }

                    ltlProperties.Text +=
                        @"<tr>
                            <td class=""PropertyLeftBorder"" >&nbsp;</td>
                            <td class=""PropertyContent"" style=""width:200px;"">" + HTMLHelper.HTMLEncode(fieldCaption) + doubleDot + @"</td>
                            <td class=""PropertyRow"">" + HTMLHelper.HTMLEncode(fieldDescription) + @"</td>
                            <td class=""PropertyRightBorder"">&nbsp;</td>
                        </tr>";
                }
            }
        }

        if (isOpenSubTable)
        {
            ltlProperties.Text += "<tr class=\"PropertyBottom\"><td class=\"PropertyLeftBottom\">&nbsp;</td><td colspan=\"2\" class=\"Center\">&nbsp;</td><td class=\"PropertyRightBottom\">&nbsp;</td></tr></table>";
        }

        if (!hasAnyProperties)
        {
            ltlProperties.Text = "<br /><div style=\"padding-left:5px;padding-right:5px; font-weight: bold;\">" + GetString("documentation.nopropertiesavaible") + "</div>";
        }
    }


    /// <summary>
    /// Tab clicked event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="ea">Event arguments</param>
    protected void tabControlElem_clicked(object sender, EventArgs ea)
    {
        if (tabControlElem.SelectedTab == 0)
        {
            pnlDoc.Visible = true;
            pnlProperties.Visible = false;
        }
        else
        {
            pnlDoc.Visible = false;
            pnlProperties.Visible = true;
        }
    }

    #endregion
}
