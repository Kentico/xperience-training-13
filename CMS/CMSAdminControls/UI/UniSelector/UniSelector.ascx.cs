using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UniSelector_UniSelector : UniSelector, IPostBackEventHandler, ICallbackEventHandler
{
    #region "Private variables"

    private DataSet mResultDs;
    private int mNewCurrentPage;
    private bool mHashIsValid = true;
    private bool mAllowInitHash = true;
    private bool mRegisterScripts;
    private bool mRegisterDialogScripts;
    private bool mLoaded;
    private bool mHashValidated;
    private bool mPageChanged;
    private bool mPageSizeChanged;
    private bool mSortingChanged;
    private bool mEnsureSelectedField;
    private bool? mDisplayNewButton;
    private bool? mDisplayEditButton;
    private string mCheckBoxClass;
    private string mValueDisplayName;
    private string mUniSelectorClientID;
    private string mDropDownSearchResults = String.Empty;
    private string mAdditionalAutocompleteWidgetClass = String.Empty;
    private string mInitSet = String.Empty;

    private readonly ListItemCollection mDropDownItems = new ListItemCollection();
    private readonly ListItemCollection mResolvedSpecialFields = new ListItemCollection();

    private readonly StringBuilder mJavaScript = new StringBuilder();


    /// <summary>
    /// A dictionary of client identifiers and their associated controls.
    /// </summary>
    /// <remarks>
    /// It caches client identifiers as they can change during control's life-cycle.
    /// To retrieve a client identifier for specific control you need to call the GetClientID method.
    /// </remarks>
    private readonly Dictionary<Control, string> mClientIdentifiers = new Dictionary<Control, string>();

    #endregion


    #region "Public properties"

    /// <summary>
    /// Additional CSS class for drop down list control.
    /// </summary>
    public string AdditionalDropDownCSSClass
    {
        get;
        set;
    }


    /// <summary>
    /// Return client ID of autocomplete value storage(hidden field).
    /// </summary>
    public string AutocompleteValueClientID
    {
        get
        {
            return hdnValue.ClientID;
        }
    }


    /// <summary>
    /// Returns control's client ID based on usage autocomplete. Usually used in javascript in combination with value property.
    /// </summary>
    public string DropDownControlID
    {
        get
        {
            return UseUniSelectorAutocomplete ? hdnValue.ClientID : drpSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    protected override string Identifier
    {
        get
        {
            return EnsureIdentifier();
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return GetValue();
        }
        set
        {
            SetValue(value);
        }
    }


    /// <summary>
    /// Gets the display name of the value item. Returns null if display name is not available.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return mValueDisplayName ?? (mValueDisplayName = GetValueDisplayName());
        }
    }


    /// <summary>
    /// Gets the value that indicates whether current selector in multiple mode displays some data or whether the dropdown contains some data.
    /// </summary>
    public override bool HasData
    {
        get
        {
            // Ensure the data
            if (!StopProcessing)
            {
                Reload(false);
            }

            return ValidationHelper.GetBoolean(ViewState["HasData"], false);
        }
        protected set
        {
            ViewState["HasData"] = value;
        }
    }


    /// <summary>
    /// Gets or sets if control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["Enabled"], true);
        }
        set
        {
            ViewState["Enabled"] = value;
            SetEnabled();
        }
    }


    /// <summary>
    /// List of items of drop down control
    /// </summary>
    public override ListItemCollection DropDownItems
    {
        get
        {
            return UseUniSelectorAutocomplete ? mDropDownItems : drpSingleSelect.Items;
        }
    }


    /// <summary>
    /// Indicates if unigrid's pager size or sorting changed.
    /// </summary>
    private bool UniGridStatusChanged
    {
        get
        {
            return (mPageSizeChanged || mSortingChanged);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether the New item button should be displayed.
    /// </summary>
    private bool DisplayNewButton
    {
        get
        {
            if (mDisplayNewButton == null)
            {
                mDisplayNewButton = !String.IsNullOrEmpty(NewItemPageUrl);

                if (!String.IsNullOrEmpty(NewItemElementName) && !String.IsNullOrEmpty(ElementResourceName))
                {
                    CurrentUserInfo ui = MembershipContext.AuthenticatedUser;

                    // Check permissions for object type
                    bool check = false;
                    BaseInfo bi = ModuleManager.GetReadOnlyObject(ObjectType);
                    if (bi != null)
                    {
                        check = bi.CheckPermissions(PermissionsEnum.Create, SiteContext.CurrentSiteName, ui);
                    }

                    mDisplayNewButton = check && ui.IsAuthorizedPerUIElement(ElementResourceName, NewItemElementName, SiteContext.CurrentSiteName);
                }
            }

            return mDisplayNewButton.Value;
        }
    }


    /// <summary>
    /// Indicates whether the Edit item button should be displayed.
    /// </summary>
    private bool DisplayEditButton
    {
        get
        {
            if (mDisplayEditButton == null)
            {
                mDisplayEditButton = !String.IsNullOrEmpty(EditItemPageUrl);

                if (!String.IsNullOrEmpty(EditItemElementName) && !String.IsNullOrEmpty(ElementResourceName))
                {
                    CurrentUserInfo ui = MembershipContext.AuthenticatedUser;

                    // Check permissions for object type
                    bool check = false;
                    BaseInfo bi = ModuleManager.GetReadOnlyObject(ObjectType);
                    if (bi != null)
                    {
                        check = bi.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, ui);
                    }

                    mDisplayEditButton = check && ui.IsAuthorizedPerUIElement(ElementResourceName, EditItemElementName, SiteContext.CurrentSiteName);
                }
            }

            return mDisplayEditButton.Value;
        }
    }


    /// <summary>
    /// Gets the UniSelector's client ID.
    /// </summary>
    private string UniSelectorClientID
    {
        get
        {
            return mUniSelectorClientID ?? (mUniSelectorClientID = GetClientID(this));
        }
    }

    #endregion


    #region "Controls properties"

    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public override CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return drpSingleSelect;
        }
    }


    /// <summary>
    /// Drop down list selection edit button.
    /// </summary>
    public override UniButton ButtonDropDownEdit
    {
        get
        {
            return btnDropEdit;
        }
    }


    /// <summary>
    /// Gets the Select button control.
    /// </summary>
    public override UniButton ButtonSelect
    {
        get
        {
            return btnSelect;
        }
    }


    /// <summary>
    /// Gets the Clear button control.
    /// </summary>
    public override UniButton ButtonClear
    {
        get
        {
            return btnClear;
        }
    }


    /// <summary>
    /// Gets the Remove selected items button.
    /// </summary>
    public override CMSMoreOptionsButton ButtonRemoveSelected
    {
        get
        {
            return btnRemove;
        }
    }


    /// <summary>
    /// Gets the Add items button control.
    /// </summary>
    public override Button ButtonAddItems
    {
        get
        {
            return btnAddItems;
        }
    }


    /// <summary>
    /// Gets the text box selection control.
    /// </summary>
    public override CMSTextBox TextBoxSelect
    {
        get
        {
            return txtSingleSelect;
        }
    }


    /// <summary>
    /// Textbox selection edit button.
    /// </summary>
    public override UniButton ButtonEdit
    {
        get
        {
            return btnEdit;
        }
    }


    /// <summary>
    /// Multiple selection grid.
    /// </summary>
    public override UniGrid UniGrid
    {
        get
        {
            return uniGrid;
        }
    }


    /// <summary>
    /// Button selection control.
    /// </summary>
    public override LocalizedButton DialogButton
    {
        get
        {
            return btnDialog;
        }
    }


    /// <summary>
    /// New item button.
    /// </summary>
    public override UniButton ButtonDropDownNew
    {
        get
        {
            return btnDropNew;
        }
    }


    /// <summary>
    /// Object transformation used in SingleTransformation selection mode.
    /// </summary>
    public override ObjectTransformation ObjectTransformation
    {
        get
        {
            return objTransform;
        }
    }


    /// <summary>
    /// Client ID of primary input control. Input control depends on selection mode - modes different from textbox or drop-down list modes return empty string.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            switch (SelectionMode)
            {
                case SelectionModeEnum.SingleTextBox:
                case SelectionModeEnum.MultipleTextBox:
                    return TextBoxSelect.ClientID;

                case SelectionModeEnum.SingleDropDownList:
                    return UseUniSelectorAutocomplete ? txtAutocomplete.ClientID : DropDownSingleSelect.ClientID;

                default:
                    return string.Empty;
            }
        }
    }


    /// <summary>
    /// Gets ClientID of the control from which the Value is retrieved or
    /// null if such a control can't be specified.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return InputClientID;
        }
    }

    #endregion


    #region "Control events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SetHashValue(true);
    }


    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(((Pair)savedState).First);

        // Do not overwrite value if ViewState doesn't contain value for current control (e.g. disabled control)
        if (Request.Form.AllKeys.Contains(hdnIdentifier.UniqueID))
        {
            // Get values from form if the control is loaded dynamically
            hdnIdentifier.Value = Request.Form[hdnIdentifier.UniqueID];
        }
        if (Request.Form.AllKeys.Contains(hiddenField.UniqueID))
        {
            hiddenField.Value = Request.Form[hiddenField.UniqueID];
        }
        if (Request.Form.AllKeys.Contains(hdnDialogSelect.UniqueID))
        {
            hdnDialogSelect.Value = Request.Form[hdnDialogSelect.UniqueID];
        }
        if (Request.Form.AllKeys.Contains(hdnHash.UniqueID))
        {
            hdnHash.Value = Request.Form[hdnHash.UniqueID];
        }
        if (Request.Form.AllKeys.Contains(hdnValue.UniqueID))
        {
            hdnValue.Value = Request.Form[hdnValue.UniqueID];
        }

        SelectionModeEnum mode = SelectionMode;
        if (!(AllowEditTextBox && (mode == SelectionModeEnum.SingleTextBox || mode == SelectionModeEnum.MultipleTextBox)))
        {
            // Validate value against hash
            ValidateValue();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (RequestHelper.IsCallback())
        {
            StopProcessing = true;
        }

        if (StopProcessing)
        {
            return;
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);
        ScriptHelper.RegisterWOpenerScript(Page);

        mCheckBoxClass = Guid.NewGuid().ToString().Replace("-", String.Empty);

        // Bound events
        drpSingleSelect.SelectedIndexChanged += drpSingleSelect_SelectedIndexChanged;

        uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        uniGrid.OnPageChanged += uniGrid_OnPageChanged;
        uniGrid.PageSizeDropdown.SelectedIndexChanged += uniGrid_PageSizeChanged;
        uniGrid.OnBeforeSorting += uniGrid_OnBeforeSorting;
        uniGrid.IsLiveSite = IsLiveSite;

        btnClear.Click += btnClear_Click;

        uniGrid.Pager.DefaultPageSize = 10;

        // If control is enabled, then display content
        if ((SelectionMode == SelectionModeEnum.Multiple) && !ControlsHelper.CausedPostBack(btnRemove, this, uniGrid.Pager.UniPager))
        {
            Reload(false);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        if (!String.IsNullOrEmpty(AdditionalDropDownCSSClass))
        {
            txtAutocomplete.AddCssClass(AdditionalDropDownCSSClass);
            drpSingleSelect.AddCssClass(AdditionalDropDownCSSClass);
        }

        // If control is enabled, then display content
        SelectionModeEnum mode = SelectionMode;
        if ((mode != SelectionModeEnum.Multiple) || !mLoaded || UniGridStatusChanged)
        {
            Reload(mode == SelectionModeEnum.Multiple && UniGridStatusChanged);
        }

        if (mEnsureSelectedField)
        {
            EnsureSelectedField(ValidationHelper.GetString(Value, String.Empty));
        }

        if (mRegisterScripts)
        {
            RegisterScripts();
        }

        SetGridHeader();

        lblStatus.Visible = !String.IsNullOrEmpty(lblStatus.Text);

        // If the page was not changed, deselect all
        if (!mPageChanged)
        {
            hiddenSelected.Value = String.Empty;
        }

        // Always reset the new value from dialog
        hdnDialogSelect.Value = String.Empty;

        // Enable/disable buttons when manually set
        SetButtonsEnabled();

        if (mode == SelectionModeEnum.Multiple)
        {
            // Display explanation text for disabled Add button in multiple mode
            if (!String.IsNullOrEmpty(DisabledAddButtonExplanationText) && !btnAddItems.Enabled)
            {
                lblDisabledAddButtonExplanationText.Text = DisabledAddButtonExplanationText;
                lblDisabledAddButtonExplanationText.Visible = true;
            }
            else
            {
                lblDisabledAddButtonExplanationText.Visible = false;
            }
        }

        if (mode == SelectionModeEnum.SingleDropDownList)
        {
            SetOnChangeClientScript();

            if (UseUniSelectorAutocomplete)
            {
                RegisterAutocompleteScripts();
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Customized SaveViewState to ensure LoadViewState call on the next postback.
    /// </summary>
    protected override object SaveViewState()
    {
        return new Pair(base.SaveViewState(), null);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (StopProcessing)
        {
            return;
        }

        string classAtr = String.Empty;
        string styleAtr = String.Empty;

        if (!String.IsNullOrEmpty(CssClass))
        {
            classAtr = $" class=\"{CssClass}\"";
        }
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            styleAtr = $" style=\"{ControlStyle}\"";
        }

        writer.Write("<div id=\"{0}\"{1}{2}>", UniSelectorClientID, classAtr, styleAtr);

        // Render child controls
        RenderChildren(writer);

        writer.Write("</div>");
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Drop-down list event handler.
    /// </summary>
    private void drpSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Only raise selected index changed when other than (more items...) is selected
        if (drpSingleSelect.SelectedValue != US_MORE_RECORDS.ToString())
        {
            var renderChangeScript = drpSingleSelect.AutoPostBack;
            RaiseSelectionChanged(renderChangeScript);
        }
    }


    /// <summary>
    /// Unigrid external data bound handler.
    /// </summary>
    private object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        object val = String.Empty;

        switch (sourceName.ToLowerInvariant())
        {
            case "yesno":
                val = UniGridFunctions.ColoredSpanYesNo(parameter);
                break;

            case "select":
                {
                    // Get item ID
                    DataRowView drv = (DataRowView)parameter;

                    string itemID = drv[ReturnColumnName].ToString();

                    CMSCheckBox chkCheckbox = new CMSCheckBox
                    {
                        ClientIDMode = ClientIDMode.Static,
                        ID = HTMLHelper.EncodeForHtmlAttribute($"chk{mCheckBoxClass}_{itemID}")
                    };
                    chkCheckbox.InputAttributes.Add("onclick", $"US_ProcessItem('{UniSelectorClientID}', {ScriptSafeValueSeparator}, this);");
                    chkCheckbox.InputAttributes.Add("class", "chk" + mCheckBoxClass);

                    // Keep the check status if checked
                    chkCheckbox.Checked = mPageChanged && (hiddenSelected.Value.IndexOf(ValuesSeparator + itemID + ValuesSeparator, StringComparison.InvariantCultureIgnoreCase) >= 0);

                    val = chkCheckbox;
                    break;
                }

            case "itemname":
                {
                    DataRowView drv = (DataRowView)parameter;

                    // Get item ID
                    string itemID = drv[ReturnColumnName].ToString();

                    // Get item name
                    string itemName = GetItemName(drv.Row);

                    LinkButton lnkButton = new LinkButton();
                    lnkButton.Text = HTMLHelper.HTMLEncode(TextHelper.LimitLength(itemName, 100));
                    lnkButton.OnClientClick = $@"if (this.href) {{
    US_ProcessItem('{UniSelectorClientID}', {ScriptSafeValueSeparator}, document.getElementById({ScriptHelper.GetString("chk" + mCheckBoxClass + "_" + itemID)}), true);
}}
return false;";

                    val = lnkButton;
                    break;
                }
        }

        var additionalDataBound = AdditionalDataBound(this, sourceName, parameter, val);

        if (additionalDataBound != null)
        {
            val = additionalDataBound;
            if (!(val is Control))
            {
                val = ValidationHelper.GetString(val, String.Empty);
            }
        }

        return val;
    }


    /// <summary>
    /// Unigrid page index changed handler.
    /// </summary>
    protected void uniGrid_OnPageChanged(object sender, EventArgs e)
    {
        mNewCurrentPage = uniGrid.Pager.CurrentPage;
        mPageChanged = true;
    }


    /// <summary>
    /// Unigrid page size changed handler.
    /// </summary>
    protected void uniGrid_PageSizeChanged(object sender, EventArgs e)
    {
        mPageSizeChanged = true;
    }


    /// <summary>
    /// Unigrid sorting changed handler.
    /// </summary>
    protected void uniGrid_OnBeforeSorting(object sender, EventArgs e)
    {
        mSortingChanged = true;

        GridViewSortEventArgs sort = (GridViewSortEventArgs)e;
        if (sort != null)
        {
            SortExpression = sort.SortExpression + (string.IsNullOrEmpty(SortExpression) ||
                SortExpression.Equals(sort.SortExpression + SqlHelper.ORDERBY_DESC, StringComparison.OrdinalIgnoreCase) ||
                !SortExpression.StartsWith(sort.SortExpression, StringComparison.OrdinalIgnoreCase) ? SqlHelper.ORDERBY_ASC : SqlHelper.ORDERBY_DESC);
        }
    }


    /// <summary>
    /// Button clear click.
    /// </summary>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        Value = null;
        Reload(true);

        // Mark as changed on client side
        RegisterChangedScript();
    }


    /// <summary>
    /// Button "Remove selected items" click handler.
    /// </summary>
    protected void btnRemoveSelected_Click(object sender, EventArgs e)
    {
        // Unselect selected items
        if (!String.IsNullOrEmpty(hiddenSelected.Value))
        {
            hiddenField.Value = DataHelper.GetNewItemsInList(hiddenSelected.Value, hiddenField.Value, ValuesSeparator);
            SetHashValue();

            Reload(true);

            RaiseSelectionChanged();
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads control's content.
    /// </summary>
    protected override void ReloadControlInternal()
    {
        base.ReloadControlInternal();

        Reload(true);
    }


    /// <summary>
    /// Reloads all controls.
    /// </summary>
    /// <param name="forceReload">Indicates if data should be loaded from DB</param>
    public override void Reload(bool forceReload)
    {
        if (!mLoaded || forceReload)
        {
            if (forceReload)
            {
                // Reset loaded flag
                mLoaded = false;
            }

            LoadObjects();

            if (!StopProcessing)
            {
                SetupControls();

                mRegisterScripts = true;

                ReloadData(forceReload);
            }

            mLoaded = true;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    /// <summary>
    /// Adds CSS class to autocomplete uniselector.
    /// </summary>
    /// <param name="cssClass">Class to add</param>
    public void AddClassToAutocompleteWidget(string cssClass)
    {
        mAdditionalAutocompleteWidgetClass = cssClass;
    }


    /// <summary>
    /// Gets the script to display the selection dialog
    /// </summary>
    public string GetSelectionDialogScript()
    {
        var script = $"US_SelectionDialog_{UniSelectorClientID}(); return false;";
        if (!OnGetSelectionDialogScript.IsBound)
        {
            return script;
        }

        var e = new GetSelectionDialogScriptEventArgs
        {
            Script = script
        };

        OnGetSelectionDialogScript.StartEvent(e);

        return e.Script;
    }

    #endregion


    #region "Private methods"

    private void SetEnabled()
    {
        btnClear.Enabled = Enabled;
        btnSelect.Enabled = Enabled;
        btnDialog.Enabled = Enabled;
        drpSingleSelect.Enabled = Enabled;
        txtSingleSelect.Enabled = Enabled;
        btnRemove.Enabled = Enabled;
        btnAddItems.Enabled = Enabled;
        btnEdit.Enabled = Enabled;
        btnNew.Enabled = Enabled;
        btnDropNew.Enabled = Enabled;
        btnDropEdit.Enabled = Enabled;
        pnlGrid.Enabled = Enabled;
        txtAutocomplete.Enabled = Enabled;
    }


    /// <summary>
    /// Ensures the unique control identifier
    /// </summary>
    private string EnsureIdentifier()
    {
        string identifier = hdnIdentifier.Value;
        if (string.IsNullOrEmpty(identifier))
        {
            identifier = Request.Form[hdnIdentifier.UniqueID];
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
            }
            hdnIdentifier.Value = identifier;
        }

        return identifier;
    }


    /// <summary>
    /// Sets the value to the selector.
    /// </summary>
    /// <param name="value">New value</param>
    private void SetValue(object value)
    {
        // Set ValueDisplayName to null -> this forces reload value display name because Value changed
        mValueDisplayName = null;

        // Reset the control if the value is null
        if (value == null)
        {
            ResetValue();

            return;
        }

        switch (SelectionMode)
        {
            // Dropdown mode
            case SelectionModeEnum.SingleDropDownList:
                SetDropDownValue(value);
                break;

            // Textbox mode
            case SelectionModeEnum.SingleTextBox:
            case SelectionModeEnum.MultipleTextBox:
                SetTextBoxValue(value);
                break;

            // Other modes
            default:
                SetHiddenValue(value);
                break;
        }

        SetHashValue();
        mAllowInitHash = false;
        ViewState["HasValue"] = true;
    }


    private void ResetValue()
    {
        hdnDialogSelect.Value = String.Empty;
        txtSingleSelect.Text = String.Empty;
        hiddenField.Value = String.Empty;
        hdnValue.Value = String.Empty;
        drpSingleSelect.SelectedIndex = -1;
        ViewState["HasValue"] = false;

        if ((SelectionMode == SelectionModeEnum.SingleDropDownList) && (drpSingleSelect.Items.Count > 0))
        {
            // Ensure selection of first item in list and proper hash value
            EnsureSelectedField(null);
        }
        SetHashValue();
    }


    private void SetHiddenValue(object value)
    {
        hiddenField.Value = String.Format("{0}{1}{0}", ValuesSeparator, ValidationHelper.GetString(value, String.Empty).Trim(ValuesSeparator));
    }


    private void SetTextBoxValue(object value)
    {
        string text = ValidationHelper.GetString(value, String.Empty).Trim(ValuesSeparator);
        if (AllowEditTextBox)
        {
            txtSingleSelect.Text = text;
        }
        hiddenField.Value = String.Format("{0}{1}{0}", ValuesSeparator, text);
    }


    private void SetDropDownValue(object value)
    {
        String val = ValidationHelper.GetString(value, String.Empty);
        hdnDialogSelect.Value = val;
        hdnValue.Value = val;

        if (!UseUniSelectorAutocomplete)
        {
            if (drpSingleSelect.Items.FindByValue(hdnDialogSelect.Value) != null)
            {
                drpSingleSelect.SelectedValue = hdnDialogSelect.Value;
            }
            else
            {
                // Reset flag, data reload is required
                mLoaded = false;
            }
        }
        else
        {
            mEnsureSelectedField = true;
        }
    }


    /// <summary>
    /// Gets the current value of the selector.
    /// </summary>
    /// <param name="trim">Indicates if separators should be trimmed from the value</param>
    private object GetValue(bool trim = true)
    {
        string result = string.Empty;

        SelectionModeEnum mode = SelectionMode;
        switch (mode)
        {
            // Dropdown mode
            case SelectionModeEnum.SingleDropDownList:
                return GetDropDownListValue();

            // Other modes
            default:
                {
                    if (AllowEditTextBox && (mode == SelectionModeEnum.SingleTextBox || mode == SelectionModeEnum.MultipleTextBox))
                    {
                        result = txtSingleSelect.Text;
                    }
                    else if (mHashIsValid)
                    {
                        result = hiddenField.Value;
                    }

                    return trim ? result.Trim(ValuesSeparator) : result;
                }
        }
    }


    /// <summary>
    /// Gets a value from the dropdown list.
    /// </summary>
    private string GetDropDownListValue()
    {
        string result;

        if (!mHashValidated && ControlsHelper.CausedPostBack(this) && String.Equals(Request.Form[Page.postEventArgumentID], "reload", StringComparison.OrdinalIgnoreCase) && Request.Form.AllKeys.Contains(hdnDialogSelect.UniqueID))
        {
            hdnDialogSelect.Value = Request.Form[hdnDialogSelect.UniqueID];
        }

        // Try to get value from hidden field
        if (!mLoaded && !string.IsNullOrEmpty(hdnDialogSelect.Value))
        {
            result = hdnDialogSelect.Value;
        }
        // For DropDown control data loaded on PreRender, get value directly from request when postback and data not loaded explicitly
        else if (Enabled && !mLoaded && mHashIsValid && RequestHelper.IsPostBack() && Request.Form.AllKeys.Contains(drpSingleSelect.UniqueID))
        {
            result = Request.Form[drpSingleSelect.UniqueID];
        }
        else
        {
            result = UseUniSelectorAutocomplete ? hdnValue.Value : drpSingleSelect.SelectedValue;
        }

        return result;
    }


    /// <summary>
    /// Validates Value against hash.
    /// </summary>
    private void ValidateValue()
    {
        string value = GetValue(false).ToString();

        // Validate only non empty values
        if (!String.IsNullOrEmpty(value))
        {
            // Validate hash (if not special value - all, empty...)
            var settings = new HashSettings(ClientID)
            {
                Redirect = false
            };

            mHashIsValid = ValidationHelper.ValidateHash(value, hdnHash.Value, settings);
            if (!mHashIsValid)
            {
                if (!IsLiveSite)
                {
                    // Data is not consistent!
                    ShowWarning(GetString("uniselector.badhash"));
                }
                // Reset value
                Value = null;
            }
        }
        mHashValidated = true;
    }


    /// <summary>
    /// Displays data according to selection mode.
    /// </summary>
    private void ReloadData(bool forceReload)
    {
        // Check that object type is not empty
        if (Object != null)
        {
            // Display form control content according to selection mode
            switch (SelectionMode)
            {
                case SelectionModeEnum.SingleTextBox:
                    DisplayTextBox();
                    break;

                case SelectionModeEnum.SingleDropDownList:
                    DisplayDropDownList(forceReload);
                    break;

                case SelectionModeEnum.Multiple:
                    DisplayMultiple(forceReload);
                    break;

                case SelectionModeEnum.MultipleTextBox:
                    DisplayMultipleTextBox();
                    break;

                case SelectionModeEnum.SingleButton:
                case SelectionModeEnum.MultipleButton:
                    DisplayButton();
                    break;

                case SelectionModeEnum.SingleTransformation:
                    DisplayTransformation();
                    break;
            }
        }
        else
        {
            // Display form control content according to selection mode
            switch (SelectionMode)
            {
                case SelectionModeEnum.SingleDropDownList:
                    DisplayDropDownList(forceReload);
                    break;
            }
        }
    }


    /// <summary>
    /// Loads objects from DB and stores it to variables.
    /// </summary>
    private void LoadObjects()
    {
        if (Object == null)
        {
            lblStatus.Text = $"[UniSelector]: Object type '{ObjectType}' not found.";
            StopProcessing = true;

            return;
        }

        // Reset string builder
        mJavaScript.Clear();

        EnsureColumnNames();

        SelectionModeEnum mode = SelectionMode;

        // Open selection dialog depending if UniSelector is on live site
        string url = "~/CMSAdminControls/UI/UniSelector/" + (IsLiveSite ? "LiveSelectionDialog.aspx" : "SelectionDialog.aspx");

        if (!String.IsNullOrEmpty(SelectItemPageUrl))
        {
            url = SelectItemPageUrl;
        }

        url += "?SelectionMode=" + mode + "&hidElem=" + GetClientID(hiddenField) + "&params=" + Server.UrlEncode(ScriptHelper.GetString(Identifier, false)) + "&clientId=" + UniSelectorClientID + "&localize=" + (LocalizeItems ? 1 : 0) + "&hashElem=" + GetClientID(hdnHash) + AdditionalUrlParameters;

        // Create modal dialogs and datasets according to selection mode
        switch (mode)
        {
            // Single text box selection mode
            case SelectionModeEnum.SingleTextBox:
                url += "&txtElem=" + GetClientID(txtSingleSelect);
                break;

            // Single drop down list selection mode
            case SelectionModeEnum.SingleDropDownList:
                {
                    var topN = MaxDisplayedTotalItems + 1;
                    if (!UseUniSelectorAutocomplete)
                    {
                        mResultDs = GetResultSet(null, topN);
                    }

                    // Do not generate script for selection dialog if there is no more items
                    if (DataHelper.GetItemsCount(mResultDs) < topN)
                    {
                        url = null;
                    }
                    else
                    {
                        url += "&selectElem=" + GetClientID(hdnDialogSelect);
                    }
                }
                break;

            // Multiple selection mode
            case SelectionModeEnum.Multiple:
                LoadUniGrid();
                break;

            // Multiple text box selection mode
            case SelectionModeEnum.MultipleTextBox:
                url += "&txtElem=" + GetClientID(txtSingleSelect);
                break;

            // Button selection
            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
            case SelectionModeEnum.SingleTransformation:
                break;

            default:
                url = null;
                mResultDs = null;
                break;
        }

        // Selection dialog window
        if (url != null)
        {
            // Add IsSiteManager parameter to handle edit and new window
            url += IsSiteManager ? "&siteManager=true" : String.Empty;

            // Add hash
            string hash = ValidationHelper.GetHashString(url.Substring(url.IndexOf('?')), new HashSettings(""));
            url += "&hash=" + hash;

            mJavaScript.Append($@"
function US_SelectionDialog_{UniSelectorClientID} (values) {{
    {Page.ClientScript.GetCallbackEventReference(this, "values", $"US_SelectionDialogReady_{UniSelectorClientID}", $"'{HttpUtility.JavaScriptStringEncode(ScriptHelper.ResolveUrl(url))}'")};
}}");

            mJavaScript.AppendLine($@"
function US_SelectionDialogReady_{UniSelectorClientID} (rvalue, context) {{
    modalDialog(context + ((rvalue != '') ? '&selectedvalue=' + rvalue : ''), '{DialogWindowName}', {DialogWindowWidth}, {DialogWindowHeight}, null, null, true);
    return false;
}}");
        }

        // Create selection changed function
        if (OnSelectionChangedAvailable())
        {
            mJavaScript.Append($"function US_SelectionChanged_{UniSelectorClientID} () {{ {Page.ClientScript.GetPostBackEventReference(this, "selectionchanged")}; }} \n");
        }

        // New item window
        if (DisplayNewButton)
        {
            // Get the new URL
            var newUrl = GetNewUrl(UniSelectorClientID);

            if (!string.IsNullOrEmpty(newUrl))
            {
                mJavaScript.Append($@"
function US_NewItem_{UniSelectorClientID} (selectedItem) {{
    var url = '{ScriptHelper.ResolveUrl(newUrl)}';
    selectedItem = US_TrimSeparators(selectedItem, {ScriptSafeValueSeparator});
    modalDialog(url.replace(/##ITEMID##/i, selectedItem), 'NewItem', {EditDialogWindowWidth}, {EditDialogWindowHeight}, 'openInFullscreen={EditDialogWindowOpenInFullscreen}');
}}");
            }
        }

        // Edit item window
        if (DisplayEditButton)
        {
            // Get the edit URL
            var editUrl = GetEditUrl(UniSelectorClientID);

            if (!String.IsNullOrEmpty(editUrl))
            {
                mJavaScript.Append($@"
function US_EditItem_{UniSelectorClientID} (selectedItem) {{
    selectedItem = US_TrimSeparators(selectedItem, {ScriptSafeValueSeparator});
    if (selectedItem == '') {{
        alert({ScriptHelper.GetLocalizedString("general.pleaseselectitem")});
        return false;
    }} else if (selectedItem.indexOf('{{%') >= 0) {{
        alert({ScriptHelper.GetLocalizedString("general.cannoteditmacro")});
        return false;
    }}
    var url = '{ScriptHelper.ResolveUrl(editUrl)}';

    if (window.US_GetEditedItemId_{UniSelectorClientID}) {{
        selectedItem = window.US_GetEditedItemId_{UniSelectorClientID}(selectedItem);
    }}

    selectedItem = US_TrimSeparators(selectedItem, {ScriptSafeValueSeparator});

    modalDialog(url.replace(/##ITEMID##/i, selectedItem),'{EditWindowName}', {EditDialogWindowWidth}, {EditDialogWindowHeight}, 'openInFullscreen={EditDialogWindowOpenInFullscreen}');
}}");
            }
        }

        if ((url != null) || DisplayNewButton || DisplayEditButton)
        {
            // Get the confirmation script
            var conf = !String.IsNullOrEmpty(SelectionConfirmation) ? $"if (!confirm({ScriptHelper.GetString(SelectionConfirmation)})) {{ return false; }}" : String.Empty;

            // Get the check changes if it is set to true
            var checkChanges = CheckChanges ? "if (CheckChanges && !CheckChanges()) return false;" : String.Empty;

            mJavaScript.Append($@"
function US_ReloadPage_{UniSelectorClientID} () {{
    {Page.ClientScript.GetPostBackEventReference(this, "reload")};
    return false;
}}

function US_RefreshPage_{UniSelectorClientID} () {{
    {Page.ClientScript.GetPostBackEventReference(this, "refresh")};
    return false;
}}

function US_SelectItems_{UniSelectorClientID} (items,hash) {{
    {checkChanges}
    {conf}
    document.getElementById('{GetClientID(hiddenField)}').value = decodeURIComponent(items);
    document.getElementById('{GetClientID(hdnHash)}').value = hash;
    {Page.ClientScript.GetPostBackEventReference(this, "selectitems")};
    return false;
}}

function US_SelectNewValue_{UniSelectorClientID} (selValue) {{
    if (window.US_GetNewItemId_{UniSelectorClientID}) {{
        selValue = window.US_GetNewItemId_{UniSelectorClientID}(selValue);
    }}
    document.getElementById('{GetClientID(hiddenSelected)}').value = selValue;
    {Page.ClientScript.GetPostBackEventReference(this, "selectnewvalue")};
    return false;
}}");
            mRegisterDialogScripts = true;
        }
    }


    /// <summary>
    /// Loads the data to UniGrid
    /// </summary>
    private void LoadUniGrid()
    {
        uniGrid.GridName = GridName;
        uniGrid.LoadGridDefinition();

        // Set custom page according to settings to restrict size of data
        if (ItemsPerPage > 0)
        {
            uniGrid.Pager.DefaultPageSize = ItemsPerPage;
        }

        // Ensure new current page number
        if (mNewCurrentPage > 0)
        {
            uniGrid.Pager.UniPager.CurrentPage = mNewCurrentPage;
        }

        int pageSize = uniGrid.Pager.CurrentPageSize;

        int currentOffset = (uniGrid.Pager.CurrentPage - 1) * pageSize;
        int totalRecords = 0;

        // Display only selected items
        string ids = ValidationHelper.GetString(Value, String.Empty);

        mResultDs = GetResultSet(ids, 0, currentOffset, pageSize, ref totalRecords);

        // If not first page and no data loaded load first page
        if (DataHelper.DataSourceIsEmpty(mResultDs) && (currentOffset > 0))
        {
            // Set unigrid page to 1 and reload data
            uniGrid.Pager.UniPager.CurrentPage = 1;
            mResultDs = GetResultSet(ids, 0, 0, pageSize, ref totalRecords);
        }

        uniGrid.PagerForceNumberOfResults = totalRecords;

        mJavaScript.Append("function US_RemoveAll_", UniSelectorClientID, "(){ if (confirm(", ScriptHelper.GetString(GetString("general.confirmremoveall")), ")) {", Page.ClientScript.GetPostBackEventReference(this, "removeall"), "; return false; }} \n");
    }


    /// <summary>
    /// Ensures that selected field is in the dropdownlist items.
    /// </summary>
    /// <param name="value">Selected value</param>
    private void EnsureSelectedField(string value)
    {
        if (UseUniSelectorAutocomplete)
        {
            if (String.IsNullOrEmpty(value))
            {
                // Empty value, select first item
                SelectFirstItem();
            }
            else
            {
                // Select item by value
                SelectItem(value);
            }
        }
        else
        {
            // Load selected value to drop-down list
            if (!String.IsNullOrEmpty(value))
            {
                // Pre-select item from Value field
                ListItem selectedItem = ControlsHelper.FindItemByValue(drpSingleSelect, value, false);

                // Select item which is already loaded in drop-down list
                if (selectedItem != null)
                {
                    Value = selectedItem.Value;
                }
                // Select item which is not in drop-down list
                else
                {
                    // Find item by ID
                    DataSet item = GetResultSet(value, 1);
                    if (!DataHelper.DataSourceIsEmpty(item))
                    {
                        ListItem newItem = NewListItem(item.Tables[0].Rows[0], ClientID);
                        if (newItem != null)
                        {
                            // Add selected item to drop down list
                            if (!drpSingleSelect.Items.Contains(newItem))
                            {
                                drpSingleSelect.Items.Insert(0, newItem);
                            }
                            Value = newItem.Value;
                        }
                    }
                    else
                    {
                        if (drpSingleSelect.Items.Count > 0)
                        {
                            // Set default hash from the first item
                            Value = drpSingleSelect.SelectedValue;
                        }
                    }
                }
            }
            else
            {
                if (drpSingleSelect.Items.Count > 0)
                {
                    // Set default hash from the first item
                    Value = drpSingleSelect.SelectedValue;
                }
            }
        }
    }


    /// <summary>
    /// Registers scripts.
    /// </summary>
    private void RegisterScripts()
    {
        if (mRegisterDialogScripts)
        {
            ScriptHelper.RegisterDialogScript(Page);
        }

        if (SelectionMode == SelectionModeEnum.SingleDropDownList)
        {
            mJavaScript.AppendFormat(
                @"
function HashElem_{0}() {{
    return document.getElementById('{1}');
}}
function SetHash_{0}(selector) {{
    if (selector != null && selector.selectedIndex >= 0) {{
        var hashElem = HashElem_{0}();
        if (hashElem != null) {{
            hashElem.value = selector.options[selector.selectedIndex].getAttribute('{2}');
        }}
    }}
}}
", UniSelectorClientID, GetClientID(hdnHash), SpecialFieldsDefinition.DATA_HASH_ATTRIBUTE);

            // DDL initialization
            ScriptHelper.RegisterStartupScript(this, typeof(string), "UniSelector_" + UniSelectorClientID, ScriptHelper.GetScript($"US_InitDropDown(document.getElementById('{GetClientID(drpSingleSelect)}'))"));
        }

        ScriptHelper.RegisterScriptFile(Page, "Controls/uniselector.js");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UniSelectorReady" + UniSelectorClientID, ScriptHelper.GetScript(mJavaScript.ToString()));
    }


    /// <summary>
    /// Enables/disables buttons when their Enabled property is manually set
    /// </summary>
    private void SetButtonsEnabled()
    {
        if (ButtonNewEnabled.HasValue)
        {
            btnNew.Enabled = ButtonNewEnabled.Value;
            btnDropNew.Enabled = ButtonNewEnabled.Value;
            btnAddItems.Enabled = ButtonNewEnabled.Value;
        }

        if (ButtonEditEnabled.HasValue)
        {
            btnEdit.Enabled = ButtonEditEnabled.Value;
            btnDropEdit.Enabled = ButtonEditEnabled.Value;
        }

        if (ButtonRemoveEnabled.HasValue)
        {
            btnRemove.Enabled = ButtonRemoveEnabled.Value;
        }

        // Disable edit button in transformation mode if no object is selected
        if (SelectionMode == SelectionModeEnum.SingleTransformation && objTransform.ObjectID == 0)
        {
            btnEdit.Enabled = false;
        }
    }


    /// <summary>
    /// Removes all selected items.
    /// </summary>
    private void RemoveAll()
    {
        // Unselect selected items
        if (!String.IsNullOrEmpty(hiddenField.Value))
        {
            hiddenField.Value = String.Empty;

            Reload(true);

            RaiseSelectionChanged();
        }
    }


    /// <summary>
    /// Retrieves a client identifier for the specified control that does not change during the control's life cycle, and returns it.
    /// </summary>
    /// <param name="control">The control to retrieve the client identifier for.</param>
    /// <returns>A client identifier for the specified control that does not change during the control's life cycle.</returns>
    private string GetClientID(Control control)
    {
        if (!mClientIdentifiers.TryGetValue(control, out string clientID))
        {
            clientID = control.ClientID;
            mClientIdentifiers.Add(control, clientID);
        }

        return clientID;
    }


    private void SetGridHeader()
    {
        // Display two columns when in multiple selection
        var gridView = uniGrid.GridView;

        if ((SelectionMode == SelectionModeEnum.Multiple) && (gridView.HeaderRow != null))
        {
            CMSCheckBox chkAll = new CMSCheckBox
            {
                ID = "chkAll",
                ToolTip = GetString("General.CheckAll"),
                Enabled = Enabled
            };
            chkAll.InputAttributes.Add("onclick", $"US_SelectAllItems('{UniSelectorClientID}', {ScriptSafeValueSeparator}, this, 'chk{mCheckBoxClass}')");

            var headerCell = gridView.HeaderRow.Cells[0];

            headerCell.Controls.Clear();
            headerCell.Controls.Add(chkAll);

            gridView.Columns[0].ItemStyle.CssClass = "unigrid-selection";

            if (DynamicColumnName)
            {
                gridView.HeaderRow.Cells[1].Text = GetString("general.itemname");
            }
        }
    }


    private void SetOnChangeClientScript()
    {
        if (drpSingleSelect.Enabled)
        {
            // Build onchange script
            string onChangeScript = string.Format("SetHash_{0}(this); if (!US_ItemChanged(this, '{0}')) return false;", UniSelectorClientID);
            if (!string.IsNullOrEmpty(OnBeforeClientChanged))
            {
                onChangeScript = OnBeforeClientChanged + onChangeScript;
            }
            if (!string.IsNullOrEmpty(OnAfterClientChanged))
            {
                onChangeScript += OnAfterClientChanged;
            }
            // Add open modal window JavaScript event
            drpSingleSelect.Attributes.Add("onchange", onChangeScript);
        }
    }

    #endregion


    #region "Setup methods"

    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        SetEnabled();

        switch (SelectionMode)
        {
            case SelectionModeEnum.SingleDropDownList:
                SetupSingleDropDownList();
                break;

            case SelectionModeEnum.Multiple:
                SetupMultiple();
                break;

            case SelectionModeEnum.SingleButton:
            case SelectionModeEnum.MultipleButton:
                SetupButtonMode();
                break;

            case SelectionModeEnum.SingleTextBox:
                SetupSingleTextBoxMode();
                break;

            case SelectionModeEnum.MultipleTextBox:
                SetupMultipleTextBox();
                break;

            case SelectionModeEnum.SingleTransformation:
                SetupSingleTransformationMode();
                break;
        }
    }


    /// <summary>
    /// Sets up the multiple textbox mode
    /// </summary>
    private void SetupMultipleTextBox()
    {
        SetupTextBoxButtons();
    }


    /// <summary>
    /// Sets up the single textbox mode.
    /// </summary>
    /// <remarks>
    /// Use only for SingleTextBox and MultipleTextBox modes.
    /// </remarks>
    private void SetupSingleTextBoxMode()
    {
        SetupTextBoxButtons();

        var control = GetControlWithValue();
        SetupEditButton(control);
        SetupNewButton(control);
    }


    /// <summary>
    /// Sets up client click for clear and select buttons.
    /// </summary>
    private void SetupTextBoxButtons()
    {
        var control = GetControlWithValue();

        if (!HasDependingFields)
        {
            btnClear.OnClientClick = GetCleanButtonScript();
        }

        btnSelect.OnClientClick = GetSelectButtonScript(control);
    }


    /// <summary>
    /// Sets up visibility and client click for editation button.
    /// </summary>
    private void SetupEditButton(Control control)
    {
        if (DisplayEditButton)
        {
            btnEdit.Visible = true;
            btnEdit.OnClientClick = GetEditButtonScript(control);
        }
    }


    /// <summary>
    /// Sets up visibility and client click for new button.
    /// </summary>
    private void SetupNewButton(Control control)
    {
        if (DisplayNewButton)
        {
            btnNew.Visible = true;
            btnNew.OnClientClick = GetNewButtonScript(control);
        }
    }


    /// <summary>
    /// Returns control holding selector value based on property <see cref="UniSelector.AllowEditTextBox"/>.
    /// </summary>
    private Control GetControlWithValue()
    {
        return AllowEditTextBox ? (Control)txtSingleSelect : hiddenField;
    }


    /// <summary>
    /// Sets up the single transformation mode
    /// </summary>
    private void SetupSingleTransformationMode()
    {
        btnSelect.OnClientClick = GetSelectionDialogScript();

        var control = GetControlWithValue();
        SetupEditButton(control);
        SetupNewButton(control);
    }


    /// <summary>
    /// Sets up the button mode
    /// </summary>
    private void SetupButtonMode()
    {
        if (Enabled)
        {
            btnDialog.OnClientClick = GetSelectionDialogScript();
        }
    }


    /// <summary>
    /// Sets up the multiple selection mode
    /// </summary>
    private void SetupMultiple()
    {
        btnAddItems.OnClientClick = GetSelectionDialogScript();

        // Remove buttons
        string confirmation = RemoveConfirmation ?? GetString("general.confirmremove");
        btnRemove.Actions.Clear();
        btnRemove.Actions.Add(new CMSButtonAction
        {
            Text = GetString("general.removeselected"),
            OnClientClick = "if (confirm(" + ScriptHelper.GetString(confirmation) + ")) { " + ControlsHelper.GetPostBackEventReference(btnRemoveSelected) + " } return false;",
        });
        btnRemove.Actions.Add(new CMSButtonAction
        {
            Text = GetString("general.removeall"),
            OnClientClick = "US_ContextRemoveAll(" + ScriptHelper.GetString(UniSelectorClientID) + "); return false;"
        });

        const string removeScript = @"
    function US_ContextRemoveAll(clientId) {
        setTimeout('US_RemoveAll_' + clientId + '();');
    }
";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "US_CM_RemoveAll", ScriptHelper.GetScript(removeScript));
    }


    /// <summary>
    /// Sets up the single dropdown list view
    /// </summary>
    private void SetupSingleDropDownList()
    {
        var ctrl = UseUniSelectorAutocomplete ? hdnValue : (Control)drpSingleSelect;

        // Edit button
        if (DisplayEditButton)
        {
            btnDropEdit.Visible = true;
            btnDropEdit.ButtonControl.RenderScript = true;
            btnDropEdit.OnClientClick = GetEditButtonScript(ctrl);
        }

        // New button
        if (DisplayNewButton)
        {
            btnDropNew.Visible = true;
            btnDropNew.OnClientClick = GetNewButtonScript(ctrl);
        }
    }


    private string GetSelectButtonScript(Control inputControl)
    {
        return $"US_SelectionDialog_{UniSelectorClientID}('$|' + US_GetVal('{GetClientID(inputControl)}')); return false;";
    }


    private string GetCleanButtonScript()
    {
        return $"US_SetVal('{GetClientID(hiddenField)}', ''); US_SetVal('{GetClientID(txtSingleSelect)}', ''); return false;";
    }


    private string GetEditButtonScript(Control inputControl)
    {
        return $"US_EditItem_{UniSelectorClientID}(US_GetVal('{GetClientID(inputControl)}')); return false;";
    }


    private string GetNewButtonScript(Control inputControl)
    {
        return $"US_NewItem_{UniSelectorClientID}(US_GetVal('{GetClientID(inputControl)}')); return false;";
    }

    #endregion


    #region "Display methods"

    /// <summary>
    /// Displays single selection textbox.
    /// </summary>
    private void DisplayTextBox()
    {
        plcTextBoxSelect.Visible = true;

        if (!AllowEmpty)
        {
            btnClear.Visible = false;
        }

        if (AllowEditTextBox)
        {
            // Load the selected value
            txtSingleSelect.ReadOnly = false;
        }
        else
        {
            // Get the item
            txtSingleSelect.Text = GetDisplayName(Value);
        }
    }


    /// <summary>
    /// Displays result via object transformation.
    /// </summary>
    private void DisplayTransformation()
    {
        plcTextBoxSelect.Visible = objTransform.Visible = true;
        txtSingleSelect.Visible = false;

        // Initialize object transformation
        objTransform.ObjectType = ObjectType;
        objTransform.ObjectID = ValidationHelper.GetInteger(Value, 0);
        objTransform.ContextResolver = ContextResolver;
        objTransform.EncodeOutput = EncodeOutput;
        objTransform.LocalizeStrings = LocalizeItems;
        objTransform.NoDataTransformation = NoDataTransformation;
        // If Transformation is empty, set object display name as default transformation
        objTransform.Transformation = !String.IsNullOrEmpty(Transformation) ? Transformation : ((Object != null) ? Object.DisplayNameColumn : String.Empty);
    }


    /// <summary>
    /// Displays single selection drop down list.
    /// </summary>
    private void DisplayDropDownList(bool forceReload)
    {
        plcDropdownSelect.Visible = true;
        String value;

        if (UseUniSelectorAutocomplete && !RequestHelper.IsCallback())
        {
            pnlAutocomplete.Visible = true;
            drpSingleSelect.Visible = false;

            // Init special fields
            LoadSpecialFields(ClientID);
            SpecialFields.FillItems(mResolvedSpecialFields, ClientID);

            // Load default items set
            mInitSet = CreateSearchResultSet(String.Empty);
            HasData = !DataHelper.DataSourceIsEmpty(mResultDs);

            EnsureSelectedField(ValidationHelper.GetString(Value, String.Empty));
            mEnsureSelectedField = false;
            value = hdnValue.Value;
        }
        else
        {
            if (!RequestHelper.IsPostBack() || forceReload || (drpSingleSelect.Items.Count == 0) || !String.IsNullOrEmpty(EnabledColumnName))
            {
                object selectedValue = Value;

                // Prepare controls and variables
                drpSingleSelect.Items.Clear();

                bool hasData = !DataHelper.DataSourceIsEmpty(mResultDs);

                // Load data to drop-down list
                if (hasData && (Object != null))
                {
                    bool maxExceeded = (mResultDs.Tables[0].Rows.Count > MaxDisplayedTotalItems);

                    // Populate the dropdownlist
                    int index = 0;
                    foreach (DataRow dr in mResultDs.Tables[0].Rows)
                    {
                        ListItem li = NewListItem(dr, ClientID);
                        if (li != null)
                        {
                            drpSingleSelect.Items.Add(li);

                            if (maxExceeded && (++index >= MaxDisplayedItems))
                            {
                                break;
                            }
                        }
                    }

                    // Check if all items were displayed or if '(more items)' item should be added
                    if (maxExceeded)
                    {
                        drpSingleSelect.Items.Add(NewListItem(GetString("general.moreitems"), US_MORE_RECORDS.ToString(), ClientID));
                    }
                }

                // Load special fields
                LoadSpecialFields(ClientID);

                // New item link
                if (DisplayNewButton)
                {
                    drpSingleSelect.Items.Add(NewListItem(GetString("general.newitem"), US_NEW_RECORD.ToString(), ClientID));
                }

                // Load selected value to drop-down list
                EnsureSelectedField(ValidationHelper.GetString(selectedValue, null));

                // If no data in drop-down list, show none
                if (drpSingleSelect.Items.Count == 0)
                {
                    // Get item name
                    string name = GetString("general.empty");

                    drpSingleSelect.Items.Insert(0, NewListItem(name, NoneRecordValue, ClientID));
                    EnsureSelectedField(null);
                }

                HasData = hasData;
            }

            if ((drpSingleSelect.Items.Count == 1) && drpSingleSelect.SelectedValue.Equals(NoneRecordValue, StringComparison.Ordinal))
            {
                // Disable if only no-data record was added
                drpSingleSelect.Enabled = false;
            }
            else
            {
                drpSingleSelect.Enabled = Enabled;
            }

            value = drpSingleSelect.SelectedValue;
        }

        // Enable / disable the edit button
        switch (value)
        {
            case "":
            case "0":
            case "-1":
            case "-2":
            case "-3":
                btnDropEdit.Enabled = false;
                break;

            default:
                btnDropEdit.Enabled = Enabled;
                break;
        }
    }


    /// <summary>
    /// Displays multiple selection grid.
    /// </summary>
    private void DisplayMultiple(bool forceReload)
    {
        pnlGrid.Visible = true;

        uniGrid.GridName = GridName;
        uniGrid.LoadGridDefinition();

        bool hasData = !DataHelper.DataSourceIsEmpty(mResultDs);

        // Load data to unigrid
        if (hasData || forceReload)
        {
            uniGrid.DataSource = mResultDs;
            if (!RequestHelper.IsPostBack())
            {
                uniGrid.Pager.DefaultPageSize = ItemsPerPage;
            }
            uniGrid.ReloadData();
        }

        // Display "No data" message
        if (!hasData)
        {
            lblStatus.Text = ZeroRowsText ?? GetString("general.nodata");
        }
        else
        {
            lblStatus.Text = String.Empty;
        }

        btnRemove.Visible = hasData;

        HasData = hasData;
    }


    /// <summary>
    /// Displays multiple selection textbox.
    /// </summary>
    private void DisplayMultipleTextBox()
    {
        plcTextBoxSelect.Visible = true;

        if (!AllowEmpty)
        {
            btnClear.Visible = false;
        }

        // Setup the textbox
        if (AllowEditTextBox)
        {
            txtSingleSelect.ReadOnly = false;
        }
        else
        {
            txtSingleSelect.Text = ValidationHelper.GetString(Value, String.Empty).Trim(';', ' ');
        }
    }


    /// <summary>
    /// Displays selection button.
    /// </summary>
    private void DisplayButton()
    {
        plcButtonSelect.Visible = true;
    }

    #endregion


    #region "Autocomplete methods"

    /// <summary>
    /// Creates JSON string with data representation
    /// </summary>
    /// <param name="search">Search term</param>
    private String CreateSearchResultSet(string search)
    {
        String result = String.Empty;

        StringBuilder sb = new StringBuilder();
        BaseInfo bi = ModuleManager.GetReadOnlyObject(ObjectType);
        if (bi != null)
        {
            // Create where condition based on search
            if (!String.IsNullOrEmpty(search))
            {
                string where = new WhereCondition()
                    .WhereContains(bi.Generalized.DisplayNameColumn, search)
                    .ToString(true);
                WhereCondition = SqlHelper.AddWhereCondition(WhereCondition, where);
            }

            // Load special fields
            if (SpecialFields.Count == 0)
            {
                LoadSpecialFields(ClientID);
                SpecialFields.FillItems(mResolvedSpecialFields, ClientID);
            }

            // Read dataset based on where condition and object type
            mResultDs = GetResultSet(null, 0);

            int count = !DataHelper.DataSourceIsEmpty(mResultDs) ? mResultDs.Tables[0].Rows.Count : 0;
            int sfCount = 0;
            sb.Append("[");

            // Add more items field
            if (count > MaxDisplayedItems)
            {
                String moreItemsLabel = String.Format(GetString("uniselector.moreitemsavailable"), MaxDisplayedItems, count);
                AppendField(moreItemsLabel, US_MORE_RECORDS.ToString(), String.Empty, sb);
            }

            if (search != null)
            {
                // Create list with special fields
                foreach (ListItem li in mResolvedSpecialFields)
                {
                    // Append only special field that match the search term
                    if (li.Text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        sfCount++;
                        AppendField(li.Text, li.Value, li.Attributes[SpecialFieldsDefinition.DATA_HASH_ATTRIBUTE], sb);
                        mDropDownItems.Add(li);
                    }
                }
            }

            // Append data
            for (int i = 0; ((i < MaxDisplayedItems) && (i < count)); i++)
            {
                DataRow dr = mResultDs.Tables[0].Rows[i];
                ListItem li = NewListItem(dr, ClientID);
                if (li != null)
                {
                    AppendField(li.Text, li.Value, li.Attributes[SpecialFieldsDefinition.DATA_HASH_ATTRIBUTE], sb);
                    mDropDownItems.Add(li);
                }
            }

            if ((count == 0) && (sfCount == 0))
            {
                // Send no data found
                AppendField(GetString("general.nodatafound"), "##NODATAFOUND##", String.Empty, sb);
            }

            result = sb.ToString().Trim(',') + "]";
        }

        return result;
    }


    /// <summary>
    /// Appends single field to JSON result set.
    /// </summary>
    /// <param name="text">Field display name</param>
    /// <param name="value">Field value</param>
    /// <param name="hash">Field has</param>
    /// <param name="sb">String builder for append</param>
    private static void AppendField(string text, string value, string hash, StringBuilder sb)
    {
        sb.Append("{\"Text\":\"", HTMLHelper.HTMLEncode(text), "\",\"Value\":\"", value, "\"");
        if (!String.IsNullOrEmpty(hash))
        {
            sb.Append(",\"Hash\":\"", hash, "\"");
        }

        sb.Append("},");
    }


    /// <summary>
    /// Selects first item, if no special fields are set, select first item from data.
    /// </summary>
    private void SelectFirstItem()
    {
        // Set first Special value field, if any
        if (mResolvedSpecialFields.Count > 0)
        {
            SelectSpecialItem(mResolvedSpecialFields[0]);
        }
        else
        {
            // Select first item from data
            SelectItem(String.Empty);
        }
    }


    /// <summary>
    /// Select first item from data (based by ID)
    /// </summary>
    /// <param name="id">ID of element to set. If empty, first item is set</param>
    private void SelectItem(String id)
    {
        // Search special fields
        ListItem li = mResolvedSpecialFields.FindByValue(id, true);
        if (li != null)
        {
            SelectSpecialItem(li);
            return;
        }

        // Search data
        DataSet ds = GetResultSet(id, 1);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Select first item by ID
            DataRow dr = ds.Tables[0].Rows[0];
            string itemname = GetItemName(dr);
            if (LocalizeItems)
            {
                itemname = ResHelper.LocalizeString(itemname);
            }

            // Store data to elements
            txtAutocomplete.Text = itemname;
            hdnValue.Value = dr[ReturnColumnName].ToString();
            SetHashValue();
        }
        else if (id != String.Empty)
        {
            // No data was found, select first item. No additional search for empty ID (empty dataset)
            SelectFirstItem();
        }
    }


    /// <summary>
    /// Select special item based by list item data
    /// </summary>
    /// <param name="li">List item with data</param>
    private void SelectSpecialItem(ListItem li)
    {
        txtAutocomplete.Text = li.Text;
        hdnValue.Value = li.Value;
        hdnHash.Value = li.Attributes[SpecialFieldsDefinition.DATA_HASH_ATTRIBUTE];
    }


    /// <summary>
    /// Registers scripts for autocomplete functionality.
    /// </summary>
    private void RegisterAutocompleteScripts()
    {
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterJQueryUI(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/Controls/Autocomplete.js");

        // Indicate whether use callback for smart search. For little items, use local array
        bool useCallback = (!DataHelper.DataSourceIsEmpty(mResultDs) && (mResultDs.Tables[0].Rows.Count > MaxDisplayedItems));

        // Events creation
        String events = String.Empty;
        if (!String.IsNullOrEmpty(OnBeforeClientChanged))
        {
            events += $"$cmsj('#{ClientID}_txtAutocomplete').bind('onBeforeChange', function (e, value) {{{OnBeforeClientChanged}}});";
        }

        if (!String.IsNullOrEmpty(OnAfterClientChanged))
        {
            events += $"$cmsj('#{ClientID}_txtAutocomplete').bind('onAfterChange', function (e, value) {{{OnAfterClientChanged}}});";
        }

        // Initial javascripts
        String init = String.Format(@"
var {0}_initialSet = {6};
$cmsj(document).ready(function () {{
    setUpSelector('{0}', '{1}',{2},{5},{7},'{9}','{10}');
    {8}
}});
function {0}_ssCallBack(search) {{{3}}};
function {0}_result (result) {{    
   $cmsj('#{0}_txtAutocomplete').data('responseData',result);
}}
function {0}_postback(){{
 {4}
}}
", ClientID, ObjectType, DropDownSingleSelect.AutoPostBack ? "true" : "false", Page.ClientScript.GetCallbackEventReference(this, "search", ClientID + "_result", null),
    Page.ClientScript.GetPostBackEventReference(this, "selectionchanged"), AllowEmpty ? "true" : "false", String.IsNullOrEmpty(mInitSet) ? "''" : mInitSet, useCallback ? "true" : "false", events,
    GetString("general.nodatafound"), mAdditionalAutocompleteWidgetClass);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(String), ClientID + "InitAutocompleteDropDown", ScriptHelper.GetScript(init));
    }


    /// <summary>
    /// Generates hash from value based on <see cref="SelectionMode"/>.
    /// </summary>
    /// <param name="initHash">Indicates whether hash should be initialized based on empty string value.</param>
    private void SetHashValue(bool initHash = false)
    {
        if (initHash && mAllowInitHash)
        {
            if (String.IsNullOrEmpty(hdnHash.Value))
            {
                hdnHash.Value = ValidationHelper.GetHashString(String.Empty, new HashSettings(ClientID));
            }

            return;
        }

        switch (SelectionMode)
        {
            case SelectionModeEnum.SingleDropDownList:
                hdnHash.Value = ValidationHelper.GetHashString(hdnValue.Value, new HashSettings(ClientID));
                break;

            default:
                hdnHash.Value = ValidationHelper.GetHashString(hiddenField.Value, new HashSettings(ClientID));
                break;
        }
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Handling of the postback event.
    /// </summary>
    /// <param name="eventArgument">Event argument (selected value)</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument)
        {
            case "removeall":
                // Remove all items
                RemoveAll();
                break;

            case "selectitems":
                // Raise items selected event
                RaiseOnItemsSelected();
                break;

            case "refresh":
                // Reload the data without raising changed event
                Reload(true);
                break;

            case "reload":
                // Reload the data
                Reload(true);

                // Raise selection changed event
                RaiseSelectionChanged();
                break;

            case "selectionchanged":
                // Raise selection changed event
                RaiseSelectionChanged();
                break;

            case "selectnewvalue":
                // Select new item
                switch (SelectionMode)
                {
                    // Single textbox mode
                    case SelectionModeEnum.SingleTextBox:
                        txtSingleSelect.Text = hiddenSelected.Value;
                        break;

                    // Single transformation
                    case SelectionModeEnum.SingleTransformation:
                        Value = hiddenSelected.Value;
                        break;

                    // Single dropdown list
                    case SelectionModeEnum.SingleDropDownList:

                        // Reload data and select new value
                        Reload(true);

                        // Ensure selected item
                        EnsureSelectedField(hiddenSelected.Value);

                        ListItem selectedItem = ControlsHelper.FindItemByValue(drpSingleSelect, hiddenSelected.Value, false);
                        if (selectedItem != null)
                        {
                            btnDropEdit.Enabled = Enabled;
                            drpSingleSelect.Enabled = Enabled;
                        }
                        break;
                }

                hiddenSelected.Value = String.Empty;
                break;
        }

        // Raise form engine user control Changed event
        RaiseOnChanged();
    }

    #endregion


    #region "ICallbackEventHandler Members"

    string ICallbackEventHandler.GetCallbackResult()
    {
        // Prepare the parameters for dialog
        SetDialogParameters();

        return UseUniSelectorAutocomplete ? mDropDownSearchResults : String.Empty;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        // Adopt new value from callback
        if (eventArgument.StartsWith("$|", StringComparison.OrdinalIgnoreCase))
        {
            Value = eventArgument.Substring(2);
        }

        if (UseUniSelectorAutocomplete)
        {
            // Request for new filtered data for smart search drop down
            if (eventArgument.StartsWith("##", StringComparison.OrdinalIgnoreCase))
            {
                mDropDownSearchResults = CreateSearchResultSet(eventArgument.Substring(2));
            }

            // Smart search more items request, add dialog where condition based on typed value
            if (eventArgument.StartsWith(US_MORE_RECORDS.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                string searchTerm = eventArgument.Substring(US_MORE_RECORDS.ToString().Length);
                if (!String.IsNullOrEmpty(searchTerm) && (Object != null))
                {
                    WhereCondition = SqlHelper.AddWhereCondition(WhereCondition, new WhereCondition().WhereContains(Object.DisplayNameColumn, searchTerm).ToString(true));
                }
            }
        }
    }

    #endregion
}
