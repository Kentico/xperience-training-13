using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_Edit : CMSModalDesignPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "page_layouts_editing";

    #endregion


    #region "Variables"

    private bool? mDialogMode;
    private PageTemplateInfo mPageTemplate;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets the current page template info
    /// </summary>
    private PageTemplateInfo PageTemplate
    {
        get
        {
            return mPageTemplate ?? (mPageTemplate = PageTemplateInfoProvider.GetPageTemplateInfo(PageTemplateId));
        }
    }


    /// <summary>
    /// Gets the current page template ID
    /// </summary>
    private int PageTemplateId
    {
        get
        {
            return QueryHelper.GetInteger("templateid", 0);
        }

    }


    /// <summary>
    /// Gets the valut that indicates whether dialog mode is required
    /// </summary>
    private bool DialogMode
    {
        get
        {
            if (mDialogMode == null)
            {
                mDialogMode = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false)) && !QueryHelper.Contains("rootelementid");
            }
            return mDialogMode.Value;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        // Disable check save changes script
        DocumentManager.RegisterSaveChangesScript = false;

        // Ensure dialog master page in dialog mode
        EnsureDialogMasterPage();

        // Gets the edited object
        UIContext.EditedObject = GetEditedObject();

        base.OnPreInit(e);
    }


    /// <summary>
    /// Ensures master page for dialog mode
    /// </summary>
    private void EnsureDialogMasterPage()
    {
        if (DialogMode)
        {
            // Set proper master page for dialog mode
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master";
            PageTitle.HelpTopicName = HELP_TOPIC_LINK;

            if (!TabMode)
            {
                PageTitle.TitleText = GetTitleText();
            }
        }
    }


    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Hide 'Preview' button as it's no longer valid for MVC scenarios (see PLATFORM-15329)
        ucHierarchy.EnablePreview = false;

        // Register client ID for autoresize of codemirror
        ucHierarchy.RegisterEnvelopeClientID();

        if (DialogMode)
        {
            ucHierarchy.ShowPanelSeparator = TabMode;
            ucHierarchy.DialogMode = true;
            ucHierarchy.DisplayTitlePane = !TabMode;
            ucHierarchy.DisplayFooter = true;

            ScriptHelper.RegisterWOpenerScript(Page);
        }

        // For edit template in properties->template->edit template - (special dialog mode), ignore session and load alias path first
        // Or for dialog mode in site manager, set ignoresessionvalues to false
        ucHierarchy.IgnoreSessionValues = (DialogMode && !String.IsNullOrEmpty(QueryHelper.GetString("aliaspath", String.Empty))) || (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));
        ucHierarchy.StorePreviewScrollPosition = true;

        if ((PageTemplate != null) && (UIContext.EditedObject is PageTemplateInfo))
        {
            ucHierarchy.PreviewObjectName = PageTemplate.PageTemplateId.ToString();
            ucHierarchy.CookiesPreviewStateName = CMSPreviewControl.PAGETEMPLATELAYOUT;
        }
        else if (UIContext.EditedObject is LayoutInfo)
        {
            ucHierarchy.PreviewObjectName = (UIContext.EditedObject as LayoutInfo).LayoutCodeName;
            ucHierarchy.CookiesPreviewStateName = CMSPreviewControl.PAGELAYOUT;
        }
        else
        {
            ucHierarchy.CookiesPreviewStateName = CMSPreviewControl.DEVICELAYUOT;
        }

        // Pass the AllowTypeSwitching setting to the inner control
        ucHierarchy.PaneContent.Values.Add(new UILayoutValue("AllowTypeSwitching", QueryHelper.GetBoolean("allowswitch", false)));
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets the title text.
    /// </summary>
    private string GetTitleText()
    {
        return GetString(PageTemplateId > 0 ? "pagetemplate.layoutproperties" : "administration-pagelayout_new.editlayout");
    }


    /// <summary>
    /// Gets the edited object.
    /// </summary>
    private object GetEditedObject()
    {
        object editedObject = null;

        if (PageTemplate != null)
        {
            int newSharedLayoutId = QueryHelper.GetInteger("newshared", -1);
            int oldSharedLayoutId = QueryHelper.GetInteger("oldshared", -1);

            // Default state - no radio buttons used
            if ((newSharedLayoutId == -1) && (oldSharedLayoutId == -1))
            {
                editedObject = PageTemplate;
            }
            else
            {
                // If new shared layout is set, than it should be used as edited object
                // This happens when switched from custom to a shared layout
                if (newSharedLayoutId > 0)
                {
                    // Standard page layout
                    editedObject = LayoutInfoProvider.GetLayoutInfo(newSharedLayoutId);
                }
                // This means user switched from shared layout to custom
                else
                {
                    var layout = LayoutInfoProvider.GetLayoutInfo(oldSharedLayoutId);

                    // We have to work with the clone, because we need to change the data of the object 
                    // (copy from shared layout)
                    var pageTemplateClone = PageTemplate.Clone();

                    if (layout != null)
                    {
                        // Delete the shared layout
                        pageTemplateClone.LayoutID = 0;

                        // Copy values
                        pageTemplateClone.PageTemplateLayout = layout.LayoutCode;
                        pageTemplateClone.PageTemplateCSS = layout.LayoutCSS;
                        pageTemplateClone.PageTemplateLayoutType = layout.LayoutType;
                    }

                    editedObject = pageTemplateClone;
                }
            }
        }
        // Load the layout  object
        else
        {
            var layoutId = QueryHelper.GetInteger("layoutid", 0);
            if (layoutId > 0)
            {
                editedObject = LayoutInfoProvider.GetLayoutInfo(layoutId);
            }
        }

        return editedObject;
    }

    #endregion
}
