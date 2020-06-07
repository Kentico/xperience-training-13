using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


[UIElement(ModuleName.CUSTOMTABLES, "CustomTable.Fields")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Fields : CMSCustomTablesPage
{
    #region "Variables"

    protected DataClassInfo dci = null;
    protected string className = null;
    private FormInfo mFormInfo;
    private HeaderAction btnGenerateGuid;

    #endregion


    #region "Properties"

    /// <summary>
    /// Form info.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            if ((mFormInfo == null) && (dci != null))
            {
                mFormInfo = FormHelper.GetFormInfo(dci.ClassName, true);
            }
            return mFormInfo;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("objectid", 0);
        dci = DataClassInfoProvider.GetDataClassInfo(classId);
        // Set edited object
        EditedObject = dci;
        CurrentMaster.BodyClass += " FieldEditorBody";

        btnGenerateGuid = new HeaderAction
        {
            Tooltip = GetString("customtable.GenerateGUID"),
            Text = GetString("customtable.GenerateGUIDField"),
            Visible = false,
            CommandName = "createguid",
            OnClientClick = "return confirm(" + ScriptHelper.GetLocalizedString("customtable.generateguidconfirmation") + ");"
        };
        FieldEditor.HeaderActions.AddAction(btnGenerateGuid);
        FieldEditor.HeaderActions.ActionPerformed += (s, ea) => { if (ea.CommandName == "createguid") CreateGUID(); };

        // Class exists
        if (dci != null)
        {
            className = dci.ClassName;
            if (dci.ClassIsCoupledClass)
            {
                FieldEditor.Visible = true;
                FieldEditor.ClassName = className;
                FieldEditor.Mode = FieldEditorModeEnum.CustomTable;
                FieldEditor.OnFieldNameChanged += FieldEditor_OnFieldNameChanged;
                FieldEditor.OnAfterDefinitionUpdate += FieldEditor_OnAfterDefinitionUpdate;
            }
            else
            {
                FieldEditor.ShowError(GetString("customtable.ErrorNoFields"));
            }
        }

        ScriptHelper.HideVerticalTabs(this);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Class exists
        if (dci != null)
        {
            if (dci.ClassIsCoupledClass)
            {
                // GUID column is not present
                if ((FormInfo != null) && (FormInfo.GetFormField("ItemGUID") == null))
                {
                    btnGenerateGuid.Visible = true;
                    FieldEditor.ShowInformation(GetString("customtable.GUIDColumMissing"));
                }
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("gen", false))
            {
                FieldEditor.ShowInformation(GetString("customtable.GUIDFieldGenerated"));
            }
        }
    }


    private void FieldEditor_OnFieldNameChanged(object sender, string oldFieldName, string newFieldName)
    {
        if (dci != null)
        {
            // Rename field in layout(s)
            FormHelper.RenameFieldInFormLayout(dci.ClassID, oldFieldName, newFieldName);
        }
    }


    private void FieldEditor_OnAfterDefinitionUpdate(object sender, EventArgs e)
    {
        if (dci != null)
        {
            // State of unigrids may contain where/order by clauses no longer valid after definition update
            UniGrid.ResetStates(CustomTableItemProvider.GetObjectType(dci.ClassName));
        }
    }


    /// <summary>
    /// Adds GUID field to form definition.
    /// </summary>
    private void CreateGUID()
    {
        bool success;

        try
        {
            if (FormInfo == null)
            {
                return;
            }

            // Create GUID field
            FormFieldInfo ffiGuid = new FormFieldInfo();

            // Fill FormInfo object
            ffiGuid.Name = "ItemGUID";
            ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "GUID");
            ffiGuid.DataType = FieldDataType.Guid;
            ffiGuid.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, String.Empty);
            ffiGuid.PrimaryKey = false;
            ffiGuid.System = true;
            ffiGuid.Visible = false;
            ffiGuid.Size = 0;
            ffiGuid.AllowEmpty = false;

            FormInfo.AddFormItem(ffiGuid);

            // Update definition
            dci.ClassFormDefinition = FormInfo.GetXmlDefinition();

            using (CMSActionContext context = new CMSActionContext())
            {
                // Disable logging into event log
                context.LogEvents = false;

                DataClassInfoProvider.SetDataClassInfo(dci);
            }

            // Clear the default queries
            QueryInfoProvider.ClearDefaultQueries(dci, true);

            // Clear the object type hashtable
            AbstractProviderDictionary.ReloadDictionaries(className, true);

            // Clear the classes hashtable
            AbstractProviderDictionary.ReloadDictionaries("cms.class", true);

            // Clear class strucures
            ClassStructureInfo.Remove(className, true);

            // Ensure GUIDs for all items
            using (CMSActionContext ctx = new CMSActionContext())
            {
                ctx.UpdateSystemFields = false;
                ctx.LogSynchronization = false;
                DataSet dsItems = CustomTableItemProvider.GetItems(className);
                if (!DataHelper.DataSourceIsEmpty(dsItems))
                {
                    foreach (DataRow dr in dsItems.Tables[0].Rows)
                    {
                        CustomTableItem item = CustomTableItem.New(className, dr);
                        item.ItemGUID = Guid.NewGuid();
                        item.Update();
                    }
                }
            }

            // Log event
            UserInfo currentUser = MembershipContext.AuthenticatedUser;

            var logData = new EventLogData(EventTypeEnum.Information, "Custom table", "GENERATEGUID")
            {
                EventDescription = String.Format(ResHelper.GetAPIString("customtable.GUIDGenerated", "Field 'ItemGUID' for custom table '{0}' was created and GUID values were generated."), dci.ClassName),
                UserID = currentUser.UserID,
                UserName = currentUser.UserName,
            };
            
            Service.Resolve<IEventLogService>().LogEvent(logData);
            
            success = true;
        }
        catch (Exception ex)
        {
            success = false;

            FieldEditor.ShowError(GetString("customtable.ErrorGUID"));

            // Log event
            Service.Resolve<IEventLogService>().LogException("Custom table", "GENERATEGUID", ex);
        }

        if (success)
        {
            URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "gen", "1"));
        }
    }
}
