using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_ContactManagement_FormControls_SearchContactFullName : CMSAbstractBaseFilterControl
{
    /// <summary>
    /// OnLoad override - check wheter filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        LoadDropDown();
        base.OnLoad(e);
    }


    /// <summary>
    /// Loads dropdown list.
    /// </summary>
    private void LoadDropDown()
    {
        if (!RequestHelper.IsPostBack())
        {
            drpCondition.Items.Clear();
            ControlsHelper.FillListWithTextSqlOperators(drpCondition);
        }
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    protected static string GenerateWhereCondition(string text, string value)
    {
        string searchPhrase = SqlHelper.EscapeQuotes(text);
        const string prefix = "ISNULL(ContactFirstName, '') + CASE WHEN (NULLIF(ContactFirstName,'') IS NULL) THEN '' ELSE ' ' END + ISNULL(ContactMiddleName, '') + CASE WHEN (NULLIF(ContactMiddleName,'') IS NULL) THEN '' ELSE ' ' END + ISNULL(ContactLastName, '')";
        switch (value)
        {
            default:
            case WhereBuilder.LIKE:
                return prefix + " LIKE N'%" + SqlHelper.EscapeLikeText(searchPhrase) + "%'";
            case WhereBuilder.NOT_LIKE:
                return prefix + " NOT LIKE N'%" + SqlHelper.EscapeLikeText(searchPhrase) + "%'";
            case WhereBuilder.EQUAL:
                return prefix + " = N'" + searchPhrase + "'";
            case WhereBuilder.NOT_EQUAL:
                return prefix + " <> N'" + searchPhrase + "'";
        }
    }


    /// <summary>
    /// Select button handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        // Set where condition
        WhereCondition = GenerateWhereCondition(txtSearch.Text, drpCondition.SelectedValue);
        //Raise OnFilterChange event
        RaiseOnFilterChanged();
    }
}