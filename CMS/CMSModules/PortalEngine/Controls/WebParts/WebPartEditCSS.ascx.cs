using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartEditCSS : CMSPreviewControl
{
    #region "Variables"

    private WebPartInfo wpi = null;
    private int previewState = 0;
    protected bool startWithFullScreen = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        previewState = GetPreviewStateFromCookies(WEBPARTCSS);
        startWithFullScreen = previewState != 0;

        // Hide to better fullscreen load
#pragma warning disable CS0618 // Type or member is obsolete
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
#pragma warning restore CS0618 // Type or member is obsolete
        pnlContent.Attributes["style"] = (startWithFullScreen && !hide) ? "display:none" : "display:block";

        // Register preview scripts
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "performAction", ScriptHelper.GetScript("function actionPerformed(action) { " + Page.ClientScript.GetPostBackEventReference(btnAction, "#").Replace("'#'", "action") + ";}"));

        wpi = UIContext.EditedObject as WebPartInfo;

        if ((wpi != null) && !RequestHelper.IsPostBack())
        {
            etaCSS.Text = wpi.WebPartCSS;
        }

        InitHeaderActions();
        RegisterInitScripts(pnlContent.ClientID, pnlMenu.ClientID, startWithFullScreen);
    }


    /// <summary>
    /// Init menu
    /// </summary>
    private void InitHeaderActions()
    {
        // Save action
        SaveAction save = new SaveAction();
        headerActions.ActionsList.Add(save);

        headerActions.ActionPerformed += (sender, e) =>
        {
            if (e.CommandName == ComponentEvents.SAVE)
            {
                Save();
            }
        };
    }


    /// <summary>
    /// Handle btnOK's OnClick event.
    /// </summary>
    protected void Save()
    {
        string errorMessage = "";

        if (wpi != null)
        {
            // Update web part CSS
            try
            {
                wpi.WebPartCSS = etaCSS.Text;
                WebPartInfoProvider.SetWebPartInfo(wpi);
                ShowChangesSaved();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            // Show error message
            if (errorMessage != "")
            {
                ShowError(errorMessage);
            }

            RegisterRefreshScript();
        }
    }


    protected void btnAction_Clicked(object sender, EventArgs ea)
    {
        Save();
    }

    #endregion
}
