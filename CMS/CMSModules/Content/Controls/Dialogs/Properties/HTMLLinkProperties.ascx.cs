using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_HTMLLinkProperties : ItemProperties
{
    /// <summary>
    /// Gets or sets the value which determines whether to show or hide general tab.
    /// </summary>
    public bool ShowGeneralTab
    {
        get
        {
            return tabGeneral.Visible;
        }
        set
        {
            tabGeneral.Visible = value;
            tabGeneral.HeaderText = (value ? GetString("general.general") : string.Empty);
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether the control is displayed on the Web tab.
    /// </summary>
    public bool IsWeb
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsWeb"], false);
        }
        set
        {
            ViewState["IsWeb"] = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }
        else
        {
            pnlEmpty.Visible = true;
            pnlTabs.Visible = false;
            lblEmpty.Text = NoSelectionText;
        }
    }


    private void SetupControls()
    {
        if (IsWeb && !ShowGeneralTab)
        {
            pnlTabs.SelectedTabIndex = 1;
        }
        else
        {
            pnlTabs.SelectedTabIndex = 0;
        }

        if (!RequestHelper.IsPostBack() && IsWeb)
        {
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";
        }

        // Script for hiding target frame
        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "targetSelectedScript", ScriptHelper.GetScript(
            "function targetSelected() { " + "\n" +
            "    var txt = document.getElementById('" + txtTargetFrame.ClientID + "');" + "\n" +
            "    var lbl = document.getElementById('" + lblTargetFrame.ClientID + "');" + "\n" +
            "    var drp = document.getElementById('" + drpTarget.ClientID + "');" + "\n" +
            "    if ((drp != null) && (txt != null) && (lbl != null)) {" + "\n" +
            "        if (drp.value != \"frame\") {" + "\n" +
            "            txt.style.display = 'none';" + "\n" +
            "            lbl.style.display = 'none'; " + "\n" +
            "        } else {" + "\n" +
            "            txt.style.display = 'inline';" + "\n" +
            "            lbl.style.display = 'block';" + "\n" +
            "        }" + "\n" +
            "    }" + "\n" +
            "}"));

        btnHidden.Click += btnHidden_Click;
        tabGeneral.HeaderText = (ShowGeneralTab ? GetString("general.general") : string.Empty);
        tabAdvanced.HeaderText = GetString("dialogs.tab.advanced");
        tabTarget.HeaderText = GetString("dialogs.tab.target");

        lblEmpty.Text = NoSelectionText;

        if (ValidationHelper.GetBoolean(SessionHelper.GetValue("HideLinkText"), false))
        {
            urlSelectElem.LinkTextEnabled = false;
        }

        string postBackRef = ControlsHelper.GetPostBackEventReference(btnHidden, string.Empty);
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

        urlSelectElem.TextBoxUrl.Attributes["onchange"] = postBackRef;
        urlSelectElem.TextBoxUrl.Attributes["onkeydown"] = postBackKeyDownRef;
        urlSelectElem.TextBoxLinkText.Attributes["onchange"] = postBackRef;
        urlSelectElem.TextBoxLinkText.Attributes["onkeydown"] = postBackKeyDownRef;
        urlSelectElem.DropDownProtocol.Attributes["onchange"] = postBackRef;
        drpTarget.Attributes["onchange"] = "targetSelected();" + postBackRef;
        txtTargetFrame.Attributes["onchange"] = postBackRef;
        txtTargetFrame.Attributes["onkeydown"] = postBackKeyDownRef;
        txtAdvId.Attributes["onchange"] = postBackRef;
        txtAdvId.Attributes["onkeydown"] = postBackKeyDownRef;
        txtAdvName.Attributes["onchange"] = postBackRef;
        txtAdvName.Attributes["onkeydown"] = postBackKeyDownRef;
        txtAdvTooltip.Attributes["onchange"] = postBackRef;
        txtAdvTooltip.Attributes["onkeydown"] = postBackKeyDownRef;
        txtAdvStyleSheet.Attributes["onchange"] = postBackRef;
        txtAdvStyleSheet.Attributes["onkeydown"] = postBackKeyDownRef;
        txtAdvStyle.Attributes["onchange"] = postBackRef;
        txtAdvStyle.Attributes["onkeydown"] = postBackKeyDownRef;

        if (drpTarget.SelectedValue != "frame")
        {
            txtTargetFrame.Style.Add("display", "none");
            lblTargetFrame.Style.Add("display", "none");
        }
        else
        {
            txtTargetFrame.Style.Add("display", "inline");
            lblTargetFrame.Style.Add("display", "block");
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!StopProcessing)
        {
            // Load target dropdown with values
            drpTarget.Items.Add(new ListItem(GetString("dialogs.target.notset"), "notset"));
            drpTarget.Items.Add(new ListItem(GetString("dialogs.target.frame"), "frame"));
            drpTarget.Items.Add(new ListItem(GetString("dialogs.target.blank"), "_blank"));
            drpTarget.Items.Add(new ListItem(GetString("dialogs.target.self"), "_self"));
            drpTarget.Items.Add(new ListItem(GetString("dialogs.target.parent"), "_parent"));
            drpTarget.Items.Add(new ListItem(GetString("dialogs.target.top"), "_top"));
        }
    }


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        SaveSession();
    }


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


    #region "Overridden methods"

    /// <summary>
    /// Loads given link parameters.
    /// </summary>
    /// <param name="item">Media item</param>
    /// <param name="properties">Collection of properties</param>
    public override void LoadSelectedItems(MediaItem item, Hashtable properties)
    {
        // Display the properties
        pnlEmpty.Visible = false;
        pnlTabs.CssClass = "Dialog_Tabs";
        tabGeneral.HeaderText = (ShowGeneralTab ? GetString("general.general") : string.Empty);

        if (properties != null)
        {
            LoadProperties(properties);

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
        // Hide the link text
        urlSelectElem.LinkTextEnabled = false;
        SessionHelper.SetValue("HideLinkText", true);

        if (properties != null)
        {
            // Display the properties
            pnlEmpty.Visible = false;
            pnlTabs.CssClass = "Dialog_Tabs";
            // Load properties
            LoadProperties(properties);
        }
        SaveSession();
    }


    /// <summary>
    /// Loads the properties.
    /// </summary>
    /// <param name="properties">Properties collection</param>
    public override void LoadProperties(Hashtable properties)
    {
        if (properties != null)
        {
            #region "General tab"

            string linkText = ValidationHelper.GetString(properties[DialogParameters.LINK_TEXT], string.Empty);
            string linkProtocol = ValidationHelper.GetString(properties[DialogParameters.LINK_PROTOCOL], "other");
            string linkUrl = ValidationHelper.GetString(properties[DialogParameters.LINK_URL], string.Empty);

            urlSelectElem.LinkText = HttpUtility.HtmlDecode(linkText);
            urlSelectElem.LinkURL = linkUrl;
            urlSelectElem.LinkProtocol = linkProtocol;

            #endregion


            #region "Target tab"

            string linkTarget = ValidationHelper.GetString(properties[DialogParameters.LINK_TARGET], string.Empty);
            if (linkTarget == string.Empty)
            {
                linkTarget = "notset";
            }

            drpTarget.ClearSelection();
            ListItem liTarget = drpTarget.Items.FindByValue(linkTarget);
            if (liTarget != null)
            {
                liTarget.Selected = true;
            }
            else
            {
                if (linkTarget != "notset")
                {
                    // Select specific frame
                    drpTarget.SelectedIndex = 1;
                    txtTargetFrame.Text = HttpUtility.HtmlDecode(linkTarget);
                }
            }

            #endregion


            #region "Advanced tab"

            string linkId = ValidationHelper.GetString(properties[DialogParameters.LINK_ID], string.Empty);
            string linkName = ValidationHelper.GetString(properties[DialogParameters.LINK_NAME], string.Empty);
            string linkTooltip = ValidationHelper.GetString(properties[DialogParameters.LINK_TOOLTIP], string.Empty);
            string linkClass = ValidationHelper.GetString(properties[DialogParameters.LINK_CLASS], string.Empty);
            string linkStyle = ValidationHelper.GetString(properties[DialogParameters.LINK_STYLE], string.Empty);

            txtAdvId.Text = HttpUtility.HtmlDecode(linkId);
            txtAdvName.Text = HttpUtility.HtmlDecode(linkName);
            txtAdvTooltip.Text = HttpUtility.HtmlDecode(linkTooltip);
            txtAdvStyleSheet.Text = HttpUtility.HtmlDecode(linkClass);
            txtAdvStyle.Text = HttpUtility.HtmlDecode(linkStyle);

            #endregion


            #region "General items"

            EditorClientID = ValidationHelper.GetString(properties[DialogParameters.EDITOR_CLIENTID], string.Empty);

            #endregion
        }
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();


        #region "General tab"

        string url = urlSelectElem.LinkURL.Trim();
        retval[DialogParameters.LINK_TEXT] = urlSelectElem.LinkText;
        retval[DialogParameters.LINK_URL] = url.StartsWithCSafe("~/") ? UrlResolver.ResolveUrl(url) : url;
        retval[DialogParameters.LINK_PROTOCOL] = urlSelectElem.LinkProtocol;

        #endregion


        #region "Target tab"

        if (drpTarget.SelectedIndex > 0)
        {
            if (drpTarget.SelectedIndex == 1)
            {
                retval[DialogParameters.LINK_TARGET] = HTMLHelper.HTMLEncode(txtTargetFrame.Text.Trim().Replace("%", "%25"));
            }
            else
            {
                retval[DialogParameters.LINK_TARGET] = drpTarget.SelectedValue;
            }
        }
        else
        {
            retval[DialogParameters.LINK_TARGET] = string.Empty;
        }

        #endregion


        #region "Advanced tab"

        retval[DialogParameters.LINK_ID] = HTMLHelper.HTMLEncode(txtAdvId.Text.Trim().Replace("%", "%25"));
        retval[DialogParameters.LINK_NAME] = HTMLHelper.HTMLEncode(txtAdvName.Text.Trim().Replace("%", "%25"));
        retval[DialogParameters.LINK_TOOLTIP] = txtAdvTooltip.Text.Trim().Replace("%", "%25");
        retval[DialogParameters.LINK_CLASS] = HTMLHelper.HTMLEncode(txtAdvStyleSheet.Text.Trim().Replace("%", "%25"));
        string style = txtAdvStyle.Text.Replace(Environment.NewLine, string.Empty).Trim();
        style = style.TrimEnd(';') + ";";
        retval[DialogParameters.LINK_STYLE] = (style != ";" ? HTMLHelper.HTMLEncode(style.Replace("%", "%25")) : string.Empty);

        #endregion


        #region "General items"

        retval[DialogParameters.EDITOR_CLIENTID] = (String.IsNullOrEmpty(EditorClientID) ? string.Empty : EditorClientID.Replace("%", "%25"));

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

        pnlTabs.SelectedTabIndex = 0;

        urlSelectElem.LinkTextEnabled = true;
        urlSelectElem.LinkProtocol = "other";
        urlSelectElem.LinkText = string.Empty;
        urlSelectElem.LinkURL = string.Empty;

        txtTargetFrame.Text = string.Empty;
        drpTarget.SelectedIndex = 0;

        txtAdvId.Text = string.Empty;
        txtAdvName.Text = string.Empty;
        txtAdvStyle.Text = string.Empty;
        txtAdvStyleSheet.Text = string.Empty;
        txtAdvTooltip.Text = string.Empty;
    }

    #endregion
}
