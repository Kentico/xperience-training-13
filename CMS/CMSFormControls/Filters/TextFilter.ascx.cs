using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;

public partial class CMSFormControls_Filters_TextFilter : TextFilterControl
{
    /// <summary>
    /// Text box control
    /// </summary>
    protected override CMSTextBox TextBoxControl
    {
        get
        {
            return txtText;
        }
    }


    /// <summary>
    /// Operator drop down list control
    /// </summary>
    protected override CMSDropDownList OperatorControl
    {
        get
        {
            return drpOperator;
        }
    }
}