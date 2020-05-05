using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;


/// <summary>
/// General control for object category selection.
/// </summary>
public partial class CMSFormControls_System_ObjectCategorySelector : FormEngineUserControl
{
    #region "Private variables"

    private List<int> mDisabledItems = new List<int>();
    private List<int> mExcludedItems = new List<int>();
    private GroupedDataSource mGroupedCategories;
    private GroupedDataSource mGroupedObjects;
    private DataSet mCategories;
    private MacroResolver mMacroResolver;
    private string mSelectedValue = String.Empty;

    // Indicates whether first selectable item was added. It's used to prevent first time load 'selection' of disabled item.
    private bool firstItem;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets current selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return correct value if control wasn't loaded yet
            return (!String.IsNullOrEmpty(drpCategory.SelectedValue)) ? drpCategory.SelectedValue : mSelectedValue;
        }
        set
        {
            mSelectedValue = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Child object type of category which you want show.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return GetValue("ObjectType", String.Empty);
        }
        set
        {
            SetValue("ObjectType", value);
        }
    }


    /// <summary>
    /// Indicates whether use allow empty in selector. Even though this property is set true, none is added only for object with no parent element. Use this for forms where object selecting their parents.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return GetValue("AllowEmpty", false);
        }
        set
        {
            SetValue("AllowEmpty", value);
        }
    }


    /// <summary>
    /// Sub-item prefix.
    /// </summary>
    public string SubItemPrefix
    {
        get
        {
            return GetValue("SubItemPrefix", "\xA0\xA0\xA0");
        }
        set
        {
            SetValue("SubItemPrefix", value);
        }
    }


    /// <summary>
    /// Category where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return GetValue("WhereCondition", String.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Child objects where condition.
    /// </summary>
    public string ObjectWhereCondition
    {
        get
        {
            return GetValue("ObjectWhereCondition", String.Empty);
        }
        set
        {
            SetValue("ObjectWhereCondition", value);
        }
    }


    /// <summary>
    /// Enabled item macro condition.
    /// </summary>
    public string EnabledCondition
    {
        get
        {
            return GetValue("EnabledCondition", String.Empty);
        }
        set
        {
            SetValue("EnabledCondition", value);
        }
    }


    /// <summary>
    /// Indicates if objects should be visible or their categories only.
    /// </summary>
    public bool ShowObjects
    {
        get
        {
            return GetValue("ShowObjects", false);
        }
        set
        {
            SetValue("ShowObjects", value);
        }
    }


    /// <summary>
    /// Indicates if empty categories should be visible.
    /// </summary>
    public bool ShowEmptyCategories
    {
        get
        {
            return GetValue("ShowEmptyCategories", true);
        }
        set
        {
            SetValue("ShowEmptyCategories", value);
        }
    }


    /// <summary>
    /// Indicates if root node should be visible.
    /// </summary>
    public bool ShowRoot
    {
        get
        {
            return GetValue("ShowRoot", false);
        }
        set
        {
            SetValue("ShowRoot", value);
        }
    }


    /// <summary>
    /// Value of node that would be a root.
    /// </summary>
    public int RootValue
    {
        get
        {
            return GetValue("RootValue", 0);
        }
        set
        {
            SetValue("RootValue", value);
        }
    }


    /// <summary>
    /// Starting path.
    /// </summary>
    public string StartingPath
    {
        get
        {
            return GetValue("StartingPath", String.Empty);
        }
        set
        {
            SetValue("StartingPath", value);
        }
    }


    /// <summary>
    /// Disabled item style.
    /// </summary>
    public string DisabledItemStyle
    {
        get
        {
            return GetValue("DisabledItemStyle", String.Empty);
        }
        set
        {
            SetValue("DisabledItemStyle", value);
        }
    }


    /// <summary>
    /// Disabled items.
    /// </summary>
    public List<int> DisabledItems
    {
        get
        {
            string disabledItems = GetValue<string>("DisabledItems", String.Empty);
            string[] items = disabledItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string val in items)
            {
                mDisabledItems.Add(ValidationHelper.GetInteger(val, 0));
            }

            return mDisabledItems;
        }
        set
        {
            mDisabledItems = value;
        }
    }


    /// <summary>
    /// Excluded items.
    /// </summary>
    public List<int> ExcludedItems
    {
        get
        {
            string excludedItems = GetValue<string>("ExcludedItems", String.Empty);
            string[] items = excludedItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string val in items)
            {
                mExcludedItems.Add(ValidationHelper.GetInteger(val, 0));
            }

            return mExcludedItems;
        }
        set
        {
            mExcludedItems = value;
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpCategory.Enabled;
        }
        set
        {
            drpCategory.Enabled = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets instance of macro resolver.
    /// </summary>
    private MacroResolver MacroResolver
    {
        get
        {
            if (mMacroResolver == null)
            {
                mMacroResolver = ContextResolver.CreateChild();
            }

            return mMacroResolver;
        }
    }


    /// <summary>
    /// Gets category info object from given object type.
    /// </summary>
    private BaseInfo CategoryInfo
    {
        get
        {
            if (!String.IsNullOrEmpty(CategoryColumn))
            {
                return ObjectInfo;
            }

            return ObjectInfo.TypeInfo.CategoryObject;
        }
    }


    /// <summary>
    /// Gets info object from given object type.
    /// </summary>
    private BaseInfo ObjectInfo
    {
        get
        {
            return ModuleManager.GetReadOnlyObject(ObjectType);
        }
    }


    /// <summary>
    /// Fake "category" column. Used when object type has no category column itself in TYPEINFO, and hierarchy is created with its own table.
    /// f.e. UIElement and it's column ElementParentID
    /// </summary>
    public String CategoryColumn
    {
        get
        {
            return GetValue("CategoryColumn", String.Empty);
        }
        set
        {
            SetValue("CategoryColumn", value);
        }
    }


    /// <summary>
    /// Gets collection of categories.
    /// </summary>
    private DataSet Categories
    {
        get
        {
            if (mCategories == null)
            {
                if (!String.IsNullOrEmpty(StartingPath))
                {
                    WhereCondition = SqlHelper.AddWhereCondition(WhereCondition, CategoryInfo.Generalized.TypeInfo.GetObjectPathWhereCondition(StartingPath));
                }
                mCategories = CategoryInfo.Generalized.GetData(null, WhereCondition, CategoryInfo.Generalized.TypeInfo.DefaultOrderBy, 0, String.Empty, false);
            }

            return mCategories;
        }
    }


    /// <summary>
    /// Gets grouped data source with all category objects grouped by CategoryIDColumn.
    /// </summary>
    private GroupedDataSource GroupedCategories
    {
        get
        {
            if (mGroupedCategories == null)
            {
                if (!DataHelper.DataSourceIsEmpty(Categories))
                {
                    mGroupedCategories = new GroupedDataSource(Categories, CategoryIDColumn);
                }
            }

            return mGroupedCategories;
        }
    }


    /// <summary>
    /// Gets grouped data source with all child objects grouped by CategoryIDColumn.
    /// </summary>
    private GroupedDataSource GroupedObjects
    {
        get
        {
            if (mGroupedObjects == null)
            {
                var objectInfo = ObjectInfo;
                var ti = objectInfo.TypeInfo;

                string objColumns = ti.CategoryIDColumn + "," + objectInfo.Generalized.DisplayNameColumn + "," + ti.IDColumn;
                DataSet dsObjects = objectInfo.Generalized.GetData(null, ObjectWhereCondition, ti.DefaultOrderBy, 0, objColumns, false);

                if (!DataHelper.DataSourceIsEmpty(dsObjects))
                {
                    mGroupedObjects = new GroupedDataSource(dsObjects, ti.CategoryIDColumn);
                }
            }

            return mGroupedObjects;
        }
    }


    /// <summary>
    /// Returns name of Category column. If this column name is not directly set, TYPEINFO is used.
    /// </summary>
    private String CategoryIDColumn
    {
        get
        {
            return String.IsNullOrEmpty(CategoryColumn) ? CategoryInfo.TypeInfo.CategoryIDColumn : CategoryColumn;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!StopProcessing)
        {
            // Check if settings are correct
            string errorMessage = ValidateSettings();

            if (string.IsNullOrEmpty(errorMessage))
            {
                SetupControl();

                if (!String.IsNullOrEmpty(mSelectedValue))
                {
                    drpCategory.SelectedValue = mSelectedValue;
                }
            }
            else
            {
                ShowError(errorMessage);
            }
        }
    }


    /// <summary>
    /// Setups control.
    /// </summary>
    private void SetupControl()
    {
        int mRootParentId = 0;

        // Load current root parent ID if starting path is defined
        if (!String.IsNullOrEmpty(StartingPath) && !DataHelper.DataSourceIsEmpty(Categories))
        {
            DataRow root = Categories.Tables[0].Rows[0];
            if (root != null)
            {
                mRootParentId = ValidationHelper.GetInteger(root[CategoryIDColumn], 0);
            }
        }

        // Save selected value for select correct value after reload items. 
        // This workaround is used because of viewstate that doesn't store item attributes such as CSSStyle
        if (RequestHelper.IsPostBack() && !String.IsNullOrEmpty(drpCategory.SelectedValue))
        {
            mSelectedValue = drpCategory.SelectedValue;
        }

        drpCategory.Items.Clear();

        FillDropDown(mRootParentId, 0);
    }


    /// <summary>
    /// Validates control settings.
    /// </summary>
    private string ValidateSettings()
    {
        string unknownColumn = ObjectTypeInfo.COLUMN_NAME_UNKNOWN;

        if (String.IsNullOrEmpty(ObjectType) || (ObjectInfo == null))
        {
            return "[ObjectCategorySelector]: The object type '" + ObjectType + "' is invalid or empty.";
        }
        
        if ((CategoryIDColumn == String.Empty) && ((ObjectInfo.Generalized.ObjectCategory == null) || (CategoryInfo.TypeInfo.CategoryIDColumn == unknownColumn)))
        {
            return "[ObjectCategorySelector]: The object type '" + ObjectType + "' has no hierarchical category object.";
        }
        
        if (ShowObjects && ((ObjectInfo.Generalized.DisplayNameColumn == unknownColumn) || (ObjectInfo.TypeInfo.IDColumn == unknownColumn)))
        {
            return "[ObjectCategorySelector]: 'Show objects' setting cannot be applied for binding object type '" + ObjectType + "'.";
        }
        
        if ((CategoryIDColumn == String.Empty) && DataHelper.DataSourceIsEmpty(GroupedCategories))
        {
            return "[ObjectCategorySelector]: No object categories were found for object type '" + ObjectType + "'.";
        }

        return string.Empty;
    }


    /// <summary>
    /// Fills dropdown with data.
    /// </summary>
    /// <param name="parentID">Identifier of node which children you want to add</param>
    /// <param name="level">Level of indentation</param>
    private void FillDropDown(int parentID, int level)
    {
        if (GroupedCategories != null)
        {
            List<DataRowView> gCategory = GroupedCategories.GetGroup(parentID);
            if (gCategory != null)
            {
                var categoryTypeInfo = CategoryInfo.TypeInfo;

                foreach (DataRowView drCategory in gCategory)
                {
                    if (!ExcludedItems.Contains(ValidationHelper.GetInteger(drCategory[categoryTypeInfo.IDColumn], 0)))
                    {
                        string name = ResHelper.LocalizeString(drCategory[CategoryInfo.Generalized.DisplayNameColumn].ToString());
                        int categoryId = ValidationHelper.GetInteger(drCategory[categoryTypeInfo.IDColumn], 0);

                        List<DataRowView> objectGroup = ((!ShowEmptyCategories || ShowObjects) && GroupedObjects != null) ? GroupedObjects.GetGroup(drCategory[categoryTypeInfo.IDColumn]) : null;
                        List<DataRowView> childCategoryGroup = !ShowEmptyCategories ? GroupedCategories.GetGroup(categoryId) : null;

                        // Check for empty categories except the root
                        if ((level == 0) || ShowEmptyCategories || ((objectGroup != null) && (objectGroup.Count > 0)) || ((childCategoryGroup != null) && (childCategoryGroup.Count > 0)))
                        {
                            bool enabled = true;

                            // Resolve Enabled condition
                            if (!String.IsNullOrEmpty(EnabledCondition))
                            {
                                MacroResolver.SetAnonymousSourceData(drCategory);
                                enabled = ValidationHelper.GetBoolean(MacroResolver.ResolveMacros(EnabledCondition), true);
                            }

                            string indentation = String.Empty;

                            if ((level > 0) || ShowRoot)
                            {
                                // Create indentation for specified level
                                indentation = String.Concat(Enumerable.Repeat(SubItemPrefix, ShowRoot ? level : level - 1));

                                string value = ((level == 0) && (RootValue > 0) ? RootValue : categoryId).ToString();
                                
                                // Prevent category list item to be selected by ID when selecting object
                                if (ShowObjects)
                                {
                                    value = "cat_" + value;
                                }

                                // Insert category
                                ListItem listItem = new ListItem(indentation + ResHelper.LocalizeString(name), value);
                                if (ShowObjects || DisabledItems.Contains(categoryId) || !enabled)
                                {
                                    listItem.Attributes.Add("style", DisabledItemStyle);
                                    listItem.Attributes.Add("disabled", "disabled");
                                }
                                else
                                {
                                    if (!firstItem && String.IsNullOrEmpty(mSelectedValue))
                                    {
                                        SetItem(parentID, value);
                                    }
                                }
                                drpCategory.Items.Add(listItem);
                            }

                            // Go deeper
                            FillDropDown(categoryId, level + 1);

                            // Insert all child objects if needed
                            if (ShowObjects && !DataHelper.DataSourceIsEmpty(objectGroup))
                            {
                                if (objectGroup != null)
                                {
                                    indentation += SubItemPrefix;
                                    foreach (DataRowView childObject in objectGroup)
                                    {
                                        string text = indentation + ResHelper.LocalizeString(childObject[ObjectInfo.Generalized.DisplayNameColumn].ToString());
                                        string objValue = childObject[ObjectInfo.TypeInfo.IDColumn].ToString();
                                        drpCategory.Items.Add(new ListItem(text, objValue));

                                        if (!firstItem && String.IsNullOrEmpty(mSelectedValue))
                                        {
                                            SetItem(parentID, objValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Set object value for drop down list
    /// </summary>
    /// <param name="parentId">Object's parent ID</param>
    /// <param name="objValue">Object's value</param>
    private void SetItem(int parentId, String objValue)
    {
        if ((parentId == 0) && AllowEmpty)
        {
            drpCategory.AllowEmpty = true;
            drpCategory.SelectedValue = "";
        }
        else
        {
            drpCategory.SelectedValue = objValue;
        }

        firstItem = true;
    }

    #endregion
}