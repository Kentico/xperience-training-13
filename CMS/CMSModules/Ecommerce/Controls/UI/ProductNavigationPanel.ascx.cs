using System;

using CMS.Base.Web.UI;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_UI_ProductNavigationPanel : CMSUserControl
{
    #region "Properties"

    private int NodeID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("NodeID"), 0);
        }
        set
        {
            SetValue("NodeID", value);
        }
    }


    private string Culture
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Culture"), string.Empty);
        }
        set
        {
            SetValue("Culture", value);
        }
    }


    private int ExpandNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ExpandNodeID"), 0);
        }
        set
        {
            SetValue("ExpandNodeID", value);
        }
    }


    /// <summary>
    /// Gets the page. This control should be used only within ecommerce context.
    /// </summary>
    protected new CMSProductsPage Page
    {
        get
        {
            return base.Page as CMSProductsPage;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // Register script for closing product section editing UI
        string script = "function CancelSectionEdit() { if(GetSelectedMode() == 'sectionedit'){ SetMode('edit',true);}}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "beforenodeselection", ScriptHelper.GetScript(script));

        UIContext[UIContextDataItemName.SELECTEDCULTURE] = Culture;

        // Pass the parameters
        menu.Values.Add(new UILayoutValue("ShowModeMenu", false));

        tree.Values.AddRange(new[] { 
            new UILayoutValue("NodeID", NodeID), 
            new UILayoutValue("ExpandNodeID", ExpandNodeID), 
            new UILayoutValue("Culture", Culture), 
            new UILayoutValue("StartingAliasPath", Page.ProductsStartingPath),
            new UILayoutValue("AllowGlobalObjects", Page.AllowGlobalObjects), 
            new UILayoutValue("AllowProductsWithoutDocuments", ECommerceSettings.AllowProductsWithoutDocuments(SiteContext.CurrentSiteName)), 
            new UILayoutValue("DisplayProductsInSectionsTree", ECommerceSettings.DisplayProductsInSectionsTree(SiteContext.CurrentSiteName)), 
            new UILayoutValue("IsProductTree", true),
            new UILayoutValue("BeforeSelectNodeScript", "CancelSectionEdit();")});

        // Show pane only for multilingual site
        language.Visible = SiteContext.CurrentSite.HasMultipleCultures;
    }

    #endregion
}
