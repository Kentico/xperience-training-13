using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Search;
using CMS.UIControls;


public partial class CMSModules_SmartSearch_Controls_Edit_SearchFields : CMSAdminEditControl
{
    #region "Private variables"

    private DataClassInfo dci;
    private DataClassInfo document;
    private ArrayList attributes = new ArrayList();
    private FormInfo fi;
    private bool mAdvancedMode = true;

    // Contains item list for 'Title' drop-down list.
    private readonly ListItem[] ALLOWED_TITLES = new[]
    {
        new ListItem("DocumentName"),
        new ListItem("DocumentPageTitle"),
        new ListItem("DocumentPageDescription"),
        new ListItem("DocumentCustomData"),
        new ListItem("DocumentTags"),
        new ListItem("NodeAliasPath"),
        new ListItem("NodeName"),
        new ListItem("NodeAlias"),
        new ListItem("NodeCustomData"),
        new ListItem("SKUNumber"),
        new ListItem("SKUName"),
        new ListItem("SKUDescription"),
        new ListItem("SKUImagePath"),
        new ListItem("SKUCustomData")
    };

    // Contains item list for 'Content' drop-down list.
    private readonly ListItem[] ALLOWED_CONTENT = new[]
    {
        new ListItem("DocumentName"),
        new ListItem("DocumentPageTitle"),
        new ListItem("DocumentPageDescription"),
        new ListItem("DocumentContent"),
        new ListItem("DocumentCustomData"),
        new ListItem("DocumentTags"),
        new ListItem("NodeAliasPath"),
        new ListItem("NodeName"),
        new ListItem("NodeAlias"),
        new ListItem("NodeCustomData"),
        new ListItem("SKUNumber"),
        new ListItem("SKUName"),
        new ListItem("SKUDescription"),
        new ListItem("SKUShortDescription"),
        new ListItem("SKUImagePath"),
        new ListItem("SKUCustomData")
    };

    // Contains item list for 'Image' field
    private readonly ListItem[] ALLOWED_IMAGE = new[]
    {
       new ListItem("DocumentContent"),
       new ListItem("SKUImagePath")
    };

    // Contains item list for 'Date' drop-down list.
    private readonly ListItem[] ALLOWED_DATE = new[]
    {
        new ListItem("DocumentModifiedWhen"),
        new ListItem("DocumentCreatedWhen"),
        new ListItem("DocumentCheckedOutWhen"),
        new ListItem("DocumentPublishFrom"),
        new ListItem("DocumentPublishTo"),
        new ListItem("SKULastModified"),
        new ListItem("SKUCreated")
    };

    private string mSaveResourceString = "general.changessaved";
    private string mRebuildIndexResourceString = "searchindex.requiresrebuild";

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
    /// Gets or sets the value that indicates whether dropdown lists should be
    /// filled by actual object values or document values only
    /// </summary>
    public bool LoadActualValues
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the resource string which is displayed after the save action.
    /// </summary>
    public string SaveResourceString
    {
        get
        {
            return mSaveResourceString;
        }
        set
        {
            mSaveResourceString = value;
        }
    }


    /// <summary>
    /// Resource text for rebuild index label.
    /// </summary>
    public string RebuildIndexResourceString
    {
        get
        {
            return mRebuildIndexResourceString;
        }
        set
        {
            mRebuildIndexResourceString = value;
        }
    }


    /// <summary>
    /// Indicates if advanced mode is used.
    /// </summary>
    public bool AdvancedMode
    {
        get
        {
            return mAdvancedMode;
        }
        set
        {
            mAdvancedMode = value;
        }
    }


