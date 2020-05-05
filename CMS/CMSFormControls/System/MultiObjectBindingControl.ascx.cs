using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_System_MultiObjectBindingControl : FormEngineUserControl
{
    #region "Variables"

    private string mTargetObjectIDColumn = string.Empty;
    private string mCurrentObjectIDColumn = string.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets form control value. Selected items count is used.
    /// </summary>
    public override object Value
    {
        get
        {
            string items = ValidationHelper.GetString(uniSelector.Value, String.Empty);
            return items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Length;

        }
        set
        {
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Binding object type which stores binding relationship.
    /// </summary>
    public string BindingObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BindingObjectType"), string.Empty);
        }
        set
        {
            SetValue("BindingObjectType", value);
        }
    }


    /// <summary>
    /// Column name of current object in binding table.
    /// If empty, current object ID column is used.
    /// </summary>
    public string ObjectColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectColumnName"), string.Empty);
        }
        set
        {
            SetValue("ObjectColumnName", value);
        }
    }


    /// <summary>
    /// Column name of target object in binding table.
    /// If empty, target object ID column is used.
    /// </summary>
    public string TargetObjectColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TargetObjectColumnName"), string.Empty);
        }
        set
        {
            SetValue("TargetObjectColumnName", value);
        }
    }


    /// <summary>
    /// Type of object to create relationship with.
    /// </summary>
    public string TargetObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TargetObjectType"), string.Empty);
        }
        set
        {
            SetValue("TargetObjectType", value);
        }
    }


    /// <summary>
    /// Uni selector where condition
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), string.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Uni selector custom filter path
    /// </summary>
    public string FilterControl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterControl"), string.Empty);
        }
        set
        {
            SetValue("FilterControl", value);
        }
    }


    /// <summary>
    /// Determines whether to show default name filter.
    /// </summary>    
    public bool UseDefaultNameFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDefaultNameFilter"), true);
        }
        set
        {
            SetValue("UseDefaultNameFilter", value);
        }
    }


    /// <summary>
    /// Uni selector display name format.
    /// </summary>
    public string DisplayNameFormat
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayNameFormat"), string.Empty);
        }
        set
        {
            SetValue("DisplayNameFormat", value);
        }
    }


    /// <summary>
    /// Current form object info.
    /// </summary>
    private GeneralizedInfo CurrentObjectInfo
    {
        get
        {
            return (BaseInfo)Form.Data;
        }
    }


    /// <summary>
    /// Target object ID column name.
    /// </summary>
    private string TargetObjectIDColumn
    {
        get
        {
            if (string.IsNullOrEmpty(mTargetObjectIDColumn))
            {
                if (string.IsNullOrEmpty(TargetObjectColumnName))
                {
                    GeneralizedInfo obj = ModuleManager.GetReadOnlyObject(Form.ResolveMacros(TargetObjectType));
                    mTargetObjectIDColumn = obj.TypeInfo.IDColumn;
                }
                else
                {
                    mTargetObjectIDColumn = TargetObjectColumnName;
                }
            }

            return mTargetObjectIDColumn;
        }
    }


    /// <summary>
    /// Current object ID column name.
    /// </summary>
    private string CurrentObjectIDColumn
    {
        get
        {
            if (string.IsNullOrEmpty(mCurrentObjectIDColumn))
            {
                mCurrentObjectIDColumn = string.IsNullOrEmpty(ObjectColumnName) ? CurrentObjectInfo.TypeInfo.IDColumn : ObjectColumnName;
            }

            return mCurrentObjectIDColumn;
        }
    }

    #endregion


    #region "Life cycle"

    protected void Page_Load(object sender, EventArgs e)
    {
        if ((Form == null) || (CurrentObjectInfo == null))
        {
            return;
        }

        Form.OnAfterSave += Form_OnAfterSave;

        uniSelector.DisplayNameFormat = DisplayNameFormat;
        uniSelector.ObjectType = TargetObjectType;
        uniSelector.ResourcePrefix = ResourcePrefix;
        uniSelector.WhereCondition = WhereCondition;
        uniSelector.FilterControl = FilterControl;
        uniSelector.UseDefaultNameFilter = UseDefaultNameFilter;

        // Check if binding object type exists and has correct properties
        if (BindingObjectTypeExists(Form.ResolveMacros(BindingObjectType)))
        {
            if (!RequestHelper.IsPostBack())
            {
                // Set up original selector values
                uniSelector.Value = GetOriginalSelectorData();
            }
        }
        else
        {
            uniSelector.Visible = false;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Saves binding relationships for edited object.
    /// </summary>
    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        // Check if object for which are binding objects created exists
        if (CurrentObjectInfo == null)
        {
            return;
        }

        // Resolve binding object type name
        string resolvedObjectType = Form.ResolveMacros(BindingObjectType);

        // Update binding objects
        try
        {
            string originalValues = GetOriginalSelectorData();

            // Remove old items
            string newValues = ValidationHelper.GetString(uniSelector.Value, null);
            string items = DataHelper.GetNewItemsInList(newValues, originalValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                // Remove bindings
                foreach (string item in newItems)
                {
                    BaseInfo bindingObj = SetBindingObject(item.ToInteger(0), resolvedObjectType);
                    bindingObj.Delete();
                }
            }

            // Add new items
            items = DataHelper.GetNewItemsInList(originalValues, newValues);
            if (!String.IsNullOrEmpty(items))
            {
                string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                // Create new binding
                foreach (string item in newItems)
                {
                    BaseInfo bindingObj = SetBindingObject(item.ToInteger(0), resolvedObjectType);
                    bindingObj.Insert();
                }
            }
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("ObjectBinding", "BindObject", ex);
        }
    }


    /// <summary>
    /// Returns true if binding object type exists. Otherwise, logs exception in event log.
    /// </summary>
    /// <param name="objectType">Object type to check existence for.</param>
    private bool BindingObjectTypeExists(string objectType)
    {
        BaseInfo bindingObject = ModuleManager.GetReadOnlyObject(objectType);
        if ((bindingObject == null) || (bindingObject.ColumnNames.Count < 2) || (!bindingObject.ContainsColumn(CurrentObjectIDColumn)))
        {
            Service.Resolve<IEventLogService>().LogError("ObjectBinding", "BindObject", "Object " + CurrentObjectInfo.ObjectDisplayName + " cannot be bound.");
            return false;
        }

        return true;
    }


    /// <summary>
    /// Creates binding object.
    /// </summary>
    /// <param name="targetObjectID">ID of object to create relationship with.</param>
    /// <param name="bindingObjectType">Type of object which stores binding relationship</param>
    private BaseInfo SetBindingObject(int targetObjectID, string bindingObjectType)
    {
        BaseInfo bindingObj = ModuleManager.GetObject(bindingObjectType);

        bindingObj.SetValue(CurrentObjectIDColumn, CurrentObjectInfo.ObjectID);
        bindingObj.SetValue(TargetObjectIDColumn, targetObjectID);

        return bindingObj;
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Finds dataset of stored binding objects and joins target object IDs from this dataset using ; separator.
    /// </summary>
    private string GetOriginalSelectorData()
    {
        var bindingObjectQuery = new ObjectQuery(BindingObjectType);
        var bindedItemsIDs = bindingObjectQuery.Column(TargetObjectIDColumn)
                                               .WhereEquals(CurrentObjectIDColumn, CurrentObjectInfo.ObjectID);

        string originalValues = string.Empty;
        if (!DataHelper.DataSourceIsEmpty(bindedItemsIDs))
        {
            originalValues = TextHelper.Join(";", DataHelper.GetStringValues(bindedItemsIDs.Tables[0], TargetObjectIDColumn));
        }

        return originalValues;
    }

    #endregion
}