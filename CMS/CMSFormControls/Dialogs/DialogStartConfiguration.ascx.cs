using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


/// <summary>
/// This form control needs other blank fields with following names to work properly:
/// dialogs_content_hide
/// dialogs_content_path
/// dialogs_content_site
/// dialogs_content_userelativeurl
/// dialogs_libraries_hide
/// dialogs_libraries_site
/// dialogs_libraries_global
/// dialogs_libraries_global_libname
/// dialogs_groups
/// dialogs_groups_name
/// dialogs_libraries_group
/// dialogs_libraries_group_libname
/// dialogs_libraries_path
/// dialogs_attachments_hide
/// dialogs_anchor_hide
/// dialogs_email_hide
/// dialogs_web_hide
/// autoresize
/// autoresize_width
/// autoresize_height
/// autoresize_maxsidesize
/// </summary>
public partial class CMSFormControls_Dialogs_DialogStartConfiguration : FormEngineUserControl
{
    #region "Variables"

    protected bool communityLoaded;
    protected bool mediaLoaded;

    #endregion


    #region "Constants"

    private const string AUTORESIZE = "autoresize";
    private const string AUTORESIZE_WIDTH = "autoresize_width";
    private const string AUTORESIZE_HEIGHT = "autoresize_height";
    private const string AUTORESIZE_MAXSIDESIZE = "autoresize_maxsidesize";
    private const string DIALOGS_CONTENT_HIDE = "dialogs_content_hide";
    private const string DIALOGS_CONTENT_PATH = "dialogs_content_path";
    private const string DIALOGS_CONTENT_SITE = "dialogs_content_site";
    private const string DIALOGS_CONTENT_USERELATIVEURL = "dialogs_content_userelativeurl";
    private const string DIALOGS_LIBRARIES_HIDE = "dialogs_libraries_hide";
    private const string DIALOGS_LIBRARIES_SITE = "dialogs_libraries_site";
    private const string DIALOGS_LIBRARIES_GLOBAL = "dialogs_libraries_global";
    private const string DIALOGS_LIBRARIES_GLOBAL_LIBNAME = "dialogs_libraries_global_libname";
    private const string DIALOGS_GROUPS = "dialogs_groups";
    private const string DIALOGS_GROUPS_NAME = "dialogs_groups_name";
    private const string DIALOGS_LIBRARIES_GROUP = "dialogs_libraries_group";
    private const string DIALOGS_LIBRARIES_GROUP_LIBNAME = "dialogs_libraries_group_libname";
    private const string DIALOGS_LIBRARIES_PATH = "dialogs_libraries_path";
    private const string DIALOGS_ATTACHMENTS_HIDE = "dialogs_attachments_hide";
    private const string DIALOGS_ANCHOR_HIDE = "dialogs_anchor_hide";
    private const string DIALOGS_EMAIL_HIDE = "dialogs_email_hide";
    private const string DIALOGS_WEB_HIDE = "dialogs_web_hide";

    private const string NONE_VALUE = "#none#";
    private const string CURENT_VALUE = "#current#";
    private const string SINGLE_VALUE = "#single#";

    #endregion


    #region "Properties"

    public override object Value
    {
        get
        {
            return true;
        }
        set
        {
            EnsureChildControls();

            // Load values from other fields
            LoadValues();
        }
    }


