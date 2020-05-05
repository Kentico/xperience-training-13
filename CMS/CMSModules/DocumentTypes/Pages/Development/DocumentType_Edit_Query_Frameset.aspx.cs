using System;

using CMS.Helpers;
using CMS.UIControls;


/// <summary>
/// This frameset is used only in web part properties for Queries selector. Do not delete this file if the dialogs are not fixed.
/// </summary>
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Query_Frameset : CMSModalDesignPage
{
    protected int mHeight = 65;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        RequireSite = false;

        if (QueryHelper.GetBoolean("editonlycode", false))
        {
            mHeight = 96;
        }
        else
        {
            CheckGlobalAdministrator();
        }
    }
}