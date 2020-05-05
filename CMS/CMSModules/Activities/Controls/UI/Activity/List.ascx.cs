using System;
using System.Collections;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;


public partial class CMSModules_Activities_Controls_UI_Activity_List : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables"

    private Hashtable mParameters;
    private int mPageSize = -1;
    private bool modifyPermission;
    private int ActivityID;
    private CMSModules_Activities_Controls_UI_Activity_Filter filter;
    private bool mShowSelection = true;


    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        Delete = 1
    }


    /// <summary>
    /// Selected objects in mass action selector.
    /// </summary>
    protected enum What
    {
        Selected = 0,
        All = 1
    }


    /// <summary>
    /// URL of the page for contact deletion.
    /// </summary>
    protected const string DELETE_PAGE = "~/CMSModules/Activities/Pages/Tools/Activities/Activity/Delete.aspx";

    #endregion


    #region "Properties"

    /// <summary>
    /// Get or sets additional WHERE condition.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets ORDER BY.
    /// </summary>
    public string OrderBy
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return mPageSize;
        }
        set
        {
            mPageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets limit of the unigrid filter
    /// </summary>
    public int FilterLimit
    {
        get
        {
            return gridElem.FilterLimit;
        }
        set
        {
            gridElem.FilterLimit = value;
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
            gridElem.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// True if site name column is visible.
    /// </summary>
    public bool ShowSiteNameColumn
    {
        get;
        set;
    }


    /// <summary>
    /// True if remove action button is visible.
    /// </summary>
    public bool ShowRemoveButton
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets contact ID
    /// </summary>
    public int ContactID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets info text for empty list.
    /// </summary>
    public string ZeroRowsText
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets info text for empty filtered list.
    /// </summary>
    public string FilteredZeroRowsText
    {
        get;
        set;
    }


    /// <summary>
    /// Activities with given site id will be displayed.
    /// If <see cref="UniSelector.US_GLOBAL_RECORD"/> is supplied, then only activities without site will be displayed.
    /// By default <see cref=SiteContext.CurrentSite/> is used.
    /// </summary>
    /// <remarks>
    /// If value of this property is less than 1 and does not equal to <see cref="UniSelector.US_GLOBAL_RECORD"/>, then all activities are displayed.
    /// </remarks>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// If true, column allowing the selection of rows is displayed on the left of the UniGrid,
    /// that can be used to perform mass action like delete. By default is true.
    /// </summary>
    public bool ShowSelection
    {
        get
        {
            return mShowSelection;
        }

        set
        {
            mShowSelection = value;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private Guid Identifier
    {
        get
        {
            Guid identifier;
            if (!Guid.TryParse(hdnIdentifier.Value, out identifier))
            {
                identifier = Guid.NewGuid();
                hdnIdentifier.Value = identifier.ToString();
            }

            return identifier;
        }
    }


    /// <summary>
    /// Gets or sets the callback argument.
    /// </summary>
    private string CallbackArgument
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        CallbackArgument = eventArgument;
        ActivityID = ValidationHelper.GetInteger(eventArgument, 0);
    }


    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        mParameters = new Hashtable();
        string queryString = null;
        mParameters["returnlocation"] = RequestContext.CurrentURL;
        mParameters["contactid"] = ContactID;

        // ...for mass action
        if (CallbackArgument.StartsWithCSafe("massaction;", true))
        {
            // Get values of callback argument
            string[] selection = CallbackArgument.Split(new[]
            {
                ";"
            }, StringSplitOptions.RemoveEmptyEntries);
            if (selection.Length != 3)
            {
                return null;
            }

            Action action = (Action)ValidationHelper.GetInteger(selection[2], 0);
            switch (action)
            {
                case Action.Delete:
                    mParameters["where"] = GetWhereCondition(selection[1]);
                    break;
                default:
                    return null;
            }

            WindowHelper.Add(Identifier.ToString(), mParameters);
            queryString = "?params=" + Identifier;
            queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));

            return queryString;
        }

        mParameters["where"] = gridElem.WhereCondition;
        string sortDirection = gridElem.SortDirect;
        if (String.IsNullOrEmpty(sortDirection))
        {
            sortDirection = gridElem.OrderBy;
        }
        mParameters["orderby"] = sortDirection;

        WindowHelper.Add(Identifier.ToString(), mParameters);

        queryString = "?params=" + Identifier;
        queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        queryString = URLHelper.AddParameterToUrl(queryString, "activityid", ActivityID.ToString());

        return queryString;
    }


    protected override void OnInit(EventArgs e)
    {
        SiteID = SiteContext.CurrentSiteID;
        gridElem.OnFilterFieldCreated += gridElem_OnFilterFieldCreated;
        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (PageSize >= 0)
        {
            gridElem.Pager.DefaultPageSize = PageSize;
        }
        gridElem.OrderBy = OrderBy;
        gridElem.WhereCondition = WhereCondition;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.GridOptions.ShowSelection = ShowSelection;

        // Set gridelement empty list info text either to set property or default value
        gridElem.ZeroRowsText = String.IsNullOrEmpty(ZeroRowsText) ? GetString("om.activities.noactivities") : GetString(ZeroRowsText);
        gridElem.FilteredZeroRowsText = String.IsNullOrEmpty(FilteredZeroRowsText) ? GetString("om.activities.noactivities.filtered") : GetString(FilteredZeroRowsText);

        if (ContactID > 0)
        {
            gridElem.ObjectType = ActivityInfo.OBJECT_TYPE;
            gridElem.WhereCondition = new WhereCondition(gridElem.WhereCondition).WhereEquals(nameof(ActivityInfo.ActivityContactID), ContactID).ToString(true);
        }
        else
        {
            gridElem.ObjectType = ActivityListInfo.OBJECT_TYPE;
            gridElem.GridColumns.Columns.Insert(2, new Column { Caption = GetString("om.contact.firstname"), Source = nameof(ContactInfo.ContactFirstName), Wrap = false });
            gridElem.GridColumns.Columns.Insert(3, new Column { Caption = GetString("om.contact.middlename"), Source = nameof(ContactInfo.ContactMiddleName), Wrap = false });
            gridElem.GridColumns.Columns.Insert(4, new Column { Caption = GetString("om.contact.lastname"), Source = nameof(ContactInfo.ContactLastName), Wrap = false });
            gridElem.Columns += ",ContactFirstName, ContactMiddleName, ContactLastName";
        }

        modifyPermission = AuthorizationHelper.AuthorizedManageActivity(SiteContext.CurrentSiteID, false);

        ScriptHelper.RegisterDialogScript(Page);

        string scriptBlock = string.Format(@"
            function ViewDetails(id) {{ modalDialog('{0}' + id, 'ActivityDetails', '900', '950'); return false; }}",
            ResolveUrl(@"~/CMSModules/Activities/Pages/Tools/Activities/Activity/Activity_Details.aspx"));

        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "Actions", scriptBlock, true);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (filter != null)
        {            
            filter.ShowContactSelector = ContactID < 1;
            filter.ShowSiteFilter = ShowSiteNameColumn;
            filter.SiteID = SiteID;

            ShowSiteNameColumn = !filter.IsUniGridFilteredBySite();
        }

        gridElem.NamedColumns["sitename"].Visible = ShowSiteNameColumn;

        // Create mass actions dropdowns
        if (gridElem.RowsCount > 0 && ShowRemoveButton && modifyPermission)
        {
            InitMassActionDDs();
            pnlFooter.Visible = true;
        }
    }


    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "view":
                CMSGridActionButton viewBtn = (CMSGridActionButton)sender;
                viewBtn.OnClientClick = string.Format("dialogParams_{0} = '{1}';{2};return false;",
                    ClientID,
                    viewBtn.CommandArgument, Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "ViewDetails", null));
                break;
            case "delete":
                CMSGridActionButton deleteBtn = (CMSGridActionButton)sender;
                if (!ShowRemoveButton || !modifyPermission)
                {
                    deleteBtn.Enabled = false;
                }
                deleteBtn.Visible = ShowRemoveButton;
                break;
            case "acttypedesc":
                var activityTypeDescription = GetActivityTypeDescription(parameter);
                return HTMLHelper.HTMLEncode(activityTypeDescription);
            case "acttype":
                var activityTypeInfo = GetActivityTypeByCodeName(parameter);
                var activityTypeName = GetActivityTypeName(parameter, activityTypeInfo);
                var activityTypeColor = activityTypeInfo?.ActivityTypeColor;

                return new Tag
                {
                    Text = activityTypeName,
                    Color = activityTypeColor
                };
        }
        return null;
    }


    private void gridElem_OnFilterFieldCreated(string columnName, UniGridFilterField filterDefinition)
    {
        filter = filterDefinition.ValueControl as CMSModules_Activities_Controls_UI_Activity_Filter;
    }


    /// <summary>
    /// Returns activity type description for provided activity type <paramref name="activityTypeCodeName"/>. 
    /// If no such activity type exists, returns activity code name.
    /// </summary>
    private static string GetActivityTypeDescription(object activityTypeCodeName)
    {
        ActivityTypeInfo activityTypeInfo = GetActivityTypeByCodeName(activityTypeCodeName);
        var activityTypeDescription = activityTypeInfo != null ? ResHelper.LocalizeString(activityTypeInfo.ActivityTypeDescription) : 
                                                                 ValidationHelper.GetString(activityTypeCodeName, null);
        return HTMLHelper.HTMLEncode(activityTypeDescription);
    }


    /// <summary>
    /// Returns activity type name for provided activity type <paramref name="activityTypeCodeName"/> and <paramref name="activityTypeInfo"/>. 
    /// If no such activity type exists, returns activity code name.
    /// </summary>
    private static string GetActivityTypeName(object activityTypeCodeName, ActivityTypeInfo activityTypeInfo)
    {
        var activityTypeName = activityTypeInfo != null ? ResHelper.LocalizeString(activityTypeInfo.ActivityTypeDisplayName) : 
                                                          ValidationHelper.GetString(activityTypeCodeName, null);
        return HTMLHelper.HTMLEncode(activityTypeName);
    }


    /// <summary>
    /// Returns existing <see cref="ActivityTypeInfo"/> with corresponding <paramref name="activityTypeCodeName"/>.
    /// </summary>
    private static ActivityTypeInfo GetActivityTypeByCodeName(object activityTypeCodeName)
    {
        string activityTypeName = ValidationHelper.GetString(activityTypeCodeName, null);
        return ActivityTypeInfo.Provider.Get(activityTypeName);
    }


    /// <summary>
    /// Inits the mass action dropdown lists.
    /// </summary>
    private void InitMassActionDDs()
    {
        drpAction.Items.Clear();
        drpWhat.Items.Clear();
        drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
        drpAction.Items.Add(new ListItem(GetString("general.delete"), Convert.ToInt32(Action.Delete).ToString()));
        drpWhat.Items.Add(new ListItem(GetString("om.activity." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
        drpWhat.Items.Add(new ListItem(GetString("om.activity." + What.All), Convert.ToInt32(What.All).ToString()));
        RegisterScripts();
    }


    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs
        script.Append(@"
function Delete(queryParameters)
{
    document.location.href = '" + ResolveUrl(DELETE_PAGE) + @"' + queryParameters;
}
function Refresh()
{
    __doPostBack('" + pnlUpdate.ClientID + @"', '');
}");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GridActions", ScriptHelper.GetScript(script.ToString()));
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "ContactStatus_" + ClientID, ScriptHelper.GetScript("var dialogParams_" + ClientID + " = '';"));

        // Register script for mass actions
        script = new StringBuilder();
        script.Append(@"
function PerformAction(selectionFunction, selectionField, actionId, actionLabel, whatId) {
    var confirmation = null;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId);
    var selectionFieldElem = document.getElementById(selectionField);
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') {
        label.innerHTML = '", GetString("MassAction.SelectSomeAction"), @"'
    }
    else if (eval(selectionFunction) && (whatDrp.value == '", (int)What.Selected, @"')) {
        label.innerHTML = '", GetString("om.activity.massaction.select"), @"';
    }
    else {
        var param = 'massaction;' + whatDrp.value;
        if (whatDrp.value == '", (int)What.Selected, @"') {
            param = param + '#' + selectionFieldElem.value;
        }
        switch(action) {
            case '", (int)Action.Delete, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.Delete, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "Delete", null), @";
                break;
            default:
                confirmation = null;
                break;
        }
        if (confirmation != null) {
            return confirm(confirmation)
        }
    }
    return false;
}
function SelectValue_" + ClientID + @"(valueID) {
    document.getElementById('" + hdnIdentifier.ClientID + @"').value = valueID;" + ControlsHelper.GetPostBackEventReference(btnOk, null) + @";
}");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "MassActions", ScriptHelper.GetScript(script.ToString()));

        // Add action to button
        btnOk.OnClientClick = "return PerformAction('" + gridElem.GetCheckSelectionScript() + "','" + gridElem.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "','" + lblInfo.ClientID + "','" + drpWhat.ClientID + "');";
    }


    /// <summary>
    /// Returns where condition depending on mass action selection.
    /// </summary>
    /// <param name="whatValue">Value of What dd-list; if the value is 'selected' it also contains selected items</param>
    private string GetWhereCondition(string whatValue)
    {
        string where = string.Empty;

        if (!string.IsNullOrEmpty(whatValue))
        {
            string selectedItems = null;
            string whatAction = null;

            if (whatValue.Contains("#"))
            {
                // Char '#' devides what-value and selected items
                whatAction = whatValue.Substring(0, whatValue.IndexOfCSafe("#"));
                selectedItems = whatValue.Substring(whatValue.IndexOfCSafe("#") + 1);
            }
            else
            {
                whatAction = whatValue;
            }

            What what = (What)ValidationHelper.GetInteger(whatAction, 0);

            switch (what)
            {
                case What.All:
                    // For all items get where condition from grid setting
                    where = SqlHelper.AddWhereCondition(gridElem.WhereCondition, gridElem.WhereClause);
                    break;

                case What.Selected:
                    // Convert array to integer values to make sure no sql injection is possible (via string values)
                    if (selectedItems != null)
                    {
                        string[] items = selectedItems.Split(new[]
                        {
                            "|"
                        }, StringSplitOptions.RemoveEmptyEntries);
                        @where = SqlHelper.GetWhereCondition<int>("ActivityID", items, false);
                    }
                    break;
            }
        }

        if (ContactID > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "ActivityContactID=" + ContactID);
        }

        return where;
    }

    #endregion
}
