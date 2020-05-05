using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Categories_Controls_MultipleCategoriesSelector : CMSAdminEditControl
{
    #region "Variables"

    private bool mSelectOnlyEnabled = true;
    private int mUserID;
    private bool mEnabled = true;
    private bool isSaved;
    private bool mDisplaySavedMessage = true;
    private string mCurrentValues = "";

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets document node for which the categories should be loaded.
    /// </summary>
    public TreeNode Node
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the UserID whose categories should be displayed.
    /// </summary>
    public int UserID
    {
        get
        {
            if (mUserID > 0)
            {
                return mUserID;
            }
            else
            {
                return MembershipContext.AuthenticatedUser.UserID;
            }
        }
        set
        {
            mUserID = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to display only enabled categories.
    /// </summary>
    public bool SelectOnlyEnabled
    {
        get
        {
            return mSelectOnlyEnabled;
        }
        set
        {
            mSelectOnlyEnabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines if form control mode is On or not.
    /// </summary>
    public bool FormControlMode
    {
        get;
        set;
    }


    public UniSelector UniSelector
    {
        get
        {
            return selectCategory;
        }
    }


    /// <summary>
    /// Enabled state of the control.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
            selectCategory.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates if control has to display its own 'The changes were saved' message.
    /// </summary>
    public bool DisplaySavedMessage
    {
        get
        {
            return mDisplaySavedMessage;
        }
        set
        {
            mDisplaySavedMessage = value;
        }
    }


    /// <summary>
    /// Indicates whether global categories are allowed for selected site.
    /// </summary>
    private bool AllowGlobalCategories
    {
        get
        {
            return SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSAllowGlobalCategories");
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs after data is saved to the database.
    /// </summary>
    public delegate void OnAfterSaveEventHandler();

    /// <summary>
    /// OnAfterSave event.
    /// </summary>
    public event OnAfterSaveEventHandler OnAfterSave;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing && (Node != null))
        {
            // Propagate options
            selectCategory.IsLiveSite = IsLiveSite;
            selectCategory.GridName = "~/CMSModules/Categories/Controls/Categories.xml";
            selectCategory.OnAdditionalDataBound += selectCategory_OnAdditionalDataBound;
            selectCategory.UniGrid.OnAfterRetrieveData += UniGrid_OnAfterRetrieveData;
            selectCategory.ItemsPerPage = 25;

            // Select appropriate dialog window
            selectCategory.SelectItemPageUrl = IsLiveSite ? "~/CMSModules/Categories/CMSPages/LiveCategorySelection.aspx" : "~/CMSModules/Categories/Dialogs/CategorySelection.aspx";

            if (!RequestHelper.IsPostBack())
            {
                ReloadData();
            }

            isSaved = QueryHelper.GetBoolean("saved", false);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (DisplaySavedMessage && !FormControlMode && isSaved)
        {
            // Changes saved
            ShowChangesSaved();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads control data
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        if (Node == null)
        {
            return;
        }

        DataSet ds = DocumentCategoryInfoProvider.GetDocumentCategories(Node.DocumentID)
            .Column("CategoryID")
            .Where(GetWhereCondition())
            .TypedResult;

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            selectCategory.Value = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "CategoryID"));
        }

        if (forceReload)
        {
            selectCategory.Reload(true);
        }
    }


    protected object selectCategory_OnAdditionalDataBound(object sender, string sourceName, object parameter, object value)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Resolve category name
            case "name":
                string namePath = parameter as string;
                if (!String.IsNullOrEmpty(namePath))
                {
                    namePath = namePath.TrimStart('/');
                    namePath = namePath.Replace("/", "&nbsp;&gt;&nbsp;");
                    value = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(namePath));
                }

                break;
        }

        return value;
    }


    private DataSet UniGrid_OnAfterRetrieveData(DataSet ds)
    {
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            return ds;
        }

        DataTable table = ds.Tables[0];
        Dictionary<int, DataRow> toDelete = new Dictionary<int, DataRow>();

        // Remove categories having child in the table
        foreach (DataRow dr in table.Rows)
        {
            string parentNamePath = ValidationHelper.GetString(dr["CategoryNamePath"], "");
            int parentId = ValidationHelper.GetInteger(dr["CategoryID"], 0);

            // Check if table contains any child
            foreach (DataRow drChild in table.Rows)
            {
                string childNamePath = ValidationHelper.GetString(drChild["CategoryNamePath"], "");
                if (toDelete.ContainsKey(parentId) || !childNamePath.StartsWithCSafe(parentNamePath + "/"))
                {
                    continue;
                }

                // Place parent on the black list
                toDelete.Add(parentId, dr);

                string selectedIds = String.Format(";{0};", ValidationHelper.GetString(selectCategory.Value, ""));
                selectCategory.Value = selectedIds.Replace(";" + parentId + ";", ";");

                break;
            }
        }

        // Remove categories from blacklist
        foreach (DataRow row in toDelete.Values)
        {
            row.Delete();
        }

        // Accept changes
        ds.AcceptChanges();

        return ds;
    }


    /// <summary>
    /// Saves the values.
    /// </summary>
    public void Save()
    {
        if (Node == null)
        {
            return;
        }

        if (!RaiseOnCheckPermissions(PERMISSION_MODIFY, this))
        {
            var cui = MembershipContext.AuthenticatedUser;
            if ((cui == null) || ((UserID != cui.UserID) && !cui.IsAuthorizedPerResource("CMS.Users", PERMISSION_MODIFY)))
            {
                RedirectToAccessDenied("CMS.Users", PERMISSION_MODIFY);
            }
        }

        // Prepare selected values
        InitCurrentValues();

        bool logUpdateTask = false;

        // Remove old items
        string newValues = ValidationHelper.GetString(selectCategory.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, mCurrentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to user
            foreach (string item in newItems)
            {
                int categoryId = ValidationHelper.GetInteger(item, 0);
                DocumentCategoryInfoProvider.RemoveDocumentFromCategory(Node.DocumentID, categoryId);
            }
            
            logUpdateTask = true;
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(mCurrentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Add all new items to user
            foreach (string item in newItems)
            {
                int categoryId = ValidationHelper.GetInteger(item, 0);

                // Make sure, that category still exists
                if (CategoryInfoProvider.GetCategoryInfo(categoryId) != null)
                {
                    DocumentCategoryInfoProvider.AddDocumentToCategory(Node.DocumentID, categoryId);
                }
            }

            var catsToRemove = DocumentCategoryInfoProvider.GetDocumentCategories(Node.DocumentID)
                .Column(QueryColumn.FromExpression("CategoryID AS ID"))
                .Where("EXISTS (SELECT CategoryID FROM CMS_Category AS CC WHERE CC.CategoryIDPath LIKE CategoryIDPath + '/%' AND CC.CategoryID IN (SELECT CategoryID FROM CMS_DocumentCategory WHERE DocumentID = " + Node.DocumentID + "))")
                .TypedResult;

            if (!DataHelper.DataSourceIsEmpty(catsToRemove))
            {
                foreach (DataRow dr in catsToRemove.Tables[0].Rows)
                {
                    // Remove categories covered by their children from document
                    DocumentCategoryInfoProvider.RemoveDocumentFromCategory(Node.DocumentID, ValidationHelper.GetInteger(dr["ID"], 0));
                }
            }

            logUpdateTask = true;
        }

        // Raise on after save
        if (OnAfterSave != null)
        {
            OnAfterSave();
        }

        if (logUpdateTask)
        {
            // Log the synchronization if category bindings were changed
            DocumentSynchronizationHelper.LogDocumentChange(Node.NodeSiteName, Node.NodeAliasPath, TaskTypeEnum.UpdateDocument, DocumentManager.Tree);
        }

        isSaved = true;

        // Clear content changed flag, changes are saved directly
        DocumentManager.ClearContentChanged();
    }


    /// <summary>
    /// Initializes the current values
    /// </summary>
    public void InitCurrentValues()
    {
        if (string.IsNullOrEmpty(mCurrentValues) && (Node != null))
        {
            DataSet ds = DocumentCategoryInfoProvider.GetDocumentCategories(Node.DocumentID)
                .Column("CategoryID")
                .Where(GetWhereCondition())
                .TypedResult;

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                mCurrentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "CategoryID"));
            }
        }
    }


    /// <summary>
    /// Creates where condition filtering only categories allowed on current site and for user specified by UserID property.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "CategorySiteID = " + SiteContext.CurrentSiteID;

        if (AllowGlobalCategories)
        {
            where = SqlHelper.AddWhereCondition(where, "CategorySiteID IS NULL AND CategoryUserID IS NULL", "OR");
        }

        if (UserID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "CategoryUserID =" + UserID, "OR");
        }

        return string.Format("({0})", where);
    }

    #endregion
}