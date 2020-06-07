using System;
using System.Text;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Taxonomy;

public partial class CMSWebParts_TaggingCategories_CategoryBreadcrumbs : CMSAbstractWebPart
{
    #region "Variables"

    private CategoryInfo mCategory;
    private CategoryInfo mStartingCategoryObj;

    #endregion


    #region "Properties"

    /// <summary>
    /// Breadcrumbs root
    /// </summary>
    public string BreadcrumbsRoot
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbsRoot"), "");
        }
        set
        {
            SetValue("BreadcrumbsRoot", value);
        }
    }


    /// <summary>
    /// Breadcrumbs separator.
    /// </summary>
    public string BreadcrumbsSeparator
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbsSeparator"), "/");
        }
        set
        {
            SetValue("BreadcrumbsSeparator", value);
        }
    }


    /// <summary>
    /// Breadcrumbs separator RTL.
    /// </summary>
    public string BreadcrumbsSeparatorRTL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbsSeparatorRTL"), "\\");
        }
        set
        {
            SetValue("BreadcrumbsSeparatorRTL", value);
        }
    }


    /// <summary>
    /// Render link title.
    /// </summary>
    public bool RenderLinkTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RenderLinkTitle"), false);
        }
        set
        {
            SetValue("RenderLinkTitle", value);
        }
    }


    /// <summary>
    /// Categories page target.
    /// </summary>
    public string CategoriesPageTarget
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesPageTarget"), "");
        }
        set
        {
            SetValue("CategoriesPageTarget", value);
        }
    }


    /// <summary>
    /// Starting category.
    /// </summary>
    public string BreadcrumbsStartingCategory
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbsStartingCategory"), "");
        }
        set
        {
            SetValue("BreadcrumbsStartingCategory", value);
        }
    }


    /// <summary>
    /// Breadcrumb content before.
    /// </summary>
    public string BreadcrumbContentBefore
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbContentBefore"), "");
        }
        set
        {
            SetValue("BreadcrumbContentBefore", value);
        }
    }


    /// <summary>
    /// Breadcrumb content after.
    /// </summary>
    public string BreadcrumbContentAfter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BreadcrumbContentAfter"), "");
        }
        set
        {
            SetValue("BreadcrumbContentAfter", value);
        }
    }


    /// <summary>
    /// Show current item.
    /// </summary>
    public bool ShowCurrentItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrentItem"), true);
        }
        set
        {
            SetValue("ShowCurrentItem", value);
        }
    }


    /// <summary>
    /// Show current item as link.
    /// </summary>
    public bool ShowCurrentItemAsLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowCurrentItemAsLink"), false);
        }
        set
        {
            SetValue("ShowCurrentItemAsLink", value);
        }
    }


    /// <summary>
    /// Indicates if control will be hidden when no category found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideControlForZeroRows"), false);
        }
        set
        {
            SetValue("HideControlForZeroRows", value);
        }
    }


    /// <summary>
    /// Categories page path.
    /// </summary>
    public string CategoriesPagePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CategoriesPagePath"), "");
        }
        set
        {
            SetValue("CategoriesPagePath", value);
        }
    }


    /// <summary>
    /// Indicates if category code name will be used in query parameter. Category ID is used by default.
    /// </summary>
    public bool UseCodeNameInQuery
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseCodeNameInQuery"), false);
        }
        set
        {
            SetValue("UseCodeNameInQuery", value);
        }
    }


    /// <summary>
    /// Current category info object.
    /// </summary>
    public CategoryInfo Category
    {
        get
        {
            if (mCategory == null)
            {
                mCategory = TaxonomyContext.CurrentCategory;
            }

            return mCategory;
        }
        set
        {
            mCategory = value;
        }
    }


    /// <summary>
    /// Starting category info object.
    /// </summary>
    private CategoryInfo StartingCategoryObj
    {
        get
        {
            if (mStartingCategoryObj == null)
            {
                mStartingCategoryObj = CategoryInfo.Provider.Get(BreadcrumbsStartingCategory, SiteContext.CurrentSiteID);
            }

            return mStartingCategoryObj;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            bool categoryOk = CheckCurrentCategory();

            // Hide control when category check failed and hiding is enabled
            Visible = categoryOk || !HideControlForZeroRows;

            if (categoryOk)
            {
                // Get processed category paths
                string idPath = Category.CategoryIDPath;
                string namePath = Category.CategoryNamePath;

                // Handle custom starting category
                if (!string.IsNullOrEmpty(BreadcrumbsStartingCategory) && (StartingCategoryObj != null))
                {
                    // Check if category from other parent selected
                    if (!idPath.StartsWithCSafe(StartingCategoryObj.CategoryIDPath) || !namePath.StartsWithCSafe(StartingCategoryObj.CategoryNamePath))
                    {
                        return;
                    }

                    // Shorten paths
                    namePath = namePath.Substring(StartingCategoryObj.CategoryNamePath.Length);
                    idPath = idPath.Replace(StartingCategoryObj.CategoryIDPath, "");
                }

                ltlBreadcrumbs.Text = CreateCategoryLine(idPath, namePath);
            }
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    /// <summary>
    /// Checks if current category exists and if can be used on current site by current user.
    /// </summary>
    protected bool CheckCurrentCategory()
    {
        if (Category != null)
        {
            if (Category.CategoryIsPersonal)
            {
                // Check if personal category belongs to current user.
                if ((MembershipContext.AuthenticatedUser != null) && (Category.CategoryUserID == MembershipContext.AuthenticatedUser.UserID))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Creates html entry for one category.
    /// </summary>
    /// <param name="categoryIdPath">ID path of rendered category.</param>
    /// <param name="categoryNamePath">Name path of rendered category.</param>
    protected string CreateCategoryLine(string categoryIdPath, string categoryNamePath)
    {
        // Split paths
        string[] idSplits = categoryIdPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        string[] nameSplits = categoryNamePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        bool separator = false;

        // Need rtl rendering
        bool jrtl = false;

        // Set rtl rendering
        jrtl = CultureHelper.IsPreferredCultureRTL();

        StringBuilder line = new StringBuilder();

        int count = idSplits.Length;

        count -= ShowCurrentItem ? 0 : 1;

        if ((count <= nameSplits.Length) && (count > 0))
        {
            // Convert string to ints
            int[] intIdSplits = ValidationHelper.GetIntegers(idSplits, -1);

            // Append prefix if any
            if (!string.IsNullOrEmpty(BreadcrumbsRoot))
            {
                line.Append(CreateRoot());
                separator = true;
            }

            // Append whole category path
            for (int i = 0; i < count; i++)
            {
                // Do not append the separator at the beginning
                if (separator)
                {
                    line.Append(jrtl ? BreadcrumbsSeparatorRTL : BreadcrumbsSeparator);
                }

                // Don't create link for current category item (last part) when disabled
                if ((i == count - 1) && ShowCurrentItem && !ShowCurrentItemAsLink)
                {
                    // Append display name part as text
                    line.Append(FormatCategoryDisplayName(nameSplits[i], true));
                }
                else
                {
                    // Append display name part as link
                    line.Append(CreateCategoryPartLink(intIdSplits[i]));
                }

                separator = true;
            }
        }

        return line.ToString();
    }


    /// <summary>
    /// Creates HTML code for category link.
    /// </summary>
    protected string CreateRoot()
    {
        // Get target url
        string url = RequestContext.CurrentURL;

        StringBuilder attrs = new StringBuilder();

        // Append target attribute
        if (!string.IsNullOrEmpty(CategoriesPageTarget))
        {
            attrs.Append(" target=\"").Append(CategoriesPageTarget).Append("\"");
        }

        return string.Format("<a href=\"{0}\"{1}>{2}</a>", HTMLHelper.EncodeForHtmlAttribute(url), attrs, FormatCategoryDisplayName(BreadcrumbsRoot, true));
    }


    /// <summary>
    /// Creates HTML code for category link.
    /// </summary>
    /// <param name="categoryId">ID of the category.</param>
    protected string CreateCategoryPartLink(int categoryId)
    {
        // Get category
        CategoryInfo category = CategoryInfo.Provider.Get(categoryId);
        if (category != null)
        {
            string categoryDisplayName = category.CategoryDisplayName;

            // Get target url
            string url = RequestContext.CurrentURL;

            // Append category parameter
            if (UseCodeNameInQuery)
            {
                url = URLHelper.AddParameterToUrl(url, "categoryname", category.CategoryName);
            }
            else
            {
                url = URLHelper.AddParameterToUrl(url, "categoryid", categoryId.ToString());
            }

            StringBuilder attrs = new StringBuilder();

            // Append target attribute
            if (!string.IsNullOrEmpty(CategoriesPageTarget))
            {
                attrs.Append(" target=\"").Append(CategoriesPageTarget).Append("\"");
            }

            // Append title attribute
            if (RenderLinkTitle)
            {
                // Encode category name
                attrs.Append(" title=\"").Append(HTMLHelper.HTMLEncode(categoryDisplayName)).Append("\"");
            }

            return string.Format("<a href=\"{0}\"{1}>{2}</a>", HTMLHelper.EncodeForHtmlAttribute(url), attrs, FormatCategoryDisplayName(categoryDisplayName, true));
        }

        return "";
    }


    /// <summary>
    /// Formats category display name. Applies CategoryContentBefore and CategoryContentAfter.
    /// </summary>
    /// <param name="categoryDisplayName">Category display name.</param>
    /// <param name="encode">Indicates whether category name will be encoded.</param>
    protected string FormatCategoryDisplayName(string categoryDisplayName, bool encode)
    {
        // Localize category display name
        categoryDisplayName = ResHelper.LocalizeString(categoryDisplayName);

        if (encode)
        {
            // Encode category display name
            categoryDisplayName = HTMLHelper.HTMLEncode(categoryDisplayName);
        }

        // Format name
        return string.Format("{0}{1}{2}", BreadcrumbContentBefore, categoryDisplayName, BreadcrumbContentAfter);
    }

    #endregion
}