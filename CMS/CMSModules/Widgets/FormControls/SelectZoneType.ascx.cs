using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Widgets_FormControls_SelectZoneType : FormEngineUserControl
{
    #region "Variables"

    private string mSelectedValue = "None";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether current zone is on dashboard template.
    /// </summary>
    public bool IsDashboard
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IsDashboard"), false);
        }
        set
        {
            SetValue("IsDashboard", value);
        }
    }


    /// <summary>
    /// Sets the property value of the control, setting the value affects only local property value.
    /// </summary>
    /// <param name="propertyName">Property name to set</param>
    /// <param name="value">New property value</param>
    public override bool SetValue(string propertyName, object value)
    {
        base.SetValue(propertyName, value);

        if (CMSString.Compare(propertyName, "isdashboard", true) == 0)
        {
            FillItemsCollection();
        }

        return false;
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return rblOptions.SelectedValue;
        }
        set
        {
            EnsureChildControls();
            mSelectedValue = ValidationHelper.GetString(value, String.Empty);
            rblOptions.SelectedValue = mSelectedValue;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// OnLoad - ensures child controls.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        EnsureChildControls();

        ScriptHelper.RegisterJQuery(Page);
    }


    /// <summary>
    /// Creates child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        FillItemsCollection();
        base.CreateChildControls();
    }


    /// <summary>
    /// Fills item collection with dependence on current settings.
    /// </summary>
    private void FillItemsCollection()
    {
        rblOptions.Items.Clear();

        ListItem li = null;

        li = new ListItem(GetString("widgets.zonetype.none"), "None");
        rblOptions.Items.Add(li);

        if (!IsDashboard)
        {
            li = new ListItem(GetString("widgets.zonetype.user"), "User");
            rblOptions.Items.Add(li);

            li = new ListItem(GetString("widgets.zonetype.editor"), "Editor");
            rblOptions.Items.Add(li);

            li = new ListItem(GetString("widgets.zonetype.group"), "Group");
            rblOptions.Items.Add(li);
        }
        else
        {
            li = new ListItem(GetString("widgets.zonetype.dashboard"), "Dashboard");
            rblOptions.Items.Add(li);
        }

        rblOptions.SelectedValue = mSelectedValue;
    }

    #endregion
}