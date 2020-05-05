using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Viewers_Documents_attachments : CMSAbstractWebPart
{
    #region "Repeater properties"

    /// <summary>
    /// Gets or sets the maximum nesting level. It specifies the number of sub-levels in the content tree 
    /// that should be included in the displayed content.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), -1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            repItems.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Gets or sets the ORDER BY part of the SELECT query.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "DocumentName");
        }
        set
        {
            SetValue("OrderBy", value);
            repItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), repItems.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            repItems.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed when there is no data.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), GetString("AttachmentsWebPart.NoDataFound"));
        }
        set
        {
            SetValue("ZeroRowsText", value);
            repItems.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to enable paging or not.
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
            repItems.EnablePaging = value;
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
            repItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the class names (document types) separated with semicolon, which should be displayed.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ClassNames"), null);
        }
        set
        {
            SetValue("ClassNames", value);
            repItems.ClassNames = value;
        }
    }

    #endregion


    /// <summary>
    /// Gets or sets the value that indicates whether to enable button for adding new attachments in EDIT mode.
    /// </summary>
    public bool EnableAddButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableAddButton"), false);
        }
        set
        {
            SetValue("EnableAddButton", value);
        }
    }


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
            repItems.StopProcessing = value;
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
            repItems.StopProcessing = true;
        }
        else
        {
            repItems.ControlContext = ControlContext;

            repItems.MaxRelativeLevel = MaxRelativeLevel;
            repItems.ClassNames = ClassNames;
            if (TransformationName != "")
            {
                repItems.TransformationName = TransformationName;
            }

            repItems.CacheItemName = CacheItemName;
            repItems.CacheDependencies = CacheDependencies;
            repItems.CacheMinutes = CacheMinutes;
            repItems.OrderBy = OrderBy;

            repItems.ZeroRowsText = ZeroRowsText;
            repItems.HideControlForZeroRows = HideControlForZeroRows;
            repItems.EnablePaging = EnablePaging;

            if ((EnableAddButton) && (PortalContext.ViewMode.IsEdit()))
            {
                DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassNames);
                if (dci != null)
                {
                    ltrAddButton.Text = "<a href=\"javascript:NewDocument( " + DocumentContext.CurrentDocument.NodeID + ", " + dci.ClassID + ");\" class=\"CMSEditModeButtonAdd\">" + GetString("AttachmentsWebPart.Add") + "</a><br />";
                }
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = repItems.Visible && !StopProcessing;

        if (DataHelper.DataSourceIsEmpty(repItems.DataSource) && (HideControlForZeroRows))
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
        repItems.ReloadData(true);
    }


    /// <summary>
    /// Clear cache.
    /// </summary>
    public override void ClearCache()
    {
        repItems.ClearCache();
    }
}