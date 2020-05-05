using System;
using System.Collections;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_BizForms_FormControls_Cloning_CMS_FormSettings : CloneSettingsControl
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

        if (!ValidationHelper.IsIdentifier(settings.CodeName))
        {
            ShowError(GetString("bizform_edit.errorformnameinidentifierformat"));
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
        lblCloneItems.ToolTip = GetString("clonning.settings.form.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            DataClassInfo classInfo = DataClassInfoProvider.GetDataClassInfo(InfoToClone.GetIntegerValue("FormClassID", 0));
            if (classInfo != null)
            {
                TableManager tm = new TableManager(null);

                txtTableName.Text = tm.GetUniqueTableName(classInfo.ClassTableName);
            }
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
        return result;
    }

    #endregion
}