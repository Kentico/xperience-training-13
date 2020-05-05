using System;

using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_SplitView_Toolbar : CMSContentPage
{
    #region "Methods

    protected void Page_Load(object sender, EventArgs e)
    {
        documentToolbar.Node = Node;
    }

    #endregion
}