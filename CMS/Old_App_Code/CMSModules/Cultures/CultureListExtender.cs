using System.Data;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("CultureListExtender", typeof(CultureListExtender))]

/// <summary>
/// Culture list extender
/// </summary>
public class CultureListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        Control.OnAction += OnAction;
        Control.OnExternalDataBound += OnExternalDataBound;
    }


    /// <summary>
    /// External data bound handler.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "culturename":
                DataRowView drv = (DataRowView)parameter;

                string name = ResHelper.LocalizeString(ValidationHelper.GetString(drv["CultureName"], string.Empty));
                string code = ValidationHelper.GetString(drv["CultureCode"], string.Empty);

                return string.Format("<img class=\"cms-icon-80\" src=\"{0}\" alt=\"{1}\" />&nbsp;{2}",
                                     UIHelper.GetFlagIconUrl(Control.Page, code, "16x16"),
                                     HTMLHelper.EncodeForHtmlAttribute(name),
                                     HTMLHelper.HTMLEncode(name));
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                DeleteCulture(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }


    private void DeleteCulture(int cultureId)
    {
        CultureInfo culture = CultureInfoProvider.GetCultureInfo(cultureId);
        Control.EditedObject = culture;

        string cultureCode = culture.CultureCode;
        DataSet ds = CultureSiteInfoProvider.GetCultureSites(cultureCode);
        if (DataHelper.DataSourceIsEmpty(ds))
        {
            CultureInfoProvider.DeleteCultureInfo(cultureCode);
        }
        else
        {
            Control.ShowError(Control.GetString("culture.errorremoveculture"));
        }
    }
}
