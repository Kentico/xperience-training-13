using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_WebAnalytics_Controls_UI_Conversion_List : CMSAdminListControl
{
    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
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
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }
    

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.WhereCondition = "ConversionSiteID =" + SiteContext.CurrentSiteID;

        var editUrl = UIContextHelper.GetElementUrl("CMS.WebAnalytics", "ConversionProperties");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "objectId", "{0}");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "displayTitle", "0");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "displayReport", QueryHelper.GetBoolean("displayReport", false).ToString());
        gridElem.EditActionUrl = editUrl;
    }
}