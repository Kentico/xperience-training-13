using System;

using CMS.Base;
using CMS.DocumentEngine.Web.UI;


public partial class CMSModules_FormControls_Controls_InheritedFilter : CMSAbstractBaseFilterControl
{
    #region "Variables"

    private string mSelectedValue = String.Empty;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return drpDownList.SelectedValue;
        }
        set
        {
            mSelectedValue = (string)value;
            if (drpDownList.Items.FindByValue(mSelectedValue) != null)
            {
                drpDownList.SelectedValue = mSelectedValue;
            }
        }
    }


    public override string WhereCondition
    {
        get
        {
            switch (drpDownList.SelectedValue.ToLowerCSafe())
            {
                case "no":
                    return "UserControlParentID IS NULL";

                case "yes":
                    return "UserControlParentID IS NOT NULL";

                default:
                    return String.Empty;
            }
        }
    }

    #endregion


    #region "Methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        drpDownList.SelectedIndexChanged += new EventHandler(drpDownList_SelectedIndexChanged);
    }


    private void drpDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
        FilterChanged = true;
        RaiseOnFilterChanged();
    }


    /// <summary>
    /// Resets filter to default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpDownList.SelectedIndex = 0;
    }

    #endregion
}
