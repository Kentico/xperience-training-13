using System;

using CMS.Base.Web.UI;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_WebPartProperties_properties : CMSWebPartPropertiesPage
{
    #region "Methods"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        CurrentMaster.BodyClass += " WebpartProperties";
    }


    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Initialize the control
        webPartProperties.AliasPath = aliasPath;
        webPartProperties.CultureCode = cultureCode;
        webPartProperties.WebPartID = webpartId;
        webPartProperties.ZoneID = zoneId;
        webPartProperties.InstanceGUID = instanceGuid;
        webPartProperties.PageTemplateID = templateId;
        webPartProperties.IsNewWebPart = isNew;

        webPartProperties.Position = position;
        webPartProperties.PositionLeft = positionLeft;
        webPartProperties.PositionTop = positionTop;

        webPartProperties.IsNewVariant = isNewVariant;
        webPartProperties.VariantID = variantId;
        webPartProperties.ZoneVariantID = zoneVariantId;
        webPartProperties.VariantMode = variantMode;

        webPartProperties.LoadData();
    }


    /// <summary>
    /// Load event handler.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Register the OnSave event handler
        FramesManager.OnSave += (sender, args) => { return webPartProperties.OnSave(); };
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register progress script
        ScriptHelper.RegisterLoader(this.Page);
    }

    #endregion
}
