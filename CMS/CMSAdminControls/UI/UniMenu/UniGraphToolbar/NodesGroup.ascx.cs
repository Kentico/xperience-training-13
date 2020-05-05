using System;
using System.Collections.Generic;

using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;


public partial class CMSAdminControls_UI_UniMenu_UniGraphToolbar_NodesGroup : CMSUserControl
{

    /// <summary>
    /// Handles value sent to control.
    /// </summary>
    public List<Item> NodesMenuItems 
    { 
        get
        {
            return (List<Item>)GetValue("NodesMenuItems");
        }
        set
        {
            SetValue("NodesMenuItems", value);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        buttons.Buttons = NodesMenuItems;
    }
}