using System;
using System.Threading;

using CMS.Helpers;
using CMS.Notifications.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;


public partial class CMSModules_Notifications_Controls_TemplateTextEdit : TemplateTextEdit
{
    #region "Public properties"

    /// <summary>
    /// Sets/Gets TemplateSubject textbox field.
    /// </summary>
    public override string TemplateSubject
    {
        get
        {
            return DataHelper.GetNotEmpty(txtSubject.Text, String.Empty);
        }
        set
        {
            txtSubject.Text = value;
        }
    }


    /// <summary>
    /// Sets/Gets TemplatePlainText textarea field.
    /// </summary>
    public override string TemplatePlainText
    {
        get
        {
            return DataHelper.GetNotEmpty(txtPlainText.Text, String.Empty);
        }
        set
        {
            txtPlainText.Text = value;
        }
    }


    /// <summary>
    /// Sets/Gets TemplateHTMLText textarea field.
    /// </summary>
    public override string TemplateHTMLText
    {
        get
        {
            return DataHelper.GetNotEmpty(htmlText.ResolvedValue, String.Empty);
        }
        set
        {
            htmlText.Value = value;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // If ID's not specified return 
        if ((TemplateID == 0) || (GatewayID == 0))
        {
            return;
        }

        // Get gateway name
        NotificationGatewayInfo ngi = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(GatewayID);
        if (ngi == null)
        {
            throw new Exception("NotificationGatewayInfo with this GatewayID does not exist.");
        }

        // Setup control according to NotificationGatewayInfo
        plcSubject.Visible = ngi.GatewaySupportsEmail;
        plcPlainText.Visible = ngi.GatewaySupportsPlainText;
        plcHTMLText.Visible = ngi.GatewaySupportsHTMLText;

        if (plcHTMLText.Visible)
        {
            // Initialize HTML editor
            htmlText.AutoDetectLanguage = false;
            htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            htmlText.EditorAreaCSS = PortalHelper.GetHtmlEditorAreaCss(SiteContext.CurrentSiteName);
            htmlText.ToolbarSet = "Basic";
            htmlText.MediaDialogConfig.UseFullURL = true;
            htmlText.LinkDialogConfig.UseFullURL = true;
            htmlText.QuickInsertConfig.UseFullURL = true;
        }

        // If gateway does not support any of text fields inform about it. 
        if (!ngi.GatewaySupportsEmail && !ngi.GatewaySupportsHTMLText && !ngi.GatewaySupportsPlainText)
        {
            ShowWarning(string.Format(GetString("notifications.templatetext.notextbox"), HTMLHelper.HTMLEncode(ngi.GatewayDisplayName)));
        }

        // Get existing TemplateTextInfoObject or create new object
        NotificationTemplateTextInfo ntti = NotificationTemplateTextInfoProvider.GetNotificationTemplateTextInfo(GatewayID, TemplateID);
        if (ntti == null)
        {
            ntti = new NotificationTemplateTextInfo();
        }

        // Set edited object
        EditedObject = ntti;

        // Setup properties
        if (!RequestHelper.IsPostBack())
        {
            TemplateSubject = ntti.TemplateSubject;
            TemplateHTMLText = ntti.TemplateHTMLText;
            TemplatePlainText = ntti.TemplatePlainText;
        }
    }

    #endregion
}