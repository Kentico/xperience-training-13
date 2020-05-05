using System;
using System.Data;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_WebServices_GridForRESTService : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the URL for querying REST Service.
    /// </summary>
    public string RESTServiceQueryURL
    {
        get
        {
            return UrlResolver.ResolveUrl(ValidationHelper.GetString(GetValue("RESTServiceQueryURL"), ""));
        }
        set
        {
            SetValue("RESTServiceQueryURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UserName"), "");
        }
        set
        {
            SetValue("UserName", value);
        }
    }


    /// <summary>
    /// Gets or sets the user password.
    /// </summary>
    public string Password
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Password"), "");
        }
        set
        {
            SetValue("Password", value);
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
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            basicDataGrid.Visible = false;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reload control's data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        if (!string.IsNullOrEmpty(RESTServiceQueryURL))
        {
            try
            {
                HttpWebRequest request = CreateWebRequest();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // If everything went ok, parse the xml recieved to dataset and bind it to the grid
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    DataSet ds = new DataSet();
                    ds.ReadXml(response.GetResponseStream());

                    basicDataGrid.DataSource = ds;
                    basicDataGrid.DataBind();
                    basicDataGrid.ItemDataBound += new DataGridItemEventHandler(basicDataGrid_ItemDataBound);
                }
            }
            catch (Exception ex)
            {
                // Handle the error
                Service.Resolve<IEventLogService>().LogException("GridForRESTService", "GETDATA", ex);

                lblError.Text = ResHelper.GetStringFormat("RESTService.RequestFailed", ex.Message);
                lblError.Visible = true;
            }
        }
    }

    
    protected void basicDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
    {
        // Encode content of the cells
        foreach (TableCell item in e.Item.Cells)
        {
            item.Text = HTMLHelper.HTMLEncode(item.Text);
        }
    }


    /// <summary>
    /// Creates the WebRequest for querying the REST service.
    /// </summary>
    private HttpWebRequest CreateWebRequest()
    {
        string url = RESTServiceQueryURL;
        if (url.StartsWithCSafe("/"))
        {
            url = URLHelper.GetAbsoluteUrl(url);
        }
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        request.Method = "GET";
        request.ContentLength = 0;
        request.ContentType = "text/xml";

        // Set credentials for basic authentication
        if (!string.IsNullOrEmpty(UserName))
        {
            // Set the authorization header for basic authentication
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(UserName + ":" + Password));
        }

        return request;
    }
}