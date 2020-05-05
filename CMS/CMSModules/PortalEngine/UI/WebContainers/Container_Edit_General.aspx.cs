using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


[UIElement(ModuleName.DESIGN, "WebPartContainer.General")]
public partial class CMSModules_PortalEngine_UI_WebContainers_Container_Edit_General : CMSModalDesignPage
{
    #region "Variables"

    private bool dialogMode;
    private bool tabMode;

    private WebPartContainerInfo webPartContainer;

    #endregion


    #region "Methods"

    protected override void OnPreInit(EventArgs e)
    {
        RequireSite = false;

        dialogMode = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));
        tabMode = QueryHelper.GetBoolean("tabmode", false);

        if (dialogMode)
        {
            if (!QueryHelper.ValidateHash("hash", "objectid"))
            {
                URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext"));
            }
        }
        else
        {
            CheckGlobalAdministrator();
        }

        var containerId = QueryHelper.GetInteger("containerid", 0);
        webPartContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerId);

        if (webPartContainer == null)
        {
            string containerName = QueryHelper.GetString("name", null);
            webPartContainer = WebPartContainerInfoProvider.GetWebPartContainerInfo(containerName);
        }

        SetEditedObject(webPartContainer, null);

        base.OnPreInit(e);
    }


    protected override void CreateChildControls()
    {
        if (webPartContainer != null)
        {
            Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);

            UIContext.EditedObject = webPartContainer;
            ucHierarchy.PreviewObjectName = webPartContainer.ContainerName;
            ucHierarchy.ShowPanelSeparator = !dialogMode || (dialogMode && tabMode);
            ucHierarchy.IgnoreSessionValues = dialogMode;
            ucHierarchy.DialogMode = dialogMode;

            String containerName = webPartContainer != null ? webPartContainer.ContainerName : "";
            String parameter = instanceGuid != Guid.Empty ? "&previewguid=" + instanceGuid : "&previewobjectidentifier=" + containerName;
            ucHierarchy.PreviewURLSuffix = parameter;
        }

        base.CreateChildControls();
    }

    #endregion
}
