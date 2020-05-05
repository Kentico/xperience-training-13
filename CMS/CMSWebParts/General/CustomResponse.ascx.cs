using System;
using System.Web;
using System.Text;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_General_CustomResponse : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the response content disposition
    /// </summary>
    public string ContentDisposition
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ContentDisposition"), String.Empty);
        }
        set
        {
            this.SetValue("ContentDisposition", value);
        }
    }


    /// <summary>
    /// Gets or sets the response content type
    /// </summary>
    public string ContentType
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ContentType"), "text/plain");
        }
        set
        {
            this.SetValue("ContentType", value);
        }
    }


    /// <summary>
    /// Gets or sets the response encoding
    /// </summary>
    public string Encoding
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Encoding"), "utf-8");
        }
        set
        {
            this.SetValue("Encoding", value);
        }
    }


    /// <summary>
    /// Gets or sets the response  status code
    /// </summary>
    public string StatusCode
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("StatusCode"), "200");
        }
        set
        {
            this.SetValue("StatusCode", value);
        }
    }


    /// <summary>
    /// Gets or sets the response content
    /// </summary>
    public string Content
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Content"), String.Empty);
        }
        set
        {
            this.SetValue("Content", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Switch by view mode
            switch (PortalContext.ViewMode)
            {

                case ViewModeEnum.LiveSite:
                case ViewModeEnum.Preview:

                    // Keep current response
                    HttpResponse response = HttpContext.Current.Response;
                    // Clear response
                    response.Clear();

                    // Content type
                    response.ContentType = ContentType;
                    // Encoding
                    SetEncoding(response);
                    // Status code
                    SetStatus(response);
                    // Content disposition
                    if (!String.IsNullOrEmpty(ContentDisposition))
                    {
                        response.AddHeader("Content-Disposition", ContentDisposition);
                    }

                    // Write to the output
                    response.Write(Content);
                    // Close response
                    RequestHelper.EndResponse();
                    break;

                default:
                    lblInfo.Visible = true;
                    break;
            }
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Sets response encoding 
    /// </summary>
    /// <param name="response">Current response</param>
    private void SetEncoding(HttpResponse response)
    {
        // Use default UTF-8 encoding
        Encoding enc = System.Text.Encoding.UTF8;
        // Try set custom encoding
        try
        {
            enc = System.Text.Encoding.GetEncoding(Encoding);
            response.ContentEncoding = enc;
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("CustomResponse", "SetEncoding", ex);
        }
    }


    /// <summary>
    /// Sets the response status code
    /// </summary>
    /// <param name="response">Current response</param>
    private void SetStatus(HttpResponse response)
    {
        // Set 200 - OK status by default
        response.StatusCode = 200;
        string status = StatusCode;

        if (!String.IsNullOrEmpty(status))
        {
            // Try split by dot
            string[] statusArr = status.Split('.');
            // Set status code
            response.StatusCode = ValidationHelper.GetInteger(statusArr[0], 200);
            // Set sub status code if is defined
            if (statusArr.Length > 1)
            {
                response.SubStatusCode = ValidationHelper.GetInteger(statusArr[1], 0);
            }
        }
    }

    #endregion
}
