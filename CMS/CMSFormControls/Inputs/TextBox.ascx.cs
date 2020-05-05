using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_Inputs_TextBox : FormEngineUserControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets encoded textbox value.
    /// </summary>
    public override object Value
    {
        get
        {
            return Trim ? txtValue.Text.Trim() : txtValue.Text;
        }
        set
        {
            txtValue.Text = ValidationHelper.GetString(value, String.Empty);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckMinMaxLength = true;
        CheckRegularExpression = true;
    }

    #endregion
}