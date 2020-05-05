using System;
using System.Collections;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Objects_FormControls_Cloning_CMS_CustomTableSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Excluded child types.
    /// </summary>
    public override string ExcludedChildTypes
    {
        get
        {
            return AlternativeFormInfo.OBJECT_TYPE;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (!ValidationHelper.IsIdentifier(txtTableName.Text))
        {
            ShowError(GetString("BizForm_Edit.ErrorFormTableNameInIdentifierFormat"));
            return false;
        }

        TableManager tm = new TableManager(null);
        if (tm.TableExists(txtTableName.Text))
        {
            ShowError(GetString("sysdev.class_edit_gen.tablenameunique"));
            return false;
        }
        return true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblCloneItems.ToolTip = GetString("clonning.settings.customtable.tooltip");
        lblTableName.ToolTip = GetString("clonning.settings.class.tablename.tooltip");
        lblCloneAlternativeForms.ToolTip = GetString("clonning.settings.class.alternativeform");

        if (!RequestHelper.IsPostBack())
        {
            TableManager tm = new TableManager(null);

            txtTableName.Text = tm.GetUniqueTableName(InfoToClone.GetStringValue("ClassTableName", ""));
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[DataClassInfo.OBJECT_TYPE + ".data"] = chkCloneItems.Checked;
        result[DataClassInfo.OBJECT_TYPE + ".tablename"] = txtTableName.Text;
        result[DataClassInfo.OBJECT_TYPE + ".alternativeforms"] = chkCloneAlternativeForms.Checked;
        return result;
    }

    #endregion
}