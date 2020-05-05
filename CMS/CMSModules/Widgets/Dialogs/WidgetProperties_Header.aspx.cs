using System;
using System.Collections;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Widgets_Dialogs_WidgetProperties_Header : CMSWidgetPropertiesPage
{
    #region "Methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CurrentMaster.BodyClass += " WidgetTabsPageHeader";
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSDialogHelper.RegisterDialogHelper(Page);
        ScriptHelper.RegisterScriptFile(Page, "Dialogs/HTMLEditor.js");
        ScriptHelper.RegisterWOpenerScript(Page);

        // Initialize page title
        pageTitle.TitleText = GetString("Widgets.Properties.Title");
        // Ensure the design mode for the dialog
        if (String.IsNullOrEmpty(aliasPath))
        {
            PortalContext.SetRequestViewMode(ViewModeEnum.Design);
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);

        if (!RequestHelper.IsPostBack())
        {
            InitalizeMenu();
        }

        // If inline edit register postback for get widget data (from JS editor)
        if (inline && !isNewWidget)
        {
            if (!RequestHelper.IsPostBack())
            {
                ltlScript.Text = ScriptHelper.GetScript("function DoHiddenPostback(){" + Page.ClientScript.GetPostBackEventReference(btnHidden, "") + "}");
                ltlScript.Text += ScriptHelper.GetScript("GetSelected('" + hdnSelected.ClientID + "');");
            }
        }
    }


    /// <summary>
    /// Initializes menu.
    /// </summary>
    protected void InitalizeMenu()
    {
        if (!String.IsNullOrEmpty(widgetId) || !String.IsNullOrEmpty(widgetName))
        {
            WidgetInfo wi = null;

            // Get page info
            PageInfo pi = CMSWebPartPropertiesPage.GetPageInfo(aliasPath, templateId, culture);

            if (pi == null)
            {
                Visible = false;
                return;
            }

            // Get template instance
            PageTemplateInstance templateInstance = CMSPortalManager.GetTemplateInstanceForEditing(pi);

            if (templateInstance != null)
            {
                // Get zone type
                WebPartZoneInstance zoneInstance = templateInstance.GetZone(zoneId);

                if (zoneInstance != null)
                {
                    zoneType = zoneInstance.WidgetZoneType;
                }
                if (!isNewWidget)
                {
                    // Get web part
                    WebPartInstance widget = templateInstance.GetWebPart(instanceGuid, widgetId);

                    if ((widget != null) && widget.IsWidget)
                    {
                        // WebPartType = codename, get widget by codename 
                        wi = WidgetInfoProvider.GetWidgetInfo(widget.WebPartType);

                        // Set the variant mode (MVT/Content personalization)
                        variantMode = widget.VariantMode;
                    }
                }
            }
            // New widget
            if (isNewWidget)
            {
                int id = ValidationHelper.GetInteger(widgetId, 0);
                if (id > 0)
                {
                    wi = WidgetInfoProvider.GetWidgetInfo(id);
                }
                else if (!String.IsNullOrEmpty(widgetName))
                {
                    wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
                }
            }

            // Get widget info from name if not found yet
            if ((wi == null) && (!String.IsNullOrEmpty(widgetName)))
            {
                wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
            }

            if (wi != null)
            {
                pageTitle.TitleText = GetString("Widgets.Properties.Title") + " (" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wi.WidgetDisplayName)) + ")";
            }

            // Use live or non live dialogs
            string documentationUrl = String.Empty;

            // If no zone type defined or not inline => do not show documentation 
            switch (zoneType)
            {
                case WidgetZoneTypeEnum.Dashboard:
                case WidgetZoneTypeEnum.Editor:
                case WidgetZoneTypeEnum.Group:
                case WidgetZoneTypeEnum.User:
                    documentationUrl = ResolveUrl("~/CMSModules/Widgets/Dialogs/WidgetDocumentation.aspx");
                    break;

                // If no zone set => do not create documentation link
                default:
                    if (inline)
                    {
                        documentationUrl = ResolveUrl("~/CMSModules/Widgets/Dialogs/WidgetDocumentation.aspx");
                    }
                    else
                    {
                        return;
                    }
                    break;
            }

            // Generate documentation link
            Literal ltr = new Literal();
            pageTitle.RightPlaceHolder.Controls.Add(ltr);

            // Ensure correct parameters in documentation URL
            documentationUrl += URLHelper.GetQuery(RequestContext.CurrentURL);
            if (wi != null)
            {
                documentationUrl = URLHelper.UpdateParameterInUrl(documentationUrl, "widgetid", wi.WidgetID.ToString());
            }

            string docScript = "NewWindow('" + ScriptHelper.GetString(documentationUrl, encapsulate: false) + "', 'WebPartPropertiesDocumentation', 800, 800); return false;";
            string tooltip = GetString("help.tooltip");
            ltr.Text += String.Format
                ("<div class=\"action-button\"><a onclick=\"{0}\" href=\"#\"><span class=\"sr-only\">{1}</span><i class=\"icon-modal-question cms-icon-80\" title=\"{1}\" aria-hidden=\"true\"></i></a></div>",
                    HTMLHelper.EncodeForHtmlAttribute(docScript), tooltip);
        }
    }


    /// <summary>
    /// Save widget state to definition.
    /// </summary>
    protected void btnHidden_Click(object sender, EventArgs e)
    {
        string value = HttpUtility.UrlDecode(hdnSelected.Value);
        if (!String.IsNullOrEmpty(value))
        {
            // Parse definition 
            Hashtable parameters = CMSDialogHelper.GetHashTableFromString(value);
            // Trim control name
            if (parameters["name"] != null)
            {
                widgetName = parameters["name"].ToString();
            }

            InitalizeMenu();

            SessionHelper.SetValue("WidgetDefinition", value);
        }

        string dialogUrl = "~/CMSModules/Widgets/Dialogs/widgetproperties_properties_frameset.aspx" + RequestContext.CurrentQueryString;
        ltlScript.Text = ScriptHelper.GetScript("if (window.parent.frames['widgetpropertiescontent']) { window.parent.frames['widgetpropertiescontent'].location= '" + ResolveUrl(dialogUrl) + "';} ");
    }

    #endregion
}
