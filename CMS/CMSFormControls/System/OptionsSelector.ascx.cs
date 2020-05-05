using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;


/// <summary>
/// Another blank form control must be registered to hold second value.
/// </summary>
public partial class CMSFormControls_System_OptionsSelector : FormEngineUserControl
{
    private int originalMode = -1;
    private string originalValue;


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtValue.Enabled;
        }
        set
        {
            txtValue.Enabled = value;
            lstOptions.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Options
            string text = txtValue.Text.Trim();
            if (lstOptions.SelectedIndex == NoneSourceIndex && !String.IsNullOrEmpty(text))
            {
                // Preselect options variant if something was inserted
                lstOptions.SelectedIndex = ListSourceIndex;
            }

            if (lstOptions.SelectedIndex == ListSourceIndex)
            {
                return text;
            }

            return null;
        }
        set
        {
            originalValue = LoadTextFromData(ValidationHelper.GetString(value, null));
            originalMode = lstOptions.SelectedIndex;
            txtValue.Text = originalValue;
        }
    }


    /// <summary>
    /// Name of the column to be used for SQL query
    /// </summary>
    public string QueryColumnName
    {
        get
        {
            return GetValue("QueryColumnName", "query");
        }
        set
        {
            SetValue("QueryColumnName", value);
        }
    }


    /// <summary>
    /// Name of the column to be used for macro data source
    /// </summary>
    public string MacroColumnName
    {
        get
        {
            return GetValue("MacroColumnName", "macro");
        }
        set
        {
            SetValue("MacroColumnName", value);
        }
    }


    /// <summary>
    /// Determines whether the Query data source option is allowed.
    /// </summary>
    public bool AllowQuery
    {
        get
        {
            return GetValue("AllowQuery", true);
        }
        set
        {
            SetValue("AllowQuery", value);
        }
    }


    /// <summary>
    /// Determines whether the Query option is allowed.
    /// </summary>
    public bool AllowMacro
    {
        get
        {
            return GetValue("AllowMacro", true);
        }
        set
        {
            SetValue("AllowMacro", value);
        }
    }


    /// <summary>
    /// Defines the data type of the edited field. This type is used for validation of the item values.
    /// </summary>
    public string EditedFieldDataType
    {
        get
        {
            return GetValue("EditedFieldDataType", FieldDataType.LongText);
        }
        set
        {
            SetValue("EditedFieldDataType", value);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Index of macro source in the list box.
    /// </summary>
    private int MacroSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Index of list source in the list box.
    /// </summary>
    private int ListSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Index of SQL source in the list box.
    /// </summary>
    private int SqlSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Index of None source in the list box.
    /// </summary>
    private int NoneSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether the list box is initialized
    /// </summary>
    private bool ListBoxInitialized
    {
        get;
        set;
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            CheckFieldEmptiness = false;

            // Control is used outside the form
            if ((Form == null) || (Form.Data == null))
            {
                ClearControl();
            }
            else
            {
                InitOptions();

                // Help icon
                imgHelp.ToolTip = ScriptHelper.FormatTooltipString(GetString("optionselector.help"), false, false);

                // Enable HTML formating in tooltip
                imgHelp.Attributes.Add("data-html", "true");

                // Register bootstrap tooltip over help icons
                ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");
            }
        }
        else
        {
            Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Init text area
        if (lstOptions.SelectedIndex == MacroSourceIndex)
        {
            txtValue.Editor.Language = LanguageEnum.CSharp;
        }
        else if (lstOptions.SelectedIndex == SqlSourceIndex)
        {
            txtValue.Editor.Language = LanguageEnum.SQL;
        }
        else
        {
            txtValue.Editor.Language = LanguageEnum.Text;
        }
        txtValue.UseAutoComplete = (lstOptions.SelectedIndex == MacroSourceIndex);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        bool valid = true;

        // Check 'options' validity
        if (lstOptions.SelectedIndex == ListSourceIndex)
        {
            // Some option must be included
            if (String.IsNullOrWhiteSpace(txtValue.Text))
            {
                ValidationError += GetString("TemplateDesigner.ErrorDropDownListOptionsEmpty") + " ";
                valid = false;
            }
            else
            {
                // Parse lines
                int lineIndex;
                string[] lines = txtValue.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                for (lineIndex = 0; lineIndex <= lines.GetUpperBound(0); lineIndex++)
                {
                    // Loop through only not-empty lines
                    string line = lines[lineIndex];
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        // Get line items
                        string[] items = line.Replace(SpecialFieldsDefinition.SEMICOLON_TO_REPLACE, SpecialFieldsDefinition.REPLACED_SEMICOLON).Split(';');

                        // Every line must have value and item element and optionally visibility macro
                        if (items.Length > 3)
                        {
                            ValidationError += GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidFormat") + " ";
                            valid = false;
                            break;
                        }

                        // Check for possible macro (with/without visibility condition)
                        if (items.Length <= 2)
                        {
                            string specialMacro = items[0].Trim();

                            // Check if special macro is used
                            if (SpecialFieldsDefinition.IsSpecialFieldMacro(specialMacro))
                            {
                                string macro = (items.Length == 2) ? items[1].Trim() : String.Empty;

                                // If special field macro used and second item isn't macro show error
                                if (!String.IsNullOrEmpty(macro) && !MacroProcessor.ContainsMacro(macro))
                                {
                                    ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidMacroFormat"), lineIndex + 1) + " ";
                                    valid = false;
                                    break;
                                }
                            }
                        }

                        // Validate input value
                        var checkType = new DataTypeIntegrity(items[0], EditedFieldDataType);

                        var result = checkType.GetValidationResult();
                        if (!result.Success)
                        {
                            ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidValue"), lineIndex + 1) + " " + result.ErrorMessage + " ";
                            valid = false;
                            break;
                        }
                    }
                }
            }
        }
        // Check SQL query validity
        else if ((lstOptions.SelectedIndex == SqlSourceIndex) && String.IsNullOrWhiteSpace(txtValue.Text))
        {
            ValidationError += GetString("TemplateDesigner.ErrorDropDownListQueryEmpty") + " ";
            valid = false;
        }
        else if ((lstOptions.SelectedIndex == MacroSourceIndex) && String.IsNullOrWhiteSpace(txtValue.Text))
        {
            ValidationError += GetString("TemplateDesigner.ErrorDropDownListMacroEmpty") + " ";
            valid = false;
        }

        return valid;
    }


    /// <summary>
    /// Returns the value of the given property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if (propertyName.EqualsCSafe("DataSourceType", StringComparison.InvariantCultureIgnoreCase))
        {
            return lstOptions.SelectedValue;
        }
        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Currently does not support loading other values explicitly
    }


    /// <summary>
    /// Returns values of other related fields.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[2, 2];
        values[0, 0] = QueryColumnName;
        values[1, 0] = MacroColumnName;

        // SQL
        if (lstOptions.SelectedIndex == SqlSourceIndex)
        {
            txtValue.Editor.ValueIsMacro = false;

            values[0, 1] = txtValue.Text.Trim();
            values[1, 1] = null;
        }
        // Macro
        else if (lstOptions.SelectedIndex == MacroSourceIndex)
        {
            txtValue.Editor.ValueIsMacro = true;

            values[0, 1] = null;
            values[1, 1] = String.Format("{{%{0}%}}", txtValue.Text.Trim());
        }
        // Plain text options (they are returned via Value property)
        else
        {
            values[0, 1] = null;
            values[1, 1] = null;
        }

        return values;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initialized shown options in the list box, correctly initialized the indexes of each options for further reference.
    /// </summary>
    private void InitOptions()
    {
        if (!ListBoxInitialized)
        {
            ListBoxInitialized = true;

            // Default indexes when all the options are visible
            NoneSourceIndex = 0;
            ListSourceIndex = 1;
            MacroSourceIndex = 2;
            SqlSourceIndex = 3;

            // Remove SQL option if there is not a field to store the query
            if (!Form.Data.ContainsColumn(QueryColumnName) || !AllowQuery)
            {
                lstOptions.Items.RemoveAt(SqlSourceIndex);
                SqlSourceIndex = -1;
            }
            // Remove macro option if there is not a field to store the macro
            if (!Form.Data.ContainsColumn(MacroColumnName) || !AllowMacro)
            {
                lstOptions.Items.RemoveAt(MacroSourceIndex);
                SqlSourceIndex--;
                MacroSourceIndex = -1;
            }

            // Hide None option for field not allowing the empty value
            if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
            {
                lstOptions.Items.RemoveAt(NoneSourceIndex);
                ListSourceIndex--;
                MacroSourceIndex--;
                SqlSourceIndex--;
                NoneSourceIndex = -1;
            }
        }
    }


    /// <summary>
    /// Loads text into textbox from value or from 'QueryColumnName' column.
    /// </summary>
    /// <param name="value">Value parameter</param>
    /// <returns>Returns text of options or query</returns>
    private string LoadTextFromData(string value)
    {
        InitOptions();
        txtValue.Editor.ValueIsMacro = false;

        // Options data 
        if (!String.IsNullOrEmpty(value))
        {
            lstOptions.SelectedIndex = ListSourceIndex;

            // Get string representation
            SpecialFieldsDefinition def = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver);
            def.LoadFromText(value);
            return def.ToString();
        }

        // Query selected
        if (ContainsColumn(QueryColumnName))
        {
            string query = ValidationHelper.GetString(Form.Data.GetValue(QueryColumnName), string.Empty).Trim();
            if (!String.IsNullOrEmpty(query))
            {
                lstOptions.SelectedIndex = SqlSourceIndex;
                return query;
            }
        }

        // Macro data source selected
        if (ContainsColumn(MacroColumnName))
        {
            string macro = ValidationHelper.GetString(Form.Data.GetValue(MacroColumnName), string.Empty).Trim();
            if (!String.IsNullOrEmpty(macro))
            {
                lstOptions.SelectedIndex = MacroSourceIndex;
                txtValue.Editor.ValueIsMacro = true;

                return MacroProcessor.RemoveDataMacroBrackets(macro);
            }
        }

        return null;
    }


    /// <summary>
    /// Resets control's state.
    /// </summary>
    private void ClearControl()
    {
        lstOptions.SelectedIndex = 0;
        txtValue.Text = null;
    }

    #endregion
}