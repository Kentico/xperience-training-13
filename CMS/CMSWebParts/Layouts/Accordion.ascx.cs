using System;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_Accordion : CMSAbstractLayoutWebPart
{
    #region "Public properties"

    /// <summary>
    /// Number of panes.
    /// </summary>
    public int Panes
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Panes"), 2);
        }
        set
        {
            SetValue("Panes", value);
        }
    }


    /// <summary>
    /// Pane headers.
    /// </summary>
    public string PaneHeaders
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PaneHeaders"), "");
        }
        set
        {
            SetValue("PaneHeaders", value);
        }
    }


    /// <summary>
    /// Active pane index.
    /// </summary>
    public int ActivePaneIndex
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ActivePaneIndex"), -1);
        }
        set
        {
            SetValue("ActivePaneIndex", value);
        }
    }


    /// <summary>
    /// Require opened pane.
    /// </summary>
    public bool RequireOpenedPane
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RequireOpenedPane"), false);
        }
        set
        {
            SetValue("RequireOpenedPane", value);
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
    /// Selected header CSS class.
    /// </summary>
    public string SelectedHeaderCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedHeaderCSSClass"), "");
        }
        set
        {
            SetValue("SelectedHeaderCSSClass", value);
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
    /// Fade transitions.
    /// </summary>
    public bool FadeTransitions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FadeTransitions"), false);
        }
        set
        {
            SetValue("FadeTransitions", value);
        }
    }


    /// <summary>
    /// Transition duration (ms).
    /// </summary>
    public int TransitionDuration
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TransitionDuration"), 500);
        }
        set
        {
            SetValue("TransitionDuration", value);
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

            Append("<tr><td id=\"", ShortClientID, "_info\" style=\"width: 100%;\">");
        }
        else
        {
            Append(">");
        }

        // Add the tabs
        var acc = new CMSAccordion();
        acc.ID = "acc";
        AddControl(acc);

        if (IsDesign)
        {
            Append("</td>");

            if (AllowDesignMode)
            {
                // Width resizer
                Append("<td class=\"HorizontalResizer\" onmousedown=\"" + GetHorizontalResizerScript("env", "Width", false, "info") + " return false;\">&nbsp;</td>");
            }

            Append("</tr>");
        }

        // Pane headers
        string[] headers = TextHelper.EnsureLineEndings(PaneHeaders, "\n").Split('\n');

        for (int i = 1; i <= Panes; i++)
        {
            // Create new pane
            var pane = new CMSAccordionPane();
            pane.ID = "pane" + i;

            // Prepare the header
            string header = null;
            if (headers.Length >= i)
            {
                header = ResHelper.LocalizeString(headers[i - 1]);
                header = HTMLHelper.HTMLEncode(header);
            }
            if (String.IsNullOrEmpty(header))
            {
                header = "Pane " + i;
            }

            pane.Header = new TextTransformationTemplate(header);
            acc.Panes.Add(pane);

            pane.WebPartZone = AddZone(ID + "_" + i, header, pane.ContentContainer);
        }

        // Setup the accordion
        if ((ActivePaneIndex >= 1) && (ActivePaneIndex <= acc.Panes.Count))
        {
            acc.SelectedIndex = ActivePaneIndex - 1;
        }

        acc.ContentCssClass = ContentCSSClass;
        acc.HeaderCssClass = HeaderCSSClass;
        acc.HeaderSelectedCssClass = SelectedHeaderCSSClass;

        acc.FadeTransitions = FadeTransitions;
        acc.TransitionDuration = TransitionDuration;
        acc.RequireOpenedPane = RequireOpenedPane;

        // If no active pane is selected and doesn't require opened one, do not preselect any
        if (!acc.RequireOpenedPane && (ActivePaneIndex < 0))
        {
            acc.SelectedIndex = -1;
        }

        if (IsDesign)
        {
            if (AllowDesignMode)
            {
                Append("<tr><td class=\"LayoutFooter cms-bootstrap\" colspan=\"2\"><div class=\"LayoutFooterContent\">");

                // Pane actions
                Append("<div class=\"LayoutLeftActions\">");

                AppendAddAction(ResHelper.GetString("Layout.AddPane"), "Panes");
                if (Panes > 1)
                {
                    AppendRemoveAction(ResHelper.GetString("Layout.RemoveLastPane"), "Panes");
                }

                Append("</div></div></td></tr>");
            }

            Append("</table>");
        }

        Append("</div>");

        FinishLayout();
    }

    #endregion
}