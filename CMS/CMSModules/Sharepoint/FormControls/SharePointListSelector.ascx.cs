using System;
using System.Data;
using System.Linq;

using CMS.Core;
using CMS.EventLog;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SharePoint;
using CMS.SiteProvider;


/// <summary>
/// Form engine control for SharePoint list title selection
/// </summary>
public partial class CMSModules_SharePoint_FormControls_SharePointListSelector : FormEngineUserControl
{
    private const string SHAREPOINT_CONNECTION_ID = "SharePointConnectionID";
    private const string SHAREPOINT_LIST_TYPE = "SharePointListType";
    private const string SHOW_CONNECTION_WARNING = "ShowConnectionWarning";

    private int[] mIncludedListTypes;
    private int mSharePointConnectionID = -1;
    private int mSharePointListType = -1;
    private bool mShowConnectionWarning;


    /// <summary>
    /// Gets or sets the control's value
    /// </summary>
    public override object Value
    {
        get
        {
            return drpSharePointLists.Value;
        }
        set
        {
            drpSharePointLists.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the field from which the control can load SharePointConnectionID
    /// The field name is used to get SharePointConnectionID from the control's Form
    /// This property is ignored if a positive SharePointConnectionID is provided directly
    /// </summary>
    public string SharePointConnectionIDFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SharePointConnectionIDFieldName"), String.Empty);
        }
        set
        {
            SetValue("SharePointConnectionIDFieldName", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of the field from which the control can load SharePointListType
    /// The field name is used to get SharePointListType from the control's Form
    /// This property is ignored if a positive SharePointListType is provided directly
    /// </summary>
    public string SharePointListTypeFieldName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SharePointListTypeFieldName"), String.Empty);
        }
        set
        {
            SetValue("SharePointListTypeFieldName", value);
        }
    }


    /// <summary>
    /// Gets or sets types of SharePoint lists that should be offered in the combo box, when SharePointListType is 0 (All).
    /// Allows you to optionally constrain the types of lists to those, which are supported.
    /// When not set, lists of all types are included (as retrieved from the SharePoint server).
    /// </summary>
    public int[] IncludedListTypes
    {
        get
        {
            if (mIncludedListTypes == null)
            {
                string propertyValue = ValidationHelper.GetString(GetValue("IncludedListTypes"), String.Empty);
                int[] listTypes = ValidationHelper.GetIntegers(propertyValue.Split(','), -1);
                mIncludedListTypes = listTypes.Where(p => p > 0).ToArray();
            }

            return mIncludedListTypes;
        }
        set
        {
            mIncludedListTypes = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the connection used to retrieve the lists.
    /// If not provided before PreRender, Connection ID is loaded from the control's Form
    /// from the field whose name is stored in property SharePointConnectionIDFieldName.
    /// </summary>
    public int SharePointConnectionID
    {
        get
        {
            if (mSharePointConnectionID <= 0)
            {
                mSharePointConnectionID = GetIntFormForm(SharePointConnectionIDFieldName);
            }
            return mSharePointConnectionID;
        }
        set
        {
            mSharePointConnectionID = value;
        }
    }


    /// <summary>
    /// Gets or sets the property indicating whether the control is enabled or not.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return drpSharePointLists.Enabled;
        }
        set
        {
            drpSharePointLists.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets the type of lists to be retrieved.
    /// If value is '0' (All), lists of all types are included (optionally constrained by <see cref="IncludedListTypes"/>).
    /// If not provided before PreRender, list type is loaded from the control's Form
    /// from the field whose name is stored in property SharePointListTypeFieldName.
    /// </summary>
    public int SharePointListType
    {
        get
        {
            // mSharePointListType equal to '0' is a correct value.
            if (mSharePointListType < 0)
            {
                mSharePointListType = GetIntFormForm(SharePointListTypeFieldName);
            }
            return mSharePointListType;
        }
        set
        {
            mSharePointListType = value;
        }
    }


    /// <summary>
    /// Sets the control up.
    /// </summary>
    /// <param name="e">Ignored</param>
    protected override void OnPreRender(EventArgs e)
    {
        // Set this control and child controls up 
        SetUpControl();

        // Call pre-render on child controls (among others)
        base.OnPreRender(e);
    }


    /// <summary>
    /// Sets this control up and fills the combo box with options.
    /// </summary>
    private void SetUpControl()
    {
        if (StopProcessing || !Enabled)
        {
            //Do nothing
            return;
        }

        if (!HasConfigurationChanged())
        {
            // Configuration didn't change. Did this configuration cause warning?
            mShowConnectionWarning = ValidationHelper.GetBoolean(ViewState[SHOW_CONNECTION_WARNING], false);
        }
        else
        {
            // Configuration has changed. Re-load data and store the configuration.
            LoadListsFromSharePoint();
            StoreConfiguration();
        }

        if (mShowConnectionWarning)
        {
            ShowWarning(GetString("SharePoint.Library.GetListsConnectionError"));
        }
    }


    /// <summary>
    /// Loads options for combo box from SharePoint.
    /// Fills the options directly in the combo box.
    /// </summary>
    private void LoadListsFromSharePoint()
    {
        drpSharePointLists.DropDownList.Items.Clear();
        int connectionID = SharePointConnectionID;
        var connectionInfo = SharePointConnectionInfoProvider.GetSharePointConnectionInfo(connectionID);
        if (connectionInfo == null)
        {
            return;
        }

        DataSet lists = null;
        try
        {
            ISharePointListService service = SharePointServices.GetService<ISharePointListService>(connectionInfo.ToSharePointConnectionData());
            lists = service.GetLists(SharePointListType);
        }
        catch (Exception ex)
        {
            mShowConnectionWarning = true;
            Service.Resolve<IEventLogService>().LogWarning("SharePoint", "GETSHAREPOINTLISTS", ex, SiteContext.CurrentSiteID, String.Empty);
        }

        DataHelper.ForEachRow(lists, AddOptionToComboBoxFromRow);
    }


    /// <summary>
    /// Adds a new option to combo box based on a provided DataRow.
    /// Respects current settings of <see cref="IncludedListTypes"/>.
    /// </summary>
    /// <param name="row">Serves as a basis for the option to be added to the combo box. Must be a row representing a SharePoint list.</param>
    private void AddOptionToComboBoxFromRow(DataRow row)
    {
        int listType = ValidationHelper.GetInteger(row["BaseTemplate"], -1);
        if ((SharePointListType != CMS.SharePoint.SharePointListType.ALL) || !IncludedListTypes.Any()  || ((listType > 0) && IncludedListTypes.Contains(listType)))
        {
            string listTitle = ValidationHelper.GetString(row["Title"], null);
            if (!String.IsNullOrEmpty(listTitle))
            {
                drpSharePointLists.DropDownList.Items.Add(listTitle);
            }
        }
    }


    /// <summary>
    /// Stores configuration of the control to the viewstate.
    /// </summary>
    private void StoreConfiguration()
    {
        ViewState[SHAREPOINT_CONNECTION_ID] = SharePointConnectionID;
        ViewState[SHAREPOINT_LIST_TYPE] = SharePointListType;
        ViewState[SHOW_CONNECTION_WARNING] = mShowConnectionWarning;
    }


    /// <summary>
    /// Indicates whether the control's configuration has changed.
    /// </summary>
    /// <returns>True if the configuration has changed, false otherwise</returns>
    private bool HasConfigurationChanged()
    {
        int oldConnectionID = ValidationHelper.GetInteger(ViewState[SHAREPOINT_CONNECTION_ID], -1);
        int oldListType = ValidationHelper.GetInteger(ViewState[SHAREPOINT_LIST_TYPE], -1);

        return ((oldConnectionID != SharePointConnectionID) || (oldListType != SharePointListType));
    }


    /// <summary>
    /// Gets an integer from the control's form from a specified field.
    /// </summary>
    /// <param name="fieldName">Name of the field to get the integer value from.</param>
    /// <returns>Integer value of the form-field, -1 on failure</returns>
    private int GetIntFormForm(string fieldName)
    {
        if (Form != null)
        {
            return ValidationHelper.GetInteger(Form.GetFieldValue(fieldName), -1);
        }

        return -1;
    }
}