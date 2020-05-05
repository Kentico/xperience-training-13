using System;
using System.Data;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_randomdocument : CMSAbstractWebPart
{
    #region "Document properties"

    /// <summary>
    /// Gets or sets the class names (document types) separated with semicolon, which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), repElem.ClassNames), repElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            repElem.ClassNames = value;
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
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), repElem.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            repElem.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to show only published documents.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), repElem.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            repElem.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Gets or sets the culture version of the displayed content.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), repElem.CultureCode), repElem.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            repElem.CultureCode = value;
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
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), repElem.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            repElem.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), repElem.Path);
        }
        set
        {
            SetValue("Path", value);
            repElem.Path = value;
        }
    }


    /// <summary>
    /// Gets or sets the codename of the site from which you want do display the content.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), repElem.SiteName), repElem.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            repElem.SiteName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), repElem.TransformationName), repElem.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            repElem.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the WHERE part of the SELECT query.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), repElem.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            repElem.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), repElem.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            repElem.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows results.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), repElem.ZeroRowsText), repElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            repElem.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of random documents that should be selected, by default one random document is selected.
    /// </summary>
    public int SelectRandomN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectRandomN"), 1);
        }

        set
        {
            SetValue("SelectRandomN", value);
            if (value > 0)
            {
                repElem.SelectTopN = value;
                repElem.OrderBy = "newid()";
            }
        }
    }


    /// <summary>
    /// Gets or sets the 'ORDER BY' expression.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), repElem.Columns);
        }
        set
        {
            SetValue("Columns", value);
            repElem.Columns = value;
        }
    }

    #endregion


    #region "Properties"

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
            repElem.StopProcessing = value;
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
            repElem.CacheMinutes = value;
        }
    }


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
            repElem.CacheItemName = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
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
            repElem.CacheDependencies = value;
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
            repElem.StopProcessing = true;
        }
        else
        {
            repElem.ControlContext = ControlContext;

            // Setup the control
            repElem.ClassNames = ClassNames;
            repElem.CultureCode = CultureCode;
            repElem.MaxRelativeLevel = MaxRelativeLevel;
            repElem.WhereCondition = WhereCondition;
            repElem.Path = Path;
            repElem.SiteName = SiteName;
            repElem.SelectOnlyPublished = SelectOnlyPublished;
            repElem.CombineWithDefaultCulture = CombineWithDefaultCulture;

            repElem.CacheMinutes = CacheMinutes;
            repElem.CacheItemName = CacheItemName;
            repElem.CacheDependencies = CacheDependencies;

            repElem.HideControlForZeroRows = HideControlForZeroRows;
            repElem.ZeroRowsText = ZeroRowsText;

            repElem.TransformationName = TransformationName;
            repElem.Columns = Columns;

            // Select random documents
            if (SelectRandomN > 0)
            {
                // Select random documents
                repElem.SelectTopN = SelectRandomN;
                repElem.OrderBy = "newid()";

                // Use custom 'Order by' expression
                if (SelectRandomN > 1)
                {
                    repElem.DataBinding += repElem_DataBinding;
                }
            }
            else
            {
                repElem.OrderBy = OrderBy;
            }
        }
    }


    private void repElem_DataBinding(object sender, EventArgs e)
    {
        // Order data by custom 'Order by' expression
        if ((OrderBy != "") &&
            (repElem.DataSource is DataSet) &&
            !DataHelper.DataSourceIsEmpty(repElem.DataSource))
        {
            ((DataSet)repElem.DataSource).Tables[0].DefaultView.Sort = OrderBy;
            repElem.DataSource = ((DataSet)repElem.DataSource).Tables[0].DefaultView;
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);


        Visible = repElem.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(repElem.DataSource) && HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        repElem.ReloadData(true);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repElem.ClearCache();
    }
}