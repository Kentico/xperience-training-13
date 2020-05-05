using System;
using System.Linq;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_DepartmentSelector : SiteSeparatedObjectSelector
{
    #region "Variables and constants"

    private const int WITHOUT_DEPARTMENT = -5;

    private bool mReflectGlobalProductsUse;
    private bool mDropDownListMode = true;
    private bool mAddWithoutDepartmentRecord;
    private bool mShowAllSites;

    #endregion


    #region "Properties"

    /// <summary>
    /// Allows to access uniselector object
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    ///  If true, selected value is DepartmentName, if false, selected value is DepartmentID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseDepartmentNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseDepartmentNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown list.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            if (DropDownListMode)
            {
                return uniSelector.DropDownSingleSelect.ClientID;
            }

            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Indicates whether global items are to be offered.
    /// </summary>
    public override bool DisplayGlobalItems
    {
        get
        {
            return base.DisplayGlobalItems || (ReflectGlobalProductsUse && ECommerceSettings.AllowGlobalProducts(SiteID));
        }
        set
        {
            base.DisplayGlobalItems = value;
        }
    }


    /// <summary>
    /// Gets or sets a value that indicates if the global items should be displayed when the global products are used on the site.
    /// </summary>
    public bool ReflectGlobalProductsUse
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ReflectGlobalProductsUse"), mReflectGlobalProductsUse);
        }
        set
        {
            SetValue("ReflectGlobalProductsUse", value);
            mReflectGlobalProductsUse = value;
        }
    }


    /// <summary>
    /// Indicates if drop down list mode is used. Default value is true.
    /// </summary>
    public bool DropDownListMode
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DropDownListMode"), mDropDownListMode);
        }
        set
        {
            SetValue("DropDownListMode", value);
            mDropDownListMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to add 'without department' item record to the dropdown list.
    /// </summary>
    public bool AddWithoutDepartmentRecord
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AddWithoutDepartmentRecord"), mAddWithoutDepartmentRecord);
        }
        set
        {
            SetValue("AddWithoutDepartmentRecord", value);
            mAddWithoutDepartmentRecord = value;
        }
    }


    /// <summary>
    /// Gets the value of 'Without department' record.
    /// </summary>
    public int WithoutDepartmentRecordValue
    {
        get
        {
            return WITHOUT_DEPARTMENT;
        }
    }


    /// <summary>
    /// Indicates whether departments from all sites are to be shown. Default value is false.
    /// </summary>
    public virtual bool ShowAllSites
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowAllSites"), mShowAllSites);
        }
        set
        {
            mShowAllSites = value;
            SetValue("ShowAllSites", value);
        }
    }

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        uniSelector.SelectionMode = (DropDownListMode ? SelectionModeEnum.SingleDropDownList : SelectionModeEnum.SingleTextBox);

        base.OnInit(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        TryInitByForm();

        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (RequestHelper.IsPostBack() && DependsOnAnotherField)
        {
            InitSelector();
        }

        uniSelector.Reload(true);

        base.OnPreRender(e);
    }

    #endregion


    #region "Initialization"

    protected override void InitSelector()
    {
        // Add special records
        if (AddWithoutDepartmentRecord)
        {
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.empty"), Value = WithoutDepartmentRecordValue.ToString() });
        }

        base.InitSelector();

        if (ShowAllSites)
        {
            uniSelector.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            uniSelector.SetValue("FilterMode", "department");
        }

        if (UseNameForSelection)
        {
            uniSelector.AllRecordValue = "";
            uniSelector.NoneRecordValue = "";
        }
    }


    /// <summary>
    /// Convert given department name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the department to be converted.</param>
    /// <param name="siteName">Name of the site of the department.</param>
    protected override int GetID(string name, string siteName)
    {
        DepartmentInfo dept;

        if (ShowAllSites)
        {
            // Take any department
            dept = DepartmentInfo.Provider.Get()
                       .TopN(1)
                       .WithCodeName(name)
                       .OrderByAscending("DepartmentSiteID")
                       .FirstOrDefault();
        }
        else
        {
            dept = DepartmentInfo.Provider.Get(name, SiteInfoProvider.GetSiteID(siteName));
        }

        return dept?.DepartmentID ?? 0;
    }


    /// <summary>
    /// Appends site where to given where condition.
    /// </summary>
    /// <param name="where">Original where condition to append site where to.</param>
    protected override string AppendSiteWhere(string where)
    {
        // Do not filter by site when showing all sites departments
        if (ShowAllSites)
        {
            return where;
        }

        return base.AppendSiteWhere(where);
    }


    private void TryInitByForm()
    {
        if ((Value != null) || (Form == null) || !Form.AdditionalData.ContainsKey("DataClassID"))
        {
            return;
        }

        var dataClassId = ValidationHelper.GetInteger(Form.AdditionalData["DataClassID"], 0);
        var dataClass = DataClassInfoProvider.GetDataClassInfo(dataClassId);
        if (dataClass != null)
        {
            var department = DepartmentInfo.Provider.Get(dataClass.ClassSKUDefaultDepartmentName, SiteID);
            if (department == null)
            {
                return;
            }

            Value = (UseNameForSelection) ? dataClass.ClassSKUDefaultDepartmentName : department.DepartmentID.ToString();
        }
    }

    #endregion
}