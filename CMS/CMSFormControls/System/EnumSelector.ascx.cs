using System;

using CMS.Base.Web.UI;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


/// <summary>
/// Represents a DDL selector form control which initializes its items from a specified enum type.
/// </summary>
public partial class CMSFormControls_System_EnumSelector : FormEngineUserControl
{
    #region "Constants"

    private const char VALUE_SEPARATOR = ';';

    #endregion


    #region "Variables"

    private SpecialFieldsDefinition mSpecialItems;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the name of the assembly where the required enum is located.
    /// </summary>
    public string AssemblyName
    {
        get
        {
           return GetValue<string>("AssemblyName", null);
        }
        set
        {
            SetValue("AssemblyName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the enum type.
    /// </summary>
    public string TypeName
    {
        get
        {
            return GetValue<string>("TypeName", null);
        }
        set
        {
            SetValue("TypeName", value);
        }
    }


    /// <summary>
    /// Gets or sets a value that indicates if the items should be sorted.
    /// </summary>
    public bool Sort
    {
        get
        {
            return GetValue("Sort", false);
        }
        set
        {
            SetValue("Sort", value);
        }
    }
    

    /// <summary>
    /// Gets or sets a value that indicates if string representation should be used.
    /// </summary>
    public bool UseStringRepresentation
    {
        get
        {
            return GetValue("UseStringRepresentation", false);
        }
        set
        {
            SetValue("UseStringRepresentation", value);
        }
    }
    

    /// <summary>
    /// Gets or sets the display type for control (dropdown, radio button list - horizontal/vertical).
    /// </summary>
    public EnumDisplayTypeEnum DisplayType
    {
        get
        {
            return (EnumDisplayTypeEnum)GetValue("DisplayType", 0);
        }
        set
        {
            SetValue("DisplayType", (int)value);
        }
    }


    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureData();
            return GetSelectedValue();
        }
        set
        {
            EnsureData();
            SetSelectedValue(value);
        }
    }


    /// <summary>
    /// Gets or sets if inner control is enabled or not.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return CurrentSelector.Enabled;
        }
        set
        {
            CurrentSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the control from which the Value is retrieved or 
    /// null if such a control can't be specified
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return CurrentSelector.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets excluded values separated by semicolon.
    /// </summary>
    public string ExcludedValues
    {
        get
        {
            return GetValue("ExcludedValues", String.Empty);
        }
        set
        {
            SetValue("ExcludedValues", value);
        }

    }


    /// <summary>
    /// Gets or sets special items.
    /// </summary>
    public SpecialFieldsDefinition SpecialItems
    {
        get
        {
            if (mSpecialItems == null)
            {
                mSpecialItems = new SpecialFieldsDefinition(ResourcePrefix, FieldInfo, ContextResolver);
                mSpecialItems.LoadFromText(GetValue("Items", String.Empty));
            }

            return mSpecialItems;
        }
        set
        {
            SetValue("Items", ValidationHelper.GetString(value, null));
            mSpecialItems = null;
        }

    }


    /// <summary>
    /// Returns current control - RadioButtonList or DropDownList.
    /// </summary>
    public ListControl CurrentSelector
    {
        get
        {
            switch (DisplayType)
            {
                case EnumDisplayTypeEnum.DropDownList:
                    return drpEnum;

                case EnumDisplayTypeEnum.CheckBoxListHorizontal:
                    chkEnum.RepeatDirection = RepeatDirection.Horizontal;
                    return chkEnum;

                case EnumDisplayTypeEnum.CheckBoxListVertical:
                    chkEnum.RepeatDirection = RepeatDirection.Vertical;
                    return chkEnum;

                case EnumDisplayTypeEnum.RadioButtonsHorizontal:
                    radEnum.RepeatDirection = RepeatDirection.Horizontal;
                    return radEnum;

                default:
                    radEnum.RepeatDirection = RepeatDirection.Vertical;
                    return radEnum;
            }
        }
    }


    /// <summary>
    /// Gets or sets selected enum categories separated by semicolon.
    /// </summary>
    public string SelectedCategories
    {
        get
        {
            return GetValue("SelectedCategories", String.Empty);
        }
        set
        {
            SetValue("SelectedCategories", value);
        }
    }

    #endregion


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EnsureData();
    }


