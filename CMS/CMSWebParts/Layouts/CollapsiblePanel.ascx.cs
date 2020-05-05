using System;
using System.Web.UI.WebControls;

using AjaxControlToolkit;

using CMS.Base;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_CollapsiblePanel : CMSAbstractLayoutWebPart
{
    #region "Properties"

    /// <summary>
    /// Collapsed text.
    /// </summary>
    public string CollapsedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CollapsedText"), "");
        }
        set
        {
            SetValue("CollapsedText", value);
        }
    }


    /// <summary>
    /// Collapsed image.
    /// </summary>
    public string CollapsedImage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CollapsedImage"), "");
        }
        set
        {
            SetValue("CollapsedImage", value);
        }
    }


    /// <summary>
    /// Expanded text.
    /// </summary>
    public string ExpandedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExpandedText"), "");
        }
        set
        {
            SetValue("ExpandedText", value);
        }
    }


    /// <summary>
    /// Expanded image.
    /// </summary>
    public string ExpandedImage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExpandedImage"), "");
        }
        set
        {
            SetValue("ExpandedImage", value);
        }
    }


    /// <summary>
    /// Header CSS class.
    /// </summary>
    public string HeaderCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderCSSClass"), "");
        }
        set
        {
            SetValue("HeaderCSSClass", value);
        }
    }


    /// <summary>
    /// Image CSS class.
    /// </summary>
    public string ImageCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ImageCSSClass"), "");
        }
        set
        {
            SetValue("ImageCSSClass", value);
        }
    }


    /// <summary>
    /// Content CSS class.
    /// </summary>
    public string ContentCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ContentCSSClass"), "");
        }
        set
        {
            SetValue("ContentCSSClass", value);
        }
    }


    /// <summary>
    /// Collapsed size (px).
    /// </summary>
    public int CollapsedSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("CollapsedSize"), -1);
        }
        set
        {
            SetValue("CollapsedSize", value);
        }
    }


    /// <summary>
    /// Expanded size (px).
    /// </summary>
    public int ExpandedSize
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ExpandedSize"), -1);
        }
        set
        {
            SetValue("ExpandedSize", value);
        }
    }


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
    /// Collapsed.
    /// </summary>
    public bool Collapsed
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("Collapsed"), false);
        }
        set
        {
            SetValue("Collapsed", value);
        }
    }


    /// <summary>
    /// Auto collapse.
    /// </summary>
    public bool AutoCollapse
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoCollapse"), false);
        }
        set
        {
            SetValue("AutoCollapse", value);
        }
    }


    /// <summary>
    /// Auto expand.
    /// </summary>
    public bool AutoExpand
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoExpand"), false);
        }
        set
        {
            SetValue("AutoExpand", value);
        }
    }


    /// <summary>
    /// Expand direction.
    /// </summary>
    public string ExpandDirection
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExpandDirection"), "");
        }
        set
        {
            SetValue("ExpandDirection", value);
        }
    }


    /// <summary>
    /// Scroll content.
    /// </summary>
    public bool ScrollContent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ScrollContent"), false);
        }
        set
        {
            SetValue("ScrollContent", value);
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

        Append("<div");

        // Width
        string width = Width;
        if (!string.IsNullOrEmpty(width))
        {
            Append(" style=\"width: ", width, "\"");
        }

        if (IsDesign)
        {
            Append(" id=\"", ShortClientID, "_env\">");

            Append("<table class=\"LayoutTable\" cellspacing=\"0\" style=\"width: 100%;\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\" colspan=\"2\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td style=\"width: 100%;\">");
        }
        else
        {
            Append(">");
        }

        // Header panel
        Panel pnlHeader = new Panel();
        pnlHeader.ID = "pnlH";
        pnlHeader.CssClass = HeaderCSSClass;
        pnlHeader.EnableViewState = false;

        // Header label
        Label lblHeader = new Label();
        lblHeader.ID = "lblH";

        pnlHeader.Controls.Add(lblHeader);

        // Header image
        Image imgHeader = new Image();
        imgHeader.CssClass = ImageCSSClass;
        imgHeader.ID = "imgH";

        pnlHeader.Controls.Add(imgHeader);


        AddControl(pnlHeader);

        // Content panel
        Panel pnlContent = new Panel();
        pnlContent.CssClass = ContentCSSClass;
        pnlContent.ID = "pnlC";

        AddControl(pnlContent);

        // Add the zone
        CMSWebPartZone zone = AddZone(ID + "_zone", ID, pnlContent);

        // Add the extender
        CollapsiblePanelExtender cp = new CollapsiblePanelExtender();
        cp.ID = "extCP";
        cp.TargetControlID = pnlContent.ID;

        cp.ExpandControlID = pnlHeader.ID;
        cp.CollapseControlID = pnlHeader.ID;

        cp.TextLabelID = lblHeader.ID;
        cp.ImageControlID = imgHeader.ID;

        cp.ExpandDirection = (ExpandDirection.EqualsCSafe("horz", true) ? CollapsiblePanelExpandDirection.Horizontal : CollapsiblePanelExpandDirection.Vertical);

        // Texts
        string expText = ResHelper.LocalizeString(ExpandedText);
        string colText = ResHelper.LocalizeString(CollapsedText);

        cp.ExpandedText = expText;
        cp.CollapsedText = colText;

        if (String.IsNullOrEmpty(expText) && String.IsNullOrEmpty(colText))
        {
            lblHeader.Visible = false;
        }

        // Images
        string expImage = ExpandedImage;
        string colImage = CollapsedImage;

        if (!String.IsNullOrEmpty(expImage) && !String.IsNullOrEmpty(colImage))
        {
            cp.ExpandedImage = expImage;
            cp.CollapsedImage = colImage;
        }
        else
        {
            imgHeader.Visible = false;
        }

        // Sizes
        int expSize = ExpandedSize;
        if (expSize > 0)
        {
            cp.ExpandedSize = expSize;
        }

        int collapsed = CollapsedSize;
        if (collapsed >= 0)
        {
            cp.CollapsedSize = CollapsedSize;
        }

        cp.Collapsed = Collapsed;

        if (!IsDesign)
        {
            cp.AutoCollapse = AutoCollapse;
            if (AutoExpand)
            {
                cp.AutoExpand = true;

                // Ensure some collapsed size
                if (collapsed < 0)
                {
                    cp.CollapsedSize = 10;
                }
            }
        }

        cp.ScrollContents = ScrollContent;

        // Add the extender
        Controls.Add(cp);

        if (IsDesign)
        {
            Append("</td>");

            // Width resizer
            if (AllowDesignMode)
            {
                Append("<td class=\"HorizontalResizer\" onmousedown=\"", GetHorizontalResizerScript("env", "Width"), " return false;\">&nbsp;</td>");
            }

            Append("</tr></table>");
        }

        Append("</div>");

        FinishLayout();
    }

    #endregion
}