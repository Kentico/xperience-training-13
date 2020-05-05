using System;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_AllowedTypes : GlobalAdminPage
{
    #region "Variables"

    protected int classId = 0;

    private string currentValues = "";
    private string parentValues = "";

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        classId = QueryHelper.GetInteger("objectid", 0);

        if (classId > 0)
        {
            SetupControls();

            LoadData();
        }
    }


    /// <summary>
    /// Allowed child types selector event.
    /// </summary>
    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        bool reloadSecond = false;

        // Remove old items
        string newValues = ValidationHelper.GetString(uniSelector.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);

        if (!String.IsNullOrEmpty(items))
        {
            var newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to site
            foreach (string item in newItems)
            {
                int childId = ValidationHelper.GetInteger(item, 0);
                AllowedChildClassInfoProvider.RemoveAllowedChildClass(classId, childId);

                if (classId == childId)
                {
                    reloadSecond = true;
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            var newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to site
            foreach (string item in newItems)
            {
                int childId = ValidationHelper.GetInteger(item, 0);
                AllowedChildClassInfoProvider.AddAllowedChildClass(classId, childId);

                if (classId == childId)
                {
                    reloadSecond = true;
                }
            }
        }

        // Reload second selector
        if (reloadSecond)
        {
            LoadParentData(true);
            selParent.Reload(true);
        }

        ShowChangesSaved();
    }


    /// <summary>
    /// Allowed parent types selector event.
    /// </summary>
    protected void selParent_OnSelectionChanged(object sender, EventArgs e)
    {
        bool reloadSecond = false;

        // Remove old items
        string newValues = ValidationHelper.GetString(selParent.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, parentValues);
        if (!String.IsNullOrEmpty(items))
        {
            var newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to site
            foreach (string item in newItems)
            {
                int parentId = ValidationHelper.GetInteger(item, 0);
                AllowedChildClassInfoProvider.RemoveAllowedChildClass(parentId, classId);

                if (classId == parentId)
                {
                    reloadSecond = true;
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(parentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            var newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to site
            foreach (string item in newItems)
            {
                int parentId = ValidationHelper.GetInteger(item, 0);
                AllowedChildClassInfoProvider.AddAllowedChildClass(parentId, classId);

                if (classId == parentId)
                {
                    reloadSecond = true;
                }
            }
        }

        // Reload second unigrid
        if (reloadSecond)
        {
            LoadChildData(true);
            uniSelector.Reload(true);
        }

        ShowChangesSaved();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads data for both uniselectors.
    /// </summary>
    private void LoadData()
    {
        LoadChildData();

        LoadParentData();
    }


    /// <summary>
    /// Loads data for allowed child types selector.
    /// </summary>
    private void LoadChildData(bool forceReload = false)
    {
        DataSet ds = AllowedChildClassInfoProvider.GetAllowedChildClasses().WhereEquals("ParentClassID", classId);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "ChildClassID"));
            if (!RequestHelper.IsPostBack() || forceReload)
            {
                uniSelector.Value = currentValues;
            }
        }
        else
        {
            if (forceReload)
            {
                uniSelector.Value = String.Empty;
            }
        }
    }


    /// <summary>
    /// Loads data for allowed parent types selector.
    /// </summary>
    private void LoadParentData(bool forceReload = false)
    {
        // Get the active child classes
        DataSet ds = AllowedChildClassInfoProvider.GetAllowedChildClasses().WhereEquals("ChildClassID", classId);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            parentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "ParentClassID"));
            if (!RequestHelper.IsPostBack() || forceReload)
            {
                selParent.Value = parentValues;
            }
        }
        else
        {
            if (forceReload)
            {
                selParent.Value = String.Empty;
            }
        }
    }


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        uniSelector.ResourcePrefix = "allowedclasscontrol";
        uniSelector.DisplayNameFormat = "{%ClassDisplayName%} ({%ClassName%})";
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;

        selParent.ResourcePrefix = "allowedclasscontrol";
        selParent.DisplayNameFormat = "{%ClassDisplayName%} ({%ClassName%})";
        selParent.OnSelectionChanged += selParent_OnSelectionChanged;
    }

    #endregion
}