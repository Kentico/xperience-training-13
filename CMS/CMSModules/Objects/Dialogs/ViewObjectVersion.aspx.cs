using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Synchronization;
using CMS.UIControls;

// Set default page title
public partial class CMSModules_Objects_Dialogs_ViewObjectVersion : CMSObjectVersioningPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check hash
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(GetString("dialogs.badhashtitle"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Even thought this page uses dialog master page, it always opens in new tab
        PageTitle.IsDialog = false;

        bool noCompare = ValidationHelper.GetBoolean(QueryHelper.GetString("nocompare", string.Empty), false);
        // Initialize view version control
        viewVersion.NoComparison = noCompare;
        int versionId = ValidationHelper.GetInteger(QueryHelper.GetString("versionhistoryid", string.Empty), 0);
        viewVersion.VersionCompareID = ValidationHelper.GetInteger(QueryHelper.GetString("comparehistoryid", string.Empty), 0);

        // Get version to initialize title
        ObjectVersionHistoryInfo version = ObjectVersionHistoryInfoProvider.GetVersionHistoryInfo(versionId);
        if (version != null)
        {
            string objectType = version.VersionObjectType;
            string objType = GetString("ObjectType." + objectType.Replace(".", "_"));
            string title = String.Format(GetString("objectversioning.viewversion.title"), objType, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(version.VersionObjectDisplayName)));
            // Set title
            SetTitle(title);

            viewVersion.Version = version;

            // Exclude site binding table data
            GeneralizedInfo infoObj = ModuleManager.GetReadOnlyObject(objectType);
            viewVersion.ExcludedTableNames = ObjectHelper.GetSerializationTableName(infoObj.TypeInfo.SiteBindingObject);
        }

        // Set what data should be displayed
        bool showAll = ValidationHelper.GetBoolean(QueryHelper.GetBoolean("showall", false), false);
        viewVersion.ObjectDataOnly = !showAll;
    }
}