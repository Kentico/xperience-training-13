using System;
using System.Linq;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.UIControls;
using CMS.WorkflowEngine;


[EditedObject(WorkflowActionInfo.OBJECT_TYPE_AUTOMATION, "objectId")]
[UIElement(ModuleName.ONLINEMARKETING, "automation.processes.actions.parameters")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_Tab_Parameters : CMSContactManagementConfigurationPage
{
    #region "Private properties"

    private WorkflowActionInfo ActionInfo
    {
        get
        {
            return (WorkflowActionInfo)EditedObject;
        }
    }

    #endregion


    #region "Event handlers"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Only global administrator can access automation process actions
        if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = "";
        CurrentMaster.BodyClass += " FieldEditorBody";

        InitializeFieldEditor();

        ScriptHelper.HideVerticalTabs(this);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes field editor.
    /// </summary>
    private void InitializeFieldEditor()
    {
        if (ActionInfo == null)
        {
            return;
        }
        fieldEditor.FormDefinition = ActionInfo.ActionParameters;
        fieldEditor.OnAfterDefinitionUpdate += fieldEditor_OnAfterDefinitionUpdate;
    }


    /// <summary>
    /// Field editor updated event.
    /// </summary>
    private void fieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        if (ActionInfo != null)
        {
            ActionInfo.ActionParameters = fieldEditor.FormDefinition;
            WorkflowActionInfo.Provider.Set(ActionInfo);
        }
    }

    #endregion
}
