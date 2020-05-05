using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Taxonomy;


public partial class CMSModules_Categories_Controls_SelectCategory : FormEngineUserControl
{
    #region "Variables"

    private int mCategoryId;
    private int mUserId;
    private int mSiteId = -1;
    private string mSubItemPrefix = "--";
    private bool dataLoaded;

    private Hashtable disabledCats;

    private GroupedDataSource gds;

    #endregion


    #region "Properties"


    public override object Value
    {
        get
        {
            if (CategoryID > 0)
            {
                return CategoryID;
            }

            return null;
        }
        set
        {
            CategoryID = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Gets or sets the category ID.
    /// </summary>
    public int CategoryID
    {
        get
        {
            return ValidationHelper.GetInteger(drpCategories.SelectedValue, 0);
        }
        set
        {
            mCategoryId = value;
            try
            {
                drpCategories.SelectedValue = mCategoryId.ToString();
            }
            catch
            {
            }
        }
    }


    /// <summary>
    /// Gets or sets the string which will be used as a prefix in order to achieve tree structure.
    /// </summary>
    public string SubItemPrefix
    {
        get
        {
            return mSubItemPrefix;
        }
        set
        {
            mSubItemPrefix = value;
        }
    }


    /// <summary>
    /// Indicates whether (none) should be displayed. Default value is false.
    /// </summary>
    public bool AddRootRecord
    {
        get
        {
            return GetValue("AddRootRecord", false);
        }
        set
        {
            SetValue("AddRootRecord", value);
        }
    }


    /// <summary>
    /// Gets or sets ID of the category which will be the root of the tree structure.
    /// </summary>
    public int RootCategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Category which (with its subtree) should be excluded from the list.
    /// </summary>
    public int ExcludeCategoryID
    {
        get
        {
            return GetValue("ExcludeCategoryID", 0);
        }
        set
        {
            SetValue("ExcludeCategoryID", value);
        }
    }


    /// <summary>
    /// ID of the site to offer categories for. Default value is ID of the current site.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId >= 0)
            {
                return mSiteId;
            }

            if ((Form != null) && (Form.EditedObject != null))
            {
                mSiteId = ((CategoryInfo)Form.EditedObject).CategorySiteID;
            }
            else if (UserID == 0)
            {
                // Don't set site ID for User category
                mSiteId = SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// ID of the user to offer categories for.
    /// </summary>
    public int UserID
    {
        get
        {
            if (Form != null)
            {
                CategoryInfo category = (CategoryInfo)Form.EditedObject;
                return category != null ? category.CategoryUserID : 0;
            }
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Additional where condition.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether site categories are to be disabled.
    /// </summary>
    public bool DisableSiteCategories
    {
        get
        {
            return GetValue("DisableSiteCategories", false);
        }
        set
        {
            SetValue("DisableSiteCategories", value);
        }
    }


    /// <summary>
    /// Indexes of categories, which have to be disabled e.g. '0|1|5|9|'.
    /// </summary>
    protected string DisabledItems
    {
        get
        {
            return ValidationHelper.GetString(ViewState["DisabledItems"], string.Empty);
        }
        set
        {
            ViewState["DisabledItems"] = value;
        }
    }


    /// <summary>
    /// Indicates if disabled categories are allowed in category selector. Default value is false.
    /// </summary>
    public bool AllowDisabledCategories
    {
        get
        {
            return GetValue("AllowDisabledCategories", false);
        }
        set
        {
            SetValue("AllowDisabledCategories", value);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData();
        }
        else
        {
            // Get disabled item indexes and disable them
            string[] indexes = DisabledItems.TrimEnd('|').Split('|');
            DisabledItems = string.Empty;
            int cnt = drpCategories.Items.Count;
            foreach (int i in indexes.Select(index => ValidationHelper.GetInteger(index, -1)).Where(i => (i >= 0) && (i < cnt)))
            {
                DisableItem(drpCategories.Items[i]);
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public void ReloadData(bool force)
    {
        if (!dataLoaded || force)
        {
            DisabledItems = string.Empty;
            disabledCats = new Hashtable();

            int shift = 0;

            drpCategories.Items.Clear();

            string where = GetWhereCondition();

            if (AddRootRecord)
            {
                ListItem item = new ListItem(GetString("general.root"), "0");
                drpCategories.Items.Add(item);
            }

            // Get the data
            DataSet ds = CategoryInfoProvider.GetCategories(where, "CategoryUserID, CategorySiteID, CategoryOrder", 0, "CategoryID, CategoryParentID, CategoryDisplayName, CategoryOrder, CategoryLevel, CategorySiteID, CategoryEnabled", SiteID);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                gds = new GroupedDataSource(ds, "CategoryParentID");

                FillDropDownList(shift, 0);
            }

            // Ensure selected category
            if ((mCategoryId > 0) && (CategoryID != mCategoryId))
            {
                CategoryID = mCategoryId;
            }

            dataLoaded = true;
        }
    }


    /// <summary>
    /// Reloads data if items are not already loaded.
    /// </summary>
    public void ReloadData()
    {
        ReloadData(false);
    }


    /// <summary>
    /// Fills existing categories in the drop down list as a tree structure.
    /// </summary>
    /// <param name="shift">Subcategory offset in the DDL</param>
    /// <param name="parentCategoryId">ID of the parent category</param>
    private void FillDropDownList(int shift, int parentCategoryId)
    {
        List<DataRowView> items = gds.GetGroupView(parentCategoryId);

        if (items != null)
        {
            shift++;

            string prefix = "";
            // Prepare prefix
            for (int i = 0; i < shift; i++)
            {
                prefix += SubItemPrefix;
            }

            foreach (DataRowView dr in items)
            {
                ListItem item = new ListItem();

                // Prepend prefix
                item.Text = prefix;

                int catId = ValidationHelper.GetInteger(dr["CategoryID"], 0);
                int catParentId = ValidationHelper.GetInteger(dr["CategoryParentID"], 0);
                string catDisplayName = ValidationHelper.GetString(dr["CategoryDisplayName"], "");
                bool catIsSite = ValidationHelper.GetInteger(dr["CategorySiteID"], 0) > 0;
                bool catIsEnabled = ValidationHelper.GetBoolean(dr["CategoryEnabled"], true);

                if (!AllowDisabledCategories)
                {
                    // Category stays enabled only if its parent is enabled
                    if (catIsEnabled)
                    {
                        catIsEnabled = !disabledCats.ContainsKey(catParentId);
                    }

                    // Add disabled category to disabled list
                    if (!catIsEnabled && !disabledCats.ContainsKey(catId))
                    {
                        disabledCats.Add(catId, null);
                    }
                }

                item.Text += ResHelper.LocalizeString(catDisplayName);
                item.Value = catId.ToString();

                // Add item to the DLL
                drpCategories.Items.Add(item);

                // Disable unwanted site categories and unwanted disabled categories
                if ((!AllowDisabledCategories && !catIsEnabled) || (DisableSiteCategories && catIsSite))
                {
                    DisableItem(item);
                }

                // Call to add the subitems
                FillDropDownList(shift, catId);
            }
        }
    }


    public override string GetWhereCondition()
    {
        string where = "ISNULL(CategoryUserID, 0) = " + UserID;

        if (ExcludeCategoryID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "CategoryIDPath NOT LIKE (SELECT N'' + CategoryIDPath + '%' FROM CMS_Category WHERE CategoryID = " + ExcludeCategoryID + ")");
        }

        return where;
    }


    /// <summary>
    /// Disables list item and change its color if the current item is not group and selector is in keyEdit mode.
    /// Have to be called after item has been added to drpCategories not before!
    /// </summary>
    /// <param name="item">Item to be disabled.</param>
    private void DisableItem(ListItem item)
    {
        item.Attributes.Add("style", "color:gray");
        item.Attributes.Add("disabled", "disabled");

        DisabledItems += drpCategories.Items.IndexOf(item) + "|";
    }

    #endregion


}