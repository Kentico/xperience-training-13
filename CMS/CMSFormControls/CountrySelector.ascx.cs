using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Globalization;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSFormControls_CountrySelector : FormEngineUserControl
{
    #region "Variables"

    private bool mDisplayAllItems = true;
    private bool? mAddAllItemsRecord;
    private bool? mAddNoneRecord;
    private bool mUseCodeNameForSelection = true;
    private bool? mAddSelectCountryRecord;
    private bool? mEnableStateSelection;
    private bool? mUseStateSelection;
    private ReturnTypeEnum? mReturnType;

    /// <summary>
    /// Indicates what return value should be submited by the control.
    /// </summary>
    private enum ReturnTypeEnum
    {
        /// <summary>
        /// Default value. Returns string value with both countryID and stateID separated by semicolumn.
        /// </summary>
        Both = 0,

        /// <summary>
        /// Returns integer value containing only countryID.
        /// </summary>
        CountryID = 1,

        /// <summary>
        /// Returns integer value containing only stateID.
        /// </summary>
        StateID = 2
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets client ID of the country drop down list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return CountryDropDown.ClientID;
        }
    }


    /// <summary>
    /// Gets client ID of the country drop down list.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            return CountryDropDown.ClientID;
        }
    }



    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            if (uniSelectorCountry != null)
            {
                uniSelectorCountry.Enabled = value;
            }
            if (uniSelectorState != null)
            {
                uniSelectorState.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value which indicates whether the selector should load all data to DDL.
    /// </summary>
    public bool DisplayAllItems
    {
        get
        {
            return mDisplayAllItems;
        }
        set
        {
            mDisplayAllItems = value;
            if ((uniSelectorCountry != null) && (uniSelectorState != null))
            {
                uniSelectorCountry.MaxDisplayedItems = (value ? 300 : UniSelector.DefaultMaxDisplayedItems);
                uniSelectorState.MaxDisplayedItems = (value ? 100 : UniSelector.DefaultMaxDisplayedItems);

                uniSelectorCountry.MaxDisplayedTotalItems = uniSelectorCountry.MaxDisplayedItems + 50;
                uniSelectorState.MaxDisplayedTotalItems = uniSelectorState.MaxDisplayedItems + 50;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add '(all)' item record to the CMSDropDownList.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            if (mAddAllItemsRecord == null)
            {
                mAddAllItemsRecord = ValidationHelper.GetBoolean(GetValue("AddAllItemsRecord"), false);
            }
            return (bool)mAddAllItemsRecord;
        }
        set
        {
            mAddAllItemsRecord = value;
        }
    }


    /// <summary>
    /// Add '(none)' record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            if (mAddNoneRecord == null)
            {
                mAddNoneRecord = ValidationHelper.GetBoolean(GetValue("AddNoneRecord"), false);
            }
            return (bool)mAddNoneRecord;
        }
        set
        {
            mAddNoneRecord = value;
        }
    }


    /// <summary>
    /// Add '(select country)' record to the dropdownlist.
    /// </summary>
    public bool AddSelectCountryRecord
    {
        get
        {
            if (mAddSelectCountryRecord == null)
            {
                mAddSelectCountryRecord = ValidationHelper.GetBoolean(GetValue("AddSelectCountryRecord"), true);
            }
            return (bool)mAddSelectCountryRecord;
        }
        set
        {
            mAddSelectCountryRecord = value;
        }
    }


    /// <summary>
    /// If set to true also state selection will be available in the control.
    /// </summary>
    public bool EnableStateSelection
    {
        get
        {
            if (mEnableStateSelection == null)
            {
                mEnableStateSelection = ValidationHelper.GetBoolean(GetValue("EnableStateSelection"), true);
            }
            return (bool)mEnableStateSelection;
        }
        set
        {
            mEnableStateSelection = value;
        }
    }


    /// <summary>
    /// Set/get Value property in the form 'CountryName;StateName' or 'CountryID;StateID'
    /// </summary>
    public bool UseCodeNameForSelection
    {
        get
        {
            return mUseCodeNameForSelection && ReturnType == ReturnTypeEnum.Both;
        }
        set
        {
            mUseCodeNameForSelection = value;
        }
    }


    /// <summary>
    /// Selected country ID.
    /// </summary>
    public int CountryID
    {
        get
        {
            if (UseCodeNameForSelection)
            {
                // Convert country name to ID
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(ValidationHelper.GetString(uniSelectorCountry.Value, String.Empty));
                if (ci != null)
                {
                    return ci.CountryID;
                }
            }
            else
            {
                return ValidationHelper.GetInteger(uniSelectorCountry.Value, 0);
            }

            return 0;
        }
        set
        {
            // Check if code name is used for selection
            if (UseCodeNameForSelection)
            {
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(value);
                if (ci != null)
                {
                    uniSelectorCountry.Value = ci.CountryName;
                }
            }
            else
            {
                uniSelectorCountry.Value = value;
            }

            uniSelectorState.WhereCondition = "CountryID = " + value;
        }
    }


    /// <summary>
    /// Selected State ID. Zero if not available.
    /// </summary>
    public int StateID
    {
        get
        {
            if (UseStateSelection)
            {
                // Check id using code name for selection
                if (UseCodeNameForSelection)
                {
                    // Convert state name to ID
                    StateInfo si = StateInfoProvider.GetStateInfo(ValidationHelper.GetString(uniSelectorState.Value, String.Empty));
                    if (si != null)
                    {
                        return si.StateID;
                    }
                }
                else
                {
                    return ValidationHelper.GetInteger(uniSelectorState.Value, 0);
                }
            }

            return 0;
        }
        set
        {
            // Check id using code name for selection
            if (UseCodeNameForSelection)
            {
                // Convert state ID to name
                StateInfo si = StateInfoProvider.GetStateInfo(value);
                if (si != null)
                {
                    uniSelectorState.Value = si.StateName;
                }
            }
            else
            {
                uniSelectorState.Value = value;
            }
        }
    }


    /// <summary>
    /// Selected Country name.
    /// </summary>
    public string CountryName
    {
        get
        {
            if (UseCodeNameForSelection)
            {
                return ValidationHelper.GetString(uniSelectorCountry.Value, String.Empty);
            }

            CountryInfo ci = CountryInfoProvider.GetCountryInfo(ValidationHelper.GetInteger(uniSelectorCountry.Value, 0));
            if (ci != null)
            {
                return ci.CountryName;
            }

            return String.Empty;
        }
        set
        {
            if (UseCodeNameForSelection)
            {
                uniSelectorCountry.Value = value;
            }
            else
            {
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(value);
                if (ci != null)
                {
                    uniSelectorCountry.Value = ci.CountryID;
                }
            }

            uniSelectorState.WhereCondition = "CountryID = " + CountryID;
        }
    }


    /// <summary>
    /// Selected State name.
    /// </summary>
    public string StateName
    {
        get
        {
            if (UseStateSelection)
            {
                if (UseCodeNameForSelection)
                {
                    return ValidationHelper.GetString(uniSelectorState.Value, String.Empty);
                }

                StateInfo si = StateInfoProvider.GetStateInfo(ValidationHelper.GetInteger(uniSelectorState.Value, 0));
                if (si != null)
                {
                    return si.StateName;
                }
            }

            return String.Empty;
        }
        set
        {
            if (UseCodeNameForSelection)
            {
                uniSelectorState.Value = value;
            }
            else
            {
                StateInfo si = StateInfoProvider.GetStateInfo(value);
                if (si != null)
                {
                    uniSelectorState.Value = si.StateID;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Return only country ID
            if (ReturnType == ReturnTypeEnum.CountryID)
            {
                return CountryID == 0 ? null : (object)CountryID;
            }

            // Return only state ID
            if (ReturnType == ReturnTypeEnum.StateID)
            {
                return StateID == 0 ? null : (object)StateID;
            }

            // Return string with country and state IDs
            string val;
            if (UseCodeNameForSelection)
            {
                val = (!string.IsNullOrEmpty(StateName)) ? CountryName + ";" + StateName : CountryName;
            }
            else
            {
                val = (StateID > 0) ? CountryID + ";" + StateID : CountryID.ToString();
            }

            return (val == ";") ? null : val;
        }
        set
        {
            // Load panel
            if ((uniSelectorCountry == null) || (uniSelectorState == null))
            {
                pnlUpdate.LoadContainer();
            }

            // Get only country ID
            if (ReturnType == ReturnTypeEnum.CountryID)
            {
                CountryID = ValidationHelper.GetInteger(value, 0);

                LoadOtherValues();
            }
                // Get only stateID
            else if (ReturnType == ReturnTypeEnum.StateID)
            {
                int stateId = ValidationHelper.GetInteger(value, 0);
                StateID = stateId;

                // Find country from state info
                StateInfo state = StateInfoProvider.GetStateInfo(stateId);
                if (state != null)
                {
                    CountryID = state.CountryID;
                }
            }
            // Get both country and state IDs
            else
            {
                string[] ids = ValidationHelper.GetString(value, string.Empty).Split(';');

                if (ids.Length >= 1)
                {
                    if (UseCodeNameForSelection)
                    {
                        CountryName = ValidationHelper.GetString(ids[0], string.Empty);
                    }
                    else
                    {
                        CountryID = ValidationHelper.GetInteger(ids[0], 0);
                    }
                    if (ids.Length == 2)
                    {
                        if (UseCodeNameForSelection)
                        {
                            StateName = ValidationHelper.GetString(ids[1], string.Empty);
                        }
                        else
                        {
                            StateID = ValidationHelper.GetInteger(ids[1], 0);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Returns the DDL with countries.
    /// </summary>
    public CMSDropDownList CountryDropDown
    {
        get
        {
            return uniSelectorCountry.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Returns the DDL with states.
    /// </summary>
    public CMSDropDownList StateDropDown
    {
        get
        {
            return uniSelectorState.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Indicates whether state is selected. Returns true if no states offered.
    /// </summary>
    public bool StateSelectionIsValid
    {
        get
        {
            return (StateID > 0) || !UseStateSelection;
        }
    }


    /// <summary>
    /// Gets a column name for state ID.
    /// </summary>
    public string StateIDColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StateIDColumnName"), String.Empty);
        }
        set
        {
            SetValue("StateIDColumnName", value);
        }
    }

    #endregion


    #region "Private properties"

    private bool UseStateSelection
    {
        get
        {
            var value = mUseStateSelection ?? (mUseStateSelection = EnableStateSelection && uniSelectorState.HasData);
            return (bool)value;
        }
    }


    private ReturnTypeEnum ReturnType
    {
        get
        {
            if (mReturnType == null)
            {
                mReturnType = (ReturnTypeEnum)ValidationHelper.GetInteger(GetValue("ReturnType"), 0);
            }
            return mReturnType.Value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (HasDependingFields && !ControlsHelper.IsInUpdatePanel(this))
        {
            // Make state selector full postback trigger if not in update panel (e.g. web parts's update panel)
            // and remove async postback trigger of country selector
            pnlUpdate.Triggers.Clear();
            pnlUpdate.Triggers.Add(new PostBackTrigger { ControlID = "uniSelectorState:drpSingleSelect" });
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            uniSelectorCountry.StopProcessing = true;
            uniSelectorState.StopProcessing = true;
        }
        else
        {
            // Init selector for countries
            uniSelectorCountry.IsLiveSite = IsLiveSite;
            uniSelectorCountry.DropDownSingleSelect.AutoPostBack = EnableStateSelection || HasDependingFields;
            uniSelectorCountry.ReturnColumnName = (UseCodeNameForSelection ? "CountryName" : "CountryID");
            uniSelectorCountry.MaxDisplayedItems = (DisplayAllItems ? 300 : UniSelector.DefaultMaxDisplayedItems);
            uniSelectorCountry.MaxDisplayedTotalItems = uniSelectorCountry.MaxDisplayedItems + 50;

            // Add (all) record when requested
            if (AddAllItemsRecord)
            {
                uniSelectorCountry.AllowAll = true;
                uniSelectorState.AllowAll = true;
            }
            else
            {
                if (AddSelectCountryRecord)
                {
                    // Add (select country) record when requested
                    uniSelectorCountry.SpecialFields.Add(new SpecialField { Text = GetString("countryselector.selectcountryrecord"), Value = string.Empty });
                    uniSelectorState.SpecialFields.Add(new SpecialField { Text = GetString("countryselector.selectstaterecord"), Value = string.Empty });
                    uniSelectorState.AllowEmpty = false;
                }
                if (AddNoneRecord)
                {
                    // Add (none) record when requested
                    uniSelectorCountry.SpecialFields.Add(new SpecialField { Text = GetString("general.selectnone"), Value = string.Empty });
                }
            }

            // Init selector for states
            uniSelectorState.IsLiveSite = IsLiveSite;
            uniSelectorState.ReturnColumnName = (UseCodeNameForSelection ? "StateName" : "StateID");
            uniSelectorState.MaxDisplayedItems = (DisplayAllItems ? 100 : UniSelector.DefaultMaxDisplayedItems);
            uniSelectorState.MaxDisplayedTotalItems = uniSelectorState.MaxDisplayedItems + 50;
            uniSelectorState.WhereCondition = "CountryID = " + CountryID;
            if (HasDependingFields)
            {
                uniSelectorState.OnSelectionChanged += uniSelectorState_OnSelectionChanged;
                uniSelectorState.DropDownSingleSelect.AutoPostBack = true;
            }
            mUseStateSelection = null;

            if (UseCodeNameForSelection)
            {
                uniSelectorState.AllRecordValue = string.Empty;
                uniSelectorState.NoneRecordValue = string.Empty;
                uniSelectorCountry.AllRecordValue = string.Empty;
                uniSelectorCountry.NoneRecordValue = string.Empty;
            }
        }
    }


    /// <summary>
    /// Hide States DDL if there is no state for selected country.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (CountryID > 0)
        {
            uniSelectorState.StopProcessing = false;
            plcStates.Visible = UseStateSelection;
        }
        else
        {
            plcStates.Visible = false;
            uniSelectorState.StopProcessing = true;
        }
    }


    /// <summary>
    /// Country DropDownList Selection change.
    /// </summary>
    protected void uniSelectorCountry_OnSelectionChanged(object sender, EventArgs e)
    {
        if (CountryID > 0)
        {
            uniSelectorState.WhereCondition = "CountryID = " + CountryID;

            uniSelectorState.StopProcessing = false;
            uniSelectorState.Reload(true);
            mUseStateSelection = null;

            // Raise change event
            RaiseOnChanged();
        }
        else
        {
            uniSelectorState.StopProcessing = true;
        }
    }


    /// <summary>
    /// State DropDownList Selection change.
    /// </summary>
    protected void uniSelectorState_OnSelectionChanged(object sender, EventArgs e)
    {
        // Raise change event
        RaiseOnChanged();

        // Update parent update panel if exists (e.g. the web part's one)
        var parentUpdatePanel = ControlsHelper.GetUpdatePanel(this);
        parentUpdatePanel?.Update();
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    /// <param name="forceReload">If true uniselectors are reloaded</param>
    public void ReloadData(bool forceReload = false)
    {
        uniSelectorCountry.Reload(forceReload);

        int countryId = 0;
        if (UseCodeNameForSelection)
        {
            CountryInfo ci = CountryInfoProvider.GetCountryInfo(ValidationHelper.GetString(uniSelectorCountry.Value, string.Empty));
            if (ci != null)
            {
                countryId = ci.CountryID;
            }
        }
        else
        {
            countryId = ValidationHelper.GetInteger(uniSelectorCountry.Value, 0);
        }

        uniSelectorState.WhereCondition = "CountryID = " + countryId;
        uniSelectorState.Reload(forceReload);
        mUseStateSelection = null;
    }


    /// <summary>
    /// Loads the other fields values to the state of the form control
    /// </summary>
    public override void LoadOtherValues()
    {
        // Try to set state from dedicated column in form data
        if (ContainsColumn(StateIDColumnName))
        {
            // Select state in stateSelector in the form if StateIDColumnName was supplied
            var stateInfo = StateInfoProvider.GetStateInfo(ValidationHelper.GetInteger(GetColumnValue(StateIDColumnName), 0));
            if ((stateInfo != null) && (uniSelectorState != null))
            {
                if (UseCodeNameForSelection)
                {
                    uniSelectorState.Value = stateInfo.StateName;
                }
                else
                {
                    uniSelectorState.Value = stateInfo.StateID;
                }
            }
        }
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (!string.IsNullOrEmpty(StateIDColumnName))
        {
            // Set properties names
            object[,] values = new object[1, 2];
            values[0, 0] = StateIDColumnName;
            values[0, 1] = null;

            if (UseCodeNameForSelection)
            {
                if (!string.IsNullOrEmpty(StateName))
                {
                    values[0, 1] = StateName;
                }
            }
            else if (StateID > 0)
            {
                values[0, 1] = StateID;
            }

            return values;
        }
        return null;
    }


    /// <summary>
    /// Checks if state selection is valid.
    /// </summary>
    /// <returns>True if state selection is valid</returns>
    public override bool IsValid()
    {
        return StateSelectionIsValid;
    }


    /// <summary>
    /// Returns true if state selector is visible.
    /// </summary>
    public bool StateIsVisible()
    {
        return uniSelectorState.HasData;
    }

    #endregion
}