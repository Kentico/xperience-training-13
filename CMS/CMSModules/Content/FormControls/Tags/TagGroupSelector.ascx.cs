using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Content_FormControls_Tags_TagGroupSelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseDropdownList;
    private int mSiteId;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (UseGroupNameForSelection)
            {
                return TagGroupName;
            }
            else
            {
                return TagGroupID;
            }
        }
        set
        {
            if (UseGroupNameForSelection)
            {
                TagGroupName = ValidationHelper.GetString(value, "");
            }
            else
            {
                TagGroupID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the TagGroup ID.
    /// </summary>
    public int TagGroupID
    {
        get
        {
            if (UseGroupNameForSelection)
            {
                string name = ValidationHelper.GetString(UniSelector.Value, "");
                TagGroupInfo tgi = TagGroupInfo.Provider.Get(name, SiteContext.CurrentSite.SiteID);
                if (tgi != null)
                {
                    return tgi.TagGroupID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(UniSelector.Value, 0);
            }
        }
        set
        {
            if (UseGroupNameForSelection)
            {
                TagGroupInfo tgi = TagGroupInfo.Provider.Get(value);
                if (tgi != null)
                {
                    UniSelector.Value = tgi.TagGroupName;
                }
            }
            else
            {
                UniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the TagGroup code name.
    /// </summary>
    public string TagGroupName
    {
        get
        {
            if (UseGroupNameForSelection)
            {
                return ValidationHelper.GetString(UniSelector.Value, "");
            }
            else
            {
                int id = ValidationHelper.GetInteger(UniSelector.Value, 0);
                TagGroupInfo tgi = TagGroupInfo.Provider.Get(id);
                if (tgi != null)
                {
                    return tgi.TagGroupName;
                }
                return "";
            }
        }
        set
        {
            if (UseGroupNameForSelection)
            {
                UniSelector.Value = value;
            }
            else
            {
                TagGroupInfo tgi = TagGroupInfo.Provider.Get(value, SiteContext.CurrentSite.SiteID);
                if (tgi != null)
                {
                    UniSelector.Value = tgi.TagGroupID;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is TagGroupName, if false, selected value is TagGroupID.
    /// </summary>
    public bool UseGroupNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseGroupNameForSelection"), true);
        }
        set
        {
            SetValue("UseGroupNameForSelection", value);
            UniSelector.ReturnColumnName = (value ? "TagGroupName" : "TagGroupID");
        }
    }


    /// <summary>
    /// Indicates whether to use dropdownlist or textbox selection mode.
    /// </summary>
    public bool UseDropdownList
    {
        get
        {
            return mUseDropdownList;
        }
        set
        {
            mUseDropdownList = value;
            UniSelector.SelectionMode = value ? SelectionModeEnum.SingleDropDownList : SelectionModeEnum.SingleTextBox;
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
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneItemsRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddNoneItemsRecord"), true);
        }
        set
        {
            SetValue("AddNoneItemsRecord", value);
            UniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether use autopostback or not.
    /// </summary>
    public bool UseAutoPostback
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseAutoPostback"), false);
        }
        set
        {
            SetValue("UseAutoPostback", value);
            if (uniSelector != null)
            {
                uniSelector.DropDownSingleSelect.AutoPostBack = value;
            }
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniSelector;
        }
    }


    /// <summary>
    /// ID of the site which tag groups are to be displayed.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId <= 0)
            {
                mSiteId = SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            UniSelector.StopProcessing = true;
        }
        else
        {
            SetFormSiteID();

            UniSelector.DisplayNameFormat = "{%TagGroupDisplayName%}";
            UniSelector.SelectionMode = UseDropdownList ? SelectionModeEnum.SingleDropDownList : SelectionModeEnum.SingleTextBox;
            UniSelector.AllowEditTextBox = UseGroupNameForSelection;
            UniSelector.WhereCondition = "TagGroupSiteID = " + SiteID;
            UniSelector.ReturnColumnName = (UseGroupNameForSelection ? "TagGroupName" : "TagGroupID");
            UniSelector.OrderBy = "TagGroupDisplayName";
            UniSelector.ObjectType = TagGroupInfo.OBJECT_TYPE;
            UniSelector.ResourcePrefix = "taggroupselect";
            uniSelector.DropDownSingleSelect.AutoPostBack = UseAutoPostback;
            UniSelector.AllowEmpty = AddNoneItemsRecord;
            UniSelector.IsLiveSite = IsLiveSite;

            if (UseGroupNameForSelection)
            {
                uniSelector.AllRecordValue = "";
                uniSelector.NoneRecordValue = "";
            }
        }
    }


    /// <summary>
    /// Loads public status according to the control settings.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.Reload(true);
    }


    /// <summary>
    /// Sets the SiteID if the SiteName field is available in the form.
    /// </summary>
    private void SetFormSiteID()
    {
        if (DependsOnAnotherField
            && (Form != null)
            && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), null);
            if (!String.IsNullOrEmpty(siteName))
            {
                SiteInfo siteObj = SiteInfo.Provider.Get(siteName);
                if (siteObj != null)
                {
                    SiteID = siteObj.SiteID;
                }
            }
            else
            {
                SiteID = -1;
            }
        }
    }
}