using System;
using System.Threading;

using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Widgets_UI_Widget_Edit_Documentation : GlobalAdminPage
{
    #region "Variables"

    private int widgetId;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        widgetId = QueryHelper.GetInteger("widgetid", 0);

        Title = "Widget part documentation";

        // Resource string
        btnOk.Text = GetString("General.Ok");

        WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetId);

        // set Documentation header - "View documentation" + "Generate Documentation"
        if (wi != null)
        {

            HeaderAction action = new HeaderAction();
            action.Text = GetString("webparteditdocumentation.view");
            action.RedirectUrl = "~/CMSModules/Widgets/Dialogs/WidgetDocumentation.aspx?widgetid=" + wi.WidgetID;
            action.Target = "_blank";
            CurrentMaster.HeaderActions.AddAction(action);
        }
        
        // HTML editor settings
        htmlText.AutoDetectLanguage = false;
        htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlText.EditorAreaCSS = "";
        htmlText.ToolbarSet = "SimpleEdit";

        // Load data
        if (!RequestHelper.IsPostBack() && (wi != null))
        {
            htmlText.ResolvedValue = wi.WidgetDocumentation;
        }
    }


    /// <summary>
    /// OK click handler, save changes.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetId);
        if (wi == null)
        {
            return;
        }

        wi.WidgetDocumentation = htmlText.ResolvedValue;
        WidgetInfoProvider.SetWidgetInfo(wi);

        ShowChangesSaved();
    }

    #endregion
}