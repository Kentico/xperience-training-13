using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.UIControls;


[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.Content", UIElements = "CheckedOutDocs;CheckedOutDocuments")]
[UIElement(ModuleName.CONTENT, "CheckedOutDocs")]
public partial class CMSModules_MyDesk_CheckedOut_Documents : CMSContentManagementPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ucCheckedOut.SiteName = SiteContext.CurrentSite.SiteName;
    }
}