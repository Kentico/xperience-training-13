using System;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ImportExport_Controls_NewSiteFinish : CMSUserControl
{
    private string mDomain = "";


    /// <summary>
    /// Site domain.
    /// </summary>
    public string Domain
    {
        get
        {
            return mDomain;
        }
        set
        {
            mDomain = value;
        }
    }


    /// <summary>
    /// Indicates if the imported site is running.
    /// </summary>
    public bool SiteIsRunning
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["SiteIsRunning"], true);
        }
        set
        {
            ViewState["SiteIsRunning"] = value;
        }
    }


    // Label
    public PlaceHolder Placeholder
    {
        get
        {
            return plcFinish;
        }
        set
        {
            plcFinish = value;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblLogonDetails.Text = GetString("NewSite_Finish.LogonDetails");

        if (SiteIsRunning)
        {
            lnkWebSite.Text = GetString("NewSite_Finish.EditYourWebSite");
            lnkWebSite.Target = "_blank";
            lnkWebSite.NavigateUrl = RequestContext.CurrentScheme + "://" + Domain + ResolveUrl("~/Admin/cmsadministration.aspx");
        }
        else
        {
            lblSiteStatus.Text = GetString("NewSite_Finish.SiteStatus");
        }

        lnkWebSite.Visible = SiteIsRunning;
        lblSiteStatus.Visible = !SiteIsRunning;
    }
}