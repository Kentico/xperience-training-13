using System;

using CMS.DocumentEngine.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;

public partial class CMSWebParts_Filters_DocumentNameFilter_files_DocumentNameFilterControl : CMSAbstractDataFilterControl
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
    /// Gets or sets the button css class.
    /// </summary>
    public string ButtonCssClass
    {
        get
        {
            return btnSelect.CssClass;
        }
        set
        {
            btnSelect.CssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the label resource string.
    /// </summary>
    public string LabelText
    {
        get
        {
            return lblValue.ResourceString;
        }
        set
        {
            lblValue.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets the label css class.
    /// </summary>
    public string LabelCssClass
    {
        get
        {
            return lblValue.CssClass;
        }
        set
        {
            lblValue.CssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the textbox css class.
    /// </summary>
    public string TextBoxCssClass
    {
        get
        {
            return txtValue.CssClass;
        }
        set
        {
            txtValue.CssClass = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad override - check whether filter is set.
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

        btnSelect.Text = ButtonText;
        base.OnLoad(e);
    }


    /// <summary>
    /// OnPreRender override.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblValue.Display = !string.IsNullOrEmpty(LabelText);
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


    /// <summary>
    /// Generates where condition.
    /// </summary>
    /// <param name="searchPhrase">Phrase to be searched</param>
    /// <returns>Where condition for given phrase.</returns>
    protected static string GenerateWhereCondition(string searchPhrase)
    {
        if (!string.IsNullOrEmpty(searchPhrase))
        {
            return "(DocumentName LIKE N'%" + SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(searchPhrase)) + "%')";
        }

        return string.Empty;
    }

    #endregion
}