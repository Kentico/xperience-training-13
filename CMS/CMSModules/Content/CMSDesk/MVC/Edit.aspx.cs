using System;
using System.Collections.Specialized;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.Internal;
using CMS.Core;
using CMS.DocumentEngine.Internal;
using CMS.DocumentEngine.PageBuilder;
using CMS.DocumentEngine.Web.UI.Internal;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_CMSDesk_MVC_Edit : CMSContentPage
{
    private bool dataPropagated;
    private string actionName;

    private readonly ITempPageBuilderWidgetsPropagator widgetsPropagator = Service.Resolve<ITempPageBuilderWidgetsPropagator>();
    private readonly TemporaryAttachmentsPropagator attachmentsPropagator = new TemporaryAttachmentsPropagator(SiteContext.CurrentSiteName);


    /// <summary>
    /// Identifies the instance of editing.
    /// </summary>
    public Guid InstanceGUID
    {
        get
        {
            Guid guid = ValidationHelper.GetGuid(ViewState["InstanceGUID"], Guid.Empty);
            if (guid == Guid.Empty)
            {
                guid = Guid.NewGuid();
                ViewState["InstanceGUID"] = guid;
            }
            return guid;
        }
        set
        {
            ViewState["InstanceGUID"] = value;
        }
    }


    /// <summary>
    /// Overridden messages placeholder for correct positioning
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
        set
        {
            plcMess = value;
        }
    }


    /// <summary>
    /// OnPreInit
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;
        DocumentManager.RedirectForNonExistingDocument = false;

        base.OnPreInit(e);
    }


    /// <summary>
    /// OnInit
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        PortalContext.ViewMode = ViewModeEnum.Edit;

        base.OnInit(e);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup Edit menu
        editMenu.NodeID = NodeID;
        editMenu.CultureCode = CultureCode;
        editMenu.UseSmallIcons = true;
        editMenu.IsLiveSite = false;

        // Bind external buttons
        var extensionTarget = editMenu as IExtensibleEditMenu;
        extensionTarget.InitializeExtenders("Content");

        EnsurSaveAsTemplateButton();

        editMenu.HeaderActions.OnGetActionScript += GetActionScript;
        ((CMSDocumentManager)DocumentManager).OnGetActionScript += GetActionScript;

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterJQuery(this);
    }


    private void EnsurSaveAsTemplateButton()
    {
        var action = new SaveAsTemplateButtonActionProvider().Get();
        if (action != null)
        {
            editMenu.MoreActionsButton.Visible = true;
            editMenu.MoreActionsButton.ToolTip = GetString("EditMenu.MoreActions");
            editMenu.MoreActionsButton.Actions.Add(action);
        }
    }


    private string GetActionScript(object sender, GetActionScriptEventArgs e)
    {
        if (IsSavePerformingAction(e.ActionName) && DocumentManager.AllowSave)
        {
            return $"window.CMS && window.CMS.PageBuilder && window.CMS.PageBuilder.save({ScriptHelper.GetString(e.OriginalScript)}); return false;";
        }

        return e.OriginalScript;
    }


    private bool IsSavePerformingAction(string actionName)
    {
        switch (actionName)
        {
            case ComponentEvents.SAVE:
            case DocumentComponentEvents.APPROVE:
            case DocumentComponentEvents.PUBLISH:
            case DocumentComponentEvents.ARCHIVE:
            case DocumentComponentEvents.REJECT:
            case DocumentComponentEvents.CHECKIN:
                return true;
        }

        return false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (TryGetUrl(out string url))
        {
            pageview.Src = "about:blank";
            RegisterMessagingScript(url);
        }
        else
        {
            pageview.Attributes.Add("src", url);
        }

        ScriptHelper.RegisterStartupScript(this, typeof(string), "pagesavedastemplate", ScriptHelper.GetScript($@"
            cmsrequire(['CMS/EventHub', 'CMS/MessageService'], function (eventHub, msgService) {{
                eventHub.subscribe('pagesavedastemplate', function () {{ 
                    msgService.showSuccess('{ ResHelper.GetString("pagetemplatesmvc.saveastemplate.succes") }');
                }});              
            }})"));

        RegisterCookiePolicyDetection();
    }


    private void RegisterMessagingScript(string url)
    {
        var uri = new Uri(url);
        var targetOrigin = uri.GetLeftPart(UriPartial.Authority);
        string moduleId = "CMS.Builder/PageBuilder/Messaging";
        var localizationProvider = Service.Resolve<IClientLocalizationProvider>();
        ScriptHelper.RegisterModule(this, "CMS/RegisterClientLocalization", localizationProvider.GetClientLocalization(moduleId));

        ScriptHelper.RegisterModule(this, "CMS.Builder/PageBuilder/Messaging", new
        {
            frameId = pageview.ClientID,
            frameUrl = url,
            guid = InstanceGUID,
            origin = targetOrigin,
            documentGuid = Node.DocumentGUID,
            applicationPath = SystemContext.ApplicationPath,
            mixedContentMessage = GetString("builder.ui.mixedcontenterrormessage"),
            // Delete displayed variants in session storage on full page refresh or for undo checkout action (variant can be removed)
            deleteDisplayedVariants = !RequestHelper.IsPostBack() || string.Equals(actionName, DocumentComponentEvents.UNDO_CHECKOUT, StringComparison.OrdinalIgnoreCase)
        });
    }


    private bool TryGetUrl(out string url)
    {
        try
        {
            var queryStringParameters = GetQueryStringParameters();
            var useReadonlyMode = !DocumentManager.AllowSave;
            url = new PreviewLinkGenerator(Node).GeneratePreviewModeUrl(MembershipContext.AuthenticatedUser.UserGUID, useReadonlyMode, true, queryStringParameters);
        }
        catch (InvalidOperationException ex)
        {
            LogAndShowError("PageEdit", "PreviewLinkGeneration", ex);
            url = null;
            return false;
        }

        if (url == null)
        {
            url = DocumentUIHelper.GetPageNotAvailableUrl();
            return false;
        }

        if (DocumentManager.AllowSave)
        {
            url = PageBuilderHelper.AddEditModeParameter(url);
        }

        if (dataPropagated)
        {
            url = PageBuilderHelper.AddClearPageCacheParameter(url);
        }

        return true;
    }


    private NameValueCollection GetQueryStringParameters()
    {
        NameValueCollection queryStringParameters = null;
        if (DocumentManager.AllowSave)
        {
            queryStringParameters = new NameValueCollection()
            {
                { PageBuilderHelper.EDITING_INSTANCE_QUERY_NAME, InstanceGUID.ToString() }
            };
        }

        return queryStringParameters;
    }


    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        actionName = e.ActionName;

        if (e.ActionName == DocumentComponentEvents.UNDO_CHECKOUT)
        {
            widgetsPropagator.Delete(InstanceGUID);
            return;
        }

        if (!IsSavePerformingAction(e.ActionName))
        {
            return;
        }

        if (!DocumentManager.SaveChanges)
        {
            return;
        }

        widgetsPropagator.Delete(InstanceGUID);
    }


    private void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        if (!DocumentManager.SaveChanges)
        {
            return;
        }

        widgetsPropagator.Propagate(e.Node, InstanceGUID);
        attachmentsPropagator.Propagate(e.Node, InstanceGUID);

        dataPropagated = true;
    }
}
