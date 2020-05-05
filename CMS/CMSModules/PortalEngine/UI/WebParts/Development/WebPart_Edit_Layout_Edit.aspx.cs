using System;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Layout_Edit : CMSModalPage
{
    protected override void OnPreInit(EventArgs e)
    {
        RequiresDialog = QueryHelper.GetBoolean("editonlycode", false);

        if (RequiresDialog)
        {
            QueryHelper.ValidateHash("hash", "objectid");
        }
        else
        {
            CheckGlobalAdministrator();
        }

        base.OnPreInit(e);
    }


    protected override void CreateChildControls()
    {
        ucHierarchy.DialogMode = RequiresDialog;

        int layoutId = QueryHelper.GetInteger("layoutid", 0);
        WebPartLayoutInfo wpli = WebPartLayoutInfoProvider.GetWebPartLayoutInfo(layoutId);
        if (wpli != null)
        {
            ucHierarchy.PreviewObjectName = wpli.WebPartLayoutCodeName;
            ucHierarchy.PreviewURLSuffix = "&previewobjectidentifier=" + wpli.WebPartLayoutCodeName;
        }
        else
        {
            ShowError(GetString("editedobject.notexists"));
        }

        if (layoutId != 0)
        {
            EditedObject = wpli;
        }

        base.CreateChildControls();
    }
}
