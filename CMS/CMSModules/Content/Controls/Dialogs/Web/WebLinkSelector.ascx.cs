using System;
using System.Collections;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Web_WebLinkSelector : CMSUserControl
{
    #region "Variables"

    private DialogConfiguration mConfig = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns whether the output format is BB.
    /// </summary>
    public bool IsBB
    {
        get
        {
            return (Config.OutputFormat == OutputFormatEnum.BBLink);
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current dialog configuration.
    /// </summary>
    public DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = DialogConfiguration.GetDialogConfiguration();
            }
            return mConfig;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Display the properties correctly
            pnlProperties.Visible = !IsBB;

            string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, "");
            ltlScript.Text = ScriptHelper.GetScript("function RaiseHiddenPostBack(){" + postBackRef + ";}\n");

            if (!RequestHelper.IsPostBack())
            {
                Hashtable selectedItem = SessionHelper.GetValue("DialogParameters") as Hashtable;
                if ((selectedItem != null) && (selectedItem.Count > 0))
                {
                    LoadSelectedItem(selectedItem, true);
                    //SessionHelper.SetValue("DialogParameters", null);
                }
            }
        }
    }


    protected void hdnButton_Click(object sender, EventArgs e)
    {
        string script = CMSDialogHelper.GetLinkItem(GetSelectedItem());
        if (!String.IsNullOrEmpty(script))
        {
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(script));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns selected item parameters as name-value collection.
    /// </summary>
    public Hashtable GetSelectedItem()
    {
        Hashtable retval = (IsBB ? new Hashtable() : propLinkProperties.GetItemProperties());

        retval[DialogParameters.LINK_PROTOCOL] = urlSelectElem.LinkProtocol;
        retval[DialogParameters.LINK_URL] = urlSelectElem.LinkURL;
        retval[DialogParameters.LINK_TEXT] = urlSelectElem.LinkText;

        retval[DialogParameters.EDITOR_CLIENTID] = Config.EditorClientID;

        return retval;
    }


    /// <summary>
    /// Loads selected item parameters into the selector.
    /// </summary>
    /// <param name="properties">Name-value collection representing item to load</param>
    /// <param name="hideLinkText">Indicates if link text textbox should be hidden</param>
    public void LoadSelectedItem(Hashtable properties, bool hideLinkText)
    {
        if ((properties != null) && (properties.Count > 0))
        {
            if (!IsBB)
            {
                if (hideLinkText)
                {
                    propLinkProperties.LoadItemProperties(properties);
                }
                else
                {
                    MediaItem mi = new MediaItem();
                    mi.Url = properties[DialogParameters.LINK_URL].ToString();
                    if ((properties[DialogParameters.LINK_TEXT] == null) || (String.IsNullOrEmpty(properties[DialogParameters.LINK_TEXT].ToString())))
                    {
                        mi.Name = properties[DialogParameters.LINK_NAME].ToString();
                    }
                    else
                    {
                        mi.Name = properties[DialogParameters.LINK_TEXT].ToString();
                    }
                    propLinkProperties.LoadSelectedItems(mi, properties);
                }
            }

            urlSelectElem.LinkTextEnabled = !hideLinkText;
            string url = ValidationHelper.GetString(properties[DialogParameters.LINK_URL], "");
            urlSelectElem.LinkURL = url;
            urlSelectElem.LinkText = ValidationHelper.GetString(properties[DialogParameters.LINK_TEXT], "");
            urlSelectElem.LinkProtocol = ValidationHelper.GetString(properties[DialogParameters.LINK_PROTOCOL], "http://");
        }
    }

    #endregion
}
