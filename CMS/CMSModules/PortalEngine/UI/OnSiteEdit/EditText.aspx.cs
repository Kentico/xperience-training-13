using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_OnSiteEdit_EditText : CMSAbstractEditablePage
{
    public override MessagesPlaceHolder MessagesPlaceHolder => pnlMessagePlaceholder;


    protected override void OnInit(EventArgs e)
    {
        DocumentManager.OnValidateData += DocumentManager_OnValidateData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        // Web part property macros need to be resolved manually in the EditText dialog.
        // In portal engine, macro resolving is being handled by the CMSAbstractWebpart control which cannot be used in this page.
        ucEditableText.OnGetValue += ucEditableText_OnGetValue;

        // Process ASPX template parameters
        if (QueryHelper.Contains("regiontype"))
        {
            ucEditableText.RegionType = CMSEditableRegionTypeEnumFunctions.GetRegionTypeEnum(QueryHelper.GetString("regiontype", string.Empty));
            if (ucEditableText.RegionType == CMSEditableRegionTypeEnum.HtmlEditor)
            {
                // HtmlEditor needs toolbar location defined (due to toolbar positioning and editing area padding)
                ucEditableText.HtmlAreaToolbarLocation = "Out:CKToolbar";
            }

            // Min/Max length
            ucEditableText.MaxLength = QueryHelper.GetInteger("maxl", ucEditableText.MaxLength);
            ucEditableText.MinLength = QueryHelper.GetInteger("minl", ucEditableText.MinLength);

            // Word wrap
            ucEditableText.WordWrap = QueryHelper.GetBoolean("wordwrap", ucEditableText.WordWrap);

            // Upload image dimensions
            ucEditableText.ResizeToHeight = QueryHelper.GetInteger("resizetoheight", ucEditableText.ResizeToHeight);
            ucEditableText.ResizeToWidth = QueryHelper.GetInteger("resizetowidth", ucEditableText.ResizeToHeight);
            ucEditableText.ResizeToMaxSideSize = QueryHelper.GetInteger("resizetomaxsidesize", ucEditableText.ResizeToHeight);

            // Toolbar set
            ucEditableText.HtmlAreaToolbar = QueryHelper.GetString("toolbarset", ucEditableText.HtmlAreaToolbar);
        }

        ucEditableText.ViewMode = CheckPermissions();
        ucEditableText.DataControl = CurrentWebPartInstance;
        ucEditableText.CurrentPageInfo = CurrentPageInfo;
        ucEditableText.SetupControl();

        CurrentMaster.FooterContainer.Visible = false;
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");

        string title = GetString("Content.EditTextTitle");
        if (!String.IsNullOrEmpty(PageTitleSuffix))
        {
            title += " - " + HTMLHelper.HTMLEncode(PageTitleSuffix);
        }
        SetTitle(title);

        base.OnInit(e);

        CssRegistration.RegisterCssLink(Page, "Design", "OnSiteEdit.css");
        ScriptHelper.RegisterJQuery(Page);

        menuElem.ShowSaveAndClose = true;

        String resize = @"function resizeEditor() {$cmsj('.cke_contents').height($cmsj(window).height()-$cmsj('.DialogsPageHeader').height() - $cmsj('.cke_top').height() - $cmsj('.cke_bottom').height() - 20) }
        $cmsj(window).resize(function() {resizeEditor()});";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(Page), "resizeHeight", ScriptHelper.GetScript(resize));

        if (ucEditableText.RegionType == CMSEditableRegionTypeEnum.TextArea)
        {
            const string resizeScript = @"
            var resizeTextAreaTimer;

            // DOM ready
            $cmsj(document).ready( function() { ResizeEditableArea(200); });

            // Window resize
            $cmsj(window).resize(function () { ResizeEditableArea(100); });

            function ResizeEditableArea(timeout) {
                clearTimeout(resizeTextAreaTimer);
                resizeTextAreaTimer = window.setTimeout(function () {
                    var textarea = $cmsj('.EditableTextTextBox');
                    var editableTextContainer = $cmsj('.EditableTextContainer');
                    var editableTextEdit = $cmsj('.EditableTextEdit');
                    var borderMargin1 = textarea.outerHeight(true) - textarea.height();
                    var borderMargin2 = editableTextEdit.outerHeight(true) - editableTextEdit.height();
                    var borderMargin3 = editableTextContainer.outerHeight(true) - editableTextContainer.height();
                    var height = $cmsj('.dialog-content').height() - borderMargin1 - borderMargin2 - borderMargin3;
                    textarea.height(height);
             }, timeout); }";


            ScriptHelper.RegisterClientScriptBlock(this, typeof(Page), "ResizeEditableArea", ScriptHelper.GetScript(resizeScript));
        }
    }


    /// <summary>
    /// Load content
    /// </summary>
    public override void LoadContent(string content, bool forceReload)
    {
        ucEditableText.LoadContent(content, true);

        // Use always inline toolbar and resize height after editor is initialized
        if (ucEditableText.Editor != null)
        {
            ucEditableText.Editor.ToolbarLocation = "CKToolbar";
            ucEditableText.Editor.Config["on"] = "{ 'instanceReady' : function(e) { resizeEditor ();  } }";
        }
    }


    /// <summary>
    /// Get content
    /// </summary>
    public override string GetContent()
    {
        // Return editable content when valid
        if (ucEditableText.IsValid())
        {
            return ucEditableText.GetContent();
        }
        else
        {
            ShowError(ucEditableText.ErrorMessage);
        }

        return null;
    }


    /// <summary>
    /// Handles the OnValidateData event of the DocumentManager control.
    /// </summary>
    protected void DocumentManager_OnValidateData(object sender, DocumentManagerEventArgs e)
    {
        if (!ucEditableText.IsValid())
        {
            // Set the error message when an error occurs
            e.IsValid = false;
            e.ErrorMessage = ucEditableText.ErrorMessage;
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the DocumentManager control.
    /// </summary>
    protected void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Update the ViewMode in order to enable/disable the edit text control (used for workflow actions).
        ucEditableText.ViewMode = CheckPermissions();

        // Refresh the page after action
        switch (e.ActionName)
        {
            case DocumentComponentEvents.UNDO_CHECKOUT:
            case DocumentComponentEvents.CHECKOUT:
            case DocumentComponentEvents.CHECKIN:
            case DocumentComponentEvents.CREATE_VERSION:
            case DocumentComponentEvents.PUBLISH:
            case DocumentComponentEvents.APPROVE:
            case DocumentComponentEvents.REJECT:
                URLHelper.RefreshCurrentPage();
                break;
        }

    }


    /// <summary>
    /// Resolve the web part property macro.
    /// </summary>
    /// <param name="value">The web part property value</param>
    /// <returns>Returns resolved web part property.</returns>
    private object ucEditableText_OnGetValue(object value)
    {
        return ResolveMacros(value);
    }
}