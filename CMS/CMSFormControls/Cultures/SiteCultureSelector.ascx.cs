using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Cultures_SiteCultureSelector : FormEngineUserControl
{
    #region "Constructor"

    public CMSFormControls_Cultures_SiteCultureSelector()
    {
        PostbackOnChange = false;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Underlying control
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets or sets selection mode for current control.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            return UniSelector.SelectionMode;
        }
        set
        {
            UniSelector.SelectionMode = value;
        }
    }


    /// <summary>
    /// Allows to switch to multiple selection mode. Overwrites SelectionMode property. Used only for 'Supported culture selector'
    /// </summary>
    public bool AllowMultipleSelection
    {
        get
        {
            return GetValue("AllowMultipleSelection", false);
        }
        set
        {
            SetValue("AllowMultipleSelection", value);
        }
    }


    /// <summary>
    /// Gets or sets display name format.
    /// </summary>
    public string DisplayNameFormat
    {
        get
        {
            return UniSelector.DisplayNameFormat;
        }
        set
        {
            UniSelector.DisplayNameFormat = value;
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
            UniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown with cultures.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return UniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return UniSelector.Value;
        }
        set
        {
            UniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to display all cultures 
    /// (If you set this property to True, SiteID property is ignored).
    /// </summary>
    public bool DisplayAllCultures
    {
        get
        {
            return GetValue("DisplayAllCultures", false);
        }
        set
        {
            SetValue("DisplayAllCultures", value);
        }
    }
    
   
    /// <summary>
    /// Column name of the object which value should be returned by the selector. 
    /// If NULL, ID column is used.
    /// </summary>
    public virtual string ReturnColumnName
    {
        get
        {
            return UniSelector.ReturnColumnName;
        }
        set
        {
            UniSelector.ReturnColumnName = value;
        }
    }

    
    /// <summary>
    /// Specifies, whether the selector allows empty selection.
    /// </summary>
    public bool AllowDefault
    {
        get
        {
            return UniSelector.AllowDefault;
        }
        set
        {
            UniSelector.AllowDefault = value;
        }
    }


    /// <summary>
    /// Gets/Sets all record value
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return GetValue("AllRecordValue", UniSelector.AllRecordValue);
        }
        set
        {
            SetValue("AllRecordValue", value);
            UniSelector.AllRecordValue = value;
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows selection of all items.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return UniSelector.AllowAll;
        }
        set
        {
            UniSelector.AllowAll = value;
        }
    }

    
    /// <summary>
    /// Gets or sets the ID of the site for which the cultures should be returned. Zero value means current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            return GetValue("SiteID", 0);
        }
        set
        {
            SetValue("SiteID", value);

        }
    }


    /// <summary>
    /// Returns UpdatePanel of selector.
    /// </summary>
    public CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUpdate;
        }
    }


    /// <summary>
    /// Returns CMSDropDownList with cultures.
    /// </summary>
    public CMSDropDownList DropDownCultures
    {
        get
        {
            return UniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Additional CSS class for drop down list control.
    /// </summary>
    public String AdditionalDropDownCSSClass
    {
        get
        {
            return uniSelector.AdditionalDropDownCSSClass;
        }
        set
        {
            uniSelector.AdditionalDropDownCSSClass = value;
        }
    }


    /// <summary>
    /// If true, full postback is raised when item changed.
    /// </summary>
    public bool PostbackOnChange
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets an additional WHERE condition which will be added with "AND" to an internal WHERE condition of UniSelector.
    /// </summary>
    public string AdditionalWhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Supported language cultures
    /// </summary>
    public string SupportedCultures
    {
        get
        {
            return GetValue("SupportedCultures", String.Empty);
        }
        set
        {
            SetValue("SupportedCultures", value);
        }
    }


    /// <summary>
    /// Special fields of the UniSelector.
    /// </summary>
    public string SpecialFields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SpecialFields"), String.Empty);
        }
        set
        {
            SetValue("SpecialFields", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            if (PostbackOnChange)
            {
                uniSelector.DropDownSingleSelect.AutoPostBack = true;
                ScriptManager man = ScriptManager.GetCurrent(Page);
                if (man != null)
                {
                    man.RegisterPostBackControl(uniSelector.DropDownSingleSelect);
                }
            }

            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ReturnColumnName = ReturnColumnName;
        uniSelector.AllRecordValue = AllRecordValue;

        if (AllowMultipleSelection)
        {
            SelectionMode = SelectionModeEnum.MultipleTextBox;
        }

        if (!DisplayAllCultures)
        {
            uniSelector.WhereCondition = GetWhereConditionInternal();
        }

        // Propagate Special fields settings to UniSelector
        uniSelector.SetValue("SpecialFields", SpecialFields);
    }


    protected void uniSelector_OnSpecialFieldsLoaded(object sender, EventArgs e)
    {
        if ((SelectionMode == SelectionModeEnum.SingleDropDownList) && AllowDefault)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.defaultchoice"), Value = String.Empty });
        }
    }


    /// <summary>
    /// Reloads uniselector.
    /// </summary>
    /// <param name="forceReload">Whether to force data reload</param>
    public void Reload(bool forceReload)
    {
        uniSelector.Reload(forceReload);
    }


    /// <summary>
    /// Returns WHERE condition for given site.
    /// </summary>
    private string GetWhereConditionInternal()
    {
        int siteId = (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID;
        string retval = string.Format("CultureID IN (SELECT CultureID FROM CMS_SiteCulture WHERE SiteID = {0})", siteId);

        if (!string.IsNullOrEmpty(AdditionalWhereCondition))
        {
            retval = SqlHelper.AddWhereCondition(retval, AdditionalWhereCondition, "AND");
        }

        // Add supported cultures
        if (!String.IsNullOrEmpty(SupportedCultures))
        {
            StringBuilder sb = new StringBuilder(retval);

            sb.Append(" AND CultureCode IN (");

            List<string> suppCultures = SupportedCultures.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();

            sb.Append(String.Join(",", suppCultures.Select(t => "'" + SqlHelper.GetSafeQueryString(t) + "'")));

            sb.Append(")");

            retval = sb.ToString();
        }

        return retval;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (SelectionMode == SelectionModeEnum.SingleTextBox)
        {
            // If macro or special value, do not validate
            string value = uniSelector.TextBoxSelect.Text.Trim();
            if (!MacroProcessor.ContainsMacro(value) && (value != "") && (value != TreeProvider.ALL_CULTURES))
            {
                // Check if culture exists
                CultureInfo ci = CultureInfo.Provider.Get(value);
                if (ci == null)
                {
                    ValidationError = GetString("formcontrols_selectculture.notexist").Replace("%%code%%", value);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return true;
    }

    #endregion
}