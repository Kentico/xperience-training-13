using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Widgets_Dialogs_WidgetSelector : CMSModalPage
{
    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Public user is not allowed for widgets
        if (!AuthenticationHelper.IsAuthenticated())
        {
            RedirectToAccessDenied(GetString("widgets.security.notallowed"));
        }

        PortalContext.SetRequestViewMode(ViewModeEnum.Design);

        ScriptHelper.RegisterWOpenerScript(Page);

        btnOk.OnClientClick = "SelectCurrentWidget(); return false;";

        // Base tag is added in master page
        AddBaseTag = false;

        // Proceeds the current item selection
        StringBuilder script = new StringBuilder();
        script.Append(@"
function SelectCurrentWidget() 
{                
    SelectWidget(selectedValue, selectedSkipDialog);
}
function SelectWidget(value, skipDialog)
{
    if ((value != null) && (value != ''))
    {");

        script.Append(@"
	    if (wopener.OnSelectWidget)
        {                    
            wopener.OnSelectWidget(value, skipDialog);                      
        }

        CloseDialog();");

        script.Append(@"  
	}
	else
	{
        alert(document.getElementById('", hdnMessage.ClientID, @"').value);		    
	}                
}            
// Cancel action
function Cancel()
{
    CloseDialog(false);
} ");

        ScriptHelper.RegisterStartupScript(this, typeof(string), "WidgetSelector", script.ToString(), true);
        selectElem.SelectFunction = "SelectWidget";
        selectElem.IsLiveSite = false;

        // Set the title and icon
        PageTitle.TitleText = GetString("widgets.selectortitle");
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");

        // Remove default css class
        if (CurrentMaster.PanelBody != null)
        {
            Panel pnl = CurrentMaster.PanelBody.FindControl("pnlContent") as Panel;
            if (pnl != null)
            {
                pnl.CssClass = String.Empty;
            }
        }
    }

    #endregion
}
