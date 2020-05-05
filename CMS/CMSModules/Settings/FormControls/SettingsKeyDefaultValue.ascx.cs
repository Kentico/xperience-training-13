using System;

using CMS.Base;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Settings_FormControls_SettingsKeyDefaultValue : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the default value of the setting.
    /// </summary>
    public override object Value
    {
        get
        {
            if (chkKeyValue.Visible)
            {
                return chkKeyValue.Checked;
            }
            else
            {
                return txtKeyValue.Text;
            }
        }
        set
        {
            txtKeyValue.Text = ValidationHelper.GetString(value, "");
            chkKeyValue.Checked = ValidationHelper.GetBoolean(value, false);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            SelectDefaultValueControl();
        }

    }


    /// <summary>
    /// Shows suitable default value edit control accordint to key type.
    /// </summary>
    private void SelectDefaultValueControl()
    {
        if (Form != null)
        {
            chkKeyValue.Visible = ValidationHelper.GetString(Form.GetFieldValue("KeyType"), "").EqualsCSafe("boolean");
            txtKeyValue.Visible = !chkKeyValue.Visible;
        }
    }
}