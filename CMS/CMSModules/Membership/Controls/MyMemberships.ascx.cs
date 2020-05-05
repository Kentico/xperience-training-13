using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Membership_Controls_MyMemberships : CMSAdminControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int UserID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the URL of 'buy membership' page.
    /// </summary>
    public string BuyMembershipURL
    {
        get
        {
            return btnBuyMembership.PostBackUrl;
        }
        set
        {
            btnBuyMembership.PostBackUrl = UrlResolver.ResolveUrl(value);
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        membershipsGrid.StopProcessing = StopProcessing;

        // Set where condition
        membershipsGrid.WhereCondition = String.Format("UserID = {0}", UserID);

        // Set pager links text on live site
        if (IsLiveSite)
        {
            membershipsGrid.Pager.FirstPageText = "&lt;&lt;";
            membershipsGrid.Pager.LastPageText = "&gt;&gt;";
            membershipsGrid.Pager.PreviousPageText = "&lt;";
            membershipsGrid.Pager.NextPageText = "&gt;";
            membershipsGrid.Pager.PreviousGroupText = "...";
            membershipsGrid.Pager.NextGroupText = "...";
        }

    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show buy membership button if it has URL
        btnBuyMembership.Visible = !String.IsNullOrEmpty(btnBuyMembership.PostBackUrl);
    }

    #endregion


    #region "Methods"

    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        switch (propertyName.ToLowerCSafe())
        {
            case "userid":
                UserID = ValidationHelper.GetInteger(value, 0);
                break;

            case "buymembershipurl":
                BuyMembershipURL = ValidationHelper.GetString(value, null);
                break;
        }

        return true;
    }


    protected object membershipsUniGridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "validto":
                DateTime validTo = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);

                // Format unlimited membership
                if (validTo.CompareTo(DateTimeHelper.ZERO_TIME) == 0)
                {
                    return "-";
                }
                break;
        }

        return parameter;
    }

    #endregion
}