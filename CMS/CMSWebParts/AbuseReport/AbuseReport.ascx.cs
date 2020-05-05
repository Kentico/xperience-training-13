using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_AbuseReport_AbuseReport : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets text which confirms that report was correctly saved.
    /// </summary>
    public string ConfirmationText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ConfirmationText"), editReport.ConfirmationText);
        }
        set
        {
            SetValue("ConfirmationText", value);
            editReport.ConfirmationText = value;
        }
    }


    /// <summary>
    /// Gets or sets Report title.
    /// </summary>
    public string ReportTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReportTitle"), editReport.ReportTitle);
        }
        set
        {
            SetValue("ReportTitle", value);
            editReport.ReportTitle = value;
        }
    }


    /// <summary>
    /// Gets or sets Report Object ID.
    /// </summary>
    public int ReportObjectID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ReportObjectID"), editReport.ReportObjectID);
        }
        set
        {
            SetValue("ReportObjectID", value);
            editReport.ReportObjectID = value;
        }
    }


    /// <summary>
    /// Gets or sets Report Object type.
    /// </summary>
    public string ReportObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ReportObjectType"), editReport.ReportObjectType);
        }
        set
        {
            SetValue("ReportObjectType", value);
            editReport.ReportObjectType = value;
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
            editReport.StopProcessing = value;
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
    /// Reload date override.
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
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            editReport.ConfirmationText = ConfirmationText;
            editReport.ReportTitle = ReportTitle;
            editReport.ReportObjectID = ReportObjectID;
            editReport.ReportObjectType = ReportObjectType;
        }
    }
}