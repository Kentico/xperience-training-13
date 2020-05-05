using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_FormControls_MediaLibrarySelector : FormEngineUserControl
{
    #region "Delegates & Events"

    public delegate void OnSelectedLibraryChanged();

    public event OnSelectedLibraryChanged SelectedLibraryChanged;

    #endregion


    #region "Variables"

    private bool mUseLibraryNameForSelection = true;
    private string mSiteName = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// ID of the site libraries should belongs to.
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
    /// Gets the site name from the site ID.
    /// </summary>
    private string SiteName
    {
        get
        {
            if (mSiteName == null)
            {
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
                if (si != null)
                {
                    mSiteName = si.SiteName;
                }
            }
            return mSiteName;
        }
    }


    /// <summary>
    /// ID of the group libraries should belongs to.
    /// </summary>
    public int GroupID
    {
        get
        {
            return GetValue("GroupID", 0);
        }
        set
        {
            SetValue("GroupID", value);
        }
    }


    /// <summary>
    /// Gets or sets WHERE condition used to filter libraries.
    /// </summary>
    public string Where
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
            if (UseLibraryNameForSelection)
            {
                return MediaLibraryName;
            }
            else
            {
                return MediaLibraryID;
            }
        }
        set
        {
            if (UseLibraryNameForSelection)
            {
                MediaLibraryName = ValidationHelper.GetString(value, "");
            }
            else
            {
                MediaLibraryID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the MediaLibrary ID.
    /// </summary>
    public int MediaLibraryID
    {
        get
        {
            if (UseLibraryNameForSelection)
            {
                string name = ValidationHelper.GetString(CurrentSelector.Value, "");
                MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(name, SiteName);
                if (mli != null)
                {
                    return mli.LibraryID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(CurrentSelector.Value, 0);
            }
        }
        set
        {
            if (UseLibraryNameForSelection)
            {
                MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(value);
                if (mli != null)
                {
                    CurrentSelector.Value = mli.LibraryID;
                }
            }
            else
            {
                CurrentSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the MediaLibrary code name.
    /// </summary>
    public string MediaLibraryName
    {
        get
        {
            if (UseLibraryNameForSelection)
            {
                return ValidationHelper.GetString(CurrentSelector.Value, "");
            }
            else
            {
                int id = ValidationHelper.GetInteger(CurrentSelector.Value, 0);
                MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(id);
                if (mli != null)
                {
                    return mli.LibraryName;
                }
                return "";
            }
        }
        set
        {
            if (UseLibraryNameForSelection)
            {
                CurrentSelector.Value = value;
            }
            else
            {
                MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(value, SiteName);
                if (mli != null)
                {
                    CurrentSelector.Value = mli.LibraryName;
                }
            }
        }
    }


    /// <summary>
    /// If true, selected value is LibraryName, if false, selected value is LibraryID.
    /// </summary>
    public bool UseLibraryNameForSelection
    {
        get
        {
            return mUseLibraryNameForSelection;
        }
        set
        {
            mUseLibraryNameForSelection = value;
        }
    }

    
    /// <summary>
    /// Indicates whether the '(none)' option should be visible. 
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return CurrentSelector.AllowEmpty;
        }
        set
        {
            CurrentSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Indicates whether the '(none)' record should be added automatically when no library loaded.
    /// </summary>
    public bool NoneWhenEmpty
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether the '(all)' option should be visible. Doesn't work with AllowEmpty = true.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return CurrentSelector.AllowAll;
        }
        set
        {
            CurrentSelector.AllowAll = value;
        }

    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add current media library record to the DropDownList.
    /// </summary>
    public bool AddCurrentLibraryRecord
    {
        get
        {
            return GetValue("AddCurrentLibraryRecord", true);
        }
        set
        {
            SetValue("AddCurrentLibraryRecord", value);
        }
    }


    /// <summary>
    /// Indicates if auto post back should be used.
    /// </summary>
    public bool UseAutoPostBack
    {
        get
        {
            return InnerControl.AutoPostBack;
        }
        set
        {
            InnerControl.AutoPostBack = value;
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
            CurrentSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the DropDownList.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return InnerControl.ClientID;
        }
    }


    /// <summary>
    /// Inner control.
    /// </summary>
    public CMSDropDownList InnerControl
    {
        get
        {
            return CurrentSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets underlying form control
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets current selector control
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event handler
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData(false);
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        ReloadData(true);
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData(bool forceReload)
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ReturnColumnName = (UseLibraryNameForSelection ? "LibraryName" : "LibraryID");
        uniSelector.WhereCondition = GetCompleteWhereCondition();
        uniSelector.OnSelectionChanged += uniSelector_OnSelectionChanged;
        uniSelector.DropDownSingleSelect.AutoPostBack = UseAutoPostBack;

        bool noLibrary = MediaLibraryInfoProvider.GetMediaLibraries()
            .Where(GetGroupsWhereCondition())
            .Count == 0;

        // Empty value '(none)' is allowed when it is allowed from outside (property 'AllowEmpty') or no libraries was found and flag 'NoneWhenEmpty' is set
        uniSelector.AllowEmpty |= (noLibrary && NoneWhenEmpty);

        if (AddCurrentLibraryRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("media.current"), Value = MediaLibraryInfoProvider.CURRENT_LIBRARY });
        }
        
        if (forceReload)
        {
            uniSelector.Reload(true);
        }
    }


    /// <summary>
    /// OnSelectionChanged event handler
    /// </summary>
    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Let registered controls to know library selection changed
        if (SelectedLibraryChanged != null)
        {
            SelectedLibraryChanged();
        }
    }


    /// <summary>
    /// Builds where condition to filter libraries for group
    /// </summary>
    private string GetGroupsWhereCondition()
    {
        string where = "LibraryGroupID " + ((GroupID > 0) ? "=" + GroupID : "IS NULL");

        if (Where != "")
        {
            where = SqlHelper.AddWhereCondition(where, Where);
        }

        return where;
    }


    /// <summary>
    /// Builds complete where condition to filter libraries for site and group
    /// </summary>
    private string GetCompleteWhereCondition()
    {
        string where = GetGroupsWhereCondition();

        if (SiteID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "LibrarySiteID = " + SiteID);
        }
        else if (SiteID == 0)
        {
            where = SqlHelper.AddWhereCondition(where, "LibrarySiteID = " + SiteContext.CurrentSiteID);
        }
        else
        {
            where = SqlHelper.AddWhereCondition(where, SqlHelper.NO_DATA_WHERE);
        }

        return where;
    }

    #endregion
}