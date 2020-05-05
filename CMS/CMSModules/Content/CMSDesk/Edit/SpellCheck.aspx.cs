using System;

using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Edit_SpellCheck : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize modal page
        PageTitle.TitleText = GetString("SpellCheck.Title");
    }
}