using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.UIControls;

[assembly: RegisterCustomClass("WebPartHeaderControlExtender", typeof(WebPartHeaderControlExtender))]

/// <summary>
/// Extender class
/// </summary>
public class WebPartHeaderControlExtender : UITabsExtender
{
    public override void OnInit()
    {
        base.OnInit();

        ScriptHelper.RegisterClientScriptBlock(Control, typeof(string), "InfoScript", ScriptHelper.GetScript("function IsCMSDesk() { return true; }"));
    }


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        Control.OnTabCreated += OnTabCreated;
    }


    void OnTabCreated(object sender, TabCreatedEventArgs e)
    {
        if (e.Tab == null)
        {
            return;
        }

        var tab = e.Tab;

        switch (tab.TabName.ToLowerCSafe())
        {
            case "webpart.theme":
                var wpi = Control.UIContext.EditedObject as WebPartInfo;

                if ((wpi != null) && StorageHelper.IsExternalStorage(wpi.GetThemePath()))
                {
                    e.Tab = null;
                }
                break;
        }
    }
}
