using System;

using CMS.Base;
using CMS.ContactManagement.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_ContactSelector : FormEngineUserControl
{
    #region "Events"

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
    /// Returns Uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether a postback to the server automatically
    /// occurs when the user changes the list selection.
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return uniSelector.DropDownSingleSelect.AutoPostBack;
        }
        set
        {
            uniSelector.DropDownSingleSelect.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Gets selected ContactID.
    /// </summary>
    public int ContactID
    {
        get
        {
            EnsureChildControls();
            return ValidationHelper.GetInteger(Value, 0);
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
    /// SQL WHERE condition of uniselector.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if uniselector is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uniSelector.Enabled;
        }
        set
        {
            base.Enabled = value;
            uniSelector.Enabled = value;
        }
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
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (string.IsNullOrEmpty(uniSelector.AdditionalSearchColumns))
        {
            uniSelector.FilterControl = "~/CMSModules/ContactManagement/FormControls/SearchContactFullName.ascx";
            uniSelector.UseDefaultNameFilter = false;
        }


        var isAuthorized = AuthorizationHelper.AuthorizedReadContact(false);

        uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, isAuthorized ? WhereCondition : "(1=0)");
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        if (ValidationHelper.GetInteger(uniSelector.Value, UniSelector.US_NONE_RECORD) == UniSelector.US_NONE_RECORD)
        {
            return FieldInfo.Name + " IS NULL";
        }
        else
        {
            return FieldInfo.Name + " = " + uniSelector.Value;
        }
    }


    /// <summary>
    /// Overrides base GetValue method and enables to return UniSelector control with 'uniselector' property name.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if (propertyName.EqualsCSafe("uniselector", true))
        {
            // Return uniselector control
            return UniSelector;
        }

        // Return other values
        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Overrides base SetValue method. Enables to set WhereCondition property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        string property = propertyName.ToLowerCSafe();

        switch (property)
        {
            case "wherecondition":
                // Set where condition
                WhereCondition = ValidationHelper.GetString(value, string.Empty);
                break;

            default:
                base.SetValue(propertyName, value);
                break;
        }

        return true;
    }

    #endregion
}