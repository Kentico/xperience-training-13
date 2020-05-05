using System;
using System.Web.UI.WebControls;
using CMS.DocumentEngine.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSWebParts_CustomTables_CustomTableDataList : CMSAbstractWebPart
{
    #region "General properties"

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
            lstItems.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, lstItems.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            lstItems.CacheDependencies = value;
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
            lstItems.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), false);
        }
        set
        {
            SetValue("CheckPermissions", value);
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), lstItems.OrderBy), lstItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            lstItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), lstItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            lstItems.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number which indicates how many items should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), lstItems.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            lstItems.SelectTopN = value;
        }
    }

    #endregion


    #region "Pager properties"

    /// <summary>
    /// Gets or sets the value that indicates whether paging is enabled.
    /// </summary>
    public bool EnablePaging
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnablePaging"), lstItems.EnablePaging);
        }
        set
        {
            SetValue("EnablePaging", value);
            lstItems.EnablePaging = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager position.
    /// </summary>
    public PagingPlaceTypeEnum PagerPosition
    {
        get
        {
            return lstItems.PagerControl.GetPagerPosition(DataHelper.GetNotEmpty(GetValue("PagerPosition"), lstItems.PagerControl.PagerPosition.ToString()));
        }
        set
        {
            SetValue("PagerPosition", value.ToString());
            lstItems.PagerControl.PagerPosition = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of the items displayed on each sigle page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PageSize"), lstItems.PageSize);
        }
        set
        {
            SetValue("PageSize", value);
            lstItems.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager query string key.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("QueryStringKey"), lstItems.PagerControl.QueryStringKey), lstItems.PagerControl.QueryStringKey);
        }
        set
        {
            SetValue("QueryStringKey", value);
            lstItems.PagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets whether PostBack or QueryString should be used for the paging.
    /// </summary>
    public PagingModeTypeEnum PagingMode
    {
        get
        {
            return lstItems.PagerControl.GetPagingMode(DataHelper.GetNotEmpty(GetValue("PagingMode"), lstItems.PagerControl.PagingMode.ToString()));
        }
        set
        {
            SetValue("PagingMode", value.ToString());
            lstItems.PagerControl.PagingMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the navigation mode.
    /// </summary>
    public BackNextLocationTypeEnum BackNextLocation
    {
        get
        {
            return lstItems.PagerControl.GetBackNextLocation(DataHelper.GetNotEmpty(GetValue("BackNextLocation"), lstItems.PagerControl.BackNextLocation.ToString()));
        }
        set
        {
            SetValue("BackNextLocation", value.ToString());
            lstItems.PagerControl.BackNextLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether  first and last page is shown if paging is allowed.
    /// </summary>
    public bool ShowFirstLast
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowFirstLast"), lstItems.PagerControl.ShowFirstLast);
        }
        set
        {
            SetValue("ShowFirstLast", value);
            lstItems.PagerControl.ShowFirstLast = value;
        }
    }


    /// <summary>
    /// Gets or sets the html before pager.
    /// </summary>
    public string PagerHTMLBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLBefore"), lstItems.PagerControl.PagerHTMLBefore);
        }
        set
        {
            SetValue("PagerHTMLBefore", value);
            lstItems.PagerControl.PagerHTMLBefore = value;
        }
    }


    /// <summary>
    /// Gets or sets the html after pager.
    /// </summary>
    public string PagerHTMLAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PagerHTMLAfter"), lstItems.PagerControl.PagerHTMLAfter);
        }
        set
        {
            SetValue("PagerHTMLAfter", value);
            lstItems.PagerControl.PagerHTMLAfter = value;
        }
    }


    /// <summary>
    /// Gets or sets the results position.
    /// </summary>
    public ResultsLocationTypeEnum ResultsPosition
    {
        get
        {
            return lstItems.PagerControl.GetResultPosition(ValidationHelper.GetString(GetValue("ResultsPosition"), ""));
        }
        set
        {
            SetValue("ResultsPosition", value);
            lstItems.PagerControl.ResultsLocation = value;
        }
    }


    /// <summary>
    /// Gets or sets the page numbers separator.
    /// </summary>
    public string PageNumbersSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PageNumbersSeparator"), lstItems.PagerControl.PageNumbersSeparator);
        }
        set
        {
            SetValue("PageNumbersSeparator", value);
            lstItems.PagerControl.PageNumbersSeparator = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), lstItems.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            lstItems.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), lstItems.ZeroRowsText), lstItems.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            lstItems.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the count of repeat columns.
    /// </summary>
    public int RepeatColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RepeatColumns"), lstItems.RepeatColumns);
        }
        set
        {
            SetValue("RepeatColumns", value);
            lstItems.RepeatColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets whether control is displayed in a table or flow layout.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            return CMSDataList.GetRepeatLayout(DataHelper.GetNotEmpty(GetValue("RepeatLayout"), lstItems.RepeatLayout.ToString()));
        }
        set
        {
            SetValue("RepeatLayout", value.ToString());
            lstItems.RepeatLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets whether DataList control displays vertically or horizontally.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            return CMSDataList.GetRepeatDirection(DataHelper.GetNotEmpty(GetValue("RepeatDirection"), lstItems.RepeatDirection.ToString()));
        }
        set
        {
            SetValue("RepeatDirection", value.ToString());
            lstItems.RepeatDirection = value;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), lstItems.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            lstItems.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets query string key name. Presence of the key in query string indicates, 
    /// that some item should be selected. The item is determined by query string value.        
    /// </summary>
    public string SelectedQueryStringKeyName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedQueryStringKeyName"), lstItems.SelectedQueryStringKeyName);
        }
        set
        {
            SetValue("SelectedQueryStringKeyName", value);
            lstItems.SelectedQueryStringKeyName = value;
        }
    }


    /// <summary>
    /// Gets or sets columns name by which the item is selected.
    /// </summary>
    public string SelectedDatabaseColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedDatabaseColumnName"), lstItems.SelectedDatabaseColumnName);
        }
        set
        {
            SetValue("SelectedDatabaseColumnName", value);
            lstItems.SelectedDatabaseColumnName = value;
        }
    }


    /// <summary>
    /// Gets or sets validation type for query string value which determines selected item. 
    /// Options are int, guid and string.
    /// </summary>
    public string SelectedValidationType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedValidationType"), lstItems.SelectedValidationType);
        }
        set
        {
            SetValue("SelectedValidationType", value);
            lstItems.SelectedValidationType = value;
        }
    }

    #endregion


    #region "Data binding properties"

    /// <summary>
    /// Gets or sets the value that indicates whether databind will be proceeded automatically.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), lstItems.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            lstItems.DataBindByDefault = value;
        }
    }


    /// <summary>
    /// Gets or sets the custom table.
    /// </summary>
    public string CustomTable
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomTable"), "");
        }
        set
        {
            SetValue("CustomTable", value);
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), lstItems.TransformationName), lstItems.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            lstItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("AlternatingTransformationName"), lstItems.AlternatingTransformationName), lstItems.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            lstItems.AlternatingTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets ItemTemplate for selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), "");
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            lstItems.SelectedItemTransformationName = value;
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
            lstItems.StopProcessing = value;
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
            lstItems.StopProcessing = true;
        }
        else
        {
            // Setup the control
            lstItems.ControlContext = ControlContext;
            lstItems.HideControlForZeroRows = HideControlForZeroRows;
            lstItems.ZeroRowsText = ZeroRowsText;

            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(CustomTable);
            if (dci != null)
            {
                // Check permissions if necessary
                if (CheckPermissions && !dci.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
                {
                    return;
                }

                // General properties
                lstItems.CacheItemName = CacheItemName;
                lstItems.CacheDependencies = CacheDependencies;
                lstItems.CacheMinutes = CacheMinutes;
                lstItems.OrderBy = OrderBy;
                lstItems.SelectTopN = SelectTopN;
                lstItems.WhereCondition = WhereCondition;
                lstItems.FilterName = FilterName;
                lstItems.SelectedQueryStringKeyName = SelectedQueryStringKeyName;
                lstItems.SelectedDatabaseColumnName = SelectedDatabaseColumnName;
                lstItems.SelectedValidationType = SelectedValidationType;

                // Pager
                lstItems.EnablePaging = EnablePaging;
                lstItems.PageSize = PageSize;
                lstItems.PagerControl.PagerPosition = PagerPosition;
                lstItems.PagerControl.QueryStringKey = QueryStringKey;
                lstItems.PagerControl.PagingMode = PagingMode;
                lstItems.PagerControl.BackNextLocation = BackNextLocation;
                lstItems.PagerControl.ShowFirstLast = ShowFirstLast;
                lstItems.PagerControl.PagerHTMLBefore = PagerHTMLBefore;
                lstItems.PagerControl.PagerHTMLAfter = PagerHTMLAfter;
                lstItems.PagerControl.ResultsLocation = ResultsPosition;
                lstItems.PagerControl.PageNumbersSeparator = PageNumbersSeparator;

                // Public
                lstItems.RepeatColumns = RepeatColumns;
                lstItems.RepeatLayout = RepeatLayout;
                lstItems.RepeatDirection = RepeatDirection;

                // Binding
                lstItems.DataBindByDefault = DataBindByDefault;

                // Transformations
                lstItems.AlternatingTransformationName = AlternatingTransformationName;
                lstItems.TransformationName = TransformationName;
                lstItems.SelectedItemTransformationName = SelectedItemTransformationName;

                lstItems.QueryName = dci.ClassName + ".selectall";
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(lstItems.DataSource) && HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        lstItems.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        lstItems.ClearCache();
    }
}