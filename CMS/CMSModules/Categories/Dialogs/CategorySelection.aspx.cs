using System;
using System.Collections;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Categories_Dialogs_CategorySelection : CMSModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get title text according to selection mode
        PageTitle.TitleText = GetString(SelectionElem.AllowMultipleSelection ? "categories.selectmultiple" : "categories.select");
        // Set actions
        SelectionElem.Actions = actionsElem;

        SetSaveJavascript("return US_Submit();");
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string identifier = QueryHelper.GetString("params", null);
        Hashtable parameters = (Hashtable)WindowHelper.GetItem(identifier);

        if (!QueryHelper.ValidateHash("hash", "selectedvalue") || parameters == null)
        {
            URLHelper.Redirect(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
        }
    }
}