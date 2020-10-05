using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_MediaLibrary_Controls_UI_MediaLibraryList : CMSAdminListControl
{
    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on a live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_READ, this);
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnAction += new OnActionEventHandler(gridElem_OnAction);
        gridElem.WhereCondition = GetWhereCondition();
        gridElem.ZeroRowsText = GetString("general.nodatafound");
    }


    private void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                SelectedItemID = ValidationHelper.GetInteger(actionArgument, 0);
                RaiseOnEdit();
                break;

            case "delete":
                MediaLibraryInfo mli = MediaLibraryInfo.Provider.Get(ValidationHelper.GetInteger(actionArgument, 0));
                // Check 'Manage' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, PERMISSION_MANAGE))
                {
                    ShowError(MediaLibraryHelper.GetAccessDeniedMessage(PERMISSION_MANAGE));
                    return;
                }
                try
                {
                    MediaLibraryInfo.Provider.Delete(mli);
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException("Media library", "DELETEOBJ", ex, SiteContext.CurrentSiteID);

                    ShowError(ex.Message, EventLogProvider.GetExceptionLogMessage(ex), null);
                }
                break;
        }

        RaiseOnAction(actionName, actionArgument);
    }


    /// <summary>
    /// Returns proper where condition.
    /// </summary>
    private string GetWhereCondition()
    {
        // Filter by current site
        string whereCond = null;
        if (SiteContext.CurrentSite != null)
        {
            whereCond = "LibrarySiteID=" + SiteContext.CurrentSite.SiteID;
        }

        return whereCond;
    }

    #endregion
}