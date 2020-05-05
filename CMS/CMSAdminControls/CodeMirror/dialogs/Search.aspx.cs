using System;

using CMS.UIControls;


/// <summary>
/// Represents a Search dialog window.
/// </summary>
public partial class CMSAdminControls_CodeMirror_dialogs_Search : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.TitleText = GetString("general.search");
        Form.DefaultButton = btnSearch.UniqueID;
        Form.DefaultFocus = btnSearch.UniqueID;
    }
}