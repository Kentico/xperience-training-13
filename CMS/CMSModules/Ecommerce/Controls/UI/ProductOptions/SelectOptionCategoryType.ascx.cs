using System;

using CMS.Helpers;

using System.Linq;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_UI_ProductOptions_SelectOptionCategoryType : CMSUserControl
{
    #region "Events"

    public event EventHandler OnSelectionChanged;

    #endregion


    #region "Properties"

    /// <summary>
    /// Selected type of category
    /// </summary>
    public OptionCategoryTypeEnum CategoryType
    {
        get
        {
            return drpTypes.SelectedValue.ToEnum<OptionCategoryTypeEnum>();
        }
        set
        {
            ListItem item = drpTypes.Items.FindByValue(ValidationHelper.GetString(value, ""));
            if (item != null)
            {
                item.Selected = true;
            }
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        // Load data
        if (!RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the list of category types.
    /// </summary>
    private void ReloadData()
    {
        // Clear items
        drpTypes.Items.Clear();

        // Add items
        AddItem("com.optioncategorytype.products", OptionCategoryTypeEnum.Products);
        AddItem("com.optioncategorytype.attribute", OptionCategoryTypeEnum.Attribute);
        AddItem("com.optioncategorytype.text", OptionCategoryTypeEnum.Text);
    }


    /// <summary>
    ///  Adds specific type of category to the list.
    /// </summary>
    /// <param name="resString">Category display name represented by the resource string</param>
    /// <param name="categType">Category type</param>
    private void AddItem(string resString, OptionCategoryTypeEnum categType)
    {
        drpTypes.Items.Add(new ListItem(GetString(resString), categType.ToStringRepresentation()));
    }


    /// <summary>
    /// Handles change of selected type
    /// </summary>
    protected void drpTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (OnSelectionChanged != null)
        {
            OnSelectionChanged(sender, e);
        }
    }

    #endregion
}
