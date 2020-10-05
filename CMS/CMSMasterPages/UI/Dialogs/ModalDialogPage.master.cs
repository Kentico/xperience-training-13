using System;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

public partial class CMSMasterPages_UI_Dialogs_ModalDialogPage : CMSMasterPage, ICMSModalMasterPage
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

            return base.ObjectEditMenu;
        }
    }


    /// <summary>
    /// Header container.
    /// </summary>
    public override Panel HeaderContainer
    {
        get
        {
            return pnlContainerHeader;
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
            return divContent;
        }
    }


    /// <summary>
    /// Gets the header panel
    /// </summary>
    public override Panel PanelHeader
    {
        get
        {
            return pnlHeader;
        }
    }


    /// <summary>
    /// Footer container.
    /// </summary>
    public override Panel FooterContainer
    {
        get
        {
            return pnlFooterContent;
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
    /// Panel containing title actions displayed above scrolling content.
    /// </summary>
    public override Panel PanelTitleActions
    {
        get
        {
            return pnlTitleActions;
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
    /// HeaderActionsPermissions place holder.
    /// </summary>
    public override UIPlaceHolder HeaderActionsPlaceHolder
    {
        get
        {
            return plcActionsPermissions;
        }
    }


    /// <summary>
    /// Footer panel.
    /// </summary>
    public override Panel PanelFooter
    {
        get
        {
            return pnlFooter;
        }
    }


    /// <summary>
    /// Fired when 'Save & close' button is clicked and the content should be saved. Pages that use this master page should add handler to this event
    /// alike binding to save button <see cref="Button.OnClick"/> event.
    /// </summary>
    public event EventHandler Save;

    #endregion


    #region "Public methods"

    /// <summary>
    /// Sets JavaScript to the "Save & close" button.
    /// </summary>
    /// <param name="javaScript">JavaScript to add to the Save & Close button</param>
    public void SetSaveJavaScript(string javaScript)
    {
        btnSaveAndClose.OnClientClick = javaScript;
    }


    /// <summary>
    /// Sets JavaScript to be processed when user clicks the "Close" button or the area around the modal window.
    /// </summary>
    /// <param name="javaScript">JavaScript to be processed when user clicks the Close button or the area around the modal window</param>
    public void SetCloseJavaScript(string javaScript)
    {
        titleElem.SetCloseJavaScript(javaScript);
    }


    /// <summary>
    /// Shows Save & Close button at the bottom of the page.
    /// </summary>
    public void ShowSaveAndCloseButton()
    {
        btnSaveAndClose.Visible = true;
    }


    /// <summary>
    /// Sets Save & Close button resource string.
    /// </summary>
    /// <param name="resourceString">Resource string</param>
    public void SetSaveResourceString(string resourceString)
    {
        btnSaveAndClose.ResourceString = resourceString;
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

        bodyElem.Attributes["class"] = mBodyClass;

        // Footer - apply fixed position
        pnlFooterContent.Style.Add("position", "fixed");
        pnlFooterContent.Style.Add("width", "100%");
        pnlFooterContent.Style.Add("bottom", "0px");

        StringBuilder resizeScript = new StringBuilder();
        resizeScript.Append(@"
var headerElem = null;
var footerElem = null;
var contentElem = null;
var jIframe = null;
var jIframeContents = null;
var oldClientWidth = 0;
var oldClientHeight = 0;
var dialogCMSHeaderPad = null;
var dialogCKFooter = null;


function ResizeWorkingAreaIE()
{
    ResizeWorkingArea();
    window.onresize = function() { ResizeWorkingArea(); };
}

function ResizeWorkingArea()
{
    if (headerElem == null)
    {
       headerElem = document.getElementById('divHeader');
    }
    if (footerElem == null)
    {
       footerElem = document.getElementById('divFooter');
    }
    if (contentElem == null)
    {
       contentElem = document.getElementById('divContent');
    }

    if (dialogCMSHeaderPad == null)
    {
       dialogCMSHeaderPad = document.getElementById('CMSHeaderPad');
    }

    if (dialogCKFooter == null)
    {
       dialogCKFooter = document.getElementById('CKFooter');
    }

    if ((headerElem != null) && (contentElem != null))
    {
        var headerHeight = headerElem.offsetHeight + ((dialogCMSHeaderPad != null) ? dialogCMSHeaderPad.offsetHeight : 0);
        var footerHeight = ((footerElem != null) ? footerElem.offsetHeight : 0) + ((dialogCKFooter != null) ? dialogCKFooter.offsetHeight : 0);
        var height = ($cmsj(window).height() - headerHeight - footerHeight);
        if (height > 0)
        {
            var h = (height > 0 ? height : '0') + 'px';
            if (contentElem.style.height != h)
            {
                contentElem.style.height = h;
            }
        }");

#pragma warning disable CS0618 // Type or member is obsolete
        if (BrowserHelper.IsIE())
#pragma warning restore CS0618 // Type or member is obsolete
        {
            resizeScript.Append(@"
        var pnlBody = null;
        var formElem = null;
        var bodyElement = null;
        if (pnlBody == null)
        {
            pnlBody = document.getElementById('", pnlBody.ClientID, @"');
        }
        if (formElem == null)
        {
            formElem = document.getElementById('", form1.ClientID, @"');
        }
        if (bodyElement == null)
        {
            bodyElement = document.getElementById('", bodyElem.ClientID, @"');
        }
        if ((bodyElement != null) && (formElem != null) && (pnlBody != null))
        {
            var newClientWidth = document.documentElement.clientWidth;
            var newClientHeight = document.documentElement.clientHeight;
            if  (newClientWidth != oldClientWidth)
            {
                bodyElement.style.width = newClientWidth;
                formElem.style.width = newClientWidth;
                pnlBody.style.width = newClientWidth;
                headerElem.style.width = newClientWidth;
                contentElem.style.width = newClientWidth;
                oldClientWidth = newClientWidth;
            }
            if  (newClientHeight != oldClientHeight)
            {
                bodyElement.style.height = newClientHeight;
                formElem.style.height = newClientHeight;
                pnlBody.style.height = newClientHeight;
                oldClientHeight = newClientHeight;
            }
        }");
        }

        resizeScript.Append(@"
    }
    if (window.afterResize) {
        window.afterResize();
    }
}");
#pragma warning disable CS0618 // Type or member is obsolete
        if (BrowserHelper.IsIE())
#pragma warning restore CS0618 // Type or member is obsolete
        {
            resizeScript.Append(@"
var timer = setInterval('ResizeWorkingAreaIE();', 50);");
        }
        else
        {
            resizeScript.Append(@"
window.onresize = function() { ResizeWorkingArea(); };
window.onload = function() { ResizeWorkingArea(); };");
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "resizeScript", ScriptHelper.GetScript(resizeScript.ToString()));

        // Register a script that will re-calculate content height when the CKToolbar is displayed
        const string ckEditorScript = @"
if (window.CKEDITOR) {
    CKEDITOR.on('instanceCreated', function(e) {
        e.editor.on('instanceReady', function(e) { setTimeout(function() { ResizeWorkingArea(); }, 200); });
    });
}";

        ScriptHelper.RegisterStartupScript(this, typeof(string), "ckEditorScript", ckEditorScript, true);

        // Register header shadow script
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");
    }


    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        // Hide actions panel if no actions are present and DisplayActionsPanel is false
        if (!DisplayActionsPanel)
        {
            if (!HeaderActions.IsVisible() && (plcActions.Controls.Count == 0))
            {
                pnlActions.Visible = false;
            }
        }

        base.Render(writer);
    }


    /// <summary>
    /// Fires <see cref="ICMSModalMasterPage.Save"/>.
    /// </summary>
    protected void btnSaveAndClose_OnClick(object sender, EventArgs e)
    {
        if (Save != null)
        {
            Save(sender, e);
        }
    }

    #endregion
}
