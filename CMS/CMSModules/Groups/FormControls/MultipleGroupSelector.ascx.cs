using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_FormControls_MultipleGroupSelector : FormEngineUserControl
{
    #region "Private variables"

    private int mSiteId = 0;
    private bool reloadData = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Helper parameter - site ID.
    /// </summary>
    public override object FormControlParameter
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets or sets site ID.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


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
            EnsureChildControls();
            base.Enabled = value;
            usGroups.Enabled = value;
        }
    }


    /// <summary>
    /// Gets the currene UniSelector instance.
    /// </summary>
    public UniSelector CurrentSelector
    {
        get
        {
            EnsureChildControls();
            return usGroups;
        }
    }


    /// <summary>
    /// Group code name.
    /// </summary>
    public string GroupCodeName
    {
        get
        {
            EnsureChildControls();
            return ValidationHelper.GetString(usGroups.Value, "");
        }
        set
        {
            EnsureChildControls();
            usGroups.Value = value;
        }
    }


    /// <summary>
    /// Gets the current drop down control.
    /// </summary>
    public CMSDropDownList CurrentDropDown
    {
        get
        {
            EnsureChildControls();
            return usGroups.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets or sets group name.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return usGroups.Value;
        }
        set
        {
            EnsureChildControls();
            usGroups.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the dropdownlist with groups.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            EnsureChildControls();
            return usGroups.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets if live iste property.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            EnsureChildControls();
            return base.IsLiveSite;
        }
        set
        {
            EnsureChildControls();
            base.IsLiveSite = value;
            usGroups.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page_load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            usGroups.StopProcessing = true;
        }
        else
        {
            // Build WHERE condition
            usGroups.WhereCondition = "GroupSiteID =" + SiteID;

            // Set ReturnColumnName
            string returnColumnName = ValidationHelper.GetString(GetValue("ReturnColumnName"), null);
            if (!String.IsNullOrEmpty(returnColumnName))
            {
                usGroups.ReturnColumnName = returnColumnName;
            }
        }
    }


    /// <summary>
    /// Reloads the selector's data.
    /// </summary>
    /// <param name="forceReload">Indicates whether data should be forcibly reloaded</param>
    public void Reload(bool forceReload)
    {
        usGroups.Reload(forceReload);
    }


    /// <summary>
    /// On PreRender.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        if (reloadData)
        {
            Reload(true);
        }
    }


    public override bool SetValue(string propertyName, object value)
    {
        switch (propertyName.ToLowerCSafe())
        {
            case "reloaddata":
                reloadData = ValidationHelper.GetBoolean(value, false);
                return true;
        }

        return base.SetValue(propertyName, value);
    }

    #endregion
}