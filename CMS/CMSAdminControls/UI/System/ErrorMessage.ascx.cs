using System;

using CMS.UIControls;


public partial class CMSAdminControls_UI_System_ErrorMessage : ErrorMessageControl
{
    #region "Properties"

    /// <summary>
    /// Error title.
    /// </summary>
    public override string ErrorTitle
    {
        get
        {
            return ptTitle.TitleText;
        }
        set
        {
            ptTitle.TitleText = value;
        }
    }


    /// <summary>
    /// Error message.
    /// </summary>
    public override string ErrorMessage
    {
        get
        {
            return lblMessage.Text;
        }
        set
        {
            lblMessage.Text = value;
        }
    }

    #endregion
}