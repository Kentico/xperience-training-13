using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.ContactManagement.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Scoring_Controls_UI_Contact_List : CMSAdminListControl, ICallbackEventHandler
{
    #region "Variables"

    private bool modifyPermission;
    private int scoreId;


    /// <summary>
    /// Available actions in mass action selector.
    /// </summary>
    protected enum Action
    {
        SelectAction = 0,
        AddToGroup = 1,
        ChangeStatus = 2
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
    /// URL of modal dialog for contact status selection.
    /// </summary>
    protected const string CONTACT_STATUS_DIALOG = "~/CMSModules/ContactManagement/FormControls/ContactStatusDialog.aspx";


    /// <summary>
    /// URL of modal dialog for contact group selection.
    /// </summary>
    protected const string CONTACT_GROUP_DIALOG = "~/CMSModules/ContactManagement/FormControls/ContactGroupDialog.aspx";


    /// <summary>
    /// URL of modal dialog for displaying score details.
    /// </summary>
    protected const string SCORE_DETAIL_DIALOG = "~/CMSModules/Scoring/Pages/ScoreDetail.aspx";


    private const int MAX_RECORDS = 10000;

    #endregion


    #region "Properties"

    /// <summary>
    /// Inner grid.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridElem;
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
    /// Dialog control identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            string identifier = hdnIdentifier.Value;
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = Guid.NewGuid().ToString();
                hdnIdentifier.Value = identifier;
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


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.IsLiveSite = IsLiveSite;

        // Get score ID from query string
        scoreId = QueryHelper.GetInteger("ScoreID", 0);

        // Initialize score filter
        FormFieldInfo ffi = new FormFieldInfo();
        ffi.Name = "SUM(Value)";
        ffi.DataType = FieldDataType.Integer;
        ucScoreFilter.FieldInfo = ffi;
        ucScoreFilter.DefaultOperator = ">=";
        ucScoreFilter.WhereConditionFormat = "{0} {2} {1}";

        // Get modify permission of current user
        modifyPermission = AuthorizationHelper.AuthorizedModifyContact(false);

        var sourceData = GetContactsWithScore();
        if (DataHelper.GetItemsCount(sourceData) >= MAX_RECORDS)
        {
            ShowInformation(GetString("om.contact.notallrecords"));
        }
        gridElem.DataSource = sourceData;


        // Register OnExternalDataBound
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnBeforeFiltering += gridElem_OnBeforeFiltering;

        // Initialize dropdown lists
        if (!RequestHelper.IsPostBack())
        {
            drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
            if (modifyPermission)
            {
                if (AuthorizationHelper.AuthorizedReadContact(false))
                {
                    drpAction.Items.Add(new ListItem(GetString("om.account." + Action.AddToGroup), Convert.ToInt32(Action.AddToGroup).ToString()));
                }
                drpAction.Items.Add(new ListItem(GetString("om.account." + Action.ChangeStatus), Convert.ToInt32(Action.ChangeStatus).ToString()));
            }
            drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.Selected), Convert.ToInt32(What.Selected).ToString()));
            drpWhat.Items.Add(new ListItem(GetString("om.contact." + What.All), Convert.ToInt32(What.All).ToString()));
        }
        else
        {
            if (ControlsHelper.CausedPostBack(btnOk))
            {
                // Set delayed reload for unigrid if mass action is performed
                gridElem.DelayedReload = true;
            }
        }

        // Register JS scripts
        RegisterScripts();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlFooter.Visible = !gridElem.IsEmpty && (drpAction.Items.Count > 1);
    }


    /// <summary>
    /// On external databound.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        ContactInfo ci;

        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", objectID);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            case "view":
                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "ViewScoreDetail(" + btn.CommandArgument + "); return false;";
                break;

            case "#statusdisplayname":
                ci = ContactInfo.Provider.Get(ValidationHelper.GetInteger(parameter, 0));
                if (ci != null)
                {
                    ContactStatusInfo statusInfo = ContactStatusInfo.Provider.Get(ci.ContactStatusID);
                    if (statusInfo != null)
                    {
                        return HTMLHelper.HTMLEncode(statusInfo.ContactStatusDisplayName);
                    }
                }
                return String.Empty;
        }

        return null;
    }


    /// <summary>
    /// Event where WHERE condition for unigrid filter could be changed.
    /// </summary>
    /// <param name="whereCondition">Where condition from filter</param>
    private string gridElem_OnBeforeFiltering(string whereCondition)
    {
        // Parse value from filter
        Regex re = RegexHelper.GetRegex(@"^\(\[Score\]\s([<>=][<>]?\s\d+)\)$");
        Match m = re.Match(whereCondition);

        if (m.Groups.Count == 2)
        {
            // Get value
            string value = m.Groups[1].ToString();

            // Add to where condition
            gridElem.WhereCondition += " HAVING SUM(Value) " + value;
        }

        // Returns empty because filter condition is added to WhereCondition property
        return string.Empty;
    }


    /// <summary>
    /// Mass operation button "OK" click.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        string resultMessage = string.Empty;
        // Get where condition depending on mass action selection
        string where;
        List<string> contactIds = null;

        What what = (What)ValidationHelper.GetInteger(drpWhat.SelectedValue, 0);
        switch (what)
        {
            // All items
            case What.All:
                // Get all contacts with scores based on filter condition
                var contacts = GetContactsWithScore();

                if (!DataHelper.DataSourceIsEmpty(contacts))
                {
                    // Get array list with IDs
                    contactIds = DataHelper.GetUniqueValues(contacts.Tables[0], "ContactID", true);
                }
                break;

            // Selected items
            case What.Selected:
                // Get selected IDs from unigrid
                contactIds = gridElem.SelectedItems;
                break;
        }

        // Prepare where condition
        if ((contactIds != null) && (contactIds.Count > 0))
        {
            where = SqlHelper.GetWhereCondition<int>("ContactID", contactIds, false);
        }
        else
        {
            where = "0=1";
        }

        Action action = (Action)ValidationHelper.GetInteger(drpAction.SelectedItem.Value, 0);
        switch (action)
        {
            // Action 'Change status'
            case Action.ChangeStatus:
                // Get selected status ID from hidden field
                int statusId = ValidationHelper.GetInteger(hdnIdentifier.Value, -1);
                // If status ID is 0, the status will be removed
                if (statusId >= 0)
                {
                    ContactInfoProvider.UpdateContactStatus(statusId, where);
                    resultMessage = GetString("om.contact.massaction.statuschanged");
                }
                break;
            // Action 'Add to contact group'
            case Action.AddToGroup:
                // Get contact group ID from hidden field
                int groupId = ValidationHelper.GetInteger(hdnIdentifier.Value, 0);
                if ((groupId > 0) && (contactIds != null))
                {
                    // Add each selected contact to the contact group, skip contacts that are already members of the group
                    foreach (string item in contactIds)
                    {
                        int contactId = ValidationHelper.GetInteger(item, 0);
                        if (contactId > 0)
                        {
                            ContactGroupMemberInfo.Provider.Add(groupId, contactId, ContactGroupMemberTypeEnum.Contact, MemberAddedHowEnum.Manual);
                        }
                    }
                    // Get contact group to show result message with its display name
                    ContactGroupInfo group = ContactGroupInfo.Provider.Get(groupId);
                    if (group != null)
                    {
                        resultMessage = string.Format(GetString("om.contact.massaction.addedtogroup"), HTMLHelper.HTMLEncode(group.ContactGroupDisplayName));
                    }
                }
                break;

            default:
                return;
        }

        if (!string.IsNullOrEmpty(resultMessage))
        {
            lblInfo.Text = resultMessage;
            lblInfo.Visible = true;
        }

        // Reload unigrid
        gridElem.ResetSelection();
        gridElem.ReloadData();
        pnlUpdate.Update();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Registers JS.
    /// </summary>
    private void RegisterScripts()
    {
        ScriptHelper.RegisterDialogScript(Page);
        StringBuilder script = new StringBuilder();

        // Register script to open dialogs
        script.Append(@"
function SelectStatus(queryParameters)
{
    modalDialog('" + ResolveUrl(CONTACT_STATUS_DIALOG) + @"' + queryParameters, 'selectStatus', '660', '590');
}
function SelectContactGroup(queryParameters)
{
    modalDialog('" + ResolveUrl(CONTACT_GROUP_DIALOG) + @"' + queryParameters, 'selectGroup', '500', '435');
}
function ViewScoreDetail(contactID)
{
    modalDialog('" + ResolveUrl(SCORE_DETAIL_DIALOG) + @"?contactid=' + contactID + '&scoreid=" + scoreId + @"', 'ScoreDetail', '700', '650');
}
function Refresh()
{
    __doPostBack('" + pnlUpdate.ClientID + @"', '');
}
var dialogParams_" + ClientID + @" = '';");

        // Register script for mass actions
        script.Append(@"
function PerformAction(selectionFunction, selectionField, actionId, actionLabel, whatId) {
    var confirmation = null;
    var label = document.getElementById(actionLabel);
    var action = document.getElementById(actionId).value;
    var whatDrp = document.getElementById(whatId);
    var selectionFieldElem = document.getElementById(selectionField);
    label.innerHTML = '';
    if (action == '", (int)Action.SelectAction, @"') {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("MassAction.SelectSomeAction"), @"
    }
    else if (eval(selectionFunction) && (whatDrp.value == '", (int)What.Selected, @"')) {
        label.innerHTML = ", ScriptHelper.GetLocalizedString("om.contact.massaction.select"), @";
    }
    else {
        var param = 'massaction;' + whatDrp.value;
        if (whatDrp.value == '", (int)What.Selected, @"') {
            param = param + '#' + selectionFieldElem.value;
        }
        switch(action) {
            case '", (int)Action.ChangeStatus, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.ChangeStatus, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectStatus", null), @";
                break;
            case '", (int)Action.AddToGroup, @"':
                dialogParams_", ClientID, @" = param + ';", (int)Action.AddToGroup, @"';", Page.ClientScript.GetCallbackEventReference(this, "dialogParams_" + ClientID, "SelectContactGroup", null), @";
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

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        string queryString = string.Empty;

        if (!string.IsNullOrEmpty(CallbackArgument))
        {
            // Prepare parameters...
            Hashtable mParameters = new Hashtable();
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

                // Get selected actions from DD-list
                Action action = (Action)ValidationHelper.GetInteger(selection[2], 0);
                switch (action)
                {
                    case Action.ChangeStatus:
                        mParameters["allownone"] = true;
                        mParameters["clientid"] = ClientID;
                        break;
                    case Action.AddToGroup:
                        break;
                    default:
                        return null;
                }
            }

            mParameters["clientid"] = ClientID;
            WindowHelper.Add(Identifier, mParameters);

            queryString = "?params=" + Identifier;
            queryString = URLHelper.AddParameterToUrl(queryString, "hash", QueryHelper.GetHash(queryString));
        }

        return queryString;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        CallbackArgument = eventArgument;
    }


    /// <summary>
    /// Returns all contacts with scores that meet filter condition.
    /// </summary>
    private DataSet GetContactsWithScore()
    {
        return ScoreContactRuleInfoProvider.GetContactsWithScore()
                                           .Source(s => s.InnerJoin<ContactInfo>("OM_ScoreContactRule.ContactID", "OM_Contact.ContactID"))
                                           .Columns("OM_Contact.ContactID", "ContactFirstName", "ContactLastName")
                                           .AddColumn(
                                               new AggregatedColumn(AggregationType.Sum, "Value").As("Score")
                                            )
                                           .WhereEquals("ScoreID", scoreId)
                                           .NewGroupBy()
                                           .GroupBy("OM_Contact.ContactID", "ContactFirstName", "ContactLastName")
                                           .Having(ucScoreFilter.GetWhereCondition())
                                           .OrderByDescending("SUM(VALUE)")
                                           // Force maximum allowed records
                                           .TopN(MAX_RECORDS)
                                           .Result;
    }

    #endregion
}
