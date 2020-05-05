using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Search;
using CMS.Search.Azure;
using CMS.UIControls;

public partial class CMSModules_SmartSearch_Controls_Edit_ClassFields : CMSAdminEditControl
{
    #region "Private variables"

    private SearchSettings mFields;
    private DataClassInfo mDci;
    private bool mDisplayIField = true;
    private bool mDisplayAzureFields = true;
    private DataSet mInfos;
    private List<ColumnDefinition> mAttributes = new List<ColumnDefinition>();
    private bool mLoaded;
    private bool mDisplaySaved = true;
    private SearchSettings mSearchSettings;
    private bool mDisplaySetAutomatically = true;

    private static readonly Dictionary<string, Dictionary<string, HashSet<string>>> mRequiredFields = new Dictionary<string, Dictionary<string, HashSet<string>>>(StringComparer.OrdinalIgnoreCase)
    {
        {
            PredefinedObjectType.DOCUMENT, new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "DocumentId", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AzureSearchFieldFlags.RETRIEVABLE, AzureSearchFieldFlags.FILTERABLE, SearchSettings.SEARCHABLE } },
                { "NodeID", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AzureSearchFieldFlags.RETRIEVABLE, AzureSearchFieldFlags.FILTERABLE, SearchSettings.SEARCHABLE } },
                { "NodeLinkedNodeID", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AzureSearchFieldFlags.RETRIEVABLE, AzureSearchFieldFlags.FILTERABLE, SearchSettings.SEARCHABLE } },
                { "NodeAliasPath", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AzureSearchFieldFlags.RETRIEVABLE, AzureSearchFieldFlags.FILTERABLE, SearchSettings.SEARCHABLE } },
            }
        }
    };

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicates if Azure Search related flags should be displayed. The default value is true.
    /// </summary>
    public bool DisplayAzureFields
    {
        get
        {
            return mDisplayAzureFields;
        }
        set
        {
            mDisplayAzureFields = value;
        }
    }


    /// <summary>
    /// Indicates if iField column should be displayed. The default value is true.
    /// </summary>
    public bool DisplayIField
    {
        get
        {
            return mDisplayIField;
        }
        set
        {
            mDisplayIField = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether "Set automatically" button should be visible.
    /// </summary>
    public bool DisplaySetAutomatically
    {
        get
        {
            return mDisplaySetAutomatically;
        }
        set
        {
            mDisplaySetAutomatically = value;
        }
    }


    /// <summary>
    /// Indicates if "Changes were saved" info label should be displayed.
    /// </summary>
    public bool DisplaySaved
    {
        get
        {
            return mDisplaySaved;
        }
        set
        {
            mDisplaySaved = value;
        }
    }


    /// <summary>
    /// Use after item saved, if true, relevant data for index rebuilt was changed.
    /// </summary>
    public bool Changed
    {
        get;
        private set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!DisplaySetAutomatically)
        {
            pnlButton.Visible = false;
        }

        ReloadData(false, false);
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    /// <param name="setAutomatically">Indicates if table should be pre-set according to field type</param>
    /// <param name="forceReload">Forces data to reload even if they are already loaded.</param>
    public void ReloadData(bool setAutomatically, bool forceReload)
    {
        if (!mLoaded || forceReload)
        {
            mLoaded = true;
            pnlContent.Controls.Clear();

            if (ItemID > 0)
            {
                var dataClassInfo = DataClassInfoProvider.GetDataClassInfo(ItemID);

                mDci = dataClassInfo;
                mAttributes = dataClassInfo.GetSearchIndexColumns()?.ToList();

                if (mDci != null)
                {
                    mSearchSettings = new SearchSettings();
                    mSearchSettings.LoadData(mDci.ClassSearchSettings);
                    if ((mDci.TypeInfo.ObjectType == PredefinedObjectType.CUSTOMTABLECLASS) || (mDci.TypeInfo.ObjectType == PredefinedObjectType.FORMCLASS))
                    {
                        DisplayAzureFields = false;
                    }
                }
            }

            btnAutomatically.Click += btnAutomatically_Click;

            // Display checkbox matrix only if field names array is not empty
            if (mAttributes?.Count > 0)
            {
                // Setup controls
                btnAutomatically.Visible = true;
                if (mFields == null)
                {
                    mFields = new SearchSettings();
                }

                if (mDci != null)
                {
                    mFields.LoadData(mDci.ClassSearchSettings);
                }
                mInfos = mFields.GetAllSettingsInfos();

                CreateTable(setAutomatically);

                Literal ltl = new Literal();
                ltl.Text = "<br />";
                pnlContent.Controls.Add(ltl);
            }
        }
    }


    /// <summary>
    /// Creates table.
    /// </summary>
    private void CreateTable(bool useDefaultValue)
    {
        Table table = new Table();
        table.CssClass = "table table-hover";
        table.CellPadding = -1;
        table.CellSpacing = -1;

        // Create table header
        TableHeaderRow topHeader = new TableHeaderRow();
        TableHeaderRow header = new TableHeaderRow();
        topHeader.TableSection = TableRowSection.TableHeader;
        header.TableSection = TableRowSection.TableHeader;

        AddTableHeaderCell(topHeader, "");
        AddTableHeaderCell(topHeader, GetString("srch.local"), false, 3);

        AddTableHeaderCell(header, GetString("srch.settings.fieldname"), true);
        AddTableHeaderCell(header, GetString("development.content"), true);
        AddTableHeaderCell(header, GetString("srch.settings.searchable"), true);
        AddTableHeaderCell(header, GetString("srch.settings.tokenized"), true);

        if (DisplayAzureFields)
        {
            AddTableHeaderCell(topHeader, GetString("srch.azure"), false, 6);

            AddTableHeaderCell(header, GetString("srch.settings." + AzureSearchFieldFlags.CONTENT), true);
            AddTableHeaderCell(header, GetString("srch.settings." + AzureSearchFieldFlags.RETRIEVABLE), true);
            AddTableHeaderCell(header, GetString("srch.settings." + AzureSearchFieldFlags.SEARCHABLE), true);

            AddTableHeaderCell(header, GetString("srch.settings." + AzureSearchFieldFlags.FACETABLE), true);
            AddTableHeaderCell(header, GetString("srch.settings." + AzureSearchFieldFlags.FILTERABLE), true);
            AddTableHeaderCell(header, GetString("srch.settings." + AzureSearchFieldFlags.SORTABLE), true);
        }

        if (DisplayIField)
        {
            AddTableHeaderCell(topHeader, GetString("general.general"));
            AddTableHeaderCell(header, GetString("srch.settings.ifield"), true);
        }

        var thc = new TableHeaderCell();
        thc.CssClass = "main-column-100";
        topHeader.Cells.Add(thc);

        thc = new TableHeaderCell();
        thc.CssClass = "main-column-100";
        header.Cells.Add(thc);

        table.Rows.Add(topHeader);
        table.Rows.Add(header);
        pnlContent.Controls.Add(table);

        // Create table content
        if ((mAttributes != null) && (mAttributes.Count > 0))
        {
            // Create row for each field
            foreach (ColumnDefinition column in mAttributes)
            {
                SearchSettingsInfo ssi = null;
                TableRow tr = new TableRow();
                if (!DataHelper.DataSourceIsEmpty(mInfos))
                {
                    DataRow[] dr = mInfos.Tables[0].Select("name = '" + column.ColumnName + "'");
                    if ((dr.Length > 0) && (mSearchSettings != null))
                    {
                        ssi = mSearchSettings.GetSettingsInfo((string)dr[0]["id"]);
                    }
                }

                // Add cell with field name
                TableCell tc = new TableCell();
                Label lbl = new Label();
                lbl.Text = column.ColumnName;
                tc.Controls.Add(lbl);
                tr.Cells.Add(tc);

                var defaultSearchSettings = useDefaultValue ? SearchHelper.CreateDefaultSearchSettings(column.ColumnName, column.ColumnType) : null;

                tr.Cells.Add(CreateTableCell(SearchSettings.CONTENT, column, useDefaultValue ? defaultSearchSettings.GetFlag(SearchSettings.CONTENT) : ssi?.GetFlag(SearchSettings.CONTENT) ?? false, "development.content"));
                tr.Cells.Add(CreateTableCell(SearchSettings.SEARCHABLE, column, useDefaultValue ? defaultSearchSettings.GetFlag(SearchSettings.SEARCHABLE) : ssi?.GetFlag(SearchSettings.SEARCHABLE) ?? false, "srch.settings.searchable"));
                tr.Cells.Add(CreateTableCell(SearchSettings.TOKENIZED, column, useDefaultValue ? defaultSearchSettings.GetFlag(SearchSettings.TOKENIZED) : ssi?.GetFlag(SearchSettings.TOKENIZED) ?? false, "srch.settings.tokenized"));

                if (DisplayAzureFields)
                {
                    tr.Cells.Add(CreateTableCell(AzureSearchFieldFlags.CONTENT, column, useDefaultValue ? defaultSearchSettings.GetFlag(AzureSearchFieldFlags.CONTENT) : ssi?.GetFlag(AzureSearchFieldFlags.CONTENT) ?? false, "srch.settings." + AzureSearchFieldFlags.CONTENT));
                    tr.Cells.Add(CreateTableCell(AzureSearchFieldFlags.RETRIEVABLE, column, useDefaultValue ? defaultSearchSettings.GetFlag(AzureSearchFieldFlags.RETRIEVABLE) : ssi?.GetFlag(AzureSearchFieldFlags.RETRIEVABLE) ?? false, "srch.settings." + AzureSearchFieldFlags.RETRIEVABLE));
                    tr.Cells.Add(CreateTableCell(AzureSearchFieldFlags.SEARCHABLE, column, useDefaultValue ? defaultSearchSettings.GetFlag(AzureSearchFieldFlags.SEARCHABLE) : ssi?.GetFlag(AzureSearchFieldFlags.SEARCHABLE) ?? false, "srch.settings." + AzureSearchFieldFlags.SEARCHABLE));

                    tr.Cells.Add(CreateTableCell(AzureSearchFieldFlags.FACETABLE, column, useDefaultValue ? defaultSearchSettings.GetFlag(AzureSearchFieldFlags.FACETABLE) : ssi?.GetFlag(AzureSearchFieldFlags.FACETABLE) ?? false, "srch.settings." + AzureSearchFieldFlags.FACETABLE));
                    tr.Cells.Add(CreateTableCell(AzureSearchFieldFlags.FILTERABLE, column, useDefaultValue ? defaultSearchSettings.GetFlag(AzureSearchFieldFlags.FILTERABLE) : ssi?.GetFlag(AzureSearchFieldFlags.FILTERABLE) ?? false, "srch.settings." + AzureSearchFieldFlags.FILTERABLE));
                    tr.Cells.Add(CreateTableCell(AzureSearchFieldFlags.SORTABLE, column, useDefaultValue ? defaultSearchSettings.GetFlag(AzureSearchFieldFlags.SORTABLE) : ssi?.GetFlag(AzureSearchFieldFlags.SORTABLE) ?? false, "srch.settings." + AzureSearchFieldFlags.SORTABLE));
                }

                // Add cell with 'iFieldname' value
                if (DisplayIField)
                {
                    tc = new TableCell();
                    CMSTextBox txt = new CMSTextBox();
                    txt.ID = column.ColumnName + SearchSettings.IFIELDNAME;
                    txt.CssClass += " form-control";
                    txt.MaxLength = 200;
                    if (ssi != null)
                    {
                        txt.Text = ssi.FieldName;
                    }
                    tc.Controls.Add(txt);
                    tr.Cells.Add(tc);
                }
                tc = new TableCell();
                tr.Cells.Add(tc);
                table.Rows.Add(tr);
            }
        }
    }


    /// <summary>
    /// Stores data and raises OnSaved event.
    /// </summary>
    public void SaveData()
    {
        // Clear old values
        mFields = new SearchSettings();
        Changed = false;

        // Create new SearchSettingInfos
        foreach (ColumnDefinition column in mAttributes)
        {
            SearchSettingsInfo ssiOld = null;

            // Return old data to compare changes
            if (mInfos != null)
            {
                DataRow[] dr = mInfos.Tables[0].Select("name = '" + column.ColumnName + "'");
                if ((dr.Length > 0) && (mSearchSettings != null))
                {
                    ssiOld = mSearchSettings.GetSettingsInfo((string)dr[0]["id"]);
                }
            }

            var name = column.ColumnName;
            bool content = (pnlContent.FindControl(name + SearchSettings.CONTENT) as CMSCheckBox)?.Checked ?? false;
            bool searchable = (pnlContent.FindControl(name + SearchSettings.SEARCHABLE) as CMSCheckBox)?.Checked ?? false;
            bool tokenized = (pnlContent.FindControl(name + SearchSettings.TOKENIZED) as CMSCheckBox)?.Checked ?? false;

            bool azureContent = (pnlContent.FindControl(name + AzureSearchFieldFlags.CONTENT) as CMSCheckBox)?.Checked ?? false;
            bool azureRetrievable = (pnlContent.FindControl(name + AzureSearchFieldFlags.RETRIEVABLE) as CMSCheckBox)?.Checked ?? false;
            bool azureSearchable = (pnlContent.FindControl(name + AzureSearchFieldFlags.SEARCHABLE) as CMSCheckBox)?.Checked ?? false;

            bool azureFacetable = (pnlContent.FindControl(name + AzureSearchFieldFlags.FACETABLE) as CMSCheckBox)?.Checked ?? false;
            bool azureFilterable = (pnlContent.FindControl(name + AzureSearchFieldFlags.FILTERABLE) as CMSCheckBox)?.Checked ?? false;
            bool azureSortable = (pnlContent.FindControl(name + AzureSearchFieldFlags.SORTABLE) as CMSCheckBox)?.Checked ?? false;

            TextBox txt = pnlContent.FindControl(name + SearchSettings.IFIELDNAME) as TextBox;
            string fieldname = txt != null ? txt.Text : String.Empty;

            bool fieldChanged;

            var flags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            flags.Add(SearchSettings.CONTENT, content);
            flags.Add(SearchSettings.SEARCHABLE, searchable);
            flags.Add(SearchSettings.TOKENIZED, tokenized);

            if (DisplayAzureFields)
            {
                flags.Add(AzureSearchFieldFlags.CONTENT, azureContent);
                flags.Add(AzureSearchFieldFlags.RETRIEVABLE, azureRetrievable);
                flags.Add(AzureSearchFieldFlags.SEARCHABLE, azureSearchable);

                flags.Add(AzureSearchFieldFlags.FACETABLE, azureFacetable);
                flags.Add(AzureSearchFieldFlags.SORTABLE, azureSortable);
                flags.Add(AzureSearchFieldFlags.FILTERABLE, azureFilterable);
            }

            var ssi = SearchHelper.CreateSearchSettings(name, flags, fieldname, ssiOld, out fieldChanged);

            if (fieldChanged)
            {
                Changed = true;
            }

            mFields.SetSettingsInfo(ssi);
        }

        // Store values to DB
        if (mDci != null)
        {
            mDci.ClassSearchSettings = mFields.GetData();
            DataClassInfoProvider.SetDataClassInfo(mDci);
        }

        if (DisplaySaved)
        {
            ShowChangesSaved();
        }
        RaiseOnSaved();
    }


    /// <summary>
    /// Creates <see cref="TableCell"/> with a checkbox checked if <param name="value"/> is true for given <paramref name="column"/>.
    /// Checkbox indicates what kind of an action can be performed on given <paramref name="column"/> e.g. faceting.
    /// </summary>
    private TableCell CreateTableCell(string searchField, ColumnDefinition column, bool value, string toolTipResourceString)
    {
        var isRequiredField = IsFieldConfigurationRequired(mDci.ClassName, column.ColumnName, searchField);

        var tc = new TableCell();
        var chk = new CMSCheckBox();
        chk.ID = column.ColumnName + searchField;
        chk.Enabled = !isRequiredField;
        chk.Checked = isRequiredField || value;
        chk.ToolTipResourceString = toolTipResourceString;
        tc.Controls.Add(chk);

        return tc;
    }


    /// <summary>
    /// Adds header cell to the <paramref name="tableHeaderRow"/> with given text <paramref name="text"/>.
    /// </summary>
    private void AddTableHeaderCell(TableHeaderRow tableHeaderRow, string text, bool subheader = false, int colspan = 1)
    {
        TableHeaderCell thc = new TableHeaderCell();
        thc.Text = text;
        thc.Scope = TableHeaderScope.Column;
        thc.ColumnSpan = colspan;
        if (subheader)
        {
            thc.AddCssClass("subheader");
        }
        tableHeaderRow.Cells.Add(thc);
    }


    /// <summary>
    /// Returns true, when the field and it's configuration is always required by the search infrastructure.
    /// </summary>
    private static bool IsFieldConfigurationRequired(string className, string fieldName, string searchFieldFlag)
    {
        if (mRequiredFields.TryGetValue(className, out Dictionary<string, HashSet<string>> fieldsAndFlags))
        {
            if (fieldsAndFlags.TryGetValue(fieldName, out HashSet<string> flags))
            {
                return flags.Contains(searchFieldFlag);
            }
        }

        return false;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Sets automatically button - click event handler.
    /// </summary>
    private void btnAutomatically_Click(object sender, EventArgs e)
    {
        ReloadData(true, true);
    }

    #endregion
}