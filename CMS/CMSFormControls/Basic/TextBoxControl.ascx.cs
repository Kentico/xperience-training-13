using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;

public partial class CMSFormControls_Basic_TextBoxControl : TextBoxControl
{
    /// <summary>
    /// Textbox control
    /// </summary>
    protected override CMSTextBox TextBox
    {
        get
        {
            return txtText;
        }
    }
}
