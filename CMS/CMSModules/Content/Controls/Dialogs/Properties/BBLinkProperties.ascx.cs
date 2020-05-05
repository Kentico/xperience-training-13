using System;
using System.Collections;
using System.Web;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_BBLinkProperties : ItemProperties
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            tabGeneral.HeaderText = GetString("general.general");
            lblEmpty.Text = NoSelectionText;

            btnHidden.Click += btnHidden_Click;
            string postBackRef = ControlsHelper.GetPostBackEventReference(btnHidden, "");
            string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

            urlSelectElem.TextBoxLinkText.Attributes["onchange"] = postBackRef;
            urlSelectElem.TextBoxLinkText.Attributes["onkeydown"] = postBackKeyDownRef;
            urlSelectElem.TextBoxUrl.Attributes["onchange"] = postBackRef;
            urlSelectElem.TextBoxUrl.Attributes["onkeydown"] = postBackKeyDownRef;
            urlSelectElem.DropDownProtocol.Attributes["onchange"] = postBackRef;
            urlSelectElem.DropDownProtocol.Attributes["onkeydown"] = postBackKeyDownRef;
        }
    }


    private void btnHidden_Click(object sender, EventArgs e)
    {
        SaveSession();
    }


    #region "Private methods"

    /// <summary>
    /// Save current properties into session.
    /// </summary>
    private void SaveSession()
    {
        Hashtable savedProperties = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable ?? new Hashtable();
        Hashtable properties = GetItemProperties();
        foreach (DictionaryEntry entry in properties)
        {
            savedProperties[entry.Key] = entry.Value;
        }
        SessionHelper.SetValue("DialogSelectedParameters", savedProperties);
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Loads given link parameters.
    /// </summary>
    public override void LoadSelectedItems(MediaItem item, Hashtable properties)
    {
        LoadProperties(properties);
        if (item.Url != null)
        {
            urlSelectElem.LinkURL = item.Url;
            urlSelectElem.LinkText = item.Name;
        }
        SaveSession();
    }


    /// <summary>
    /// Loads the properties into control.
    /// </summary>
    /// <param name="properties">Collection of properties</param>
    public override void LoadItemProperties(Hashtable properties)
    {
        LoadProperties(properties);
    }


    /// <summary>
    /// Loads the properties into control.
    /// </summary>
    /// <param name="properties">Collection of properties</param>
    public override void LoadProperties(Hashtable properties)
    {
        if (properties != null)
        {
            // Display the properties
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";


            #region "General tab"

            string linkText = ValidationHelper.GetString(properties[DialogParameters.LINK_TEXT], "");
            string linkProtocol = ValidationHelper.GetString(properties[DialogParameters.LINK_PROTOCOL], "other");
            string linkUrl = ValidationHelper.GetString(properties[DialogParameters.LINK_URL], "");

            urlSelectElem.LinkText = linkText;
            urlSelectElem.LinkURL = linkUrl;
            urlSelectElem.LinkProtocol = linkProtocol;

            #endregion


            #region "General items"

            EditorClientID = ValidationHelper.GetString(properties[DialogParameters.EDITOR_CLIENTID], "");

            #endregion
        }

        SaveSession();
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();


        #region "General tab"
        
        string url = urlSelectElem.LinkURL.Trim();
        retval[DialogParameters.LINK_TEXT] = HttpUtility.HtmlEncode(urlSelectElem.LinkText);
        retval[DialogParameters.LINK_URL] = url.StartsWith("~/", StringComparison.Ordinal) ? UrlResolver.ResolveUrl(url) : url;
        retval[DialogParameters.LINK_PROTOCOL] = urlSelectElem.LinkProtocol;

        #endregion


        #region "General items"

        retval[DialogParameters.EDITOR_CLIENTID] = EditorClientID;

        #endregion


        return retval;
    }


    /// <summary>
    /// Clears the properties form.
    /// </summary>
    public override void ClearProperties(bool hideProperties)
    {
        // Hide the properties
        pnlEmpty.Visible = hideProperties;
        pnlTabs.CssClass = (hideProperties ? "DialogElementHidden" : "Dialog_Tabs");

        urlSelectElem.LinkText = "";
        urlSelectElem.LinkURL = "";
        urlSelectElem.LinkProtocol = "other";
    }

    #endregion
}