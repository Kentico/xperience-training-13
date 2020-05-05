using System;

using CMS.UIControls;


public partial class CMSFormControls_LiveSelectors_InsertImageOrMedia_Default : CMSLiveModalPage
{
    protected string mBlankUrl = null;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        mBlankUrl = ResolveUrl("~/CMSPages/blank.htm");
    }
}