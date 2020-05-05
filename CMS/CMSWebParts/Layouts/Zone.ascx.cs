using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_Zone : CMSAbstractLayoutWebPart
{
    #region "Properties"

    /// <summary>
    /// Width.
    /// </summary>
    public string Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Width"), "");
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Height.
    /// </summary>
    public string Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Height"), "");
        }
        set
        {
            SetValue("Height", value);
        }
    }


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
        StartLayout();

        if (IsDesign)
        {
            Append("<table class=\"LayoutTable\" cellspacing=\"0\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\" colspan=\"2\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td>");
        }

        string style = null;

        // Width
        string width = Width;
        if (!String.IsNullOrEmpty(width))
        {
            style += " width: " + width + ";";
        }

        // Height
        string height = Height;
        if (!String.IsNullOrEmpty(height))
        {
            style += " height: " + height + ";";
        }

        string cssclass = ZoneCSSClass;

        // Render the envelope if needed
        bool renderEnvelope = IsDesign || !String.IsNullOrEmpty(style) || !String.IsNullOrEmpty(cssclass);
        if (renderEnvelope)
        {
            Append("<div");

            if (IsDesign)
            {
                Append(" id=\"", ShortClientID, "_env\"");
            }

            if (!String.IsNullOrEmpty(style))
            {
                Append(" style=\"", style, "\"");
            }

            if (!String.IsNullOrEmpty(cssclass))
            {
                Append(" class=\"", cssclass, "\"");
            }

            Append(">");
        }

        // Add the zone
        CMSWebPartZone zone = AddZone(ID + "_zone", ID);

        if (renderEnvelope)
        {
            Append("</div>");
        }

        if (IsDesign)
        {
            Append("</td>");

            // Resizers
            if (AllowDesignMode)
            {
                // Vertical resizer
                Append("<td class=\"HorizontalResizer\" onmousedown=\"", GetHorizontalResizerScript("env", "Width", false, null), " return false;\">&nbsp;</td></tr><tr>");

                // Horizontal resizer
                Append("<td class=\"VerticalResizer\" onmousedown=\"", GetVerticalResizerScript("env", "Height"), " return false;\">&nbsp;</td>");
                Append("<td class=\"BothResizer\" onmousedown=\"", GetBothResizerScript("env", "Width", "Height"), " return false;\">&nbsp;</td>");
            }

            Append("</tr></table>");
        }

        // Panel for extender
        PlaceHolder pnlEx = new PlaceHolder();
        pnlEx.ID = "pnlEx";
        //pnlEx.Visible = false;

        AddControl(pnlEx);


        FinishLayout();
    }

    #endregion
}