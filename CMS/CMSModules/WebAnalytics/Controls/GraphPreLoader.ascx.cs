using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_WebAnalytics_Controls_GraphPreLoader : CMSUserControl
{
    /// <summary>
    /// OnPreRender override - set preloader area if is required.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Hide panel by default
        pnlPreLoader.Style.Add("display", "none");

        bool useAutoWidth = ValidationHelper.GetBoolean(RequestStockHelper.GetItem("CMSGraphAutoWidth"), false);

        // If is initial request, display preparing information
        if (!RequestHelper.IsPostBack() && useAutoWidth)
        {
            pnlPreLoader.Style.Clear();
            pnlPreLoader.Style.Add("position", "absolute");
            pnlPreLoader.Style.Add("top", "0");
            pnlPreLoader.Style.Add("left", "0");
            pnlPreLoader.Style.Add("width", "100%");
            pnlPreLoader.Style.Add("height", "100%");
            pnlPreLoader.Style.Add("z-index", "100000");
            pnlPreLoader.Style.Add("background-color", "#ffffff");
            pnlPreLoader.Style.Add("overflow", "hidden");
        }

        base.OnPreRender(e);
    }
}