using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSInstall_Controls_LayoutPanels_Error : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Error label text.
    /// </summary>
    public string ErrorLabelText
    {
        get 
        {
            return lblError.Text;
        }
        set
        {
            lblError.Text = ResHelper.GetFileString(value);
            lblError.RemoveCssClass("hidden");
        }
    }


    /// <summary>
    /// Error label client ID.
    /// </summary>
    public string ErrorLabelClientID
    {
        get
        {
           return lblError.ClientID;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Displays error help.
    /// </summary>
    /// <param name="resourceString">Resource string</param>
    /// <param name="topic">Topic of the error</param>
    public void DisplayError(string resourceString, string topic)
    {
        hlpTroubleshoot.Text = ResHelper.GetFileString(resourceString);
        hlpTroubleshoot.TopicName = topic;
    }

    #endregion
}