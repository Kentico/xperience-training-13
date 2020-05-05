using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.SiteProvider;


public partial class CMSFormControls_System_FieldSelector : FormEngineUserControl
{
    #region "Private properties"

    private string mValue = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Field condition
    /// </summary>
    public string FieldCondition
    {
        get
        {
            return GetValue("FieldCondition", String.Empty);
        }
        set
        {
            SetValue("FieldCondition", value);
        }
    }


    /// <summary>
    /// Class type
    /// </summary>
    /// <remarks>0 - Custom tables, 1 - Document types, 2 - System tables</remarks>
    public int ClassType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ClassType"), 1);
        }
        set
        {
            SetValue("ClassType", value);
        }
    }


    /// <summary>
    /// Gets class name value.
    /// </summary>
    private string ClassName
    {
        get
        {
            return !String.IsNullOrEmpty(ClassNameColumnName) ? ValidationHelper.GetString(Form.Data.GetValue(ClassNameColumnName), String.Empty) : String.Empty;
        }
    }


    /// <summary>
    /// Gets name of class name column.
    /// </summary>
    private string ClassNameColumnName
    {
        get
        {
            return GetValue("ClassNameColumnName", String.Empty);
        }
    }


    /// <summary>
    /// Indicates if (none) record should be visible in class list.
    /// </summary>
    private bool AllowNone
    {
        get
        {
            return ((FieldInfo != null) && (FieldInfo.AllowEmpty));
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return fieldSelector.SelectedValue;
        }
        set
        {
            mValue = ValidationHelper.GetString(value, String.Empty);
        }
    }

    #endregion


    #region "Page and controls events"

    /// <summary>
    /// Page_Load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup uni-selector
        classSelector.UniSelector.AllowEmpty = AllowNone;
        classSelector.DropDownSingleSelect.AutoPostBack = true;
        classSelector.IsLiveSite = false;
        classSelector.UniSelector.OnSelectionChanged += DropDownSingleSelect_SelectedIndexChanged;

        switch (ClassType)
        {
            // Custom tables
            case 0:
                classSelector.UniSelector.ObjectType = CustomTableInfo.OBJECT_TYPE_CUSTOMTABLE;
                break;

            // System tables
            case 2:
                classSelector.UniSelector.ObjectType = DataClassInfo.OBJECT_TYPE_SYSTEMTABLE;
                classSelector.ShowOnlySystemTables = true;
                break;

            // Document types
            default:
                classSelector.UniSelector.ObjectType = DocumentTypeInfo.OBJECT_TYPE_DOCUMENTTYPE;
                classSelector.ShowOnlyCoupled = true;
                break;
        }

        // Check backward compatibility
        if (mValue.Contains("|"))
        {
            SetupSelectorsObsolete();
        }
        else
        {
            SetupSelectors();
        }
    }


    /// <summary>
    /// Setups the class selector and field selector when the value is in an obsolete format "guid|text" (backward compatibility)
    /// </summary>
    private void SetupSelectorsObsolete()
    {
        string[] values = mValue.Split('|');
        Guid guid = new Guid(values[0]);

        string className = GetObsoleteSelectedClassName(guid);

        // Column found, preselect class name and column in drop down lists
        if (!String.IsNullOrEmpty(className))
        {
            SetupClassSelector(className);
            SetupFieldSelector(guid.ToString(), false);
        }
    }


    /// <summary>
    /// Setups the class selector and field selector 
    /// </summary>
    private void SetupSelectors()
    {
        SetupClassSelector(ClassName);
        SetupFieldSelector(mValue, false);
    }


    private void SetupClassSelector(string className)
    {
        if (!RequestHelper.IsPostBack())
        {
            classSelector.Value = className;
            classSelector.ReloadData(false);
        }
    }


    /// <summary>
    /// 'Selected index changed' event handler
    /// </summary>
    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetupFieldSelector(mValue, true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Resolves backward compatibility issue. 
    /// </summary>
    private string GetObsoleteSelectedClassName(Guid guid)
    {
        DataSet data;

        switch (ClassType)
        {
            // Custom tables
            case 0:
                {
                    data = CustomTableHelper.GetCustomTableClasses(SiteContext.CurrentSiteID)
                                                .Column("ClassName");
                }
                break;

            // System tables
            case 2:
                {
                    data = DataClassInfoProvider.GetClasses()
                                                .WhereTrue("ClassShowAsSystemTable")
                                                .Column("ClassName");
                    break;
                }

            // Document types
            default:
                {
                    data = DataClassInfoProvider.GetClasses()
                                                .OnSite(SiteContext.CurrentSiteID)
                                                .WhereTrue("ClassIsDocumentType")
                                                .WhereTrue("ClassIsCoupledClass")
                                                .Column("ClassName");
                    break;
                }
        }

        // Go through selected classes and try find right field
        foreach (DataRow row in data.Tables[0].Rows)
        {
            string className = row["ClassName"].ToString();
            var fi = FormHelper.GetFormInfo(className, false);

            if (fi != null)
            {
                // Find field with given GUID
                var ffi = fi.GetFields(true, true).FirstOrDefault(f => f.Guid == guid);
                if (ffi != null)
                {
                    return className;
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Loads fields to drop down control.
    /// </summary>
    private void SetupFieldSelector(string selectedValue, bool forceReload)
    {
        if (forceReload || (fieldSelector.Items.Count == 0))
        {
            // Clear dropdown list
            fieldSelector.Items.Clear();

            // Get data class info
            string className = ValidationHelper.GetString(classSelector.Value, null);
            if (!String.IsNullOrEmpty(className) && (className != SpecialFieldValue.NONE.ToString()))
            {
                // Get fields of type file
                var dci = DataClassInfoProvider.GetDataClassInfo(className);
                var fi = FormHelper.GetFormInfo(dci.ClassName, false);
                var ffi = fi.GetFields(true, true);

                // Filter fields
                if (!String.IsNullOrEmpty(FieldCondition))
                {
                    MacroResolver resolver = MacroResolver.GetInstance();
                    var visibleFields = new List<FormFieldInfo>();
                    foreach (FormFieldInfo field in ffi)
                    {
                        resolver.SetAnonymousSourceData(field);
                        bool result = ValidationHelper.GetBoolean(resolver.ResolveMacros(FieldCondition), true);
                        if (result)
                        {
                            visibleFields.Add(field);
                        }
                    }

                    ffi = visibleFields;
                }

                if (ffi.Count > 0)
                {
                    // Fill dropdown list with fields
                    var list = ffi.Select(t => new Tuple<string, string>(!String.IsNullOrEmpty(t.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, ContextResolver)) ? t.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, ContextResolver) : t.Name, t.Guid.ToString())).OrderBy(t => t.Item1);
                    foreach (var item in list)
                    {
                        fieldSelector.Items.Add(new ListItem(item.Item1, item.Item2));
                    }

                    if (!String.IsNullOrEmpty(selectedValue) && (fieldSelector.Items.FindByValue(selectedValue) != null) && !forceReload)
                    {
                        // Selected value from database
                        fieldSelector.SelectedValue = selectedValue;
                    }
                    else
                    {
                        // Select first item if nothing selected or class name was changed
                        fieldSelector.SelectedIndex = 0;
                    }

                    fieldSelector.Enabled = true;
                }
                else
                {
                    fieldSelector.Enabled = false;
                }

                pnlFields.Visible = true;
            }
            else
            {
                // Hide field selector for (none) option
                pnlFields.Visible = false;
            }
        }
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Class name loads on-the-fly, no need to load it explicitly
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (!String.IsNullOrEmpty(ClassNameColumnName))
        {
            string selectedValue = classSelector.DropDownSingleSelect.SelectedValue;

            // Set properties names
            object[,] values = new object[1, 2];
            values[0, 0] = ClassNameColumnName;
            values[0, 1] = selectedValue == "0" ? String.Empty : selectedValue;

            return values;
        }

        return null;
    }

    #endregion
}