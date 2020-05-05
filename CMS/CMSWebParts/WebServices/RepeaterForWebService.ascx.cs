using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_WebServices_RepeaterForWebService : CMSAbstractWebPart
{
    #region "Variables"

    // Transformation name
    protected string transformationName = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to hide control when no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), basicRepeater.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            basicRepeater.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed when no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), basicRepeater.ZeroRowsText), basicRepeater.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            basicRepeater.ZeroRowsText = value;
        }
    }

    #endregion


    #region "Data binding properties"

    /// <summary>
    /// Gets or sets the value that indicates whether to databind by default.
    /// </summary>
    public bool DataBindByDefault
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), basicRepeater.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            basicRepeater.DataBindByDefault = value;
        }
    }

    #endregion


    #region "Webservices properties"

    /// <summary>
    /// Gets or sets the parameters of the web service.
    /// </summary>
    public string WebServiceParameters
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WebServiceParameters"), wsDataSource.WebServiceParameters);
        }
        set
        {
            SetValue("WebServiceParameters", value);
            wsDataSource.WebServiceParameters = value;
        }
    }


    /// <summary>
    /// Gets or sets the url of the web service.
    /// </summary>
    public string WebServiceURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WebServiceURL"), wsDataSource.WebServiceUrl);
        }
        set
        {
            SetValue("WebServiceURL", value);
            wsDataSource.WebServiceUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TransformationName"), transformationName);
        }
        set
        {
            SetValue("TransformationName", value);
            transformationName = value;
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
            wsDataSource.StopProcessing = value;
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
            basicRepeater.Visible = false;
            wsDataSource.StopProcessing = true;
        }
        else
        {
            // Setup the control
            // Public
            basicRepeater.HideControlForZeroRows = HideControlForZeroRows;
            basicRepeater.ZeroRowsText = ZeroRowsText;
            basicRepeater.DataBindByDefault = DataBindByDefault;

            // Get web service URL, parameters and transformation name
            wsDataSource.WebServiceUrl = WebServiceURL;
            wsDataSource.WebServiceParameters = WebServiceParameters;
            transformationName = TransformationName;

            // Connect with data source
            basicRepeater.DataSource = wsDataSource.DataSource;

            if (!DataHelper.DataSourceIsEmpty(basicRepeater.DataSource))
            {
                if ((transformationName != null) && (transformationName.Trim() != ""))
                {
                    // Get url to transformation and load it
                    basicRepeater.ItemTemplate = TransformationHelper.LoadTransformation(basicRepeater, transformationName);
                }
            }
            else
            {
                if (HideControlForZeroRows)
                {
                    Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Reload control's data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
        basicRepeater.ReloadData(true);
    }
}