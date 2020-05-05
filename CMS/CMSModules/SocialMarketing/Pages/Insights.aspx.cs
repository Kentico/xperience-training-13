using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_SocialMarketing_Pages_Insights : CMSAdministrationPage
{
    #region "Properties"

    private int TreeWidth
    {
        get
        {
            string source = QueryHelper.GetString("source", string.Empty).ToLowerInvariant();
            switch (source)
            {
                case "facebook":
                    return 390;
                
                default:
                    return 295;
            }
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        CheckLicense(FeatureEnum.SocialMarketingInsights);
        CheckPermissions(ModuleName.SOCIALMARKETING, PermissionsEnum.Read.ToString());
    }


    protected override void OnPreRender(EventArgs e)
    {
        ScriptHelper.HideVerticalTabs(this);

        analyticsTree.Attributes["src"] = "InsightsMenu.aspx" + RequestContext.CurrentQueryString;
        if (CultureHelper.IsUICultureRTL())
        {
            ControlsHelper.ReverseFrames(colsFrameset);
        }

        colsFrameset.Attributes.Add("cols", String.Format("{0}, *", TreeWidth));
        base.OnPreRender(e);
    }

    #endregion
}
