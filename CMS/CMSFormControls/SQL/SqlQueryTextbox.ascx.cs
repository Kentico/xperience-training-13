using System;
using System.Text.RegularExpressions;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_SQL_SqlQueryTextbox : SqlFormControl
{
    /// <summary>
    /// Editing textbox
    /// </summary>
    protected override CMSTextBox TextBoxControl
    {
        get
        {
            return txtRegEx;
        }
    }


    /// <summary>
    /// Type of RegEx eg WhereRegEx, OrderByRegEx, Columns
    /// </summary>
    protected SQLQueryEnum QueryPart
    {
        get
        {
            return (SQLQueryEnum)ValidationHelper.GetInteger(GetValue("QueryPart"), 0);
        }
        set
        {
            SetValue("QueryPart", value);
        }
    }


    /// <summary>
    /// Gets the regular expression for the safe value
    /// </summary>
    protected override Regex GetSafeRegEx()
    {
        return SelectSafeRegEx(QueryPart);
    }


    /// <summary>
    /// Gets the regular expression for the safe value
    /// </summary>
    protected Regex SelectSafeRegEx(SQLQueryEnum regExType)
    {
        switch (regExType)
        {
            case SQLQueryEnum.Where:
                return SqlSecurityHelper.WhereRegex;
            case SQLQueryEnum.OrderBy:
                return SqlSecurityHelper.OrderByRegex;
            case SQLQueryEnum.Columns:
                return SqlSecurityHelper.ColumnsRegex;
            default: return SqlSecurityHelper.ColumnsRegex;
        }
    }


    /// <summary>
    /// Gets the regular expression for the safe value
    /// </summary>
    protected Regex GetSafeRegEx(SQLQueryEnum regExType)
    {
        return SelectSafeRegEx(regExType);
    }


    /// <summary>
    /// Set CheckMinMaxLength and CheckRegularExpression.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected new void Page_Load(object sender, EventArgs e)
    {
        CheckMinMaxLength = true;
        CheckRegularExpression = true;
    }
}