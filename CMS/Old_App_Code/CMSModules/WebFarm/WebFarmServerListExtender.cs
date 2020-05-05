using System;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.WebFarmSync.Internal;

[assembly: RegisterCustomClass("WebFarmServerListExtender", typeof(WebFarmServerListExtender))]

/// <summary>
/// Web farm server unigrid extender.
/// </summary>
public class WebFarmServerListExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAction += OnAction;
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.ZeroRowsText = ResHelper.GetString("general.nodatafound");

        string messageToShow;

        if (WebFarmContext.WebFarmEnabled)
        {
            messageToShow = SystemContext.IsRunningOnAzure ? "WebFarm.EnabledAzure" : "WebFarm.Enabled";
        }
        else
        {
            messageToShow = "WebFarm.Disabled";
        }

        Control.ShowInformation(String.Format(ResHelper.GetString(messageToShow), SystemContext.ServerName));

        if (!WebFarmLicenseHelper.LicenseIsValid)
        {
            Control.ShowError(ResHelper.GetString("webfarm.unsufficientdomainlicense"));
        }
    }


    /// <summary>
    /// Handles action event of unigrid.
    /// </summary>
    protected void OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl("WebFarm_Server_Edit.aspx?serverid=" + ValidationHelper.GetString(actionArgument, String.Empty)));
        }
        else if (actionName == "delete")
        {
            // Delete WebFarmServerInfo object from database
            WebFarmServerInfoProvider.DeleteWebFarmServerInfo(ValidationHelper.GetInteger(actionArgument, 0));
        }
    }


    /// <summary>
    /// Handles external databound event of unigrid.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "serveredit":
                if (WebFarmContext.WebFarmMode == WebFarmModeEnum.Automatic)
                {
                    CMSGridActionButton button = (CMSGridActionButton)sender;
                    button.Enabled = false;
                    button.ToolTip = ResHelper.GetString("webfarmservers_list.disablededit");
                }
                break;

            case "serverenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);

            case "serverstatus":
                var server = WebFarmServerInfoProvider.GetWebFarmServerInfo(ValidationHelper.GetInteger(parameter, 0));
                switch (server.Status)
                {
                    case WebFarmServerStatusEnum.Healthy:
                        return new Tag
                        {
                            Text = ResHelper.GetString("webfarmservers_list.status.healthy"),
                            Color = "#497d04"
                        };

                    case WebFarmServerStatusEnum.Transitioning:
                        return new Tag
                        {
                            Text = ResHelper.GetString("webfarmservers_list.status.transitioning"),
                            Color = "#c98209"
                        };

                    default:
                        return new Tag
                        {
                            Text = ResHelper.GetString("webfarmservers_list.status.notresponding"),
                            Color = "#b12628"
                        };
                }
        }
        return parameter;
    }
}