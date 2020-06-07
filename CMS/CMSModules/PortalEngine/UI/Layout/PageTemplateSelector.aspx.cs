using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_PortalEngine_UI_Layout_PageTemplateSelector : CMSModalPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		string selectorid = QueryHelper.GetString("selectorid", "");
		string selectedItem = QueryHelper.GetString("selectedPageTemplateId", "");
	    
		selectElem.DocumentID = QueryHelper.GetInteger("documentid", 0);

        // If document id is not defined try get id from nodeid if is available
        if (selectElem.DocumentID <= 0)
        {
            int nodeId = QueryHelper.GetInteger("nodeid", 0);
            if (nodeId > 0)
            {
                TreeProvider tp = new TreeProvider(MembershipContext.AuthenticatedUser);
                TreeNode tn = tp.SelectSingleNode(nodeId);
                if (tn != null)
                {
                    selectElem.DocumentID = tn.DocumentID;
                }
            }
        }
        selectElem.IsNewPage = QueryHelper.GetBoolean("isnewpage", false);

		// Proceeds the current item selection
		string javascript =
@"
function SelectCurrentPageTemplate()
{                      
	SelectPageTemplate(selectedValue);                
}

function SelectPageTemplate(value)
{                                
	if (value != null)
	{                                                            
		if (wopener.OnSelectPageTemplate)
		{                       
			wopener.OnSelectPageTemplate(value, " + ScriptHelper.GetString(selectorid) + @");
		}
		CloseDialog();       
	}
	else
	{
		alert(""" + GetString("PageTemplateSelection.NoPageTemplateSelected") + @""");		    
	}                
}            
"
		;

		ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "PageTemplateSelector", ScriptHelper.GetScript(javascript));

		// Set name of selection function for double click
		selectElem.SelectFunction = "SelectPageTemplate";

		int rootCategoryId = QueryHelper.GetInteger("rootcategoryid", 0);
		if (rootCategoryId > 0)
		{
			selectElem.RootCategoryID = rootCategoryId;
		}

		// Preset item
		if (!RequestHelper.IsPostBack())
		{
			selectElem.SelectedItem = selectedItem;

			// Selected category
			String selCat = QueryHelper.GetString("TreeSelectedCategory", String.Empty);
			if (selCat != String.Empty)
			{
				PageTemplateCategoryInfo cat = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(selCat);
				if (cat != null)
				{
					selectElem.TreeSelectedCategory = cat.CategoryId.ToString();
				}
			}
		}

		// Set the title and icon
		Page.Title = GetString("portalengine-PageTemplateSelection.title");

		PageTitle.TitleText = Page.Title;
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

		SetSaveJavascript("SelectCurrentPageTemplate();return false;");
		SetSaveResourceString("general.select");
	}
}
