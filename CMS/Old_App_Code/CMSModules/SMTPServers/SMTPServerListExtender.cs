using System.Data;
using System.Web.UI.WebControls;

using CMS;
using CMS.EmailEngine;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("SMTPServerListExtender", typeof(SMTPServerListExtender))]

/// <summary>
/// SMTP server unigrid extender
/// </summary>
public class SMTPServerListExtender : ControlExtender<UniGrid>
{
    public override void OnInit()
    {
        Control.OnAction += OnAction;
        Control.OnExternalDataBound += OnExternalDataBound;
    }


    /// <summary>
    /// Handles action event of unigrid.
    /// </summary>
    protected void OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "enable":
                // Enable SMTP server
                SMTPServerInfoProvider.EnableSMTPServer(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "disable":
                // Disable SMTP server
                SMTPServerInfoProvider.DisableSMTPServer(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }


    /// <summary>
    /// Handles external databound event of unigrid.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            // Display/hide buttons for enabling/disabling SMTP servers
            case "enable":
                ((CMSGridActionButton)sender).Visible = !ServerEnabled(parameter);
                return parameter;

            case "disable":
                ((CMSGridActionButton)sender).Visible = ServerEnabled(parameter);
                return parameter;

            default:
                return parameter;
        }
    }


    /// <summary>
    /// Check if SMTP server is enabled.
    /// </summary>
    /// <param name="parameter">GridView row with SMTP server data</param>
    private static bool ServerEnabled(object parameter)
    {
        DataRowView rowView = ((parameter as GridViewRow).DataItem) as DataRowView;
        return ValidationHelper.GetBoolean(rowView["ServerEnabled"], false);
    }
}