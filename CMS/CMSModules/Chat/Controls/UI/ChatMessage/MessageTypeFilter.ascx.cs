using System;
using System.Web.UI.WebControls;

using CMS.Chat;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;

public partial class CMSModules_Chat_Controls_UI_ChatMessage_MessageTypeFilter : CMSAbstractBaseFilterControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = (ddlMessageTypeFilter.SelectedValue == "") ? null : String.Format("ChatMessageSystemMessageType = {0}", ddlMessageTypeFilter.SelectedValue);
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Life-cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        InitializeMessageTypes();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes drop down list with message types.
    /// </summary>
    private void InitializeMessageTypes()
    {
        ddlMessageTypeFilter.Items.Clear();
        ddlMessageTypeFilter.Items.Add(new ListItem(ResHelper.GetString("general.selectall"), ""));
        foreach (ChatMessageTypeEnum messageType in Enum.GetValues(typeof(ChatMessageTypeEnum)))
        {
            ddlMessageTypeFilter.Items.Add(new ListItem(messageType.ToStringValue((int)ChatMessageTypeStringValueUsageEnum.CMSDeskDescription), Convert.ToString((int)messageType)));
        }
    }
    
    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("Value", ddlMessageTypeFilter.SelectedValue);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        ddlMessageTypeFilter.SelectedValue = state.GetString("Value");
    }


    /// <summary>
    /// Resets filter to default state.
    /// </summary>
    public override void ResetFilter()
    {
        ddlMessageTypeFilter.SelectedValue = String.Empty;
    }

    #endregion
}