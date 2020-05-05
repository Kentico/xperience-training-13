using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Taxonomy;


public partial class CMSModules_Content_FormControls_Categories_CategorySelector : FormEngineUserControl
{
    #region "Variables"

    private bool mUseCategoryNameForSelection = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Decide which property to get
            if (mUseCategoryNameForSelection)
            {
                return CategoryName;
            }
            else
            {
                return CategoryID;
            }
        }
        set
        {
            // Decide which property to set
            if (mUseCategoryNameForSelection)
            {
                CategoryName = ValidationHelper.GetString(value, "");
            }
            else
            {
                CategoryID = ValidationHelper.GetInteger(value, 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the Category ID.
    /// </summary>
    public int CategoryID
    {
        get
        {
            if (mUseCategoryNameForSelection)
            {
                // Convert ID to name
                string name = ValidationHelper.GetString(selectCategory.Value, "");
                CategoryInfo ngi = CategoryInfoProvider.GetCategoryInfo(name, SiteContext.CurrentSiteName);
                if (ngi != null)
                {
                    return ngi.CategoryID;
                }
                return 0;
            }
            else
            {
                return ValidationHelper.GetInteger(selectCategory.Value, 0);
            }
        }
        set
        {
            if (selectCategory == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (mUseCategoryNameForSelection)
            {
                // Covnert ID to name
                CategoryInfo ngi = CategoryInfoProvider.GetCategoryInfo(value);
                if (ngi != null)
                {
                    selectCategory.Value = ngi.CategoryName;
                }
            }
            else
            {
                selectCategory.Value = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the Category code name.
    /// </summary>
    public string CategoryName
    {
        get
        {
            if (mUseCategoryNameForSelection)
            {
                return ValidationHelper.GetString(selectCategory.Value, "");
            }
            else
            {
                // Convert id to name
                int id = ValidationHelper.GetInteger(selectCategory.Value, 0);
                CategoryInfo ngi = CategoryInfoProvider.GetCategoryInfo(id);
                if (ngi != null)
                {
                    return ngi.CategoryName;
                }
                return "";
            }
        }
        set
        {
            if (selectCategory == null)
            {
                pnlUpdate.LoadContainer();
            }

            if (mUseCategoryNameForSelection)
            {
                selectCategory.Value = value;
            }
            else
            {
                // Convert name to ID
                CategoryInfo ngi = CategoryInfoProvider.GetCategoryInfo(value, SiteContext.CurrentSiteName);
                if (ngi != null)
                {
                    selectCategory.Value = ngi.CategoryID;
                }
            }
        }
    }


    /// <summary>
    ///  If true, selected value is CategoryName, if false, selected value is CategoryID.
    /// </summary>
    public bool UseCategoryNameForSelection
    {
        get
        {
            return mUseCategoryNameForSelection;
        }
        set
        {
            mUseCategoryNameForSelection = value;
            if (selectCategory != null)
            {
                selectCategory.ReturnColumnName = (value ? "CategoryName" : "CategoryID");
            }
        }
    }


    public bool DisplayPersonalCategories
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayPersonalCategories"), true);
        }
        set
        {
            SetValue("DisplayPersonalCategories", value);

            if (!string.IsNullOrEmpty(selectCategory.DisabledItems))
            {
                selectCategory.DisabledItems = selectCategory.DisabledItems.Replace("personal", "");
            }
            selectCategory.DisabledItems += value ? "" : "personal";
        }
    }


    public bool DisplayGeneralCategories
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayGeneralCategories"), true);
        }
        set
        {
            SetValue("DisplayGeneralCategories", value);

            if (!string.IsNullOrEmpty(selectCategory.DisabledItems))
            {
                selectCategory.DisabledItems = selectCategory.DisabledItems.Replace("globalandsite", "");
            }
            selectCategory.DisabledItems += value ? "" : "globalandsite";
        }
    }


    /// <summary>
    /// Underlying form control, if provided, the form control automatically redirects all properties to that control
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return selectCategory;
        }  
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            selectCategory.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }

    #endregion


    #region "Method"

    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        if (!StopProcessing)
        {
            // Propagate options
            selectCategory.IsLiveSite = IsLiveSite;
            selectCategory.Enabled = Enabled;
            selectCategory.GridName = "~/CMSModules/Categories/Controls/Categories.xml";
            selectCategory.OnAdditionalDataBound += selectCategory_OnAdditionalDataBound;

            // Init disabled items property
            selectCategory.DisabledItems = selectCategory.DisabledItems ?? "";

            if (!string.IsNullOrEmpty(selectCategory.DisabledItems))
            {
                selectCategory.DisabledItems = selectCategory.DisabledItems.Replace("personal", "");
            }
            selectCategory.DisabledItems += DisplayPersonalCategories ? "" : "personal";

            if (!string.IsNullOrEmpty(selectCategory.DisabledItems))
            {
                selectCategory.DisabledItems = selectCategory.DisabledItems.Replace("globalandsite", "");
            }
            selectCategory.DisabledItems += DisplayGeneralCategories ? "" : "globalandsite";


            // Select appropriate dialog window
            selectCategory.SelectItemPageUrl = IsLiveSite ? "~/CMSModules/Categories/CMSPages/LiveCategorySelection.aspx" : "~/CMSModules/Categories/Dialogs/CategorySelection.aspx";
        }
    }


    protected object selectCategory_OnAdditionalDataBound(object sender, string sourceName, object parameter, object value)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Localize category name
            case "name":
                string namePath = parameter as string;
                if (!string.IsNullOrEmpty(namePath))
                {
                    value = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(namePath));
                }

                break;
        }

        return value;
    }

    #endregion
}