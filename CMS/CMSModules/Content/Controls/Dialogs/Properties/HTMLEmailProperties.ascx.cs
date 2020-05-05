using System;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Properties_HTMLEmailProperties : ItemProperties
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get reffernce causing postback to hidden button
        string postBackRef = ControlsHelper.GetPostBackEventReference(hdnButton, "");
        string raiseOnAction = " function RaiseHiddenPostBack(){" + postBackRef + ";}\n";

        ltlScript.Text = ScriptHelper.GetScript(raiseOnAction);

        if (ValidationHelper.GetBoolean(SessionHelper.GetValue("HideLinkText"), false))
        {
            plcLinkText.Visible = false;
        }

        if (!RequestHelper.IsPostBack())
        {
            Hashtable properties = SessionHelper.GetValue("DialogParameters") as Hashtable;
            if ((properties != null) && (properties.Count > 0))
            {
                // Hide the link text
                plcLinkText.Visible = false;
                SessionHelper.SetValue("HideLinkText", true);

                LoadItemProperties(properties);
            }
            else
            {
                properties = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
                if ((properties != null) && (properties.Count > 0))
                {
                    LoadItemProperties(properties);
                }
            }
        }

        postBackRef = ControlsHelper.GetPostBackEventReference(hdnButtonUpdate, "");
        string postBackKeyDownRef = "var keynum;if(window.event){keynum = event.keyCode;}else if(event.which){keynum = event.which;}if(keynum == 13){" + postBackRef + "; return false;}";

        txtLinkText.Attributes["onchange"] = postBackRef;
        txtLinkText.Attributes["onkeydown"] = postBackKeyDownRef;
        txtTo.Attributes["onchange"] = postBackRef;
        txtTo.Attributes["onkeydown"] = postBackKeyDownRef;
        txtCc.Attributes["onchange"] = postBackRef;
        txtCc.Attributes["onkeydown"] = postBackKeyDownRef;
        txtBcc.Attributes["onchange"] = postBackRef;
        txtBcc.Attributes["onkeydown"] = postBackKeyDownRef;
        txtSubject.Attributes["onchange"] = postBackRef;
        txtSubject.Attributes["onkeydown"] = postBackKeyDownRef;
        txtBody.Attributes["onchange"] = postBackRef;
    }


    protected void hdnButton_Click(object sender, EventArgs e)
    {
        if (Validate())
        {
            // Get selected item information
            Hashtable properties = GetItemProperties();

            // Get JavaScript for inserting the item
            string script = CMSDialogHelper.GetEmailItem(properties);
            if (!string.IsNullOrEmpty(script))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "insertItemScript", ScriptHelper.GetScript(script));
            }
        }
    }


    protected void hdnButtonUpdate_Click(object sender, EventArgs e)
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

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Loads the properites into control.
    /// </summary>
    /// <param name="properties">Hashtable with properties</param>
    public override void LoadItemProperties(Hashtable properties)
    {
        if (properties != null)
        {
            string linkText = ValidationHelper.GetString(properties[DialogParameters.EMAIL_LINKTEXT], "");
            string linkSubject = ValidationHelper.GetString(properties[DialogParameters.EMAIL_SUBJECT], "");
            string linkTo = ValidationHelper.GetString(properties[DialogParameters.EMAIL_TO], "");
            string linkCc = ValidationHelper.GetString(properties[DialogParameters.EMAIL_CC], "");
            string linkBcc = ValidationHelper.GetString(properties[DialogParameters.EMAIL_BCC], "");
            string linkBody = ValidationHelper.GetString(properties[DialogParameters.EMAIL_BODY], "");

            txtLinkText.Text = linkText;
            txtSubject.Text = HttpUtility.UrlDecode(linkSubject);
            txtCc.Text = HttpUtility.UrlDecode(linkCc);
            txtBcc.Text = HttpUtility.UrlDecode(linkBcc);
            txtTo.Text = HttpUtility.UrlDecode(linkTo);
            txtBody.Text = HttpUtility.UrlDecode(linkBody);
        }
    }


    /// <summary>
    /// Returns all parameters of the selected item as name – value collection.
    /// </summary>
    public override Hashtable GetItemProperties()
    {
        Hashtable retval = new Hashtable();

        retval[DialogParameters.EMAIL_LINKTEXT] = txtLinkText.Text.Trim().Replace("%", "%25");
        retval[DialogParameters.EMAIL_SUBJECT] = HttpUtility.UrlPathEncode(txtSubject.Text.Trim().Replace("%", "%25"));
        retval[DialogParameters.EMAIL_TO] = HttpUtility.UrlPathEncode(txtTo.Text.Trim().Replace("%", "%25"));
        retval[DialogParameters.EMAIL_CC] = HttpUtility.UrlPathEncode(txtCc.Text.Trim().Replace("%", "%25"));
        retval[DialogParameters.EMAIL_BCC] = HttpUtility.UrlPathEncode(txtBcc.Text.Trim().Replace("%", "%25"));
        retval[DialogParameters.EMAIL_BODY] = HttpUtility.UrlPathEncode(txtBody.Text.Trim().Replace("%", "%25").Replace("?", "%3F"));

        retval[DialogParameters.EDITOR_CLIENTID] = QueryHelper.GetString(DialogParameters.EDITOR_CLIENTID, "").Replace("%", "%25");
        return retval;
    }


    /// <summary>
    /// Validates From, Cc and Bcc e-mails.
    /// </summary>
    public override bool Validate()
    {
        string errorMessage = "";

        if (string.IsNullOrEmpty(txtTo.Text))
        {
            errorMessage += "<br/>" + GetString("general.requireemail");
        }
        else if (!ValidationHelper.AreEmails(txtTo.Text.Trim()))
        {
            errorMessage += "<br/>" + GetString("dialogs.email.invalidto");
        }
        if ((txtCc.Text.Trim() != "") && !ValidationHelper.AreEmails(txtCc.Text.Trim()))
        {
            errorMessage += "<br/>" + GetString("dialogs.email.invalidcc");
        }
        if ((txtBcc.Text.Trim() != "") && !ValidationHelper.AreEmails(txtBcc.Text.Trim()))
        {
            errorMessage += "<br/>" + GetString("dialogs.email.invalidbcc");
        }
        errorMessage = errorMessage.Trim();

        if (errorMessage != "")
        {
            lblError.Text = errorMessage;
            lblError.Visible = true;
            plnEmailUpdate.Update();
            return false;
        }

        return true;
    }


    /// <summary>
    /// Clears the properties form.
    /// </summary>
    public override void ClearProperties(bool hideProperties)
    {
        txtLinkText.Text = "";
        txtSubject.Text = "";
        txtTo.Text = "";
        txtCc.Text = "";
        txtBcc.Text = "";
        txtBody.Text = "";
    }

    #endregion
}
