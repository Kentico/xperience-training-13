using System;

using CMS.Base.Web.UI;


public partial class CMSAdminControls_UI_UniGrid_Controls_UpDownMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string menuId = ContextMenu.MenuID;

        iTop.Text = GetString("General.Top");
        iTop.OnClientClick = "MoveNode('top',GetContextMenuParameter('" + menuId + "'))";

        iUp.Text = GetString("General.Up");
        iUp.OnClientClick = "MoveNode('up',GetContextMenuParameter('" + menuId + "'))";

        iDown.Text = GetString("General.Down");
        iDown.OnClientClick = "MoveNode('down',GetContextMenuParameter('" + menuId + "'))";

        iBottom.Text = GetString("General.Bottom");
        iBottom.OnClientClick = "MoveNode('bottom',GetContextMenuParameter('" + menuId + "'))";
    }
}
