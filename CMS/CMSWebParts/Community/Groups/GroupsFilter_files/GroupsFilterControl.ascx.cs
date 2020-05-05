using System;

using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;

public partial class CMSWebParts_Community_Groups_GroupsFilter_files_GroupsFilterControl : CMSAbstractBaseFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            if (string.IsNullOrEmpty(btnSelect.Text))
            {
                btnSelect.Text = ResHelper.GetString("general.search");
            }
            return btnSelect.Text;
        }
        set
        {
            btnSelect.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the group name link text.
    /// </summary>
    public string SortGroupNameLinkText
    {
        get
        {
            if (string.IsNullOrEmpty(lnkSortByGroupName.Text))
            {
                lnkSortByGroupName.Text = ResHelper.GetString("unigrid.forums.columns.groupname");
            }
            return lnkSortByGroupName.Text;
        }
        set
        {
            lnkSortByGroupName.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the group created link text.
    /// </summary>
    public string SortGroupCreatedLinkText
    {
        get
        {
            if (string.IsNullOrEmpty(lnkSortByGroupCreated.Text))
            {
                lnkSortByGroupCreated.Text = ResHelper.GetString("groups.created");
            }
            return lnkSortByGroupCreated.Text;
        }
        set
        {
            lnkSortByGroupCreated.Text = value;
        }
    }

    #endregion


    /// <summary>
    /// OnLoad override - check wheter filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        // Set filter only if it is not filter request
        if (Request.Form[btnSelect.UniqueID] == null)
        {
            // Try to get where condition
            string wherePart = ValidationHelper.GetString(ViewState["FilterCondition"], string.Empty);
            if (!string.IsNullOrEmpty(wherePart))
            {
                // Set where condition and raise OnFilter change event
                WhereCondition = GenerateWhereCondition(wherePart);
                // Raise event
                RaiseOnFilterChanged();
            }
        }
        if ((Request.Form[lnkSortByGroupName.UniqueID] == null) && (Request.Form[lnkSortByGroupCreated.UniqueID] == null))
        {
            string orderByPart = ValidationHelper.GetString(ViewState["OrderClause"], string.Empty);
            if (!string.IsNullOrEmpty(orderByPart))
            {
                // Set order by clause and raise OnFilter change event
                OrderBy = orderByPart;
                // Raise event
                RaiseOnFilterChanged();
            }
        }
        lblSortBy.Text = ResHelper.GetString("general.sortby") + ":";
        lnkSortByGroupName.Text = SortGroupNameLinkText;
        lnkSortByGroupCreated.Text = SortGroupCreatedLinkText;
        btnSelect.Text = ButtonText;
        base.OnLoad(e);
    }


    /// <summary>
    /// Select button handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        // Set where condition
        WhereCondition = GenerateWhereCondition(txtValue.Text);
        // Save filter condition
        ViewState["FilterCondition"] = txtValue.Text;
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }


    protected void lnkSortByGroupName_Click(object sender, EventArgs e)
    {
        // Get order by clause from viewstate
        OrderBy = ValidationHelper.GetString(ViewState["OrderClause"], string.Empty);
        // Set new order by clause
        OrderBy = OrderBy.Contains("GroupName DESC") ? "GroupName ASC" : "GroupName DESC";
        // Save new order by clause to viewstate
        ViewState["OrderClause"] = OrderBy;
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }


    protected void lnkSortByGroupCreated_Click(object sender, EventArgs e)
    {
        // Get order by clause from viewstate
        OrderBy = ValidationHelper.GetString(ViewState["OrderClause"], string.Empty);
        // Set new order by clause
        OrderBy = OrderBy.Contains("GroupCreatedWhen DESC") ? "GroupCreatedWhen ASC" : "GroupCreatedWhen DESC";
        // Save new order by clause to viewstate
        ViewState["OrderClause"] = OrderBy;
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    /// <param name="searchPhrase">Phrase to be searched</param>
    /// <returns>Where condition for given phrase.</returns>
    protected static string GenerateWhereCondition(string searchPhrase)
    {
        searchPhrase = SqlHelper.GetSafeQueryString(searchPhrase, false);
        return "GroupDisplayName LIKE N'%" + searchPhrase + "%'";
    }
}