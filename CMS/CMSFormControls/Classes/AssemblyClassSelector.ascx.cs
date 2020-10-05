using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;

using Newtonsoft.Json;

public partial class CMSFormControls_Classes_AssemblyClassSelector : FormEngineUserControl, ICallbackEventHandler
{
    #region "Constants and variables"

    private const string APPCODE_PREFIX = ClassHelper.ASSEMBLY_APPCODE + ".";

    private readonly string customClassString = ResHelper.GetString("general.customclasses");
    private readonly string noneValue = ResHelper.GetString("general.empty");
    private ClassTypeSettings mSettings;
    private bool mAllowEmpty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets value for form engine
    /// </summary>
    public override object Value
    {
        get
        {
            return AssemblyName;
        }
        set
        {
            // Set assembly name
            AssemblyName = ValidationHelper.GetString(value, noneValue);

            LoadOtherValues();
        }
    }


    /// <summary>
    /// Gets a column name for full class name
    /// </summary>
    public string ClassNameColumnName
    {
        get
        {
            return GetValue("ClassNameColumnName", String.Empty);
        }
        set
        {
            SetValue("ClassNameColumnName", value);
        }
    }


    /// <summary>
    /// Gets or sets file system filter for list assemblies.
    /// </summary>
    public string AssembliesFilter
    {
        get
        {
            return GetValue("AssembliesFilter", "");
        }
        set
        {
            SetValue("AssembliesFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets base class names (separated by semi-colon) to filter classes.
    /// </summary>
    public string BaseClassNames
    {
        get
        {
            return GetValue("BaseClassName", String.Empty);
        }
        set
        {
            SetValue("BaseClassName", value);
            Settings.BaseClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets assembly name value to textbox. If 'custom class' is selected gets/sets App_Code.
    /// </summary>
    public string AssemblyName
    {
        get
        {
            string selectedValue = (SimpleMode) ? txtAssemblyName.Text : drpAssemblyName.SelectedValue;
            if (AllowEmpty && (selectedValue == noneValue))
            {
                return String.Empty;
            }

            return GetAssemblyName(selectedValue);
        }
        set
        {
            if (AllowEmpty && String.IsNullOrEmpty(value))
            {
                drpAssemblyName.SelectedValue = noneValue;
            }
            else
            {
                drpAssemblyName.SelectedValue = (value == ClassHelper.ASSEMBLY_APPCODE) ? customClassString : value;
            }

            txtAssemblyName.Text = drpAssemblyName.SelectedValue;
        }
    }


    /// <summary>
    /// Gets or sets full class name value to textbox.
    /// </summary>
    public string ClassName
    {
        get
        {
            return (SimpleMode) ? txtClassName.Text : drpClassName.SelectedValue;
        }
        set
        {
            drpClassName.SelectedValue = txtClassName.Text = value;
        }
    }


    /// <summary>
    /// Indicates if classes should be selected.
    /// </summary>
    public bool ShowClasses
    {
        get
        {
            return GetValue("ShowClasses", true);
        }
        set
        {
            SetValue("ShowClasses", value);
            Settings.ShowClasses = value;
        }
    }


    /// <summary>
    /// Indicates if enumerations should be selected.
    /// </summary>
    public bool ShowEnumerations
    {
        get
        {
            return GetValue("ShowEnumerations", false);
        }
        set
        {
            SetValue("ShowEnumerations", value);
            Settings.ShowEnumerations = value;
        }
    }


    /// <summary>
    /// Indicates if interfaces should be selected.
    /// </summary>
    public bool ShowInterfaces
    {
        get
        {
            return GetValue("ShowInterfaces", false);
        }
        set
        {
            SetValue("ShowInterfaces", value);
            Settings.ShowInterfaces = value;
        }
    }


    /// <summary>
    /// If true, the control will try to load the assembly and the class to validate it's availability. If false, this check is not done in Validate method.
    /// </summary>
    public bool ValidateAssembly
    {
        get
        {
            return GetValue("ValidateAssembly", true);
        }
        set
        {
            SetValue("ValidateAssembly", value);
        }
    }


    /// <summary>
    /// Indicates if empty value is allowed. Combines its value with FieldInfo.AllowEmpty
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return mAllowEmpty || ((FieldInfo != null) && FieldInfo.AllowEmpty);
        }
        set
        {
            mAllowEmpty = value;
        }
    }


    /// <summary>
    /// Gets error message.
    /// </summary>
    public override string ErrorMessage
    {
        get
        {
            return GetString("AssemblySelector.BadAssemblyOrClass");
        }
    }


    /// <summary>
    /// Gets current class selection settings.
    /// </summary>
    private ClassTypeSettings Settings
    {
        get
        {
            return mSettings ?? (mSettings = new ClassTypeSettings
            {
                BaseClassNames = BaseClassNames,
                ShowClasses = ShowClasses,
                ShowEnumerations = ShowEnumerations,
                ShowInterfaces = ShowInterfaces,
                CheckAutoCreation = CheckAutoCreation
            });
        }
    }


    /// <summary>
    /// Indicates whether automatic type creation by system should be checked.
    /// </summary>
    /// <seealso cref="ClassTypeSettings.CheckAutoCreation"/>
    public bool CheckAutoCreation
    {
        get
        {
            return GetValue("CheckAutoCreation", false);
        }
        set
        {
            SetValue("CheckAutoCreation", value);
            Settings.CheckAutoCreation = value;
        }
    }


    /// <summary>
    /// Indicates if the control is in simple mode in which textboxes are used instead of selectors.
    /// Default value is <c>false</c>.
    /// </summary>
    public bool SimpleMode
    {
        get
        {
            return GetValue("SimpleMode", false);
        }
        set
        {
            SetValue("SimpleMode", value);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        string classNameValue = (Form != null) ? ValidationHelper.GetString(Form.Data.GetValue(ClassNameColumnName), String.Empty) : String.Empty;
        ClassName = classNameValue;
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (!String.IsNullOrEmpty(ClassNameColumnName))
        {
            // Set properties names
            object[,] values = new object[1, 2];
            values[0, 0] = ClassNameColumnName;
            values[0, 1] = ClassName;
            return values;
        }
        return null;
    }


    /// <summary>
    /// Checks validity of inputs
    /// </summary>
    public override bool IsValid()
    {
        if (!ValidateAssembly)
        {
            return true;
        }

        // Do not validate values returned via textboxes
        if (SimpleMode)
        {
            return true;
        }

        // Do not validate if macros are used
        if (MacroProcessor.ContainsMacro(AssemblyName) || MacroProcessor.ContainsMacro(ClassName))
        {
            return true;
        }

        // Check allow empty setting
        if (AllowEmpty && (String.IsNullOrEmpty(AssemblyName) || String.IsNullOrEmpty(ClassName)))
        {
            return true;
        }

        try
        {
            // Check assembly and class
            Type classObject = ClassHelper.GetClassType(AssemblyName, ClassName);
            if (classObject != null)
            {
                // Check base class
                if (String.IsNullOrEmpty(BaseClassNames) || (ClassHelper.IsAllowed(classObject, Settings)))
                {
                    return true;
                }
            }
        }
        catch
        {
        }

        // Set validation error message
        ValidationError = ErrorMessage;

        return false;
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!StopProcessing && !RequestHelper.IsCallback() && !SimpleMode)
        {
            SetupControl();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        txtAssemblyName.Visible = txtClassName.Visible = SimpleMode;
        drpAssemblyName.Visible = drpClassName.Visible = !SimpleMode;
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        if (SimpleMode)
        {
            return;
        }

        foreach (ListItem item in drpAssemblyName.DropDownList.Items)
        {
            // Get assembly name
            string assemblyName = GetAssemblyName(item.Value);

            // Get classes
            var classes = ClassHelper.GetClasses(assemblyName, Settings);
            if (classes != null)
            {
                foreach (string className in classes)
                {
                    // Register all possible values for event validation
                    Page.ClientScript.RegisterForEventValidation(drpClassName.DropDownList.UniqueID, className);
                }
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets Assembly name from given value.
    /// </summary>
    /// <param name="value"></param>
    private string GetAssemblyName(string value)
    {
        // Get assembly name
        string assemblyName = value;
        if (!SimpleMode && ((!AllowEmpty && String.IsNullOrEmpty(assemblyName)) || (assemblyName == customClassString)))
        {
            assemblyName = ClassHelper.ASSEMBLY_APPCODE;
        }

        return assemblyName;
    }


    /// <summary>
    /// Ensures selector data.
    /// </summary>
    private void EnsureData()
    {
        FillAssemblySelector();
        FillClassNameSelector(AssemblyName);
    }


    /// <summary>
    /// Setups control.
    /// </summary>
    private void SetupControl()
    {
        EnsureData();

        // Register scripts
        ScriptHelper.RegisterJQueryUI(Page);
        
        var script = String.Format(@"
$cmsj(function() {{
    $cmsj('#{0}_txtCombo').on('autocompleteselect', function(event, ui) {{
        var assemblyName = ui.item.value;
        {1}
    }})

    $cmsj('#{0}_txtCombo').on('autocompletesearch', function(event, ui) {{        
        var assemblyName = $cmsj('#{0}_txtCombo').val();
        {1}
    }})

    function fillClassInput(returnedArrayAsJSON) {{
        var returnedArray = JSON.parse(returnedArrayAsJSON);
        $cmsj('#{2}_txtCombo').val(returnedArray[0]);
        $cmsj('#{2}_txtCombo').autocomplete('option', 'source', returnedArray);
    }}
}});
", drpAssemblyName.ClientID, Page.ClientScript.GetCallbackEventReference(this, "assemblyName", "fillClassInput", null), drpClassName.ClientID);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "acsDelayedLoad" + ClientID, ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Fills assembly selector with data.
    /// </summary>
    private void FillAssemblySelector()
    {
        if (AllowEmpty)
        {
            drpAssemblyName.DropDownList.Items.Add(noneValue);
        }

        var classes = ClassHelper.GetClasses(ClassHelper.ASSEMBLY_APPCODE, Settings);
        if ((classes != null) && (classes.Count > 0))
        {
            drpAssemblyName.DropDownList.Items.Add(customClassString);
        }

        // Get assembly names filtered by given restriction settings
        var assemblies = ClassHelper.GetAssemblyNames(AssembliesFilter, Settings)
            // Web site App_Code assembly are not supported. Custom class must be used instead
            .Where(assemblyName => !assemblyName.StartsWith(APPCODE_PREFIX, StringComparison.OrdinalIgnoreCase));

        // Fill assemblies list
        foreach (string assemblyName in assemblies)
        {
            drpAssemblyName.DropDownList.Items.Add(assemblyName);
        }

        // Include macro value if specified
        if (!String.IsNullOrEmpty(AssemblyName) && MacroProcessor.ContainsMacro(AssemblyName))
        {
            drpAssemblyName.DropDownList.Items.Insert(0, AssemblyName);
        }

        // Clear selected value if drop down is empty
        if (drpAssemblyName.DropDownList.Items.Count == 0)
        {
            drpAssemblyName.SelectedValue = String.Empty;
        }
        else
        {
            // Select first item if nothing was selected
            if (drpAssemblyName.SelectedIndex < 0 && String.IsNullOrEmpty(drpAssemblyName.SelectedValue))
            {
                drpAssemblyName.SelectedIndex = 0;
            }
        }
    }


    /// <summary>
    /// Fills class name selector with data.
    /// </summary>
    private void FillClassNameSelector(string assemblyName)
    {
        // Clear items
        drpClassName.DropDownList.Items.Clear();

        if (String.IsNullOrEmpty(assemblyName))
        {
            return;
        }

        if (MacroProcessor.ContainsMacro(assemblyName))
        {
            // Include macro value if specified
            drpClassName.DropDownList.Items.Add(ClassName);
            return;
        }

        var classes = ClassHelper.GetClasses(assemblyName, Settings);

        if (classes != null)
        {
            // Fill drop-down list
            foreach (string className in classes)
            {
                drpClassName.DropDownList.Items.Add(className);
            }

            // Select first item if nothing was selected
            if ((classes.Count > 0) && !AllowEmpty && (drpClassName.SelectedIndex < 0) && String.IsNullOrEmpty(drpClassName.SelectedValue))
            {
                drpClassName.SelectedIndex = 0;
            }
        }
        else
        {
            drpClassName.DropDownList.Items.Add(drpClassName.SelectedValue);
        }
    }

    #endregion


    #region "ICallbackEventHandler"

    public string GetCallbackResult()
    {
        List<string> array = new List<string>();

        foreach (var item in drpClassName.DropDownList.Items)
        {
            array.Add(((ListItem)item).Text);
        }

        return JsonConvert.SerializeObject(array, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
    }


    public void RaiseCallbackEvent(string eventArgument)
    {
        string assemblyName = GetAssemblyName(eventArgument);

        FillClassNameSelector(assemblyName);
    }

    #endregion
}