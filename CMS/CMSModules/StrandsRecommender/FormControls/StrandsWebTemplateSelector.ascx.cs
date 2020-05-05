using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.StrandsRecommender;

using Newtonsoft.Json;

public partial class CMSModules_StrandsRecommender_FormControls_StrandsWebTemplateSelector : FormEngineUserControl
{
    public override object Value
    {
        get
        {
            return hdnSelectedTemplate.Value;
        }
        set
        {
            hdnSelectedTemplate.Value = (string)value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (RequestHelper.IsPostBack())
        {
            return;
        }

        if (!StrandsSettings.IsStrandsEnabled(CurrentSite.SiteName))
        {
            ddlTemplates.Visible = false;

            // Ensures OK button won't update template
            hdnSelectedTemplate.Value = null;

            lblNoToken.Visible = true;
            lblNoToken.Text = GetString("strands.notoken");
        }
        else
        {
            ddlTemplates.Items.Add(new ListItem(GetString("general.loading")));

            RegisterScripts();
        }
    }


    /// <summary>
    /// Registers all necessary scripts.
    /// </summary>
    private void RegisterScripts()
    {
        // Ensure jQuery is loaded
        ScriptHelper.RegisterJQuery(Page);

        // Ensure application path is available in global namespace
        ScriptHelper.RegisterApplicationConstants(Page);

        // File with all common Strands module methods
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/StrandsRecommender/Scripts/StrandsModule.js");

        // File with all JavaScript logic related with this form control
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/StrandsRecommender/Scripts/WebTemplateSelector.js");

        // Creates init arguments object
        string initObject = JsonConvert.SerializeObject(
            new
            {
                dropDownList = ddlTemplates.ClientID,
                hdnSelectedTemplate = hdnSelectedTemplate.ClientID,
                defaultValue = Value
            },
            new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml }
        );
        var initScript = string.Format("STRANDS.initWebTemplateSelector({0});", initObject);

        // Call init method
        ScriptHelper.RegisterStartupScript(this, typeof(string), initScript, initScript, true);
    }
}