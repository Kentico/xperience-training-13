using System;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_Documents : GlobalAdminPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        filterDocuments.ClassPlaceHolder.Visible = false;

        docElem.SiteName = filterDocuments.SelectedSite;
        docElem.UniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;
        docElem.UniGrid.OnAfterDataReload += UniGrid_OnAfterDataReload;
    }

    #endregion


    #region "Grid events"

    protected void UniGrid_OnBeforeDataReload()
    {
        // Get current page template ID
        int objectId = QueryHelper.GetInteger("objectId", 0);

        string where = "ClassName IN (SELECT ClassName FROM CMS_Class WHERE ClassID =" + objectId + ")";
        where = SqlHelper.AddWhereCondition(where, filterDocuments.WhereCondition);
        docElem.UniGrid.WhereCondition = SqlHelper.AddWhereCondition(docElem.UniGrid.WhereCondition, where);
    }


    protected void UniGrid_OnAfterDataReload()
    {
        bool displayFilter = docElem.UniGrid.DisplayExternalFilter(filterDocuments.FilterIsSet);
        plcFilter.Visible = displayFilter;
        
        // Display title if unigrid is not empty
        plcTitle.Visible = !DataHelper.DataSourceIsEmpty(docElem.UniGrid.GridView.DataSource);
        
        if (displayFilter)
        {
            // If filter is visible, ZeroRowsText will be default FilteredZeroRowsText from unigrid.
            docElem.ZeroRowsText = docElem.UniGrid.FilteredZeroRowsText;
        }
        else
        {
            // If filter is not visible, ZeroRowsText will say, that DocumentType is not used for any document. 
            docElem.ZeroRowsText = GetString("documenttype_edit_general.documents.nodata");
        }
    }

    #endregion
}