using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Globalization;
using CMS.Helpers;
using CMS.UIControls;


[Title("TimeZ.List.Header")]
[UIElement(ModuleName.CMS, "Development.TimeZones")]
public partial class CMSModules_TimeZones_Pages_TimeZone_List : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.OnExternalDataBound += uniGrid_OnExternalDataBound;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");

        // New item link
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("TimeZ.List.NewItem"),
            RedirectUrl = ResolveUrl("TimeZone_Edit.aspx")

        });
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                URLHelper.Redirect(UrlResolver.ResolveUrl("TimeZone_Edit.aspx?zoneid=" + Convert.ToString(actionArgument)));
                break;

            case "delete":
                TimeZoneInfoProvider.DeleteTimeZoneInfo(Convert.ToInt32(actionArgument));
                break;
        }
    }


    private object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "daylight":
                return UniGridFunctions.ColorLessSpanYesNo(parameter);
        }

        return parameter;
    }
}