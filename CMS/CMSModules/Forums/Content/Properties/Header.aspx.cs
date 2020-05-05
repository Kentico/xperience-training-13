using System;

using CMS.UIControls;


public partial class CMSModules_Forums_Content_Properties_Header : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("forum.header.forum");
    }
}