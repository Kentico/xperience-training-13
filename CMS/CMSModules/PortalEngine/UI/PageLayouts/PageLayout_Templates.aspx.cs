using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Templates : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CheckGlobalAdministrator();
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        CurrentMaster.PanelContent.AddCssClass("dialog-content");

        // Filter templates for current layout
        gridTemplates.WhereCondition = "PageTemplateLayoutID = " + QueryHelper.GetInteger("layoutid", 0);
        gridTemplates.ZeroRowsText = GetString("layout.notemplates");
        gridTemplates.FilteredZeroRowsText = GetString("layout.notemplates");

        // Register edit script 
        String url = ApplicationUrlHelper.GetElementDialogUrl("cms.design", "PageTemplate.EditPageTemplate");
        string script = @"
function EditPageTemplate(id){
     modalDialog('" + url + @"&objectid=' + id, 'TemplateSelection', 1024, 768);
}";

        ScriptHelper.RegisterDialogScript(this.Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EditPageTemplate", ScriptHelper.GetScript(script));
    }
}
