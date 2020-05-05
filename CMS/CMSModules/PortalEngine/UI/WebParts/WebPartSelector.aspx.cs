using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_WebPartSelector : CMSModalDesignPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// Set dialog body class
		CurrentMaster.PanelBody.CssClass = "DialogPageBody";
		selectElem.ShowInheritedWebparts = true;
		selectElem.ShowUIWebparts = QueryHelper.GetBoolean("isui", false);

		// Proceeds the current item selection
		string javascript =
@"
function SelectCurrentWebPart() 
{
	SelectWebPart(selectedValue, selectedSkipDialog);
}
function SelectWebPart(value, skipDialog)
{
	if ((value != null) && (value != ''))
	{
		if (wopener.OnSelectWebPart)
		{
			wopener.OnSelectWebPart(value, skipDialog);
		}	  
		CloseDialog();          
	}
	else
	{
		alert(document.getElementById('" + hdnMessage.ClientID + @"').value);		    
	}                
}            
// Cancel action
function Cancel()
{
	CloseDialog();
} ";

		ScriptHelper.RegisterStartupScript(this, typeof(string), "WebPartSelector", ScriptHelper.GetScript(javascript));

		// Set name of selection function
		selectElem.SelectFunction = "SelectWebPart";

		// Set the title and icon
		string title = GetString("portalengine-webpartselection.title");
		Page.Title = title;
		PageTitle.TitleText = title;
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

		SetSaveJavascript("SelectCurrentWebPart();return false;");
		SetSaveResourceString("general.select");
	}
}
