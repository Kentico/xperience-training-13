using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_FormControls_ForumGroupSelector : FormEngineUserControl
{
    #region "Private variables"

    private int mSiteId = 0;

    #endregion


    #region "Properties"

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
    /// Gets or sets field value.
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
    /// Gets or sets the value which determines whether to allow more than one user to select.
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
            base.IsLiveSite = value;
            UniSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the current uniselector instance.
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
    /// Gets the single select drop down field.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return UniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets the site id of site which forum groups will be displayed.
    /// </summary>
    public int SiteId
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

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set uniselector
        uniSelector.DisplayNameFormat = "{%GroupDisplayName%}";
        if (String.IsNullOrEmpty(uniSelector.ReturnColumnName))
        {
            uniSelector.ReturnColumnName = "GroupName";
        }
        uniSelector.AllowEmpty = false;
        uniSelector.AllowAll = false;

        // Set resource prefix based on mode
        if ((SelectionMode == SelectionModeEnum.Multiple) || (SelectionMode == SelectionModeEnum.MultipleButton) || (SelectionMode == SelectionModeEnum.MultipleTextBox))
        {
            uniSelector.ResourcePrefix = "forumgroupssel";
        }
        else
        {
            uniSelector.ResourcePrefix = "forumgroupsel";
        }

        SetupWhereCondition();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (SiteId < 0)
        {
            Enabled = false;
        }

        if (RequestHelper.IsPostBack() && DependsOnAnotherField)
        {
            SetupWhereCondition();
            uniSelector.Reload(true);
            pnlUpdate.Update();
        }
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Reloads the selector.
    /// </summary>
    /// <param name="forceLoad">Indicates whether data should be always loaded</param>
    public void Reload(bool forceLoad)
    {
        uniSelector.Reload(forceLoad);
    }


    private void SetupWhereCondition()
    {
        SetFormSiteId();

        // If SiteId not ste use current site
        int siteId = SiteId;
        if (siteId == 0)
        {
            siteId = SiteContext.CurrentSiteID;
        }

        if (siteId < 0)
        {
            uniSelector.Value = "";
        }

        // Set where condition - do not show groups forumgroups and adhoc forumsgroups
        uniSelector.WhereCondition = "GroupSiteID = " + siteId + " AND GroupGroupID IS NULL AND GroupName NOT LIKE 'AdHoc%'";
    }


    /// <summary>
    /// Sets the SiteId if the SiteName field is available in the form.
    /// </summary>
    private void SetFormSiteId()
    {
        if (DependsOnAnotherField && (Form != null) && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), null);
            if (!String.IsNullOrEmpty(siteName))
            {
                SiteInfo siteObj = SiteInfoProvider.GetSiteInfo(siteName);
                if (siteObj != null)
                {
                    SiteId = siteObj.SiteID;
                }
            }
            else
            {
                if (SiteId > 0)
                {
                    SiteId = 0;
                }
            }
        }
    }

    #endregion
}