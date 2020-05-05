using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("BadWords_List.HeaderCaption")]
[UIElement(ModuleName.BADWORDS, "Administration.BadWords")]
public partial class CMSModules_BadWords_BadWords_List : GlobalAdminPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        BadWordsListControlExtender extender = new BadWordsListControlExtender();
        extender.Init(UniGrid);

        ActionAttribute addNewBadWord = new ActionAttribute(0, "BadWords_List.NewItemCaption", 
            URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("CMS.BadWords", "Administration.BadWords.New"), "displaytitle", "false"));
        addNewBadWord.Apply(this);

        UniGrid.ZeroRowsText = GetString("general.nodatafound");
    }
}
