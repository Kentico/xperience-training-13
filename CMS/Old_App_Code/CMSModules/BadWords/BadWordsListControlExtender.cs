using System;
using System.Data;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("BadWordsListControlExtender", typeof(BadWordsListControlExtender))]

/// <summary>
/// Bad words list control extender
/// </summary>
public class BadWordsListControlExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.OnAction += OnAction;
    }


    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        bool inherited = false;
        BadWordActionEnum action = BadWordActionEnum.None;
        string siteName = SiteContext.CurrentSiteName;

        switch (sourceName.ToLowerCSafe())
        {
            case "wordaction":
                if (!string.IsNullOrEmpty(parameter.ToString()))
                {
                    action = (BadWordActionEnum)Enum.Parse(typeof(BadWordActionEnum), parameter.ToString());
                }
                else
                {
                    action = BadWordsHelper.BadWordsAction(siteName);
                    inherited = true;
                }

                // Ensure displaying text labels instead of numbers
                switch (action)
                {
                    case BadWordActionEnum.Remove:
                        parameter = Control.GetString("general.remove");
                        break;

                    case BadWordActionEnum.Replace:
                        parameter = Control.GetString("general.replace");
                        break;

                    case BadWordActionEnum.ReportAbuse:
                        parameter = Control.GetString("BadWords_Edit.ReportAbuse");
                        break;

                    case BadWordActionEnum.RequestModeration:
                        parameter = Control.GetString("BadWords_Edit.RequestModeration");
                        break;

                    case BadWordActionEnum.Deny:
                        parameter = Control.GetString("Security.Deny");
                        break;
                }
                if (inherited)
                {
                    parameter += " " + Control.GetString("BadWords_Edit.Inherited");
                }
                break;

            case "wordreplacement":

                // Get DataRowView
                DataRowView drv = parameter as DataRowView;
                if (drv != null)
                {
                    string replacement = drv.Row["WordReplacement"].ToString();
                    string toReturn = replacement;

                    // Set 'inherited' only if WordReplacement is empty
                    if (string.IsNullOrEmpty(replacement))
                    {
                        // Get action from cell
                        string actionText = drv.Row["WordAction"].ToString();

                        // Get action enum value
                        if (string.IsNullOrEmpty(actionText))
                        {
                            action = BadWordsHelper.BadWordsAction(siteName);
                        }
                        else
                        {
                            action = (BadWordActionEnum)ValidationHelper.GetInteger(actionText, 0);
                        }

                        // Set replacement only if action is replace
                        if (action == BadWordActionEnum.Replace)
                        {
                            // Get inherited replacement from settings
                            if (string.IsNullOrEmpty(toReturn))
                            {
                                string inheritedSetting = SettingsKeyInfoProvider.GetValue(siteName + ".CMSBadWordsReplacement");
                                toReturn += inheritedSetting + " " + Control.GetString("BadWords_Edit.Inherited");
                            }
                        }
                        else
                        {
                            toReturn = string.Empty;
                        }
                    }
                    return HTMLHelper.HTMLEncode(toReturn);
                }
                return null;

            case "global":
                bool global = ValidationHelper.GetBoolean(parameter, false);
                return UniGridFunctions.ColoredSpanYesNo(global);
        }
        return HTMLHelper.HTMLEncode(parameter.ToString());
    }


    private void OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                BadWordInfoProvider.DeleteBadWordInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "edit":
                string editUrl = UIContextHelper.GetElementUrl("CMS.BadWords", "Administration.BadWords.Edit");
                editUrl = URLHelper.AddParameterToUrl(editUrl, "objectid", actionArgument.ToString());
                editUrl = URLHelper.AddParameterToUrl(editUrl, "displaytitle", "false");
                URLHelper.Redirect(editUrl);
                break;
        }
    }
}