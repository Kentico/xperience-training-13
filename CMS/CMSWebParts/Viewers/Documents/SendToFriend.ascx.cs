using System;
using System.Text;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MacroEngine;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;


public partial class CMSWebParts_Viewers_Documents_SendToFriend : CMSAbstractWebPart
{
    #region "Variables"

    private bool mSendEmail = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the text which will be displayed as header of webpart.
    /// </summary>
    public string HeaderText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderText"), "");
        }
        set
        {
            SetValue("HeaderText", value);
            lblHeader.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets email address from which will be email send.
    /// </summary>
    public string EmailFrom
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailFrom"), "");
        }
        set
        {
            SetValue("EmailFrom", value);
        }
    }


    /// <summary>
    /// Gets or sets an email subject.
    /// </summary>
    public string EmailSubject
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailSubject"), "");
        }
        set
        {
            SetValue("EmailSubject", value);
        }
    }


    /// <summary>
    /// Gets or sets email template text.
    /// </summary>
    public string EmailTemplate
    {
        get
        {
            return ValidationHelper.GetString(GetValue("EmailTemplate"), "");
        }
        set
        {
            SetValue("EmailTemplate", value);
        }
    }


    /// <summary>
    /// Hide form after email sent.
    /// </summary>
    public bool HideAfterEmailSent
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideAfterEmailSent"), false);
        }
        set
        {
            SetValue("HideAfterEmailSent", value);
        }
    }

    #endregion


    #region "Document properties"

    /// <summary>
    /// Check permissions.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), repItems.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            repItems.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Classnames"), repItems.ClassNames), repItems.ClassNames);
        }
        set
        {
            SetValue("ClassNames", value);
            repItems.ClassNames = value;
        }
    }


    /// <summary>
    /// Combine with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CombineWithDefaultCulture"), repItems.CombineWithDefaultCulture);
        }
        set
        {
            SetValue("CombineWithDefaultCulture", value);
            repItems.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Filter out duplicate documents.
    /// </summary>
    public bool FilterOutDuplicates
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FilterOutDuplicates"), repItems.FilterOutDuplicates);
        }
        set
        {
            SetValue("FilterOutDuplicates", value);
            repItems.FilterOutDuplicates = value;
        }
    }


    /// <summary>
    /// Culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), repItems.CultureCode), repItems.CultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
            repItems.CultureCode = value;
        }
    }


    /// <summary>
    /// Maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxRelativeLevel"), repItems.MaxRelativeLevel);
        }
        set
        {
            SetValue("MaxRelativeLevel", value);
            repItems.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("OrderBy"), repItems.OrderBy), repItems.OrderBy);
        }
        set
        {
            SetValue("OrderBy", value);
            repItems.OrderBy = value;
        }
    }


    /// <summary>
    /// Nodes path.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Path"), null);
        }
        set
        {
            SetValue("Path", value);
            repItems.Path = value;
        }
    }


    /// <summary>
    /// Select only published nodes.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SelectOnlyPublished"), repItems.SelectOnlyPublished);
        }
        set
        {
            SetValue("SelectOnlyPublished", value);
            repItems.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), repItems.SiteName), repItems.SiteName);
        }
        set
        {
            SetValue("SiteName", value);
            repItems.SiteName = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WhereCondition"), repItems.WhereCondition);
        }
        set
        {
            SetValue("WhereCondition", value);
            repItems.WhereCondition = value;
        }
    }


    /// <summary>
    /// Select top N items.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectTopN"), repItems.SelectTopN);
        }
        set
        {
            SetValue("SelectTopN", value);
            repItems.SelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to get.
    /// </summary>
    public string Columns
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Columns"), repItems.Columns);
        }
        set
        {
            SetValue("Columns", value);
            repItems.Columns = value;
        }
    }

    #endregion


    #region "Transformation properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("TransformationName"), repItems.TransformationName), repItems.TransformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            repItems.TransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results of the selected item.
    /// </summary>
    public string SelectedItemTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SelectedItemTransformationName"), repItems.SelectedItemTransformationName), repItems.SelectedItemTransformationName);
        }
        set
        {
            SetValue("SelectedItemTransformationName", value);
            repItems.SelectedItemTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the alternate results.
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("AlternatingTransformationName"), repItems.AlternatingTransformationName), repItems.AlternatingTransformationName);
        }
        set
        {
            SetValue("AlternatingTransformationName", value);
            repItems.AlternatingTransformationName = value;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            repItems.StopProcessing = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        // Disable resolving on email properties
        NotResolveProperties += "emailtemplate;emailsubject";


        if (StopProcessing)
        {
            repItems.StopProcessing = true;
        }
        else
        {
            // Sets header label or hides it if property is not set
            if (HeaderText != String.Empty)
            {
                lblHeader.Text = HeaderText;
            }
            else
            {
                lblHeader.Visible = false;
            }

            btnSend.Text = GetString("SendToFriend.SendButtonLabel");
            // hide message panel at first load
            pnlMessageText.Attributes.Add("style", "display:none");
            lblYourMessage.Text = GetString("SendToFriend.YourMessageLabel");

            // Label for 508 validation
            lblEmailTo.Text = GetString("SendToFriend.EnterEmail");
            lblEmailTo.AssociatedControlID = "txtEmailTo";
            lblEmailTo.Attributes.Add("style", "display:none;");
            lblMessageText.Text = GetString("SendToFriend.EnterMessage");
            lblMessageText.Attributes.Add("style", "display:none;");
            lblMessageText.AssociatedControlID = "txtMessageText";

            // add action for showing/hiding message panel
            lblYourMessage.Attributes.Add("onclick", "STF_ShowHideElem('" + pnlMessageText.ClientID + "');");
            rfvEmailTo.ErrorMessage = GetString("SendToFriend.rfvEmailTo");

            // register JavaScript function for showing/hiding message panel
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "STF_ShowHideElem", ScriptHelper.GetScript(
                @"function STF_ShowHideElem(elemid) 
                {
                    var elem = document.getElementById(elemid);
                    if (elem != null) 
                    {
                        if (elem.style.display == '') 
                        {
                            elem.style.display = 'none';
                        } else 
                        {
                            elem.style.display = '';
                        }
                    }
                }"
                                                                                                 ));


            // SET UP REPEATER

            repItems.ControlContext = ControlContext;

            // Document properties
            repItems.CheckPermissions = CheckPermissions;
            repItems.ClassNames = ClassNames;
            repItems.CombineWithDefaultCulture = CombineWithDefaultCulture;
            repItems.CultureCode = CultureCode;
            repItems.MaxRelativeLevel = MaxRelativeLevel;
            repItems.OrderBy = OrderBy;
            repItems.SelectTopN = SelectTopN;
            repItems.Columns = Columns;
            repItems.SelectOnlyPublished = SelectOnlyPublished;
            repItems.FilterOutDuplicates = FilterOutDuplicates;

            repItems.Path = Path;

            repItems.SiteName = SiteName;
            repItems.WhereCondition = WhereCondition;

            // Transformation properties
            repItems.TransformationName = TransformationName;
            repItems.StopProcessing = true;
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = repItems.Visible && !StopProcessing;

        // Sets visibility of control based on repeater datasource
        if (DataHelper.DataSourceIsEmpty(repItems.DataSource) && repItems.HideControlForZeroRows)
        {
            Visible = false;
        }
    }


    /// <summary>
    /// Event after send button is clicked.
    /// </summary>
    protected void btnSend_Click(object sender, EventArgs e)
    {
        //check if the email is valid
        if (!ValidationHelper.IsEmail(txtEmailTo.Text))
        {
            lblError.Text = GetString("SendToFriend.InvalidEmail");
        }
        else
        {
            // indicates that email should be sent
            mSendEmail = true;

            repItems.StopProcessing = false;
            repItems.ReloadData(true);
        }
    }


    /// <summary>
    /// Render override.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        // if everything ok, email should be send
        if (mSendEmail)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            Html32TextWriter mwriter = new Html32TextWriter(sw);
            repItems.RenderControl(mwriter);

            repItems.Visible = false;

            // Prepare the macro resolver
            MacroResolver resolver = ContextResolver.CreateChild();

            resolver.SetNamedSourceData("message", txtMessageText.Text, false);
            resolver.SetNamedSourceData("document", URLHelper.MakeLinksAbsolute(sb.ToString()), false);

            if (EmailTemplate != null)
            {
                EmailMessage message = new EmailMessage();
                message.EmailFormat = EmailFormatEnum.Html;

                message.From = EmailFrom;
                message.Recipients = txtEmailTo.Text;

                // resolve EmailSubject here
                message.Subject = resolver.ResolveMacros(EmailSubject);
                // resolve EmailTemplate, wrap to HTML code and add the CSS files
                message.Body = "<html><head></head><body class=\"EmailBody\">" + resolver.ResolveMacros(EmailTemplate) + "</body></html>";

                // check recipients
                if ((message.Recipients != null) && (message.Recipients.Trim() != ""))
                {
                    try
                    {
                        EmailSender.SendEmail(SiteContext.CurrentSiteName, message);

                        // Display message, email was sent successfully
                        lblInfo.Text = GetString("sendtofriend.emailsent");
                        txtEmailTo.Text = "";
                        txtMessageText.Text = "";

                        if (HideAfterEmailSent)
                        {
                            plcForm.Visible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = GetString("SendToFriend.SendEmailError");
                        try
                        {
                            Service.Resolve<IEventLogService>().LogException("Send email", "SendToFriend", ex);
                        }
                        catch
                        {
                            // Unable to log the event
                        }
                    }
                }
            }
        }

        base.Render(writer);
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        repItems.ClearCache();
    }
}
