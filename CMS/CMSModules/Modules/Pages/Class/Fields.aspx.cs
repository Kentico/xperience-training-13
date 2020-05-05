using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Fields")]
public partial class CMSModules_Modules_Pages_Class_Fields : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetupControls();

        if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("gen", false))
        {
            fieldEditor.ShowInformation(GetString("EditTemplateFields.FormDefinitionGenerated"));
        }

        ScriptHelper.HideVerticalTabs(this);
    }


    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        // Get info on the class
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(QueryHelper.GetInteger("classid", 0));
        if (dci != null)
        {
            if (dci.ClassIsDocumentType && !dci.ClassIsCoupledClass)
            {
                ShowError(GetString("EditTemplateFields.ErrorIsNotCoupled"));
            }
            else
            {
                fieldEditor.Visible = true;
                fieldEditor.ClassName = dci.ClassName;

                ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(QueryHelper.GetInteger("moduleid", 0));
                bool devMode = SystemContext.DevelopmentMode;
                bool resourceIsEditable = (resource != null) && resource.IsEditable;
                bool classIsEditable = resourceIsEditable || dci.ClassShowAsSystemTable || devMode;

                // Allow development mode only for non-system tables
                fieldEditor.DevelopmentMode = resourceIsEditable;
                fieldEditor.Enabled = classIsEditable;
                fieldEditor.Mode = dci.ClassShowAsSystemTable ? FieldEditorModeEnum.SystemTable : FieldEditorModeEnum.ClassFormDefinition;

                if (devMode)
                {
                    // Add header action for generating default form definition
                    fieldEditor.HeaderActions.AddAction(new HeaderAction()
                    {
                        Text = GetString("EditTemplateFields.GenerateFormDefinition"),
                        Tooltip = GetString("EditTemplateFields.GenerateFormDefinition"),
                        OnClientClick = "if (!confirm('" + GetString("EditTemplateFields.GenerateFormDefConfirmation") + "')) {{ return false; }}",
                        Visible = !dci.ClassIsDocumentType,
                        CommandName = "gendefinition"
                    });

                    fieldEditor.HeaderActions.ActionPerformed += (s, ea) => { if (ea.CommandName == "gendefinition") GenerateDefinition(); };
                }
            }
        }
    }


    /// <summary>
    /// Generates default form definition.
    /// </summary>
    private void GenerateDefinition()
    {
        // Get info on the class
        var classInfo = DataClassInfoProvider.GetDataClassInfo(QueryHelper.GetInteger("classid", 0));
        if (classInfo == null)
        {
            return;
        }

        var manager = new TableManager(classInfo.ClassConnectionString);
        var fi = new FormInfo();

        try
        {
            fi.LoadFromDataStructure(classInfo.ClassTableName, manager, true);
        }
        catch (Exception ex)
        {
            // Show error message if something caused unhandled exception
            LogAndShowError("ClassFields", "GenerateDefinition", ex);
            return;
        }

        classInfo.ClassFormDefinition = fi.GetXmlDefinition();
        DataClassInfoProvider.SetDataClassInfo(classInfo);

        URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "gen", "1"));
    }
}
