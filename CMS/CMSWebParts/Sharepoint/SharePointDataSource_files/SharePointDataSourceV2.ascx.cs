using System;
using System.Data;
using System.Net;

using CMS.Core;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.SharePoint;


public partial class CMSWebParts_SharePoint_SharePointDataSource_files_SharePointDatasourceV2 : CMSBaseDataSource
{
    #region "Enum"

    /// <summary>
    /// Mode in which the data source operates.
    /// </summary>
    public enum MODE
    {
        /// <summary>
        /// Listing of all lists or libraries (of certain type).
        /// </summary>
        LISTS,

        /// <summary>
        /// Listing of items of certain list or library.
        /// </summary>
        ITEMS
    }

    #endregion


    #region "Fields"

    private int mListType;
    private string mErrorMessage = null;
    private bool dataSourceLoaded;

    /// <summary>
    /// Whether the data has been loaded from the server or cache.
    /// </summary>
    private bool DataRetrievedFromServer;

    #endregion


    #region "Public properties"

    /// <summary>
    /// SharePoint connection info used for data retrieval.
    /// </summary>
    public SharePointConnectionInfo ConnectionInfo
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the mode the data source operates in.
    /// Use <see cref="ListType"/> to specify list type.
    /// </summary>
    public MODE Mode
    {
        get;
        set;
    }


    /// <summary>
    /// Type of SharePoint list the data source is working with.
    /// Zero by default.
    /// </summary>
    public int ListType
    {
        get
        {
            return mListType;
        }
        set
        {
            mListType = value;
        }
    }


    /// <summary>
    /// Title of the SharePoint list to retrieve items from.
    /// Required only when <see cref="Mode"/> is set to <see cref="MODE.ITEMS"/> mode.
    /// </summary>
    public string ListTitle
    {
        get;
        set;
    }


    /// <summary>
    /// Relative URL of listed folder (if list or library supports subfolders). Keep blank for root listing.
    /// Used only when <see cref="Mode"/> is set to <see cref="MODE.ITEMS"/> mode.
    /// </summary>
    public string FolderServerRelativeUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Configuration options for list items retrieval.
    /// Used only when <see cref="Mode"/> is set to <see cref="MODE.ITEMS"/> mode.
    /// </summary>
    public SharePointView View
    {
        get;
        set;
    }


    /// <summary>
    /// Constraint for selection of certain items.
    /// Used only when <see cref="Mode"/> is set to <see cref="MODE.ITEMS"/> mode.
    /// </summary>
    public SharePointListItemsSelection ListItemsSelection
    {
        get;
        set;
    }


    /// <summary>
    /// Enables displaying debugging information in the markup.
    /// </summary>
    public bool DebugEnabled
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    public override object DataSource
    {
        get
        {
            if (!dataSourceLoaded)
            {
                mDataSource = GetDataSource();
                dataSourceLoaded = true;
            }

            return mDataSource;
        }
        set
        {
            mDataSource = value;
            dataSourceLoaded = true;
        }
    }


    /// <summary>
    /// Gets the value that indicates whether data source contains selected item.
    /// Item can be selected only when <see cref="Mode"/> is set to <see cref="MODE.ITEMS"/> mode.
    /// To make a selection use <see cref="ListItemsSelection"/> property.
    /// </summary>
    public override bool IsSelected
    {
        get
        {
            return ((ListItemsSelection != null) && !String.IsNullOrEmpty(ListItemsSelection.FieldName) && !String.IsNullOrEmpty(ListItemsSelection.FieldType) && !String.IsNullOrEmpty(ListItemsSelection.FieldValue));
        }
        set
        {
            throw new InvalidOperationException("[SharePointDataSourceV2.IsSelected._set]: Can not set the property IsSelected. Specify ListItemsSelection instead.");
        }
    }


