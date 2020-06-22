using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSAdminControls_UI_SmartTip : SmartTipControl
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CollapsedStateIdentifier))
        {
            CollapsedStateIdentifier = ClientID;
        }

        var resources = new Dictionary<string, string>
        {
            {"smarttip.smarttip", GetString("smarttip.smarttip")},
            {"smarttip.expand", GetString("smarttip.expand")},
            {"general.collapse", GetString("general.collapse")},
        };

        if (string.IsNullOrEmpty(CollapsedHeader))
        {
            CollapsedHeader = ExpandedHeader;
        }

        if (string.IsNullOrEmpty(ExpandedHeader))
        {
            ExpandedHeader = CollapsedHeader;
        }

        if (RequestHelper.IsAsyncPostback() && !ControlsHelper.IsInUpdatePanel(this))
        {
            return;
        }

        ScriptHelper.RegisterModule(this, "CMS/SmartTips/SmartTip", new
        {
            selector = "#" + pnlTooltip.ClientID,
            expandedHeader = ResHelper.LocalizeString(ExpandedHeader),
            collapsedHeader = ResHelper.LocalizeString(CollapsedHeader),
            content = ResHelper.LocalizeString(Content),
            isCollapsed = UserSmartTipManager.IsSmartTipDismissed(CollapsedStateIdentifier),
            identifier = CollapsedStateIdentifier,
            resources
        });
    }
}