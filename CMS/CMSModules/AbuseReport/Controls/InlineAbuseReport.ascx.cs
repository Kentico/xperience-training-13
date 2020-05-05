using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_AbuseReport_Controls_InlineAbuseReport : CMSUserControl
{
    #region "Variables"

    private string mReportDialogTitle;
    private bool mDisplayButtons = true;
    private readonly Hashtable parameters = new Hashtable();
    protected ViewModeEnum mViewMode = ViewModeEnum.LiveSite;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets and sets Confirmation text.
    /// </summary>
    public string ConfirmationText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Report title.
    /// </summary>
    public string ReportTitle
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ReportTitle"], null);
        }
        set
        {
            ViewState["ReportTitle"] = value;
        }
    }


    /// <summary>
    /// Gets or sets CSS class.
    /// </summary>
    public string CssClass
    {
        get
        {
            return lnkText.CssClass;
        }
        set
        {
            lnkText.CssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets Report dialog title.
    /// </summary>
    public string ReportDialogTitle
    {
        get
        {
            if (string.IsNullOrEmpty(mReportDialogTitle))
            {
                return GetString("abuse.title");
            }
            return mReportDialogTitle;
        }
        set
        {
            mReportDialogTitle = value;
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
    /// Gets or sets Report Object type.
    /// </summary>
    public string ReportObjectType
    {
        get;
        set;
    }


    /// <summary>
    /// Gets wrapping panel.
    /// </summary>
    public SecurityPanel CMSPanel
    {
        get
        {
            return ucWrapPanel;
        }
    }


    /// <summary>
    /// Indicates if buttons should be displayed.
    /// </summary>
    public bool DisplayButtons
    {
        get
        {
            return mDisplayButtons;
        }
        set
        {
            mDisplayButtons = value;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    protected string Identifier
    {
        get
        {
            string identifier = ValidationHelper.GetString(ViewState["Identifier"], null);
            if (identifier == null)
            {
                identifier = Guid.NewGuid().ToString();
                ViewState["Identifier"] = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// View mode of the current control. Necessary for desicion
    /// which dialog to open.
    /// By default the control assumes its on LiveSite.
    /// </summary>
    public virtual ViewModeEnum ViewMode
    {
        get
        {
            return mViewMode;
        }
        set
        {
            mViewMode = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_PreRender(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        string url = (ViewMode.IsLiveSite())
                         ? ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/AbuseReport/CMSPages/ReportAbuse.aspx")
                         : ApplicationUrlHelper.ResolveDialogUrl("~/CMSModules/AbuseReport/Dialogs/ReportAbuse.aspx");
        url = URLHelper.AddParameterToUrl(url, "params", Identifier);

        parameters.Add("confirmationtext", ConfirmationText);
        parameters.Add("reporttitle", ReportTitle);
        parameters.Add("reportdialogtitle", ReportDialogTitle);
        parameters.Add("reportobjectid", ReportObjectID);
        parameters.Add("reportobjecttype", ReportObjectType);
        parameters.Add("reporturl", RequestContext.CurrentURL);
        WindowHelper.Add(Identifier, parameters);

        lnkText.NavigateUrl = "javascript:modalDialog('" + url + "', 'reportDialog', 500, 440);";
    }

    #endregion
}
