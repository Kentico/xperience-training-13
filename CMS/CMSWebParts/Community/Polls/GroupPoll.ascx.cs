using System;

using CMS.Community;
using CMS.Core;
using CMS.Helpers;
using CMS.Localization;
using CMS.Polls;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.WebAnalytics;

public partial class CMSWebParts_Community_Polls_GroupPoll : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the code name of the poll, which should be displayed.
    /// </summary>
    public string PollCodeName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PollCodeName"), "");
        }
        set
        {
            SetValue("PollCodeName", value);
        }
    }


    /// <summary>
    /// Gets or sets the community group name.
    /// </summary>
    public string GroupName
    {
        get
        {
            string groupName = ValidationHelper.GetString(GetValue("GroupName"), "");
            if ((string.IsNullOrEmpty(groupName) || groupName == GroupInfoProvider.CURRENT_GROUP) && (CommunityContext.CurrentGroup != null))
            {
                return CommunityContext.CurrentGroup.GroupName;
            }
            return groupName;
        }
        set
        {
            SetValue("GroupName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the graph of the poll is displayed.
    /// </summary>
    public bool ShowGraph
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowGraph"), viewPoll.ShowGraph);
        }
        set
        {
            SetValue("ShowGraph", value);
        }
    }


    /// <summary>
    /// Gets or sets the type of the representation of the answers' count in the graph.
    /// </summary>
    public CountTypeEnum CountType
    {
        get
        {
            int countTypeInt = ValidationHelper.GetInteger(GetValue("CountType"), 0);
            if (countTypeInt == 1)
            {
                return CountTypeEnum.Absolute;
            }
            else if (countTypeInt == 2)
            {
                return CountTypeEnum.Percentage;
            }
            else
            {
                return CountTypeEnum.None;
            }
        }
        set
        {
            if (value == CountTypeEnum.Absolute)
            {
                SetValue("CountType", 1);
            }
            else if (value == CountTypeEnum.Percentage)
            {
                SetValue("CountType", 2);
            }
            else
            {
                SetValue("CountType", 0);
            }
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the graph is displayed after answering the poll.
    /// </summary>
    public bool ShowResultsAfterVote
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowResultsAfterVote"), viewPoll.ShowResultsAfterVote);
        }
        set
        {
            SetValue("ShowResultsAfterVote", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether check if current user has voted.
    /// </summary>
    public bool CheckVoted
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckVoted"), viewPoll.CheckVoted);
        }
        set
        {
            SetValue("CheckVoted", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), viewPoll.CheckPermissions);
        }
        set
        {
            SetValue("CheckPermissions", value);
            viewPoll.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the control hides when not authorized, 
    /// otherwise the control displays the message and does not allow to vote.
    /// </summary>
    public bool HideWhenNotAuthorized
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideWhenNotAuthorized"), viewPoll.HideWhenNotAuthorized);
        }
        set
        {
            SetValue("HideWhenNotAuthorized", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the control hides when not opened, 
    /// otherwise the control does not allow to vote.
    /// </summary>
    public bool HideWhenNotOpened
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("HideWhenNotOpened"), viewPoll.HideWhenNotOpened);
        }
        set
        {
            SetValue("HideWhenNotOpened", value);
        }
    }


    /// <summary>
    /// Gets or sets the text of the vote button.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), viewPoll.ButtonText);
        }
        set
        {
            SetValue("ButtonText", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
        }
        set
        {
            if (value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion


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
            // Do nothing
            viewPoll.Visible = false;
        }
        else
        {
            GroupInfo gi = GroupInfoProvider.GetGroupInfo(GroupName, SiteContext.CurrentSiteName);
            if (gi != null)
            {
                viewPoll.ControlContext = ControlContext;
                viewPoll.PollCodeName = PollCodeName;
                viewPoll.CheckPermissions = CheckPermissions;
                viewPoll.CheckVoted = CheckVoted;
                viewPoll.CountType = CountType;
                viewPoll.CacheMinutes = CacheMinutes;
                viewPoll.HideWhenNotAuthorized = HideWhenNotAuthorized;
                viewPoll.ShowGraph = ShowGraph;
                viewPoll.ShowResultsAfterVote = ShowResultsAfterVote;
                viewPoll.HideWhenNotOpened = HideWhenNotOpened;
                viewPoll.ButtonText = ButtonText;
                viewPoll.PollSiteID = SiteContext.CurrentSiteID;
                viewPoll.PollGroupID = gi.GroupID;
            }
            else
            {
                viewPoll.Visible = false;
            }
        }
    }


    /// <summary>
    /// OnPrerender override (Set visibility).
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = viewPoll.Visible;
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
        viewPoll.ReloadData(true);
    }
}