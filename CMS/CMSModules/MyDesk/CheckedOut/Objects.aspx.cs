using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.UIControls;


[CheckLicence(FeatureEnum.WorkflowVersioning)]
[Security(Resource = "CMS.Content", UIElements = "CheckedOutDocs;CheckedOutObjects")]
[UIElement(ModuleName.CONTENT, "CheckedOutDocs")]
public partial class CMSModules_MyDesk_CheckedOut_Objects : CMSContentManagementPage
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        ucDisabledModuleInfo.ParentPanel = pnlDisabled;
    }
}
