using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_OnlineMarketing_Controls_UI_AbTest_List : CMSAdminListControl
{    
    private readonly string permissionsRequiredTooltip = ResHelper.GetString("abtesting.permission.manage.tooltip");

    private bool IsAuthorizedToManage { get; } = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ModuleName.ABTEST, PERMISSION_MANAGE);

    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Alias path of document to which this abtest belongs.
    /// </summary>
    public string AliasPath
    {
        get;
        set;
    } = String.Empty;


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
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
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// NodeID.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the column with original page should be displayed.
    /// </summary>
    public bool ShowOriginalPageColumn
    {
        get;
        set;
    } = true;

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();

        // Set nice 'No data' message (message differs based on whether the user is in content tree or on on-line marketing tab
        gridElem.ZeroRowsText = GetString(NodeID > 0 ? "abtesting.abtest.nodataondocument" : "abtesting.abtest.nodata");
        
        string url = UIContextHelper.GetElementUrl("CMS.ABTest", "Detail", gridElem.EditInDialog);

        url = URLHelper.AddParameterToUrl(url, "objectid", "{0}");
        url = URLHelper.AddParameterToUrl(url, "aliasPath", AliasPath);

        if (NodeID > 0)
        {
            url = URLHelper.AddParameterToUrl(url, "NodeID", NodeID.ToString());
        }

        gridElem.EditActionUrl = url;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!ShowOriginalPageColumn)
        {
            HideOriginalPageColumn();
        }
    }


    /// <summary>
    /// Handles Unigrid's OnAction event.
    /// </summary>
    protected void gridElem_OnOnAction(string actionname, object actionargument)
    {
        string argument = actionargument.ToString();

        switch (actionname)
        {
            case "delete":
                int testId = ValidationHelper.GetInteger(argument, 0);
                if (testId > 0)
                {
                    ABTestInfo.Provider.Delete(ABTestInfo.Provider.Get(testId));
                    LoadData();
                }
                break;
        }
    }


    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "status":
            {
                if (Enum.TryParse(parameter.ToString(), out ABTestStatusEnum status))
                {
                    return ABTestStatusEvaluator.GetFormattedStatus(status);
                }
                break;
            }

            case "page":
            {
                var dataRowView = parameter as DataRowView;
                if (dataRowView == null)
                {
                    return String.Empty;
                }

                var abTestInfo = new ABTestInfo(dataRowView.Row);

                var selectionParameters = new NodeSelectionParameters
                {
                    AliasPath = abTestInfo.ABTestOriginalPage,
                    CultureCode = abTestInfo.ABTestCulture,
                    SiteName = SiteContext.CurrentSiteName,
                    SelectOnlyPublished = true,
                    CombineWithDefaultCulture = false
                };

                var node = new TreeProvider().SelectSingleNode(selectionParameters);
                var encodedTestPage = HTMLHelper.HTMLEncode(abTestInfo.ABTestOriginalPage);

                return node == null
                    ? (object)encodedTestPage
                    : new HyperLink
                    {
                        NavigateUrl = DocumentURLProvider.GetAbsoluteUrl(node),
                        Text = encodedTestPage,
                        Target = "_blank"
                    };
            }

            case "visitors":
            case "conversions":
            {
                string statisticsCodeName = (sourceName.ToLowerInvariant() == "visitors" ? "abvisitfirst" : "absessionconversionfirst");

                var dataRowView = parameter as DataRowView;
                if (dataRowView == null)
                {
                    return 0;
                }

                var abTestInfo = new ABTestInfo(dataRowView.Row);

                return ValidationHelper.GetInteger(HitsInfoProvider.GetAllHitsInfo(SiteContext.CurrentSiteID, HitsIntervalEnum.Year, statisticsCodeName + ";" + abTestInfo.ABTestName + ";%", "SUM(HitsCount)", abTestInfo.ABTestCulture).Tables[0].Rows[0][0], 0);
            }
            
            case "delete":
                CMSGridActionButton btn = (CMSGridActionButton)sender;
                btn.Enabled = IsAuthorizedToManage;
                if(!IsAuthorizedToManage)
                {
                    btn.ToolTip = permissionsRequiredTooltip;
                }
                break;
        }

        return null;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads data into Unigrid.
    /// </summary>
    private void LoadData()
    {
        var whereCondition = new WhereCondition();
        if (!String.IsNullOrEmpty(AliasPath))
        {
            whereCondition.WhereEquals("ABTestOriginalPage", AliasPath);
        }

        DataSet abTests = ABTestInfo.Provider.Get().OnSite(SiteContext.CurrentSiteID).Where(whereCondition);
        abTests.Tables[0].Columns.Add("ABTestStatus", typeof(int));

        foreach (DataRow abTestDataRow in abTests.Tables[0].Rows)
        {
            abTestDataRow["ABTestStatus"] = (int)ABTestStatusEvaluator.GetStatus(new ABTestInfo(abTestDataRow));
        }

        gridElem.DataSource = abTests;
    }


    /// <summary>
    /// Hides original page grid column.
    /// </summary>
    private void HideOriginalPageColumn()
    {
        if (gridElem.NamedColumns.TryGetValue("ABTestOriginalPage", out DataControlField originalPage))
        {
            originalPage.Visible = false;
        }
    }

    #endregion
}