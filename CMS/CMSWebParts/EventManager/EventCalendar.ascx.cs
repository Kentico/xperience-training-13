using System;
using System.Web.UI;

using CMS.DocumentEngine.Web.UI;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.PortalEngine;


public partial class CMSWebParts_EventManager_EventCalendar : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether permissions are checked.
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
            repEvent.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ClassNames"), calItems.ClassNames), calItems.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            calItems.ClassNames = value;
            repEvent.ClassNames = value;
        }
    }


    /// <summary>
    ///  Gets or sets the value that indicates whether data are combined with default control.
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
            repEvent.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), calItems.CultureCode), calItems.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            calItems.CultureCode = value;
            repEvent.CultureCode = value;
        }
    }


    /// <summary>
    ///  Gets or sets the max. relative level.
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
    ///  Gets or sets the order by value.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), calItems.OrderBy), calItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            calItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path.
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
    ///  Gets or sets the value that indicates whether related node is on the left side.
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
    ///  Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("RelationshipName"), calItems.RelationshipName), calItems.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            calItems.RelationshipName = value;
        }
    }


    /// <summary>
    ///  Gets or sets the relationShip with node GUID.
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
    ///  Gets or sets the value that indicates whether selected documents must be only published.
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
            repEvent.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    ///  Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), calItems.SiteName), calItems.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            calItems.SiteName = value;
            repEvent.SiteName = value;
        }
    }


    /// <summary>
    ///  Gets or sets the name of the transformation which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), "cms.bookingevent.EventCalendarItem"), "cms.bookingevent.EventCalendarItem");
        }
        set
        {
            SetValue("TransformationName", value);
            calItems.TransformationName = value;
        }
    }


    /// <summary>
    ///  Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("WhereCondition"), calItems.WhereCondition), calItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            calItems.WhereCondition = value;
            repEvent.WhereCondition = value;
        }
    }


    /// <summary>
    ///  Gets or sets the field of event start date.
    /// </summary>
    public string DayField
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("DayField"), "EventDate"), "EventDate");
        }
        set
        {
            SetValue("DayField", value);
            calItems.DayField = value;
        }
    }


    /// <summary>
    /// Gets or sets the field of event end date.
    /// </summary>
    public string EventEndField
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EventEndField"), calItems.EventEndField);
        }
        set
        {
            SetValue("EventEndField", value);
            calItems.EventEndField = value;
        }
    }


    /// <summary>
    ///  Gets or sets the no event transformation name.
    /// </summary>
    public string NoEventTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NoEventTransformationName"), "cms.bookingevent.calendarnoevent");
        }
        set
        {
            SetValue("NoEventTransformationName", value);
            calItems.NoEventTransformationName = value;
        }
    }


    /// <summary>
    /// Indicates whether the day number is displayed or cell is full filled by the transformation.
    /// Current day is saved in the "__day" column.
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
    /// Gets or sets the columns to retrieve.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Columns"), calItems.Columns), calItems.Columns);
        }
        set
        {
            SetValue("Columns", value);
            calItems.Columns = value;
            repEvent.Columns = value;
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
                calItems.SkinID = SkinID;
            }
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
            calItems.CacheMinutes = value;
            repEvent.CacheMinutes = value;
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
            repEvent.CacheDependencies = value;
        }
    }


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
            if (!String.IsNullOrEmpty(value))
            {
                repEvent.CacheItemName = value + "detail";
            }
            else
            {
                repEvent.CacheItemName = string.Empty;
            }
        }
    }

    #endregion


    #region "Repeater properties"

    /// <summary>
    /// Event detail transformation.
    /// </summary>
    public string EventDetailTransformation
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EventDetailTransformation"), "cms.bookingevent.Default");
        }
        set
        {
            SetValue("EventDetailTransformation", value);
            repEvent.SelectedItemTransformationName = value;
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
    /// Reloads control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();

        calItems.ReloadData(true);
        repEvent.ReloadData(true);
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            calItems.StopProcessing = true;
        }
        else
        {
            calItems.ControlContext = repEvent.ControlContext = ControlContext;

            // Calendar properties
            calItems.CacheItemName = CacheItemName;

            calItems.CacheDependencies = CacheDependencies;
            calItems.CacheMinutes = CacheMinutes;
            calItems.CheckPermissions = CheckPermissions;
            calItems.ClassNames = ClassNames;
            calItems.CombineWithDefaultCulture = CombineWithDefaultCulture;

            calItems.CultureCode = CultureCode;
            calItems.MaxRelativeLevel = MaxRelativeLevel;
            calItems.OrderBy = OrderBy;
            calItems.WhereCondition = WhereCondition;
            calItems.Columns = Columns;
            calItems.Path = Path;
            calItems.SelectOnlyPublished = SelectOnlyPublished;
            calItems.SiteName = SiteName;
            calItems.FilterName = FilterName;

            calItems.RelationshipName = RelationshipName;
            calItems.RelationshipWithNodeGuid = RelationshipWithNodeGuid;
            calItems.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;

            calItems.TransformationName = TransformationName;
            calItems.NoEventTransformationName = NoEventTransformationName;

            calItems.DayField = DayField;
            calItems.EventEndField = EventEndField;
            calItems.HideDefaultDayNumber = HideDefaultDayNumber;

            var currentDate = TimeZoneUIMethods.ConvertDateTime(DateTime.Now, this).Date;
            calItems.TodaysDate = currentDate;
            calItems.VisibleDate = currentDate;

            bool detail = false;

            // If calendar event path is defined event is loaded in accordance to the selected path
            string eventPath = QueryHelper.GetString("CalendarEventPath", null);
            if (!String.IsNullOrEmpty(eventPath))
            {
                detail = true;
                repEvent.Path = eventPath;

                // Set selected date to specific document
                TreeNode node = GetDocument(eventPath);
                if (node != null)
                {
                    object value = node.GetValue(DayField);
                    if (ValidationHelper.GetDateTimeSystem(value, DateTimeHelper.ZERO_TIME) != DateTimeHelper.ZERO_TIME)
                    {
                        calItems.TodaysDate = TimeZoneUIMethods.ConvertDateTime((DateTime)value, this);
                    }
                }
            }

            // By default select current event from current document value
            PageInfo currentPage = DocumentContext.CurrentPageInfo;
            if ((currentPage != null) && (ClassNames.IndexOf(currentPage.ClassName, StringComparison.InvariantCultureIgnoreCase) >= 0))
            {
                detail = true;
                repEvent.Path = currentPage.NodeAliasPath;

                // Set selected date to current document
                object value = DocumentContext.CurrentDocument.GetValue(DayField);
                if (ValidationHelper.GetDateTimeSystem(value, DateTimeHelper.ZERO_TIME) != DateTimeHelper.ZERO_TIME)
                {
                    calItems.SelectedDate = TimeZoneUIMethods.ConvertDateTime((DateTime)value, this).Date;
                    calItems.VisibleDate = calItems.SelectedDate;

                    // Get name of coupled class ID column
                    string idColumn = DocumentContext.CurrentDocument.CoupledClassIDColumn;
                    if (!string.IsNullOrEmpty(idColumn))
                    {
                        // Set selected item ID and the ID column name so it is possible to highlight specific event in the calendar
                        calItems.SelectedItemIDColumn = idColumn;
                        calItems.SelectedItemID = ValidationHelper.GetInteger(DocumentContext.CurrentDocument.GetValue(idColumn), 0);
                    }
                }
            }

            if (detail)
            {
                // Setup the detail repeater
                repEvent.Visible = true;
                repEvent.StopProcessing = false;

                repEvent.SelectedItemTransformationName = EventDetailTransformation;
                repEvent.ClassNames = ClassNames;
                repEvent.Columns = Columns;

                if (!String.IsNullOrEmpty(CacheItemName))
                {
                    repEvent.CacheItemName = CacheItemName + "|detail";
                }

                repEvent.CacheDependencies = CacheDependencies;
                repEvent.CacheMinutes = CacheMinutes;
                repEvent.CheckPermissions = CheckPermissions;
                repEvent.CombineWithDefaultCulture = CombineWithDefaultCulture;

                repEvent.CultureCode = CultureCode;

                repEvent.SelectOnlyPublished = SelectOnlyPublished;
                repEvent.SiteName = SiteName;

                repEvent.WhereCondition = WhereCondition;
            }
        }
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
    /// Clear cache.
    /// </summary>
    public override void ClearCache()
    {
        calItems.ClearCache();
        repEvent.ClearCache();
    }


    private TreeNode GetDocument(string path)
    {
        var tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        var node = tree.SelectSingleNode(SiteName, path, CultureCode, CombineWithDefaultCulture, ClassNames, SelectOnlyPublished, CheckPermissions);

        if ((node != null) && (PortalContext.ViewMode != ViewModeEnum.LiveSite))
        {
            node = DocumentHelper.GetDocument(node, tree);
        }

        return node;
    }
}