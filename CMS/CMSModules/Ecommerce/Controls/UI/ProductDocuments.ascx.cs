using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_Controls_UI_ProductDocuments : CMSUserControl
{
    #region "Public properties"

    /// <summary>
    /// ID of the current product.
    /// </summary>
    public int ProductID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get current product info
        var product = SKUInfo.Provider.Get(ProductID);
        if (product != null)
        {
            // Allow site selector for global admins
            if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin))
            {
                filterDocuments.LoadSites = false;
                filterDocuments.SitesPlaceHolder.Visible = false;
            }

            // Get no data message text
            string productNameLocalized = ResHelper.LocalizeString(product.SKUName);
            string noDataMessage = String.Format(GetString("ProductDocuments.Documents.nodata"), HTMLHelper.HTMLEncode(productNameLocalized));
            if (filterDocuments.FilterIsSet)
            {
                noDataMessage = GetString("ProductDocuments.Documents.noresults");
            }
            else if (filterDocuments.SelectedSite != TreeProvider.ALL_SITES)
            {
                SiteInfo si = SiteInfo.Provider.Get(filterDocuments.SelectedSite);
                if (si != null)
                {
                    noDataMessage = string.Format(GetString("ProductDocuments.Documents.nodataforsite"), HTMLHelper.HTMLEncode(productNameLocalized), HTMLHelper.HTMLEncode(si.DisplayName));
                }
            }

            // Init documents control
            docElem.ZeroRowsText = noDataMessage;
            docElem.SiteName = filterDocuments.SelectedSite;
            docElem.UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
            docElem.UniGrid.OnAfterDataReload += UniGrid_OnAfterDataReload;
        }
    }

    #endregion


    #region "Grid events"

    protected void UniGrid_OnBeforeDataReload()
    {
        string where = "(SKUID=" + ProductID + " AND DocumentNodeID IN (SELECT NodeID FROM CMS_Tree WHERE NodeSKUID=" + ProductID + "))";
        where = SqlHelper.AddWhereCondition(where, filterDocuments.WhereCondition);
        docElem.UniGrid.WhereCondition = SqlHelper.AddWhereCondition(docElem.UniGrid.WhereCondition, where);
    }


    protected void UniGrid_OnAfterDataReload()
    {
        plcFilter.Visible = docElem.UniGrid.DisplayExternalFilter(filterDocuments.FilterIsSet);
    }

    #endregion
}