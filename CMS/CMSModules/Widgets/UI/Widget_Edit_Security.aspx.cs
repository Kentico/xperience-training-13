using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Widgets_UI_Widget_Edit_Security : GlobalAdminPage
{
    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        widgetSecurity.WidgetID = QueryHelper.GetInteger("widgetid", 0);
    }

    #endregion
}