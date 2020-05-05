using System;
using System.Linq;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


#pragma warning disable BH3501 // UI Web Part must inherit from right class.
public partial class CMSModules_AdminControls_Controls_UIControls_HeaderActions : CMSAbstractLayoutWebPart
#pragma warning restore BH3501 // UI Web Part must inherit from right class.
{
    #region "Properties"

    /// <summary>
    /// Zone CSS class.
    /// </summary>
    public string ZoneCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZoneCSSClass"), "");
        }
        set
        {
            SetValue("ZoneCSSClass", value);
        }
    }
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        HeaderActions = headerElem;

        StartLayout();

        if (IsDesign)
        {
            Append("<table class=\"LayoutTable\" cellspacing=\"0\" width=\"100%\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\" colspan=\"2\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td>");
        }

        string cssclass = ZoneCSSClass;

        // Render the envelope if needed
        bool renderEnvelope = IsDesign || !String.IsNullOrEmpty(cssclass);
        if (renderEnvelope)
        {
            Append("<div");

            if (IsDesign)
            {
                Append(" id=\"", ShortClientID, "_env\"");
            }

            if (!String.IsNullOrEmpty(cssclass))
            {
                Append(" class=\"", cssclass, "\"");
            }

            Append(">");
        }

        // Add the zone
        AddZone(ID + "_zone", ID);

        if (renderEnvelope)
        {
            Append("</div>");
        }

        if (IsDesign)
        {
            Append("</td>");
            Append("</tr></table>");
        }

        FinishLayout();
    }
    
    #endregion
}
