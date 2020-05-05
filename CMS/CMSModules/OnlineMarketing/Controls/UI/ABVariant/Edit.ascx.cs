using System;
using System.Linq;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_UI_ABVariant_Edit : CMSAdminEditControl
{
    #region "Variables"

    private ABTestInfo mABTest;

    #endregion


    #region "Properties"

    /// <summary>
    /// Strongly typed EditedObject.
    /// </summary>
    private ABVariantInfo ABVariant
    {
        get
        {
            return form.EditedObject as ABVariantInfo;
        }
    }


    /// <summary>
    /// ABVariant's ABTest.
    /// </summary>
    private ABTestInfo ABTest
    {
        get
        {
            return mABTest ?? (mABTest = ABTestInfo.Provider.Get(QueryHelper.GetInteger("abTestID", 0)));
        }
    }

    #endregion


    #region "Events"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check whether parameters are valid
        if (ABVariant == null)
        {
            return;
        }
        if (ABTest == null)
        {
            return;
        }

        // Disable variant path editing for original variant
        if ((ABVariant.ABVariantPath == ABTest.ABTestOriginalPage) && (ABVariant.ABVariantID > 0))
        {
            form.FieldControls["ABVariantPath"].Enabled = false;
        }
    }
           

    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        if (ABTest == null)
        {
            ShowError(GetString("abtest.error"));
            form.StopProcessing = true;
            return;
        }

        ABVariant.ABVariantSiteID = CurrentSite.SiteID;

        string filledVariantPath = ABVariant.ABVariantPath;

        // Check if filled page exists
        CheckPageExistence(filledVariantPath);

        // Check if page is not already assigned
        if (!form.StopProcessing)
        {
            CheckDoublePageAssignment(filledVariantPath);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Checks if page is not already assigned.
    /// </summary>
    /// <param name="filledVariantPath">Variant path filled in form</param>
    private void CheckDoublePageAssignment(string filledVariantPath)
    {
        string originalVariantPath = ValidationHelper.GetString(ABVariant.GetOriginalValue("ABVariantPath"), "");

        //Check if variantPath is changed
        if (originalVariantPath != filledVariantPath)
        {
            var condition = new WhereCondition()
                .WhereEquals("ABVariantTestID", ABTest.ABTestID)
                .WhereEquals("ABVariantPath", filledVariantPath);

            if (ABVariant.ABVariantID > 0)
            {
                condition.Where("ABVariantID", QueryOperator.NotEquals, ABVariant.ABVariantID);
            }

            var variants = ABVariantInfo.Provider.Get().Where(condition).TopN(1);
            if (variants.Any())
            {
                ShowError(GetString("abtesting.variantpath.alreadyassigned"));
                form.StopProcessing = true;
            }
        }
    }


    /// <summary>
    /// Checks if the page specified by the variant path exists.
    /// </summary>
    /// <param name="filledVariantPath">Variant path filled in form</param>
    private void CheckPageExistence(string filledVariantPath)
    {
        bool pageExists = DocumentHelper.GetDocuments()
                                          .PublishedVersion()
                                          .TopN(1)
                                          .All()
                                          .OnCurrentSite()
                                          .Culture(ABTest.ABTestCulture)
                                          .CombineWithDefaultCulture(false)
                                          .Columns("NodeID")
                                          .WhereEquals("NodeAliasPath", filledVariantPath)
                                          .Any();

        if (!pageExists)
        {
            ShowError(GetString("abtesting.variantpath.pagenotfound"));
            form.StopProcessing = true;
        }
    }

    #endregion
}