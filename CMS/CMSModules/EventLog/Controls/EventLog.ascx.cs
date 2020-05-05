using System;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_EventLog_Controls_EventLog : CMSAdminControl, ICallbackEventHandler
{
    #region "Variables"

    private int mSiteId = -1;
    private Hashtable mParameters = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the site id which the event log is to be displayed
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
        }
    }


    /// <summary>
    /// Gets or sets the event id
    /// </summary>
    public int EventID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the visibility of the filter.
    /// </summary>
    public bool ShowFilter
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the event log uniGrid.
    /// </summary>
    public UniGrid EventLogGrid
    {
        get
        {
            return gridEvents;
        }
    }

    #endregion


    #region "Stop processing"

    /// <summary>
    /// Returns true if the control processing should be stopped
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
            gridEvents.StopProcessing = value;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing)
        {
            // System control properties
            gridEvents.ReloadData();
        }
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Updates the update panel of this control.
    /// </summary>
    public void Update()
    {
        pnlUpdate.Update();
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!ShowFilter)
        {
            gridEvents.OnFilterFieldCreated += (name, definition) => definition.ValueControl.Visible = false;
        }

        gridEvents.HideFilterButton = true;
        gridEvents.LoadGridDefinition();

        gridEvents.GridView.CssClass += " event-log";
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // check access permissions
        CheckPermissions("CMS.EventLog", PERMISSION_READ);

        // Register the dialog script
        ScriptHelper.RegisterDialogScript(Page);

        string openEventDetailScript = "function OpenEventDetail(queryParameters) {\n" +
                                       "modalDialog(" + ScriptHelper.GetString(ResolveUrl("~/CMSModules/EventLog/EventLog_Details.aspx")) + " + queryParameters, 'eventdetails', 1080, 700);\n" +
                                       "}";

        // Register the dialog script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EventLog_OpenDetail", ScriptHelper.GetScript(openEventDetailScript));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "EventLog_" + ClientID, ScriptHelper.GetScript("var eventDialogParams_" + ClientID + " = '';"));

        gridEvents.GridView.RowDataBound += GridView_RowDataBound;
        gridEvents.OnExternalDataBound += gridEvents_OnExternalDataBound;
        gridEvents.Columns = "EventID,EventType,EventTime,Source,EventCode,UserName,IPAddress,DocumentName,SiteID,EventMachineName";

        if (String.IsNullOrEmpty(gridEvents.WhereCondition))
        {
            gridEvents.WhereCondition = GenerateWhereCondition();
        }

        if (!RequestHelper.IsPostBack())
        {
            if (String.IsNullOrEmpty(gridEvents.OrderBy))
            {
                // if not set externally => set defaults
                gridEvents.OrderBy = "EventID DESC";
            }
        }
    }

    #endregion


    #region "UI Handlers"

    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string code = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["EventType"], string.Empty);
            switch (code.ToLowerCSafe())
            {
                case "e":
                    e.Row.CssClass = "error";
                    break;

                case "w":
                    e.Row.CssClass = "warning";
                    break;
            }
        }
    }


    protected object gridEvents_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "eventtype":
                {
                    string evetType = ValidationHelper.GetString(parameter, "");
                    return "<div style=\"width:100%;text-align:center;cursor:help;\" title=\"" + HTMLHelper.HTMLEncode(EventLogHelper.GetEventTypeText(evetType)) + "\">" + evetType + " </div>";
                }

            case "formattedusername":
                return HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(Convert.ToString(parameter)));

            case "view":
                {
                    if (sender is CMSGridActionButton)
                    {
                        CMSGridActionButton img = sender as CMSGridActionButton;
                        DataRowView drv = UniGridFunctions.GetDataRowView(img.Parent as DataControlFieldCell);
                        int eventId = ValidationHelper.GetInteger(drv["EventID"], 0);
                        //img.AlternateText = GetString("Unigrid.EventLog.Actions.Display");
                        img.ToolTip = GetString("Unigrid.EventLog.Actions.Display");
                        img.OnClientClick = "eventDialogParams_" + ClientID + " = '" + eventId + "';" + Page.ClientScript.GetCallbackEventReference(this, "eventDialogParams_" + ClientID, "OpenEventDetail", null) + ";return false;";
                    }
                    return sender;
                }
        }

        return parameter;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Generates site filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        string whereCond = "";

        if (SiteID > 0)
        {
            whereCond += " (SiteID=" + SiteID + ")";
        }
        else if (SiteID == 0)
        {
            whereCond += "(SiteID IS NULL)";
        }

        return whereCond;
    }


    /// <summary>
    /// Gets the dialog identifier used for sharing data between windows.
    /// </summary>
    /// <returns>Dialog identifier</returns>
    private Guid GetDialogIdentifier()
    {
        Guid identifier;

        // Try parse the identifier as a Guid value
        if (!Guid.TryParse(hdnIdentifier.Value, out identifier))
        {
            // If the identifier value is not a valid Guid value, generates a new Guid
            identifier = Guid.NewGuid();
            hdnIdentifier.Value = identifier.ToString();
        }

        return identifier;
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Get callback result
    /// </summary>
    public string GetCallbackResult()
    {
        string whereCondition = (SiteID > 0) ? SqlHelper.AddWhereCondition(gridEvents.WhereClause, GenerateWhereCondition()) : gridEvents.WhereClause;

        mParameters = new Hashtable();
        mParameters["where"] = whereCondition;
        mParameters["orderby"] = gridEvents.SortDirect;

        // Get the dialog identifier
        Guid dialogIdentifier = GetDialogIdentifier();

        // Store the dialog identifier with appropriate data in the session
        WindowHelper.Add(dialogIdentifier.ToString(), mParameters);

        string queryString = "?params=" + dialogIdentifier;

        if (SiteID > 0)
        {
            queryString = URLHelper.AddParameterToUrl(queryString, "siteid", SiteID.ToString());
        }
        queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        queryString = URLHelper.AddParameterToUrl(queryString, "eventid", EventID.ToString());

        return queryString;
    }


    /// <summary>
    /// Raise callback method
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        EventID = ValidationHelper.GetInteger(eventArgument, 0);
    }

    #endregion
}
