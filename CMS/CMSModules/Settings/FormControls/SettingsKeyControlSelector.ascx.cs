using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Settings_FormControls_SettingsKeyControlSelector : FormEngineUserControl
{
    #region "Private members"

    private bool controlsLoaded;

    private const string STRING = "string";
    private const string INT = "int";

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets form control code name or user control file system path.
    /// </summary>
    public override object Value
    {
        get
        {
            return ControlPath;
        }
        set
        {
            throw new NotSupportedException("Setting value of SettingsKeyControlSelector is not supported. Use SetSelectorProperties method instead.");
        }
    }


    /// <summary>
    /// Gets or sets form control code name or user control file system path.
    /// </summary>
    public string ControlPath
    {
        get
        {
            if (radFormControl.Checked)
            {
                // Form control
                return (string)ucFormControlSelector.Value;
            }
            if (radFileSystem.Checked)
            {
                // File system path
                return (string)ucFileSystemSelector.Value;
            }
            // No control
            return null;
        }
        private set
        {
            LoadControls(value);
        }
    }


    /// <summary>
    /// Gets or sets a data type.
    /// </summary>
    public string DataType
    {
        get
        {
            // Convert form engine data types to settings data types (text->string, integer->int)
            if (ucFormControlSelector.DataType.EqualsCSafe(FieldDataType.Text, StringComparison.InvariantCultureIgnoreCase))
            {
                return STRING;
            }
            else if (ucFormControlSelector.DataType.EqualsCSafe(FieldDataType.Integer, StringComparison.InvariantCultureIgnoreCase))
            {
                return INT;
            }

            return ucFormControlSelector.DataType;
        }
        private set
        {
            // Convert settings data types to form engine data types (string->text, int->integer)
            if (value.EqualsCSafe(STRING, StringComparison.InvariantCultureIgnoreCase))
            {
                ucFormControlSelector.DataType = FieldDataType.Text;
            }
            else if (value.EqualsCSafe(INT, StringComparison.InvariantCultureIgnoreCase))
            {
                ucFormControlSelector.DataType = FieldDataType.Integer;
            }
            else
            {
                ucFormControlSelector.DataType = value;
            }
        }
    }


    /// <summary>
    /// Gets a value that indicates if the form control is selected.
    /// </summary>
    public bool IsFormControlSelected
    {
        get
        {
            return radFormControl.Checked;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            // Do nothing
            return;
        }

        // Setup file system selector
        var config = new FileSystemDialogConfiguration
        {
            AllowedExtensions = "ascx",
            ShowFolders = false,
        };
        ucFileSystemSelector.DialogConfig = config;

        if (!RequestHelper.IsPostBack() && !controlsLoaded)
        {
            // Initial load
            LoadControls(ControlPath);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        pnlFormControl.Visible = radFormControl.Checked;
        pnlFileSystem.Visible = radFileSystem.Checked;
    }


    protected void component_Changed(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }

    #endregion


    #region "Methods"

    private void LoadControls(string controlPath)
    {
        controlsLoaded = true;

        radDefault.Checked = false;
        radFormControl.Checked = false;
        radFileSystem.Checked = false;

        if (string.IsNullOrEmpty(controlPath))
        {
            // No control
            radDefault.Checked = true;
        }
        else if (controlPath.EndsWith(".ascx", StringComparison.Ordinal))
        {
            // File system path
            ucFileSystemSelector.Value = controlPath;
            radFileSystem.Checked = true;
        }
        else
        {
            // Form control
            ucFormControlSelector.Value = controlPath;
            radFormControl.Checked = true;
        }
    }


    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if (radFormControl.Checked && (string.IsNullOrEmpty((string)ucFormControlSelector.Value) || ((string)ucFormControlSelector.Value == "0")))
        {
            return false;
        }

        if (radFileSystem.Checked)
        {
            return ucFileSystemSelector.IsValid();
        }

        return true;
    }


    /// <summary>
    /// Sets selector properties
    /// </summary>
    /// <param name="dataType">Data type of the form selector we want to select</param>
    /// <param name="controlPath">Path or name of the form selector. Depends on type of control used to select the form selector</param>
    public void SetSelectorProperties(string dataType, string controlPath = null)
    {
        DataType = dataType;
        if (controlPath != null)
        {
            ControlPath = controlPath;
        }

        // Must be called after DataType change for the change to apply.
        // Also Controlpath must be set before this method is called. 
        // Otherwise (if not fixed in UniSelector) FormControlSelector will not return correct selected value.
        ucFormControlSelector.Reload(true);
    }

    #endregion
}