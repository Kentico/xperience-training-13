using System;

using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;


public partial class CMSModules_Membership_FormControls_Users_MultipleCategoriesSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets coma separated ID of selected categories.
    /// </summary>
    public override object Value
    {
        get
        {
            if (FieldInfo != null)
            {
                return FieldInfo.AllowEmpty ? null : String.Empty;
            }

            return null;
        }
        set
        {
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
            base.IsLiveSite = value;
            categorySelector.IsLiveSite = value;
        }
    }


    /// <summary>
    /// This form control should not be checked for min/max value length.
    /// </summary>
    public override bool CheckMinMaxLength
    {
        get
        {
            return false;
        }
        set
        {
            base.CheckMinMaxLength = value;
        }
    }


    /// <summary>
    /// This form control should not be checked for regular expression.
    /// </summary>
    public override bool CheckRegularExpression
    {
        get
        {
            return false;
        }
        set
        {
            base.CheckRegularExpression = value;
        }
    }

    #endregion


    #region "Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        CheckFieldEmptiness = false;
        if (Form != null)
        {
            Form.OnAfterDataLoad += Form_OnAfterDataLoad;
            Form.OnBeforeSave += Form_OnBeforeSave;
            Form.OnAfterSave += Form_OnAfterSave;

            InitializeCategorySelector();
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if ((Form != null) && Form.IsFirstLoad)
        {
            categorySelector.ReloadData();
        }
    }


    private void Form_OnBeforeSave(object sender, EventArgs e)
    {
        // Initialize current values before save to support categories added using handler
        categorySelector.InitCurrentValues();
    }


    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        // Set document ID - insert mode
        InitializeCategorySelector();

        categorySelector.Save();
    }


    private void Form_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Set document ID - edit mode
        InitializeCategorySelector();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads control value
    /// </summary>
    /// <param name="value">Value to load</param>
    public override void LoadControlValue(object value)
    {
        categorySelector.ReloadData(true);
    }


    public override bool IsValid()
    {
        var isValid = base.IsValid();

        // Check emptiness
        if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
        {
            var isEmpty = String.IsNullOrEmpty(ValidationHelper.GetString(categorySelector.UniSelector.Value, ""));
            if (isEmpty)
            {
                ValidationError = ResHelper.GetString("BasicForm.ErrorEmptyValue");
            }

            isValid &= !isEmpty;
        }

        return isValid;
    }


    /// <summary>
    /// Initializes category selector.
    /// </summary>
    private void InitializeCategorySelector()
    {
        if (Form.Data != null)
        {
            TreeNode node = Form.Data as TreeNode;
            if (node != null)
            {
                categorySelector.Node = node;
                categorySelector.UserID = MembershipContext.AuthenticatedUser.UserID;
            }
        }
    }

    #endregion
}