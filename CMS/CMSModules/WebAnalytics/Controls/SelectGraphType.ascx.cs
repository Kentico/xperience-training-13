using System;
using System.Collections.Generic;

using CMS.Base.Web.UI;
using CMS.UIControls;
using CMS.WebAnalytics;


public partial class CMSModules_WebAnalytics_Controls_SelectGraphType : CMSAdminControl
{
    private HitsIntervalEnum? mSelectedValue;

    /// <summary>
    /// Thrown when graph type changed.
    /// </summary>
    public event EventHandler GraphTypeChanged;
    
    
    /// <summary>
    /// Selected graph type.
    /// </summary>
    public HitsIntervalEnum SelectedValue
    {
        get
        {
            if (mSelectedValue != null)
            {
                return mSelectedValue.Value;
            }

            var value = graphIntervalElem.SelectedActionName ?? "day";

            return HitsIntervalEnumFunctions.StringToHitsConversion(value);
        }
        set
        {
            mSelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets a semicolon delimited list of visible graph types.
    /// </summary>
    public string VisibleGraphTypes
    {
        get
        {
            return ViewState["VisibleGraphTypes"] as string ?? "hour;day;week;month;year";
        }
        set
        {
            ViewState["VisibleGraphTypes"] = value;
        }
    }

    
    /// <summary>
    /// OnPreRender event handler
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPreRender(EventArgs e)
    {
        InitializeActions();

        // Select action 
        graphIntervalElem.SelectedActionName = HitsIntervalEnumFunctions.HitsConversionToString(SelectedValue);

        base.OnPreRender(e);
    }


    /// <summary>
    /// Updates the specified graph type controls depending on their visibility.
    /// </summary>
    private void InitializeActions()
    {
        var visibleGraphTypes = new HashSet<HitsIntervalEnum>();
        foreach (var token in VisibleGraphTypes.Split(';'))
        {
            var visibleGraphType = HitsIntervalEnumFunctions.StringToHitsConversion(token);
            visibleGraphTypes.Add(visibleGraphType);
        }
        
        if (visibleGraphTypes.Contains(HitsIntervalEnum.Hour))
        {
            graphIntervalElem.Actions.Add(new CMSButtonGroupAction
            {
                Name = "hour",
                Text = GetString("general.hour"),
            });
        }

        if (visibleGraphTypes.Contains(HitsIntervalEnum.Day))
        {
            graphIntervalElem.Actions.Add(new CMSButtonGroupAction
            {
                Name = "day",
                Text = GetString("general.day"),
            });
        }

        if (visibleGraphTypes.Contains(HitsIntervalEnum.Week))
        {
            graphIntervalElem.Actions.Add(new CMSButtonGroupAction
            {
                Name = "week",
                Text = GetString("general.week"),
            });
        }

        if (visibleGraphTypes.Contains(HitsIntervalEnum.Month))
        {
            graphIntervalElem.Actions.Add(new CMSButtonGroupAction
            {
                Name = "month",
                Text = GetString("general.month"),
            });
        }

        if (visibleGraphTypes.Contains(HitsIntervalEnum.Year))
        {
            graphIntervalElem.Actions.Add(new CMSButtonGroupAction
            {
                Name = "year",
                Text = GetString("general.year"),
            });
        }        
    }


    /// <summary>
    /// Button group clicked event handler
    /// </summary>
    protected void graphIntervalElem_OnButtonClick(object sender, CMSButtonActionClickedEventArgs e)
    {
        if (e.ActionName != graphIntervalElem.SelectedActionName)
        {
            graphIntervalElem.SelectedActionName = e.ActionName;

            if (GraphTypeChanged != null)
            {
                GraphTypeChanged(this, new EventArgs());
            }
        }
    }
}