using System;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_AlternativeForms_AlternativeForms_Layout : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.PanelContent.CssClass = string.Empty;

        layoutElem.FormLayoutType = FormLayoutTypeEnum.Document;
        layoutElem.ObjectID = QueryHelper.GetInteger("objectid", 0);
        layoutElem.IsAlternative = true;
    }
}