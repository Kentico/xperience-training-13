using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_ZonesWithEffect : CMSAbstractLayoutWebPart
{
    #region "Properties"

    /// <summary>
    /// Number of zones.
    /// </summary>
    public int Zones
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Zones"), 2);
        }
        set
        {
            SetValue("Zones", value);
        }
    }


    /// <summary>
    /// Content before all zones.
    /// </summary>
    public string BeforeZones
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BeforeZones"), "");
        }
        set
        {
            SetValue("BeforeZones", value);
        }
    }


    /// <summary>
    /// Separator between zones.
    /// </summary>
    public string Separator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Separator"), "");
        }
        set
        {
            SetValue("Separator", value);
        }
    }


    /// <summary>
    /// Content after all zones.
    /// </summary>
    public string AfterZones
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AfterZones"), "");
        }
        set
        {
            SetValue("AfterZones", value);
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


    /// <summary>
    /// Zone width.
    /// </summary>
    public string ZoneWidth
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZoneWidth"), "");
        }
        set
        {
            SetValue("ZoneWidth", value);
        }
    }


    /// <summary>
    /// Content before each zone.
    /// </summary>
    public string BeforeZone
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BeforeZone"), "");
        }
        set
        {
            SetValue("BeforeZone", value);
        }
    }


    /// <summary>
    /// Content after each zone.
    /// </summary>
    public string AfterZone
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AfterZone"), "");
        }
        set
        {
            SetValue("AfterZone", value);
        }
    }


    /// <summary>
    /// Script files.
    /// </summary>
    public string ScriptFiles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ScriptFiles"), "");
        }
        set
        {
            SetValue("ScriptFiles", value);
        }
    }


    /// <summary>
    /// Initialization script.
    /// </summary>
    public string InitScript
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InitScript"), "");
        }
        set
        {
            SetValue("InitScript", value);
        }
    }


    /// <summary>
    /// Additional CSS files.
    /// </summary>
    public string CSSFiles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSFiles"), "");
        }
        set
        {
            SetValue("CSSFiles", value);
        }
    }


    /// <summary>
    /// Inline CSS styles.
    /// </summary>
    public string InlineCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InlineCSS"), "");
        }
        set
        {
            SetValue("InlineCSS", value);
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

        // Content before zones
        Append(BeforeZones);

        string separator = Separator;
        string before = BeforeZone;
        string after = AfterZone;

        string zoneclass = ZoneCSSClass;
        string zonewidth = ZoneWidth;

        // Render the zones
        for (int i = 1; i <= Zones; i++)
        {
            if (i > 1)
            {
                Append(separator);
            }
            Append("<div");

            // Zone class
            if (!String.IsNullOrEmpty(zoneclass))
            {
                Append(" class=\"", zoneclass, "\"");
            }

            // Zone width
            if (!String.IsNullOrEmpty(zonewidth))
            {
                Append(" style=\"width: ", zonewidth, "\";");
            }

            Append(">", before);

            // Add the zone
            CMSWebPartZone zone = AddZone(ID + "_" + i, "[" + i + "]");

            Append(after, "</div>");
        }

        // Content after zones
        Append(AfterZones);

        if (IsDesign)
        {
            Append("</td></tr>");

            // Footer
            if (AllowDesignMode)
            {
                Append("<tr><td class=\"LayoutFooter cms-bootstrap\" colspan=\"2\"><div class=\"LayoutFooterContent\">");

                // Zone actions
                AppendRemoveAction(ResHelper.GetString("Layout.RemoveZone"), "Zones");
                Append("&nbsp;&nbsp;");
                AppendAddAction(ResHelper.GetString("Layout.AddZone"), "Zones");

                Append("</div></td></tr>");
            }

            Append("</table>");
        }

        // Register scripts
        string[] scripts = ScriptFiles.Split('\r', '\n');
        foreach (string script in scripts)
        {
            // Register the script file
            string sfile = script.Trim();
            if (!String.IsNullOrEmpty(sfile))
            {
                ScriptHelper.RegisterScriptFile(Page, sfile);
            }
        }

        // Add init script
        string resolvedInitScript = MacroResolver.Resolve(InitScript);
        if (!string.IsNullOrEmpty(resolvedInitScript))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), ShortClientID + "_Init", ScriptHelper.GetScript(resolvedInitScript));
        }

        // Register CSS files
        string[] cssFiles = CSSFiles.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        Array.ForEach(cssFiles, cssFile => CssRegistration.RegisterCssLink(Page, cssFile.Trim()));

        // Add inline CSS
        string inlinecss = MacroResolver.Resolve(InlineCSS);
        if (!string.IsNullOrEmpty(inlinecss))
        {
            // Add css to page header
            CssRegistration.RegisterCssBlock(Page, "zonesWithEffectInlineCss_" + ClientID, inlinecss);
        }

        FinishLayout();
    }

    #endregion
}
