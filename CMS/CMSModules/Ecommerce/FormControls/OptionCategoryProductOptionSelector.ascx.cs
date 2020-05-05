using System;
using System.Data;

using CMS.Base.Web.UI;
using CMS.Helpers;

using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.FormEngine.Web.UI;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_FormControls_OptionCategoryProductOptionSelector : FormEngineUserControl
{
    #region "Variables"

    private InfoDataSet<SKUInfo> mProductOptionsData;
    private InfoDataSet<SKUInfo> mTextOptionsData;
    private bool? mProductOptionsExist;
    private bool? mTextOptionExists;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Option category ID.
    /// </summary>
    private int OptionCategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Decides of price should be visible next to selection option.
    /// </summary>
    private bool DisplayPrice
    {
        get;
        set;
    }


    /// <summary>
    /// Selected type of category selection type in the form.
    /// </summary>
    private OptionCategorySelectionTypeEnum SelectionType
    {
        get;
        set;
    }


    /// <summary>
    /// Previously selected selection type (by default value stored in the database).
    /// </summary>
    public OptionCategorySelectionTypeEnum PreviouslySelectedSelectionType
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PreviouslySelectedSelectionType"], "").ToEnum<OptionCategorySelectionTypeEnum>();
        }
        set
        {
            ViewState["PreviouslySelectedSelectionType"] = ValidationHelper.GetString(value.ToStringRepresentation(), "");
        }
    }


    /// <summary>
    /// Gets selection control.
    /// </summary>
    private Control SelectionControl
    {
        get
        {
            return GetSelectionControl(SelectionType);
        }
    }


    /// <summary>
    /// Previously selected selection control.
    /// </summary>
    private Control PreviouslySelectedSelectionControl
    {
        get
        {
            return GetSelectionControl(PreviouslySelectedSelectionType);
        }
    }


    /// <summary>
    /// Additional option item.
    /// </summary>
    private ListItem AdditionalOptionItem
    {
        get
        {
            if (string.IsNullOrEmpty(AdditionalOptionText))
            {
                return null;
            }

            // Create additional list item
            return new ListItem(AdditionalOptionText, AdditionalOptionText);
        }
    }


    /// <summary>
    /// Additional item text.
    /// </summary>
    private string AdditionalOptionText
    {
        get;
        set;
    }


    /// <summary>
    /// Data set with the product options data which should be loaded to the product options selector.
    /// </summary>
    private InfoDataSet<SKUInfo> ProductOptionsData
    {
        get
        {
            if (DataHelper.DataSourceIsEmpty(mProductOptionsData))
            {
                // Get the data
                mProductOptionsData = SKUInfoProvider.GetSKUOptions(OptionCategoryID, true).Where("SKUProductType", QueryOperator.NotEquals, "TEXT").OrderBy("SKUOrder").TypedResult;
            }

            return mProductOptionsData;
        }
    }


    /// <summary>
    /// Decides if product or attribute options exist.
    /// </summary>
    private bool ProductOptionsExist
    {
        get
        {
            if (mProductOptionsExist == null)
            {
                mProductOptionsExist = !DataHelper.DataSourceIsEmpty(ProductOptionsData);
            }

            return mProductOptionsExist.Value;
        }
    }


    /// <summary>
    /// Data set with the text option data which should be loaded to the product options selector.
    /// </summary>
    private InfoDataSet<SKUInfo> TextOptionData
    {
        get
        {
            if (DataHelper.DataSourceIsEmpty(mProductOptionsData))
            {
                // Get the data
                mTextOptionsData = SKUInfoProvider.GetSKUOptions(OptionCategoryID, true).Where("SKUProductType", QueryOperator.Equals, "TEXT").TypedResult;
            }

            return mTextOptionsData;
        }
    }


    /// <summary>
    /// Decide if text option exists.
    /// </summary>
    private bool TextOptionsExists
    {
        get
        {
            if (mTextOptionExists == null)
            {
                mTextOptionExists = !DataHelper.DataSourceIsEmpty(TextOptionData);
            }

            return mTextOptionExists.Value;
        }
    }


    /// <summary>
    /// Checks if selected category type is text.
    /// </summary>
    private bool SelectedTextCategory
    {
        get
        {
            return (SelectionType == OptionCategorySelectionTypeEnum.TextArea) || (SelectionType == OptionCategorySelectionTypeEnum.TextBox);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Selected option category values separated by colon.
    /// </summary>
    public override object Value
    {
        get;
        set;
    }


    /// <summary>
    /// Stores column with display price property value.
    /// </summary>
    public string OptionCategoryDisplayPriceColumn
    {
        get
        {
            return GetValue("OptionCategoryDisplayPriceColumn", string.Empty);
        }
        set
        {
            SetValue("OptionCategoryDisplayPriceColumn", value);
        }
    }


    /// <summary>
    /// Stores column with option category default record value.
    /// </summary>
    public string OptionCategoryDefaultRecordColumn
    {
        get
        {
            return GetValue("OptionCategoryDefaultRecordColumn", string.Empty);
        }
        set
        {
            SetValue("OptionCategoryDefaultRecordColumn", value);
        }
    }


    /// <summary>
    /// Stores column with option category selection type value.
    /// </summary>
    public string OptionCategorySelectionTypeColumn
    {
        get
        {
            return GetValue("OptionCategorySelectionTypeColumn", string.Empty);
        }
        set
        {
            SetValue("OptionCategorySelectionTypeColumn", value);
        }
    }


    /// <summary>
    /// Stores column with option category ID column.
    /// </summary>
    public string OptionCategoryIDColumn
    {
        get
        {
            return GetValue("OptionCategoryIDColumn", string.Empty);
        }
        set
        {
            SetValue("OptionCategoryIDColumn", value);
        }
    }


    /// <summary>
    /// Stores column with option category type value.
    /// </summary>
    public string OptionCategoryTypeColumn
    {
        get
        {
            return GetValue("OptionCategoryTypeColumn", string.Empty);
        }
        set
        {
            SetValue("OptionCategoryTypeColumn", value);
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Form == null)
        {
            return;
        }

        OptionCategoryID = ValidationHelper.GetInteger(Form.Data.GetValue(OptionCategoryIDColumn), 0);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get control properties from edited option category
        GetSelectorConfiguration();

        if (RequestHelper.IsPostBack())
        {
            bool wasTextCategory = (PreviouslySelectedSelectionType == OptionCategorySelectionTypeEnum.TextArea) || (PreviouslySelectedSelectionType == OptionCategorySelectionTypeEnum.TextBox);

            // Reset value of selected options field in case selection type was changed to one of selection types of text category type
            if ((wasTextCategory && !SelectedTextCategory) || (!wasTextCategory && SelectedTextCategory))
            {
                Value = null;
            }
            else
            {
                // Set selected values
                Value = GetSelectedOptions();
            }
        }

        // Display newly selected control
        PrepareSelectionControl();

        // Set selected values in newly selected control
        SetSelectedOptions();

        // Remember selected control to be able to restore selected values
        PreviouslySelectedSelectionType = SelectionType;
    }


    /// <summary>
    /// Reads option category configuration fields.
    /// </summary>
    private void GetSelectorConfiguration()
    {
        if (Form != null)
        {
            // Display price adjustment
            if (Form.FieldControls.Contains(OptionCategoryDisplayPriceColumn))
            {
                DisplayPrice = ValidationHelper.GetBoolean(Form.FieldControls[OptionCategoryDisplayPriceColumn].Value, false);
            }

            // Default record text
            if (Form.FieldControls.Contains(OptionCategoryDefaultRecordColumn))
            {
                AdditionalOptionText = ValidationHelper.GetString(Form.FieldControls[OptionCategoryDefaultRecordColumn].Value, "").Trim();
            }

            // Selection type
            if (Form.FieldControls.Contains(OptionCategorySelectionTypeColumn))
            {
                SelectionType = ValidationHelper.GetString(Form.FieldControls[OptionCategorySelectionTypeColumn].Value, "").ToEnum<OptionCategorySelectionTypeEnum>();
            }
        }
    }

    #endregion


    #region "Methods"

    protected void SelectionControl_DataBound(object sender, EventArgs e)
    {
        // Check if data source has data and bind selection control if data are available
        if (!DataHelper.DataSourceIsEmpty(((ListControl)sender).DataSource))
        {
            foreach (DataRow row in ((DataSet)((ListControl)sender).DataSource).Tables[0].Rows)
            {
                int skuId = ValidationHelper.GetInteger(row["SKUID"], 0);
                var item = ((ListControl)sender).Items.FindByValue(skuId.ToString());

                if (item != null)
                {
                    if (sender != ddlDropDown)
                    {
                        item.Text = HTMLHelper.HTMLEncode(item.Text);
                    }

                    if (DisplayPrice)
                    {
                        // Append price to option's text
                        item.Text += GetPrice(row);
                    }
                }
            }
        }

        if ((AdditionalOptionItem != null) && (SelectionType != OptionCategorySelectionTypeEnum.CheckBoxesHorizontal)
                                           && (SelectionType != OptionCategorySelectionTypeEnum.CheckBoxesVertical)
                                           && (SelectionType != OptionCategorySelectionTypeEnum.RadioButtonsHorizontal)
                                           && (SelectionType != OptionCategorySelectionTypeEnum.RadioButtonsVertical))
        {
            if (sender != ddlDropDown)
            {
                AdditionalOptionText = HTMLHelper.HTMLEncode(AdditionalOptionText);
            }

            // Insert default item on top of the list
            if (SelectionControl is ListControl)
            {
                ((ListControl)SelectionControl).Items.Insert(0, AdditionalOptionItem);
            }
        }
    }


    /// <summary>
    /// Gets selected product options from the selection control.
    /// </summary>
    private string GetSelectedOptions()
    {
        switch (PreviouslySelectedSelectionType)
        {
            // Return text for text types
            case OptionCategorySelectionTypeEnum.TextBox:
            case OptionCategorySelectionTypeEnum.TextArea:
                return ((TextBox)(PreviouslySelectedSelectionControl)).Text.Trim();

            // Return all values separated by ',' for multiple choice types
            case OptionCategorySelectionTypeEnum.CheckBoxesHorizontal:
            case OptionCategorySelectionTypeEnum.CheckBoxesVertical:
                var selectedItems = ((CMSCheckBoxList)PreviouslySelectedSelectionControl).GetSelectedItems();
                if (selectedItems != null)
                {
                    var listItems = selectedItems.ToList();
                    if (listItems.Any())
                    {
                        return listItems.Select(i => i.Value).Aggregate((current, next) => current + "," + next);
                    }
                }

                return "";

                // Return selected value for single choice types
            default:
                return ((ListControl)PreviouslySelectedSelectionControl).SelectedValue;
        }
    }


    /// <summary>
    /// Gets selected product options from the selection control.
    /// </summary>
    public void SetSelectedOptions()
    {
        string selectedValue = ValidationHelper.GetString(Value, "");

        if (string.IsNullOrEmpty(selectedValue))
        {
            return;
        }

        switch (SelectionType)
        {
            // Set text for text types
            case OptionCategorySelectionTypeEnum.TextBox:
            case OptionCategorySelectionTypeEnum.TextArea:
                ((TextBox)(SelectionControl)).Text = selectedValue;

                break;

            // Set values for multiple choice types
            case OptionCategorySelectionTypeEnum.CheckBoxesHorizontal:
            case OptionCategorySelectionTypeEnum.CheckBoxesVertical:
                foreach (var skuId in selectedValue.Split(','))
                {
                    // Ensure value is not empty
                    string itemVal = (skuId != "") ? skuId : "0";

                    ListItem item = ((CMSCheckBoxList)SelectionControl).Items.FindByValue(itemVal);
                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }

                break;

            // Set selected value for single choice types
            default:
                ListControl list = (ListControl)SelectionControl;

                string val = selectedValue.Split(',')[0];
                if (list.Items.FindByValue(val) != null)
                {
                    list.SelectedValue = val;
                }

                break;
        }
    }


    /// <summary>
    /// Loads data (SKU options) to the selection control.
    /// </summary>
    public void LoadSelectorOptions()
    {
        // Only for none-text types
        if (SelectionControl is TextBox)
        {
            if (TextOptionsExists)
            {
                DataRow dr = TextOptionData.Tables[0].Rows[0];
                lblTextPrice.Text = DisplayPrice ? GetPrice(dr) : "";
            }
        }
        else
        {
            if (ProductOptionsExist)
            {
                var control = SelectionControl as ListControl;
                if (control != null)
                {
                    control.DataSource = ProductOptionsData;
                    SelectionControl.DataBind();
                }
            }
        }
    }


    /// <summary>
    /// Shows appropriate selection control, sets up its properties and loads options.
    /// </summary>
    protected void PrepareSelectionControl()
    {
        ShowSelectionControl();

        switch (SelectionType)
        {
            case OptionCategorySelectionTypeEnum.CheckBoxesHorizontal:
                chbCheckBoxes.RepeatDirection = RepeatDirection.Horizontal;
                break;

            case OptionCategorySelectionTypeEnum.CheckBoxesVertical:
                chbCheckBoxes.RepeatDirection = RepeatDirection.Vertical;
                break;

            case OptionCategorySelectionTypeEnum.RadioButtonsHorizontal:
                rblRadioButtons.RepeatDirection = RepeatDirection.Horizontal;
                break;

            case OptionCategorySelectionTypeEnum.RadioButtonsVertical:
                rblRadioButtons.RepeatDirection = RepeatDirection.Vertical;
                break;

            case OptionCategorySelectionTypeEnum.TextBox:
                txtText.TextMode = TextBoxMode.SingleLine;
                break;

            case OptionCategorySelectionTypeEnum.TextArea:
                txtText.TextMode = TextBoxMode.MultiLine;
                break;
        }

        LoadSelectorOptions();
    }


    /// <summary>
    /// Shows selection control according to SelectionType property.
    /// </summary>
    protected void ShowSelectionControl()
    {
        ddlDropDown.Visible = false;
        chbCheckBoxes.Visible = false;
        rblRadioButtons.Visible = false;
        txtText.Visible = false;
        lblInfoMessage.Visible = false;

        // Display or hide default options selector
        var displaySelector = ProductOptionsExist || SelectedTextCategory;
        SelectionControl.Visible = displaySelector;
        lblInfoMessage.Visible = !displaySelector;
        lblTextPrice.Visible = displaySelector && DisplayPrice;
    }


    /// <summary>
    /// Recalculates and formats price.
    /// </summary>
    /// <param name="row">Data row to create price label for.</param>
    private string GetPrice(DataRow row)
    {
        var sku = new SKUInfo(row);

        // Get site main currency
        var currency = CurrencyInfoProvider.GetMainCurrency(sku.SKUSiteID);

        // Get product price
        var price = sku.SKUPrice;

        if (price != 0)
        {
            var roundingServiceFactory = Service.Resolve<IRoundingServiceFactory>();
            var roundingService = roundingServiceFactory.GetRoundingService(SiteContext.CurrentSiteID);

            // Round price
            price = roundingService.Round(price, currency);

            // Prevent double encoding in DDL
            bool encode = SelectionType != OptionCategorySelectionTypeEnum.Dropdownlist;

            // Format price
            string formattedPrice = CurrencyInfoProvider.GetRelativelyFormattedPrice(price, currency, encode);

            return " (" + formattedPrice + ")";
        }

        return string.Empty;
    }


    /// <summary>
    /// Finds used selection control.
    /// </summary>
    /// <param name="selectionType"></param>
    private Control GetSelectionControl(OptionCategorySelectionTypeEnum selectionType)
    {
        switch (selectionType)
        {
            case OptionCategorySelectionTypeEnum.RadioButtonsHorizontal:
            case OptionCategorySelectionTypeEnum.RadioButtonsVertical:
                return rblRadioButtons;

            case OptionCategorySelectionTypeEnum.CheckBoxesHorizontal:
            case OptionCategorySelectionTypeEnum.CheckBoxesVertical:
                return chbCheckBoxes;

            case OptionCategorySelectionTypeEnum.TextBox:
            case OptionCategorySelectionTypeEnum.TextArea:
                return txtText;

            default:
                return ddlDropDown;
        }
    }

    #endregion
}