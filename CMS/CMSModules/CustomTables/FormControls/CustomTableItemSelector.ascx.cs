using System;

using CMS.CustomTables;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_CustomTables_FormControls_CustomTableItemSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Custom table class name
    /// </summary>
    public string CustomTable 
    { 
        get
        {
            return ValidationHelper.GetString(GetValue("CustomTable"), null);
        }
        set
        {
            SetValue("CustomTable", value);
            uniSelector.ObjectType = CustomTableItemProvider.GetObjectType(value);
        }
    }


    /// <summary>
    /// Display name format (for example 'Item {%ItemGUID%}')
    /// </summary>
    public string DisplayNameFormat
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayNameFormat"), null);
        }
        set
        {
            SetValue("DisplayNameFormat", value);
            uniSelector.DisplayNameFormat = value;
        }
    }


    /// <summary>
    /// Return column name
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReturnColumnName"), null);
        }
        set
        {
            SetValue("ReturnColumnName", value);
            uniSelector.ReturnColumnName = value;
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
            base.Enabled = value;
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with class names.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets inner uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing || (CustomTable == null))
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }

    #endregion


    protected override void EnsureChildControls()
    {
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        base.EnsureChildControls();
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.DisplayNameFormat = DisplayNameFormat;
        uniSelector.ReturnColumnName = ReturnColumnName;
        uniSelector.DropDownSingleSelect.AutoPostBack = HasDependingFields;
        uniSelector.ObjectType = CustomTableItemProvider.GetObjectType(CustomTable);
        uniSelector.Reload(false);
    }
}