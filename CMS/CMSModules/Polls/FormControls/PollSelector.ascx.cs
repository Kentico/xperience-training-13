using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Polls;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_FormControls_PollSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            string value = uniSelector.Value.ToString();

            if ((FieldInfo.DataType == FieldDataType.Text) && !string.IsNullOrEmpty(value))
            {
                PollInfo poll = PollInfoProvider.GetPollInfo(value, CurrentSite.SiteID);

                if (poll != null)
                {
                    return poll.PollDisplayName;
                }
            }

            return base.ValueDisplayName;
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
            base.Enabled = value;
            uniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets ClientID of the CMSDropDownList with polls.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteFilter"), true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether group polls is included in list.
    /// </summary>
    private bool ShowGroupPolls
    {
        get;
        set;
    }


    private bool? mAllowGlobalPolls = null;

    /// <summary>
    /// Returns if global polls are allowed for current site.
    /// </summary>
    private bool AllowGlobalPolls
    {
        get
        {
            if (mAllowGlobalPolls == null)
            {
                mAllowGlobalPolls = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSPollsAllowGlobal");
            }

            return (bool)mAllowGlobalPolls;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

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
        // Return form name or ID according to type of field (if no field specified form name is returned)
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            object value = uniSelector.Value;
            uniSelector.SelectionMode = SelectionModeEnum.SingleDropDownList;
            uniSelector.AllowEmpty = true;
            uniSelector.Value = value;
            ShowGroupPolls = true;
        }
        else
        {
            uniSelector.OnSelectionChanged += new EventHandler(uniSelector_OnSelectionChanged);
        }

        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ReturnColumnName = "PollID";

        if (ShowSiteFilter && AllowGlobalPolls)
        {
            // Init site filter
            uniSelector.FilterControl = "~/CMSModules/Polls/Controls/Filters/SiteSelector.ascx";
        }

        // Set selector WHERE condition according to user permissions
        int siteId = SiteContext.CurrentSiteID;
        string where = null;
        if (!ShowGroupPolls)
        {
            where = "PollGroupID IS NULL";
        }
        where = SqlHelper.AddWhereCondition(where, "PollSiteID=" + siteId);
        if (AllowGlobalPolls)
        {
            where = SqlHelper.AddWhereCondition(where, "PollSiteID IS NULL AND PollID IN (SELECT PollID FROM Polls_PollSite WHERE SiteID=" + siteId + ")", "OR");
        }
        uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, where);
    }


    protected void uniSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        // Translate selected ID to ".<code name>" (for global polls) or "<codename>" (for site polls)
        int id = ValidationHelper.GetInteger(uniSelector.Text, 0);
        if (id > 0)
        {
            PollInfo pi = PollInfoProvider.GetPollInfo(id);
            if (pi != null)
            {
                uniSelector.Text = string.Empty;
                if (pi.PollSiteID <= 0)
                {
                    uniSelector.Text = ".";
                }

                uniSelector.Text += pi.PollCodeName;
            }
        }
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}