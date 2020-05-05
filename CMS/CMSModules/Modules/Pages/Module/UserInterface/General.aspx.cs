using System;

using CMS.Core;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[UIElement(ModuleName.CMS, "Modules.UserInterface.General")]
[EditedObject(UIElementInfo.OBJECT_TYPE, "elementid")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_General : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ElementID = QueryHelper.GetInteger("elementid", 0);
        editElem.ResourceID = QueryHelper.GetInteger("moduleid", 0);
    }
}