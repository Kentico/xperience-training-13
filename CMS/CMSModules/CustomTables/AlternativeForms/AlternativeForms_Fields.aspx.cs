using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "AlternativeForm.Fields")]
public partial class CMSModules_CustomTables_AlternativeForms_AlternativeForms_Fields : CMSCustomTablesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int altFormId = QueryHelper.GetInteger("objectid", 0);
        CurrentMaster.BodyClass += " FieldEditorBody";

        altFormFieldEditor.Mode = FieldEditorModeEnum.AlternativeCustomTable;
        altFormFieldEditor.AlternativeFormID = altFormId;
        altFormFieldEditor.DisplayedControls = FieldEditorControlsEnum.CustomTables;

        ScriptHelper.HideVerticalTabs(this);
    }
}
