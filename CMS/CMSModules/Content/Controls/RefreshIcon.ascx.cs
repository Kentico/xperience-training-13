using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_RefreshIcon : CMSUserControl
{
    #region "Variables"

    private string mFixedInControlClientID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Control in which the icon is fixed (even when scroll). 
    /// Parent by default.
    /// </summary>
    public string FixedInControlClientID
    {
        get
        {
            return mFixedInControlClientID ?? Parent.ClientID;
        }
        set
        {
            mFixedInControlClientID = value;
        }
    }


    /// <summary>
    /// Client click action.
    /// </summary>
    public string OnClientClick
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Fix position for tree-refresh icon
        ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshIcon_" + ClientID, ScriptHelper.GetScript(
@"
var originalPosition_" + ClientID + @" = $cmsj('#" + pnlRefresh.ClientID + @"').position();
var fixedArea_" + ClientID + @" = $cmsj('#" + FixedInControlClientID + @"');
fixedArea_" + ClientID + @".scroll(function () {
  document.getElementById('" + pnlRefresh.ClientID + @"').style.top = originalPosition_" + ClientID + @".top + fixedArea_" + ClientID + @".scrollTop() + 'px';
});"));

        btnRefresh.ToolTip = GetString("general.refresh");
        btnRefresh.OnClientClick = OnClientClick;
        pnlRefresh.Attributes.Add("onmouseover", "document.getElementById('" + btnRefresh.ClientID + "').style.visibility = 'visible';");
        pnlRefresh.Attributes.Add("onmouseout", "document.getElementById('" + btnRefresh.ClientID + "').style.visibility = 'hidden';");
    }

    #endregion
}
