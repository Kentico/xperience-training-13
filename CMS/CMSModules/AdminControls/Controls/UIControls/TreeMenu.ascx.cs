using System;
using System.Linq;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_TreeMenu : CMSAbstractUIWebpart
{
    #region "Variables"

    /// <summary>
    /// Indicates whether automatically select first item or display UI guide
    /// </summary>
    protected bool displayGuide = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Maximum relative level for tree.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return GetIntContextValue("MaxRelativeLevel", 1);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
        }
    }


    /// <summary>
    /// Returns true if the control processing should be stopped
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            layoutElem.StopProcessing = value;
            layoutElem.Visible = !value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // No actions if processing is stopped
        }
        else
        {
            // Handle title
            ManagePaneTitle(paneTitle, true);

            // Show dialog footer only when used in a dialog
            layoutElem.DisplayFooter = DisplayFooter;

            Control t = paneContent.FindControl("treeElem");
            t.DataBind();

            ScriptHelper.HideVerticalTabs(Page);
        }
    }

    #endregion
}
