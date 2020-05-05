using System;
using System.Text;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Modules_Controls_Settings_Category_SettingsCategoryEdit : CMSAdminEditControl
{
    #region "Private Members"

    private SettingsCategoryInfo mSettingsCategoryObj;
    private int mSettingsCategoryId;
    private bool mIncludeRootCategory;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates selected category
    /// </summary>
    private int ModuleID
    {
        get
        {
            return QueryHelper.GetInteger("moduleid", 0);
        }
    }


    /// <summary>
    /// Gets or sets Category order. Specifies order of the category.
    /// </summary>
    public int CategoryOrder
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets RootCategoryID. Specifies SettingsCategory which should be set up as the root of the SettingsCategorySelector. 
    /// </summary>
    public int RootCategoryID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets current selected parent category or RootCategoryID for new category.
    /// </summary>
    public int SelectedParentCategory
    {
        get
        {
            return (SettingsCategoryObj != null) ? drpCategory.SelectedCategory : RootCategoryID;
        }
    }


    /// <summary>
    /// Gets or sets enabled state of inclusion of RootCategory. Default false.
    /// </summary>
    public bool IncludeRootCategory
    {
        get
        {
            if (SettingsCategoryObj == null)
            {
                return true;
            }
            return mIncludeRootCategory;
        }
        set
        {
            mIncludeRootCategory = value;
        }
    }


    /// <summary>
    /// Gets or sets SettingsCategoryID. Specifies Id of SettingsCategory object.
    /// </summary>
    public int SettingsCategoryID
    {
        get
        {
            return mSettingsCategoryId;
        }
        set
        {
            mSettingsCategoryId = value;
            drpCategory.CurrentCategoryId = value;
            mSettingsCategoryObj = null;
        }
    }


    /// <summary>
    /// Gets or sets SettingsCategory object. Specifies SettingsCategory object which should be edited.
    /// </summary>
    public SettingsCategoryInfo SettingsCategoryObj
    {
        get
        {
            return mSettingsCategoryObj ?? (mSettingsCategoryObj = (mSettingsCategoryId > 0) ? SettingsCategoryInfoProvider.GetSettingsCategoryInfo(mSettingsCategoryId) : null);
        }
        set
        {
            mSettingsCategoryObj = value;
            if (value != null)
            {
                mSettingsCategoryId = value.CategoryID;
                CategoryOrder = value.CategoryOrder;
            }
            else
            {
                mSettingsCategoryId = 0;
                CategoryOrder = 0;
            }
        }
    }


    /// <summary>
    /// Gets or sets visible state of parent category selector.
    /// </summary>
    public bool ShowParentSelector
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Gets or sets IsGroupEdit property. If set to <c>true</c> parent category selector will be hidden.
    /// </summary>
    public bool IsGroupEdit
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Enabled property. If set to <c>false</c> all child control are disabled.
    /// </summary>
    public bool Enabled
    {
        get;
        set;
    } = true;


    /// <summary>
    /// Gets or sets DisplayOnlyCategories property of SelectSettingsCategory drop down list. If set to false, groups wil be included.
    /// </summary>
    public bool DisplayOnlyCategories
    {
        get
        {
            return drpCategory.DisplayOnlyCategories;
        }
        set
        {
            drpCategory.DisplayOnlyCategories = value;
        }
    }


    /// <summary>
    /// Url for refreshing tree with settings.
    /// </summary>
    public string TreeRefreshUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Url for refreshing page with categories.
    /// </summary>
    public string ContentRefreshUrl
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        InitControls();

        // Load the form data
        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }

        // Set edited object
        EditedObject = (SettingsCategoryID > 0) ? SettingsCategoryObj : new SettingsCategoryInfo();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        txtCategoryDisplayName.Enabled =
            rfvCategoryDisplayName.Enabled =
            txtCategoryName.Enabled =
            rfvCategoryName.Enabled =
            txtIconPath.Enabled =
            drpCategory.Enabled =
            btnOk.Enabled = Enabled;

        // Set CSS for the button
        btnOk.ButtonStyle = ButtonStyle.Primary;

        // Display parent category selector
        trParentCategory.Visible = trParentCategory.Visible && ShowParentSelector;
        trIconPath.Visible = !IsGroupEdit;
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool IsValid()
    {
        // Validate required fields
        string errMsg = new Validator().NotEmpty(txtCategoryName.Text.Trim(), GetString("General.RequiresCodeName"))
            .NotEmpty(txtCategoryDisplayName.Text.Trim(), GetString("General.RequiresDisplayName"))
            .IsCodeName(txtCategoryName.Text.Trim(), GetString("General.ErrorCodeNameInIdentifierFormat"))
            .Result;

        // Set up error message
        if (!string.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);

            return false;
        }

        return true;
    }


    /// <summary>
    /// Initialization of controls.
    /// </summary>
    private void InitControls()
    {
        // Init validators
        rfvCategoryDisplayName.ErrorMessage = ResHelper.GetString("general.requiresdisplayname");
        rfvCategoryName.ErrorMessage = ResHelper.GetString("general.requirescodename");

        int moduleId = ModuleID;
        // Disable edit for category which is not for selected module
        if (SettingsCategoryObj != null)
        {
            ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(moduleId);
            if ((((resource != null) && !resource.ResourceIsInDevelopment) || (SettingsCategoryObj.CategoryResourceID != moduleId)) && !SystemContext.DevelopmentMode)
            {
                Enabled = false;
                HeaderActions.Enabled = false;
            }
        }

        if (SystemContext.DevelopmentMode)
        {
            ucSelectModule.Value = (SettingsCategoryObj == null) ? moduleId : (!RequestHelper.IsPostBack()) ? SettingsCategoryObj.CategoryResourceID : ucSelectModule.Value;
        }
        pnlDevelopmentMode.Visible = SystemContext.DevelopmentMode;

        // Set the root category
        if (RootCategoryID > 0)
        {
            drpCategory.RootCategoryId = RootCategoryID;
            drpCategory.IncludeRootCategory = IncludeRootCategory;
        }
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    private void LoadData()
    {
        // Load the form from the Info object
        if (SettingsCategoryObj != null)
        {
            txtCategoryName.Text = SettingsCategoryObj.CategoryName;
            txtCategoryDisplayName.Text = SettingsCategoryObj.CategoryDisplayName;
            txtIconPath.Text = SettingsCategoryObj.CategoryIconPath;
            if (SettingsCategoryObj.CategoryParentID > 0)
            {
                drpCategory.SelectedCategory = SettingsCategoryObj.CategoryParentID;
            }
        }
        else
        {
            trParentCategory.Visible = false;
        }
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles OnClick event of btnOk.
    /// </summary>
    /// <param name="sender">Asp Button instance</param>
    /// <param name="e">EventArgs instance</param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (IsValid())
        {
            // Get category by name
            SettingsCategoryInfo categoryObj = SettingsCategoryInfoProvider.GetSettingsCategoryInfoByName(txtCategoryName.Text.Trim());
            // If name is unique OR ids are same
            if ((categoryObj == null) || (categoryObj.CategoryID == SettingsCategoryID))
            {
                SettingsCategoryInfo sci = SettingsCategoryObj;
                if (sci == null)
                {
                    sci = new SettingsCategoryInfo();
                    sci.CategoryOrder = CategoryOrder;
                }

                if (sci.CategoryParentID != drpCategory.SelectedCategory)
                {
                    // When parent has been changed set the order for the category as the last possible order
                    sci.CategoryOrder = SettingsCategoryInfoProvider.GetLastSettingsCategoryOrder(drpCategory.SelectedCategory) + 1;
                }

                sci.CategoryName = txtCategoryName.Text.Trim();
                sci.CategoryDisplayName = txtCategoryDisplayName.Text.Trim();
                sci.CategoryIconPath = txtIconPath.Text.Trim();
                sci.CategoryParentID = SelectedParentCategory;
                sci.CategoryIsGroup = IsGroupEdit;
                sci.CategoryResourceID = ValidationHelper.GetInteger(ucSelectModule.Value, ModuleID);

                SettingsCategoryInfoProvider.SetSettingsCategoryInfo(sci);
                SettingsCategoryObj = sci;
                RaiseOnSaved();

                // Set the info message
                if (ContentRefreshUrl == null)
                {
                    ShowChangesSaved();
                }

                // Reload header and content after save
                int categoryIdToShow = sci.CategoryIsGroup ? sci.CategoryParentID : sci.CategoryID;

                StringBuilder sb = new StringBuilder();
                sb.Append("if (window.parent != null) {");
                if (!String.IsNullOrEmpty(TreeRefreshUrl))
                {
                    sb.AppendFormat(@"if (window.parent.parent.frames['settingstree'] != null) {{
   window.parent.parent.frames['settingstree'].location = '{0}&categoryid={1}';
}}
if (window.parent.frames['settingstree'] != null) {{
   window.parent.frames['settingstree'].location = '{0}&categoryid={1}';
}}", ResolveUrl(TreeRefreshUrl), categoryIdToShow);
                }
                if (!String.IsNullOrEmpty(ContentRefreshUrl))
                {
                    sb.AppendFormat("window.location = '{0}&categoryid={1}';", ResolveUrl(ContentRefreshUrl), sci.CategoryID);
                }

                sb.Append("}");
                ltlScript.Text = ScriptHelper.GetScript(sb.ToString());
            }
            else
            {
                ShowError(GetString("general.codenameexists"));
            }
        }
    }

    #endregion
}
