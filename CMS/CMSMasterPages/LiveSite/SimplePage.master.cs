using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSMasterPages_LiveSite_SimplePage : AbstractMasterPage, ICMSMasterPage
{
    #region "Properties"

    /// <summary>
    /// PageTitle control.
    /// </summary>
    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }


    /// <summary>
    /// HeaderActions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            if (base.HeaderActions != null)
            {
                return base.HeaderActions;
            }
            return actionsElem;
        }
    }


    /// <summary>
    /// Body panel.
    /// </summary>
    public override Panel PanelBody
    {
        get
        {
            return pnlBody;
        }
    }


    /// <summary>
    /// Prepared for specifying the additional HEAD elements.
    /// </summary>
    public override Literal HeadElements
    {
        get
        {
            return ltlHeadElements;
        }
        set
        {
            ltlHeadElements = value;
        }
    }


    /// <summary>
    /// Body element control
    /// </summary>
    public override System.Web.UI.HtmlControls.HtmlGenericControl Body
    {
        get
        {
            return bodyElem;
        }
    }


    /// <summary>
    /// Gets the labels container.
    /// </summary>
    public override PlaceHolder PlaceholderLabels
    {
        get
        {
            return plcLabels;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        SetRTL();
        SetBrowserClass();

        PageStatusContainer = plcStatus;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Display panel with additional controls place holder if required
        if (DisplayControlsPanel)
        {
            pnlAdditionalControls.Visible = true;
        }

        bodyElem.Attributes["class"] = mBodyClass;
    }


    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        // Hide actions panel if no actions are present and DisplayActionsPanel is false
        if (!DisplayActionsPanel)
        {
            if (!actionsElem.IsVisible() && (plcActions.Controls.Count == 0))
            {
                pnlActions.Visible = false;
            }
        }

        base.Render(writer);
    }


    public void SetBrowserClass()
    {
        BodyClass = EnsureBodyClass(BodyClass);
    }


    public void SetRTL()
    {
        if (CultureHelper.IsUICultureRTL())
        {
            BodyClass += " RTL";
            BodyClass = BodyClass.Trim();
        }
    }


    /// <summary>
    /// Sets the browser class to the body class.
    /// </summary>
    /// <param name="bodyClass">The body class.</param>
    /// <param name="generateCultureClass">if set to true generate culture class.</param>
    internal static string EnsureBodyClass(string bodyClass, bool generateCultureClass = true)
    {
        // Add browser type
#pragma warning disable CS0618 // Type or member is obsolete
        string browserClass = BrowserHelper.GetBrowserClass();
#pragma warning restore CS0618 // Type or member is obsolete
        if (!String.IsNullOrEmpty(browserClass))
        {
            bodyClass = string.Format("{0} {1}", bodyClass, browserClass).Trim();
        }

        if (generateCultureClass)
        {
            // Add culture type
            string cultureClass = DocumentContext.GetUICultureClass();
            if (!String.IsNullOrEmpty(cultureClass))
            {
                bodyClass = string.Format("{0} {1}", bodyClass, cultureClass).Trim();
            }
        }
        // Add bootstrap
        PortalUIHelper.EnsureBootstrapBodyClass(ref bodyClass, PortalContext.ViewMode, PageContext.CurrentPage);

        return bodyClass;
    }

    #endregion
}