using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Chat_FormControls_RoomSelector : FormEngineUserControl
{
    #region "Variables"

    private int mSiteId = 0;

    private string mResourcePrefix = String.Empty;
    private bool mHideDisabledRooms = true;
    private bool mDisplayRoomsFromAllSites = false;
    private bool mAllowAll = false;
    private bool mAllowEmpty = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets ID of the site. User of this site are displayed
    /// Use 0 for current site, -1 for all sites(no filter)
    /// Note: SiteID is not used if site filter is enabled
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to allow more than one user to select.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            EnsureChildControls();
            return usRooms.SelectionMode;
        }
        set
        {
            EnsureChildControls();
            usRooms.SelectionMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the text displayed if there are no data.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            EnsureChildControls();
            return usRooms.ZeroRowsText;
        }
        set
        {
            EnsureChildControls();
            usRooms.ZeroRowsText = value;
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
            EnsureChildControls();

            base.Enabled = value;
            usRooms.Enabled = value;
        }
    }


    ///<summary>
    /// Gets or sets field value.
    ///</summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usRooms.Value;
        }
        set
        {
            EnsureChildControls();
            usRooms.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteFilter"), true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return usRooms;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the UniSelector should apply WhereCondition for the selected value (default: true). This does not affect the modal dialog.
    /// </summary>
    public bool ApplyValueRestrictions
    {
        get
        {
            EnsureChildControls();
            return usRooms.ApplyValueRestrictions;
        }
        set
        {
            EnsureChildControls();
            usRooms.ApplyValueRestrictions = value;
        }
    }


    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return usRooms.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets the resource prefix of uniselector. If not set default values are used.
    /// </summary>
    public override string ResourcePrefix
    {
        get
        {
            return mResourcePrefix;
        }
        set
        {
            mResourcePrefix = value;
            usRooms.ResourcePrefix = value;
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
            EnsureChildControls();
            base.IsLiveSite = value;
            usRooms.IsLiveSite = value;
        }
    }


    /// <summary>
    /// If enabled disabled users aren't shown in selector.
    /// </summary>
    public bool HideDisabledRooms
    {
        get
        {
            return mHideDisabledRooms;
        }
        set
        {
            mHideDisabledRooms = value;
        }
    }


    /// <summary>
    /// If true selector will always display users from all sites.
    /// Suited for selecting users having access to the site.
    /// </summary>
    public bool DisplayRoomsFromAllSites
    {
        get
        {
            return mDisplayRoomsFromAllSites;
        }
        set
        {
            mDisplayRoomsFromAllSites = value;
        }
    }


    /// <summary>
    /// Enables or disables (all) item in selector.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return mAllowAll;
        }
        set
        {
            mAllowAll = value;
            EnsureChildControls();
            usRooms.AllowAll = value;
        }
    }


    /// <summary>
    /// Enables or disables (empty) item in selector.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return mAllowEmpty;
        }
        set
        {
            mAllowEmpty = value;
            EnsureChildControls();
            usRooms.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Additional users to show identified by user IDs.
    /// </summary>
    public int[] AdditionalRooms
    {
        get
        {
            return (int[])GetValue("AdditionalRooms");
        }
        set
        {
            SetValue("AdditionalRooms", value);
        }
    }


    /// <summary>
    /// Where condition used to filter control data.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }

    #endregion


    #region "Control methods"

    protected override void OnInit(EventArgs e)
    {
        usRooms.UniGrid.Pager.UniPager.PageSize = 10;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
            ReloadData();
        }
        else
        {
            usRooms.StopProcessing = true;
        }
    }

    protected void SetupControls()
    {
        //Display only current site's rooms
        usRooms.WhereCondition = SqlHelper.AddWhereCondition(usRooms.WhereCondition, string.Format("((ChatRoomSiteID = {0}) OR ChatRoomSiteID IS NULL) AND (ISNULL(ChatRoomPassword, '') = '' ) AND (ChatRoomIsOneToOne = 0) AND (ChatRoomPrivate = 0)", (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID));

        // Set prefix if not set
        if (ResourcePrefix == String.Empty)
        {
            // Set resource prefix based on mode
            if ((SelectionMode == SelectionModeEnum.Multiple) || (SelectionMode == SelectionModeEnum.MultipleButton) || (SelectionMode == SelectionModeEnum.MultipleTextBox))
            {
                usRooms.ResourcePrefix = "selectrooms";
            }
            else
            {
                usRooms.ResourcePrefix = "selectroom";
            }
        }

        // Hide disabled rooms
        if (HideDisabledRooms)
        {
            const string where = "ChatRoomEnabled = 1";
            usRooms.WhereCondition = SqlHelper.AddWhereCondition(usRooms.WhereCondition, where);
        }

        // Add additional rooms
        if ((AdditionalRooms != null) && (AdditionalRooms.Length > 0))
        {
            usRooms.WhereCondition = SqlHelper.AddWhereCondition(usRooms.WhereCondition, SqlHelper.GetWhereCondition("ChatRoomID", AdditionalRooms.AsEnumerable()), "OR");
        }

        // Control where condition
        if (!String.IsNullOrEmpty(WhereCondition))
        {
            usRooms.WhereCondition = SqlHelper.AddWhereCondition(usRooms.WhereCondition, WhereCondition);
        }

        pnlUpdate.ShowProgress = (this.SelectionMode == SelectionModeEnum.Multiple);
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usRooms == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Reloads the data of the UniSelector.
    /// </summary>
    public void ReloadData()
    {
        usRooms.Reload(true);
        pnlUpdate.Update();
    }


    /// <summary>
    /// Reloads whole control including control settings and data.
    /// </summary>
    public void Reload()
    {
        SetupControls();
        ReloadData();
    }

    #endregion
}