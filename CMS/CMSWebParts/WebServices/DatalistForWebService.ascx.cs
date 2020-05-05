using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_WebServices_DatalistForWebService : CMSAbstractWebPart
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
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), basicDatalist.HideControlForZeroRows);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
            basicDatalist.HideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed when no data found.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("ZeroRowsText"), basicDatalist.ZeroRowsText), basicDatalist.ZeroRowsText);
        }
        set
        {
            SetValue("ZeroRowsText", value);
            basicDatalist.ZeroRowsText = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of columns.
    /// </summary>
    public int RepeatColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RepeatColumns"), basicDatalist.RepeatColumns);
        }
        set
        {
            SetValue("RepeatColumns", value);
            basicDatalist.RepeatColumns = value;
        }
    }


    /// <summary>
    /// Gets or sets the repeat layout.
    /// </summary>
    public RepeatLayout RepeatLayout
    {
        get
        {
            return CMSDataList.GetRepeatLayout(DataHelper.GetNotEmpty(GetValue("RepeatLayout"), basicDatalist.RepeatLayout.ToString()));
        }
        set
        {
            SetValue("RepeatLayout", value.ToString());
            basicDatalist.RepeatLayout = value;
        }
    }


    /// <summary>
    /// Gets or sets the repeat direction.
    /// </summary>
    public RepeatDirection RepeatDirection
    {
        get
        {
            return CMSDataList.GetRepeatDirection(DataHelper.GetNotEmpty(GetValue("RepeatDirection"), basicDatalist.RepeatDirection.ToString()));
        }
        set
        {
            SetValue("RepeatDirection", value.ToString());
            basicDatalist.RepeatDirection = value;
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
            return ValidationHelper.GetBoolean(GetValue("DataBindByDefault"), basicDatalist.DataBindByDefault);
        }
        set
        {
            SetValue("DataBindByDefault", value);
            basicDatalist.DataBindByDefault = value;
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
            basicDatalist.Visible = false;
            wsDataSource.StopProcessing = true;
        }
        else
        {
            // Setup the control
            // Public
            basicDatalist.RepeatColumns = RepeatColumns;
            basicDatalist.RepeatLayout = RepeatLayout;
            basicDatalist.RepeatDirection = RepeatDirection;
            basicDatalist.HideControlForZeroRows = HideControlForZeroRows;
            basicDatalist.ZeroRowsText = ZeroRowsText;
            basicDatalist.DataBindByDefault = DataBindByDefault;

            // Get web service URL, parameters and transformation name
            wsDataSource.WebServiceUrl = WebServiceURL;
            wsDataSource.WebServiceParameters = WebServiceParameters;
            transformationName = TransformationName;

            // Connect with data source
            basicDatalist.DataSource = wsDataSource.DataSource;

            if (!DataHelper.DataSourceIsEmpty(basicDatalist.DataSource))
            {
                if (transformationName != "")
                {
                    basicDatalist.ItemTemplate = TransformationHelper.LoadTransformation(basicDatalist, transformationName);
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
        basicDatalist.ReloadData(true);
    }
}