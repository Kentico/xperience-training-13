using System;
using System.Linq;

using CMS.Base;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.SharePoint;


public partial class CMSWebParts_SharePoint_SharePointDataSourceV2 : CMSAbstractWebPart
{
    #region "Fields"

    private SharePointConnectionInfo mConnectionInfo;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Specifies SharePointConnectionInfo's ID to be used for data retrieval.
    /// </summary>
    public int ConnectionID
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ConnectionID"), 0);
        }
        set
        {
            SetValue("ConnectionID", value);
        }
    }


    /// <summary>
    /// Determines what type of SharePoint list do you want to work with.
    /// </summary>
    public int ListType
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ListType"), 0);
        }
        set
        {
            SetValue("ListType", value);
        }
    }


    /// <summary>
    /// Specifies whether all lists / libraries of given type will be retrieved or just items of particular list / library.
    /// </summary>
    public string Mode
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Mode"), "");
        }
        set
        {
            SetValue("Mode", value);
        }
    }


    /// <summary>
    /// Name of the list or library on SharePoint server from which the items should be loaded.
    /// Needed only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public string ListName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ListName"), "");
        }
        set
        {
            SetValue("ListName", value);
        }
    }


    /// <summary>
    /// Internal name of SharePoint column to be used for item(s) selection.
    /// Allows you to select certain item(s) of all items that would be retrieved (with respect to optional Query clause) from the SharePoint server.
    /// Can be used only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public string SelectionFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectionFieldName"), "");
        }
        set
        {
            SetValue("SelectionFieldName", value);
        }
    }


    /// <summary>
    /// Type of column specified in Field name (i.e. "Text", "Integer", "Counter" for IDs, etc.).
    /// Can be used only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public string SelectionFieldType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectionFieldType"), "");
        }
        set
        {
            SetValue("SelectionFieldType", value);
        }
    }


    /// <summary>
    /// Value used in equality comparison for item(s) selection.
    /// Can be used only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public string SelectionFieldValue
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectionFieldValue"), "");
        }
        set
        {
            SetValue("SelectionFieldValue", value);
        }
    }


    /// <summary>
    /// Specify server relative URL of SharePoint picture library folder or keep blank for listing root folder.
    /// Can be used only when listing of Picture library items is specified in <see cref="ListType"/> and <see cref="Mode"/>.
    /// </summary>
    public string FolderRelativeUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FolderRelativeUrl"), "");
        }
        set
        {
            SetValue("FolderRelativeUrl", value);
        }
    }


    /// <summary>
    /// Allows you to specify whether to retrieve files and folders or just files. Inclusion of subfolders can be specified as well.
    /// Known values are Default, FilesOnly, RecursiveAll and Recursive.
    /// Can be used only when listing of Picture library items is specified in <see cref="ListType"/> and <see cref="Mode"/>.
    /// </summary>
    public string Scope
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Scope"), "");
        }
        set
        {
            SetValue("Scope", value);
        }
    }


    /// <summary>
    /// Sets the maximum number of items (data rows) to be retrieved.
    /// Can be used only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public int RowLimit
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("RowLimit"), 0);
        }
        set
        {
            SetValue("RowLimit", value);
        }
    }


    /// <summary>
    /// Query.
    /// Can be used only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public string Query
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Query"), "");
        }
        set
        {
            SetValue("Query", value);
        }
    }


    /// <summary>
    /// Can be used to specify SharePoint list or library field names (internal names separated by semicolon) to be retrieved. Keep blank for retrieval of all fields. Note: Certain fields will always be retrieved.
    /// Can be used only when listing of items is specified in <see cref="Mode"/>.
    /// </summary>
    public string ViewFields
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ViewFields"), "");
        }
        set
        {
            SetValue("ViewFields", value);
        }
    }


    /// <summary>
    /// Gets or sets the cache minutes.
    /// </summary>
    public override int CacheMinutes
    {
        get
        {
            return base.CacheMinutes;
        }
        set
        {
            base.CacheMinutes = value;
            SPDataSource.CacheMinutes = value;
        }
    }


    /// <summary>
    /// Gets or sets the cache item name.
    /// </summary>
    public override string CacheItemName
    {
        get
        {
            return base.CacheItemName;
        }
        set
        {
            base.CacheItemName = value;
            SPDataSource.CacheItemName = value;
        }
    }


    /// <summary>
    /// Cache dependencies, each cache dependency on a new line.
    /// </summary>
    public override string CacheDependencies
    {
        get
        {
            return ValidationHelper.GetString(base.CacheDependencies, SPDataSource.CacheDependencies);
        }
        set
        {
            base.CacheDependencies = value;
            SPDataSource.CacheDependencies = value;
        }
    }


    /// <summary>
    /// Causes the web part to display information suitable for debugging.
    /// </summary>
    public bool DebugEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DebugEnabled"), false);
        }
        set
        {
            SetValue("DebugEnabled", value);
        }
    }


    /// <summary>
    /// Gets the value that indicates whether data source contains selected item.
    /// Item can be selected only in modes for listing items.
    /// Use <see cref="SelectionFieldName"/>, <see cref="SelectionFieldType"/> and <see cref="SelectionFieldValue"/> to specify a selection.
    /// </summary>
    public bool IsSelected
    {
        get
        {
            return SPDataSource.IsSelected;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            SPDataSource.StopProcessing = value;
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets SharePointConnectionInfo specified in web part settings via <see cref="ConnectionID"/> property.
    /// </summary>
    private SharePointConnectionInfo ConnectionInfo
    {
        get
        {
            return mConnectionInfo ?? (mConnectionInfo = SharePointConnectionInfoProvider.GetSharePointConnectionInfo(ConnectionID));
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
        
        // Set datasource control filter name = this webpart name
        SPDataSource.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);

        SPDataSource.ConnectionInfo = ConnectionInfo;
        SPDataSource.DebugEnabled = DebugEnabled;
        SPDataSource.FolderServerRelativeUrl = FolderRelativeUrl;
        if (!String.IsNullOrEmpty(SelectionFieldName) && !String.IsNullOrEmpty(SelectionFieldType) && !String.IsNullOrEmpty(SelectionFieldValue))
        {
            SPDataSource.ListItemsSelection = new SharePointListItemsSelection(SelectionFieldName, SelectionFieldType, SelectionFieldValue);
        }
        SPDataSource.ListTitle = ListName;
        SPDataSource.ListType = ListType;
        SPDataSource.Mode = String.Equals(Mode, "items", StringComparison.OrdinalIgnoreCase) ? CMSWebParts_SharePoint_SharePointDataSource_files_SharePointDatasourceV2.MODE.ITEMS : CMSWebParts_SharePoint_SharePointDataSource_files_SharePointDatasourceV2.MODE.LISTS;
        SPDataSource.View = GetView();
        SPDataSource.CacheMinutes = CacheMinutes;
        if (SPDataSource.ListItemsSelection == null || !String.IsNullOrEmpty(CacheItemName))
        {
            // Keep default cache item name if no selection has been made or cache item name explicitly set
            SPDataSource.CacheItemName = CacheItemName;
        }
        else
        {
            // Explicitly include selection in cache item name otherwise
            CacheItemName = RequestContext.URL + "|" + ClientID + "|" + SelectionFieldName + "|" + SelectionFieldValue;
        }
        SPDataSource.CacheDependencies = CacheDependencies;
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    /// <summary>
    /// Gets View based on <see cref="Query"/>, <see cref="Scope"/>, <see cref="RowLimit"/> and <see cref="ViewFields"/> properties.
    /// </summary>
    /// <returns>View.</returns>
    private SharePointView GetView()
    {
        SharePointView view = new SharePointView
        {
            QueryInnerXml = Query,
            Scope = Scope,
            RowLimit = RowLimit
        };
        view.ViewFields = (ViewFields != null) ? ViewFields.Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries).ToList() : null;

        return view;
    }

    #endregion
}
