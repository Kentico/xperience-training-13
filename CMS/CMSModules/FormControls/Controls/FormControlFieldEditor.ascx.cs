using System;

using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.UIControls;


public partial class CMSModules_FormControls_Controls_FormControlFieldEditor : CMSAdminControl
{
    #region "Private properties"

    /// <summary>
    /// Current edited form control
    /// </summary>
    private FormUserControlInfo EditedFormControl
    {
        get
        {
            if (FormControlID > 0)
            {
                return FormUserControlInfoProvider.GetFormUserControlInfo(FormControlID);
            }
            else
            {
                return (FormUserControlInfo)UIContext.EditedObject;
            }
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets form control identifier.
    /// </summary>
    public int FormControlID
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        FormUserControlInfo fci = FormUserControlInfoProvider.GetFormUserControlInfo(FormControlID);
        UIContext.EditedObject = fci;

        if (fci != null)
        {
            // Set form definition
            fieldEditor.FormDefinition = fci.UserControlMergedParameters;

            // Set field editor settings
            fieldEditor.Mode = (fci.UserControlParentID > 0) ? FieldEditorModeEnum.InheritedFormControl : FieldEditorModeEnum.FormControls;
            fieldEditor.DisplayedControls = FieldEditorControlsEnum.ModeSelected;
            fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
            fieldEditor.AfterItemDeleted += fieldEditor_AfterItemDeleted;

            if (fieldEditor.Mode == FieldEditorModeEnum.InheritedFormControl)
            {
                // Allow extra fields for inherited form controls
                fieldEditor.AllowExtraFields = true;

                // Set original form definition from parent form control
                FormUserControlInfo parentFci = FormUserControlInfoProvider.GetFormUserControlInfo(fci.UserControlParentID);
                if (parentFci != null)
                {
                    fieldEditor.OriginalFormDefinition = parentFci.UserControlParameters;
                }
            }
            else
            {
                // Add custom delete confirmation message
                string confirmScript = String.Format(@"
var confirmElem = document.getElementById('confirmdelete');
if (confirmElem != null) {{ confirmElem.value = '{0}'; }}", ScriptHelper.GetString(GetString("formcontrolparams.confirmdelete"), false));

                ScriptHelper.RegisterStartupScript(this, typeof(string), "FormControlConfirmDelete_" + ClientID, confirmScript, true);
            }
        }
        else
        {
            ShowError(GetString("general.invalidid"));
        }
    }


    protected void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        // Update Form user control parameters
        if (EditedFormControl != null)
        {
            if (EditedFormControl.UserControlParentID > 0)
            {
                FormUserControlInfo parent = FormUserControlInfoProvider.GetFormUserControlInfo(EditedFormControl.UserControlParentID);
                // Get only differences
                EditedFormControl.UserControlParameters = FormHelper.GetFormDefinitionDifference(parent.UserControlParameters, fieldEditor.FormDefinition, true);
            }
            else
            {
                EditedFormControl.UserControlParameters = fieldEditor.FormDefinition;
            }

            FormUserControlInfoProvider.SetFormUserControlInfo(EditedFormControl);

            // Clear cached data
            FormUserControlInfoProvider.Clear(true);
        }
    }


    protected void fieldEditor_AfterItemDeleted(object sender, FieldEditorEventArgs e)
    {
        // Update parameters of inherited form user controls if a field or a category was deleted
        if ((e != null))
        {
            // Details of item which was deleted
            string itemName = e.ItemName;
            FieldEditorSelectedItemEnum itemType = e.ItemType;
            int itemOrder = e.ItemOrder;

            // Get form controls inherited from edited one
            var inheritedControls = FormUserControlInfoProvider.GetFormUserControls().WhereEquals("UserControlParentID", FormControlID);
            foreach (FormUserControlInfo control in inheritedControls)
            {
                // Remove deleted item (field/category) from inherited form control parameters
                switch (itemType)
                {
                    case FieldEditorSelectedItemEnum.Field:
                        control.UserControlParameters = FormHelper.RemoveFieldFromAlternativeDefinition(control.UserControlParameters, itemName, itemOrder);
                        break;

                    case FieldEditorSelectedItemEnum.Category:
                        control.UserControlParameters = FormHelper.RemoveCategoryFromAlternativeDefinition(control.UserControlParameters, itemName, itemOrder);
                        break;
                }

                // Update inherited form control
                FormUserControlInfoProvider.SetFormUserControlInfo(control);
            }
        }
    }

    #endregion
}
