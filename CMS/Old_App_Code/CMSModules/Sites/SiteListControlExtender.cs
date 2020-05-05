using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

/// <summary>
/// Custom class registration.
/// </summary>
[assembly: RegisterCustomClass("SiteListControlExtender", typeof(SiteListControlExtender))]

/// <summary>
/// Site list control extender.
/// </summary>
public class SiteListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// Extender initialization.
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        Control.OnAction += Control_OnAction;
        Control.Page.PreRender += Page_PreRender;
    }


    /// <summary>
    /// Page pre-render event handling.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Add extra header actions
        ICMSPage page = Control.Page as ICMSPage;
        if (page != null)
        {
            var importElement = UIElementInfoProvider.GetUIElementInfo("CMS", "ImportSiteOrObjects");
            if (importElement != null)
            {
                page.HeaderActions.AddAction(new HeaderAction
                {
                    RedirectUrl = UIContextHelper.GetElementUrl(importElement, false),
                    Text = UIElementInfoProvider.GetElementCaption(importElement)
                });
            }

            var exportElement = UIElementInfoProvider.GetUIElementInfo("CMS", "Export");
            if (exportElement != null)
            {
                page.HeaderActions.AddAction(new HeaderAction
                {
                    RedirectUrl = UIContextHelper.GetElementUrl(exportElement, false),
                    Text = UIElementInfoProvider.GetElementCaption(exportElement)
                });
            }

            var exportHistory = UIElementInfoProvider.GetUIElementInfo("CMS", "ExportHistory");
            if (exportHistory != null)
            {
                page.HeaderActions.AddAction(new HeaderAction
                {
                    RedirectUrl = UIContextHelper.GetElementUrl(exportHistory, false),
                    Text = UIElementInfoProvider.GetElementCaption(exportHistory)
                });
            }
        }
    }


    /// <summary>
    /// External data binding handler.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        bool running;

        switch (sourceName.ToLowerCSafe())
        {
            case "openlivesite":
                {
                    // Open live site action
                    DataRowView row = (DataRowView)((GridViewRow)parameter).DataItem;
                    running = SiteIsRunning(row["SiteStatus"]);
                    if (!running)
                    {
                        var button = ((CMSGridActionButton)sender);
                        button.Enabled = false;
                    }
                }
                break;

            case "sitestatus":
                // Colorize site status
                {
                    DataRowView row = (DataRowView)parameter;
                    running = SiteIsRunning(row["SiteStatus"]);
                    bool offline = ValidationHelper.GetBoolean(row["SiteIsOffline"], false);

                    if (running)
                    {
                        if (offline)
                        {
                            return UniGridFunctions.SpanMsg(ResHelper.GetString("Site_List.Offline"), "SiteStatusOffline");
                        }
                        else
                        {
                            return UniGridFunctions.SpanMsg(ResHelper.GetString("Site_List.Running"), "SiteStatusRunning");
                        }
                    }
                    else
                    {
                        return UniGridFunctions.SpanMsg(ResHelper.GetString("Site_List.Stopped"), "SiteStatusStopped");
                    }
                }

            case "culture":
                // Culture with flag
                {
                    DataRowView row = (DataRowView)parameter;
                    string siteName = ValidationHelper.GetString(row["SiteName"], "");
                    string cultureCode = CultureHelper.GetDefaultCultureCode(siteName);
                    return UniGridFunctions.DocumentCultureFlag(cultureCode, null, Control.Page);
                }

            case "start":
                {
                    // Start action
                    DataRowView row = (DataRowView)((GridViewRow)parameter).DataItem;
                    running = SiteIsRunning(row["SiteStatus"]);
                    ((CMSGridActionButton)sender).Visible = !running;
                }
                break;

            case "stop":
                {
                    // Stop action
                    DataRowView row = (DataRowView)((GridViewRow)parameter).DataItem;
                    running = SiteIsRunning(row["SiteStatus"]);
                    ((CMSGridActionButton)sender).Visible = running;
                }
                break;
        }

        return parameter;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void Control_OnAction(string actionName, object actionArgument)
    {
        SiteInfo si = SiteInfoProvider.GetSiteInfo(ValidationHelper.GetInteger(actionArgument, 0));
        if (si != null)
        {
            string siteName = si.SiteName;

            switch (actionName)
            {
                case "delete":
                    URLHelper.Redirect("~/CMSModules/Sites/Pages/site_delete.aspx?siteid=" + actionArgument);
                    break;

                case "editContent":
                    {
                        // Build URL for site in format 'http(s)://sitedomain/application/admin'
                        string sitedomain = si.DomainName.TrimEnd('/');
                        string application = null;

                        // Support of multiple web sites on single domain
                        if (!sitedomain.Contains("/"))
                        {
                            application = UrlResolver.ResolveUrl("~/.").TrimEnd('/');
                        }

                        // Application includes string '/admin'.
                        application += "/admin/";
                        string url = RequestContext.CurrentScheme + "://" + sitedomain + application;
                        ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "EditContentScript", ScriptHelper.GetScript("window.open('" + url + "');"));
                    }
                    break;

                case "openLiveSite":
                    {
                        var url = DocumentUIHelper.GetHomePageUrl(si.SiteID, CultureHelper.GetDefaultCultureCode(si.SiteName));
                        ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "OpenLiveSiteScript", ScriptHelper.GetScript("window.open('" + url + "');"));
                    }
                    break;

                case "start":
                    try
                    {
                        SiteInfoProvider.RunSite(siteName);
                    }
                    catch (Exception ex)
                    {
                        Control.ShowError(ResHelper.GetString("Site_List.ErrorMsg"), ex.Message, null);
                    }
                    break;

                case "stop":
                    SiteInfoProvider.StopSite(siteName);
                    break;

                case "export":
                    URLHelper.Redirect(URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Export", false), "siteID=" + actionArgument));
                    break;
            }
        }
    }


    /// <summary>
    /// Returns true when the given site status indicates a running site.
    /// </summary>
    private bool SiteIsRunning(object siteStatus)
    {
        if (siteStatus == null)
        {
            return false;
        }

        string str = siteStatus.ToString();
        return str.ToEnum<SiteStatusEnum>() == SiteStatusEnum.Running;
    }
}