using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Base;

public partial class CMSModules_ContactManagement_FormControls_ContactRoleSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return uniSelector.Value;
        }
        set
        {
            EnsureChildControls();
            uniSelector.Value = ValidationHelper.GetString(value, "");
        }
    }


    /// <summary>
    /// Returns Uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniSelector;
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows selection of all roles
    /// </summary>
    public bool AllowAllRoles
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAllRoles"), false);
        }
        set
        {
            SetValue("AllowAllRoles", value);
        }
    }


    /// <summary>
    /// CMSDropDownList used in Uniselector.
    /// </summary>
    public CMSDropDownList DropDownList
    {
        get
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// SQL WHERE condition of uniselector.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }

    
    /// <summary>
    /// Reloads control.
    /// </summary>
    public void ReloadData()
    {
        string where = WhereCondition;
        uniSelector.AllowAll = AllowAllRoles;

        // Do not add condition to empty condition which allows everything
        if (!String.IsNullOrEmpty(where))
        {
            string role = ValidationHelper.GetString(Value, "");
            if (!String.IsNullOrEmpty(role))
            {
                where = SqlHelper.AddWhereCondition(where, String.Format("{0} = {1}", SqlHelper.EscapeQuotes(uniSelector.ReturnColumnName), SqlHelper.EscapeQuotes(role)), "OR");
            }
        }

        uniSelector.WhereCondition = where;
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        switch (uniSelector.Value.ToInteger(GetDefaultValue()))
        {
            case UniSelector.US_ALL_RECORDS:
                return GetWhereConditionForAllRecords();
            case UniSelector.US_NONE_RECORD:
                return GetWhereConditionForNullRecords();
            default:
                return GetWhereConditionForExactMatch();
        }
    }


    /// <summary>
    /// Sets the given <paramref name="value"/> as the <see cref="Value"/> of current selector. In case <c>null</c> is passed, 
    /// sets value denoting all roles should be displayed.
    /// </summary>
    public override void LoadControlValue(object value)
    {
        base.LoadControlValue(value);

        if (value == null)
        {
            Value = UniSelector.US_ALL_RECORDS;
        }
    }


    private int GetDefaultValue()
    {
        return AllowAllRoles ? UniSelector.US_ALL_RECORDS : UniSelector.US_NONE_RECORD;
    }


    private string GetWhereConditionForExactMatch()
    {
        return string.Format("{0} = {1}", FieldInfo.Name, uniSelector.Value);
    }


    private string GetWhereConditionForNullRecords()
    {
        return string.Format("{0} IS NULL", FieldInfo.Name);
    }


    private static string GetWhereConditionForAllRecords()
    {
        return string.Empty;
    }
    #endregion
}