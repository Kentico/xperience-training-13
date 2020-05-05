using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_General_URLSelector : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the link URL. If url contains supported protocol, it is set to <see cref="LinkProtocol"/> property.
    /// </summary>
    public string LinkURL
    {
        get
        {
            return txtUrl.Text.Trim();
        }
        set
        {
            txtUrl.Text = ExtractSupportedProtocols(value);
        }
    }


    /// <summary>
    /// Gets or sets the link text.
    /// </summary>
    public string LinkText
    {
        get
        {
            return txtLinkText.Text.Trim();
        }
        set
        {
            txtLinkText.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the link text is enabled.
    /// </summary>
    public bool LinkTextEnabled
    {
        get
        {
            return plcLinkText.Visible;
        }
        set
        {
            plcLinkText.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the link protocol.
    /// </summary>
    public string LinkProtocol
    {
        get
        {
            if (drpProtocol.SelectedValue != "other")
            {
                return drpProtocol.SelectedValue;
            }
            else
            {
                return "";
            }
        }
        set
        {
            string protocol = value.TrimEnd('/') + "//";
            drpProtocol.ClearSelection();
            ListItem li = drpProtocol.Items.FindByValue(protocol);
            if (li != null)
            {
                li.Selected = true;
            }
            else
            {
                drpProtocol.SelectedIndex = 4;
            }
        }
    }


    /// <summary>
    /// Url textbox.
    /// </summary>
    public CMSTextBox TextBoxUrl
    {
        get
        {
            return txtUrl;
        }
        set
        {
            txtUrl = value;
        }
    }


    /// <summary>
    /// Link text textbox.
    /// </summary>
    public CMSTextBox TextBoxLinkText
    {
        get
        {
            return txtLinkText;
        }
        set
        {
            txtLinkText = value;
        }
    }


    /// <summary>
    /// Protocol dropdown list.
    /// </summary>
    public CMSDropDownList DropDownProtocol
    {
        get
        {
            return drpProtocol;
        }
        set
        {
            drpProtocol = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!StopProcessing)
        {
            // Load protocol dropdown with values
            drpProtocol.Items.Add(new ListItem(GetString("dialogs.protocol.http"), "http://"));
            drpProtocol.Items.Add(new ListItem(GetString("dialogs.protocol.https"), "https://"));
            drpProtocol.Items.Add(new ListItem(GetString("dialogs.protocol.ftp"), "ftp://"));
            drpProtocol.Items.Add(new ListItem(GetString("dialogs.protocol.news"), "news://"));
            drpProtocol.Items.Add(new ListItem(GetString("dialogs.protocol.other"), "other"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Detect URL JavaScript
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "URLProtocolDetection", ScriptHelper.GetScript(
                "function detectUrl (urlId, protocolId) { \n" +
                "    var re = new RegExp('^(http|https|ftp|news)://(.*)'); \n" +
                "    var urlElem = document.getElementById(urlId); \n" +
                "    var protocolElem = document.getElementById(protocolId); \n" +
                "    if ((urlElem != null) && (protocolElem != null)) { \n" +
                "        var url = urlElem.value.replace(re, '$2'); \n" +
                "        var protocol = urlElem.value.replace(re, '$1'); \n" +
                "        if (protocol != urlElem.value) { \n" +
                "            protocol += '://'; \n" +
                "            urlElem.value = url; \n" +
                "            protocolElem.value = protocol; \n" +
                "        } \n" +
                "    } \n" +
                "} \n"));

            txtUrl.Attributes["onchange"] = "detectUrl('" + txtUrl.ClientID + "','" + drpProtocol.ClientID + "');";
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns url without supported protocol (protocol is set to Protocol dropdown list). Returns original Url if protocol not specified or not supported.
    /// </summary>
    /// <param name="url">Url to extract protocol from.</param>
    private string ExtractSupportedProtocols(string url)
    {
        if(string.IsNullOrEmpty(url))
        {
            return url;
        }

        var shortUrl = URLHelper.RemoveProtocol(url);
        var protocol = url.Substring(0, url.Length - shortUrl.Length);
        if ((protocol == "http://") || (protocol == "https://") ||
            (protocol == "ftp://") || (protocol == "news://"))
        {
            LinkProtocol = protocol;
            return shortUrl;
        }

        return url;
    }

    #endregion
}
