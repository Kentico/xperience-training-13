using System;
using System.Linq;

using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;


[EditedObject("om.abtest", "objectID")]
[Action(0, ResourceString = "abtesting.variant.new", TargetUrl = "Edit.aspx?abtestID={%EditedObject.ID%}&nodeid={?nodeID?}")]
[Security(Resource = "CMS.ABTest", UIElements = "Variants")]
[Security(Resource = "CMS.ABTest", UIElements = "Detail")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_List : CMSABTestPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        pnlDisabled.Visible = true;

        ucDisabledModule.ParentPanel = pnlDisabled;
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Add Resource name and Permission to check user permissions
        var action = HeaderActions.ActionsList.FirstOrDefault();
        if (action != null)
        {
            action.ResourceName = "CMS.ABTest";
            action.Permission = "Manage"; 
        }
    }

    #endregion
}