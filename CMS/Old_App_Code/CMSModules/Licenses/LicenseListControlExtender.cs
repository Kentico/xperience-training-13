using System;
using System.Text;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.UIControls;

// Custom class registration.
[assembly: RegisterCustomClass("LicenseListControlExtender", typeof(LicenseListControlExtender))]

/// <summary>
/// License list control extender.
/// </summary>
public class LicenseListControlExtender : ControlExtender<UniGrid>
{
    #region "Constants"

    /// <summary>
    /// Client portal URL
    /// </summary>
    private const string CLIENT_PORTAL = "http://client.kentico.com/";


    /// <summary>
    /// Sales e-mail address
    /// </summary>
    private const string SALES_MAIL = "sales@kentico.com";

    #endregion


    #region "Variables"

    private bool mRefreshUI;

    #endregion


    #region "Events"

    /// <summary>
    /// Extender initialization.
    /// </summary>
    public override void OnInit()
    {
        mRefreshUI = QueryHelper.GetBoolean("reload", false);

        Control.GridView.PageSize = 20;
        Control.OnAction += Control_OnAction;
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        Control.Page.PreRender += Page_PreRender;
        Control.ZeroRowsText = ResHelper.GetString("general.nodatafound");

        // Check if valid license for current domain exists
        var validationResult = LicenseHelper.ValidateLicenseForDomain(RequestContext.CurrentDomain);
        if (validationResult != LicenseValidationEnum.Valid)
        {
            // Build invalid license string
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} <strong>{2}</strong><br /><br />", GetString("invalidlicense.currentdomain"), GetString("InvalidLicense.Result"), LicenseHelper.GetValidationResultString(validationResult))
              .AppendFormat("<strong>{0}</strong><br /><ul>", GetString("invalidlicense.howto"))
              .AppendFormat("<li>{0} ", GetString("invalidlicense.howto.option1.firstpart"))
              .AppendFormat("<a target=\"_blank\" href=\"{0}\" title=\"{1}\">{2}</a>", CLIENT_PORTAL, CLIENT_PORTAL, GetString("invalidlicense.clientportal"))
              .AppendFormat("{0}</li>", GetString("invalidlicense.howto.option1.secondpart"))
              .AppendFormat("<li>{0} <a href=\"mailto:{1}\">{2}</a>.</li>", GetString("invalidlicense.howto.option2"), SALES_MAIL, SALES_MAIL)
              .AppendFormat("<li>{0} ", GetString("invalidlicense.howto.option3.firstpart"))
              .AppendFormat("<a target=\"_blank\" href=\"{0}\" title=\"{1}\">{2}</a>", CLIENT_PORTAL, CLIENT_PORTAL, GetString("invalidlicense.clientportal"))
              .AppendFormat("{0}</li>", GetString("invalidlicense.howto.option3.secondpart"));

            if (validationResult == LicenseValidationEnum.Expired)
            {
                sb.Append("<li>" + GetString("invalidlicense.trialexpired") + "</li>");
            }

            sb.Append("</ul>");

            Control.ShowInformation(sb.ToString());
        }
    }


    private void Page_PreRender(object sender, EventArgs e)
    {
        // Add extra header actions
        var page = Control.Page as ICMSPage;
        if (page != null)
        {
            var newAction = new HeaderAction
            {
                Text = ResHelper.GetString("license.list.export"),
                RedirectUrl = "~/CMSModules/Licenses/Pages/License_Export_Domains.aspx",
            };

            page.HeaderActions.AddAction(newAction);
        }

        if (mRefreshUI)
        {
            // Register javascript to reload application
            ScriptHelper.RegisterModule(Control, "CMS/Licenses", true);
        }
    }


    /// <summary>
    /// External data binding handler.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourcename, object parameter)
    {
        switch (sourcename.ToLowerCSafe())
        {
            case "editionname":
                string edition = ValidationHelper.GetString(parameter, "").ToUpperCSafe();
                try
                {
                    return LicenseHelper.GetEditionName(edition.ToEnum<ProductEditionEnum>());
                }
                catch
                {
                    return "#UNKNOWN#";
                }

            case "expiration":
                return ResHelper.GetString(Convert.ToString(parameter));

            case "licenseservers":
                int count = ValidationHelper.GetInteger(parameter, -1);
                if (count == LicenseKeyInfo.SERVERS_UNLIMITED)
                {
                    return ResHelper.GetString("general.unlimited");
                }
                if (count > 0)
                {
                    return count.ToString();
                }
                return String.Empty;
        }
        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionname">Name of item (button) that throws event</param>
    /// <param name="actionargument">ID (value of Primary key) of corresponding data row</param>
    private void Control_OnAction(string actionname, object actionargument)
    {
        if (actionname == "delete")
        {
            LicenseKeyInfoProvider.DeleteLicenseKeyInfo(Convert.ToInt32(actionargument));

            using (new CMSActionContext { AllowLicenseRedirect = false })
            {
                UserInfoProvider.ClearLicenseValues();
                Functions.ClearHashtables();
            }

            mRefreshUI = true;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets resource string.
    /// </summary>
    /// <param name="stringKey">Resource string key.</param>
    private string GetString(string stringKey)
    {
        return ResHelper.GetString(stringKey);
    }

    #endregion
}
