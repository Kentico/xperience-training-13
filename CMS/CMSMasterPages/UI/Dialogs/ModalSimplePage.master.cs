using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.UIControls;

public partial class CMSMasterPages_UI_Dialogs_ModalSimplePage : CMSMasterPage
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
    /// Header panel.
    /// </summary>
    public override Panel PanelHeader
    {
        get
        {
            return pnlHeader;
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
    /// Content panel.
    /// </summary>
    public override Panel PanelContent
    {
        get
        {
            return pnlContent;
        }
    }


    /// <summary>
    /// Body object.
    /// </summary>
    public override HtmlGenericControl Body
    {
        get
        {
            return bodyElem;
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


    /// <summary>
    /// Gets header container.
    /// </summary>
    public override Panel HeaderContainer
    {
        get
        {
            return pnlHeaderContainer;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        PageStatusContainer = plcStatus;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Display panel with additional controls place holder if required
        if (DisplayControlsPanel)
        {
            pnlAdditionalControls.Visible = true;
        }

        // Display panel with site selector
        if (DisplaySiteSelectorPanel)
        {
            pnlSiteSelector.Visible = true;
        }

        // Set separator visibility
        pnlSeparator.Visible = DisplaySeparatorPanel;

        bodyElem.Attributes["class"] = mBodyClass;

        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
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

    #endregion


    #region "Methods"

    /// <summary>
    /// Ensures message labels on the page.
    /// </summary>
    protected override MessagesPlaceHolder EnsureMessagesPlaceHolder()
    {
        MessagesPlaceHolder messagesPlaceHolder = base.EnsureMessagesPlaceHolder();
        messagesPlaceHolder.OffsetX = 16;
        messagesPlaceHolder.OffsetY = 16;
        return messagesPlaceHolder;
    }

    #endregion
}
