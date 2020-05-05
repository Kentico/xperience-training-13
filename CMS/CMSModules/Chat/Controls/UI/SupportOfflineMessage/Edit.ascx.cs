using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_SupportOfflineMessage_Edit : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Message text.
    /// </summary>
    public string MessageText
    {
        get
        {
            return txtMessage.Text;
        }
        set
        {
            txtMessage.Text = value;
        }
    }


    /// <summary>
    /// Sender's email.
    /// </summary>
    public string Sender
    {
        get
        {
            return txtEmail.Text.Trim();
        }
        set
        {
            txtEmail.Text = value;
        }
    }


    /// <summary>
    /// Email subject.
    /// </summary>
    public string Subject
    {
        get
        {
            return txtSubject.Text;
        }
        set
        {
            txtSubject.Text = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        rfvSubject.ErrorMessage = rfvMessage.ErrorMessage = ResHelper.GetString("general.requiresvalue");
        lblEmailError.Visible = false;
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Validate this form and return if validation was successful.
    /// </summary>
    public bool Validate()
    {
        bool emailIsValid = ValidationHelper.IsEmail(Sender, checkLength: true);
        if (!emailIsValid)
        {
            lblEmailError.Visible = true;
        }
        else
        {
            lblEmailError.Visible = false;
        }
        rfvSubject.Validate();
        rfvMessage.Validate();
        return (rfvSubject.IsValid && rfvMessage.IsValid && emailIsValid);
    }

    #endregion
}