    /// <summary>
    /// Gets current class.
    /// </summary>
    private DataClassInfo ClassInfo
    {
        get
        {
            return dci ?? (dci = DataClassInfoProvider.GetDataClassInfo(ItemID));
        }
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return pnlBody.Enabled;
        }
        set
        {
            pnlBody.Enabled = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.AppendTooltip(iconHelpDataSource, GetString("srch.pagedatasource.tooltip"), null);

        var link = string.Format("<a target=\"_blank\" href=\"{0}\">{1}</a>", DocumentationHelper.GetDocumentationTopicUrl("search_results"), GetString("general.ourdocumentation"));
        var imgElement = string.Format("<img class=\"img-responsive\" src=\"{0}\"/>", URLHelper.ResolveUrl("~/CMSPages/GetResource.ashx?image=CMSModules/CMS_SmartSearch/search-results-smarttip.png"));
        smarttipSearchResults.Content = string.Format(GetString("srch.pageresults.smarttip.content"), imgElement, link);

        ClassFields.OnSaved += ClassFields_OnSaved;
        ClassFields.DisplaySaved = false;

        // Setup controls
        if (!RequestHelper.IsPostBack() && (ClassInfo != null))
        {
            pnlSearchFields.Visible = chkSearchEnabled.Checked = ClassInfo.ClassSearchEnabled;
        }

        plcAdvancedMode.Visible = AdvancedMode;

        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads data in control.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        ReloadSearch(false);
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    /// <param name="setAutomatically">Indicates whether search options should be set automatically</param>
    public void ReloadSearch(bool setAutomatically)
    {
        ClassFields.ItemID = ItemID;

        // Initialize properties
        List<IDataDefinitionItem> itemList = null;

        if (ClassInfo != null)
        {
            if (ClassInfo.ClassIsDocumentType == true)
            {
                plcPageIndexingOptions.Visible = true;

                PreselectPageDataSource();
            }

            // Load XML definition
            fi = FormHelper.GetFormInfo(ClassInfo.ClassName, true);

            if (CMSString.Compare(ClassInfo.ClassName, "cms.user", true) == 0)
            {
                plcImage.Visible = false;
                ClassFields.DisplaySetAutomatically = false;
                pnlIndent.Visible = true;

                document = DataClassInfoProvider.GetDataClassInfo("cms.usersettings");
                if (document != null)
                {
                    FormInfo fiSettings = FormHelper.GetFormInfo(document.ClassName, true);
                    fi.CombineWithForm(fiSettings, true, String.Empty);
                }
            }

            // Get all fields
            itemList = fi.GetFormElements(true, true);
        }

        ClassFields.ReloadData(setAutomatically, true);

        if (itemList != null)
        {
            if (itemList.Any())
            {
                pnlIndent.Visible = true;
            }

            // Store each field to array
            foreach (var item in itemList)
            {
                var formField = item as FormFieldInfo;
                if (formField != null)
                {
                    object[] obj = { formField.Name, DataTypeManager.GetSystemType(TypeEnum.Field, formField.DataType) };
                    attributes.Add(obj);
                }
            }
        }

        if (AdvancedMode)
        {
            ReloadControls();
        }
    }


    /// <summary>
    /// Enables or disables search for current class.
    /// </summary>
    public bool SaveSearchAvailability()
    {
        bool changed = false;
        if (ItemID > 0)
        {
            DataClassInfo classInfo = DataClassInfoProvider.GetDataClassInfo(ItemID);
            if (classInfo != null)
            {
                if (classInfo.ClassSearchEnabled != chkSearchEnabled.Checked)
                {
                    changed = true;
                }
                classInfo.ClassSearchEnabled = chkSearchEnabled.Checked;
                DataClassInfoProvider.SetDataClassInfo(classInfo);

                if (!chkSearchEnabled.Checked)
                {
                    ShowConfirmation(GetString("search.searchwasdisabled"));
                }
            }
        }
        return changed;
    }


    /// <summary>
    /// Calls method from ClassFields control which stores data.
    /// </summary>
    public void SaveData()
    {
        ClassFields.SaveData();
    }


    /// <summary>
    /// Reloads drop-down lists with new data.
    /// </summary>
    private void ReloadControls()
    {
        if ((ClassInfo == null))
        {
            return;
        }

        // Load drop-down list 'Title field'
        LoadAndPreselectListControl(drpTitleField, ALLOWED_TITLES, false, attributes, ClassInfo.ClassSearchTitleColumn, SearchHelper.DEFAULT_SEARCH_TITLE_COLUMN);

        // Load drop-down list 'Content field'
        LoadAndPreselectListControl(drpContentField, PrependNoneOption(ALLOWED_CONTENT), true, attributes, ClassInfo.ClassSearchContentColumn);

        // Load drop-down list 'Image field'
        LoadAndPreselectListControl(drpImageField, PrependNoneOption(ALLOWED_IMAGE), true, attributes, ClassInfo.ClassSearchImageColumn);

        // "Load drop-down list 'Date field'
        LoadAndPreselectListControl(drpDateField, ALLOWED_DATE, true, attributes, ClassInfo.ClassSearchCreationDateColumn, SearchHelper.DEFAULT_SEARCH_CREATION_DATE_COLUMN);
    }


    /// <summary>
    /// Loads data into given <paramref name="listControl"/> and tries to preselect value according to <paramref name="valueToSelect"/> or <paramref name="defaultValueToSelect"/>.
    /// </summary>
    /// <param name="listControl">List control</param>
    /// <param name="defaultData">First data source to be loaded; only if <see cref="LoadActualValues"/> is false</param>
    /// <param name="allowNoneOption">Indicates if "(none)" option can be loaded if <see cref="LoadActualValues"/> is true</param>
    /// <param name="fieldsData">Fields data source which is always loaded</param>
    /// <param name="valueToSelect">Value that is preselected in the <paramref name="listControl"/> if it is not empty</param>
    /// <param name="defaultValueToSelect">Value that is preselected if <paramref name="valueToSelect"/> is empty or is not among loaded items and <see cref="LoadActualValues"/> is false</param>
    /// <remarks>Any items present in <paramref name="listControl"/> are removed.</remarks>
    private void LoadAndPreselectListControl(ListControl listControl, IEnumerable<ListItem> defaultData, bool allowNoneOption, IEnumerable fieldsData, string valueToSelect, string defaultValueToSelect = null)
    {
        // Clear current items
        listControl.Items.Clear();

        // Load new items
        if (!LoadActualValues)
        {
            foreach (var item in defaultData)
            {
                listControl.Items.Add(item);
            }
        }
        else if (allowNoneOption)
        {
            listControl.Items.Add(GetNoneOption());
        }

        foreach (object[] item in fieldsData)
        {
            listControl.Items.Add(new ListItem(item[0].ToString()));
        }

        // Preselect value
        if (!String.IsNullOrEmpty(valueToSelect) && (listControl.Items.FindByValue(valueToSelect) != null))
        {
            listControl.SelectedValue = valueToSelect;
        }
        else
        {
            if (!LoadActualValues && !String.IsNullOrEmpty(defaultValueToSelect))
            {
                listControl.SelectedValue = defaultValueToSelect;
            }
        }
    }


    /// <summary>
    /// Returns new <see cref="ListItem"/> for none option
    /// using general resource string for none options and zero as item's value.
    /// </summary>
    private ListItem GetNoneOption()
    {
        return new ListItem(GetString("general.selectnone"), "0");
    }


    /// <summary>
    /// Prepends given collection of options with none-option retrieved from <see cref="GetNoneOption"/> method.
    /// </summary>
    /// <param name="options">Collection to be prepended with none-option.</param>
    /// <returns>Collection of none-option followed by all items from the <paramref name="options"/>.</returns>
    private IEnumerable<ListItem> PrependNoneOption(IEnumerable<ListItem> options)
    {
        return Enumerable
            .Empty<ListItem>()
            .Union(new[] { GetNoneOption() })
            .Union(options);
    }


    private void PreselectPageDataSource()
    {
        if (!ClassInfo.ClassHasURL)
        {
            rblPageDataSource.Enabled = false;
            rblPageDataSource.ToolTipResourceString = "srch.pagedatasource.disabled.tooltip";
        }

        SearchIndexDataSourceEnum preselectValue;
        if (ClassInfo.GetValue("ClassSearchIndexDataSource") == null)
        {
            if (!ClassInfo.ClassIsCoupledClass)
            {
                preselectValue = SearchIndexDataSourceEnum.HTMLOutput;
            }
            else
            {
                preselectValue = ClassInfo.ClassUsesPageBuilder ? SearchIndexDataSourceEnum.Both : SearchIndexDataSourceEnum.ContentFields;
            }
        }
        else
        {
            preselectValue = ClassInfo.ClassSearchIndexDataSource;
        }

        ClassFields.SearchIndexDataSource = preselectValue;
        rblPageDataSource.SelectedIndex = (int)preselectValue;
    }

    #endregion


    #region "Events"

    public void btnOK_Click(object sender, EventArgs e)
    {
        SaveData();
    }


    /// <summary>
    /// CheckedChanged event handler.
    /// </summary>
    protected void chkSearchEnabled_CheckedChanged(object sender, EventArgs e)
    {
        pnlSearchFields.Visible = chkSearchEnabled.Checked;

        if (chkSearchEnabled.Checked)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// OK button click handler.
    /// </summary>
    private void ClassFields_OnSaved(object sender, EventArgs e)
    {
        bool enabledChanged = false;
        if (ClassInfo != null)
        {
            enabledChanged = SaveSearchAvailability();

            if (ClassInfo.ClassIsDocumentType)
            {
                ClassInfo.ClassSearchIndexDataSource = (SearchIndexDataSourceEnum)rblPageDataSource.SelectedIndex;
            }

            if (AdvancedMode)
            {
                // Save advanced information only in advanced mode
                ClassInfo.ClassSearchTitleColumn = drpTitleField.SelectedValue;
                ClassInfo.ClassSearchContentColumn = (drpContentField.SelectedValue != "0") ? drpContentField.SelectedValue : DBNull.Value.ToString();
                ClassInfo.ClassSearchImageColumn = (drpImageField.SelectedValue != "0") ? drpImageField.SelectedValue : DBNull.Value.ToString();
                ClassInfo.ClassSearchCreationDateColumn = drpDateField.SelectedValue;
            }

            DataClassInfoProvider.SetDataClassInfo(ClassInfo);

            RaiseOnSaved();
        }

        // Display a message
        if (!String.IsNullOrEmpty(SaveResourceString))
        {
            string saveMessage = GetString(SaveResourceString);
            if (!String.IsNullOrEmpty(saveMessage))
            {
                ShowConfirmation(saveMessage);
            }
        }

        if ((ClassFields.Changed || enabledChanged) && (!String.IsNullOrEmpty(RebuildIndexResourceString)))
        {
            ShowInformation(GetString(RebuildIndexResourceString));
        }
    }

    protected void rblPageDataSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClassFields.SearchIndexDataSource = (SearchIndexDataSourceEnum)rblPageDataSource.SelectedIndex;
        ClassFields.ReloadData(false, true);
    }

    #endregion
}