using System;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Layout_PageTemplateFlatSelector : CMSAdminControl
{
    #region "Variables"

    private PageTemplateCategoryInfo mSelectedCategory;
    private string mTreeSelectedItem;
    private int mDocumentID;
    private bool mIsNewPage;
    private bool mMultipleRoots;

    #endregion


    #region "Page template properties"

    /// <summary>
    /// Indicates whether selector supports multiple roots
    /// </summary>
    public bool MultipleRoots
    {
        get
        {
            return mMultipleRoots;
        }
        set
        {
            mMultipleRoots = value;
        }
    }


    /// <summary>
    /// Gets or sets document id.
    /// </summary>
    public int DocumentID
    {
        get
        {
            return mDocumentID;
        }
        set
        {
            mDocumentID = value;
        }
    }


    /// <summary>
    /// Whether selecting new page.
    /// </summary>
    public bool IsNewPage
    {
        get
        {
            return mIsNewPage;
        }
        set
        {
            mIsNewPage = value;
        }
    }


    /// <summary>
    /// Extra where condition on templates
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }

    #endregion


    #region "Flat selector properties"

    /// <summary>
    /// Retruns inner instance of UniFlatSelector control.
    /// </summary>
    public UniFlatSelector UniFlatSelector
    {
        get
        {
            return flatElem;
        }
    }

    /// <summary>
    /// Gets or sets selected item in flat selector.
    /// </summary>
    public string SelectedItem
    {
        get
        {
            return flatElem.SelectedItem;
        }
        set
        {
            flatElem.SelectedItem = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template category.
    /// </summary>
    public PageTemplateCategoryInfo SelectedCategory
    {
        get
        {
            // If not loaded yet
            if (mSelectedCategory == null)
            {
                int categoryId = ValidationHelper.GetInteger(TreeSelectedItem, 0);
                if (categoryId > 0)
                {
                    mSelectedCategory = PageTemplateCategoryInfoProvider.GetPageTemplateCategoryInfo(categoryId);
                }
            }

            return mSelectedCategory;
        }
        set
        {
            mSelectedCategory = value;
            // Update ID
            if (mSelectedCategory != null)
            {
                mTreeSelectedItem = mSelectedCategory.CategoryId.ToString();
            }
        }
    }


    /// <summary>
    /// Gets or sets the selected item in tree, usually the category id.
    /// </summary>
    public string TreeSelectedItem
    {
        get
        {
            return mTreeSelectedItem;
        }
        set
        {
            // Clear loaded category if change
            if (value != mTreeSelectedItem)
            {
                mSelectedCategory = null;
            }
            mTreeSelectedItem = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            flatElem.StopProcessing = value;
            EnableViewState = !value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        ScriptHelper.RegisterJQuery(Page);

        // Setup flat selector
        flatElem.QueryName = "cms.pagetemplate.selectall";
        flatElem.ValueColumn = "PageTemplateID";
        flatElem.SearchLabelResourceString = "pagetemplates.templatename";
        flatElem.SearchColumn = "PageTemplateDisplayName";
        flatElem.SelectedColumns = "PageTemplateCodeName, PageTemplateThumbnailGUID, PageTemplateIconClass, PageTemplateDisplayName, PageTemplateID";
        flatElem.OrderBy = "PageTemplateDisplayName";
        flatElem.NotAvailableImageUrl = GetImageUrl("Objects/CMS_PageTemplate/notavailable.png");
        flatElem.NoRecordsMessage = "pagetemplates.norecordsincategory";
        flatElem.NoRecordsSearchMessage = "pagetemplates.norecords";

        flatElem.OnItemSelected += flatElem_OnItemSelected;
    }


    /// <summary>
    /// On PreRender.
    /// </summary>    
    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        string where = flatElem.WhereCondition;

        // Do not display dashboard items
        where = SqlHelper.AddWhereCondition(where, "PageTemplateType IS NULL OR PageTemplateType <> N'" + PageTemplateInfoProvider.GetPageTemplateTypeCode(PageTemplateTypeEnum.Dashboard) + "'");

        // Restrict to items in selected category (if not root) - For multiple roots filter items for every root
        if ((SelectedCategory != null) && ((SelectedCategory.ParentId > 0) || MultipleRoots))
        {
            string cat = SqlHelper.EscapeLikeText(SqlHelper.EscapeQuotes(SelectedCategory.CategoryPath));
            if (!cat.EndsWith("/", StringComparison.Ordinal)) 
            {
                cat += "/";
            }

            cat += "%";

            where = SqlHelper.AddWhereCondition(where, "PageTemplateCategoryID = " + SelectedCategory.CategoryId + " OR PageTemplateCategoryID IN (SELECT CategoryID FROM CMS_PageTemplateCategory WHERE CategoryPath LIKE N'" + cat + "')");
        }

        // Add extra where condition
        where = SqlHelper.AddWhereCondition(where, WhereCondition);

        TreeProvider tp = new TreeProvider(MembershipContext.AuthenticatedUser);
        TreeNode node = DocumentHelper.GetDocument(DocumentID, tp);

        string culture = LocalizationContext.PreferredCultureCode;

        if (node != null)
        {
            int level = node.NodeLevel;
            string path = node.NodeAliasPath;

            if (IsNewPage)
            {
                level++;
                path = path + "/%";
            }
            else
            {
                culture = node.DocumentCulture;
            }

            string className = node.NodeClassName;

            // Check if class id is in query string - then use it's value instead of document class name 
            int classID = QueryHelper.GetInteger("classid", 0);
            if (classID != 0)
            {
                DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classID);
                if (dci != null)
                {
                    className = dci.ClassName;
                }
            }
        }

        flatElem.WhereCondition = where;

        // Description area
        ltrDescription.Text = ShowInDescriptionArea(SelectedItem);

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handling"

    /// <summary>
    /// Updates description after item is selected in flat selector.
    /// </summary>
    protected string flatElem_OnItemSelected(string selectedValue)
    {
        return ShowInDescriptionArea(selectedValue);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        flatElem.ReloadData();
        flatElem.ResetToDefault();
        pnlUpdate.Update();
    }


    /// <summary>
    /// Generates HTML text to be used in description area.
    /// </summary>
    ///<param name="selectedValue">Selected item for which generate description</param>
    private string ShowInDescriptionArea(string selectedValue)
    {
        string description = String.Empty;

        if (!String.IsNullOrEmpty(selectedValue))
        {
            int templateId = ValidationHelper.GetInteger(selectedValue, 0);

            // Get the template data
            DataSet ds = PageTemplateInfoProvider.GetTemplates()
                .WhereEquals("PageTemplateID", templateId)
                .Columns("PageTemplateDisplayName", "PageTemplateDescription", "PageTemplateType");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                DataRow dr = ds.Tables[0].Rows[0];

                description = ResHelper.LocalizeString(ValidationHelper.GetString(dr["PageTemplateDescription"], ""));
            }
        }

        if (!String.IsNullOrEmpty(description))
        {
            return "<div class=\"Description\">" + HTMLHelper.HTMLEncode(description) + "</div>";
        }

        return String.Empty;
    }


    /// <summary>
    /// Add a reload script to the page which will update the page size (items count) according to the window size.
    /// </summary>
    /// <param name="forceResize">Indicates whether to invoke resizing of the page before calculating the items count</param>
    public void RegisterRefreshPageSizeScript(bool forceResize)
    {
        flatElem.RegisterRefreshPageSizeScript(forceResize);
    }

    #endregion
}
