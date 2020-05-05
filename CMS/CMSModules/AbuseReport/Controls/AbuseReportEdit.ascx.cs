using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AbuseReport_Controls_AbuseReportEdit : CMSAdminControl
{
    #region "Variables"

    private string mConfirmationText = string.Empty;
    private string mReportTitle = string.Empty;
    private string mReportObjectType = string.Empty;
    private string mReportURL = string.Empty;
    private LocalizedButton mReportButton;

    #endregion


    #region "Properties"

    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets and sets Confirmation text.
    /// </summary>
    public string ConfirmationText
    {
        get
        {
            if (string.IsNullOrEmpty(mConfirmationText))
            {
                return "abuse.saved";
            }
            else
            {
                return mConfirmationText;
            }
        }
        set
        {
            mConfirmationText = value;
        }
    }


    /// <summary>
    /// Gets or sets Report title.
    /// </summary>
    public string ReportTitle
    {
        get
        {
            return mReportTitle;
        }
        set
        {
            mReportTitle = value;
        }
    }


    /// <summary>
    /// Gets or sets Report Object ID.
    /// </summary>
    public int ReportObjectID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the URL of the abuse to be reported.
    /// </summary>
    public string ReportURL
    {
        get
        {
            if (string.IsNullOrEmpty(mReportURL))
            {
                mReportURL = URLHelper.GetAbsoluteUrl(RequestContext.CurrentURL);
            }
            return mReportURL;
        }
        set
        {
            mReportURL = value;
        }
    }


    /// <summary>
    /// Gets or sets Report Object type.
    /// </summary>
    public string ReportObjectType
    {
        get
        {
            return mReportObjectType;
        }
        set
        {
            mReportObjectType = value;
        }
    }


    /// <summary>
    /// Returns textbox control.
    /// </summary>
    public CMSTextArea TextField
    {
        get
        {
            return txtText;
        }
    }


    /// <summary>
    /// Returns report button control.
    /// </summary>
    public LocalizedButton ReportButton
    {
        get
        {
            return mReportButton ?? (mReportButton = btnReport);
        }
        set
        {
            mReportButton = value;
        }
    }


    /// <summary>
    /// Indicates if buttons should be displayed.
    /// </summary>
    public bool DisplayButtons
    {
        get
        {
            return plcButtons.Visible;
        }
        set
        {
            plcButtons.Visible = value;
        }
    }


    /// <summary>
    /// Returns panel control.
    /// </summary>
    public Panel BodyPanel
    {
        get
        {
            return pnlBody;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        rfvText.ErrorMessage = GetString("abuse.textreqired");
        rfvText.ValidationGroup = "Abuse" + ClientID;
        ReportButton.ValidationGroup = "Abuse" + ClientID;

        // WAI validation
        lblText.CssClass = "sr-only";

        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Resets all properties.
    /// </summary>
    public void Reload()
    {
        txtText.Visible = true;
        ReportButton.Visible = true;
        txtText.Text = String.Empty;
    }

    #endregion


    #region "Button handling"

    /// <summary>
    /// Report abuse event handler.
    /// </summary>
    protected void btnReport_Click(object sender, EventArgs e)
    {
        PerformAction();
    }


    /// <summary>
    /// Performs reporting of abuse.
    /// </summary>
    public void PerformAction()
    {
        // Check banned ip
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
            return;
        }

        string report = txtText.Text;

        // Check that text area is not empty or too long
        report = report.Trim();
        report = TextHelper.LimitLength(report, txtText.MaxLength);

        if (report.Length > 0)
        {
            // Create new AbuseReport
            AbuseReportInfo abuseReport = new AbuseReportInfo();
            if (ReportTitle != "")
            {
                // Set AbuseReport properties
                // Decode first, from forums it can be encoded
                ReportTitle = Server.HtmlDecode(ReportTitle);
                // Remove BBCode tags
                ReportTitle = DiscussionMacroResolver.RemoveTags(ReportTitle);
                abuseReport.ReportTitle = TextHelper.LimitLength(ReportTitle, 100);
                abuseReport.ReportURL = URLHelper.GetAbsoluteUrl(ReportURL);
                abuseReport.ReportCulture = LocalizationContext.PreferredCultureCode;
                if (ReportObjectID > 0)
                {
                    abuseReport.ReportObjectID = ReportObjectID;
                }

                if (ReportObjectType != "")
                {
                    abuseReport.ReportObjectType = ReportObjectType;
                }

                abuseReport.ReportComment = report;

                if (MembershipContext.AuthenticatedUser.UserID > 0)
                {
                    abuseReport.ReportUserID = MembershipContext.AuthenticatedUser.UserID;
                }

                abuseReport.ReportWhen = DateTime.Now;
                abuseReport.ReportStatus = AbuseReportStatusEnum.New;
                abuseReport.ReportSiteID = SiteContext.CurrentSite.SiteID;

                // Save AbuseReport
                AbuseReportInfoProvider.SetAbuseReportInfo(abuseReport);

                ShowConfirmation(GetString(ConfirmationText), true);
                txtText.Visible = false;
                ReportButton.Visible = false;
            }
            else
            {
                ShowError(GetString("abuse.errors.reporttitle"));
            }
        }
        else
        {
            ShowError(GetString("abuse.errors.reportcomment"));
        }

        // Additional form modification
        ReportButton.Visible = false;
    }

    #endregion
}