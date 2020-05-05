using System;

using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_FormControls_ShippingServiceSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            drpService.Enabled = value;
        }
    }


    /// <summary>
    /// Gets carrier id from edited form.
    /// </summary>
    private int CarrierID
    {
        get
        {
            if ((Form != null) && !String.IsNullOrEmpty(CarrierIDColumnName))
            {
                var carrierSelector = Form.FieldControls[CarrierIDColumnName];
                if (carrierSelector != null)
                {
                    return ValidationHelper.GetInteger(carrierSelector.Value, 0);
                }
            }

            return 0;
        }
    }


    /// <summary>
    /// Gets a column name for carrier ID.
    /// </summary>
    private string CarrierIDColumnName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CarrierIDColumnName"), String.Empty);
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpService.SelectedValue;
        }
        set
        {
            if (value != null)
            {
                drpService.SelectedValue = value.ToString();
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (drpService.Items.Count == 0)
        {
            InitServices();
        }
    }


    /// <summary>
    /// Loads services to service selector.
    /// </summary>
    private void InitServices()
    {
        drpService.Items.Clear();

        if (CarrierID > 0)
        {
            var carrierProvider = CarrierInfoProvider.GetCarrierProvider(CarrierID);
            if (carrierProvider != null)
            {
                drpService.DataSource = carrierProvider.GetServices();
                drpService.DataBind();
            }
        }
    }

    #endregion
}