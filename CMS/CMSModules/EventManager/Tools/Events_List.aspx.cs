using System;

using CMS.Core;
using CMS.UIControls;


[UIElement(ModuleName.EVENTMANAGER, "EventManager")]
public partial class CMSModules_EventManager_Tools_Events_List : CMSEventManagerPage
{
    protected override void OnPreRender(EventArgs e)
    {
        // Set the page title
        PageTitle.TitleText = GetString("Events_List.HeaderCaption");
        eventList.ReloadData();
        base.OnPreRender(e);
    }
}