using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_Rows : CMSAbstractLayoutWebPart
{
    #region "Properties"

    /// <summary>
    /// Number of rows.
    /// </summary>
    public int Rows
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Rows"), 2);
        }
        set
        {
            SetValue("Rows", value);
        }
    }


    /// <summary>
    /// First row width.
    /// </summary>
    public string Row1Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row1Width"), "");
        }
        set
        {
            SetValue("Row1Width", value);
        }
    }


    /// <summary>
    /// First row height.
    /// </summary>
    public string Row1Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row1Height"), "");
        }
        set
        {
            SetValue("Row1Height", value);
        }
    }


    /// <summary>
    /// First row CSS class.
    /// </summary>
    public string Row1CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row1CSSClass"), "");
        }
        set
        {
            SetValue("Row1CSSClass", value);
        }
    }


    /// <summary>
    /// Second row width.
    /// </summary>
    public string Row2Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row2Width"), "");
        }
        set
        {
            SetValue("Row2Width", value);
        }
    }


    /// <summary>
    /// Second row height.
    /// </summary>
    public string Row2Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row2Height"), "");
        }
        set
        {
            SetValue("Row2Height", value);
        }
    }


    /// <summary>
    /// Second row CSS class.
    /// </summary>
    public string Row2CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row2CSSClass"), "");
        }
        set
        {
            SetValue("Row2CSSClass", value);
        }
    }


    /// <summary>
    /// Third row width.
    /// </summary>
    public string Row3Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row3Width"), "");
        }
        set
        {
            SetValue("Row3Width", value);
        }
    }


    /// <summary>
    /// Third row height.
    /// </summary>
    public string Row3Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row3Height"), "");
        }
        set
        {
            SetValue("Row3Height", value);
        }
    }


    /// <summary>
    /// Third row CSS class.
    /// </summary>
    public string Row3CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row3CSSClass"), "");
        }
        set
        {
            SetValue("Row3CSSClass", value);
        }
    }


    /// <summary>
    /// Fourth row width.
    /// </summary>
    public string Row4Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row4Width"), "");
        }
        set
        {
            SetValue("Row4Width", value);
        }
    }


    /// <summary>
    /// Fourth row height.
    /// </summary>
    public string Row4Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row4Height"), "");
        }
        set
        {
            SetValue("Row4Height", value);
        }
    }


    /// <summary>
    /// Fourth row CSS class.
    /// </summary>
    public string Row4CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row4CSSClass"), "");
        }
        set
        {
            SetValue("Row4CSSClass", value);
        }
    }


    /// <summary>
    /// Fifth row width.
    /// </summary>
    public string Row5Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row5Width"), "");
        }
        set
        {
            SetValue("Row5Width", value);
        }
    }


    /// <summary>
    /// Fifth row height.
    /// </summary>
    public string Row5Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row5Height"), "");
        }
        set
        {
            SetValue("Row5Height", value);
        }
    }


    /// <summary>
    /// Fifth row CSS class.
    /// </summary>
    public string Row5CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Row5CSSClass"), "");
        }
        set
        {
            SetValue("Row5CSSClass", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        // Prepare the main markup
        StartLayout();

        if (IsDesign)
        {
            Append("<table class=\"LayoutTable\" cellspacing=\"0\" style=\"width: 100%;\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td>");
        }


        // Add the columns
        for (int i = 1; i <= Rows; i++)
        {
            // Set the width property
            string heightPropertyName = "Row" + i + "Height";
            string widthPropertyName = "Row" + i + "Width";

            string width = ValidationHelper.GetString(GetValue(widthPropertyName), "");
            string height = ValidationHelper.GetString(GetValue(heightPropertyName), "");

            string rowId = "row" + i;

            if (IsDesign)
            {
                Append("<table cellspacing=\"0\" cellpadding=\"0\"><tr><td id=\"", ShortClientID, "_", rowId, "\"");
            }
            else
            {
                Append("<div");
            }

            string style = "vertical-align: top;";

            // Row height
            if (!String.IsNullOrEmpty(height))
            {
                style += "height: " + height + ";";
            }

            // Row width
            if (!String.IsNullOrEmpty(width))
            {
                style += "width: " + width + ";";
            }
            else if (IsDesign)
            {
                style += "width: 100%;";
            }

            // Append style
            if (!String.IsNullOrEmpty(style))
            {
                Append(" style=\"", style, "\"");
            }

            // Cell class
            string thisRowClass = ValidationHelper.GetString(GetValue("Row" + i + "CSSClass"), "");
            if (!String.IsNullOrEmpty(thisRowClass))
            {
                Append(" class=\"", thisRowClass, "\"");
            }

            Append(">");

            // Add the zone
            AddZone(ID + "_" + i, "[" + i + "]");

            if (IsDesign)
            {
                Append("</td>");

                // Resizers
                if (AllowDesignMode)
                {
                    Append("<td class=\"HorizontalResizer\" onmousedown=\"", GetHorizontalResizerScript(rowId, widthPropertyName, false, null), " return false;\">&nbsp;</td></tr><tr>");

                    Append("<td class=\"VerticalResizer\" onmousedown=\"", GetVerticalResizerScript(rowId, heightPropertyName), " return false;\">&nbsp;</td>");
                    Append("<td class=\"BothResizer\" onmousedown=\"", GetBothResizerScript(rowId, widthPropertyName, heightPropertyName), " return false;\">&nbsp;</td>");
                }

                Append("</tr></table>");
            }
            else
            {
                Append("</div>");
            }
        }

        if (IsDesign)
        {
            Append("</td></tr>");

            // Footer
            if (AllowDesignMode)
            {
                Append("<tr><td class=\"LayoutFooter cms-bootstrap\"><div class=\"LayoutFooterContent\">");

                Append("<div class=\"LayoutLeftActions\">");

                // Row actions
                AppendAddAction(ResHelper.GetString("Layout.AddRow"), "Rows");
                if (Rows > 1)
                {
                    AppendRemoveAction(ResHelper.GetString("Layout.RemoveRow"), "Rows");
                }

                Append("</div></div></td></tr>");
            }

            Append("</table>");
        }

        // Finalize
        FinishLayout();
    }

    #endregion
}