using System;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_WebContainers_General : CMSPreviewControl
{
    #region "Variables"

    private int previewState = 0;
    protected bool startWithFullScreen = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if the page is in tab mode.
    /// </summary>
    public virtual bool TabMode
    {
        get
        {
            return QueryHelper.GetBoolean("tabmode", false);
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        previewState = GetPreviewStateFromCookies(WEBPARTCONTAINER);

        // Add preview action
        HeaderAction preview = new HeaderAction
        {
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            Visible = (previewState == 0),
            Index = 1,
            Tooltip = GetString("preview.tooltip")
        };
        editMenuElem.ObjectEditMenu.AddExtraAction(preview);
        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;

#pragma warning disable CS0618 // Type or member is obsolete
        bool hide = !(BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
#pragma warning restore CS0618 // Type or member is obsolete
        if (hide)
        {
            pnlContainer.CssClass += " Hidden ";
        }

        // Register action script for dialog purposes
        if (DialogMode)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());
        RegisterInitScripts(pnlContainer.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);
    }


    private void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        codeElem.Editor.ParentElementID = ParentClientID;
    }


    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName == ComponentEvents.SAVE)
        {
            if (DialogMode)
            {
                string script = String.Empty;
                string selector = QueryHelper.GetControlClientId("selectorid", string.Empty);
                if (!string.IsNullOrEmpty(selector))
                {
                    // Selects newly created container in the UniSelector
                    script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{ wopener.US_SelectNewValue_{0}('{1}'); }}",
                        selector, QueryHelper.GetInteger("cssstylesheetid", -1));
                }

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    script += "; CloseDialog();";
                }

                if (script != String.Empty)
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "UpdateSelector", ScriptHelper.GetScript(script));
                }
            }

            RegisterRefreshScript();
        }

        if (DialogMode)
        {
            switch (e.ActionName)
            {
                case ComponentEvents.SAVE:
                case ComponentEvents.CHECKOUT:
                case ComponentEvents.UNDO_CHECKOUT:
                case ComponentEvents.CHECKIN:
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "wopenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
                    break;
            }
        }


        if ((e.ActionName != ComponentEvents.CHECKOUT) && EditForm.DisplayNameChanged)
        {
            ScriptHelper.RefreshTabHeader(Page, EditForm.EditedObject.Generalized.ObjectDisplayName);
        }
    }

    #endregion
}
