using System;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;


public partial class CMSWebParts_Viewers_Documents_relateddocuments : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WhereCondition"), "");
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Gets or sets the order by clause.
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
    /// Gets or sets the value that indicates whether the current document is on the left side of the relationship.
    /// </summary>
    public bool CurrentDocumentIsOnTheLeftSide
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CurrentDocumentIsOnTheLeftSide"), true);
        }
        set
        {
            SetValue("CurrentDocumentIsOnTheLeftSide", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the relationship between documents.
    /// </summary>
    public string RelationshipName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RelationshipName"), "");
        }
        set
        {
            SetValue("RelationshipName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), "");
        }
        set
        {
            SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to hide the control when no data found.
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
    /// Gets or sets the text which should be displayed when no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), repElem.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            repElem.ZeroRowsText = value;
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
    /// Gets or sets the number which indicates how many documents should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), repElem.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            repElem.SelectTopN = value;
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


    /// <summary>
    /// Gets or sets the class names which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ClassNames"), repElem.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            repElem.ClassNames = value;
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
            repElem.StopProcessing = value;
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
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
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
            SetContext();

            // Setup the control
            PageInfo currentPage = DocumentContext.CurrentPageInfo;
            if (currentPage != null)
            {
                repElem.ControlContext = ControlContext;
                repElem.SiteName = SiteContext.CurrentSiteName;
                repElem.Path = "/%";
                repElem.CultureCode = currentPage.DocumentCulture;
                repElem.CombineWithDefaultCulture = SiteInfoProvider.CombineWithDefaultCulture(SiteContext.CurrentSiteName);
                repElem.ClassNames = ClassNames;
                repElem.OrderBy = OrderBy;
                repElem.MaxRelativeLevel = -1;
                repElem.SelectOnlyPublished = SelectOnlyPublished;
                repElem.RelationshipWithNodeGuid = currentPage.NodeGUID;
                repElem.RelationshipName = RelationshipName;
                repElem.RelatedNodeIsOnTheLeftSide = CurrentDocumentIsOnTheLeftSide;
                repElem.TopN = SelectTopN;
                repElem.Columns = Columns;
                repElem.WhereCondition = WhereCondition;
                repElem.CacheDependencies = CacheDependencies;
                repElem.CacheMinutes = CacheMinutes;
                
                if (TransformationName != string.Empty)
                {
                    repElem.TransformationName = TransformationName;
                }

                repElem.HideControlForZeroRows = HideControlForZeroRows;
                repElem.ZeroRowsText = ZeroRowsText;
                repElem.EnablePaging = false;
            }

            ReleaseContext();
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
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repElem.ClearCache();
    }
}