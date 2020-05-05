using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Forums_FormControls_ForumSelector : FormEngineUserControl
{
    #region "Constants"

    const string ADHOCFORUM_VALUE = "ad_hoc_forum";

    #endregion


    #region "Variables"

    private int siteId = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return uniSelector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            uniSelector.Enabled = value;
            uniSelector.TextBoxSelect.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();

            // If field type is integer and selected item is "ad-hoc-forum" return int.MinValue
            if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
            {
                if (ValidationHelper.GetString(uniSelector.Value, null) == ADHOCFORUM_VALUE)
                {
                    return int.MinValue;
                }
            }
            return uniSelector.Value;
        }
        set
        {
            EnsureChildControls();

            // If field type is integer and incoming value is in.MinValue or "ad-hoc-forum" preselect "ad-hoc-forum" in selector
            if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
            {
                if ((ValidationHelper.GetInteger(value, 0) == int.MinValue) || (ValidationHelper.GetString(value, null) == ADHOCFORUM_VALUE))
                {
                    value = ADHOCFORUM_VALUE;
                }
            }
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines whether to allow more than one user to select.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            EnsureChildControls();
            return uniSelector.SelectionMode;
        }
        set
        {
            EnsureChildControls();
            uniSelector.SelectionMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'Create adHoc forum' option should be displayed.
    /// </summary>
    public bool DisplayAdHocOption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAdHocOption"), true);
        }
        set
        {
            SetValue("DisplayAdHocOption", value);
        }
    }


    /// <summary>
    /// Gets or sets the sitename. If sitename is defined selector displays forums only from this site. 
    /// If sitename is not defined, selector displays forums for current site
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
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
            EnsureChildControls();
            base.IsLiveSite = value;
            uniSelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'All forums' option should be displayed in dropdown list.
    /// </summary>
    public bool DisplayAllForumsOption
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayAllForumsOption"), false);
        }
        set
        {
            SetValue("DisplayAllForumsOption", value);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to check the AliasPath variable in the URL.
    /// If set to true, use AliasPath variable from URL to decide whether to show or hide the "ad-hoc forum" item in the selector.
    /// </summary>
    public bool CheckAliasPath
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckAliasPath"), false);
        }
        set
        {
            SetValue("CheckAliasPath", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether group forums is included in list.
    /// </summary>
    private bool ShowGroupForums
    {
        get;
        set;
    }


    /// <summary>
    /// Sets the property value of control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    public override bool SetValue(string propertyName, object value)
    {
        // Add special behavior for selection mode
        if (propertyName.ToLowerCSafe() == "selectionmode")
        {
            SelectionMode = (SelectionModeEnum)Enum.Parse(typeof(SelectionModeEnum), Convert.ToString(value));
        }

        return base.SetValue(propertyName, value);
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set uniselector
        uniSelector.DisplayNameFormat = "{%ForumDisplayName%}";
        uniSelector.ReturnColumnName = "ForumName";
        uniSelector.AllowEmpty = false;
        uniSelector.AllowAll = false;

        // Return forum name or ID according to type of field (if no field specified forum name is returned)
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            uniSelector.ReturnColumnName = "ForumID";
            uniSelector.AllowEmpty = true;
            ShowGroupForums = true;
        }

        if (DependsOnAnotherField)
        {
            CheckAliasPath = true;
        }

        if (DisplayAllForumsOption)
        {
            uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("general.selectall"), Value = String.Empty });
        }

        if (ContainsAdHocForum())
        {
            uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("ForumSelector.AdHocForum"), Value = ADHOCFORUM_VALUE });
        }


        // Set resource prefix based on mode
        if ((SelectionMode == SelectionModeEnum.Multiple) || (SelectionMode == SelectionModeEnum.MultipleButton) || (SelectionMode == SelectionModeEnum.MultipleTextBox))
        {
            uniSelector.ResourcePrefix = "forumsselector";
            uniSelector.FilterControl = "~/CMSModules/Forums/Filters/ForumGroupFilter.ascx";
        }
        else
        {
            uniSelector.ResourcePrefix = "forumselector";
        }

        SetupWhereCondition();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (RequestHelper.IsPostBack()
            && DependsOnAnotherField)
        {
            SetupWhereCondition();
            uniSelector.Reload(true);
            pnlUpdate.Update();
        }
    }


    /// <summary>
    /// Creates child controls and loads update panle container if it is required.
    /// </summary>
    protected override void CreateChildControls()
    {
        // If selector is not defined load updat panel container
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        // Call base method
        base.CreateChildControls();
    }


    /// <summary>
    /// Generates a where condition for the uniselector.
    /// </summary>
    private void SetupWhereCondition()
    {
        SetFormSiteName();

        SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteName);
        if (si != null)
        {
            siteId = si.SiteID;
        }
        else
        {
            siteId = SiteContext.CurrentSiteID;
        }

        // Select non group forum of current site
        uniSelector.WhereCondition = "ForumDocumentID IS NULL AND ForumGroupID IN (SELECT GroupID FROM Forums_ForumGroup WHERE " + (!ShowGroupForums ? "GroupGroupID IS NULL AND " : "") + "GroupSiteID = " + siteId + ")";
        uniSelector.SetValue("SiteID", siteId);
    }


    /// <summary>
    /// Sets the site name if the SiteName field is available in the form.
    /// </summary>
    private void SetFormSiteName()
    {
        if (DependsOnAnotherField
            && (Form != null)
            && Form.IsFieldAvailable("SiteName"))
        {
            SiteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");
        }
    }


    /// <summary>
    /// Returns true when the "Ah-hoc forum" item should be displayed.
    /// </summary>
    private bool ContainsAdHocForum()
    {
        string aliasPath = QueryHelper.GetString("AliasPath", null);
        return (!CheckAliasPath) || (!String.IsNullOrEmpty(aliasPath));
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
            // Check if 'ad-hoc-forum" has been selected and modify condition accordingly
            else if (ValidationHelper.GetString(uniSelector.Value, null) == ADHOCFORUM_VALUE)
            {
                return String.Format("[{1}] IN (SELECT ForumID FROM Forums_Forum WHERE ForumSiteID={0} AND ForumName LIKE 'AdHoc-%')", SiteContext.CurrentSiteID, FieldInfo.Name);
            }
        }
        return null;
    }

    #endregion
}