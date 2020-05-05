using System;
using System.Web;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_SelectCssStylesheet : FormEngineUserControl
{
    #region "Private variables"

    private int mSiteId;
    private bool mAllowEditButtons = true;

    #endregion


    #region "Public properties"

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
    /// Adds aliaspath parameter to edit url. Used for preview.
    /// </summary>
    public String AliasPath
    {
        get;
        set;
    }
    

    /// <summary>
    /// Indicates whether "(default)" record should be added to the dropdown list.
    /// </summary>
    public bool AddDefaultRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("adddefaultrecord"), true);
        }
        set
        {
            SetValue("adddefaultrecord", value);
        }
    }


    /// <summary>
    /// Gets the current UniSelector instance.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return usStyleSheet;
        }
    }


    /// <summary>
    /// If true edit buttons are shown.
    /// </summary>
    public bool AllowEditButtons
    {
        get
        {
            return mAllowEditButtons;
        }
        set
        {
            mAllowEditButtons = value;
        }
    }


    /// <summary>
    /// CSS stylesheet code name.
    /// </summary>
    public string StylesheetCodeName
    {
        get
        {
            return ValidationHelper.GetString(CurrentSelector.Value, String.Empty);
        }
        set
        {
            CurrentSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets the current drop down control.
    /// </summary>
    public CMSDropDownList CurrentDropDown
    {
        get
        {
            return CurrentSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets stylesheet name.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(CurrentSelector.Value, String.Empty);
        }
        set
        {
            CurrentSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the dropdown list with stylesheets.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return CurrentSelector.ClientID;
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
            CurrentSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the site id. If set, only stylesheets of the site are displayed.
    /// </summary>
    public int SiteId
    {
        get
        {
            if ((mSiteId == 0) && !String.IsNullOrEmpty(SiteName))
            {
                string siteName = SiteName.ToLowerCSafe();
                if (siteName == "##all##")
                {
                    mSiteId = -1;
                }
                else if (siteName == "##currentsite##")
                {
                    mSiteId = SiteContext.CurrentSiteID;
                }
                else
                {
                    // Get site id from site name if sets.
                    mSiteId = SiteInfoProvider.GetSiteID(siteName);
                }
            }
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name. If set, only stylesheets of the site are displayed.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Name of the column used for return value.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return CurrentSelector.ReturnColumnName;
        }
        set
        {
            CurrentSelector.ReturnColumnName = value;
        }
    }


    /// <summary>
    /// Sets a value indicating whether the New button should be enabled.
    /// </summary>
    public bool ButtonNewEnabled
    {
        set
        {
            CurrentSelector.ButtonNewEnabled = value;
        }
    }


    /// <summary>
    /// Sets a value indicating whether the Edit button should be enabled.
    /// </summary>
    public bool ButtonEditEnabled
    {
        set
        {
            CurrentSelector.ButtonEditEnabled = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Underlying form control.
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return usStyleSheet;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Add "(default)" if required
        CurrentSelector.AllowDefault = AddDefaultRecord;

        // If site specified, restrict to stylesheets assigned to the site
        if (SiteId > 0)
        {
            usStyleSheet.WhereCondition = SqlHelper.AddWhereCondition(usStyleSheet.WhereCondition, "StylesheetID IN (SELECT StylesheetID FROM CMS_CssStylesheetSite WHERE SiteID = " + SiteId + ")");
        }

        // Check if user can edit the stylesheet
        var currentUser = MembershipContext.AuthenticatedUser;
        bool uiElement = currentUser.IsAuthorizedPerUIElement("CMS.Content", new[] { "Properties", "Properties.General", "General.Design" }, SiteContext.CurrentSiteName);

        if (AllowEditButtons && uiElement && ReturnColumnName.EqualsCSafe("StylesheetID", true))
        {
            usStyleSheet.AdditionalDropDownCSSClass = "SelectorDropDown";
            usStyleSheet.ElementResourceName = "CMS.Design";
            usStyleSheet.EditItemElementName = "EditStylesheet";
            usStyleSheet.NewItemElementName = "newcssstylesheet";

            if (!String.IsNullOrEmpty(AliasPath))
            {
                usStyleSheet.AdditionalUrlParameters = "&aliaspath=" + HttpUtility.UrlEncode(AliasPath);
            }
        }
    }


    /// <summary>
    /// Reloads the selector's data.
    /// </summary>
    /// <param name="forceReload">Indicates whether data should be forcibly reloaded</param>
    public void Reload(bool forceReload)
    {
        usStyleSheet.Reload(forceReload);
    }


    /// <summary>
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usStyleSheet == null)
        {
            pnlUpdate.LoadContainer();
        }

        // Call base method
        base.CreateChildControls();
    }

    #endregion
}