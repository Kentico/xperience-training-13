using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_UI_ProductOptions_SelectVariantCategory : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets ProductID.
    /// </summary>
    public int ProductID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets list of unselected items.
    /// </summary>
    public IEnumerable<int> UnSelectedCategories
    {
        get
        {
            IEnumerable<int> allCategoriesIds = from c in AllCategoriesOptions.Keys
                                                select ValidationHelper.GetInteger(c["CategoryId"], 0);

            IEnumerable<int> avaibleCategoriesIds = from k in SelectedCategories
                                                    where k.Value != VariantOptionInfo.ExistingUnselectedOption
                                                    select k.Key;

            return allCategoriesIds.Except(avaibleCategoriesIds);
        }
    }


    /// <summary>
    /// Gets selected items - Dictionary[CategoryId, OptionId].
    /// </summary>
    public Dictionary<int, int> SelectedCategories
    {
        get
        {
            if (ViewState["Categories"] is Dictionary<int, int>)
            {
                return (Dictionary<int, int>)ViewState["Categories"];
            }

            return null;
        }
        private set
        {
            ViewState["Categories"] = value;
        }
    }


    /// <summary>
    /// Gets or sets all attribute categories with all options.
    /// </summary>
    public Dictionary<OptionCategoryInfo, List<SKUInfo>> AllCategoriesOptions
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets attribute categories with all options which are in variants.
    /// </summary>
    public Dictionary<OptionCategoryInfo, List<SKUInfo>> VariantCategoriesOptions
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event fired after checkbox or dropdown selection changed.
    /// </summary>
    public event EventHandler OnSelectionChanged;

    #endregion


    #region "Page events"

    /// <summary>
    /// Overrides OnInit method.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SelectedCategories = new Dictionary<int, int>();
    }


    /// <summary>
    /// Overrides OnLoad method.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadControl();
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Fires when dropdownlist selected item is changed.
    /// </summary>
    void checkBoxWithDropDown_OnDropDownSelectionChanged(object sender, EventArgs e)
    {
        var checkBoxWithDropDown = sender as CheckBoxWithDropDown;
        if (checkBoxWithDropDown != null)
        {
            SelectedCategories[checkBoxWithDropDown.Value] = checkBoxWithDropDown.SelectedDropDownItem;

            // Raise the SelectionChanged event
            if (OnSelectionChanged != null)
            {
                OnSelectionChanged(this, e);
            }
        }
    }


    /// <summary>
    /// Fires when checkbox selection is changed.
    /// </summary>
    void checkBoxWithDropDown_OnCheckBoxSelectionChanged(object sender, EventArgs e)
    {
        var checkBoxWithDropDown = sender as CheckBoxWithDropDown;
        if (checkBoxWithDropDown != null)
        {
            if (checkBoxWithDropDown.CheckboxChecked)
            {
                if (checkBoxWithDropDown.DropDownVisible)
                {
                    SelectedCategories.Add(checkBoxWithDropDown.Value, VariantOptionInfo.ExistingUnselectedOption);
                }
                else
                {
                    SelectedCategories.Add(checkBoxWithDropDown.Value, VariantOptionInfo.NewOption);
                }
            }
            else
            {
                SelectedCategories.Remove(checkBoxWithDropDown.Value);
            }

            // Raise the SelectionChanged event
            if (OnSelectionChanged != null)
            {
                OnSelectionChanged(this, e);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads control.
    /// </summary>
    private void LoadControl()
    {
        // Get all product categories with options which are already in variants
        if (VariantCategoriesOptions.Count == 0)
        {
            DataSet variantCategoriesDS = VariantHelper.GetProductVariantsCategories(ProductID);
            FillCategoriesOptionsDictionary(VariantCategoriesOptions, variantCategoriesDS);
        }

        // Get all product attribute categories with options
        if (AllCategoriesOptions.Count == 0)
        {
            DataSet allCategoriesDS = OptionCategoryInfoProvider.GetProductOptionCategories(ProductID, true, OptionCategoryTypeEnum.Attribute);
            FillCategoriesOptionsDictionary(AllCategoriesOptions, allCategoriesDS);
        }

        foreach (KeyValuePair<OptionCategoryInfo, List<SKUInfo>> keyValuePair in AllCategoriesOptions)
        {
            if (keyValuePair.Value.Count > 0)
            {
                OptionCategoryInfo optionCategory = keyValuePair.Key;

                // Create new instance of CheckBoxWithDropDown control and prefill all necessary values
                CheckBoxWithDropDown checkBoxWithDropDown = new CheckBoxWithDropDown();
                checkBoxWithDropDown.ID = ValidationHelper.GetString(optionCategory.CategoryID, string.Empty);
                checkBoxWithDropDown.Value = optionCategory.CategoryID;
                // Use live site display name instead of category display name in case it is available
                checkBoxWithDropDown.CheckboxText = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(optionCategory.CategoryTitle));
                
                // Attach listeners
                checkBoxWithDropDown.OnCheckBoxSelectionChanged += checkBoxWithDropDown_OnCheckBoxSelectionChanged;
                checkBoxWithDropDown.OnDropDownSelectionChanged += checkBoxWithDropDown_OnDropDownSelectionChanged;

                // Option category is in variants too
                if (VariantCategoriesOptions.Keys.Any(c => ValidationHelper.GetInteger(c["categoryId"], 0) == optionCategory.CategoryID))
                {
                    // Check and disable checkbox
                    checkBoxWithDropDown.CheckboxChecked = true;
                    checkBoxWithDropDown.Enabled = false;

                    // Already existing variants add to selected categories too
                    if (!SelectedCategories.ContainsKey(optionCategory.CategoryID))
                    {
                        SelectedCategories.Add(optionCategory.CategoryID, VariantOptionInfo.ExistingSelectedOption);
                    }

                }
                // Option category is not in variant, but some categories in variants already exists
                else if (VariantCategoriesOptions.Count > 0)
                {
                    // Set prompt message and visibility
                    checkBoxWithDropDown.DropDownPrompt = GetString("general.pleaseselect");
                    checkBoxWithDropDown.DropDownLabel = GetString("com.variants.dropdownlabel");
                    checkBoxWithDropDown.DropDownVisible = true;

                    // Get all product options and bind them to dropdownlist
                    var options = SKUInfoProvider.GetSKUOptionsForProduct(ProductID, optionCategory.CategoryID, true).OrderBy("SKUOrder");
                    var dropDownItems = new ListItemCollection();

                    foreach (var option in options)
                    {
                        dropDownItems.Add(new ListItem(option.SKUName, option.SKUID.ToString()));
                    }

                    checkBoxWithDropDown.DropDownItems = dropDownItems;
                }

                // Finally bind this control to parent
                chboxPanel.Controls.Add(checkBoxWithDropDown);
            }
        }
    }


    /// <summary>
    /// Fills dictionary with categories and their options.
    /// </summary>
    /// <param name="CategoriesOptions">Dictionary to fill</param>
    /// <param name="CategoriesDS">Option categories dataset</param>
    private void FillCategoriesOptionsDictionary(Dictionary<OptionCategoryInfo, List<SKUInfo>> CategoriesOptions, DataSet CategoriesDS)
    {
        if (!DataHelper.DataSourceIsEmpty(CategoriesDS))
        {
            foreach (DataRow categoryRow in CategoriesDS.Tables[0].Rows)
            {
                var categoryId = ValidationHelper.GetInteger(categoryRow["CategoryID"], 0);
                var options = SKUInfoProvider.GetSKUOptionsForProduct(ProductID, categoryId, true).OrderBy("SKUOrder");
                CategoriesOptions.Add(new OptionCategoryInfo(categoryRow), options.ToList());
            }
        }
    }

    #endregion
}
