using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Site selector for selecting site specific, all sites or global objects.
/// </summary>
public partial class CMSModules_Polls_Controls_Filters_SiteSelector : CMSAbstractBaseFilterControl
{
    #region "Properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets site ID.
    /// </summary>
    public override int SiteID
    {
        get
        {
            return siteSelector.SiteID;
        }
        set
        {
            siteSelector.SiteID = value;
        }
    }


    /// <summary>
    /// Filter value.
    /// </summary>
    public override object Value
    {
        get
        {
            return siteSelector.Value;
        }
        set
        {
            siteSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets inner uniselector.
    /// </summary>
    public CMSDropDownList Selector
    {
        get
        {
            return siteSelector.Selector;
        }
    }


    /// <summary>
    /// Indicates if all global polls should be displayed. If FALSE then only objects
    /// assigned to given site are displayed. Default is FALSE.
    /// </summary>
    public bool DisplayAllGlobals
    {
        get;
        set;
    }

    #endregion


    #region "Methods and events"

    /// <summary>
    /// OnLoad override - check whether filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        siteSelector.Selector.SelectedIndexChanged += Selector_SelectedIndexChanged;
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    public string GetWhereCondition()
    {
        int currentSiteID = SiteContext.CurrentSiteID;

        switch (siteSelector.SiteID)
        {
            case UniSelector.US_GLOBAL_RECORD: // Global objects only
                string where = "(PollGroupID IS NULL) AND (PollSiteID IS NULL)";
                if (!DisplayAllGlobals)
                {
                    where += "AND (PollID IN (SELECT PollID FROM Polls_PollSite WHERE SiteID=" + currentSiteID + "))";
                }
                return where;

            case UniSelector.US_GLOBAL_AND_SITE_RECORD: // Global and site objects
                if (DisplayAllGlobals)
                {
                    return "(PollGroupID IS NULL) AND ((PollSiteID IS NULL) OR (PollSiteID=" + currentSiteID + "))";
                }
                else
                {
                    return "(PollGroupID IS NULL) AND ((PollSiteID=" + currentSiteID + ") OR ((PollSiteID IS NULL) AND (PollID IN (SELECT PollID FROM Polls_PollSite WHERE SiteID=" + currentSiteID + "))))";
                }

            default: // Site objects only
                return "(PollGroupID IS NULL) AND PollSiteID=" + siteSelector.SiteID;
        }
    }


    /// <summary>
    /// Handles Site selector OnSelectionChanged event.
    /// </summary>
    protected void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }

    #endregion
}