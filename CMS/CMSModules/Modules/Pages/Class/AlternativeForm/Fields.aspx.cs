using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Modules_Pages_Class_AlternativeForm_Fields : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get alternative form ID from URL
        int altFormId = QueryHelper.GetInteger("altformid", 0);
        CurrentMaster.BodyClass += " FieldEditorBody";

        // Get alternative form
        AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(altFormId);
        if (afi != null)
        {
            // Initialize field editor
            altFormFieldEditor.AlternativeFormID = altFormId;
        }

        ScriptHelper.HideVerticalTabs(this);
    }
}
