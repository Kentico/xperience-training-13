using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MessageBoards_FormControls_SelectBoard : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or Sets return column name. Default is BoardID.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return GetValue("ReturnColumnName", "BoardID");
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return UniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// ID of the current group.
    /// </summary>
    public int GroupID
    {
        get;
        set;
    }


    /// <summary>
    /// ID of the current site. If SiteID is null, value will be retrieved from the form control named SiteName located at the same Form.
    /// </summary>
    public int? SiteID
    {
        get;
        set;
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
    /// Gets or sets the value which determines, whether to add (all) item record to the dropdownlist.
    /// </summary>
    public bool AddAllItemsRecord
    {
        get
        {
            return GetValue("AddAllItemsRecord", false);
        }
        set
        {
            SetValue("AddAllItemsRecord", value);
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    private bool DisableSelectorForAllSites
    {
        get
        {
            return GetValue("DisableSelectorForAllSites", false);
        }
        set
        {
            SetValue("DisableSelectorForAllSites", value);
        }
    }


    /// <summary>
    /// Gets the inner DDL control.
    /// </summary>
    public DropDownList DropDownSingleSelect
    {
        get
        {
            return UniSelector.DropDownSingleSelect;
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
    /// Gets underlying form control.
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnPreRender event handler. Loads data and sets Enabled property. 
    /// Loading data is done in PreRender instead of in Load, because it relies on value of another form field (Form.GetFieldValue("SiteName")) and this value is not available until PreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData(false);
        }

        // Disable selector for all sites selection mode
        if (DisableSelectorForAllSites && (SiteID.Value == 0))
        {
            uniSelector.Enabled = false;

            if (uniSelector.DropDownSingleSelect.Items.Count > 0)
            {
                // Select (all) option
                uniSelector.DropDownSingleSelect.SelectedIndex = 0;
            }
        }
        else
        {
            uniSelector.Enabled = true;
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    /// <param name="forced">Indicates whether UniSelector Reload data should be called</param>
    public void ReloadData(bool forced)
    {
        // Try get site identifier selected in another control
        if (SiteID == null)
        {
            SiteID = GetSiteIDFromForm();
            forced = true;
        }

        uniSelector.WhereCondition = "BoardSiteID = " + (SiteID.Value != 0 ? SiteID.Value : SiteContext.CurrentSiteID) + " AND " + (GroupID > 0 ? "BoardGroupID = " + GroupID : "((BoardGroupID = 0) OR (BoardGroupID IS NULL))");
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ReturnColumnName = ReturnColumnName;

        if (AddAllItemsRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField() {Text = GetString("general.selectall"), Value = "ALL"});
        }

        if (forced)
        {
            uniSelector.Reload(true);
        }
    }


    /// <summary>
    /// Gets site id from the SiteName field if is available in the form.
    /// </summary>
    private int GetSiteIDFromForm()
    {
        if (DependsOnAnotherField && (Form != null) && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");

            // All sites
            if (String.IsNullOrEmpty(siteName) || siteName.EqualsCSafe("##all##", true))
            {
                return 0;
            }
                // Current site
            else if (siteName.EqualsCSafe("##currentsite##", true))
            {
                siteName = SiteContext.CurrentSiteName;
            }

            if (!String.IsNullOrEmpty(siteName))
            {
                // Get site ID
                SiteInfo siteObj = SiteInfoProvider.GetSiteInfo(siteName);
                if (siteObj != null)
                {
                    return siteObj.SiteID;
                }
            }
        }

        return SiteContext.CurrentSiteID;
    }

    #endregion
}