    /// <summary>
    /// Gets the error message if an error condition has been met.
    /// </summary>
    public string ErrorMessage
    {
        get
        {
            return mErrorMessage;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Gets data from SharePoint.
    /// </summary>
    /// <returns>Dataset</returns>
    protected override object GetDataSourceFromDB()
    {
        object data = null;

        try
        {
            data = GetSharePointData();
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("SharePointDataSource", "GetSharePointData", ex);
        }

        return data;
    }


    /// <summary>
    /// Retrieves requested data from SharePoint server.
    /// Displays error message if not able to do so and returns null.
    /// </summary>
    /// <returns>Requested data, or null if something goes wrong.</returns>
    private DataSet GetSharePointData()
    {
        // Data does not originate from cache
        DataRetrievedFromServer = true;
        DataSet result = null;

        if (ConnectionInfo == null)
        {
            DisplayError(GetString("sharepoint.wp.datasource.noconnection"));

            return null;
        }

        try
        {
            ISharePointListService listService = SharePointServices.GetService<ISharePointListService>(ConnectionInfo.ToSharePointConnectionData());

            if (Mode == MODE.LISTS)
            {
                result = listService.GetLists(ListType);
            }
            else
            {
                result = listService.GetListItems(ListTitle, FolderServerRelativeUrl, View, ListItemsSelection);
            }

            if (result != null && (result.Tables.Count == 0 || result.Tables[0].Columns.Count == 0))
            {
                result = null;
            }
        }
        catch (SharePointServiceFactoryNotSupportedException)
        {
            // No service factory for given SharePoint version
            DisplayError(GetString("sharepoint.versionnotsupported"));
        }
        catch (SharePointServiceNotSupportedException)
        {
            // No ISharePointListService implementation for SharePoint version
            DisplayError(GetString("sharepoint.listsnotsupported"));
        }
        catch (SharePointConnectionNotSupportedException)
        {
            // The ISharePointListService implementation rejected connection data
            DisplayError(GetString("sharepoint.invalidconfiguration"));
        }
        catch (SharePointCCSDKException ex)
        {
            var message = string.Format(GetString("sharepoint.ccsdk.idcrl.msoidclilerror"), DocumentationHelper.GetDocumentationTopicUrl("sharepoint_online_connecting"));
            DisplayError(message);
            Service.Resolve<IEventLogService>().LogException("SharePoint", "DATASOURCE", ex);
        }
        catch (WebException ex)
        {
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                // Connection established, but response indicates error condition
                if (ex.Message.Contains("401"))
                {
                    // Unauthorized.
                    DisplayError(GetString("sharepoint.protocolerror.unauthorized"));
                }
                else if (ex.Message.Contains("404"))
                {
                    // SharePoint instance not found on given URL
                    DisplayError(GetString("sharepoint.protocolerror.notfound"));
                }
                else
                {
                    // Some other protocol error
                    DisplayError(GetString("sharepoint.protocolerror"));
                }
            }
            else if (ex.Status == WebExceptionStatus.NameResolutionFailure)
            {
                // Given site URL does not have a resolution
                DisplayError(GetString("sharepoint.nameresolutionfailure"));
            }
            else
            {
                DisplayError(GetString("sharepoint.unknownerror"));
                Service.Resolve<IEventLogService>().LogException("SharePoint", "DATASOURCE", ex);
            }
        }
        catch (SharePointServerException ex)
        {
            DisplayError(String.Format(GetString("sharepoint.servererror"), HTMLHelper.HTMLEncode(ex.Message)));
            Service.Resolve<IEventLogService>().LogException("SharePoint", "DATASOURCE", ex);
        }
        catch (Exception ex)
        {
            DisplayError(GetString("sharepoint.unknownerror"));
            Service.Resolve<IEventLogService>().LogException("SharePoint", "DATASOURCE", ex);
        }

        return result;
    }


    /// <summary>
    /// Displays the error message.
    /// </summary>
    /// <param name="message">Message describing the error cause</param>
    private void DisplayError(string message)
    {
        mErrorMessage = message;
    }


    /// <summary>
    /// Renders the debugging information to the page if <see cref="DebugEnabled"/> is set.
    /// </summary>
    private void DisplayDebugInfo()
    {
        if (!DebugEnabled)
        {
            return;
        }

        plcDebug.Visible = true;
        if (DataSource != null)
        {
            ugDebug.Visible = true;
            ugDebug.DataSource = EncodeDataSet(DataSource);
            ugDebug.DataBind();
        }

        // Render this after accessing the DataSource.
        lblDebug.Text = (DataRetrievedFromServer) ? "Data retrieved from: SharePoint server" : "Data retrieved from: Cache" + "<br />";
        lblDebugError.Text = mErrorMessage + "<br />";
    }


    /// <summary>
    /// Transforms all DataSet's tables (column names and items) to string and HTML encodes them.
    /// Works with DataSource represented by DataSet only.
    /// </summary>
    /// <param name="dataSource">Data source to be encoded.</param>
    /// <returns>Encoded DataSet, or null if dataSource is not instance of DataSet.</returns>
    private DataSet EncodeDataSet(object dataSource)
    {
        DataSet dataSet = dataSource as DataSet;
        if (dataSet != null)
        {
            DataSet encodedDataSet = new DataSet(dataSet.DataSetName);
            foreach (DataTable table in dataSet.Tables)
            {
                DataTable encodedDataTable = new DataTable();
                encodedDataSet.Tables.Add(encodedDataTable);
                foreach (DataColumn column in table.Columns)
                {
                    encodedDataTable.Columns.Add(HTMLHelper.HTMLEncode(column.ColumnName));
                }

                foreach (DataRow row in table.Rows)
                {
                    DataRow encodedDataRow = encodedDataTable.NewRow();
                    int i = 0;
                    foreach (var item in row.ItemArray)
                    {
                        encodedDataRow[i++] = HTMLHelper.HTMLEncode(item.ToString());
                    }
                    encodedDataTable.Rows.Add(encodedDataRow);
                }
            }

            return encodedDataSet;
        }

        return null;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        DisplayDebugInfo();
    }

    #endregion
}