    /// <summary>
    /// Indicates if the Autoresize settings should be available.
    /// </summary>
    public bool DisplayAutoresize
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAutoresize"), true);
        }
        set
        {
            SetValue("DisplayAutoresize", value);
        }
    }


    /// <summary>
    /// Indicates if the E-mail tab settings should be available.
    /// </summary>
    public bool DisplayEmailTabSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayEmailTabSettings"), true);
        }
        set
        {
            SetValue("DisplayEmailTabSettings", value);
        }
    }


    /// <summary>
    /// Indicates if the Anchor tab settings should be available.
    /// </summary>
    public bool DisplayAnchorTabSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAnchorTabSettings"), true);
        }
        set
        {
            SetValue("DisplayAnchorTabSettings", value);
        }
    }


    /// <summary>
    /// Indicates if the Web tab settings should be available.
    /// </summary>
    public bool DisplayWebTabSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayWebTabSettings"), true);
        }
        set
        {
            SetValue("DisplayWebTabSettings", value);
        }
    }


    /// <summary>
    /// Indicates if the configuration dialog should be displayed.
    /// </summary>
    private bool ShowConfiguration
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["ShowConfiguration"], false);
        }
        set
        {
            ViewState["ShowConfiguration"] = value;
        }
    }

    #endregion


    #region "Control events"

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        lnkAdvacedFieldSettings.Click += lnkAdvacedFieldSettings_Click;

        communityLoaded = ModuleEntryManager.IsModuleLoaded(ModuleName.COMMUNITY);
        mediaLoaded = ModuleEntryManager.IsModuleLoaded(ModuleName.MEDIALIBRARY);

        if (communityLoaded)
        {
            drpGroups.OnSelectionChanged += drpGroups_SelectedIndexChanged;
        }

        LoadSites();
        LoadSiteLibraries(null);
        LoadSiteGroups(null);
        LoadGroupLibraries(null, null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!lnkAdvacedFieldSettings.IsEnabled)
        {
            // Display configuration settings in read-only mode
            plcAdvancedFieldSettings.Visible = true;
            lnkAdvacedFieldSettings.Visible = false;
            elemAutoResize.Enabled = false;
        }
        else
        {
            plcAdvancedFieldSettings.Visible = ShowConfiguration;
        }

        plcMedia.Visible = mediaLoaded;
        plcGroups.Visible = communityLoaded;

        selectPathElem.PathTextBox.ToolTip = GetString("formcontrols.dialogstartconfiguration.contentstartingpath");
    }


    /// <summary>
    /// Advanced dialog link event handler.
    /// </summary>
    protected void lnkAdvacedFieldSettings_Click(object sender, EventArgs e)
    {
        ShowConfiguration = !ShowConfiguration;
    }


    /// <summary>
    /// Group drop-down list event handler.
    /// </summary>
    protected void drpGroups_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectGroup();
    }


    /// <summary>
    /// Handles site selection change event.
    /// </summary>
    protected void UniSelectorMediaSites_OnSelectionChanged(object sender, EventArgs e)
    {
        string selectedSite = ValidationHelper.GetString(siteSelectorMedia.Value, String.Empty);
        SelectSite(selectedSite);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Processes the data loading after the site is selected.
    /// </summary>
    private void SelectSite(string selectedSite)
    {
        if (communityLoaded)
        {
            LoadSiteGroups(selectedSite);
        }
        LoadSiteLibraries(selectedSite);
        SelectGroup();
    }


    /// <summary>
    /// Processes the data loading after the group is selected.
    /// </summary>
    private void SelectGroup()
    {
        bool isNone = string.Equals(drpGroups.DropDownSingleSelect.SelectedValue, NONE_VALUE, StringComparison.InvariantCultureIgnoreCase);
        string selectedMediaSite = ValidationHelper.GetString(siteSelectorMedia.Value, String.Empty);
        
        drpGroupLibraries.Enabled = !isNone;
        LoadGroupLibraries(selectedMediaSite, ValidationHelper.GetString(drpGroups.Value, String.Empty), isNone); 

        if (isNone)
        {
            drpGroupLibraries.DropDownSingleSelect.SelectedIndex = 0;
        }
    }


    /// <summary>
    /// Loads the site DropDownLists.
    /// </summary>
    private void LoadSites()
    {
        // Define special fields
        SpecialFieldsDefinition specialFields = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver);
        specialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = "##all##" });
        specialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentsite"), Value = "##current##" });

        // Set site selector
        siteSelectorContent.DropDownSingleSelect.AutoPostBack = true;
        siteSelectorContent.AllowAll = false;
        siteSelectorContent.UseCodeNameForSelection = true;
        siteSelectorContent.UniSelector.SpecialFields = specialFields;

        siteSelectorMedia.DropDownSingleSelect.AutoPostBack = true;
        siteSelectorMedia.AllowAll = false;
        siteSelectorMedia.UseCodeNameForSelection = true;
        siteSelectorMedia.UniSelector.SpecialFields = specialFields;

        if (mediaLoaded)
        {
            siteSelectorMedia.UniSelector.OnSelectionChanged += UniSelectorMediaSites_OnSelectionChanged;
        }
    }


    /// <summary>
    /// Reloads the site groups.
    /// </summary>
    /// <param name="siteName">Name of the site</param>
    private void LoadSiteGroups(string siteName)
    {
        if (communityLoaded && mediaLoaded)
        {
            drpGroups.DropDownItems.Clear();
            drpGroups.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = String.Empty });
            drpGroups.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = NONE_VALUE });
            drpGroups.SpecialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentgroup"), Value = CURENT_VALUE });

            if (siteName != null)
            {
                drpGroups.WhereCondition = "GroupSiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = '" + SqlHelper.EscapeQuotes(siteName) + "')";
                drpGroups.Reload(true);
            }
        }
    }


    /// <summary>
    /// Reloads the site media libraries.
    /// </summary>
    /// <param name="siteName">Name of the site</param>
    private void LoadSiteLibraries(string siteName)
    {
        if (mediaLoaded)
        {
            drpSiteLibraries.DropDownItems.Clear();

            drpSiteLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = String.Empty });
            drpSiteLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = NONE_VALUE });
            drpSiteLibraries.SpecialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentlibrary"), Value = CURENT_VALUE });

            if (siteName != null)
            {
                drpSiteLibraries.WhereCondition = "LibrarySiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = '" + SqlHelper.EscapeQuotes(siteName) + "')";
                drpSiteLibraries.Reload(true);
            }
        }
    }


    /// <summary>
    /// Reloads the group media libraries.
    /// </summary>
    /// <param name="siteName">Name of the site</param>
    /// <param name="groupName">Name of the group</param>
    /// <param name="addNone">If true the (none) option is added</param>
    private void LoadGroupLibraries(string siteName, string groupName, bool addNone = false)
    {
        if (mediaLoaded && communityLoaded)
        {
            drpGroupLibraries.DropDownItems.Clear();
            
            if (addNone)
            {
                drpGroupLibraries.Value = null;
                drpGroupLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = NONE_VALUE });
            }

            drpGroupLibraries.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = String.Empty });
            drpGroupLibraries.SpecialFields.Add(new SpecialField { Text = GetString("dialogs.config.currentlibrary"), Value = CURENT_VALUE });

            if ((siteName != null) && (groupName != null))
            {
                drpGroupLibraries.WhereCondition = String.Format("LibraryGroupID IN (SELECT GroupID FROM Community_Group WHERE GroupSiteID IN (SELECT SiteID FROM CMS_Site WHERE SiteName = '{0}') AND GroupName = N'{1}')", SqlHelper.EscapeQuotes(siteName), SqlHelper.EscapeQuotes(groupName));
                drpGroupLibraries.Reload(true);
            }
        }
    }


    /// <summary>
    /// Selects correct item in given DDL.
    /// </summary>
    /// <param name="selector">Selector with the data</param>
    /// <param name="origKey">Key in hashtable which determines whether the value is special or specific item</param>
    /// <param name="singleItemKey">Key in hashtable for specified item</param>
    private void SelectInDDL(UniSelector selector, string origKey, string singleItemKey)
    {
        string item = ValidationHelper.GetString(Form.Data.GetValue(origKey), "").ToLowerInvariant();
        if (item == SINGLE_VALUE)
        {
            item = ValidationHelper.GetString(Form.Data.GetValue(singleItemKey), "");
        }

        ListItem li = selector.DropDownItems.FindByValue(item);
        if (li != null)
        {
            selector.Value = li.Value;
        }
    }


    private void CheckColumn(string columnName, string dataTypeString, ref bool isValid)
    {
        if (!ContainsColumn(columnName))
        {
            ValidationError += String.Format(GetString("formcontrol.missingcolumn"), columnName, dataTypeString);
            isValid = false;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Sets inner controls according to the parameters and their values included in configuration collection. Parameters collection will be passed from Field editor.
    /// </summary>
    public void LoadValues()
    {
        // Set settings configuration
        plcDisplayAnchor.Visible = DisplayAnchorTabSettings;
        plcDisplayEmail.Visible = DisplayEmailTabSettings;
        plcDisplayWeb.Visible = DisplayWebTabSettings;
        plcAutoResize.Visible = DisplayAutoresize;

        LoadOtherValues();
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        if ((Form == null) || (Form.Data == null))
        {
            return;
        }

        var data = Form.Data;

        if (ContainsColumn(AUTORESIZE))
        {
            elemAutoResize.Form = Form;
            elemAutoResize.Value = ValidationHelper.GetString(data.GetValue(AUTORESIZE), null);
        }

        // Content tab
        if (ContainsColumn(DIALOGS_CONTENT_HIDE))
        {
            chkDisplayContentTab.Checked = !ValidationHelper.GetBoolean(data.GetValue(DIALOGS_CONTENT_HIDE), false);
        }
        if (ContainsColumn(DIALOGS_CONTENT_USERELATIVEURL))
        {
            chkUseRelativeUrl.Checked = ValidationHelper.GetBoolean(data.GetValue(DIALOGS_CONTENT_USERELATIVEURL), true);
        }

        if (ContainsColumn(DIALOGS_CONTENT_PATH))
        {
            selectPathElem.Value = ValidationHelper.GetString(data.GetValue(DIALOGS_CONTENT_PATH), "");
        }

        if (ContainsColumn(DIALOGS_CONTENT_SITE))
        {
            siteSelectorContent.Value = ValidationHelper.GetString(data.GetValue(DIALOGS_CONTENT_SITE), null);
        }

        // Media tab
        if (mediaLoaded)
        {
            if (ContainsColumn(DIALOGS_LIBRARIES_HIDE))
            {
                chkDisplayMediaTab.Checked = !ValidationHelper.GetBoolean(data.GetValue(DIALOGS_LIBRARIES_HIDE), false);
            }

            // Site DDL                
            string libSites = null;
            if (ContainsColumn(DIALOGS_LIBRARIES_SITE))
            {
                libSites = ValidationHelper.GetString(data.GetValue(DIALOGS_LIBRARIES_SITE), null);
            }
            siteSelectorMedia.Value = libSites;
            SelectSite(libSites);

            // Site libraries DDL
            if (ContainsColumn(DIALOGS_LIBRARIES_GLOBAL) && ContainsColumn(DIALOGS_LIBRARIES_GLOBAL_LIBNAME))
            {
                SelectInDDL(drpSiteLibraries, DIALOGS_LIBRARIES_GLOBAL, DIALOGS_LIBRARIES_GLOBAL_LIBNAME);
            }

            if (communityLoaded)
            {
                // Groups DDL
                if (ContainsColumn(DIALOGS_GROUPS) && ContainsColumn(DIALOGS_GROUPS_NAME))
                {
                    SelectInDDL(drpGroups, DIALOGS_GROUPS, DIALOGS_GROUPS_NAME);
                }

                SelectGroup();

                // Group libraries DDL
                if (ContainsColumn(DIALOGS_LIBRARIES_GROUP) && ContainsColumn(DIALOGS_LIBRARIES_GROUP_LIBNAME))
                {
                    SelectInDDL(drpGroupLibraries, DIALOGS_LIBRARIES_GROUP, DIALOGS_LIBRARIES_GROUP_LIBNAME);
                }
            }

            // Starting path
            if (ContainsColumn(DIALOGS_LIBRARIES_PATH))
            {
                txtMediaStartPath.Text = ValidationHelper.GetString(data.GetValue(DIALOGS_LIBRARIES_PATH), "");
            }
        }

        // Other tabs        
        if (ContainsColumn(DIALOGS_ATTACHMENTS_HIDE))
        {
            chkDisplayAttachments.Checked = !ValidationHelper.GetBoolean(data.GetValue(DIALOGS_ATTACHMENTS_HIDE), false);
        }
        if (ContainsColumn(DIALOGS_ANCHOR_HIDE))
        {
            chkDisplayAnchor.Checked = !ValidationHelper.GetBoolean(data.GetValue(DIALOGS_ANCHOR_HIDE), false);
        }
        if (ContainsColumn(DIALOGS_EMAIL_HIDE))
        {
            chkDisplayEmail.Checked = !ValidationHelper.GetBoolean(data.GetValue(DIALOGS_EMAIL_HIDE), false);
        }
        if (ContainsColumn(DIALOGS_WEB_HIDE))
        {
            chkDisplayWeb.Checked = !ValidationHelper.GetBoolean(data.GetValue(DIALOGS_WEB_HIDE), false);
        }
    }


    /// <summary>
    /// Returns other values related to this control.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[21, 2];
        values[0, 0] = AUTORESIZE;
        values[1, 0] = AUTORESIZE_WIDTH;
        values[2, 0] = AUTORESIZE_HEIGHT;
        values[3, 0] = AUTORESIZE_MAXSIDESIZE;
        values[4, 0] = DIALOGS_CONTENT_HIDE;
        values[5, 0] = DIALOGS_CONTENT_PATH;
        values[6, 0] = DIALOGS_CONTENT_SITE;
        values[7, 0] = DIALOGS_LIBRARIES_HIDE;
        values[8, 0] = DIALOGS_LIBRARIES_SITE;
        values[9, 0] = DIALOGS_LIBRARIES_GLOBAL;
        values[10, 0] = DIALOGS_LIBRARIES_GLOBAL_LIBNAME;
        values[11, 0] = DIALOGS_GROUPS;
        values[12, 0] = DIALOGS_GROUPS_NAME;
        values[13, 0] = DIALOGS_LIBRARIES_GROUP;
        values[14, 0] = DIALOGS_LIBRARIES_GROUP_LIBNAME;
        values[15, 0] = DIALOGS_LIBRARIES_PATH;
        values[16, 0] = DIALOGS_ATTACHMENTS_HIDE;
        values[17, 0] = DIALOGS_ANCHOR_HIDE;
        values[18, 0] = DIALOGS_EMAIL_HIDE;
        values[19, 0] = DIALOGS_WEB_HIDE;
        values[20, 0] = DIALOGS_CONTENT_USERELATIVEURL;

        // Resize control values
        values[0, 1] = elemAutoResize.Value;
        if (plcAutoResize.Visible)
        {
            var resizeValues = elemAutoResize.GetOtherValues();
            if ((resizeValues != null) && (resizeValues.Length > 3))
            {
                values[1, 1] = resizeValues[0, 1];
                values[2, 1] = resizeValues[1, 1];
                values[3, 1] = resizeValues[2, 1];
            }
        }
        else
        {
            // Set default values
            values[1, 1] = Form.GetDataValue(AUTORESIZE_WIDTH);
            values[2, 1] = Form.GetDataValue(AUTORESIZE_HEIGHT);
            values[3, 1] = Form.GetDataValue(AUTORESIZE_MAXSIDESIZE);
        }

        // Content tab
        values[4, 1] = !chkDisplayContentTab.Checked;

        if (!chkUseRelativeUrl.Checked)
        {
            values[20, 1] = false;
        }

        if ((string)selectPathElem.Value != "")
        {
            values[5, 1] = selectPathElem.Value;
        }

        string selectedSite = ValidationHelper.GetString(siteSelectorContent.Value, String.Empty);
        if (selectedSite != String.Empty)
        {
            values[6, 1] = selectedSite;
        }

        // Media tab
        if (mediaLoaded)
        {
            if (!chkDisplayMediaTab.Checked)
            {
                values[7, 1] = true;
            }

            selectedSite = ValidationHelper.GetString(siteSelectorMedia.Value, String.Empty);
            if (selectedSite != String.Empty)
            {
                values[8, 1] = selectedSite;
            }

            // Site libraries DDL
            var value = ValidationHelper.GetString(drpSiteLibraries.Value, String.Empty);
            if ((value == NONE_VALUE) || (value == CURENT_VALUE))
            {
                values[9, 1] = value;
            }
            else if (value != "")
            {
                values[9, 1] = SINGLE_VALUE;
                values[10, 1] = value;
            }

            if (communityLoaded)
            {
                // Groups DDL
                value = ValidationHelper.GetString(drpGroups.Value, String.Empty);
                if ((value == NONE_VALUE) || (value == CURENT_VALUE))
                {
                    values[11, 1] = value;
                }
                else if (value != "")
                {
                    values[11, 1] = SINGLE_VALUE;
                    values[12, 1] = value;
                }

                // Group libraries DDL
                value = ValidationHelper.GetString(drpGroupLibraries.Value, String.Empty);
                if ((value == NONE_VALUE) || (value == CURENT_VALUE))
                {
                    values[13, 1] = value;
                }
                else if (value != "")
                {
                    values[13, 1] = SINGLE_VALUE;
                    values[14, 1] = value;
                }
            }

            // Starting path
            value = txtMediaStartPath.Text.Trim();
            if (value != "")
            {
                values[15, 1] = value;
            }
        }

        // Other tabs
        if (!chkDisplayAttachments.Checked)
        {
            values[16, 1] = true;
        }
        if (!chkDisplayAnchor.Checked)
        {
            values[17, 1] = true;
        }
        if (!chkDisplayEmail.Checked)
        {
            values[18, 1] = true;
        }
        if (!chkDisplayWeb.Checked)
        {
            values[19, 1] = true;
        }

        return values;
    }


    /// <summary>
    /// Validation of form control.
    /// </summary>
    public override bool IsValid()
    {
        bool isValid = true;

        // Check validity of autoresize element.
        if (plcAutoResize.Visible)
        {
            isValid = elemAutoResize.IsValid();
        }

        var boolType = GetString("templatedesigner.fieldtypes.boolean");
        var textType = GetString("general.text");
        var intType = GetString("templatedesigner.fieldtypes.integer");

        CheckColumn(DIALOGS_CONTENT_HIDE, boolType, ref isValid);
        CheckColumn(DIALOGS_CONTENT_PATH, boolType, ref isValid);
        CheckColumn(DIALOGS_CONTENT_SITE, textType, ref isValid);
        CheckColumn(DIALOGS_CONTENT_USERELATIVEURL, boolType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_HIDE, boolType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_SITE, textType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_GLOBAL, textType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_GLOBAL_LIBNAME, textType, ref isValid);
        CheckColumn(DIALOGS_GROUPS, textType, ref isValid);
        CheckColumn(DIALOGS_GROUPS_NAME, textType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_GROUP, textType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_GROUP_LIBNAME, textType, ref isValid);
        CheckColumn(DIALOGS_LIBRARIES_PATH, textType, ref isValid);
        CheckColumn(DIALOGS_ATTACHMENTS_HIDE, boolType, ref isValid);
        CheckColumn(DIALOGS_ANCHOR_HIDE, boolType, ref isValid);
        CheckColumn(DIALOGS_EMAIL_HIDE, boolType, ref isValid);
        CheckColumn(DIALOGS_WEB_HIDE, boolType, ref isValid);
        CheckColumn(AUTORESIZE, textType, ref isValid);
        CheckColumn(AUTORESIZE_WIDTH, intType, ref isValid);
        CheckColumn(AUTORESIZE_HEIGHT, intType, ref isValid);
        CheckColumn(AUTORESIZE_MAXSIDESIZE, intType, ref isValid);

        return isValid;
    }

    #endregion
}