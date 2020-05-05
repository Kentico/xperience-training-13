using System;

using CMS.Base;

using System.Linq;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_ABVariant_List : CMSAdminListControl
{
    #region "Variables"

    protected int nodeID;
    protected ABTestInfo mABTest;
    private TreeProvider mTreeProvider;

    #endregion


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
    /// Gets ab test on demand.
    /// </summary>
    private ABTestInfo ABTest
    {
        get
        {
            return mABTest ?? (mABTest = UIContext.EditedObject as ABTestInfo);
        }
    }


    /// <summary>
    /// Gets ab test id on demand.
    /// </summary>
    public int ABTestID
    {
        get
        {
            return (ABTest == null) ? 0 : ABTest.ABTestID;
        }
    }


    /// <summary>
    /// Gets tree provider object on demand.
    /// </summary>
    private TreeProvider TreeProvider
    {
        get
        {
            return mTreeProvider ?? (mTreeProvider = new TreeProvider());
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query parameter
        nodeID = QueryHelper.GetInteger("nodeID", 0);
        SetWhereCondition();
    }


    /// <summary>
    /// Handles Unigrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of the action</param>
    /// <param name="actionArgument">Action argument</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                int variantID = ValidationHelper.GetInteger(actionArgument, 0);
                if (variantID > 0)
                {
                    var variant = ABVariantInfo.Provider.Get(variantID);
                    if (variant != null)
                    {
                        if (variant.ABVariantPath != ABTest.ABTestOriginalPage)
                        {
                            ABVariantInfo.Provider.Delete(ABVariantInfo.Provider.Get(variantID));
                        }
                        else
                        {
                            ShowError(GetString("abtesting.deleteoriginalvariant"));
                        }
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Sets where condition.
    /// </summary>
    private void SetWhereCondition()
    {
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "ABVariantTestID =" + ABTestID);
        gridElem.QueryParameters = new QueryDataParameters();
        gridElem.QueryParameters.Add("TestName", (ABTest != null ? ABTest.ABTestName : ""));
    }


    /// <summary>
    /// Handles Unigrid's ExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "path":
                var encodedParam = HTMLHelper.HTMLEncode(parameter.ToString());
                string result = UIHelper.GetAccessibleIconTag("icon-exclamation-triangle", GetString("abtesting.variantculturewarning"), FontIconSizeEnum.NotDefined, "warning-icon") + "&nbsp;" + encodedParam;

                if (ABTest != null)
                {
                    // If AB test is for specified culture
                    if (!string.IsNullOrEmpty(ABTest.ABTestCulture))
                    {
                        // Try to find document in required culture
                        if (TreeProvider.SelectSingleNode(SiteContext.CurrentSiteName, parameter.ToString(), ABTest.ABTestCulture, false, null, false) != null)
                        {
                            return parameter;
                        }
                    }
                    else
                    {
                        // For all cultures, compare number of site cultures and number of variant translations
                        var siteCultures = CultureSiteInfoProvider.GetSiteCultures(SiteContext.CurrentSiteName).Items.Count;
                        var nodeCultures = DocumentHelper.GetDocuments()
                                                           .PublishedVersion()
                                                           .All()
                                                           .OnCurrentSite()
                                                           .AllCultures()
                                                           .Column("DocumentCulture")
                                                           .WhereEquals("NodeAliasPath", parameter.ToString())
                                                           .Count();

                        return (siteCultures != nodeCultures) ? result : encodedParam;
                    }
                }
                return result;
        }
        return parameter;
    }

    #endregion
}