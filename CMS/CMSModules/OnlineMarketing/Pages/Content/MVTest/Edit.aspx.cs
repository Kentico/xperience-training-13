using System;

using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;


[EditedObject("om.mvtest", "objectID")]
[Security(Resource = "CMS.MVTest", UIElements = "MVTestListing;Detail;General")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTest_Edit : CMSMVTestPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ucDisabledModule.ParentPanel = pnlDisabled;
    }

    #endregion
}