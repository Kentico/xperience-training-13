using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_OnlineMarketing_Controls_WebParts_WebPartZonePersonalized : CMSUserControl, IWebPartZoneProperties
{
    #region "Variables"

    protected string zoneId = QueryHelper.GetString("zoneid", "");
    protected int templateId = QueryHelper.GetInteger("templateid", 0);
    protected int variantId = QueryHelper.GetInteger("variantid", 0);
    protected VariantModeEnum variantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", string.Empty));

    #endregion


    #region "Page methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // When displaying an existing variant of a zone, get the variant mode for its original zone
        if (variantId > 0)
        {
            PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(templateId);
            if ((pti != null) && ((pti.TemplateInstance != null)))
            {
                // Get the original zone and retrieve its variant mode
                WebPartZoneInstance zoneInstance = pti.TemplateInstance.GetZone(zoneId);
                if ((zoneInstance != null) && (zoneInstance.VariantMode != VariantModeEnum.None))
                {
                    variantMode = zoneInstance.VariantMode;
                }
            }
        }

        if (variantMode == VariantModeEnum.MVT)
        {
            // Display MVT edit dialog
            mvtEditElem.UIFormControl.EditedObject = ProviderHelper.GetInfoById(MVTVariantInfo.OBJECT_TYPE, QueryHelper.GetInteger("variantid", 0));
            mvtEditElem.UIFormControl.ParentObject = ProviderHelper.GetInfoById(PageTemplateInfo.OBJECT_TYPE, QueryHelper.GetInteger("templateid", 0));
            mvtEditElem.Visible = true;
            mvtEditElem.UIFormControl.SubmitButton.Visible = false;
            mvtEditElem.UIFormControl.ReloadData();
        }
        else if (variantMode == VariantModeEnum.ContentPersonalization)
        {
            // Display Content personalization edit dialog
            cpEditElem.UIFormControl.EditedObject = ProviderHelper.GetInfoById(ContentPersonalizationVariantInfo.OBJECT_TYPE, QueryHelper.GetInteger("variantid", 0));
            cpEditElem.UIFormControl.ParentObject = ProviderHelper.GetInfoById(PageTemplateInfo.OBJECT_TYPE, QueryHelper.GetInteger("templateid", 0));
            cpEditElem.Visible = true;
            cpEditElem.UIFormControl.SubmitButton.Visible = false;
            cpEditElem.UIFormControl.ReloadData();
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Saves the zone variant.
    /// </summary>
    public bool Save()
    {
        bool result = false;
        if (variantMode == VariantModeEnum.MVT)
        {
            result = mvtEditElem.UIFormControl.SaveData(null);
        }
        else if (variantMode == VariantModeEnum.ContentPersonalization)
        {
            if (cpEditElem.ValidateData())
            {
                result = cpEditElem.UIFormControl.SaveData(null);
            }
        }
        if (result)
        {
            cpEditElem.UIFormControl.ShowChangesSaved();
        }

        return result;
    }

    #endregion
}