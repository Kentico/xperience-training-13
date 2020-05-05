using System;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SocialMarketing;


public partial class CMSModules_SocialMarketing_FormControls_TwitterPostTextArea : FormEngineUserControl
{
    #region "Constants"

    private const int TWITTER_POST_MAX_LENGTH = 280;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets field value. You need to override this method to make the control work properly with the form.
    /// </summary>
    public override object Value
    {
        get
        {
            return textArea.Text;
        }
        set
        {
            textArea.Text = (string)value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            textArea.Enabled = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page init event.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!Enabled)
        {
            return;
        }

        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/SocialMarketing/FormControls/TwitterPostTextArea_files/twitter-text.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/SocialMarketing/FormControls/TwitterPostTextArea_files/TwitterPostTextArea.js");
        string script = String.Format(@"
TwitterPostTextAreaManagerInit({{
    shortUrlLength: {0}, 
    shortUrlHttpsLength: {1}, 
    postMaxLength: {2},
    control: {{ 
        textAreaId: '{3}', 
        labelId: '{4}',
        hiddenFieldId: '{5}',
    }}
}});",
            TwitterHelper.TwitterConfiguration.ShortUrlLength,
            TwitterHelper.TwitterConfiguration.ShortUrlLengthHttps,
            TWITTER_POST_MAX_LENGTH,
            textArea.ClientID,
            lblCharCount.ClientID,
            hdnCharCount.ClientID);
        ScriptHelper.RegisterStartupScript(Page, Page.GetType(), "TwitterPostTextArea_" + ClientID, script, true);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns true if entered data is valid. If data is invalid, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if (String.IsNullOrWhiteSpace(textArea.Text))
        {
            ValidationError = GetString("sm.twitter.posts.msg.postempty");

            return false;
        }

        ValidationError = String.Format(GetString("basicform.invalidlength"), TWITTER_POST_MAX_LENGTH);
        int textLength = ValidationHelper.GetInteger(hdnCharCount.Value, -1);

        return (textLength <= TWITTER_POST_MAX_LENGTH);
    }

    #endregion

}