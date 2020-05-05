using System;
using System.Collections;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_Layouts_Columns : CMSAbstractLayoutWebPart
{

    #region "Properties"

    /// <summary>
    /// Number of left columns.
    /// </summary>
    public int LeftColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("LeftColumns"), 1);
        }
        set
        {
            SetValue("LeftColumns", value);
        }
    }


    /// <summary>
    /// Use center column.
    /// </summary>
    public bool CenterColumn
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CenterColumn"), true);
        }
        set
        {
            SetValue("CenterColumn", value);
        }
    }


    /// <summary>
    /// Number of right columns.
    /// </summary>
    public int RightColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RightColumns"), 1);
        }
        set
        {
            SetValue("RightColumns", value);
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
    /// Equal columns height.
    /// </summary>
    public bool EqualHeight
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EqualHeight"), false);
        }
        set
        {
            SetValue("EqualHeight", value);
        }
    }


    /// <summary>
    /// First left column width.
    /// </summary>
    public string LColumn1Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn1Width"), "");
        }
        set
        {
            SetValue("LColumn1Width", value);
        }
    }


    /// <summary>
    /// First left column height.
    /// </summary>
    public string LColumn1Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn1Height"), "");
        }
        set
        {
            SetValue("LColumn1Height", value);
        }
    }


    /// <summary>
    /// First left column CSS class.
    /// </summary>
    public string LColumn1CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn1CSSClass"), "");
        }
        set
        {
            SetValue("LColumn1CSSClass", value);
        }
    }


    /// <summary>
    /// Second left column width.
    /// </summary>
    public string LColumn2Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn2Width"), "");
        }
        set
        {
            SetValue("LColumn2Width", value);
        }
    }


    /// <summary>
    /// Second left column height.
    /// </summary>
    public string LColumn2Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn2Height"), "");
        }
        set
        {
            SetValue("LColumn2Height", value);
        }
    }


    /// <summary>
    /// Second left column CSS class.
    /// </summary>
    public string LColumn2CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn2CSSClass"), "");
        }
        set
        {
            SetValue("LColumn2CSSClass", value);
        }
    }


    /// <summary>
    /// Third left column width.
    /// </summary>
    public string LColumn3Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn3Width"), "");
        }
        set
        {
            SetValue("LColumn3Width", value);
        }
    }


    /// <summary>
    /// Third left column height.
    /// </summary>
    public string LColumn3Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn3Height"), "");
        }
        set
        {
            SetValue("LColumn3Height", value);
        }
    }


    /// <summary>
    /// Third left column CSS class.
    /// </summary>
    public string LColumn3CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LColumn3CSSClass"), "");
        }
        set
        {
            SetValue("LColumn3CSSClass", value);
        }
    }


    /// <summary>
    /// Use center column.
    /// </summary>
    public string CenterColumnCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CenterColumnCSSClass"), "");
        }
        set
        {
            SetValue("CenterColumnCSSClass", value);
        }
    }


    /// <summary>
    /// Center column height.
    /// </summary>
    public string CenterColumnHeight
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CenterColumnHeight"), "");
        }
        set
        {
            SetValue("CenterColumnHeight", value);
        }
    }


    /// <summary>
    /// First right column width.
    /// </summary>
    public string RColumn1Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn1Width"), "");
        }
        set
        {
            SetValue("RColumn1Width", value);
        }
    }


    /// <summary>
    /// First right column height.
    /// </summary>
    public string RColumn1Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn1Height"), "");
        }
        set
        {
            SetValue("RColumn1Height", value);
        }
    }


    /// <summary>
    /// First right column CSS class.
    /// </summary>
    public string RColumn1CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn1CSSClass"), "");
        }
        set
        {
            SetValue("RColumn1CSSClass", value);
        }
    }


    /// <summary>
    /// Second right column width.
    /// </summary>
    public string RColumn2Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn2Width"), "");
        }
        set
        {
            SetValue("RColumn2Width", value);
        }
    }


    /// <summary>
    /// Second right column height.
    /// </summary>
    public string RColumn2Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn2Height"), "");
        }
        set
        {
            SetValue("RColumn2Height", value);
        }
    }


    /// <summary>
    /// Second right column CSS class.
    /// </summary>
    public string RColumn2CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn2CSSClass"), "");
        }
        set
        {
            SetValue("RColumn2CSSClass", value);
        }
    }


    /// <summary>
    /// Third right column width.
    /// </summary>
    public string RColumn3Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn3Width"), "");
        }
        set
        {
            SetValue("RColumn3Width", value);
        }
    }


    /// <summary>
    /// Third right column height.
    /// </summary>
    public string RColumn3Height
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn3Height"), "");
        }
        set
        {
            SetValue("RColumn3Height", value);
        }
    }


    /// <summary>
    /// Third right column CSS class.
    /// </summary>
    public string RColumn3CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RColumn3CSSClass"), "");
        }
        set
        {
            SetValue("RColumn3CSSClass", value);
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

        string height = ValidationHelper.GetString(GetValue("Height"), "");

        // Prepare the data for equal heights script
        bool equal = EqualHeight;

        string groupClass = null;
        if (equal)
        {
            groupClass = "Cols_" + InstanceGUID.ToString().Replace("-", "");
        }

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

        // Prepare automatic width
        string autoWidth = null;
        int cols = LeftColumns + RightColumns;
        if (CenterColumn)
        {
            cols++;
        }
        if (cols > 0)
        {
            autoWidth = ((100 - cols) / cols) + "%";
        }

        // Encapsulating div
        if (IsDesign && AllowDesignMode)
        {
            Append("<div id=\"", ShortClientID, "_all\">");
        }
        else
        {
            Append("<div>");
        }

        // Left columns
        CreateColumns(LeftColumns, height, equal, groupClass, autoWidth, false);

        // Right columns
        CreateColumns(RightColumns, height, equal, groupClass, autoWidth, true);

        // Center column
        if (CenterColumn)
        {
            if (IsDesign && AllowDesignMode)
            {
                Append("<div style=\"overflow: auto;\" class=\"LayoutCenterColumn\">");
            }

            Append("<div");

            // Cell class
            string thisColumnClass = ValidationHelper.GetString(GetValue("CenterColumnCSSClass"), "");
            if (equal)
            {
                thisColumnClass = CssHelper.JoinClasses(thisColumnClass, groupClass);
            }

            if (!String.IsNullOrEmpty(thisColumnClass))
            {
                Append(" class=\"");
                Append(thisColumnClass);
                Append("\"");
            }

            string style = "overflow: auto;";

            // Height
            if (!equal)
            {
                height = DataHelper.GetNotEmpty(GetValue("CenterColumnHeight"), height);
            }
            if (!String.IsNullOrEmpty(height))
            {
                style += " height: " + height + ";";
            }

            // Append style
            if (!String.IsNullOrEmpty(style))
            {
                Append(" style=\"");
                Append(style);
                Append("\"");
            }

            if (IsDesign)
            {
                Append(" id=\"", ShortClientID, "_col_c\"");
            }

            Append(">");

            // Add the zone
            AddZone(ID + "_center", "Center");

            Append("</div>");

            if (IsDesign)
            {
                // Vertical resizer for center column
                if (AllowDesignMode && !equal)
                {
                    Append("<div class=\"VerticalResizer\" onmousedown=\"" + GetVerticalResizerScript("col_c", "CenterColumnHeight") + " return false;\">&nbsp;</div>");
                }

                Append("</div>");
            }
        }

        // End of encapsulating div
        Append("<div style=\"clear: both;\"></div></div>");

        if (IsDesign)
        {
            Append("</td></tr>");

            // Footer with actions
            if (AllowDesignMode)
            {
                // Vertical resizer for all columns
                if (equal)
                {
                    Append("<tr><td class=\"VerticalResizer\" onmousedown=\"" + GetVerticalResizerScript("all", "Height", null, "SetAllHeight_" + groupClass) + " return false;\">&nbsp;</td></tr>");
                }

                Append("<tr><td class=\"LayoutFooter cms-bootstrap\"><div class=\"LayoutFooterContent\">");

                // Pane actions
                Append("<div class=\"LayoutLeftActions\">");
                AppendAddAction(ResHelper.GetString("Layout.AddLeftColumn"), "LeftColumns");
                if (LeftColumns > 0)
                {
                    AppendRemoveAction(ResHelper.GetString("Layout.RemoveLeftColumn"), "LeftColumns");
                }
                Append("</div>");

                Append("<div class=\"LayoutRightActions\">");
                AppendAddAction(ResHelper.GetString("Layout.AddRightColumn"), "RightColumns");
                if (RightColumns > 0)
                {
                    AppendRemoveAction(ResHelper.GetString("Layout.RemoveRightColumn"), "RightColumns");
                }
                Append("</div>");

                Append("</div></td></tr>");
            }

            Append("</table>");
        }

        FinishLayout();

        // Enforce equal height with a javascript
        if (equal)
        {
            PortalHelper.RegisterLayoutsScript(this.Page);
            ScriptHelper.RegisterScriptFile(Page, "jquery/jquery-equalheight.js");

            StringBuilder sb = new StringBuilder();

            sb.Append(
@"
function SetAllHeight_", groupClass, @"(h) {
    SetAllHeight('", groupClass, @"', h);
}

InitEqualHeight('", groupClass, @"');
"
            );

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EqualHeight_" + groupClass, ScriptHelper.GetScript(sb.ToString()));
        }
    }


    /// <summary>
    /// Creates the columns in the layout.
    /// </summary>
    /// <param name="cols">Number of columns</param>
    /// <param name="height">Height</param>
    /// <param name="equal">If true, the column heights should equal</param>
    /// <param name="groupClass">Group class</param>
    /// <param name="autoWidth">Automatic width</param>
    /// <param name="right">Right columns</param>
    protected void CreateColumns(int cols, string height, bool equal, string groupClass, string autoWidth, bool right)
    {
        for (int i = 1; i <= cols; i++)
        {
            string colMark = (right ? "r" : "l");

            // Set the width property
            string widthPropertyName = colMark + "Column" + i + "Width";
            string heightPropertyName = colMark + "Column" + i + "Height";

            // Do not use automatic width in case of design mode
            if (IsDesign)
            {
                autoWidth = "";
            }

            string width = DataHelper.GetNotEmpty(GetValue(widthPropertyName), autoWidth);
            if (!equal)
            {
                height = DataHelper.GetNotEmpty(GetValue(heightPropertyName), height);
            }

            Append("<div");

            string colId = "col" + colMark + i;

            // Add alignment
            string fl = null;
            if (right)
            {
                fl = "float: right;";
            }
            else
            {
                fl = "float: left;";
            }

            string style = fl;

            if (IsDesign)
            {
                // Append style
                if (!String.IsNullOrEmpty(style))
                {
                    Append(" style=\"", style, "\"");
                }

                // Design mode classes
                if (AllowDesignMode)
                {
                    Append(" class=\"", (right ? "LayoutRightColumn" : "LayoutLeftColumn"), "\"");
                }

                Append("><table cellspacing=\"0\" cellpadding=\"0\" border=\"0\"><tr>");

                if (right)
                {
                    // Width resizer
                    if (AllowDesignMode)
                    {
                        Append("<td class=\"HorizontalResizer\" onmousedown=\"", GetHorizontalResizerScript(colId, widthPropertyName, true, null, null), " return false;\">&nbsp;</td>");
                    }
                }

                Append("<td style=\"vertical-align: top;\">");

                Append("<div");

                style = null;
            }

            // Column width
            if (!String.IsNullOrEmpty(width))
            {
                style += "width: " + width + ";";
            }

            // Height
            if (!String.IsNullOrEmpty(height))
            {
                style += "height: " + height + ";";
            }

            // Append style
            if (!String.IsNullOrEmpty(style))
            {
                Append(" style=\"", style, "\"");
            }

            // Cell class
            string thisColumnClass = ValidationHelper.GetString(GetValue(colMark + "Column" + i + "CSSClass"), "");
            if (equal)
            {
                thisColumnClass = CssHelper.JoinClasses(thisColumnClass, groupClass);
            }

            if (!String.IsNullOrEmpty(thisColumnClass))
            {
                Append(" class=\"", thisColumnClass, "\"");
            }

            if (IsDesign)
            {
                Append(" id=\"", ShortClientID, "_", colId, "\"");
            }

            Append(">");

            // Add the zone
            AddZone(ID + "_" + colMark + i, "[" + colMark.ToUpperCSafe() + i + "]");

            Append("</div>");

            if (IsDesign)
            {
                // Right column
                Append("</td>");

                if (AllowDesignMode)
                {
                    if (right)
                    {
                        // Resizers
                        if (!equal)
                        {
                            Append("</tr><tr>");

                            Append("<td class=\"BothResizer\" onmousedown=\"", GetBothResizerScript(colId, widthPropertyName, heightPropertyName, true, null), " return false;\">&nbsp;</td>");
                            Append("<td class=\"VerticalResizer\" onmousedown=\"", GetVerticalResizerScript(colId, heightPropertyName), " return false;\">&nbsp;</td>");
                        }
                    }
                    else
                    {
                        // Resizers
                        Append("<td class=\"HorizontalResizer\" onmousedown=\"");
                        Append(GetHorizontalResizerScript(colId, widthPropertyName, false, null, null));
                        Append(" return false;\">&nbsp;</td>");

                        if (!equal)
                        {
                            Append("</tr><tr>");

                            Append("<td class=\"VerticalResizer\" onmousedown=\"", GetVerticalResizerScript(colId, heightPropertyName), " return false;\">&nbsp;</td>");
                            Append("<td class=\"BothResizer\" onmousedown=\"", GetBothResizerScript(colId, widthPropertyName, heightPropertyName), " return false;\">&nbsp;</td>");
                        }
                    }
                }

                Append("</tr></table>");

                Append("</div>");
            }
        }
    }

    #endregion
}
