using System;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Routing;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("DomainAliasListControlExtender", typeof(DomainAliasListControlExtender))]

/// <summary>
/// Domain alias list control extender.
/// </summary>
public class DomainAliasListControlExtender : ControlExtender<UniGrid>
{
    #region "Methods"

    /// <summary>
    /// Initialization.
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
        Control.OnAction += Control_OnAction;
        Control.OnBeforeDataReload += Control_OnBeforeDataReload;
    }


    private void Control_OnBeforeDataReload()
    {
        var siteId = ((Control.Parent.Page as CMSUIPage)?.EditedObjectParent as SiteInfo)?.SiteID ?? 0;

        if (PageRoutingHelper.GetUrlCultureFormat(siteId) == PageRoutingUrlCultureFormatEnum.LanguagePrefix
          && PageRoutingHelper.GetRoutingMode(siteId) == PageRoutingModeEnum.BasedOnContentTree)
        {
            Control.NamedColumns["DefaultVisitorCulture"].Visible = false;
        }
    }

 
    /// <summary>
    /// Handle OnExternalDataBound event of the grid.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="sourceName">Event source name</param>
    /// <param name="parameter">Event parameter</param>
    /// <returns></returns>
    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "sitedefaultvisitorculture":
              
                // Get visitor culture
                string defaultCulture = parameter as string;

                // If not set it is Automatic
                if (String.IsNullOrEmpty(defaultCulture))
                {
                    return ResHelper.GetString("Site_Edit.Automatic");
                }
                else
                {
                    CultureInfo ci = CultureInfoProvider.GetCultureInfo(defaultCulture);
                    if (ci != null)
                    {
                        return ci.CultureName;
                    }
                }
                break;
        }

        return String.Empty;
    }


    /// <summary>
    /// UniGrid actions.
    /// </summary>
    protected void Control_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int aliasId = ValidationHelper.GetInteger(actionArgument, 0);
            SiteDomainAliasInfoProvider.DeleteSiteDomainAliasInfo(aliasId);
        }
    }

    #endregion
}