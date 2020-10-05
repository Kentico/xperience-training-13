using System;
using System.Data;

using CMS;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

[assembly: RegisterCustomClass("PageTemplateEditControlExtender", typeof(PageTemplateEditControlExtender))]

/// <summary>
/// Page template edit extender
/// </summary>
public class PageTemplateEditControlExtender : ControlExtender<UIForm>
{
    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAfterValidate += Control_OnAfterValidate;
    }


    /// <summary>
    /// Handles the OnAfterValidate event of the Control control.
    /// </summary>
    void Control_OnAfterValidate(object sender, EventArgs e)
    {
        PageTemplateInfo pti = Control.EditedObject as PageTemplateInfo;
        if (pti == null)
        {
            return;
        }

        String result = String.Empty;
        PageTemplateTypeEnum type = EnumStringRepresentationExtensions.ToEnum<PageTemplateTypeEnum>((Control.GetFieldValue("PageTemplateType") as String));

        // Check dashboard prerequisites
        if ((pti.PageTemplateId > 0) && (type == PageTemplateTypeEnum.Dashboard))
        {
            // Check valid zones
            PageTemplateInstance inst = pti.TemplateInstance;
            if (inst != null)
            {
                foreach (WebPartZoneInstance zone in inst.WebPartZones)
                {
                    switch (zone.WidgetZoneType)
                    {
                        case WidgetZoneTypeEnum.Dashboard:
                        case WidgetZoneTypeEnum.None:
                            continue;
                    }

                    result = ResHelper.GetString("template.dashboardinvalidzone");
                    break;
                }
            }
        }

        if (result != String.Empty)
        {
            Control.StopProcessing = true;
            CMSPage pg = Control.Page as CMSPage;
            if (pg != null)
            {
                pg.ShowError(result);
                Control.StopProcessing = true;
            }
        }
    }
}

