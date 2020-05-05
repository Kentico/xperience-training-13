using System;
using System.Web.UI.WebControls;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_General_CssStyleSelector : CMSAbstractWebPart
{
    #region "Private variables"

    protected LinkButton lnkFirstLink = null;
    protected LinkButton lnkSecondLink = null;
    protected LinkButton lnkThirdLink = null;
    protected LinkButton lnkFourthLink = null;
    protected LinkButton lnkFifthLink = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Name of the cookie for class information.
    /// </summary>
    public string CookieName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CookieName"), ""), CMS.Helpers.CookieName.BodyClass);
        }
        set
        {
            SetValue("CookieName", value);
        }
    }


    /// <summary>
    /// Text of the first link.
    /// </summary>
    public string FirstLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstLinkText"), "");
        }
        set
        {
            SetValue("FirstLinkText", value);
        }
    }


    /// <summary>
    /// Text of the first selected link.
    /// </summary>
    public string FirstLinkSelectedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstLinkSelectedText"), "");
        }
        set
        {
            SetValue("FirstLinkSelectedText", value);
        }
    }


    /// <summary>
    /// Title of the first link.
    /// </summary>
    public string FirstLinkTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstLinkTitle"), "");
        }
        set
        {
            SetValue("FirstLinkTitle", value);
        }
    }


    /// <summary>
    /// Body CSS class of the first link.
    /// </summary>
    public string FirstBodyCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FirstBodyCSSClass"), "");
        }
        set
        {
            SetValue("FirstBodyCSSClass", value);
        }
    }


    /// <summary>
    /// Text of the second link.
    /// </summary>
    public string SecondLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SecondLinkText"), "");
        }
        set
        {
            SetValue("SecondLinkText", value);
        }
    }


    /// <summary>
    /// Text of the second selected link.
    /// </summary>
    public string SecondLinkSelectedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SecondLinkSelectedText"), "");
        }
        set
        {
            SetValue("SecondLinkSelectedText", value);
        }
    }


    /// <summary>
    /// Title of the second link.
    /// </summary>
    public string SecondLinkTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SecondLinkTitle"), "");
        }
        set
        {
            SetValue("SecondLinkTitle", value);
        }
    }


    /// <summary>
    /// Body CSS class of the second link.
    /// </summary>
    public string SecondBodyCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SecondBodyCSSClass"), "");
        }
        set
        {
            SetValue("SecondBodyCSSClass", value);
        }
    }


    /// <summary>
    /// Text of the third link.
    /// </summary>
    public string ThirdLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ThirdLinkText"), "");
        }
        set
        {
            SetValue("ThirdLinkText", value);
        }
    }


    /// <summary>
    /// Text of the third selected link.
    /// </summary>
    public string ThirdLinkSelectedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ThirdLinkSelectedText"), "");
        }
        set
        {
            SetValue("ThirdLinkSelectedText", value);
        }
    }


    /// <summary>
    /// Title of the third link.
    /// </summary>
    public string ThirdLinkTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ThirdLinkTitle"), "");
        }
        set
        {
            SetValue("ThirdLinkTitle", value);
        }
    }


    /// <summary>
    /// Body CSS class of the third link.
    /// </summary>
    public string ThirdBodyCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ThirdBodyCSSClass"), "");
        }
        set
        {
            SetValue("ThirdBodyCSSClass", value);
        }
    }


    /// <summary>
    /// Text of the fourth link.
    /// </summary>
    public string FourthLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FourthLinkText"), "");
        }
        set
        {
            SetValue("FourthLinkText", value);
        }
    }


    /// <summary>
    /// Text of the fourth selected link.
    /// </summary>
    public string FourthLinkSelectedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FourthLinkSelectedText"), "");
        }
        set
        {
            SetValue("FourthLinkSelectedText", value);
        }
    }


    /// <summary>
    /// Title of the fourth link.
    /// </summary>
    public string FourthLinkTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FourthLinkTitle"), "");
        }
        set
        {
            SetValue("FourthLinkTitle", value);
        }
    }


    /// <summary>
    /// Body CSS class of the fourth link.
    /// </summary>
    public string FourthBodyCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FourthBodyCSSClass"), "");
        }
        set
        {
            SetValue("FourthBodyCSSClass", value);
        }
    }


    /// <summary>
    /// Text of the fifth link.
    /// </summary>
    public string FifthLinkText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FifthLinkText"), "");
        }
        set
        {
            SetValue("FifthLinkText", value);
        }
    }


    /// <summary>
    /// Text of the fifth selected link.
    /// </summary>
    public string FifthLinkSelectedText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FifthLinkSelectedText"), "");
        }
        set
        {
            SetValue("FifthLinkSelectedText", value);
        }
    }


    /// <summary>
    /// Title of the fifth link.
    /// </summary>
    public string FifthLinkTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FifthLinkTitle"), "");
        }
        set
        {
            SetValue("FifthLinkTitle", value);
        }
    }


    /// <summary>
    /// Body CSS class of the fifth link.
    /// </summary>
    public string FifthBodyCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FifthBodyCSSClass"), "");
        }
        set
        {
            SetValue("FifthBodyCSSClass", value);
        }
    }


    /// <summary>
    /// HTML text inserted between links as separator.
    /// </summary>
    public string LinksSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LinksSeparator"), "");
        }
        set
        {
            SetValue("LinksSeparator", value);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        SetupControl();
        base.ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        base.OnPreRender(e);

        string cookieValue = ValidationHelper.GetString(CookieHelper.GetValue(CookieName), "");

        // If cookie value is set add value to the body class
        if (!string.IsNullOrEmpty(cookieValue))
        {
            DocumentContext.CurrentBodyClass += " " + cookieValue;
        }

        // Set proper link texts
        if (lnkFirstLink != null)
        {
            lnkFirstLink.Text = (cookieValue == FirstBodyCSSClass) ? FirstLinkSelectedText : FirstLinkText;
        }

        if (lnkSecondLink != null)
        {
            lnkSecondLink.Text = (cookieValue == SecondBodyCSSClass) ? SecondLinkSelectedText : SecondLinkText;
        }

        if (lnkThirdLink != null)
        {
            lnkThirdLink.Text = (cookieValue == ThirdBodyCSSClass) ? ThirdLinkSelectedText : ThirdLinkText;
        }

        if (lnkFourthLink != null)
        {
            lnkFourthLink.Text = (cookieValue == FourthBodyCSSClass) ? FourthLinkSelectedText : FourthLinkText;
        }

        if (lnkFifthLink != null)
        {
            lnkFifthLink.Text = (cookieValue == FifthBodyCSSClass) ? FifthLinkSelectedText : FifthLinkText;
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            pnlLinks.Controls.Clear();
            if (FirstLinkText != "")
            {
                lnkFirstLink = new LinkButton();
                lnkFirstLink.EnableViewState = false;
                lnkFirstLink.ID = "lnkFirstLink";
                lnkFirstLink.Command += lnkLink_Command;
                lnkFirstLink.CommandName = "linkCommand";
                lnkFirstLink.CommandArgument = FirstBodyCSSClass;
                lnkFirstLink.ToolTip = FirstLinkTitle;
                pnlLinks.Controls.Add(lnkFirstLink);
            }

            if (SecondLinkText != "")
            {
                // Add links separator
                AddSeparator("ltlFirstSeparator");

                lnkSecondLink = new LinkButton();
                lnkSecondLink.EnableViewState = false;
                lnkSecondLink.ID = "lnkSecondLink";
                lnkSecondLink.Command += lnkLink_Command;
                lnkSecondLink.CommandName = "linkCommand";
                lnkSecondLink.CommandArgument = SecondBodyCSSClass;
                lnkSecondLink.ToolTip = SecondLinkTitle;
                pnlLinks.Controls.Add(lnkSecondLink);
            }

            if (ThirdLinkText != "")
            {
                // Add links separator
                AddSeparator("ltlSecondSeparator");

                lnkThirdLink = new LinkButton();
                lnkThirdLink.EnableViewState = false;
                lnkThirdLink.ID = "lnkThirdLink";
                lnkThirdLink.Command += lnkLink_Command;
                lnkThirdLink.CommandArgument = ThirdBodyCSSClass;
                lnkThirdLink.ToolTip = ThirdLinkTitle;
                pnlLinks.Controls.Add(lnkThirdLink);
            }

            if (FourthLinkText != "")
            {
                // Add links separator
                AddSeparator("ltlThirdSeparator");

                lnkFourthLink = new LinkButton();
                lnkFourthLink.EnableViewState = false;
                lnkFourthLink.ID = "lnkFourthLink";
                lnkFourthLink.Command += lnkLink_Command;
                lnkFourthLink.CommandArgument = FourthBodyCSSClass;
                lnkFourthLink.ToolTip = FourthLinkTitle;
                pnlLinks.Controls.Add(lnkFourthLink);
            }

            if (FifthLinkText != "")
            {
                // Add links separator
                AddSeparator("ltlFourthSeparator");

                lnkFifthLink = new LinkButton();
                lnkFifthLink.EnableViewState = false;
                lnkFifthLink.ID = "lnkFifthLink";
                lnkFifthLink.Command += lnkLink_Command;
                lnkFifthLink.CommandArgument = FifthBodyCSSClass;
                lnkFifthLink.ToolTip = FifthLinkTitle;
                pnlLinks.Controls.Add(lnkFifthLink);
            }
        }
    }


    /// <summary>
    /// Link click handler.
    /// </summary>
    protected void lnkLink_Command(object sender, CommandEventArgs e)
    {
        // Set new value to the cookie
        CookieHelper.SetValue(CookieName, e.CommandArgument.ToString(), DateTime.Now.AddYears(50));
    }


    /// <summary>
    /// Adds separator to the panel.
    /// </summary>
    /// <param name="separatorID">Id of the separator</param>
    private void AddSeparator(string separatorID)
    {
        if ((LinksSeparator != "") && (pnlLinks.Controls.Count > 0))
        {
            Literal ltlSeparator = new Literal();
            ltlSeparator.ID = separatorID;
            ltlSeparator.EnableViewState = false;
            ltlSeparator.Text = LinksSeparator;
            pnlLinks.Controls.Add(ltlSeparator);
        }
    }

    #endregion
}