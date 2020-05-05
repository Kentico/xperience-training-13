using System;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_InsertImageOrMedia_Default : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string output = QueryHelper.GetString("output", "");
        if (output == "copy")
        {
            Title = GetString("dialogs.header.title.copydoc");
        }
        else if (output == "move")
        {
            Title = GetString("dialogs.header.title.movedoc");
        }
        else if ((output == "link") || (output == "linkdoc"))
        {
            Title = GetString("dialogs.header.title.linkdoc");
        }
    }
}