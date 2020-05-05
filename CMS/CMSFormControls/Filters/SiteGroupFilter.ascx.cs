using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_Filters_SiteGroupFilter : CMSAbstractBaseFilterControl
{
    #region "Properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            if (ShowSiteFilter)
            {
                base.WhereCondition = SqlHelper.AddWhereCondition(siteFilter.WhereCondition, GenerateWhereCondition(SelectedGroupID));
            }
            else
            {
                base.WhereCondition = GenerateWhereCondition(SelectedGroupID);
            }

            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Filtered control.
    /// </summary>
    public override Control FilteredControl
    {
        get
        {
            return siteFilter.FilteredControl;
        }
        set
        {
            base.FilteredControl = value;
            siteFilter.FilteredControl = value;
        }
    }


    /// <summary>
    /// Indicates if site filter should be displayed.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(((CMSUserControl)siteFilter.FilteredControl).GetValue("ShowSiteFilter"), false);
        }
    }


    /// <summary>
    /// Document group ID.
    /// </summary>
    public int GroupID
    {
        get
        {
            return ValidationHelper.GetInteger(((CMSUserControl)siteFilter.FilteredControl).GetValue("GroupID"), 0);
        }
    }


    /// <summary>
    /// Selected group.
    /// </summary>
    private int SelectedGroupID
    {
        get
        {
            return ValidationHelper.GetInteger(drpGroup.SelectedValue, 0);
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// OnLoad override - check wheter filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Hide default site filter label
        siteFilter.ShowLabel = false;
        siteFilter.Selector.OnSelectionChanged += Filter_Changed;

        // Check if site filter should be displayed
        if (ShowSiteFilter)
        {
            lblSite.ResourceString = "general.site";
            lblSite.DisplayColon = true;
        }
        else
        {
            plcSite.Visible = false;
        }

        // Check if group filter should be displayed
        if (GroupID > 0)
        {
            // Initialize DDL
            if (!RequestHelper.IsPostBack())
            {
                string groupName = String.Empty;
                GeneralizedInfo gi = ModuleCommands.CommunityGetGroupInfo(GroupID);
                if (gi != null)
                {
                    groupName = ValidationHelper.GetString(gi.GetValue("GroupDisplayName"), "");
                }

                // Initialize DDL using obtained group name
                switch (siteFilter.FilterMode)
                {
                    case "user":
                        drpGroup.Items.Add(new ListItem(ResHelper.GetString("sitegroupselector.generalusers"), "0"));
                        if (!String.IsNullOrEmpty(groupName))
                        {
                            drpGroup.Items.Add(new ListItem(groupName, GroupID.ToString()));
                        }
                        break;

                    case "role":
                        drpGroup.Items.Add(new ListItem(ResHelper.GetString("sitegroupselector.generalroles"), "0"));
                        if (!String.IsNullOrEmpty(groupName))
                        {
                            drpGroup.Items.Add(new ListItem(groupName, GroupID.ToString()));
                        }
                        break;
                }
            }

            // Initialize group label
            lblGroup.Visible = true;
            lblGroup.ResourceString = "general.group";
            lblGroup.DisplayColon = true;
        }
        else
        {
            plcGroup.Visible = false;
        }

        // Generate current where condition 
        if (ShowSiteFilter)
        {
            WhereCondition = SqlHelper.AddWhereCondition(siteFilter.WhereCondition, GenerateWhereCondition(SelectedGroupID));
        }
        else
        {
            WhereCondition = GenerateWhereCondition(SelectedGroupID);
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Generates where condition.
    /// </summary>
    protected string GenerateWhereCondition(int groupId)
    {
        if (!String.IsNullOrEmpty(siteFilter.FilterMode))
        {
            switch (siteFilter.FilterMode)
            {
                case "user":
                    {
                        // If some group filter users
                        if (groupId > 0)
                        {
                            return "UserID IN(SELECT MemberUserID FROM Community_GroupMember WHERE MemberGroupID = " + groupId + ")";
                        }
                    }
                    break;

                case "role":
                    {
                        // If some group filter users
                        if (groupId > 0)
                        {
                            return "RoleGroupID = " + groupId;
                        }

                        return "RoleGroupID IS NULL";
                    }
            }
        }
        return String.Empty;
    }

    #endregion


    #region "Other events"

    /// <summary>
    /// Handles group DDL OnSelectionChanged event.
    /// </summary>
    protected void drpGroup_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }


    /// <summary>
    /// Handles site DDL OnSelectionChanged event.
    /// </summary>
    protected void Filter_Changed(object sender, EventArgs e)
    {
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }

    #endregion
}