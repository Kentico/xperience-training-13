using System;
using System.Web.UI;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_WebAnalytics_FormControls_SelectConversion : FormEngineUserControl
{
    #region "Variables"

    private bool mWasInit;
    private string mWhereCondition = String.Empty;
    private string mABTestName = String.Empty;
    private string mMVTestName = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// All record value for UniSelector
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return usConversions.AllRecordValue;
        }
        set
        {
            usConversions.AllRecordValue = value;
        }
    }

    /// <summary>
    /// Selection mode of control (dropdown,multiselect...).
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            return usConversions.SelectionMode;
        }
        set
        {
            usConversions.SelectionMode = value;
        }
    }


    /// <summary>
    /// If true change of dropdown raises PostBack.
    /// </summary>
    public bool PostbackOnDropDownChange
    {
        get
        {
            return ddlConversions.AutoPostBack;
        }
        set
        {
            ddlConversions.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Enables or disables UniSelector
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return usConversions.Enabled;
        }
        set
        {
            usConversions.Enabled = value;
        }
    }


    /// <summary>
    /// Value representing the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return usConversions.Value;
        }
        set
        {
            usConversions.Value = value;
        }
    }


    /// <summary>
    /// Where condition for selector.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// If true (all) is added to conversion selector.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return usConversions.AllowAll;
        }
        set
        {
            usConversions.AllowAll = value;
        }
    }


    /// <summary>
    /// For drop down mode - we select only conversions for one test.
    /// </summary>
    public string ABTestName
    {
        get
        {
            return mABTestName;
        }
        set
        {
            mABTestName = value;
        }
    }


    /// <summary>
    /// For drop down mode - we select only conversions for one test.
    /// </summary>
    public string MVTestName
    {
        get
        {
            return mMVTestName;
        }
        set
        {
            mMVTestName = value;
        }
    }


    /// <summary>
    /// Uniselector controls
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return usConversions;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        usConversions.IsLiveSite = IsLiveSite;
        usConversions.TextBoxSelect.MaxLength = 200;

        // Check user authorization
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageConversions") && (SelectionMode == SelectionModeEnum.SingleTextBox))
        {
            var url = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/edit.aspx?conversionName=##ITEMID##&modaldialog=true";
            usConversions.EditItemPageUrl = url;

            url = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/edit.aspx?modaldialog=true";
            usConversions.NewItemPageUrl = url;

            usConversions.EditDialogWindowWidth = 720;
            usConversions.EditDialogWindowHeight = 360;
        }

        // PostBack action
        if (PostbackOnDropDownChange)
        {
            usConversions.DropDownSingleSelect.AutoPostBack = true;
            var scr = ScriptManager.GetCurrent(Page);
            scr.RegisterPostBackControl(usConversions);
        }

        // Create where condition
        usConversions.WhereCondition = SqlHelper.AddWhereCondition(usConversions.WhereCondition, "ConversionSiteID = " + SiteContext.CurrentSiteID);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Reload data
            ReloadData(false);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Reloads data
    /// </summary>
    /// <param name="forceReload">Indicates whether apply force load</param>
    public void ReloadData(bool forceReload)
    {
        if (forceReload || !mWasInit)
        {
            // Reload data
            CreateWhereCondition();
            usConversions.Reload(forceReload);
            mWasInit = true;
        }
    }


    /// <summary>
    /// Creates where condition
    /// </summary>
    private void CreateWhereCondition()
    {
        usConversions.WhereCondition = SqlHelper.AddWhereCondition(usConversions.WhereCondition, WhereCondition);

        // Where condition for A/B testing
        if (ABTestName != String.Empty)
        {
            usConversions.WhereCondition = SqlHelper.AddWhereCondition(usConversions.WhereCondition, "ConversionName IN (SELECT StatisticsObjectName FROM Analytics_Statistics WHERE StatisticsSiteID = " + SiteContext.CurrentSiteID + " AND StatisticsCode LIKE (N'abconversion;" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(ABTestName)) + ";%'))");
        }

        // Where condition for MVT testing
        if (MVTestName != String.Empty)
        {
            usConversions.WhereCondition = SqlHelper.AddWhereCondition(usConversions.WhereCondition, "ConversionName IN (SELECT StatisticsObjectName FROM Analytics_Statistics WHERE StatisticsSiteID = " + SiteContext.CurrentSiteID + " AND StatisticsCode LIKE (N'mvtconversion;" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(MVTestName)) + ";%'))");
        }
    }


    /// <summary>
    /// Test if conversion is valid and create new conversion if not exists
    /// </summary>    
    public override bool IsValid()
    {
        // Get validated string
        var value = ValidationHelper.GetString(usConversions.Value, String.Empty).Trim();
        if (value != String.Empty)
        {
            var domain = RequestContext.CurrentDomain;
            if (DataHelper.GetNotEmpty(domain, "") != "")
            {
                var parsedDomain = LicenseKeyInfoProvider.ParseDomainName(domain);
                if (!LicenseKeyInfoProvider.IsFeatureAvailable(parsedDomain, FeatureEnum.CampaignAndConversions))
                {
                    ValidationError = GetString("conversionselector.nolicence");
                    return false;
                }
            }

            // Validate for code name
            if (!ValidationHelper.IsCodeName(value))
            {
                ValidationError = GetString("conversion.validcodename");
                return false;
            }

            // Test if selected name exists
            var ci = ConversionInfoProvider.GetConversionInfo(value, SiteContext.CurrentSiteName);

            // If not exist create new one
            if (ci == null)
            {
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageConversions"))
                {
                    ValidationError = GetString("conversion.notallowedcreate");
                    return false;
                }

                // Create new object
                ci = new ConversionInfo();
                ci.ConversionName = value;
                ci.ConversionDisplayName = value;
                ci.ConversionSiteID = SiteContext.CurrentSiteID;

                // Save to database
                ConversionInfoProvider.SetConversionInfo(ci);
            }
        }

        return true;
    }

    #endregion
}