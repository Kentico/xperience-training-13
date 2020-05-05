using System;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_Ecommerce_Controls_ProductOptions_ProductOptionSelector : ProductOptionSelector
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadSelector();
    }


    /// <summary>
    /// Loads selector's data.
    /// </summary>
    private void LoadSelector()
    {
        if (SelectionControl != null)
        {
            // Add selection control to the collection 
            pnlSelector.Controls.Add(SelectionControl);

            if (SelectionControl is FormEngineUserControl)
            {
                FormEngineUserControl fc = (FormEngineUserControl)SelectionControl;
                fc.SetValue("ProductOptionCategoryID", OptionCategoryId);
                fc.IsLiveSite = false;
            }

            if (IsSelectionControlEmpty())
            {
                // Load selection control data according to the options category data
                ReloadData();
            }

            // There is no choice -> hide control
            if (!HasSelectableOptions())
            {
                pnlContainer.Visible = false;
            }
            // Option category is not empty -> display option category details
            else if (OptionCategory != null)
            {
                // Show / hide option category name
                if (ShowOptionCategoryName)
                {
                    // Show category title (live site or ordinary display name)
                    lblCategName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(OptionCategory.CategoryTitle));
                }
                else
                {
                    lblCategName.Visible = false;
                }

                // Show / hide option category description
                if (ShowOptionCategoryDescription)
                {
                    lblCategDescription.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(OptionCategory.CategoryDescription));
                }
                else
                {
                    lblCategDescription.Visible = false;
                }
            }

            // WAI validation
            if (OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.Dropdownlist)
            {
                lblCategName.AssociatedControlClientID = SelectionControl.ClientID;
            }
        }
    }


    /// <summary>
    /// Validates selected/entered product option. If it is valid, returns true, otherwise returns false.
    /// </summary> 
    public override bool IsValid()
    {
        // Validation for TextBox and TextArea
        if (OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextBox ||
             OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextArea)
        {
            // Get text length
            int textLength = GetSelectedSKUOptions().Trim().Length;

            // Validate min text length
            if ((textLength < OptionCategory.CategoryTextMinLength) && (OptionCategory.CategoryTextMinLength != 0))
            {
                if (textLength == 0)
                {
                    lblError.Text = string.Format(GetString("com.optioncategory.emptytextvalue"), OptionCategory.CategoryTextMinLength);
                }
                else
                {
                    lblError.Text = string.Format(GetString("com.optioncategory.mintextlengthconstraint"), OptionCategory.CategoryTextMinLength);
                }
                plnError.Visible = true;
                return false;
            }

            // Validate max text length
            if ((textLength > OptionCategory.CategoryTextMaxLength) && (OptionCategory.CategoryTextMaxLength != 0))
            {
                lblError.Text = string.Format(GetString("com.optioncategory.maxtextlengthexceeded"), OptionCategory.CategoryTextMaxLength);
                plnError.Visible = true;
                return false;
            }
        }
        // Validation for other controls
        else
        {
            // Validate empty value
            if (!string.IsNullOrEmpty(OptionCategory.CategoryDefaultRecord) && (GetSelectedSKUOptions() == "0"))
            {
                lblError.Text = string.Format(GetString("com.optioncategory.emptyvalue"));
                plnError.Visible = true;
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Show no product options label visibility
    /// </summary>
    /// <param name="visible">Is label "no product options" visible</param>
    protected override void SetEmptyInfoVisibility(bool visible)
    {
        base.SetEmptyInfoVisibility(visible);
        lblNoProductOptions.Visible = visible;
    }
}