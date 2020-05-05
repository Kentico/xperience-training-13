using System;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSMasterPages_UI_Tree : CMSMasterPage
{
    #region "Properties"

    /// <summary>
    /// Gets placeholder located after form element.
    /// </summary>
    public override PlaceHolder AfterFormPlaceholder
    {
        get
        {
            return plcAfterForm;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Do not keep space for menu content if not needed
        if (plcMenu.Controls.Count == 0)
        {
            treeAreaMenu.Attributes["class"] += " tree-area-menu-empty";
        }
    }

    #endregion
}