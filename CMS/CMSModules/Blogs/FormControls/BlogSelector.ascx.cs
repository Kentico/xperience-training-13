using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Blogs;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Blogs_FormControls_BlogSelector : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets display name from uniselector.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return UniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets return column name.
    /// </summary>
    public string ReturnColumnName
    {
        get
        {
            return GetValue("ReturnColumnName", "NodeAliasPath");
        }
        set
        {
            SetValue("ReturnColumnName", value);
        }
    }


    /// <summary>
    /// Gets or sets whether show only user's blogs.
    /// </summary>
    public bool OnlyUsersBlogs
    {
        get
        {
            return GetValue("OnlyUsersBlogs", true);
        }
        set
        {
            SetValue("OnlyUsersBlogs", value);
        }
    }


    /// <summary>
    /// Gets or sets selected items.
    /// </summary>
    public override object Value
    {
        get
        {
            if (ReturnColumnName == "NodeAliasPath")
            {
                string aliasPath = ValidationHelper.GetString(UniSelector.Value, String.Empty);

                if (!String.IsNullOrEmpty(aliasPath) && !aliasPath.EndsWithCSafe("/%"))
                {
                    aliasPath = aliasPath.TrimEnd('/') + "/%";
                }

                return aliasPath;
            }
            else
            {
                return UniSelector.Value;
            }
        }
        set
        {
            if (ReturnColumnName == "NodeAliasPath")
            {
                string aliasPath = ValidationHelper.GetString(value, String.Empty);

                UniSelector.Value = (aliasPath == "/%") ? "/" : aliasPath;
            }
            else
            {
                UniSelector.Value = value;
            }
        }
    }


    /// <summary>
    /// Indicates if (my blogs) and (all) option should be visible or only (select blog) is visible. 
    /// </summary>
    public bool UseAlternativeMode
    {
        get
        {
            return GetValue("UseAlternativeMode", false);
        }
        set
        {
            SetValue("UseAlternativeMode", value);
        }
    }


    /// <summary>
    /// Gets the inner blogSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return blogSelector;
        }
    }


    /// <summary>
    /// Gets the single select drop down field.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return UniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Gets UpdatePanel of selector.
    /// </summary>
    public CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUpdate;
        }
    }


    /// <summary>
    /// Enables or disables site selector dropdown.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            DropDownSingleSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Enables or disables live site mode of selection dialog.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return UniSelector.IsLiveSite;
        }
        set
        {
            UniSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets the underlying form control.
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return blogSelector;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            blogSelector.StopProcessing = true;
            return;
        }

        blogSelector.ObjectType = BlogHelper.BLOG_OBJECT_TYPE;

        if (UseAlternativeMode)
        {
            blogSelector.Enabled = !String.IsNullOrEmpty(GetSiteName());
        }
    }


    protected void blogSelector_OnSpecialFieldsLoaded(object sender, EventArgs e)
    {
        if (UseAlternativeMode)
        {
            blogSelector.SpecialFields.Add(new SpecialField { Text = GetString("blogselector.myblogs"), Value = "##myblogs##" });
            
            if (MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                blogSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.selectall"), Value = "##all##" });
            }
        }
        else
        {
            blogSelector.SpecialFields.Add(new SpecialField() { Text = GetString("blogselector.selectblog"), Value = String.Empty });
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        string siteName = GetSiteName();

        // Set selector
        blogSelector.ReturnColumnName = ReturnColumnName;
        blogSelector.IsLiveSite = IsLiveSite;
        blogSelector.WhereCondition = UpdateWhereCondition(siteName);

        try
        {
            Reload(true);
        }
        catch (Exception ex)
        {
            blogSelector.StopProcessing = true;
            ShowError("BlogSelector: " + GetString("report_error.executequery"), ex.ToString());
        }
    }


    /// <summary>
    /// OnPreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Select (all) option for disabled selector
        if (!blogSelector.Enabled)
        {
            blogSelector.DropDownSingleSelect.SelectedValue = "##all##";
        }
        else
        {
            ReloadData();
        }
    }

    /// <summary>
    /// Updates uni selector where condition based on current properties values.
    /// </summary>
    public string UpdateWhereCondition(string siteName)
    {
        string where = String.Empty;

        if (OnlyUsersBlogs)
        {
            where = "NodeOwner=" + MembershipContext.AuthenticatedUser.UserID;
        }

        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo("cms.blog");
        if (dci != null)
        {
            where = SqlHelper.AddWhereCondition(where, "NodeClassID=" + dci.ClassID);
        }

        if (!String.IsNullOrEmpty(siteName))
        {
            where = SqlHelper.AddWhereCondition(where, "NodeSiteID=" + SiteInfoProvider.GetSiteID(siteName));
        }

        return where;
    }


    /// <summary>
    /// Reloads all controls.
    /// </summary>
    /// <param name="forceReload">Indicates if data should be loaded from DB</param>
    public void Reload(bool forceReload)
    {
        blogSelector.Reload(forceReload);
    }


    /// <summary>
    /// Retrieves a site name from the form, if applicable, and returns it.
    /// </summary>
    /// <returns>A site name from the form, if applicable; otherwise, the current site name.</returns>
    private string GetSiteName()
    {
        if (DependsOnAnotherField && (Form != null) && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), String.Empty);

            // Resolve special values
            if (String.IsNullOrEmpty(siteName) || siteName.ToLowerCSafe().EqualsCSafe("##all##", true))
            {
                return String.Empty;
            }
            else if (siteName.EqualsCSafe("##currentsite##", true))
            {
                return SiteContext.CurrentSiteName;
            }

            return siteName;
        }

        return SiteContext.CurrentSiteName;
    }

    #endregion
}