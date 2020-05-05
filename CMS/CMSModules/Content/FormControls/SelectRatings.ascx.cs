using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.IO;


public partial class CMSModules_Content_FormControls_SelectRatings : FormEngineUserControl
{
    private string[] ratingControls = { "DropDown", "RadioButtons", "Stars" };

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpRatingsControls.Enabled;
        }
        set
        {
            drpRatingsControls.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return ValidationHelper.GetString(drpRatingsControls.SelectedValue, null);
        }
        set
        {
            EnsureChildControls();
            var selected = ValidationHelper.GetString(value, null);
            if (drpRatingsControls.Items.FindByValue(selected) != null)
            {
                drpRatingsControls.SelectedValue = selected;
            }
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        return true;
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        string oldSelectedValue = drpRatingsControls.SelectedValue;

        drpRatingsControls.Items.Clear();
        foreach (string fileName in ratingControls)
        {
            drpRatingsControls.Items.Add(new ListItem(fileName, fileName));
        }

        if (drpRatingsControls.Items.FindByValue(oldSelectedValue) != null)
        {
            drpRatingsControls.SelectedValue = oldSelectedValue;
        }
    }
}