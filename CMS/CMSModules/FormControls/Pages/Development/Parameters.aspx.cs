using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.BIZFORM, "Properties")]
public partial class CMSModules_FormControls_Pages_Development_Parameters : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " FieldEditorBody";

        // If saved is found in query string
        if ((!RequestHelper.IsPostBack()) && (QueryHelper.GetInteger("saved", 0) == 1))
        {
            ShowChangesSaved();
        }

        // Load form control
        int controlId = QueryHelper.GetInteger("controlId", 0);
        if (controlId > 0)
        {
            FormControlFieldEditor.FormControlID = controlId;
        }

        ScriptHelper.HideVerticalTabs(this);
    }
}
