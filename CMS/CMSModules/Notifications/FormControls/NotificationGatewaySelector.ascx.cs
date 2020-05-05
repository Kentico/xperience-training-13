using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Notifications.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Notifications_FormControls_NotificationGatewaySelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseGatewayNameForSelection = true;
    private bool mAddNoneRecord;
    private bool mUseAutoPostBack;

    #endregion


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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown with gateways.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            if (mUseGatewayNameForSelection)
            {
                return GatewayName;
            }
            else
            {
                return GatewayID;
            }
        }
        set
        {
            if (mUseGatewayNameForSelection)
            {
                GatewayName = ValidationHelper.GetString(value, String.Empty);
            }
            else
            {
                GatewayID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the Gateway ID.
    /// </summary>
    public int GatewayID
    {
        get
        {
            if (mUseGatewayNameForSelection)
            {
                // Convert name to ID
                string name = ValidationHelper.GetString(uniSelector.Value, String.Empty);
                NotificationGatewayInfo ngi = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(name);
                if (ngi != null)
                {
                    return ngi.GatewayID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(uniSelector.Value, 0);
            }
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (mUseGatewayNameForSelection)
            {
                // Convert ID to name
                NotificationGatewayInfo ngi = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(value);
                if (ngi != null)
                {
                    uniSelector.Value = ngi.GatewayName;
                }
            }
            else
            {
                uniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the Gateway code name.
    /// </summary>
    public string GatewayName
    {
        get
        {
            if (mUseGatewayNameForSelection)
            {
                return ValidationHelper.GetString(uniSelector.Value, String.Empty);
            }
            else
            {
                // Convert ID to name
                int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
                NotificationGatewayInfo ngi = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(id);
                if (ngi != null)
                {
                    return ngi.GatewayName;
                }
                return String.Empty;
            }
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (mUseGatewayNameForSelection)
            {
                uniSelector.Value = value;
            }
            else
            {
                // Convert name to ID
                NotificationGatewayInfo ngi = NotificationGatewayInfoProvider.GetNotificationGatewayInfo(value);
                if (ngi != null)
                {
                    uniSelector.Value = ngi.GatewayID;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is GatewayName, if false, selected value is GatewayID.
    /// </summary>
    public bool UseGatewayNameForSelection
    {
        get
        {
            return mUseGatewayNameForSelection;
        }
        set
        {
            mUseGatewayNameForSelection = value;
            if (uniSelector != null)
            {
                uniSelector.ReturnColumnName = (value ? "GatewayName" : "GatewayID");
            }
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add none item record to the dropdownlist.
    /// </summary>
    public bool AddNoneRecord
    {
        get
        {
            return mAddNoneRecord;
        }
        set
        {
            mAddNoneRecord = value;
            if (uniSelector != null)
            {
                uniSelector.AllowEmpty = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether the dropdown should use AutoPostBack.
    /// </summary>
    public bool UseAutoPostBack
    {
        get
        {
            return mUseAutoPostBack;
        }
        set
        {
            mUseAutoPostBack = value;
            if (uniSelector != null)
            {
                uniSelector.DropDownSingleSelect.AutoPostBack = value;
            }
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
        uniSelector.ReturnColumnName = (UseGatewayNameForSelection ? "GatewayName" : "GatewayID");
        uniSelector.DropDownSingleSelect.AutoPostBack = UseAutoPostBack;
        uniSelector.AllowEmpty = AddNoneRecord;
    }
}