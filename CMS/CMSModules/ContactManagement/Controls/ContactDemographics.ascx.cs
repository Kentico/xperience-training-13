using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

using Newtonsoft.Json;

[CheckLicence(FeatureEnum.Ecommerce)]
public partial class CMSModules_ContactManagement_Controls_ContactDemographics : CMSUserControl
{
    private ObjectQuery<ContactInfo> mContacts;
    private Dictionary<string, string> mCommonResourcesStrings;
    private NameValueCollection mQueryParameters;


    protected void Page_Load(object sender, EventArgs e)
    {
        EnsurePermissions();

        SetUpQueryParameters();
        SetUpRetriever();
        SetUpTitle();
        HandlePersonaModuleAvailability();

        RegisterJavascriptModules();

        gridElem.OnDataReload += GridElem_OnDataReload;
        gridElem.OnExternalDataBound += SetUpEditLink;
    }


    private IContactDemographicsDataRetriever mContactDemographicsDataRetriever;


    private void EnsurePermissions()
    {
        var user = MembershipContext.AuthenticatedUser;
        var modules = new[]
        {
            ModuleName.PERSONAS,
            ModuleName.CONTACTMANAGEMENT
        };

        string readPermission = UserSecurityHelper.GetPermissionName(PermissionsEnum.Read);

        foreach (var module in modules)
        {
            if (!user.IsAuthorizedPerResource(module, readPermission))
            {
                RedirectToAccessDenied(module, readPermission);
            }
        }
    }


    private void HandlePersonaModuleAvailability()
    {
        if(!ModuleEntryManager.IsModuleLoaded(ModuleName.PERSONAS))
        {
            lblPersonaAndGender.ResourceString = "om.contact.demographics.graphicalrepresentation.gender";
            personaChartDiv.Visible = false;
        }
    }


    private string SetUpEditLink(object sender, string sourceName, object parameter)
    {
        var button = (CMSGridActionButton)sender;
        int contactID = button.CommandArgument.ToInteger(0);
        string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", contactID);
        ScriptHelper.RegisterDialogScript(Page);

        button.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
        return null;
    }


    private void SetUpQueryParameters()
    {
        mQueryParameters = HttpUtility.ParseQueryString(CMSHttpContext.Current.Request.QueryString.ToString());
    }


    private void SetUpTitle()
    {
        string caption = GetString("om.contact.demographics");
        mContactDemographicsDataRetriever.GetCaption();

        ScriptHelper.RegisterRequireJs(Page);
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "BreadcrumbsOverwriting", ScriptHelper.GetScript($@"
        cmsrequire(['CMS/EventHub', 'jQuery'], function(hub, $) {{
              hub.publish('OverwriteBreadcrumbs', {GetBreadcrumbsData(caption)});
              window.top.document.title += $('<div/>').html(' / {caption}').text();
        }});"));


        hdrTitle.InnerText = HTMLHelper.HTMLEncode(mContactDemographicsDataRetriever.GetCaption());
    }


    private string GetBreadcrumbsData(string caption)
    {
        var breadcrumbsList = new List<dynamic>
        {
            new
            {
                text = caption,
                isRoot = true
            }
        };

        return JsonConvert.SerializeObject(new { data = breadcrumbsList }, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
    }


    private void SetUpRetriever()
    {
        string retrieverIdentifier = mQueryParameters.Get("retrieverIdentifier");
        if (retrieverIdentifier == null)
        {
            throw new InvalidOperationException("retrieverIdentifier has to be set in the query string when using this user control");
        }

        mContactDemographicsDataRetriever = Service.Resolve<IContactDemographicsDataRetrieverFactory>().Get(retrieverIdentifier);

        try
        {
            mContacts = mContactDemographicsDataRetriever.GetContactObjectQuery(mQueryParameters);
        }
        catch (PermissionException e)
        {
            RedirectToAccessDenied(e.Message);
        }
    }


    private void RegisterJavascriptModules()
    {
        ScriptHelper.RegisterModule(Page, "CMS.ContactManagement/Demographics/module", new
        {
        });

        mCommonResourcesStrings = new Dictionary<string, string>
        {
            { "om.contact.demographics.graphicalrepresentation.nodata", GetString("om.contact.demographics.graphicalrepresentation.nodata") }
        };

        RegisterMap();
        RegisterGenderPieChart();
        RegisterPersonaPieChart();
        RegisterAgeChart();
    }


    private void RegisterMap()
    {
        ScriptHelper.RegisterModule(Page, "CMS.ContactManagement/Demographics/map", new
        {
            mapDiv = mapDiv.ClientID,
            parameters = mQueryParameters.ToString(),
            resourceStrings = new Dictionary<string, string>
            {
                { "map.backtoworldmap", GetString("map.backtoworldmap") }
            }
        });
    }


    private void RegisterPersonaPieChart()
    {
        if (ModuleEntryManager.IsModuleLoaded(ModuleName.PERSONAS))
        {
            RegisterChart("CMS.ContactManagement/Demographics/personaChart", personaChartDiv.ClientID);
        }
    }


    private void RegisterGenderPieChart()
    {
        RegisterChart("CMS.ContactManagement/Demographics/genderChart", genderChartDiv.ClientID);
    }


    private void RegisterAgeChart()
    {
        RegisterChart("CMS.ContactManagement/Demographics/ageChart", ageChartDiv.ClientID);
    }


    private void RegisterChart(string moduleName, string chartDiv)
    {
        ScriptHelper.RegisterModule(Page, moduleName, new
        {
            chartDiv,
            resourceStrings = mCommonResourcesStrings,
            parameters = mQueryParameters.ToString()
        });
    }


    private DataSet GridElem_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        var query = mContacts.Where(completeWhere).OrderBy(currentOrder).TopN(currentTopN).Columns(columns);
        query.Offset = currentOffset;
        query.MaxRecords = currentPageSize;
        totalRecords = query.TotalRecords;

        return query.Result;
    }
}