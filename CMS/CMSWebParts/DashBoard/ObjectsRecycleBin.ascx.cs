using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_DashBoard_ObjectsRecycleBin : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the order by condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "VersionDeletedWhen");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Gets or sets the sorting direction.
    /// </summary>
    public string Sorting
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Sorting"), "ASC");
        }
        set
        {
            SetValue("Sorting", value);
        }
    }


    /// <summary>
    /// Gets or sets the value of items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemsPerPage"), "25");
        }
        set
        {
            SetValue("ItemsPerPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the document name filter value.
    /// </summary>
    public string ObjectDisplayName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectDisplayName"), String.Empty);
        }
        set
        {
            SetValue("ObjectDisplayName", value);
        }
    }


    /// <summary>
    /// Gets or sets the document type names which should be displayed in grid.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectType"), String.Empty);
        }
        set
        {
            SetValue("ObjectType", value);
        }
    }

    #endregion


    #region "Methods"

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
            recBin.StopProcessing = true;
        }
        else
        {
            recBin.ObjectDisplayName = ObjectDisplayName;
            recBin.ObjectType = ObjectType;
            recBin.IsLiveSite = ViewMode.IsLiveSite();
            recBin.ItemsPerPage = ItemsPerPage;
            recBin.IsSingleSite = true;
            recBin.SiteName = SiteContext.CurrentSiteName;
            recBin.RestrictUsers = false;

            if (!String.IsNullOrEmpty(OrderBy))
            {
                recBin.OrderBy = OrderBy + " " + Sorting;
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}