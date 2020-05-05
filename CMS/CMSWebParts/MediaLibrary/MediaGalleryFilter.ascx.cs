using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_MediaLibrary_MediaGalleryFilter : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the source filter name.
    /// </summary>
    public string FilterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FilterName"), String.Empty);
        }
        set
        {
            SetValue("FilterName", value);
            libSort.SourceFilterName = value;
        }
    }


    /// <summary>
    /// Gets or sets the file id querysting parameter.
    /// </summary>
    public string FileIDQueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FileIDQueryStringKey"), String.Empty);
        }
        set
        {
            SetValue("FileIDQueryStringKey", value);
            libSort.FileIDQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the sort querysting parameter.
    /// </summary>
    public string SortQueryStringKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SortQueryStringKey"), String.Empty);
        }
        set
        {
            SetValue("SortQueryStringKey", value);
            libSort.SortQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the filter method.
    /// </summary>
    public int FilterMethod
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("FilterMethod"), 0);
        }
        set
        {
            SetValue("FilterMethod", value);
            libSort.FilterMethod = value;
        }
    }

    #endregion


    protected override void OnInit(EventArgs e)
    {
        // Add sort to the filter collection
        CMSControlsHelper.SetFilter(ValidationHelper.GetString(GetValue("WebPartControlID"), ID), libSort);

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int fid = QueryHelper.GetInteger("fid", 0);
        if (fid > 0)
        {
            libSort.StopProcessing = true;
            libSort.Visible = false;
        }
        libSort.SourceFilterName = FilterName;
        libSort.FileIDQueryStringKey = FileIDQueryStringKey;
        libSort.SortQueryStringKey = SortQueryStringKey;
        libSort.FilterMethod = FilterMethod;
    }
}