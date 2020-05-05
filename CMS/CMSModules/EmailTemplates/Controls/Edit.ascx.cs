using System;
using System.ComponentModel;

using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.EmailEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject(EmailTemplateInfo.OBJECT_TYPE, "templateid")]
public partial class CMSModules_EmailTemplates_Controls_Edit : CMSAdminEditControl
{
    /// <summary>
    /// SiteId to save
    /// </summary>
    [DefaultValue(0)]
    public int SiteId
    {
        get;
        set;
    }

    /// <summary>
    /// SelectedSiteId to save
    /// </summary>
    [DefaultValue(0)]
    public int SelectedSiteId
    {
        get;
        set;
    }


    public UIForm EditForm
    {
        get
        {
            return editForm;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if ((EditedObject == null) || (((BaseInfo)EditedObject).Generalized.ObjectID <= 0))
        {
            StringBuilder sb = new StringBuilder();

            // Selector ID in query means dialog mode
            string selector = QueryHelper.GetString("selectorid", string.Empty);
            if (String.IsNullOrEmpty(selector))
            {
                // Prepare redirect URL for creation
                sb.Append("Frameset.aspx?templateid={%EditedObject.ID%}&saved=1&tabmode=", QueryHelper.GetInteger("tabmode", 0));
                sb.Append((SiteId > 0) ? "&siteid=" + SiteId : "&selectedsiteid=" + SelectedSiteId);
            }

            EditForm.RedirectUrlAfterCreate = sb.ToString();
        }

        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
        EditForm.OnAfterSave += EditForm_OnAfterSave;
    }


    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        if(SiteInfoProvider.GetSiteInfo(SiteId) != null)
        {
            EditForm.Data["EmailTemplateSiteID"] = SiteId;
        }
        else if (SiteInfoProvider.GetSiteInfo(SelectedSiteId) != null)
        {
            EditForm.Data["EmailTemplateSiteID"] = SelectedSiteId;
        }
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        if (EditForm.IsInsertMode)
        {
            // Selector ID in query means dialog mode
            string selector = QueryHelper.GetControlClientId("selectorid", string.Empty);
            if (!string.IsNullOrEmpty(selector))
            {
                EmailTemplateInfo templateInfo = (EmailTemplateInfo)EditForm.EditedObject;
                if (templateInfo != null)
                {
                    StringBuilder script = new StringBuilder();
                    // Add selector refresh
                    script.Append(@"
if (wopener) {{                        
    wopener.US_SelectNewValue_", selector, "('", templateInfo.TemplateName, @"');
}}");

                    // Prepare redirect URL for creation
                    StringBuilder redirectUrl = new StringBuilder(UrlResolver.ResolveUrl("~/CMSModules/EmailTemplates/Pages/Frameset.aspx"));
                    redirectUrl.Append("?templateid=", templateInfo.TemplateID, "&saved=1&tabmode=", QueryHelper.GetInteger("tabmode", 0));
                    redirectUrl.Append((SiteId > 0) ? "siteid=" + SiteId : "selectedsiteid=" + SelectedSiteId);
                    // Add dialog specific query parameters
                    redirectUrl.Append("&editonlycode=1");
                    redirectUrl.Append("&name=", templateInfo.TemplateName, "&selectorid=", selector);
                    // Add hash
                    redirectUrl.Append("&hash=", QueryHelper.GetHash("?editonlycode=1"));

                    script.Append(@"
window.name = '", selector, @"';
window.open('", URLHelper.UrlEncodeQueryString(redirectUrl.ToString()), "', window.name);");

                    ScriptHelper.RegisterStartupScript(this, GetType(), "UpdateSelector", script.ToString(), true);
                }
            }
        }
    }
}
