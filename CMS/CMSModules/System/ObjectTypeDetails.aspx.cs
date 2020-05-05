using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

public partial class CMSModules_System_ObjectTypeDetails : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string objectType = QueryHelper.GetString("objectType", String.Empty);

        GenerateBreadcrumbs(objectType);

        // Attempts to get the TypeInfo for the object type, shows an error if not found
        var typeInfo = ObjectTypeManager.GetTypeInfo(objectType);
        if (typeInfo == null)
        {
            ShowError(String.Format(GetString("administration.system.objecttypegraph.notfound"), HTMLHelper.HTMLEncode(objectType)));
            return;
        }

        InitControls(objectType, typeInfo);

        ScriptHelper.RegisterClientScriptInclude(Page, typeof(string), "ObjectTypeVisGraph", URLHelper.ResolveUrl("~/CMSScripts/vis/vis-network.min.js"));        

        // Sets up and registers the ObjectTypeGraph JavaScript module
        object data = new
        {
            objectType = objectType,
            filterId = chlGraphFilter.ClientID,
            networkId = pnlObjectTypeGraph.ClientID
        };
        ScriptHelper.RegisterModule(this, "CMS/ObjectTypeGraph", data);
    }


    private void GenerateBreadcrumbs(string objectType)
    {
        UIElementInfo elementObjectTypes = UIElementInfoProvider.GetUIElementInfo("cms", "objecttypes");
        string elementObjectTypesUrl = UIContextHelper.GetElementUrl(elementObjectTypes, UIContext);
        elementObjectTypesUrl = URLHelper.AppendQuery(elementObjectTypesUrl, "displaytitle=false");

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            RedirectUrl = elementObjectTypesUrl
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = objectType
        });
    }


    private void InitControls(string objectType, ObjectTypeInfo typeInfo)
    {
        plcMain.Visible = true;

        lblObjectTypeNameValue.Text = TypeHelper.GetNiceObjectTypeName(objectType);
        lblObjectTypeValue.Text = HTMLHelper.HTMLEncode(objectType);

        var dataClass = DataClassInfoProvider.GetDataClassInfo(typeInfo.ObjectClassName);
        lblDBTableValue.Text = dataClass != null ? dataClass.ClassTableName : GetString("general.none");
        lblSiteRelationshipValue.Text = GetSiteRelationshipTypeDescription(typeInfo.IsSiteObject, typeInfo.SupportsGlobalObjects);
        lblIsBindingValue.Text = typeInfo.IsBinding ? GetString("general.yes") : GetString("general.no");
        lblSupportsCIValue.Text = typeInfo.ContinuousIntegrationSettings.Enabled ? GetString("general.yes") : GetString("general.no");

        if (!typeInfo.IsBinding)
        {
            headingObjectTypeGraph.ResourceString = "administration.system.objecttypes.hierarchy";

            lblGraphFilter.Visible = true;
            chlGraphFilter.Visible = true;

            AddGraphFilterOption("administration.system.objecttypegraph.parentobjects", "1");
            AddGraphFilterOption("administration.system.objecttypegraph.childobjects", "2");
            AddGraphFilterOption("administration.system.objecttypegraph.bindings", "4");
            AddGraphFilterOption("administration.system.objecttypegraph.otherbindings", "8");
        }
        else
        {
            headingObjectTypeGraph.ResourceString = "administration.system.objecttypes.bindingrelationships";
        }
    }


    private void AddGraphFilterOption(string resourceString, string optionValue)
    {
        chlGraphFilter.Items.Add(new ListItem(GetString(resourceString), optionValue) { Selected = true });
    }


    private string GetSiteRelationshipTypeDescription(bool siteObject, bool supportsGlobalObjects)
    {
        if (!siteObject)
        {
            return GetString("administration.system.objecttypes.siterelation.global");
        }

        if (supportsGlobalObjects)
        {
            return GetString("administration.system.objecttypes.siterelation.globalorsite");
        }

        return GetString("administration.system.objecttypes.siterelation.site");
    }
}
