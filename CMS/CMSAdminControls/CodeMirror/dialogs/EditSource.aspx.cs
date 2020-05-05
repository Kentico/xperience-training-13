using System;

using CMS.UIControls;


/// <summary>
/// Represents an Edit source dialog in CodeMirror (used for example in CSS edit).
/// </summary>
public partial class CMSAdminControls_CodeMirror_dialogs_EditSource : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetSaveJavascript("setSource(); CloseDialog();");
        PageTitle.TitleText = GetString("general.editsource");
    }
}