using System;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_cmsuniview : CMSAbstractWebPart
{
    #region "Document properties"

    protected CMSDocumentsDataSource mDataSourceControl = null;

    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            uniView.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, uniView.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            uniView.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            uniView.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), uniView.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            uniView.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Classnames"), uniView.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            uniView.ClassNames = value;
        }
    }


    /// <summary>
    /// Code name of the category to display documents from.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryName"), uniView.CategoryName);
        }
        set
        {
            SetValue("CategoryName", value);
            uniView.CategoryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), uniView.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            uniView.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), uniView.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            uniView.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), uniView.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            uniView.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), uniView.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            uniView.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OrderBy"), uniView.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            uniView.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), null);
        }
        set
        {
            SetValue("Path", value);
            uniView.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), uniView.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            uniView.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), uniView.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            uniView.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), uniView.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            uniView.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), uniView.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            uniView.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), uniView.Columns);
        }
        set
        {
            SetValue("Columns", value);
            uniView.Columns = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Load pages individually.
    /// </summary>
    public bool LoadPagesIndividually
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LoadPagesIndividually"), true);
        }
        set
        {
            SetValue("LoadPagesIndividually", value);
            uniView.LoadPagesIndividually = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether scroll position should be cleared after post back paging
    /// </summary>
    public bool ResetScrollPositionOnPostBack
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ResetScrollPositionOnPostBack"), uniView.ResetScrollPositionOnPostBack);
        }
        set
        {
            SetValue("ResetScrollPositionOnPostBack", value);
            uniView.ResetScrollPositionOnPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), false);
        }
        set
        {
            SetValue("EnablePaging", value);
            uniView.EnablePaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return BasicDataPager.StringToPagingPlaceTypeEnum(DataHelper.GetNotEmpty(GetValue("PagerPosition"), uniView.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            uniView.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the documents displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), uniView.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            uniView.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("QueryStringKey"), uniView.PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            uniView.PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the paging mode.
    /// </summary>
    public UniPagerMode PagerMode
    {
        get
        {
            string strMode = ValidationHelper.GetString(GetValue("PagerMode"), String.Empty).ToLowerCSafe();
            switch (strMode)
            {
                case "postback":
                    return UniPagerMode.PostBack;

                default:
                    return UniPagerMode.Querystring;
            }
        }
        set
        {
            SetValue("PagerMode", value.ToString());
            uniView.PagerControl.PagerMode = value;
        }
    }


    ///// <summary>
    ///// Gets or sets the results position
    ///// </summary>
    //public ResultsLocationTypeEnum ResultsPosition
    //{
    //    get
    //    {
    //        return uniView.PagerControl.GetResultPosition(ValidationHelper.GetString(GetValue("ResultsPosition"), ""));
    //    }
          //    set
    //    {
    //        SetValue("ResultsPosition", value);
    //        uniView.PagerControl.ResultsLocation = value;
    //    }
          //}


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayFirstLastAutomatically"), uniView.PagerControl.DisplayFirstLastAutomatically);
        }
        set
        {
            SetValue("DisplayFirstLastAutomatically", value);
            uniView.PagerControl.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPreviousNextAutomatically"), uniView.PagerControl.DisplayPreviousNextAutomatically);
        }
        set
        {
            SetValue("DisplayPreviousNextAutomatically", value);
            uniView.PagerControl.DisplayPreviousNextAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("GroupSize"), uniView.PagerControl.GroupSize);
        }
        set
        {
            SetValue("GroupSize", value);
            uniView.PagerControl.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HidePagerForSinglePage"), uniView.PagerControl.HidePagerForSinglePage);
        }
        set
        {
            SetValue("HidePagerForSinglePage", value);
            uniView.PagerControl.HidePagerForSinglePage = value;
        }
    }

    #endregion


    #region "Relationships properties"

    /// <summary>
    /// Gets or sets the value that indicates whether related node is on the left side.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), uniView.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            uniView.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("RelationshipName"), uniView.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            uniView.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), uniView.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            uniView.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the hierarchical transforamtion which is used for displaying the results.
    /// </summary>
    public string HierarchicalTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("HierarchicalTransformationName"), uniView.HierarchicalTransformationName);
        }
        set
        {
            SetValue("HierarchicalTransformationName", value);
            uniView.HierarchicalTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("TransformationName"), uniView.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            uniView.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AlternatingTransformationName"), uniView.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            uniView.AlternatingTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the header transforamtion which is used for displaying the results.
    /// </summary>
    public string HeaderTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("HeaderTransformationName"), uniView.HeaderTransformationName);
        }
        set
        {
            SetValue("HeaderTransformationName", value);
            uniView.HeaderTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the footer transforamtion which is used for displaying the results.
    /// </summary>
    public string FooterTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FooterTransformationName"), uniView.FooterTransformationName);
        }
        set
        {
            SetValue("FooterTransformationName", value);
            uniView.FooterTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the first transforamtion which is used for displaying the results.
    /// </summary>
    public string FirstTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FirstTransformationName"), uniView.FirstTransformationName);
        }
        set
        {
            SetValue("FirstTransformationName", value);
            uniView.FirstTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the last transforamtion which is used for displaying the results.
    /// </summary>
    public string LastTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LastTransformationName"), uniView.LastTransformationName);
        }
        set
        {
            SetValue("LastTransformationName", value);
            uniView.LastTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the single transforamtion which is used for displaying the results.
    /// </summary>
    public string SeparatorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SeparatorTransformationName"), uniView.SeparatorTransformationName);
        }
        set
        {
            SetValue("SeparatorTransformationName", value);
            uniView.SeparatorTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the single transforamtion which is used for displaying the results.
    /// </summary>
    public string SingleTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SingleTransformationName"), uniView.SingleTransformationName);
        }
        set
        {
            SetValue("SingleTransformationName", value);
            uniView.SingleTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedItemTransformationName"), uniView.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            uniView.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the header transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedHeaderItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedHeaderItemTransformationName"), uniView.SelectedHeaderItemTransformationName);
        }
        set
        {
            SetValue("SelectedHeaderItemTransformationName", value);
            uniView.SelectedHeaderItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the footer transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedFooterItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SelectedFooterItemTransformationName"), uniView.SelectedFootertemTransformationName);
        }
        set
        {
            SetValue("SelectedFooterItemTransformationName", value);
            uniView.SelectedFootertemTransformationName = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the nested controls IDs. Use ';' like separator.
    /// </summary>
    public string NestedControlsID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NestedControlsID"), uniView.NestedControlsID);
        }
        set
        {
            SetValue("NestedControlsID", value);
            uniView.NestedControlsID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), uniView.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            uniView.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ZeroRowsText"), uniView.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            uniView.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator (tetx, html code) which is displayed between displayed items.
    /// </summary>
    public string ItemSeparator
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ItemSeparator"), uniView.ItemSeparatorValue);
        }
        set
        {
            SetValue("ItemSeparator", value);
            uniView.ItemSeparatorValue = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), uniView.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            uniView.FilterName = value;
        }
    }


    /// <summary>
    /// Data source name.
    /// </summary>
    public string DataSourceName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DataSourceName"), uniView.DataSourceName);
        }
        set
        {
            SetValue("DataSourceName", value);
            uniView.DataSourceName = value;
        }
    }


    /// <summary>
    /// Control with data source.
    /// </summary>
    public CMSDocumentsDataSource DataSourceControl
    {
        get
        {
            return mDataSourceControl;
        }
        set
        {
            mDataSourceControl = value;
            uniView.DataSourceControl = value;
        }
    }


    /// <summary>
    /// Repeater control.
    /// </summary>
    public CMSUniView RepeaterControl
    {
        get
        {
            return uniView;
        }
    }

    #endregion


    #region "Editing mode buttons properties"

    /// <summary>
    /// Gets or sets the value that indicates whether 'New button' should be displayed.
    /// </summary>
    public bool ShowNewButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowNewButton"), btnAdd.Visible);
        }
        set
        {
            SetValue("ShowNewButton", value);
            btnAdd.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the new button text.
    /// </summary>
    public string NewButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NewButtonText"), btnAdd.Text);
        }
        set
        {
            SetValue("NewButtonText", value);
            btnAdd.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether edit and delete buttons should be displayed.
    /// </summary>
    public bool ShowEditDeleteButtons
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditDeleteButtons"), uniView.ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowEditDeleteButtons", value);
            uniView.ShowEditDeleteButtons = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether delete buttons should be displayed.
    /// </summary>
    public bool ShowDeleteButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowDeleteButton"), ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowDeleteButton", value);
            uniView.ShowDeleteButton = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether edit buttons should be displayed.
    /// </summary>
    public bool ShowEditButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEditButton"), ShowEditDeleteButtons);
        }
        set
        {
            SetValue("ShowEditButton", value);
            uniView.ShowEditButton = value;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
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
            uniView.StopProcessing = value;
            btnAdd.StopProcessing = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Pages"), String.Empty);
        }
        set
        {
            SetValue("Pages", value);
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CurrentPage"), String.Empty);
        }
        set
        {
            SetValue("CurrentPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageSeparator"), String.Empty);
        }
        set
        {
            SetValue("PageSeparator", value);
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstPage"), String.Empty);
        }
        set
        {
            SetValue("FirstPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LastPage"), String.Empty);
        }
        set
        {
            SetValue("LastPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousPage"), String.Empty);
        }
        set
        {
            SetValue("PreviousPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextPage"), String.Empty);
        }
        set
        {
            SetValue("NextPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PreviousGroup"), String.Empty);
        }
        set
        {
            SetValue("PreviousGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("NextGroup"), String.Empty);
        }
        set
        {
            SetValue("NextGroup", value);
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerLayout"), String.Empty);
        }
        set
        {
            SetValue("PagerLayout", value);
        }
    }


    /// <summary>
    /// Gets or sets the direct page template.
    /// </summary>
    public string DirectPageTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DirectPage"), String.Empty);
        }
        set
        {
            SetValue("DirectPage", value);
        }
    }

    #endregion


    #region "CMSUniView properties"

    /// <summary>
    /// Gets or sets the value that indicates whether the data should be loaded to the load event instead of default init event.
    /// </summary>
    public bool DelayedLoading
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DelayedLoading"), uniView.DelayedLoading);
        }
        set
        {
            SetValue("DelayedLoading", value);
            uniView.DelayedLoading = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indictes whether data should be binded in default format
    /// or changet to hierarchical grouped dataset
    /// </summary>
    public bool LoadHierarchicalData
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LoadHierarchicalData"), uniView.LoadHierarchicalData);
        }
        set
        {
            SetValue("LoadHierarchicalData", value);
            uniView.LoadHierarchicalData = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether default hierarchical order value should be used.
    /// The order is used only if LoadHierarchicalData is set to true.
    /// Default order value is "NodeLevel, NodeOrder". Value of OrderBy property is joined at the end of the order by expression
    /// </summary>
    public bool UseHierarchicalOrder
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseHierarchicalOrder"), uniView.UseHierarchicalOrder);
        }
        set
        {
            SetValue("UseHierarchicalOrder", value);
            uniView.UseHierarchicalOrder = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether hierarchical data should be displayed in inner or separate mode.
    /// </summary>
    public HierarchicalDisplayModeEnum HierarchicalDisplayMode
    {
        get
        {
            string displayMode = ValidationHelper.GetString(GetValue("HierarchicalDisplayMode"), "inner").ToLowerCSafe();
            switch (displayMode)
            {
                case "separate":
                    return HierarchicalDisplayModeEnum.Separate;

                default:
                    return HierarchicalDisplayModeEnum.Inner;
            }
        }
        set
        {
            switch (value)
            {
                case HierarchicalDisplayModeEnum.Inner:
                    SetValue("HierarchicalDisplayMode", "inner");
                    break;

                case HierarchicalDisplayModeEnum.Separate:
                    SetValue("HierarchicalDisplayMode", "separate");
                    break;
            }

            uniView.HierarchicalDisplayMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether header and footer items should be hidden if single item is displayed.
    /// </summary>
    public bool HideHeaderAndFooterForSingleItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideHeaderAndFooterForSingleItem"), uniView.HideHeaderAndFooterForSingleItem);
        }
        set
        {
            SetValue("HideHeaderAndFooterForSingleItem", value);
            uniView.HideHeaderAndFooterForSingleItem = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            uniView.StopProcessing = true;
        }
        else
        {
            uniView.ControlContext = ControlContext;

            // Document properties
            uniView.NestedControlsID = NestedControlsID;
            uniView.CacheItemName = CacheItemName;
            uniView.CacheDependencies = CacheDependencies;
            uniView.CacheMinutes = CacheMinutes;
            uniView.CheckPermissions = CheckPermissions;
            uniView.ClassNames = ClassNames;
            uniView.CategoryName = CategoryName;
            uniView.CombineWithDefaultCulture = CombineWithDefaultCulture;
            uniView.CultureCode = CultureCode;
            uniView.MaxRelativeLevel = MaxRelativeLevel;
            uniView.OrderBy = OrderBy;
            uniView.SelectTopN = SelectTopN;
            uniView.Columns = Columns;
            uniView.SelectOnlyPublished = SelectOnlyPublished;
            uniView.FilterOutDuplicates = FilterOutDuplicates;
            uniView.Path = Path;

            uniView.SiteName = SiteName;
            uniView.WhereCondition = WhereCondition;

            // CMSUniView properties
            uniView.LoadHierarchicalData = LoadHierarchicalData;
            uniView.UseHierarchicalOrder = UseHierarchicalOrder;
            uniView.HideHeaderAndFooterForSingleItem = HideHeaderAndFooterForSingleItem;
            uniView.HierarchicalDisplayMode = HierarchicalDisplayMode;
            uniView.DelayedLoading = DelayedLoading;

            // Data source settings - must be before pager settings
            uniView.DataSourceName = DataSourceName;
            uniView.DataSourceControl = DataSourceControl;

            // Pager
            uniView.LoadPagesIndividually = LoadPagesIndividually;
            uniView.EnablePaging = EnablePaging;
            uniView.PageSize = PageSize;
            uniView.PagerControl.QueryStringKey = QueryStringKey;
            uniView.PagerControl.PagerMode = PagerMode;
            uniView.PagerPosition = PagerPosition;
            uniView.PagerControl.HidePagerForSinglePage = HidePagerForSinglePage;
            uniView.PagerControl.GroupSize = GroupSize;
            uniView.PagerControl.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
            uniView.PagerControl.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
            uniView.PagerControl.ResetScrollPositionOnPostBack = ResetScrollPositionOnPostBack;

            // Pager transformations


            #region "UniPager template properties"

            // UniPager template properties
            if (!String.IsNullOrEmpty(PagesTemplate))
            {
                uniView.PagerControl.PageNumbersTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, PagesTemplate);
            }

            if (!String.IsNullOrEmpty(CurrentPageTemplate))
            {
                uniView.PagerControl.CurrentPageTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, CurrentPageTemplate);
            }

            if (!String.IsNullOrEmpty(SeparatorTemplate))
            {
                uniView.PagerControl.PageNumbersSeparatorTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, SeparatorTemplate);
            }

            if (!String.IsNullOrEmpty(FirstPageTemplate))
            {
                uniView.PagerControl.FirstPageTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, FirstPageTemplate);
            }

            if (!String.IsNullOrEmpty(LastPageTemplate))
            {
                uniView.PagerControl.LastPageTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, LastPageTemplate);
            }

            if (!String.IsNullOrEmpty(PreviousPageTemplate))
            {
                uniView.PagerControl.PreviousPageTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, PreviousPageTemplate);
            }

            if (!String.IsNullOrEmpty(NextPageTemplate))
            {
                uniView.PagerControl.NextPageTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, NextPageTemplate);
            }

            if (!String.IsNullOrEmpty(PreviousGroupTemplate))
            {
                uniView.PagerControl.PreviousGroupTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, PreviousGroupTemplate);
            }

            if (!String.IsNullOrEmpty(NextGroupTemplate))
            {
                uniView.PagerControl.NextGroupTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, NextGroupTemplate);
            }

            if (!String.IsNullOrEmpty(DirectPageTemplate))
            {
                uniView.PagerControl.DirectPageTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, DirectPageTemplate);
            }

            if (!String.IsNullOrEmpty(LayoutTemplate))
            {
                uniView.PagerControl.LayoutTemplate = TransformationHelper.LoadTransformation(uniView.PagerControl, LayoutTemplate);
            }

            #endregion


            // Relationships
            uniView.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
            uniView.RelationshipName = RelationshipName;
            uniView.RelationshipWithNodeGuid = RelationshipWithNodeGUID;

            // Transformation properties
            uniView.TransformationName = TransformationName;
            uniView.HierarchicalTransformationName = HierarchicalTransformationName;
            uniView.AlternatingTransformationName = AlternatingTransformationName;
            uniView.FooterTransformationName = FooterTransformationName;
            uniView.HeaderTransformationName = HeaderTransformationName;
            uniView.FirstTransformationName = FirstTransformationName;
            uniView.LastTransformationName = LastTransformationName;
            uniView.SingleTransformationName = SingleTransformationName;
            uniView.SeparatorTransformationName = SeparatorTransformationName;
            uniView.SelectedItemTransformationName = SelectedItemTransformationName;
            uniView.SelectedFootertemTransformationName = SelectedFooterItemTransformationName;
            uniView.SelectedHeaderItemTransformationName = SelectedHeaderItemTransformationName;


            // Public properties
            uniView.HideControlForZeroRows = HideControlForZeroRows;
            uniView.ZeroRowsText = ZeroRowsText;
            uniView.ItemSeparatorValue = ItemSeparator;
            uniView.FilterName = FilterName;

            // Edit mode buttons
            if (PageManager.ViewMode.IsLiveSite())
            {
                btnAdd.Visible = false;
                uniView.ShowEditDeleteButtons = false;
            }
            else
            {
                btnAdd.Visible = ShowNewButton;
                btnAdd.Text = NewButtonText;
                uniView.ShowDeleteButton = ShowDeleteButton;
                uniView.ShowEditButton = ShowEditButton;
            }


            string[] mClassNames = uniView.ClassNames.Split(';');
            btnAdd.ClassName = DataHelper.GetNotEmpty(mClassNames[0], "");

            string mPath = "";
            if (uniView.Path.EndsWithCSafe("/%"))
            {
                mPath = uniView.Path.Remove(uniView.Path.Length - 2);
            }
            if (uniView.Path.EndsWithCSafe("/"))
            {
                mPath = uniView.Path.Remove(uniView.Path.Length - 1);
            }

            btnAdd.Path = DataHelper.GetNotEmpty(mPath, "");

            // Add repeater to the filter collection
            CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), uniView);

            if ((uniView.DataSourceControl != null)
                && (uniView.DataSourceControl.SourceFilterControl != null))
            {
                uniView.DataSourceControl.SourceFilterControl.OnFilterChanged += FilterControl_OnFilterChanged;
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = !uniView.StopProcessing;

        if (DataHelper.DataSourceIsEmpty(uniView.DataSource) && (uniView.HideControlForZeroRows))
        {
            Visible = false;
        }

        // Hide the Add button for selected items which have the SelectedItem transformation specified
        if (ShowNewButton && !String.IsNullOrEmpty(uniView.SelectedItemTransformationName) && uniView.IsSelected)
        {
            btnAdd.Visible = false;
        }
    }


    /// <summary>
    /// Event risen when the source filter has changed
    /// </summary>
    protected void FilterControl_OnFilterChanged()
    {
        // Override previously set visibility. Control's visibility is managed in the PreRender event.
        Visible = true;
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        uniView.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        uniView.ClearCache();
    }
}