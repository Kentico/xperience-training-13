using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_properties_frameset : CMSWebPartPropertiesPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        rowsFrameset.Attributes.Add("rows", "*, " + FooterFrameHeight);
        
        frameContent.Attributes.Add("src", "webpartproperties_properties.aspx" + RequestContext.CurrentQueryString);
        frameButtons.Attributes.Add("src", "webpartproperties_buttons.aspx" + RequestContext.CurrentQueryString);
    }
}