using System;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Web.UI;

[EditedObject(CampaignInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.WEBANALYTICS, "Campaign.Schedule")]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Schedule : CMSCampaignPage
{
    /// <summary>
    /// Tolerance allowing to pick 'Now' in date time picker and hit Save without 'date in past' error
    /// </summary>
    private readonly TimeSpan mNowTolerance = TimeSpan.FromMinutes(3);

    private CampaignInfo mEditedCampaign;
    private CampaignStatusEnum mCampaignStatus;
    private readonly ICampaignScheduleService mScheduleService = Service.Resolve<ICampaignScheduleService>();
    private readonly ICampaignValidationService mValidationService = Service.Resolve<ICampaignValidationService>();
    private readonly int mSiteID = SiteContext.CurrentSiteID;


    protected void Page_Load(object sender, EventArgs e)
    {
        mEditedCampaign = EditedObject as CampaignInfo;
        if ((mEditedCampaign == null) || (mEditedCampaign.CampaignSiteID != mSiteID))
        {
            RedirectToInformation(GetString("campaign.schedule.nocampaign"));
            return;
        }

        mCampaignStatus = mEditedCampaign.GetCampaignStatus(DateTime.Now);

        if (!RequestHelper.IsPostBack())
        {
            InitSelectors();
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // reload the status
        mCampaignStatus = mEditedCampaign.GetCampaignStatus(DateTime.Now);

        InitHeader();
        ShowInfoMessage();

        // Disable DateTime pickers according current status
        if (mCampaignStatus == CampaignStatusEnum.Running)
        {
            dtFrom.Enabled = false;
        }
        else if (mCampaignStatus == CampaignStatusEnum.Finished)
        {
            dtFrom.Enabled = false;
            dtTo.Enabled = false;
        }
    }


    /// <summary>
    /// Initializes the header with action buttons according current campaign status.
    /// </summary>
    private void InitHeader()
    {
        if (mCampaignStatus == CampaignStatusEnum.Finished)
        {
            return;
        }

        HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("campaign.saveschedule"),
            EventName = "save"
        });

        if (mCampaignStatus == CampaignStatusEnum.Running)
        {
            HeaderActions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("campaign.finishnow"),
                ButtonStyle = ButtonStyle.Default,
                EventName = "finnish",
                OnClientClick = GetConfirmationScript("campaign.finish.confirm", "campaign.finish.infomessage")
            });
        }
        else
        {
            HeaderActions.ActionsList.Add(new HeaderAction
            {
                Text = GetString("campaign.launch"),
                ButtonStyle = ButtonStyle.Default,
                EventName = "launch",
                OnClientClick = GetConfirmationScript("campaign.launch.confirm", "campaign.launch.infomessage")
            });
        }
    }


    private void ShowInfoMessage()
    {
        switch (mCampaignStatus)
        {
            case CampaignStatusEnum.Draft:
            case CampaignStatusEnum.Scheduled:
                ShowInformation(GetString("campaign.launch.infomessage"));
                break;
            case CampaignStatusEnum.Running:
                ShowInformation(GetString("campaign.finish.infomessage"));
                break;
        }
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (!ValidateCampaign())
        {
            return;
        }

        switch (e.CommandName.ToLowerInvariant())
        {
            case "save":
                SaveSchedule();
                break;
            case "launch":
                LaunchCampaign();
                InitSelectors();
                break;
            case "finnish":
                FinnishCampaign();
                InitSelectors();
                break;
        }
    }


    private void InitSelectors()
    {
        dtFrom.SelectedDateTime = mEditedCampaign.CampaignOpenFrom;
        dtTo.SelectedDateTime = mEditedCampaign.CampaignOpenTo;
    }


    private void LaunchCampaign()
    {
        if ((mCampaignStatus == CampaignStatusEnum.Running) || (mCampaignStatus == CampaignStatusEnum.Finished))
        {
            ShowError(GetString("campaign.launch.alreadylaunched"));
            return;
        }

        if (mScheduleService.Launch(mEditedCampaign, mSiteID))
        {
            ShowConfirmation(GetString("campaign.launched"));
        }
    }


    private void FinnishCampaign()
    {
        if (mCampaignStatus == CampaignStatusEnum.Finished)
        {
            ShowError(GetString("campaign.finish.alreadyfinished"));
            return;
        }

        if (mScheduleService.Finish(mEditedCampaign, mSiteID))
        {
            ShowConfirmation(GetString("campaign.finished"));
        }
    }


    private void SaveSchedule()
    {
        var from = dtFrom.SelectedDateTime;
        var to = dtTo.SelectedDateTime;

        if (ValidateScheduleDates(ref from, ref to))
        {
            var success = false;

            switch (mCampaignStatus)
            {
                case CampaignStatusEnum.Draft:
                    success = mScheduleService.Schedule(mEditedCampaign, from, to, mSiteID);
                    break;

                case CampaignStatusEnum.Scheduled:
                    if (from != DateTimeHelper.ZERO_TIME)
                    {
                        success = mScheduleService.Reschedule(mEditedCampaign, from, to, mSiteID);
                    }
                    else
                    {
                        success = mScheduleService.Unschedule(mEditedCampaign);
                    }
                    break;

                case CampaignStatusEnum.Running:
                    success = mScheduleService.Finish(mEditedCampaign, mSiteID, to);
                    break;

                default:
                    ShowError(GetString("campaign.finish.alreadyfinished"));
                    break;
            }

            if (success)
            {
                ShowChangesSaved();
            }
        }
    }


    private bool ValidateCampaign()
    {
        if (!mValidationService.Exists(mEditedCampaign))
        {
            mEditedCampaign = null;
        }

        if (!mValidationService.IsOnSite(mEditedCampaign, mSiteID))
        {
            ShowError(GetString("campaign.wrongsite"));
            return false;
        }

        if (!mValidationService.HasConversion(mEditedCampaign))
        {
            ShowError(GetString("campaign.conversion.atleastone"));
            return false;
        }

        return true;
    }


    private bool ValidateScheduleDates(ref DateTime from, ref DateTime to)
    {
        if (mCampaignStatus == CampaignStatusEnum.Finished)
        {
            ShowError(GetString("campaign.finish.alreadyfinished"));
            InitSelectors();
            return false;
        }

        if (!dtFrom.IsValidRange() || !dtTo.IsValidRange())
        {
            // Invalid finish date
            ShowError(GetString("campaign.validdate"));
            return false;
        }

        if ((to != DateTimeHelper.ZERO_TIME) && (from == DateTimeHelper.ZERO_TIME))
        {
            // Invalid finish date
            ShowError(GetString("campaign.validlaunchdate"));
            return false;
        }

        if ((from == DateTimeHelper.ZERO_TIME) && (mEditedCampaign.CampaignOpenFrom == DateTimeHelper.ZERO_TIME))
        {
            // Empty launch date
            ShowError(GetString("campaign.validlaunchdate"));
            return false;
        }

        if ((to < DateTime.Now.Subtract(mNowTolerance)) && (to != DateTimeHelper.ZERO_TIME))
        {
            // Trying to finish in the past
            ShowError(GetString("campaign.datepast"));
            return false;
        }
        to = FixPastDate(to);

        // Check launch dateTime only for draft and scheduled campaigns
        if (mCampaignStatus != CampaignStatusEnum.Running)
        {
            if (!dtFrom.IsValidRange())
            {
                // Invalid launch date
                ShowError(GetString("campaign.validdate"));
                return false;
            }
            if ((from < DateTime.Now.Subtract(mNowTolerance)) && (from != DateTimeHelper.ZERO_TIME))
            {
                // Schedule to the past
                ShowError(GetString("campaign.datepast"));
                return false;
            }
            from = FixPastDate(from);

            if ((from >= to) && (to != DateTimeHelper.ZERO_TIME))
            {
                // Range overlaps
                ShowError(GetString("general.dateoverlaps"));
                return false;
            }
        }
        else if (dtFrom.Enabled)
        {
            // Trying to modify campaign launch but campaign is already running
            ShowError(GetString("campaign.launch.alreadylaunched"));
            InitSelectors();
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns DateTime.Now if given <paramref name="date"/> is in the past.
    /// </summary>
    /// <param name="date">Date & time to be checked.</param>
    private DateTime FixPastDate(DateTime date)
    {
        if (date == DateTimeHelper.ZERO_TIME)
        {
            return date;
        }

        var now = DateTime.Now;

        return (date < now) ? now : date;
    }


    private string GetConfirmationScript(string headingResStr, string bodyResStr)
    {
        var translation = string.Join(Environment.NewLine, GetString(headingResStr), GetString("campaign.launch.confirm.delimiter"), GetString(bodyResStr));
        return string.Format("return confirm({0});", ScriptHelper.GetString(translation));
    }
}
