using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_header : CMSWebPartPropertiesPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize page title
        pageTitle.TitleText = GetString("WebpartProperties.Title");
        if (!RequestHelper.IsPostBack())
        {
            InitializeMenu();
        }

        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes menu.
    /// </summary>
    protected void InitializeMenu()
    {
        if (webpartId != string.Empty)
        {
            // get pageinfo
            PageInfo pi = GetPageInfo(aliasPath, templateId, cultureCode);
            if (pi == null)
            {
                Visible = false;
                return;
            }

            PageTemplateInfo pti = pi.UsedPageTemplateInfo;
            if (pti != null)
            {
                // Get web part
                WebPartInstance webPart = pti.TemplateInstance.GetWebPart(instanceGuid, webpartId);
                
                // New webpart is loaded via WebPartID
                if ((webPart == null) && !isNew)
                {
                    // Clone templateInstance to avoid caching of the temporary template instance loaded with CP/MVT variants
                    var tempTemplateInstance = pti.TemplateInstance.Clone();
                    tempTemplateInstance.LoadVariants(false, VariantModeEnum.None);

                    webPart = tempTemplateInstance.GetWebPart(instanceGuid, -1, 0);
                }

                WebPartInfo wi = (webPart != null) ? WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType) :
                    WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(webpartId, 0));

                if (wi != null)
                {
                    // Generate documentation link
                    Literal ltr = new Literal();
                    string docScript = "NewWindow('" + ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/WebPartDocumentationPage.aspx") + "?webpartid=" + ScriptHelper.GetString(wi.WebPartName, false) + "', 'WebPartPropertiesDocumentation', 800, 800); return false;";
                    string tooltip = GetString("help.tooltip");
                    ltr.Text += String.Format
                        ("<div class=\"action-button\"><a onclick=\"{0}\" href=\"#\"><span class=\"sr-only\">{1}</span><i class=\"icon-modal-question cms-icon-80\" title=\"{1}\" aria-hidden=\"true\"></i></a></div>",
                            docScript, tooltip);

                    pageTitle.RightPlaceHolder.Controls.Add(ltr);
                    pageTitle.TitleText = GetString("WebpartProperties.Title") + " (" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(wi.WebPartDisplayName)) + ")";
                }
            }
        }
    }

    #endregion
}
