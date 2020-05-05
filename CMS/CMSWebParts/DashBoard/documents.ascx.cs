using System;

using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSWebParts_DashBoard_Documents : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the type of documents to display.
    /// </summary>
    public ListingTypeEnum ListingType
    {
        get
        {
            string listingType = ValidationHelper.GetString(GetValue("ListingType"), String.Empty).ToLowerCSafe();
            
            // Return correct listing type
            switch (listingType)
            {
                case "recent":
                    return ListingTypeEnum.RecentDocuments;

                case "pending":
                    return ListingTypeEnum.PendingDocuments;

                case "checkedout":
                    return ListingTypeEnum.CheckedOut;

                case "recyclebin":
                    return ListingTypeEnum.RecycleBin;

                case "outdated":
                    return ListingTypeEnum.OutdatedDocuments;

                case "workflow":
                    return ListingTypeEnum.WorkflowDocuments;

                case "pagetemplate":
                    return ListingTypeEnum.PageTemplateDocuments;

                case "tag":
                    return ListingTypeEnum.TagDocuments;

                case "category":
                    return ListingTypeEnum.CategoryDocuments;

                case "doctype":
                    return ListingTypeEnum.DocTypeDocuments;

                case "product":
                    return ListingTypeEnum.ProductDocuments;

                case "all":
                    return ListingTypeEnum.All;

                case "mydocuments":
                default:
                    return ListingTypeEnum.MyDocuments;
            }
        }
        set
        {
            string strListType = String.Empty;

            // Set correct listing type
            switch (value)
            {
                case ListingTypeEnum.All:
                    strListType = "all";
                    break;

                case ListingTypeEnum.CheckedOut:
                    strListType = "checkedout";
                    break;

                case ListingTypeEnum.MyDocuments:
                    strListType = "mydocuments";
                    break;

                case ListingTypeEnum.PendingDocuments:
                    strListType = "pending";
                    break;

                case ListingTypeEnum.RecentDocuments:
                    strListType = "recent";
                    break;

                case ListingTypeEnum.OutdatedDocuments:
                    strListType = "outdated";
                    break;

                case ListingTypeEnum.WorkflowDocuments:
                    strListType = "workflow";
                    break;

                case ListingTypeEnum.PageTemplateDocuments:
                    strListType = "pagetemplate";
                    break;

                case ListingTypeEnum.TagDocuments:
                    strListType = "tag";
                    break;

                case ListingTypeEnum.CategoryDocuments:
                    strListType = "category";
                    break;

                case ListingTypeEnum.DocTypeDocuments:
                    strListType = "doctype";
                    break;

                case ListingTypeEnum.ProductDocuments:
                    strListType = "product";
                    break;

                case ListingTypeEnum.RecycleBin:
                    strListType = "recyclebin";
                    break;
            }

            SetValue("ListingType", strListType);
        }
    }


    /// <summary>
    /// Gets or sets the site name. If is empty, documents from all sites are displayed.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SiteName"), String.Empty).Replace("##currentsite##", SiteContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }


    /// <summary>
    /// Gets or sets the order by condition.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(GetValue("OrderBy"), "DocumentModifiedWhen");
        }
        set
        {
            SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Gets or sets the sorting direction.
    /// </summary>
    public string Sorting
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Sorting"), "ASC");
        }
        set
        {
            SetValue("Sorting", value);
        }
    }


    /// <summary>
    /// Gets or sets the value of items per page.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ItemsPerPage"), "25");
        }
        set
        {
            SetValue("ItemsPerPage", value);
        }
    }


    /// <summary>
    /// Gets or sets the path filter for selected documents.
    /// </summary>
    public string Path
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Path"), String.Empty);
        }
        set
        {
            SetValue("Path", value);
        }
    }


    /// <summary>
    /// Gets or sets the age of documents in days.
    /// </summary>
    public string DocumentAge
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentAge"), String.Empty);
        }
        set
        {
            SetValue("DocumentAge", value);
        }
    }


    /// <summary>
    /// Gets or sets the document name filter value.
    /// </summary>
    public string DocumentName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentName"), String.Empty);
        }
        set
        {
            SetValue("DocumentName", value);
        }
    }


    /// <summary>
    /// Gets or sets the document type names which should be displayed in grid.
    /// </summary>
    public string DocumentType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DocumentType"), String.Empty);
        }
        set
        {
            SetValue("DocumentType", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            return;
        }

        ReloadData();
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        switch (ListingType)
        {
            case ListingTypeEnum.MyDocuments:
            case ListingTypeEnum.RecentDocuments:
            case ListingTypeEnum.PendingDocuments:
            case ListingTypeEnum.CheckedOut:
            case ListingTypeEnum.All:
                // Set Documents control
                ucDocuments.ListingType = ListingType;
                SetDocumentFilter();
                break;

            case ListingTypeEnum.RecycleBin:
                // Set Recycle bin control
                ucRecycle.Visible = true;
                ucRecycle.SiteName = SiteName;
                ucRecycle.DocumentAge = DocumentAge;
                ucRecycle.DocumentName = DocumentName;
                ucRecycle.ItemsPerPage = ItemsPerPage;
                ucRecycle.DocumentType = DocumentType;
                ucRecycle.RestrictUsers = false;

                // Transform OrderBy to proper column names of recycle bin
                string properOrderBy = String.Empty;
                switch (OrderBy.ToLowerCSafe())
                {
                    case "documentname":
                        properOrderBy = "VersionDocumentName";
                        break;

                    case "documentmodifiedwhen":
                        properOrderBy = "ModifiedWhen";
                        break;

                    case "type":
                        properOrderBy = "ClassName";
                        break;
                }

                if (properOrderBy != String.Empty)
                {
                    ucRecycle.OrderBy = properOrderBy + " " + Sorting;
                }

                ucRecycle.StopProcessing = false;
                break;
        }
    }


    /// <summary>
    /// Sets filter for document control (mydocuments,pending documents,approval,recent).
    /// </summary>
    private void SetDocumentFilter()
    {
        // Set Documents control
        ucDocuments.Visible = true;
        ucDocuments.Path = Path;
        ucDocuments.DocumentAge = DocumentAge;
        ucDocuments.DocumentName = DocumentName;
        ucDocuments.DocumentType = DocumentType;
        ucDocuments.ItemsPerPage = ItemsPerPage;
        ucDocuments.DisplayOnlyRunningSites = true;
        
        // Set OrderBy with sorting
        if ((ListingType == ListingTypeEnum.All) || ((ListingType != ListingTypeEnum.All) && (OrderBy != "type")))
        {
            ucDocuments.OrderBy = OrderBy + " " + Sorting;
        }
        ucDocuments.SiteName = SiteName;
        ucDocuments.StopProcessing = false;
        ucDocuments.IsLiveSite = ViewMode.IsLiveSite();
    }

    #endregion
}