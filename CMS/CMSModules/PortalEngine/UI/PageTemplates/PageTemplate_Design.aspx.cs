using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_Design : PortalPage
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
            docMan.RegisterSaveChangesScript = (PortalContext.ViewMode.IsEdit());
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
        PageManager = manPortal;
        manPortal.SetMainPagePlaceholder(plc);

        int pageTemplateId = QueryHelper.GetInteger("templateid", 0);
        UIContext.EditedObject = PageTemplateInfoProvider.GetPageTemplateInfo(pageTemplateId);

        DocumentContext.CurrentPageInfo = PageInfoProvider.GetVirtualPageInfo(pageTemplateId);

        // Set the design mode
        PortalContext.SetRequestViewMode(ViewModeEnum.Design);
        RequestStockHelper.Add(CookieName.DisplayContentInDesignMode, "0", true);

        ManagersContainer = plcManagers;
        ScriptManagerControl = manScript;
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Init the header tags
        ltlTags.Text = HeaderTags;
    }

    #endregion
}
