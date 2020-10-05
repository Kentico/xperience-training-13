using System;
using System.Net;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SharePoint;


public partial class CMSModules_SharePoint_FormControls_ConnectionTest : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// SharePoint site URL currently filled in the form.
    /// </summary>
    private string SharePointConnectionSiteUrl
    {
        get
        {
            return ValidationHelper.GetString(Form.GetFieldValue("SharePointConnectionSiteUrl"), String.Empty);
        }
    }


    /// <summary>
    /// SharePoint user name currently filled in the form.
    /// </summary>
    private string SharePointConnectionUserName
    {
        get
        {
            return ValidationHelper.GetString(Form.GetFieldValue("SharePointConnectionUserName"), String.Empty);
        }
    }


    /// <summary>
    /// SharePoint password currently filled in the form.
    /// </summary>
    private string SharePointConnectionPassword
    {
        get
        {
            string encryptedPassword = ValidationHelper.GetString(Form.GetFieldValue("SharePointConnectionPassword"), String.Empty);
            return EncryptionHelper.DecryptData(encryptedPassword);
        }
    }


    /// <summary>
    /// Does not really matter.
    /// </summary>
    public override object Value
    {
        get;
        set;
    }

    #endregion


    #region "Life-cycle methods"
    
    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterScripts();
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Handles button click - runs connection test.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event args</param>
    protected void btnTestConnection_OnClick(object sender, EventArgs e)
    {
        TestConnection();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Tests SharePoint connection using configuration currently filled in the form.
    /// </summary>
    private void TestConnection()
    {
        try
        {
            ISharePointSiteService siteService = SharePointServices.GetService<ISharePointSiteService>(GetConnectionData());

            siteService.GetSiteUrl();

            DisplayConnectionStatus(GetString("sharepoint.testconnection.success"));
        }
        catch (SharePointServiceFactoryNotSupportedException)
        {
            // No service factory for given SharePoint version
            DisplayConnectionStatus(GetString("sharepoint.versionnotsupported"), false);
        }
        catch (SharePointServiceNotSupportedException)
        {
            // No ISiteService implementation for SharePoint version
            DisplayConnectionStatus(GetString("sharepoint.testconnectionnotsupported"), false);
        }
        catch (SharePointConnectionNotSupportedException)
        {
            // The ISiteService implementation rejected connection data
            DisplayConnectionStatus(GetString("sharepoint.invalidconfiguration"), false);
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                // Connection established, but response indicates error condition
                if (ex.Message.Contains("401"))
                {
                    // Unauthorized.
                    DisplayConnectionStatus(GetString("sharepoint.protocolerror.unauthorized"), false);
                }
                else if (ex.Message.Contains("404"))
                {
                    // SharePoint instance not found on given URL
                    DisplayConnectionStatus(GetString("sharepoint.protocolerror.notfound"), false);
                }
                else
                {
                    // Some other protocol error
                    DisplayConnectionStatus(GetString("sharepoint.protocolerror"), false);
                }
            }
            else if (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                // Given site URL does not have a resolution
                DisplayConnectionStatus(GetString("sharepoint.nameresolutionfailure"), false);
            }
            else
            {
                DisplayConnectionStatus(GetString("sharepoint.unknownerror"), false);
                Service.Resolve<IEventLogService>().LogException("SharePoint", "TESTCONNECTION", ex);
            }
        }
        catch (Exception ex)
        {
            DisplayConnectionStatus(GetString("sharepoint.unknownerror"), false);
            Service.Resolve<IEventLogService>().LogException("SharePoint", "TESTCONNECTION", ex);
        }
    }


    /// <summary>
    /// Gets connection data constructed from actual values of the form fields.
    /// </summary>
    /// <returns>Connection data</returns>
    private SharePointConnectionData GetConnectionData()
    {
        var connectionData = new SharePointConnectionData()
        {
            SharePointConnectionPassword = SharePointConnectionPassword,
            SharePointConnectionSiteUrl = SharePointConnectionSiteUrl,
            SharePointConnectionUserName = SharePointConnectionUserName
        };

        return connectionData;
    }


    /// <summary>
    /// Displays conection test result message.
    /// </summary>
    /// <param name="message">Result message</param>
    /// <param name="success">True if test succeeded, false otherwise</param>
    /// <param name="messageIsHtmlEncoded">Indicates whether message is HTML encoded. If not, it will be encoded.</param>
    private void DisplayConnectionStatus(string message, bool success = true, bool messageIsHtmlEncoded = false)
    {
        StringBuilder status = new StringBuilder();
        if (success)
        {
            status.Append(UIHelper.GetAccessibleIconTag("icon-check-circle", GetString("sharepoint.testconnection.success"), FontIconSizeEnum.NotDefined, "sharepoint-connection-success")).
                Append("<span class=\"sharepoint-connection-success-text\">");
        }
        else
        {
            status.Append(UIHelper.GetAccessibleIconTag("icon-times-circle", GetString("sharepoint.testconnection.failure"), FontIconSizeEnum.NotDefined, "sharepoint-connection-error")).
                Append("<span class=\"sharepoint-connection-error-text\">"); ;
        }
        if (!messageIsHtmlEncoded)
        {
            message = HTMLHelper.HTMLEncode(message);
        }
        status.Append(message).Append("</span>");

        lblConnectionStatus.Text = status.ToString();
    }


    /// <summary>
    /// Registers JavaScript for connection test reporting.
    /// </summary>
    private void RegisterScripts()
    {
        object testConnectionData = new
        {
            testButtonId = btnTestConnection.ClientID,
            statusLabelId = lblConnectionStatus.ClientID,
            inprogressMessage = GetString("sharepoint.testconnection.progressmessage")
        };

        ScriptHelper.RegisterModule(this, "CMSSharePoint/ConnectionTest", testConnectionData);
    }

    #endregion
}