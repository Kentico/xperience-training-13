using System;
using System.Threading;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Documentation : GlobalAdminPage
{
    private int webpartId;


    protected void Page_Load(object sender, EventArgs e)
    {
        webpartId = QueryHelper.GetInteger("webpartid", 0);

        Title = "Web part documentation";

        // Resource string
        btnOk.Text = GetString("General.Ok");
        
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webpartId);
        EditedObject = wpi;
        if (wpi != null)
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("webparteditdocumentation.view");
            action.RedirectUrl = "~/CMSModules/PortalEngine/UI/WebParts/WebPartDocumentationPage.aspx?webpartid=" + wpi.WebPartName;
            action.Target = "_blank";
            CurrentMaster.HeaderActions.AddAction(action);
        }

        // HTML editor settings        
        htmlText.AutoDetectLanguage = false;
        htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlText.EditorAreaCSS = "";
        htmlText.ToolbarSet = "SimpleEdit";

        // Load data
        if (!RequestHelper.IsPostBack() && (wpi != null))
        {
            htmlText.ResolvedValue = wpi.WebPartDocumentation;
        }
    }


    /// <summary>
    /// OK click handler, save changes.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webpartId);
        if (wpi == null)
        {
            return;
        }

        wpi.WebPartDocumentation = htmlText.ResolvedValue;
        WebPartInfoProvider.SetWebPartInfo(wpi);

        ShowChangesSaved();
    }
}