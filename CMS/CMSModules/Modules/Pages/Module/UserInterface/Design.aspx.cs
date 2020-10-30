using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.UserInterface.Design")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_Design : CMSUIPage
{
    #region "Public properties"

    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            // Enable document manager
            docMan.Visible = true;
            docMan.StopProcessing = false;
            docMan.MessagesPlaceHolder.UseRelativePlaceHolder = false;
            return docMan;
        }
    }


    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return DocumentManager.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Init the page components
        manPortal.SetMainPagePlaceholder(plc);

        var ui = UIElementInfo.Provider.Get(QueryHelper.GetInteger("elementid", 0));

        // Clear UIContext data of element "Modules.UserInterface.Design" (put by UIElement attribute to check permissions)
        var ctx = pnlContext.UIContext;

        ctx.Data = null;
        ctx.HideControlOnError = false;

        if (ui != null)
        {
            ctx.UIElement = ui;

            // Store resource name
            ctx.ResourceName = ApplicationUrlHelper.GetResourceName(ui.ElementResourceID);

            // Provide empty object in case of editing
            if (!ui.RepresentsNew)
            {
                var objectType = UIContextHelper.GetObjectType(ctx);
                if (!String.IsNullOrEmpty(objectType))
                {
                    ctx.EditedObject = GetEmptyObject(objectType);
                }
            }
            
            int pageTemplateId = ui.ElementPageTemplateID;

            // If no page template is set, dont show any content
            if (pageTemplateId == 0)
            {
                RedirectToInformation(GetString("uielement.design.notemplate"));
            }

            DocumentContext.CurrentPageInfo = PageInfoProvider.GetVirtualPageInfo(pageTemplateId);

            // Set the design mode
            bool enable = (SystemContext.DevelopmentMode || (ui.ElementResourceID == QueryHelper.GetInteger("moduleId", 0) && ui.ElementIsCustom));
            PortalContext.SetRequestViewMode(ViewModeEnum.Design);

            // If displayed module is not selected, disable design mode
            if (!enable)
            {
                plc.ViewMode = ViewModeEnum.DesignDisabled;
            }

            RequestStockHelper.Add(CookieName.DisplayContentInDesignMode, PortalHelper.DisplayContentInUIElementDesignMode, true);

            ManagersContainer = plcManagers;
            ScriptManagerControl = manScript;
        }
    }


    private static BaseInfo GetEmptyObject(string objectType)
    {
        var infoObj = ModuleManager.GetReadOnlyObject(objectType);
        var ds = infoObj.Generalized.GetDataQuery(true, q => q.TopN(1), false).Result;

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            return ModuleManager.GetObject(ds.Tables[0].Rows[0], objectType);
        }
        return ModuleManager.GetObject(objectType);
    }

    #endregion
}