using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_HTMLAnchorProperties : ItemProperties
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!StopProcessing)
        {
            if (!RequestHelper.IsPostBack())
            {
                // Load drop-down list data
                LoadAnchorNames();
                LoadAnchorIds();
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            if (ValidationHelper.GetBoolean(SessionHelper.GetValue("HideLinkText"), false))
            {
                txtLinkText.Visible = false;
                lblLinkText.Visible = false;
            }

            if (!RequestHelper.IsPostBack())
            {
                // Initialize controls
                SetupControls();
            }
        }
        else
        {
            Visible = false;
        }
    }


    #region "Private methods"

    /// <summary>
    /// Initializes all the nested controls.
    /// </summary>
    private void SetupControls()
    {
        rbAnchorName.Text = GetString("dialogs.anchor.byname");
        rbAnchorId.Text = GetString("dialogs.anchor.byid");
        rbAnchorText.Text = GetString("dialogs.anchor.bytext");

        // Select by default
        if (!RequestHelper.IsPostBack())
        {
            rbAnchorText.Checked = true;
            drpAnchorId.Enabled = false;
            drpAnchorName.Enabled = false;
        }

        Hashtable dialogParameters = SessionHelper.GetValue("DialogParameters") as Hashtable;
        if ((dialogParameters != null) && (dialogParameters.Count > 0))
        {
            // Hide the link text
            txtLinkText.Visible = false;
            lblLinkText.Visible = false;
            SessionHelper.SetValue("HideLinkText", true);

            LoadItemProperties(dialogParameters);
        }
        else
        {
            dialogParameters = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
            if ((dialogParameters != null) && (dialogParameters.Count > 0))
            {
                LoadItemProperties(dialogParameters);
            }
        }

        // Get reffernce causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, "");
        string raiseOnAction = " function RaiseHiddenPostBack(){" + postBackRef + ";}\n";

        ltlScript.Text = ScriptHelper.GetScript(raiseOnAction);

        postBackRef = ControlsHelper.GetPostBackEventReference(btnHiddenUpdate, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

        txtLinkText.Attributes["onchange"] = postBackRef;
        txtLinkText.Attributes["onkeydown"] = postBackKeyDownRef;
        rbAnchorName.InputAttributes["onchange"] = postBackRef;
        rbAnchorId.InputAttributes["onchange"] = postBackRef;
        rbAnchorText.InputAttributes["onchange"] = postBackRef;
        drpAnchorName.Attributes["onchange"] = postBackRef;
        drpAnchorId.Attributes["onchange"] = postBackRef;
        txtAnchorText.Attributes["onchange"] = postBackRef;
        txtAnchorText.Attributes["onkeydown"] = postBackKeyDownRef;
    }


    protected void btnHiddenUpdate_Click(object sender, EventArgs e)
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


    /// <summary>
    /// Loads available anchors by IDs passed from the underlying editor.
    /// </summary>
    private void LoadAnchorIds()
    {
        drpAnchorId.Items.Clear();

        // Get anchors by ID
        ArrayList anchorIds = (SessionHelper.GetValue("Ids") as ArrayList);
        if (anchorIds != null)
        {
            // Fill drop-down list with items
            foreach (string anchorId in anchorIds)
            {
                drpAnchorId.Items.Add(new ListItem(anchorId, anchorId));
            }
        }
        else
        {
            // If no items available
            string none = GetString("general.empty");

            drpAnchorId.Items.Add(new ListItem(none, none));
            drpAnchorId.Enabled = false;
            rbAnchorId.Enabled = false;
        }

        drpAnchorId.DataBind();
    }


    /// <summary>
    /// Loads available anchors by name passed from the underlying editor.
    /// </summary>
    private void LoadAnchorNames()
    {
        drpAnchorName.Items.Clear();

        // Get anchors by name
        ArrayList anchorNames = (SessionHelper.GetValue("Anchors") as ArrayList);
        if (anchorNames != null)
        {
            // Fill drop-down list with items
            foreach (string anchorName in anchorNames)
            {
                drpAnchorName.Items.Add(new ListItem(anchorName, anchorName));
            }
        }
        else
        {
            // If no items available
            string none = GetString("general.empty");

            drpAnchorName.Items.Add(new ListItem(none, none));
            drpAnchorName.Enabled = false;
            rbAnchorName.Enabled = false;
        }

        drpAnchorName.DataBind();
    }


    /// <summary>
    /// Setup radio buttons, dropdowns and text according to anchor type.
    /// </summary>
    /// <param name="type">Type of anchor ("name","id","text")</param>
    private void SelectAnchorType(string type)
    {
        if (!String.IsNullOrEmpty(type))
        {
            rbAnchorName.Checked = false;
            rbAnchorId.Checked = false;
            rbAnchorText.Checked = false;
            drpAnchorName.Enabled = false;
            drpAnchorId.Enabled = false;
            txtAnchorText.Enabled = false;
            switch (type.ToLowerCSafe())
            {
                case "name":
                    rbAnchorName.Checked = true;
                    drpAnchorName.Enabled = true;
                    break;
                case "id":
                    rbAnchorId.Checked = true;
                    drpAnchorId.Enabled = true;
                    break;
                case "text":
                default:
                    rbAnchorText.Checked = true;
                    txtAnchorText.Enabled = true;
                    break;
            }
        }
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Loads the properites into control.
    /// </summary>
    /// <param name="properties">Properties to load</param>
    public override void LoadItemProperties(Hashtable properties)
    {
        if (properties != null)
        {
            // Get link text and hide the textbox
            string linkText = ValidationHelper.GetString(properties[DialogParameters.ANCHOR_LINKTEXT], "");
            txtLinkText.Text = linkText;

            // If anchor by name is selected
            string anchorName = ValidationHelper.GetString(properties[DialogParameters.ANCHOR_NAME], "");
            if (!string.IsNullOrEmpty(anchorName))
            {
                bool found = false;
                foreach (ListItem item in drpAnchorName.Items)
                {
                    if (item.Value.ToLowerCSafe() == anchorName.ToLowerCSafe())
                    {
                        SelectAnchorType("name");
                        drpAnchorName.SelectedValue = anchorName;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    foreach (ListItem item in drpAnchorId.Items)
                    {
                        if (item.Value.ToLowerCSafe() == anchorName.ToLowerCSafe())
                        {
                            SelectAnchorType("id");
                            drpAnchorId.SelectedValue = anchorName;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    SelectAnchorType("text");
                    txtAnchorText.Text = anchorName.Replace(" ", "_");
                }
            }
        }
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();

        // Get selected information
        retval[DialogParameters.ANCHOR_LINKTEXT] = txtLinkText.Text.Replace("%", "%25");

        if (rbAnchorName.Checked)
        {
            retval[DialogParameters.ANCHOR_NAME] = drpAnchorName.SelectedValue.Replace("%", "%25");
        }
        else if (rbAnchorId.Checked)
        {
            retval[DialogParameters.ANCHOR_NAME] = drpAnchorId.SelectedValue.Replace("%", "%25");
        }
        else if (rbAnchorText.Checked)
        {
            retval[DialogParameters.ANCHOR_NAME] = txtAnchorText.Text.Replace(" ", "_").Replace("%", "%25");
        }
        retval[DialogParameters.EDITOR_CLIENTID] = QueryHelper.GetString(DialogParameters.EDITOR_CLIENTID, "").Replace("%", "%25");

        return retval;
    }


    /// <summary>
    /// Clears the properties form.
    /// </summary>
    public override void ClearProperties(bool hideProperties)
    {
        txtAnchorText.Text = "";
        txtLinkText.Text = "";
        drpAnchorId.SelectedIndex = 0;
        drpAnchorName.SelectedIndex = 0;
    }


    /// <summary>
    /// Validates the user input.
    /// </summary>
    public override bool Validate()
    {
        if (rbAnchorText.Checked && (txtAnchorText.Text.Trim() == ""))
        {
            lblError.Text = GetString("dialogs.anchor.emptyanchor");
            lblError.Visible = true;
            plnAnchorUpdate.Update();
            return false;
        }
        return true;
    }

    #endregion


    #region "Event handlers"

    protected void rbAnchorName_CheckedChanged(object sender, EventArgs e)
    {
        rbAnchorId.Checked = false;
        rbAnchorText.Checked = false;
        txtAnchorText.Enabled = false;
        drpAnchorId.Enabled = false;
        drpAnchorName.Enabled = true;
    }


    protected void rbAnchorId_CheckedChanged(object sender, EventArgs e)
    {
        rbAnchorName.Checked = false;
        rbAnchorText.Checked = false;
        txtAnchorText.Enabled = false;
        drpAnchorId.Enabled = true;
        drpAnchorName.Enabled = false;
    }


    protected void rbAnchorText_CheckedChanged(object sender, EventArgs e)
    {
        rbAnchorId.Checked = false;
        rbAnchorName.Checked = false;
        txtAnchorText.Enabled = true;
        drpAnchorId.Enabled = false;
        drpAnchorName.Enabled = false;
    }


    protected void hdnButton_Click(object sender, EventArgs e)
    {
        if (Validate())
        {
            // Get selected item information
            Hashtable properties = GetItemProperties();

            // Get JavaScript for inserting the item
            string script = CMSDialogHelper.GetAnchorItem(properties);
            if (!string.IsNullOrEmpty(script))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(script));
            }
        }
    }

    #endregion
}
