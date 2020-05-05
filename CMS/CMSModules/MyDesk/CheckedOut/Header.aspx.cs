using System;

using CMS.DataEngine;
using CMS.UIControls;


[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.Content", UIElements = "CheckedOutDocs")]
[Tabs("CMS.Content", "CheckedOutDocs", "content")]
public partial class CMSModules_MyDesk_CheckedOut_Header : CMSContentManagementPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var title = GetString("MyDesk.CheckedOutTitle");
        Title = title;
        PageTitle.TitleText = title;
    }
}
