using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_TagGroups_Pages_Development_Tags_Documents : GlobalAdminPage
{
    #region "Variables"

    private int mTagId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current tag ID
        mTagId = QueryHelper.GetInteger("tagid", 0);
        TagInfo ti = TagInfoProvider.GetTagInfo(mTagId);
        EditedObject = ti;

        if (ti != null)
        {
            int groupId = QueryHelper.GetInteger("groupid", 0);
            int siteId = QueryHelper.GetInteger("siteid", 0);

            UIElementInfo ui = UIElementInfoProvider.GetUIElementInfo("CMS.Taxonomy", "tags");
            String url = String.Empty;
            if (ui != null)
            {
                url = UIContextHelper.GetElementUrl(ui, UIContext);
                url += String.Format("&parentobjectid={0}&tagid={1}&siteid={2}&displaytitle={3}", groupId, mTagId, siteId, QueryHelper.GetBoolean("displaytitle", false));
            }

            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = GetString("taggroup_edit.itemlistlink"),
                RedirectUrl = url
            });

            PageBreadcrumbs.Items.Add(new BreadcrumbItem()
            {
                Text = ti.TagName
            });

            docElem.SiteName = filterDocuments.SelectedSite;
            docElem.UniGrid.OnBeforeDataReload += new OnBeforeDataReload(UniGrid_OnBeforeDataReload);
            docElem.UniGrid.OnAfterDataReload += new OnAfterDataReload(UniGrid_OnAfterDataReload);
        }
    }

    #endregion


    #region "Grid events"

    protected void UniGrid_OnBeforeDataReload()
    {
        string where = "(DocumentID IN (SELECT CMS_DocumentTag.DocumentID FROM CMS_DocumentTag WHERE TagID = " + mTagId + "))";
        where = SqlHelper.AddWhereCondition(where, filterDocuments.WhereCondition);
        docElem.UniGrid.WhereCondition = SqlHelper.AddWhereCondition(docElem.UniGrid.WhereCondition, where);
    }


    protected void UniGrid_OnAfterDataReload()
    {
        plcFilter.Visible = docElem.UniGrid.DisplayExternalFilter(filterDocuments.FilterIsSet);
    }

    #endregion
}