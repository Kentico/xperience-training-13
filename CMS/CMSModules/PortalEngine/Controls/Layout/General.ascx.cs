using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Layout_General : CMSPreviewControl
{
    #region "Variables"

    protected string aliasPath = QueryHelper.GetString("aliaspath", "");
    protected string webpartId = QueryHelper.GetString("webpartid", "");
    protected string zoneId = QueryHelper.GetString("zoneid", "");
    protected Guid instanceGuid = QueryHelper.GetGuid("instanceguid", Guid.Empty);
    protected int templateId = QueryHelper.GetInteger("templateid", 0);
    protected bool isNewVariant = QueryHelper.GetBoolean("isnewvariant", false);
    protected int variantId = QueryHelper.GetInteger("variantid", 0);
    protected int zoneVariantId = QueryHelper.GetInteger("zonevariantid", 0);
    protected string culture = QueryHelper.GetString("culture", LocalizationContext.PreferredCultureCode);
    protected VariantModeEnum variantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", string.Empty));
    protected bool isSiteManager = false;
    bool isNew;
    bool isDefault;

    CurrentUserInfo currentUser;

    private WebPartInfo webPartInfo;

    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti;


    WebPartLayoutInfo wpli;
    WebPartInfo wpi;

    /// <summary>
    /// Current web part.
    /// </summary>
    private WebPartInstance webPart;

    private string mLayoutCodeName;
    bool previewIsActive;
    int layoutID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets code name of edited layout.
    /// </summary>
    private string LayoutCodeName
    {
        get
        {
            return mLayoutCodeName ?? (mLayoutCodeName = QueryHelper.GetString("layoutcodename", string.Empty));
        }
        set
        {
            mLayoutCodeName = value;
        }
    }


    /// <summary>
    /// Returns true if object is checked out or checkin/out is not used 
    /// </summary>
    public bool IsChecked
    {
        get
        {
            CMSObjectManager om = CMSObjectManager.GetCurrent(Page);
            if (om != null)
            {
                return om.IsObjectChecked();
            }

            return false;
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


    #region "Control events"

    /// <summary>
    /// Handles form's after data load event.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        etaCode.Language = LanguageEnum.HTML;

        cssLayoutEditor.Editor.Language = LanguageEnum.CSS;
        cssLayoutEditor.Editor.ShowBookmarks = true;

        // Do not check changes
        DocumentManager.RegisterSaveChangesScript = false;

        EditForm.OnBeforeSave += EditForm_OnBeforeSave;

        etaCode.Language = LanguageEnum.HTML;

        wpli = UIContext.EditedObject as WebPartLayoutInfo;

        layoutID = QueryHelper.GetInteger("layoutid", 0);

        isSiteManager = ((MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) && layoutID != 0) || QueryHelper.GetBoolean("sitemanager", false));
        isNew = (LayoutCodeName == "|new|");
        isDefault = (LayoutCodeName == "|default|") || (!isSiteManager && string.IsNullOrEmpty(LayoutCodeName));

        if ((wpli == null) || (wpli.WebPartLayoutID <= 0))
        {
            isNew |= isSiteManager;
            editMenuElem.ObjectManager.ObjectType = WebPartLayoutInfo.OBJECT_TYPE;
        }

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; } " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));

        currentUser = MembershipContext.AuthenticatedUser;

        // Get web part instance (if edited in administration)
        if ((webpartId != "") && !isSiteManager)
        {
            // Get page info
            pi = CMSWebPartPropertiesPage.GetPageInfo(aliasPath, templateId, culture);
            if (pi == null)
            {
                ShowInformation(GetString("WebPartProperties.WebPartNotFound"), persistent: false);
            }
            else
            {
                // Get page template
                pti = pi.UsedPageTemplateInfo;
                if ((pti != null) && ((pti.TemplateInstance != null)))
                {
                    webPart = pti.TemplateInstance.GetWebPart(instanceGuid, zoneVariantId, variantId) ?? pti.TemplateInstance.GetWebPart(webpartId);
                }
            }
        }

        // If the web part is not found, try web part ID
        if (webPart == null)
        {
            wpi = WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(webpartId, 0));
            if (wpi == null)
            {
                ShowError(GetString("WebPartProperties.WebPartNotFound"));
                return;
            }
        }
        else
        {
            // CMS desk
            wpi = WebPartInfoProvider.GetWebPartInfo(webPart.WebPartType);
            if (string.IsNullOrEmpty(LayoutCodeName))
            {
                // Get the current layout name
                LayoutCodeName = ValidationHelper.GetString(webPart.GetValue("WebPartLayout"), "");
            }
        }

        if (wpi != null)
        {
            // Load the web part information
            webPartInfo = wpi;
            bool loaded = false;

            if (!RequestHelper.IsPostBack())
            {
                if (wpli != null)
                {
                    editMenuElem.MenuPanel.Visible = true;

                    // Read-only code text area
                    etaCode.Editor.ReadOnly = false;
                    loaded = true;
                }

                if (!loaded)
                {
                    string fileName = WebPartInfoProvider.GetFullPhysicalPath(webPartInfo);

                    // Check if filename exist
                    if (!FileHelper.FileExists(fileName))
                    {
                        ShowError(GetString("WebPartProperties.FileNotExist"));
                        pnlContent.Visible = false;
                        editMenuElem.ObjectEditMenu.Visible = false;
                    }
                    else
                    {
                        // Load default web part layout code
                        etaCode.Text = File.ReadAllText(Server.MapPath(fileName));

                        // Load default web part CSS
                        cssLayoutEditor.Text = wpi.WebPartCSS;
                    }
                }
            }
        }

        if (((wpli == null) || (wpli.WebPartLayoutID <= 0)) && isSiteManager)
        {
            editMenuElem.Title.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("WebParts.Layout"),
                RedirectUrl = String.Format("{0}&parentobjectid={1}&displaytitle={2}", UIContextHelper.GetElementUrl("CMS.Design", "WebPart.Layout"), QueryHelper.GetInteger("webpartid", 0), false)
            });

            editMenuElem.Title.Breadcrumbs.AddBreadcrumb(new BreadcrumbItem
            {
                Text = GetString("webparts_layout_newlayout"),
            });
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ApplyButton", ScriptHelper.GetScript(
            "function SetRefresh(refreshpage) { document.getElementById('" + hidRefresh.ClientID + @"').value = refreshpage; }
             function OnApplyButton(refreshpage) { SetRefresh(refreshpage); actionPerformed('save');refreshPreview(); }  
             function OnOKButton(refreshpage) { SetRefresh(refreshpage); actionPerformed('saveandclose'); } "));

        InitLayoutForm();
    }


    /// <summary>
    /// OnLoad override, setup access denied page with dependence on current usage.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        if (ShowPreview)
        {
            previewIsActive = (GetPreviewStateFromCookies(WEBPARTLAYOUT) > 0);
        }

        if (previewIsActive)
        {
            etaCode.TopOffset = 40;
        }

        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnBeforeAction += ObjectManager_OnBeforeAction;
        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        editMenuElem.ObjectManager.OnSaveData += ObjectManager_OnSaveData;

        // Hide submit button of the form
        EditForm.SubmitButton.Visible = false;

        base.OnLoad(e);
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        bool startWithFullScreen = previewIsActive && editMenuElem.ObjectManager.IsObjectChecked();

        pnlBody.Attributes["style"] = startWithFullScreen ? "display:none" : "display:block";

        // Check whether virtual objects are allowed
        if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"));
        }

        RegisterInitScripts(pnlBody.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);
    }


    /// <summary>
    /// Handles the OnBeforeAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnBeforeAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if ((e.ActionName == ComponentEvents.SAVE) && (webPart != null) && isDefault)
        {
            if (!isSiteManager)
            {
                SetCurrentLayout(true);

                // Reload the parent page after save
                EnsureParentPageRefresh();

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    // If window to close, register close script
                    CloseDialog();
                }
            }

            ShowChangesSaved();
            // Do not save default layout
            e.IsValid = false;
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        wpli = EditForm.EditedObject as WebPartLayoutInfo;

        if ((wpli == null) || (wpli.WebPartLayoutID <= 0) || (!e.IsValid))
        {
            // Do not continue if the object has not been created
            return;
        }

        LayoutCodeName = wpli.WebPartLayoutCodeName;

        if (e.ActionName == ComponentEvents.SAVE)
        {
            if (EditForm.ValidateData())
            {
                if (!isSiteManager)
                {
                    SetCurrentLayout(true);
                }

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    // If window to close, register close script
                    CloseDialog();
                }
                else
                {
                    // Redirect parent for new
                    if (isNew)
                    {
                        if (isSiteManager)
                        {
                            URLHelper.Redirect(String.Format("{0}&parentobjectid={1}&objectid={2}&displaytitle={3}",
                                UIContextHelper.GetElementUrl("CMS.Design", "Edit.WebPartLayout"), webPartInfo.WebPartID, wpli.WebPartLayoutID, false));
                        }
                        else
                        {
                            var codeName = (wpli != null) ? wpli.WebPartLayoutCodeName : string.Empty;
                            var redirectUrl = ResolveUrl("~/CMSModules/PortalEngine/UI/WebParts/WebPartProperties_layout_frameset.aspx") + URLHelper.UpdateParameterInUrl(RequestContext.CurrentQueryString, "layoutcodename", codeName);
                            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "RefreshHeader", ScriptHelper.GetScript("parent.location ='" + redirectUrl + "'"));

                            // Reload the parent page after save
                            EnsureParentPageRefresh();
                        }
                    }
                    else
                    {
                        if (!isSiteManager)
                        {
                            // Reload the parent page after save
                            EnsureParentPageRefresh();
                        }

                        // If all ok show changes saved
                        RegisterRefreshScript();
                    }
                }
            }

            // Clear warning text
            editMenuElem.MessagesPlaceHolder.WarningText = "";
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

        if (isSiteManager && (e.ActionName != ComponentEvents.CHECKOUT) && EditForm.DisplayNameChanged)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "RefreshBreadcrumbs", ScriptHelper.GetScript("if (parent.refreshBreadcrumbs != null && parent.document.pageLoaded) {parent.refreshBreadcrumbs('" + ResHelper.LocalizeString(EditForm.EditedObject.Generalized.ObjectDisplayName) + "')}"));
        }
    }


    /// <summary>
    /// Handles the OnBeforeSave event of the EditForm control.
    /// </summary>
    protected void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Remove "." due to virtual path provider replacement            
        txtCodeName.Text = txtCodeName.Text.Replace(".", "");

        WebPartLayoutInfo li = EditForm.EditedObject as WebPartLayoutInfo;
        if (li != null)
        {
            if (webPartInfo != null)
            {
                li.WebPartLayoutWebPartID = webPartInfo.WebPartID;
            }
        }
    }


    /// <summary>
    /// Handles the OnSaveData event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        // Enclose the standard save action into try/catch block to handle a possible exception
        try
        {
            // Standard save action
            ComponentEvents.RequestEvents.RaiseComponentEvent(this, e, ComponentName, DocumentComponentEvents.SAVE_DATA, e.ActionName);
        }
        catch (WebPartLayoutException ex)
        {
            // Handle exception -> display error message
            ShowError(ex.Message);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Selected index changed.
    /// </summary>
    private void InitLayoutForm()
    {
        if ((wpli == null) || (wpli.WebPartLayoutID <= 0))
        {
            if (!RequestHelper.IsPostBack())
            {
                // Prefill with default layout
                etaCode.Text = GetDefaultCode();
            }

            // Do not display the layout control in a preview mode for a new or default layout
            previewIsActive = false;

            if ((LayoutCodeName == "|new|") || isSiteManager)
            {
                // New layout
                plcDescription.Visible = true;
                plcValues.Visible = true;
                etaCode.Editor.ReadOnly = false;
                cssLayoutEditor.Editor.ReadOnly = false;
                etaCode.Editor.Height = Unit.Pixel(300);
                editMenuElem.MenuPanel.Visible = true;

                if (!RequestHelper.IsPostBack())
                {
                    cssLayoutEditor.Text = webPartInfo.WebPartCSS;
                }
            }
            else
            {
                // Default layout
                etaCode.Editor.ReadOnly = true;
                cssLayoutEditor.Button.Enabled = false;
                cssLayoutEditor.Editor.ReadOnly = true;
                editMenuElem.MenuPanel.Visible = false;
                pnlFormArea.Attributes["style"] = "";

                pnlFormArea.CssClass = "";
                etaCode.Editor.Height = Unit.Pixel(300);
            }
        }
        else
        {
            etaCode.Editor.Height = Unit.Pixel(300);
            plcDescription.Visible = true;
            plcValues.Visible = (layoutID != 0);
            etaCode.Editor.ReadOnly = false;
            editMenuElem.MenuPanel.Visible = true;

            if (wpli != null)
            {
                cssLayoutEditor.Editor.ReadOnly = false;
            }
        }

        // Display the "IsDefault" checkbox for inherited webparts
        plcIsDefault.Visible = (webPartInfo != null) && (webPartInfo.WebPartParentID > 0);

        // Ensure that hidden fields are truly hidden and not validated
        foreach (var field in EditForm.Fields)
        {
            var formControl = EditForm.FieldControls[field];
            if (!formControl.Visible)
            {
                EditForm.FieldsToHide.Add(field);
            }
        }
    }


    /// <summary>
    /// Gets the default layout code for the web part
    /// </summary>
    protected string GetDefaultCode()
    {
        string fileFullPath = WebPartInfoProvider.GetFullPhysicalPath(webPartInfo);
        if (File.Exists(fileFullPath))
        {
            return File.ReadAllText(fileFullPath);
        }

        return null;
    }


    /// <summary>
    /// Sets current layout.
    /// </summary>
    protected void SetCurrentLayout(bool saveToWebPartInstance)
    {
        if ((webPart != null) && (LayoutCodeName != "|new|"))
        {
            if (saveToWebPartInstance)
            {
                if (LayoutCodeName == "|default|")
                {
                    webPart.SetValue("WebPartLayout", "");
                }
                else
                {
                    webPart.SetValue("WebPartLayout", LayoutCodeName);
                }

                bool isWebPartVariant = (variantId > 0) || (zoneVariantId > 0) || isNewVariant;
                if (!isWebPartVariant)
                {
                    // Update page template
                    PageTemplateInfoProvider.SetPageTemplateInfo(pti);
                }
                else
                {
                    // Save the variant properties
                    if ((webPart != null)
                        && (webPart.ParentZone != null)
                        && (webPart.ParentZone.ParentTemplateInstance != null)
                        && (webPart.ParentZone.ParentTemplateInstance.ParentPageTemplate != null))
                    {
                        XmlNode xmlWebParts = (zoneVariantId > 0) ? webPart.ParentZone.GetXmlNode() : webPart.GetXmlNode();

                        VariantHelper.SetVariantWebParts(webPart.VariantMode, (zoneVariantId > 0) ? zoneVariantId : variantId, xmlWebParts);

                        // The variants are cached -> Reload
                        webPart.ParentZone.ParentTemplateInstance.LoadVariants(true, VariantModeEnum.None);
                    }
                }
            }

            string parameters = aliasPath + "/" + zoneId + "/" + webpartId;
            string cacheName = "CMSVirtualWebParts|" + parameters.ToLowerCSafe().TrimStart('/');

            CacheHelper.Remove(cacheName);
        }
    }


    /// <summary>
    /// Registers the client script to close the dialog.
    /// </summary>
    private void CloseDialog()
    {
        bool refresh = ValidationHelper.GetBoolean(hidRefresh.Value, false);

        var script = new StringBuilder();
        if (refresh)
        {
            // Reload the parent page after save
            script.AppendLine("SendEvent('setrefreshpage');");
        }
        script.AppendLine("SendEvent('close');");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "closeLayoutDialog", script.ToString(), true);
    }


    /// <summary>
    /// Refreshes the parent page.
    /// </summary>
    private void EnsureParentPageRefresh()
    {
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "LayoutRefreshParent", "SendEvent('setrefreshpage');", true);
    }

    #endregion
}