    private void EnsureData()
    {
        if (CurrentSelector.Items.Count == 0)
        {
            LoadData();
        }
    }


    private void LoadData()
    {
        // Stop processing if the assembly and type is not specified
        if (string.IsNullOrEmpty(AssemblyName) || string.IsNullOrEmpty(TypeName))
        {
            StopProcessing = true;
            return;
        }

        Type enumType = null;
        try
        {
            enumType = TryGetEnumType(AssemblyName, TypeName);
        }
        catch
        {
            drpEnum.Visible = false;
            chkEnum.Visible = false;
            radEnum.Visible = false;

            lblError.Visible = true;
            lblError.Text = string.Format(GetString("enumselector.cannotloadassembly"), TypeName);
        }

        // Stop processing if enum type is not found
        if (enumType == null)
        {
            StopProcessing = true;
            return;
        }

        // Initializes control
        SetupControl(enumType);
    }


    /// <summary>
    /// Initializes control due to settings
    /// </summary>
    private void SetupControl(Type enumType)
    {
        chkEnum.Visible = false;
        drpEnum.Visible = false;
        radEnum.Visible = false;

        var ctrl = CurrentSelector;
        ctrl.Visible = true;
        ctrl.Items.Clear();

        // Fill special items
        SpecialItems.FillItems(ctrl.Items);

        // Fill the control with enumeration values
        var excludedValues = !string.IsNullOrEmpty(ExcludedValues) ? ExcludedValues.Split(';').ToList() : null;
        var selectedCategories = !string.IsNullOrEmpty(SelectedCategories) ? SelectedCategories.Split(';').ToList() : null;

        ControlsHelper.FillListControlWithEnum(ctrl, enumType, null, Sort, UseStringRepresentation, excludedValues, selectedCategories);

        if (!String.IsNullOrEmpty(CssClass))
        {
            ctrl.AddCssClass(CssClass);
        }
    }


    private string GetSelectedValue()
    {
        string value = String.Empty;
        // Multiple selected items possible
        if (CurrentSelector.ID == chkEnum.ID)
        {
            foreach (ListItem item in chkEnum.Items)
            {
                if (item.Selected)
                {
                    value += item.Value + VALUE_SEPARATOR;
                }
            }
            value = value.Trim(VALUE_SEPARATOR);
        }
        else
        {
            value = CurrentSelector.SelectedValue;
        }

        return value;
    }


    private string GetUniqueValue(string value)
    {
        if (value == null)
        {
            return null;
        }

        return VALUE_SEPARATOR + value.Trim(VALUE_SEPARATOR) + VALUE_SEPARATOR;
    }


    private void SetSelectedValue(object value)
    {
        var stringValue = ValidationHelper.GetString(value, null);
        // Multiple selected items possible
        if (CurrentSelector.ID == chkEnum.ID)
        {
            stringValue = GetUniqueValue(stringValue);
            if (stringValue != null)
            {
                foreach (ListItem item in chkEnum.Items)
                {
                    item.Selected = stringValue.Contains(GetUniqueValue(item.Value));
                }
            }
        }
        else
        {
            var selectedItem = CurrentSelector.Items.FindByValue(stringValue);
            CurrentSelector.SelectedIndex = selectedItem != null ? CurrentSelector.Items.IndexOf(selectedItem) : 0;
        }
    }


    #region "Reflection"

    private Type TryGetEnumType(string assemblyname, string typeName)
    {
        var type = ClassHelper.GetClassType(assemblyname, typeName);

        if ((type != null) && type.IsEnum)
        {
            return type;
        }

        return null;
    }

    #endregion
}