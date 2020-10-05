using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_Widgets_Controls_WidgetFlatSelector : CMSAdminControl
{
    #region "Variables"

    private string mTreeSelectedItem = null;
    private WidgetCategoryInfo mSelectedCategory = null;

    #endregion


    #region "Flat selector properties"

    /// <summary>
    /// Returns inner instance of UniFlatSelector control.
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
    /// Gets or sets the current widget category.
    /// </summary>
    public WidgetCategoryInfo SelectedCategory
    {
        get
        {
            // If not loaded yet
            if (mSelectedCategory == null)
            {
                int categoryId = ValidationHelper.GetInteger(TreeSelectedItem, 0);
                if (categoryId > 0)
                {
                    mSelectedCategory = WidgetCategoryInfoProvider.GetWidgetCategoryInfo(categoryId);
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
                mTreeSelectedItem = mSelectedCategory.WidgetCategoryID.ToString();
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

        // Setup flat selector
        flatElem.QueryName = "cms.widget.selectall";
        flatElem.ValueColumn = "WidgetID";
        flatElem.SearchLabelResourceString = "widget.widgetname";
        flatElem.SearchColumn = "WidgetDisplayName";
        flatElem.SelectedColumns = "WidgetName, WidgetThumbnailGUID, WidgetIconClass, WidgetDisplayName, WidgetID, WidgetSkipInsertProperties";
        flatElem.SkipPropertiesDialogColumn = "WidgetSkipInsertProperties";
        flatElem.PageSize = 15;
        flatElem.OrderBy = "WidgetDisplayName";
        flatElem.NoRecordsMessage = "widgets.norecordsincategory";
        flatElem.NoRecordsSearchMessage = "widgets.norecords";

        flatElem.OnItemSelected += new UniFlatSelector.ItemSelectedEventHandler(flatElem_OnItemSelected);

        flatElem.SearchCheckBox.Visible = true;
        flatElem.SearchCheckBox.Text = GetString("webparts.searchindescription");

        if (!RequestHelper.IsPostBack())
        {
            // Search in description default value
            flatElem.SearchCheckBox.Checked = false;
        }

        if (flatElem.SearchCheckBox.Checked)
        {
            flatElem.SearchColumn += ";WidgetDescription";
        }
    }


    /// <summary>
    /// Creates authorization and authentication where condition.
    /// </summary>
    private string CreateAuthWhereCondition()
    {
        var currentUser = MembershipContext.AuthenticatedUser;

        // Allowed for all  
        string securityWhere = "((WidgetSecurity = 0) ";

        if (AuthenticationHelper.IsAuthenticated())
        {
            // Authenticated
            securityWhere += " OR (WidgetSecurity = 1)";

            // Global admin
            if (currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                securityWhere += " OR (WidgetSecurity = 7)";
            }

            // Authorized roles
            securityWhere += " OR ((WidgetSecurity = 2)";
            if (!currentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                securityWhere += "AND (WidgetID IN ( SELECT WidgetID FROM CMS_WidgetRole WHERE RoleID IN (SELECT RoleID FROM View_CMS_UserRole_MembershipRole_ValidOnly_Joined WHERE UserID = " + currentUser.UserID + ")))))";
            }
            else
            {
                securityWhere += "))";
            }
        }
        else
        {
            securityWhere += ")";
        }
        return securityWhere;
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

        // Create security where condition
        string securityWhere = CreateAuthWhereCondition();
        flatElem.WhereCondition = SqlHelper.AddWhereCondition(flatElem.WhereCondition, securityWhere);

        // Restrict to items in selected category (if not root)
        if ((SelectedCategory != null) && (SelectedCategory.WidgetCategoryParentID > 0))
        {
            flatElem.WhereCondition = SqlHelper.AddWhereCondition(flatElem.WhereCondition, "WidgetCategoryID = " + SelectedCategory.WidgetCategoryID + " OR WidgetCategoryID IN (SELECT WidgetCategoryID FROM CMS_WidgetCategory WHERE WidgetCategoryPath LIKE '" + SelectedCategory.WidgetCategoryPath + "/%')");
        }

        // Description area and recently used
        litCategory.Text = ShowInDescriptionArea(SelectedItem);

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
            int widgetId = ValidationHelper.GetInteger(selectedValue, 0);
            WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetId);
            if (wi != null)
            {
                description = wi.WidgetDescription;
            }
        }

        if (!String.IsNullOrEmpty(description))
        {
            return "<div class=\"Description\">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(description)) + "</div>";
        }

        return String.Empty;
    }

    #endregion
}