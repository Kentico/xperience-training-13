using System;
using System.Web.UI;

using CMS.DocumentEngine.Web.UI;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_cmscalendar : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the name of the cache item. If not explicitly specified, the name is automatically 
    /// created based on the control unique ID
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
            calItems.CacheItemName = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of minutes for which the content is kept in the cache 
    /// until the latest version is reloaded from the database. If you specify 0, the 
    /// content is not cached. If you specify -1, the site-level settings are used
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
            calItems.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item dependencies.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return base.CacheDependencies;
        }
        set
        {
            base.CacheDependencies = value;
            calItems.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), calItems.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            calItems.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names (document types) separated with semicolon, which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ClassNames"), calItems.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            calItems.ClassNames = value;
        }
    }


    /// <summary>
    /// Code name of the category to display documents from.
    /// </summary>
    public string CategoryName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoryName"), calItems.CategoryName);
        }
        set
        {
            SetValue("CategoryName", value);
            calItems.CategoryName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the default language version of the document 
    /// should be displayed if the document is not translated to the current language.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), calItems.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            calItems.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture version of the displayed content.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CultureCode"), calItems.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            calItems.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximum nesting level. It specifies the number of sub-levels in the content tree 
    /// that should be included in the displayed content
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), calItems.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            calItems.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the ORDER BY part of the SELECT query.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("OrderBy"), calItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            calItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), calItems.Path);
        }
        set
        {
            SetValue("Path", value);
            calItems.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the main document is on the left side of the relationship.
    /// </summary>
    public bool RelatedNodeIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), calItems.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            calItems.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the relationship between documents.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("RelationshipName"), calItems.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            calItems.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets whether not to use relationships (Guid.Empty) or use relationships with main document
    /// as Currentdocument ("11111111-1111-1111-1111-111111111111") or document with specified guid
    /// </summary>
    public Guid RelationshipWithNodeGuid
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), calItems.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            calItems.RelationshipWithNodeGuid = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show only published documents.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), calItems.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            calItems.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the codename of the site from which you want do display the content.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SiteName"), calItems.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            calItems.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID which should be used.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;
            if ((calItems != null) && (PageCycle < PageCycleEnum.Initialized))
            {
                calItems.SkinID = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("TransformationName"), calItems.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            calItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the WHERE part of the SELECT query.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), calItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            calItems.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the filed (column name) which contains the date.
    /// </summary>
    public string DayField
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("DayField"), calItems.DayField);
        }
        set
        {
            SetValue("DayField", value);
            calItems.DayField = value;
        }
    }


    /// <summary>
    /// Gets or sets the transformation name which should be used when there is no event for the specified date.
    /// </summary>
    public string NoEventTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NoEventTransformationName"), calItems.NoEventTransformationName);
        }
        set
        {
            SetValue("NoEventTransformationName", value);
            calItems.NoEventTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the day number is displayed or cell is full filled by the transformation
    /// Current day is available in the "__day" column
    /// </summary>
    public bool HideDefaultDayNumber
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideDefaultDayNumber"), calItems.HideDefaultDayNumber);
        }
        set
        {
            SetValue("HideDefaultDayNumber", value);
            calItems.HideDefaultDayNumber = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the only one item is displayed in the day.
    /// </summary>
    public bool DisplayOnlySingleDayItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlySingleDayItem"), calItems.DisplayOnlySingleDayItem);
        }
        set
        {
            SetValue("DisplayOnlySingleDayItem", value);
            calItems.DisplayOnlySingleDayItem = value;
        }
    }


    /// <summary>
    /// Calendar control.
    /// </summary>
    public CMSCalendar CalendarControl
    {
        get
        {
            return calItems;
        }
    }


    /// <summary>
    /// Filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), calItems.FilterName);
        }
        set
        {
            SetValue("FilterName", value);
            calItems.FilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), calItems.Columns);
        }
        set
        {
            SetValue("Columns", value);
            calItems.Columns = value;
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
            calItems.StopProcessing = value;
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
        // In design mode is pocessing of control stoped
        if (StopProcessing)
        {
            calItems.StopProcessing = true;
        }
        else
        {
            // Set properties from Webpart form   
            calItems.CacheItemName = CacheItemName;
            calItems.CacheDependencies = CacheDependencies;
            calItems.CacheMinutes = CacheMinutes;
            calItems.CheckPermissions = CheckPermissions;
            calItems.ClassNames = ClassNames;
            calItems.CategoryName = CategoryName;
            calItems.CombineWithDefaultCulture = CombineWithDefaultCulture;

            calItems.CultureCode = CultureCode;
            calItems.MaxRelativeLevel = MaxRelativeLevel;
            calItems.OrderBy = OrderBy;
            calItems.Path = Path;
            calItems.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;

            calItems.RelationshipName = RelationshipName;
            calItems.RelationshipWithNodeGuid = RelationshipWithNodeGuid;
            calItems.SelectOnlyPublished = SelectOnlyPublished;
            calItems.SiteName = SiteName;

            calItems.TransformationName = TransformationName;
            calItems.WhereCondition = WhereCondition;
            calItems.DayField = DayField;
            calItems.FilterName = FilterName;

            calItems.SelectedColumns = Columns;

            calItems.HideDefaultDayNumber = HideDefaultDayNumber;
            calItems.DisplayOnlySingleDayItem = DisplayOnlySingleDayItem;
            calItems.NoEventTransformationName = NoEventTransformationName;

            var currentDate = TimeZoneUIMethods.ConvertDateTime(DateTime.Now, this).Date;
            calItems.TodaysDate = currentDate;
            calItems.VisibleDate = currentDate;
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = calItems.Visible && !StopProcessing;
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        calItems.SkinID = SkinID;
        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
        calItems.ReloadData(true);
    }
}