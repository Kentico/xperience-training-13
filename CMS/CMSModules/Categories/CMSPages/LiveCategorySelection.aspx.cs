using System;
using System.Collections;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Categories_CMSPages_LiveCategorySelection : CMSLiveModalPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get title text according to selection mode
        PageTitle.TitleText = GetString(SelectionElem.AllowMultipleSelection ? "categories.selectmultiple" : "categories.select");
        // Set actions
        SelectionElem.Actions = actionsElem;
    }


    /// <summary>
    /// Load event handler
    /// </summary>
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