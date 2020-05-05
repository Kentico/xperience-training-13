using System;
using System.Linq;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSWebParts_Chat_ChatErrors : CMSAbstractWebPart
{
    bool mIsSupport = false;

    #region Properties

    /// <summary>
    /// Gets or sets ItemTemplate property.
    /// </summary>
    public string ErrorTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ErrorTransformationName"), ChatSettingsProvider.TransformationErrors);
        }
        set
        {
            this.SetValue("ErrorTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ButtonTemplate property.
    /// </summary>
    public string ButtonDeleteAllTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(this.GetValue("ButtonDeleteAllTransformationName"), ChatSettingsProvider.TransformationErrorsDeleteAll);
        }
        set
        {
            this.SetValue("ButtonDeleteAllTransformationName", value);
        }
    }


    /// <summary>
    /// Gets or sets ShowButton property.
    /// </summary>
    public bool ShowDeleteAllBtn
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowDeleteAllBtn"), false);
        }
        set
        {
            this.SetValue("ShowDeleteAllBtn", value);
        }
    }

    public string InnerContainerName { get; set; }
    public string InnerContainerTitle { get; set; }
    public bool IsSupport
    {
        get
        {
            return mIsSupport;
        }
        set
        {
            mIsSupport = value;
        }
    }

    #endregion


    protected void Page_Prerender(object sender, EventArgs e)
    {
        ChatUIHelper.MakeWebpartEnvelope("ChatWebpartEnvelope ChatWebpartEnvelopeErrors", this, InnerContainerTitle, InnerContainerName);
        if (IsSupport)
        {
            ChatCssHelper.RegisterStylesheet(Page, true);
        }
        else
        {
            ChatCssHelper.RegisterStylesheet(Page);
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // Registration to chat webservice
        AbstractCMSPage cmsPage = Page as AbstractCMSPage;
        if (cmsPage != null)
        {
            ChatScriptHelper.RegisterChatAJAXProxy(cmsPage);
        }

        // Script references insertion
        ChatScriptHelper.RegisterChatManager(Page);
        ScriptHelper.RegisterJQueryTemplates(Page);
        ScriptHelper.RegisterScriptFile(Page, "~/CMSWebParts/Chat/ChatErrors_files/ChatErrors.js");

        // Run script
        string startupScript = String.Format("InitErrorsWebpart({{errorTemplate:{0},contentClientID:{1}, clientID:'{2}', showDeleteAll:{3}, envelopeID: '#envelope_{2}' }});",
            ScriptHelper.GetString(ChatUIHelper.GetWebpartTransformation(ErrorTransformationName,"chat.error.transformation.errorwp.error")), 
            ScriptHelper.GetString("#" + pnlChatErrors.ClientID), 
            ClientID,
            ScriptHelper.GetString( ShowDeleteAllBtn ? ChatUIHelper.GetWebpartTransformation(ButtonDeleteAllTransformationName,"chat.error.transformation.errorwp.deleteallbtn") : "")
            );

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ChatErrors_" + ClientID, startupScript, true);
    }

}
