using System;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSMasterPages_LiveSite_Dialogs_ModalDialogPage : AbstractMasterPage
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
    /// Content panel
    /// </summary>
    public override Panel PanelContent
    {
        get
        {
            return divContent;
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
    /// Panel containing title actions disaplyed above scrolling content.
    /// </summary>
    public override Panel PanelTitleActions
    {
        get
        {
            return pnlTitleActions;
        }
    }


    /// <summary>
    /// Footer container.
    /// </summary>
    public override Panel FooterContainer
    {
        get
        {
            return pnlFooterContainer;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        // Display panel with additional controls place holder if required
        if (DisplayControlsPanel)
        {
            pnlAdditionalControls.Visible = true;
        }
        bodyElem.Attributes["class"] = mBodyClass;

        StringBuilder resizeScript = new StringBuilder();
        resizeScript.Append(@"
var headerElem = null;
var footerElem = null;
var contentElem = null;
var oldClientWidth = 0;
var oldClientHeight = 0;

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
   if ((headerElem != null) && (contentElem != null))
   {
       var headerHeight = $cmsj('.CMSFixPanel').height();
       var footerHeight = (footerElem != null) ? footerElem.offsetHeight : 0;
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
            resizeScript.AppendFormat(@"
       var pnlBody = null;
       var formElem = null;
       var bodyElement = null;
       if (pnlBody == null)
       {{
          pnlBody = document.getElementById('{0}');
       }}
       if (formElem == null)
       {{
          formElem = document.getElementById('{1}');
       }}
       if (bodyElement == null)
       {{
          bodyElement = document.getElementById('{2}');
       }}
       if ((bodyElement != null) && (formElem != null) && (pnlBody != null))
       {{
           var newClientWidth = document.documentElement.clientWidth;
           var newClientHeight = document.documentElement.clientHeight;
           if  (newClientWidth != oldClientWidth)
           {{
               bodyElement.style.width = newClientWidth;
               formElem.style.width = newClientWidth;
               pnlBody.style.width = newClientWidth;
               headerElem.style.width = newClientWidth;
               contentElem.style.width = newClientWidth;
               oldClientWidth = newClientWidth;
           }}
           if  (newClientHeight != oldClientHeight)
           {{
               bodyElement.style.height = newClientHeight;
               formElem.style.height = newClientHeight;
               pnlBody.style.height = newClientHeight;
               oldClientHeight = newClientHeight;
           }}
       }}", pnlBody.ClientID, form1.ClientID, bodyElem.ClientID);
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
}
