using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

[assembly: RegisterCustomClass("UserInterfaceEditTabsControlExtender", typeof(UserInterfaceEditTabsControlExtender))]

/// <summary>
/// Permission list control extender
/// </summary>
public class UserInterfaceEditTabsControlExtender : UITabsExtender
{
    private Panel pnlContent;
    private CMSCheckBox chkContent;

    int designTab = -1;


    /// <summary>
    /// Initialization of tabs.
    /// </summary>
    public override void OnInitTabs()
    {
        AppendDesignContentCheckBox();
        Control.OnTabsLoaded += HandleContentCheckBox;
    }


    /// <summary>
    /// Registers the page scripts
    /// </summary>
    private void HandleContentCheckBox(List<UITabItem> items)
    {
        var script = $@"
var IsCMSDesk = true;

function ShowContent(show) {{
    document.getElementById('{pnlContent.ClientID}').style.display = show ? 'block' : 'none';
}}
";

        ScriptHelper.RegisterClientScriptBlock(Control.Page, typeof(string), "UserInterfaceEditTabsControlExtender", ScriptHelper.GetScript(script));

        UpdateTabs(items);
    }


    /// <summary>
    /// Append display content checkbox to tabs header
    /// </summary>
    private void AppendDesignContentCheckBox()
    {
        pnlContent = new Panel
        {
            ID = "pc",
            CssClass = "design-showcontent"
        };

        chkContent = new CMSCheckBox
        {
            Text = Control.GetString("EditTabs.DisplayContent"),
            ID = "chk",
            AutoPostBack = true,
            EnableViewState = false,
            Checked = PortalHelper.DisplayContentInUIElementDesignMode
        };
        chkContent.CheckedChanged += ContentCheckBoxCheckedChanged;

        pnlContent.Controls.Add(chkContent);

        Control.Parent.Controls.Add(pnlContent);
    }


    /// <summary>
    /// Modifies and loads the tabs so that clicking on the tab will show/hide the content checkbox
    /// </summary>
    private void UpdateTabs(IEnumerable<UITabItem> items)
    {
        int index = 0;

        foreach (var tab in items)
        {
            var isDesign = tab.TabName == "Modules.UserInterface.Design";
            var script = $"ShowContent({isDesign.ToString().ToLowerInvariant()})";

            if (isDesign)
            {
                designTab = index;
            }
            
            tab.OnClientClick = ScriptHelper.AddScript(tab.OnClientClick, script);

            index++;
        }
    }


    /// <summary>
    /// Fires when the show content checkbox changes
    /// </summary>
    private void ContentCheckBoxCheckedChanged(object sender, EventArgs e)
    {
        if (designTab >= 0)
        {
            Control.SelectedTab = designTab;
        }

        PortalHelper.DisplayContentInUIElementDesignMode = chkContent.Checked;
    }
}
