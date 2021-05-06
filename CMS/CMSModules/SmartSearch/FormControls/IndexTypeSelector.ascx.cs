using System;
using System.Collections.Generic;

using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Search;


/// <summary>
/// Represents a DDL selector for the search index type.
/// </summary>
public partial class CMSModules_SmartSearch_FormControls_IndexTypeSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the selected value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureData();

            return drpIndexType.SelectedValue;
        }
        set
        {
            EnsureData();

            var stringValue = ValidationHelper.GetString(value, null);
            drpIndexType.SelectedValue = stringValue;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        EnsureData();
    }


    private void EnsureData()
    {
        if (drpIndexType.Items.Count == 0)
        {
            LoadData();
        }
    }


    private void LoadData()
    {
        var searchIndex = EditedObject as SearchIndexInfo;

        // Create sorted list for drop down values
        var items = new SortedList<string, string>()
        {
            { GetString("smartsearch.indextype." + TreeNode.OBJECT_TYPE), TreeNode.OBJECT_TYPE },
            { GetString("smartsearch.indextype." + SearchHelper.GENERALINDEX), SearchHelper.GENERALINDEX }
        };

        if (searchIndex?.IndexProvider.Equals(SearchIndexInfo.LUCENE_SEARCH_PROVIDER, StringComparison.OrdinalIgnoreCase) ?? false)
        {
            items.Add(GetString("smartsearch.indextype." + UserInfo.OBJECT_TYPE), UserInfo.OBJECT_TYPE);
            items.Add(GetString("smartsearch.indextype." + CustomTableInfo.OBJECT_TYPE_CUSTOMTABLE), CustomTableInfo.OBJECT_TYPE_CUSTOMTABLE);
            items.Add(GetString("smartsearch.indextype." + SearchHelper.CUSTOM_SEARCH_INDEX), SearchHelper.CUSTOM_SEARCH_INDEX);
           
            // Allow on-line forms only if the module is available
            if ((ModuleManager.IsModuleLoaded(ModuleName.BIZFORM)))
            {
                items.Add(GetString("smartsearch.indextype." + SearchHelper.ONLINEFORMINDEX), SearchHelper.ONLINEFORMINDEX);
            }
        }

        // Data bind DDL
        drpIndexType.DataSource = items;
        drpIndexType.DataValueField = "value";
        drpIndexType.DataTextField = "key";
        drpIndexType.DataBind();

        // Pre-select documents
        drpIndexType.SelectedValue = TreeNode.OBJECT_TYPE;
    }
}