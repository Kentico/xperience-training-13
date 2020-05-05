using System;
using System.Data;

using CMS.Base;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_Modules_Pages_Module_Sites : GlobalAdminPage
{
    protected int moduleId = 0;
    private string currentValues = String.Empty;


    protected void Page_Load(object sender, EventArgs e)
    {
        moduleId = QueryHelper.GetInteger("moduleId", 0);

        if (moduleId > 0)
        {
            var module = ResourceInfoProvider.GetResourceInfo(moduleId);
            if ((module != null) && (module.ResourceName.EqualsCSafe("cms", true)))
            {
                ShowInformation(GetString("resource.cmsmoduleassignedtoallsites"));
                headTitle.Visible = false;
                usSites.Visible = false;
                return;
            }

            // Get the active sites
            DataSet ds = ResourceSiteInfoProvider.GetResourceSites().WhereEquals("ResourceID", moduleId).Column("SiteID");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                currentValues = TextHelper.Join(";", DataHelper.GetStringValues(ds.Tables[0], "SiteID"));
            }

            if (!RequestHelper.IsPostBack())
            {
                usSites.Value = currentValues;
            }
        }

        usSites.OnSelectionChanged += usSites_OnSelectionChanged;
    }


    protected void usSites_OnSelectionChanged(object sender, EventArgs e)
    {
        SaveSites();
    }


    protected void SaveSites()
    {
        // Remove old items
        string newValues = ValidationHelper.GetString(usSites.Value, null);
        string items = DataHelper.GetNewItemsInList(newValues, currentValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int siteId = ValidationHelper.GetInteger(item, 0);

                    // Remove
                    ResourceSiteInfoProvider.RemoveResourceFromSite(moduleId, siteId);
                }
            }
        }

        // Add new items
        items = DataHelper.GetNewItemsInList(currentValues, newValues);
        if (!String.IsNullOrEmpty(items))
        {
            string[] newItems = items.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (newItems != null)
            {
                // Add all new items to site
                foreach (string item in newItems)
                {
                    int siteId = ValidationHelper.GetInteger(item, 0);

                    // Add
                    ResourceSiteInfoProvider.AddResourceToSite(moduleId, siteId);
                }
            }
        }

        // Show message
        ShowChangesSaved();
    }
}