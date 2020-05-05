using System;

using CMS.Base.Web.UI;
using CMS.OnlineMarketing.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


[EditedObject("om.abtest", "objectID")]
[Security(Resource = "CMS.ABTest", UIElements = "Settings")]
[Security(Resource = "CMS.ABTest", UIElements = "Detail")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_Tab_General : CMSABTestPage
{
    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        MessagesPlaceHolder = plcMess;

        ucDisabledModule.TestSettingKeys = "CMSABTestingEnabled";

        base.OnInit(e);
    }


    protected void Page_PreRender()
    {
        ScriptHelper.RegisterScriptFile(Page, "DesignMode/PortalManager.js");
    }

    #endregion
}