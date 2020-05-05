using System;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_AlternativeForms_AlternativeForms_Fields : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " FieldEditorBody";
        int altFormId = QueryHelper.GetInteger("objectid", 0);

        altFormFieldEditor.Mode = FieldEditorModeEnum.AlternativeClassFormDefinition;
        altFormFieldEditor.AlternativeFormID = altFormId;
        altFormFieldEditor.DisplayedControls = FieldEditorControlsEnum.DocumentTypes;
        altFormFieldEditor.EnableSystemFields = true;

        ScriptHelper.HideVerticalTabs(this);
    }
}
