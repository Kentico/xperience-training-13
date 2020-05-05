using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Groups_FormControls_MembershipGroupSelector : FormEngineUserControl
{
    #region "Private variables"

    private int mSiteId = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Enables or Disables showing all groups or current site groups.
    /// </summary>
    public bool ShowCurrentSiteGroups
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrentSiteGroups"), false);
        }
        set
        {
            SetValue("ShowCurrentSiteGroups", value);
        }
    }


    /// <summary>
    /// Enables or Disables multi selection. Default is multi select.
    /// </summary>
    public bool MultiSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("MultiSelection"), true);
        }
        set
        {
            SetValue("MultiSelection", value);
        }
    }


    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return usGroups.ValueDisplayName;
        }
    }


    /// <summary>
    /// Helper parameter - site id.
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
    /// Gets or sets site id.
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
    /// Gets or sets the value that indicates whether friendly mode should be used. Display name is displayed instead code name.
    /// </summary>
    public bool UseFriendlyMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseFriendlyMode"), false);
        }
        set
        {
            SetValue("UseFriendlyMode", value);
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
    /// Gets the current UniSelector instance.
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
            return Convert.ToString(usGroups.Value);
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
    /// Gets or sets value - group name.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return Convert.ToString(usGroups.Value);
        }
        set
        {
            EnsureChildControls();
            usGroups.Value = value;
        }
    }


    /// <summary>
    /// Gets Value element id.
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
    /// Gets or sets if live site property.
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

    /// <summary>
    /// Gets or sets stop processing.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return usGroups.StopProcessing;
        }
        set
        {
            usGroups.StopProcessing = value;
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
            string where = "";
            
            if (SiteID > 0)
            {
                where = "GroupSiteID =" + SiteID;
            }

            if (ShowCurrentSiteGroups)
            {
                where = SqlHelper.AddWhereCondition(where, "(GroupSiteID = " + CurrentSite.SiteID + ")");
            }

            usGroups.WhereCondition = where;

            usGroups.AllowEmpty = true;
        }

        usGroups.SelectionMode = (MultiSelection) ? SelectionModeEnum.MultipleTextBox : SelectionModeEnum.SingleTextBox;
    }


    /// <summary>
    ///  Page_PreRender event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Settings for (un)friendly mode
            if (UseFriendlyMode)
            {
                usGroups.ButtonClear.Visible = true;
                usGroups.TextBoxSelect.ReadOnly = true;
                usGroups.DisplayNameFormat = "{%GroupDisplayName%}";
            }
            else
            {
                usGroups.ButtonClear.Visible = false;
                usGroups.TextBoxSelect.ReadOnly = false;
                usGroups.DisplayNameFormat = "{%GroupName%}";
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
    /// Creates child controls and loads update panel container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load update panel container
        if (usGroups == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }

    #endregion
}