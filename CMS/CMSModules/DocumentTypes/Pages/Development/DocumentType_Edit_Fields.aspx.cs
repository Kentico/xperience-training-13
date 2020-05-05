using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Fields : GlobalAdminPage
{
    #region "Private variables"

    private int classId;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " FieldEditorBody";

        // Get classId from query string
        classId = QueryHelper.GetInteger("objectid", 0);
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classId);
        FieldEditor.Visible = false;

        if (dci != null)
        {
            string className = dci.ClassName;

            // Init field editor
            if (dci.ClassIsCoupledClass)
            {
                FieldEditor.Visible = true;
                FieldEditor.ClassName = className;
                FieldEditor.Mode = FieldEditorModeEnum.ClassFormDefinition;
                FieldEditor.OnFieldNameChanged += FieldEditor_OnFieldNameChanged;
            }
            else
            {
                ShowWarning(GetString("EditTemplateFields.ErrorIsNotCoupled"));
                FieldEditor.Visible = false;
            }
        }

        ScriptHelper.HideVerticalTabs(this);
    }


    private void FieldEditor_OnFieldNameChanged(object sender, string oldFieldName, string newFieldName)
    {
        if (classId > 0)
        {
            // Rename field in layout(s)
            FormHelper.RenameFieldInFormLayout(classId, oldFieldName, newFieldName);
        }
    }
}
