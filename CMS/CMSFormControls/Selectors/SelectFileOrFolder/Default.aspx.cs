using System;

using CMS.UIControls;


public partial class CMSFormControls_Selectors_SelectFileOrFolder_Default : CMSModalPage
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        ValidateHash();
    }
}