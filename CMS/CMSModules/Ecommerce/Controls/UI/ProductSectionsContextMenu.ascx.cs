using System;

using CMS.Base.Web.UI;


public partial class CMSModules_Ecommerce_Controls_UI_ProductSectionsContextMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Add "Section properties" menu item to context menu
        ContextMenuItem menuItem = new ContextMenuItem() { ID = "iSectionProperties", ResourceString = "general.edit" };
        menuItem.Attributes.Add("onclick", "SectionProperties(GetContextMenuParameter('nodeMenu'));");
        menuItem.AddCssClass("sectionPropertiesMenuItem");
        menuElem.AdditionalMenuItems.Controls.Add(menuItem);

        const string SCRIPT = @"// Section properties
function SectionProperties(nodeId) {
    if (!CheckChanges()) {
        return false;
    }
    if (!nodeId) {
        nodeId = GetSelectedNodeId();
    }
    if (nodeId != 0) {
        window.PerformContentRedirect('sectionEdit', '', nodeId, '');
        return true;
    }
    return false;
}";
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "SectionPropertiesFunction", ScriptHelper.GetScript(SCRIPT));
    }
}
