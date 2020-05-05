using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.OnlineForms;
using CMS.Polls;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_Polls_Controls_AnswerList : CMSAdminListControl
{
    #region "Variables"

    private bool mAllowEdit = true;
    private bool bizFormsAvailable;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets and sets Poll ID.
    /// </summary>
    public int PollId
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState[ClientID + "PollID"], 0);
        }
        set
        {
            ViewState[ClientID + "PollID"] = value;
        }
    }


    /// <summary>
    /// Group ID, or 0 if not in group context.
    /// </summary>
    public int GroupId
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if DelayedReload for Unigrid should be used.
    /// </summary>
    public bool DelayedReload
    {
        get
        {
            return uniGrid.DelayedReload;
        }
        set
        {
            uniGrid.DelayedReload = value;
        }
    }


    /// <summary>
    /// Indicates if move/edit actions should be allowed
    /// </summary>
    public bool AllowEdit
    {
        get
        {
            return mAllowEdit;
        }
        set
        {
            mAllowEdit = value;
        }
    }


    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if parent object exists
        if ((PollId > 0) && !IsLiveSite)
        {
            UIContext.EditedObject = PollInfoProvider.GetPollInfo(PollId);
        }

        ScriptHelper.RegisterDialogScript(Page);

        uniGrid.IsLiveSite = IsLiveSite;
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.GridView.AllowSorting = false;
        uniGrid.WhereCondition = "AnswerPollID=" + PollId;
        uniGrid.ZeroRowsText = GetString("general.nodatafound");
        uniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        uniGrid.OnBeforeDataReload += UniGrid_OnBeforeDataReload;

        if (!AllowEdit)
        {    
            uniGrid.ShowObjectMenu = false;
        }

        bizFormsAvailable = ModuleEntryManager.IsModuleLoaded(ModuleName.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleName.BIZFORM, SiteContext.CurrentSiteName);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            SelectedItemID = Convert.ToInt32(actionArgument);
            RaiseOnEdit();
        }
        else if (actionName == "delete")
        {
            if (!AllowEdit)
            {
                return;
            }
            if (GroupId > 0)
            {
                CMSDeskPage.CheckGroupPermissions(GroupId, PERMISSION_MANAGE);
            }

            // Delete PollAnswerInfo object from database
            PollAnswerInfoProvider.DeletePollAnswerInfo(Convert.ToInt32(actionArgument));
            ReloadData(true);
        }
    }


    /// <summary>
    /// Forces unigrid to reload data.
    /// </summary>
    public override void ReloadData(bool forceReload)
    {
        uniGrid.WhereCondition = "AnswerPollID=" + PollId;

        if (forceReload)
        {
            uniGrid.Reset();
        }

        uniGrid.ReloadData();
    }


    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "edit":
            case "delete":
                (sender as CMSGridActionButton).Visible = AllowEdit;
                return sender;
            case "answerenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
            case "answerisopenended":
                return String.IsNullOrEmpty(ValidationHelper.GetString(parameter, string.Empty)) ? GetString("polls.AnswerTypeStandard") : GetString("polls.AnswerTypeOpenEnded");
            case "answerform":
                if (sender is CMSGridActionButton)
                {
                    CMSGridActionButton actionButton = sender as CMSGridActionButton;
                    GridViewRow gvr = parameter as GridViewRow;

                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Form", "ReadData"))
                    {
                        actionButton.Visible = false;
                    }
                    else if (gvr != null)
                    {
                        DataRowView drv = gvr.DataItem as DataRowView;
                        if (drv != null)
                        {
                            string formName = ValidationHelper.GetString(drv["AnswerForm"], null);
                            if (String.IsNullOrEmpty(formName))
                            {
                                actionButton.Visible = false;
                            }
                            else
                            {
                                BizFormInfo bfi = BizFormInfoProvider.GetBizFormInfo(formName, SiteContext.CurrentSiteID);
                                if ((bfi != null) && bizFormsAvailable)
                                {
                                    actionButton.OnClientClick = "modalDialog('" + ResolveUrl("~/CMSModules/Polls/Tools/Polls_Answer_Results.aspx") + "?formid=" + bfi.FormID + "&dialogmode=1', 'AnswerForm', '1000', '700'); return false;";
                                }
                                else
                                {
                                    actionButton.Visible = false;
                                }
                            }
                        }
                    }
                }
                return sender;
        }
        return parameter;
    }


    protected void UniGrid_OnBeforeDataReload()
    {
        PollInfo pi = PollInfoProvider.GetPollInfo(PollId);
        uniGrid.GridView.Columns[4].Visible = (pi != null) && (pi.PollSiteID > 0) && (pi.PollGroupID == 0) && bizFormsAvailable;
    }

    #endregion
}
