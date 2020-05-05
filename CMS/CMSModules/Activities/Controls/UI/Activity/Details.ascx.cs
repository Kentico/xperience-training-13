using System;
using System.Net;
using System.Web;
using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Base.Web.UI;
using CMS.UIControls;

public partial class CMSModules_Activities_Controls_UI_Activity_Details : CMSAdminListControl
{
    private const string PATH_TO_CONTROLS = "~/CMSModules/Activities/Controls/UI/ActivityDetails/{0}.ascx";

    /// <summary>
    /// Gets or sets activity ID.
    /// </summary>
    public int ActivityID
    {
        get;
        set;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData();
    }


    private void LoadData()
    {
        ActivityInfo ai = ActivityInfo.Provider.Get(ActivityID);
        if (ai == null)
        {
            return;
        }

        string pathToControl = String.Format(PATH_TO_CONTROLS, ai.ActivityType);
        ActivityDetail ucDetails = null;
        try
        {
            ucDetails = Page.LoadUserControl(pathToControl) as ActivityDetail;
        }
        catch (HttpException exception)
        {
            // Ignore not existing control.
            // File.Exists cannot be used because user control is not available on filesystem for precompiled web application.
            if (exception.GetHttpCode() != (int)HttpStatusCode.NotFound)
            {
                throw;
            }
        }

        if (ucDetails != null && ucDetails.LoadData(ai))
        {
            pnlDetails.Controls.Add(ucDetails);
            return;
        }

        // Control doesn't exist or couldn't load data. It's ok for custom activities or activities without details.
        Visible = false;
    }
}