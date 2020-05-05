using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Categories_Controls_CategoryEdit : CMSAdminEditControl
{
    #region "Variables"

    private CategoryInfo mParentCategory;
    private UserInfo mUser;
    private int mUserID;
    private int mSiteId = -1;
    private string mRefreshPageURL;
    private bool? mAllowGlobalCategories;
    private bool canModifySite;
    private bool canModifyGlobal;
    private bool mDisplayOkButton = true;
    private bool mShowEnabled = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets CssClass of the panel wrapping edit form.
    /// </summary>
    public string PanelCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return base.ComponentName;
        }
        set
        {
            headerActions.ComponentName = value;
            base.ComponentName = value;
        }
    }


    /// <summary>
    /// Header actions control
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return headerActions;
        }
    }


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
    /// Current category ID.
    /// </summary>
    public int CategoryID
    {
        get
        {
            if (Category != null)
            {
                return Category.CategoryID;
            }
            return 0;
        }
        set
        {
            pnlContext.UIContext.EditedObject = CategoryInfoProvider.GetCategoryInfo(value);
        }
    }


    /// <summary>
    /// Edited category object.
    /// </summary>
    public CategoryInfo Category
    {
        get
        {
            return (CategoryInfo)pnlContext.UIContext.EditedObject;
        }
        set
        {
            pnlContext.UIContext.EditedObject = value;
        }
    }


    /// <summary>
    /// Parent category ID for creating new category.
    /// </summary>
    public int ParentCategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Parent category ID for creating new category.
    /// </summary>
    public CategoryInfo ParentCategory
    {
        get
        {
            if (mParentCategory == null)
            {
                mParentCategory = CategoryInfoProvider.GetCategoryInfo(ParentCategoryID);
            }

            return mParentCategory;
        }
    }


    /// <summary>
    /// ID of the site to create categories for.
    /// </summary>
    public int SiteID
    {
        get
        {
            if (mSiteId < 0)
            {
                mSiteId = SiteContext.CurrentSiteID;
            }

            return mSiteId;
        }
        set
        {
            mSiteId = value;

            mAllowGlobalCategories = null;
        }
    }


    /// <summary>
    /// ID of the user to create/edit category for.
    /// </summary>
    public int UserID
    {
        get
        {
            if ((Category != null) && (Category.CategoryIsPersonal))
            {
                // Return current category's user ID
                return Category.CategoryUserID;
            }

            return mUserID;
        }
        set
        {
            mUserID = value;
            mUser = null;
        }
    }


    /// <summary>
    /// User object to create/edit category for.
    /// </summary>
    public UserInfo User
    {
        get
        {
            if (mUser == null)
            {
                mUser = UserInfoProvider.GetUserInfo(UserID);
            }

            return mUser;
        }
        set
        {
            mUser = value;
            mUserID = 0;

            if (value != null)
            {
                mUserID = value.UserID;
            }
        }
    }


    /// <summary>
    /// Indicates if global categories are created by default under global parent category.
    /// </summary>
    public bool AllowCreateOnlyGlobal
    {
        get;
        set;
    }


    /// <summary>
    /// The URL of the page where the result is redirected.
    /// </summary>
    public string RefreshPageURL
    {
        get
        {
            return mRefreshPageURL;
        }
        set
        {
            mRefreshPageURL = value;
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
            EnsureChildControls();
            base.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if OK button will be shown.
    /// </summary>
    public bool DisplayOkButton
    {
        get
        {
            return mDisplayOkButton;
        }
        set
        {
            mDisplayOkButton = value;
        }
    }


    /// <summary>
    /// Indicates if Enabled property will be shown.
    /// </summary>
    public bool ShowEnabled
    {
        get
        {
            return mShowEnabled;
        }
        set
        {
            mShowEnabled = value;
        }
    }


    /// <summary>
    /// Indicates if global categories are allowed even when disabled by settings.
    /// </summary>
    public bool AllowGlobalCategories
    {
        get
        {
            if (!mAllowGlobalCategories.HasValue)
            {
                // Get site name
                string siteName = SiteInfoProvider.GetSiteName(SiteID);

                // Figure out from settings
                mAllowGlobalCategories = SettingsKeyInfoProvider.GetBoolValue(siteName + ".CMSAllowGlobalCategories");
            }

            return (bool)mAllowGlobalCategories;
        }
        set
        {
            mAllowGlobalCategories = value;
        }
    }


    /// <summary>
    /// Indicates if disabled categories are allowed in parent category selector. Default value is false.
    /// </summary>
    public bool AllowDisabledParents
    {
        get;
        set;
    }

    /// <summary>
    /// Editing form control.
    /// </summary>
    public UIForm EditingForm
    {
        get
        {
            return editElem;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        editElem.StopProcessing = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing && Visible)
        {
            editElem.OnBeforeValidate += editElem_OnBeforeValidate;
            editElem.OnBeforeSave += editElem_OnBeforeSave;
            editElem.OnAfterValidate += editElem_OnAfterValidate;

            // Initialize the security properties
            InitializeSecurity();

            if (CategoryID > 0)
            {
                LoadData();
            }
            else
            {
                InitializeNewForm();
            }

            editElem.SubmitButton.Visible = DisplayOkButton;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!String.IsNullOrEmpty(PanelCssClass))
        {
            pnlEdit.CssClass = PanelCssClass;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the category data in the control.
    /// </summary>
    public override void ReloadData()
    {
        if (!StopProcessing)
        {
            base.ReloadData();
            headerActions.ReloadData();

            InitializeSecurity();

            if (CategoryID > 0)
            {
                LoadData();
            }
            else
            {
                InitializeNewForm();
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the control.
    /// </summary>
    private void InitializeSecurity()
    {
        // Get and store permissions
        canModifySite = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", "Modify");
        canModifyGlobal = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", "GlobalModify");
    }


    /// <summary>
    /// Loads the data.
    /// </summary>
    private void LoadData()
    {
        // Get information on current category
        if (Category != null)
        {
            if (Category.CategoryIsPersonal)
            {
                editElem.AlternativeFormName = "personal";
            }
            else
            {
                editElem.FieldsToHide.Add("CategorySiteID");
            }

            editElem.EditedObject = Category;
            editElem.ReloadData();
        }
    }


    /// <summary>
    /// Initializes form for new category.
    /// </summary>
    private void InitializeNewForm()
    {
        // Display type selector only if there is a choice (according to permissions and settings) 
        bool canChooseType = canModifyGlobal && canModifySite && AllowGlobalCategories;

        // Assign site if category isn't personal
        EditingForm.ObjectSiteID = (UserID > 0) ? 0 : SiteID;

        bool siteVisible;
        if (ParentCategory != null)
        {
            siteVisible = canChooseType && ParentCategory.CategoryIsGlobal && !ParentCategory.CategoryIsPersonal && !AllowCreateOnlyGlobal;
        }
        else
        {
            siteVisible = canChooseType && (UserID == 0) && !AllowCreateOnlyGlobal;
        }

        if (UserID > 0)
        {
            editElem.AlternativeFormName = "personal";
        }
        else if (!siteVisible)
        {
            editElem.FieldsToHide.Add("CategorySiteID");
        }

        if (!ShowEnabled)
        {
            editElem.FieldsToHide.Add("CategoryEnabled");
        }

        editElem.ReloadData();
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void editElem_OnBeforeSave(object sender, EventArgs e)
    {
        if (editElem.IsInsertMode)
        {
            if (ParentCategoryID > 0)
            {
                editElem.Data["CategoryParentID"] = ParentCategoryID;
            }

            if (UserID > 0)
            {
                editElem.Data["CategoryUserID"] = UserID;
            }
        }
    }


    /// <summary>
    /// OnBeforeValidate event handler.
    /// </summary>
    protected void editElem_OnBeforeValidate(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!RaiseOnCheckPermissions("Modify", this))
        {
            // Check User's Modify permission
            if ((UserID > 0) && (UserID != MembershipContext.AuthenticatedUser.UserID) && (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify")))
            {
                editElem.ShowError(String.Format(GetString("general.permissionresource"), "Modify", "CMS.Users"));
                editElem.StopProcessing = true;
            }
        }

        if (UserID == 0)
        {
            if (editElem.IsInsertMode)
            {
                // Select global type when can not create site category
                if (!canModifySite)
                {
                    editElem.FieldControls["CategorySiteID"].Value = null;
                }

                // Select site type when can not create global category
                if (!canModifyGlobal)
                {
                    editElem.FieldControls["CategorySiteID"].Value = SiteID;
                }
            }

            // Need Modify or GlobalModify permission to edit non-personal categories
            string permission = String.IsNullOrEmpty(editElem.FieldControls["CategorySiteID"].Text) ? "GlobalModify" : "Modify";
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Categories", permission))
            {
                ShowError(String.Format(GetString("general.permissionresource"), permission, "CMS.Categories"));
                editElem.StopProcessing = true;
            }
        }
    }


    /// <summary>
    /// OnAfterValidate event handler.
    /// </summary>
    protected void editElem_OnAfterValidate(object sender, EventArgs e)
    {
        if (editElem.FieldControls["CategoryDisplayName"].Text.IndexOfCSafe('/') >= 0)
        {
            editElem.DisplayErrorLabel("CategoryDisplayName", GetString("category_edit.SlashNotAllowedInDisplayName"));
            editElem.StopProcessing = true;
        }
    }

    #endregion
}