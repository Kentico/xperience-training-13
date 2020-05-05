using System;
using System.Web.UI.WebControls;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;


public partial class CMSModules_Badges_FormControls_BadgeSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets a value indicating whether the control is enabled.
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
            drpBadges.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the identifier of the selected badge.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpBadges.SelectedValue;
        }
        set
        {
            drpBadges.SelectedValue = ValidationHelper.GetString(value, String.Empty);
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (drpBadges.Items.Count > 0)
        {
            return;
        }

        var badges = BadgeInfo.Provider.Get().Columns("BadgeDisplayName, BadgeIsAutomatic, BadgeID");
        foreach (BadgeInfo badge in badges)
        {
            string badgeDisplayName = ResHelper.LocalizeString(badge.BadgeDisplayName);
            if (badge.BadgeIsAutomatic)
            {
                badgeDisplayName += GetString("badge.automatic");
            }
            drpBadges.Items.Add(new ListItem(badgeDisplayName, badge.BadgeID.ToString()));
        }
        drpBadges.Items.Insert(0, new ListItem(GetString("general.selectnone"), "0"));
    }

    #endregion
}