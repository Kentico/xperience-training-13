using System;
using System.Linq;
using System.Text;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;


/// <summary>
/// Displays selector of images predefined for the given object type.
/// The control is used in the ClassThumbnailSelector.aspx dialog page.
/// </summary>
public partial class CMSModules_AdminControls_Controls_Class_ClassThumbnailSelector_ClassThumbnailSelector : AbstractUserControl
{
    #region "Properties"

    /// <summary>
    /// CSS selector describing items in uniflat selector.
    /// </summary>
    public string ItemsCSSSelector
    {
        get
        {
            return flatElem.ItemsCSSSelector;
        }
    }

    #endregion


    #region "Page methods and events"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Set page size to 500, to disable paging when there are less than 500 images. Scrolling works here better than paging.
        flatElem.ManualPageSize = 500;
    }

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        flatElem.WhereCondition = new WhereCondition()
            .WhereEquals("MetaFileObjectType", "cms.class")
            .WhereEquals("MetaFileGroupName", ObjectAttachmentsCategories.THUMBNAIL)
            .WhereEquals("MetaFileObjectID", QueryHelper.GetInteger("classid", 0))
            .ToString(true);
        
        flatElem.OrderBy = "(CASE WHEN MetaFileName LIKE 'default%' THEN 1 ELSE 2 END), MetaFileTitle";
        
        // Select default item
        flatElem.SelectedItem = QueryHelper.GetString("metafileguid", "");
    }

    #endregion
}