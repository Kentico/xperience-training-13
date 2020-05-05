using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_FileSystemSelector_EditTextFile : CMSModalGlobalAdminPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register client ID for autoresize of codemirror
        ucHierarchy.RegisterEnvelopeClientID();
    }


    protected override void CreateChildControls()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        ucHierarchy.DisplayTitlePane = true;
        ucHierarchy.DialogMode = true;
        ucHierarchy.StorePreviewScrollPosition = true;
        ucHierarchy.PreviewObjectName = "cms.theme";

        base.CreateChildControls();
    }
}
