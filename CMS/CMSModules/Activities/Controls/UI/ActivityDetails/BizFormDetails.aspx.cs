using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("om.activitydetals.viewrecorddetail")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Controls_UI_ActivityDetails_BizFormDetails : CMSModalPage
{
    private const string FORM_ITEM_PREVIEW_ROUTE_TEMPLATE = "/Kentico.FormBuilder/FormItem/Preview/{0}/{1}";


    protected void Page_Init(object sender, EventArgs e)
    {
        // Check permissions
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        int bizId = QueryHelper.GetInteger("bizid", 0);
        int recId = QueryHelper.GetInteger("recid", 0);

        if ((bizId > 0) && (recId > 0))
        {
            var bfi = BizFormInfo.Provider.Get(bizId);
            if (bfi == null)
            {
                return;
            }

            var path = string.Format(FORM_ITEM_PREVIEW_ROUTE_TEMPLATE, bfi.FormID, recId);
            var site = bfi.Site as SiteInfo;
            var url = new PresentationUrlRetriever().RetrieveForAdministration(site.SiteName);

            // Modify frame 'src' attribute and add administration domain into it
            ScriptHelper.RegisterModule(this, "CMS.Builder/FrameSrcAttributeModifier", new
            {
                frameId = mvcFrame.ClientID,
                frameSrc = url.TrimEnd('/') + VirtualContext.GetFormBuilderPath(path, MembershipContext.AuthenticatedUser.UserGUID),
                mixedContentMessage = GetString("builder.ui.mixedcontenterrormessage"),
                applicationPath = SystemContext.ApplicationPath
            });
            
            RegisterCookiePolicyDetection();
        }
    }
}