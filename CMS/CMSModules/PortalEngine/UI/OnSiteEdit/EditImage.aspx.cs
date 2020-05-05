using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_OnSiteEdit_EditImage : CMSAbstractEditablePage
{
    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ucEditableImage.ViewMode = CheckPermissions();
        ucEditableImage.DataControl = CurrentWebPartInstance;
        ucEditableImage.CurrentPageInfo = CurrentPageInfo;
        ucEditableImage.SetupControl();

        // Web part property macros need to be resolved manually in the EditText dialog.
        // In portal engine, macro resolving is being handled by the CMSAbstractWebpart control which cannot be used in this page.
        ucEditableImage.OnGetValue += ucEditableText_OnGetValue;

        string title = GetString("Content.EditImageTitle");
        if (!String.IsNullOrEmpty(PageTitleSuffix))
        {
            title += " - " + HTMLHelper.HTMLEncode(PageTitleSuffix);
        }
        SetTitle(title);

        // Check whether control is initialized form ASPX page
        if (QueryHelper.GetBoolean("aspxc", false))
        {
            // Alternate text
            ucEditableImage.AlternateText = QueryHelper.GetString("at", ucEditableImage.AlternateText);

            // Image CSS class
            ucEditableImage.ImageCssClass = QueryHelper.GetString("icc", ucEditableImage.ImageCssClass);

            // Width/Height
            ucEditableImage.ImageHeight = QueryHelper.GetInteger("ih", ucEditableImage.ImageHeight);
            ucEditableImage.ImageWidth = QueryHelper.GetInteger("iw", ucEditableImage.ImageWidth);

            // Resizing values
            ucEditableImage.ResizeToHeight = QueryHelper.GetInteger("rth", ucEditableImage.ResizeToHeight);
            ucEditableImage.ResizeToWidth = QueryHelper.GetInteger("rtw", ucEditableImage.ResizeToWidth);
            ucEditableImage.ResizeToMaxSideSize = QueryHelper.GetInteger("rtmss", ucEditableImage.ResizeToMaxSideSize);
        }

        base.OnInit(e);

        CssRegistration.RegisterCssLink(Page, "Design", "OnSiteEdit.css");
        ScriptHelper.RegisterJQuery(Page);

        menuElem.ShowSave = false;
        menuElem.ShowSaveAndClose = true;
    }

    
    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Update the ViewMode in order to enable/disable the selector control (used for workflow actions).
        ucEditableImage.ViewMode = CheckPermissions();

        base.OnPreRender(e);
    }


    /// <summary>
    /// Loads editable image content
    /// </summary>
    /// <param name="content">Content</param>
    /// <param name="forceReload">Indicates whether content should be loaded always</param>
    public override void LoadContent(string content, bool forceReload)
    {
        ucEditableImage.LoadContent(content, forceReload);
    }


    /// <summary>
    /// Returns editable image content
    /// </summary>
    public override string GetContent()
    {
        return ucEditableImage.GetContent();
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
