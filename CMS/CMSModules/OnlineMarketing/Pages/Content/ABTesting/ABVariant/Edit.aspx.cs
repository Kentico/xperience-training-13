using System;

using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;
using CMS.OnlineMarketing;

[Help("variant_edit")]
[EditedObject(ABVariantInfo.OBJECT_TYPE, "variantId")]
[ParentObject(ABTestInfo.OBJECT_TYPE, "abTestID")]
[Breadcrumb(0, ResourceString = "abtesting.variant.list", TargetUrl = "~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABVariant/List.aspx?objectID={?abTestID?}&nodeid={?nodeID?}")]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "abtesting.variant.new", NewObject = true)]
[Security(Resource = "CMS.ABTest", UIElements = "Variants")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABVariant_Edit : CMSABTestPage
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ucDisabledModule.ParentPanel = pnlDisabled;
    }

    #endregion
}