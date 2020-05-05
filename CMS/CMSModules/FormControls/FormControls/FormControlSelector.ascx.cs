using System;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;


public partial class CMSModules_FormControls_FormControls_FormControlSelector : FormEngineUserControl
{
    #region "Private variables"

    private bool showInheritedControls = true;

    #endregion


    #region "Public events"

    /// <summary>
    /// OnSelectionChanged event.
    /// </summary>
    public event EventHandler OnSelectionChanged
    {
        add
        {
            uniSelector.OnSelectionChanged += value;
        }
        remove
        {
            uniSelector.OnSelectionChanged -= value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Form control data type
    /// </summary>
    public string DataType
    {
        get;
        set;
    }


    /// <summary>
    /// Column name of the object which value should be returned by the selector. 
    /// If NULL, ID column is used.
    /// </summary>
    public string ReturnColumnName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if selector shows also inherited controls.
    /// </summary>
    public bool ShowInheritedControls
    {
        get
        {
            return showInheritedControls;
        }
        set
        {
            showInheritedControls = value;
        }
    }


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
            uniSelector.ReturnColumnName = ReturnColumnName;
            uniSelector.Value = value;

            // If value is not present, set it anyways. Used in field editor - simple mode. 
            // Not all controls are shown there, though one must get preselected.
            if (value != null && ForcedValueSet)
            {
                FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(value.ToString());
                if (control != null)
                {
                    uniSelector.SpecialFields.Add(new SpecialField { Text = control.UserControlDisplayName, Value = value.ToString() });
                }
            }
        }
    }


    /// <summary>
    /// Forces value of dropdownlist to be created as a special field.
    /// </summary>
    public bool ForcedValueSet
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if empty value item is visible in selection.
    /// </summary>
    public bool AllowEmptyValue
    {
        get
        {
            return uniSelector.AllowEmpty;
        }
        set
        {
            uniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Sets AutoPostBack of control.
    /// </summary>
    public bool AutoPostBack
    {
        set
        {
            uniSelector.DropDownSingleSelect.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Client ID of primary input control.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            return uniSelector.InputClientID;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        AddWhereConditions();
        uniSelector.ReturnColumnName = ReturnColumnName;
    }


    /// <summary>
    /// Adds where conditions based on inner properties.
    /// </summary>
    private void AddWhereConditions()
    {
        uniSelector.WhereCondition = WhereCondition;

        // Set conditions
        if (!ShowInheritedControls)
        {
            // Show only not inherited controls
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, "UserControlParentID IS NULL");
        }

        if (!String.IsNullOrEmpty(DataType))
        {
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, FormHelper.GetDataTypeWhereCondition(DataType));
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void Reload(bool forceReload)
    {
        AddWhereConditions();
        uniSelector.ReturnColumnName = ReturnColumnName;
        uniSelector.Reload(forceReload);
    }

    #endregion


    #region "View State Methods"

    protected override void LoadViewState(object savedState)
    {
        object[] updatedState = (object[])savedState;

        // Load orig ViewState
        if (updatedState[0] != null)
        {
            base.LoadViewState(updatedState[0]);
        }
        if (updatedState[1] != null)
        {
            WhereCondition = (string)updatedState[1];
        }
    }


    protected override object SaveViewState()
    {
        object[] updatedState = new object[2];
        updatedState[0] = base.SaveViewState();
        updatedState[1] = WhereCondition;
        return updatedState;
    }

    #endregion

}