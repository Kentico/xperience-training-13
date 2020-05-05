using System;

using CMS.UIControls;


/// <summary>
/// Represents a Find and replace dialog window.
/// </summary>
public partial class CMSAdminControls_CodeMirror_dialogs_Replace : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("general.replace");
        Form.DefaultButton = btnFind.UniqueID;
        Form.DefaultFocus = btnFind.UniqueID;
    }
}