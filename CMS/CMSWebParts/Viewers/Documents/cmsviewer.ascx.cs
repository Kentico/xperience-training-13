using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_cmsviewer : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), viewElem.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            viewElem.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), viewElem.SelectedColumns);
        }
        set
        {
            SetValue("Columns", value);
            viewElem.SelectedColumns = value;
        }
    }


    /// <summary>
    /// Gest or sest the cache item name.
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
            viewElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, viewElem.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            viewElem.CacheDependencies = value;
        }
    }


    /// <summary>
    ///Gets or sets the cache minutes.
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
            viewElem.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), viewElem.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            viewElem.CheckPermissions = value;
        }
    }


    /// <summary>
    ///Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ClassNames"), viewElem.ClassNames), viewElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            viewElem.ClassNames = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether documents are combined with default culture version.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), viewElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            viewElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture code of the documents.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), viewElem.CultureCode), viewElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            viewElem.CultureCode = value;
        }
    }


    /// <summary>
    /// Gets or sets the maximal relative level of the documents to be shown.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), viewElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            viewElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), viewElem.OrderBy), viewElem.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            viewElem.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the path of the documents.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), viewElem.Path);
        }
        set
        {
            SetValue("Path", value);
            viewElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether only published documents are selected.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), viewElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            viewElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), viewElem.SiteName), viewElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            viewElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), viewElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            viewElem.WhereCondition = value;
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
            return ValidationHelper.GetBoolean(GetValue("RelatedNodeIsOnTheLeftSide"), viewElem.RelatedNodeIsOnTheLeftSide);
        }
        set
        {
            SetValue("RelatedNodeIsOnTheLeftSide", value);
            viewElem.RelatedNodeIsOnTheLeftSide = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship name.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("RelationshipName"), viewElem.RelationshipName), viewElem.RelationshipName);
        }
        set
        {
            SetValue("RelationshipName", value);
            viewElem.RelationshipName = value;
        }
    }


    /// <summary>
    /// Gets or sets the relationship with node GUID.
    /// </summary>
    public Guid RelationshipWithNodeGUID
    {
        get
        {
            return ValidationHelper.GetGuid(GetValue("RelationshipWithNodeGuid"), viewElem.RelationshipWithNodeGuid);
        }
        set
        {
            SetValue("RelationshipWithNodeGuid", value);
            viewElem.RelationshipWithNodeGuid = value;
        }
    }

    #endregion


    #region "Basic viewer properties"

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), viewElem.TransformationName), viewElem.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            viewElem.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), viewElem.SelectedItemTransformationName), viewElem.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            viewElem.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets HideControlForZeroRows property.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), true);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            viewElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets ZeroRowsText property.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), "");
        }
        set
        {
            SetValue("ZeroRowsText", value);
            viewElem.ZeroRowsText = value;
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
            viewElem.StopProcessing = value;
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
            viewElem.StopProcessing = true;
        }
        else
        {
            if (!String.IsNullOrEmpty(TransformationName))
            {
                // Basic control properties
                viewElem.HideControlForZeroRows = HideControlForZeroRows;
                viewElem.ZeroRowsText = ZeroRowsText;

                // Set context
                viewElem.ControlContext = ControlContext;

                // Set properties from Webpart form   
                viewElem.CacheItemName = CacheItemName;
                viewElem.CacheDependencies = CacheDependencies;
                viewElem.CacheMinutes = CacheMinutes;
                viewElem.CheckPermissions = CheckPermissions;
                viewElem.ClassNames = ClassNames;
                viewElem.CombineWithDefaultCulture = CombineWithDefaultCulture;
                viewElem.CultureCode = CultureCode;
                viewElem.MaxRelativeLevel = MaxRelativeLevel;
                viewElem.OrderBy = OrderBy;
                viewElem.WhereCondition = WhereCondition;
                viewElem.SelectOnlyPublished = SelectOnlyPublished;
                viewElem.SiteName = SiteName;
                viewElem.Path = Path;

                // Set relationship properties
                viewElem.RelatedNodeIsOnTheLeftSide = RelatedNodeIsOnTheLeftSide;
                viewElem.RelationshipName = RelationshipName;
                viewElem.RelationshipWithNodeGuid = RelationshipWithNodeGUID;


                #region "Viewer template properties"

                // Apply transformations
                viewElem.TransformationName = TransformationName;
                viewElem.SelectedItemTransformationName = SelectedItemTransformationName;

                #endregion


                viewElem.SelectTopN = SelectTopN;
                viewElem.Columns = Columns;
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (viewElem.NoData && HideControlForZeroRows)
        {
            Visible = false;
        }
        else
        {
            Visible = viewElem.Visible && !StopProcessing;
        }
        base.OnPreRender(e);
    }


    /// <summary>
    /// ReloadData override.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }
}