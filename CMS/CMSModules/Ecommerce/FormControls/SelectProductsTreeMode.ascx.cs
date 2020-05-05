using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_FormControls_SelectProductsTreeMode : FormEngineUserControl
{
    #region "Properties"

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
            base.Enabled = value;
            chkTreeMode.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return chkTreeMode.Checked ? "Sections" : "None";
        }
        set
        {
            chkTreeMode.Checked = ValidationHelper.GetString(value, "Sections").Equals("Sections", StringComparison.Ordinal);
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion
}