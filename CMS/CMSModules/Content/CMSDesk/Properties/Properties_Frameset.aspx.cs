using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Properties_Properties_Frameset : CMSModalPage
{
    protected string contentPage = null;


    /// <summary>
    /// Gets the footer frame height with dependance on current mode
    /// </summary>
    protected int PropertiesFooterHeight 
    {
        get
        {
            if (QueryHelper.GetString("mode", String.Empty).EqualsCSafe("editlive", true))
            {
                return FooterFrameHeight;
            }
            return 0;
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        contentPage = UIContextHelper.GetElementUrl("CMS.Content", "Properties");

        contentPage = URLHelper.AppendQuery(contentPage, "?displaytitle=false");
        contentPage = URLHelper.AppendQuery(contentPage, QueryHelper.EncodedQueryString);
    }
}