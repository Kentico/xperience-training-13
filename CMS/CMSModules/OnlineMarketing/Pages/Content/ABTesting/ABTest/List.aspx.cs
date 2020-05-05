using System;

using CMS.Helpers;
using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;


[Security(Resource = "CMS.ABTest", UIElements = "ABTestListing")]
[UIElement("CMS.ABTest", "ABTestListing")]
public partial class CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_List : CMSABTestPage
{
    private const string SMART_TIP_IDENTIFIER = "howtovideo|abtest|mvclisting";


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ucDisabledModule.TestSettingKeys = "CMSABTestingEnabled";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ucDisabledModule.ParentPanel = pnlDisabled;

        InitTitle();
        InitSmartTip();
    }


    /// <summary>
    /// Sets title if not in content.
    /// </summary>
    private void InitTitle()
    {
        if (NodeID <= 0)
        {
            SetTitle(GetString("analytics_codename.abtests"));
        }
    }


    /// <summary>
    /// Initialize the smart tip with the how to video.
    /// Shows how to video.
    /// </summary>
    private void InitSmartTip()
    {
        tipHowToListing.ExpandedHeader = GetString("abtesting.howto.howtosetupabtest.title");
        tipHowToListing.CollapsedStateIdentifier = SMART_TIP_IDENTIFIER;
        tipHowToListing.Content = String.Format(GetString("abtesting.howto.howtosetupabtestmvc.text"), DocumentationHelper.GetDocumentationTopicUrl("ab_testing_mvc"));
    }
}