using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_FormControls_LiveSelectors_InsertImageOrMedia_Tabs_Media : CMSPage, ICallbackEventHandler
{
    private const string UI_LAYOUT_KEY = nameof(CMSModules_MediaLibrary_FormControls_LiveSelectors_InsertImageOrMedia_Tabs_Media);


    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            // Check site availability
            if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", SiteContext.CurrentSiteName))
            {
                RedirectToResourceNotAvailableOnSite("CMS.MediaLibrary");
            }

            string output = QueryHelper.GetString("output", "");

            bool checkUI = ValidationHelper.GetBoolean(Service.Resolve<IAppSettingsService>()["CKEditor:PersonalizeToolbarOnLiveSite"], false);
            if ((output == "copy") || (output == "move") || (output == "relationship") || (output == "selectpath"))
            {
                checkUI = false;
            }

            if (checkUI)
            {
                string errorMessage = "";

                OutputFormatEnum outputFormat = CMSDialogHelper.GetOutputFormat(output, QueryHelper.GetBoolean("link", false));
                if ((outputFormat == OutputFormatEnum.HTMLLink) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertLink"))
                {
                    errorMessage = "InsertLink";
                }
                else if ((outputFormat == OutputFormatEnum.HTMLMedia) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertImageOrMedia"))
                {
                    errorMessage = "InsertImageOrMedia";
                }

                if (errorMessage != "")
                {
                    RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", errorMessage);
                }

                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "MediaLibrariesTab"))
                {
                    errorMessage = "MediaLibrariesTab";
                }

                if (errorMessage != "")
                {
                    RedirectToUIElementAccessDenied("CMS.MediaDialog", errorMessage);
                }
            }

            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);

            linkMedia.InitFromQueryString();
        }
        else
        {
            linkMedia.StopProcessing = true;
        }

        linkMedia.UILayoutKey = UI_LAYOUT_KEY;
    }


    string ICallbackEventHandler.GetCallbackResult()
    {
        return null;
    }


    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        var parsed = eventArgument.Split(new[] { UILayoutHelper.DELIMITER });
        if (parsed.Length == 2 && String.Equals(UILayoutHelper.WIDTH_ARGUMENT, parsed[0], StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(parsed[1], out var width))
            {
                UILayoutHelper.SetLayoutWidth(UI_LAYOUT_KEY, width);
            }
        }
        else if (parsed.Length == 2 && String.Equals(UILayoutHelper.COLLAPSED_ARGUMENT, parsed[0], StringComparison.OrdinalIgnoreCase))
        {
            if (bool.TryParse(parsed[1], out var value))
            {
                UILayoutHelper.SetVerticalResizerCollapsed(UI_LAYOUT_KEY, value);
            }
        }
    }
}
