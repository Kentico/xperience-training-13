using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.MediaLibrary.Internal;
using CMS.MediaLibrary.Web.UI.Internal;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_MediaLibrary_MediaFileUsage : CMSAdminControl
{
    /// <summary>
    /// Page init event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        UsageGrid.GridOptions.DisplayFilter = false;
        UsageGrid.PagerConfig.ShowPageSize = false;
        UsageGrid.PagerConfig.DisplayPager = false;
        UsageGrid.ApplyPageSize = false;
        UsageGrid.OrderBy = "UsageObjectName";
        UsageGrid.ZeroRowsText = String.Empty;

        ltlUsageLoading.Text = ScriptHelper.GetLoaderInlineHtml();
        UsageGrid.Visible = false;
    }


    /// <summary>
    /// Sets up usage grid.
    /// </summary>
    public void Setup(IEnumerable<IMediaFileUsageSearchResult> usages)
    {
        UsageGrid.Visible = true;
        ltlUsageLoading.Text = String.Empty;

        UsageGrid.DataSource = PrepareUsageDataSource(usages);
        UsageGrid.OnExternalDataBound += UsageGrid_OnExternalDataBound;
        UsageGrid.ReloadData();
    }


    /// <summary>
    /// Prepare usage data set from given usages list.
    /// </summary>
    /// <param name="usagesList">List of usages.</param>
    private DataSet PrepareUsageDataSource(IEnumerable<IMediaFileUsageSearchResult> usagesList)
    {
        DataSet ds = new DataSet("UsagesDS");
        DataTable dt = new DataTable("UsagesDT");

        dt.Columns.Add(new DataColumn("UsageObjectEditUrl", typeof(string)));
        dt.Columns.Add(new DataColumn("UsageObjectName", typeof(string)));
        dt.Columns.Add(new DataColumn("UsageObjectType", typeof(string)));
        dt.Columns.Add(new DataColumn("UsageLocation", typeof(string)));
        dt.Columns.Add(new DataColumn("UsageSiteName", typeof(string)));
        var editUrlRetriever = new MediaFileUsageEditUrlRetriever();

        foreach (var usage in usagesList)
        {
            var row = dt.NewRow();

            row["UsageObjectEditUrl"] = editUrlRetriever.Get(usage);
            row["UsageObjectName"] = GetObjectDisplayName(usage);
            row["UsageObjectType"] = GetObjectTypeDisplayName(usage);
            row["UsageLocation"] = GetUsageLocation(usage);
            row["UsageSiteName"] = GetSiteDisplayName(usage);

            dt.Rows.Add(row);
        }

        ds.Tables.Add(dt);

        return ds;
    }


    private static string GetSiteDisplayName(IMediaFileUsageSearchResult usage)
    {
        var site = usage.ObjectSiteID > 0 ? SiteInfo.Provider.Get(usage.ObjectSiteID) : null;

        return site?.DisplayName ?? ResHelper.GetString("general.global");
    }


    private string GetUsageLocation(IMediaFileUsageSearchResult usage)
    {
        if (!usage.ObjectType.Equals(PredefinedObjectType.DOCUMENT, StringComparison.InvariantCultureIgnoreCase))
        {
            return "-";
        }

        var location = ResHelper.GetString(usage.Location.ToStringRepresentation());
        var version = ResHelper.GetString(usage.Version.ToStringRepresentation());

        return $"{location} - {version}";
    }


    private string GetObjectTypeDisplayName(IMediaFileUsageSearchResult usage)
    {
        if (usage.ObjectType.StartsWith(PredefinedObjectType.CUSTOM_TABLE_ITEM_PREFIX, StringComparison.InvariantCultureIgnoreCase))
        {
            var info = ModuleManager.GetReadOnlyObject(usage.ObjectType);
            var classDisplayName = DataClassInfoProvider.GetDataClassInfo(info.TypeInfo.ObjectClassName).ClassDisplayName;
            return string.Format(ResHelper.GetString("medialibrary.dependencytracker.customtablename"), ResHelper.LocalizeString(classDisplayName));
        }

        return TypeHelper.GetNiceObjectTypeName(usage.ObjectType);
    }


    private string GetObjectDisplayName(IMediaFileUsageSearchResult usage)
    {
        if (usage.ObjectType.StartsWith(PredefinedObjectType.CUSTOM_TABLE_ITEM_PREFIX, StringComparison.InvariantCultureIgnoreCase))
        {
            return string.Format(ResHelper.GetString("medialibrary.dependencytracker.customtableitem"), usage.ObjectID);
        }

        return ResHelper.LocalizeString(usage.ObjectDisplayName);
    }


    private object UsageGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerInvariant())
        {
            case "edit":
                {
                    var row = (DataRowView)((GridViewRow)parameter).DataItem;
                    var editUrl = DataHelper.GetStringValue(row.Row, "UsageObjectEditUrl");
                    var editButton = (CMSGridActionButton)sender;
                    if (!string.IsNullOrEmpty(editUrl))
                    {
                        editButton.OnClientClick = $"window.open(\"{editUrl}\"); return false;";
                    }
                    else
                    {
                        editButton.Enabled = false;
                    }
                }
                break;
        }
        return parameter;
    }
}