using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSMasterPages_UI_SimplePage : CMSMasterPage
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
            return actionsElem.HeaderActions;
        }
    }


    /// <summary>
    /// Container with header actions menu
    /// </summary>
    public override ObjectEditMenu ObjectEditMenu
    {
        get
        {
            if (actionsElem != null)
            {
                return actionsElem.ObjectEditMenu;
            }

            return null;
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
    /// Gets the top panel.
    /// </summary>
    public override Panel PanelHeader
    {
        get
        {
            return pnlContainer;
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
    /// Gets the content panel.
    /// </summary>
    public override Panel PanelContent
    {
        get
        {
            return pnlContent;
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


    /// <summary>
    /// Gets UIPlaceHolder which wraps up HeaderActions and allows to modify visibility of its children.
    /// </summary>
    public override UIPlaceHolder HeaderActionsPlaceHolder
    {
        get
        {
            return plcActionsPermissions;
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
    /// Gets placeholder located after form element.
    /// </summary>
    public override PlaceHolder AfterFormPlaceholder
    {
        get
        {
            return plcAfterForm;
        }
    }

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;
        bodyElem.Attributes["class"] = mBodyClass;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display panel with additional controls place holder if required
        if (DisplayControlsPanel != pnlAdditionalControls.Visible)
        {
            pnlAdditionalControls.Visible = DisplayControlsPanel;
        }

        // Display panel with site selector
        if (DisplaySiteSelectorPanel != pnlSiteSelector.Visible)
        {
            pnlSiteSelector.Visible = DisplaySiteSelectorPanel;
        }

        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
    }


    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        // Hide actions panel if no actions are present
        if (!HeaderActions.IsVisible() && (plcActions.Controls.Count == 0) && (plcBeforeActions.Controls.Count == 0))
        {
            pnlActions.Visible = false;
        }

        base.Render(writer);
    }

    #endregion
}
