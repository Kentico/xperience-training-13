using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AbuseReport_Controls_AbuseReportList : CMSAdminControl
{
    #region "Variables"

    private string mWhereCondition = String.Empty;
    private string mSiteName = String.Empty;
    private string mOrderBy = null;
    private string mItemsPerPage = string.Empty;
    private int mStatus = -1;
    private bool mUseEditOnPostback = false;
    private int mEditReportID = 0;

    #endregion


    #region "Properties"

    /// <summary>
    /// Where condition for grid.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// When edited with postback this value contains edited reportID.
    /// </summary>
    public int EditReportID
    {
        get
        {
            return mEditReportID;
        }
        set
        {
            mEditReportID = value;
        }
    }


    /// <summary>
    /// Items per page for grid.
    /// </summary>
    public string ItemsPerPage
    {
        get
        {
            return mItemsPerPage;
        }
        set
        {
            mItemsPerPage = value;
        }
    }


    /// <summary>
    /// Site name filter.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mSiteName;
        }
        set
        {
            mSiteName = value;
        }
    }


    /// <summary>
    /// Status of abuse report.
    /// </summary>
    public int Status
    {
        get
        {
            return mStatus;
        }
        set
        {
            mStatus = value;
        }
    }


    /// <summary>
    /// Order by for uni grid.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mOrderBy;
        }
        set
        {
            mOrderBy = value;
        }
    }


    /// <summary>
    /// If true and edit no redirect is applied.
    /// </summary>
    public bool UseEditOnPostback
    {
        get
        {
            return mUseEditOnPostback;
        }
        set
        {
            mUseEditOnPostback = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            ucAbuseReportGrid.StopProcessing = true;
        }
        else
        {
            // Load unigrid
            ucAbuseReportGrid.OnAction += UniGrid_OnAction;
            ucAbuseReportGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
            if (!string.IsNullOrEmpty(OrderBy))
            {
                ucAbuseReportGrid.OrderBy = OrderBy;
            }
            ucAbuseReportGrid.ZeroRowsText = ResHelper.GetString("general.nodatafound");
            // If where condition is not set directly try set from parameters
            if (WhereCondition == String.Empty)
            {
                // Site name
                SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteName);
                if (si != null)
                {
                    ucAbuseReportGrid.WhereCondition = SqlHelper.AddWhereCondition(ucAbuseReportGrid.WhereCondition, "(ReportSiteID = " + si.SiteID + ")");
                }

                // Status
                if (Status != -1)
                {
                    ucAbuseReportGrid.WhereCondition = SqlHelper.AddWhereCondition(ucAbuseReportGrid.WhereCondition, "(ReportStatus = " + Status + ")");
                }
            }
            else
            {
                ucAbuseReportGrid.WhereCondition = WhereCondition;
            }

            if ((!RequestHelper.IsPostBack()) && (!string.IsNullOrEmpty(ItemsPerPage)))
            {
                ucAbuseReportGrid.Pager.DefaultPageSize = ValidationHelper.GetInteger(ItemsPerPage, -1);
            }
        }
    }

    #endregion


    #region "Grid events"

    /// <summary>
    /// Unigrid external databound handler.
    /// </summary>
    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Display link instead of title
            case "title":
                if (parameter != DBNull.Value)
                {
                    DataRowView row = (DataRowView)parameter;
                    string url = ValidationHelper.GetString(row["ReportURL"], "");
                    string title = HTMLHelper.HTMLEncode(ValidationHelper.GetString(row["ReportTitle"], ""));

                    HyperLink link = new HyperLink();
                    string culture = ValidationHelper.GetString(row["ReportCulture"], "");
                    if (culture != String.Empty)
                    {
                        url = URLHelper.AddParameterToUrl(url, URLHelper.LanguageParameterName, culture);
                    }
                    link.NavigateUrl = url;
                    link.Target = "_blank";
                    link.Text = title;
                    link.ToolTip = HTMLHelper.HTMLEncode(url);
                    link.Style.Add("cursor", "help");
                    return link;
                }
                return sourceName;

            // Insert status label
            case "status":
                if (parameter != DBNull.Value)
                    switch (parameter.ToString().ToLowerCSafe())
                    {
                        default:
                            return ResHelper.GetString("general.new");

                        case "1":
                            return "<span class=\"AbuseSolved\">" + ResHelper.GetString("general.solved") + "</span>";

                        case "2":
                            return "<span class=\"AbuseRejected\">" + ResHelper.GetString("general.rejected") + "</span>";
                    }
                return sourceName;

            case "solve":
                if (parameter != DBNull.Value)
                {
                    string status = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["ReportStatus"], "");
                    var button = ((CMSGridActionButton)sender);
                    switch (status)
                    {
                        // Disables the button and changes its icon
                        case "1":
                            button.Enabled = false;
                            break;

                        case "2":
                            button.Enabled = true;
                            break;
                    }
                }
                break;

            case "reject":
                if (parameter != DBNull.Value)
                {
                    string status = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["ReportStatus"], "");
                    var button = ((CMSGridActionButton)sender);
                    
                    switch (status)
                    {
                        // Disables the button and changes its icon
                        case "1":
                            button.Enabled = true;
                            break;

                        case "2":
                            button.Enabled = false;
                            break;
                    }
                }
                break;

            case "objecttype":
                string objectType = ImportExportHelper.GetSafeObjectTypeName(parameter.ToString());
                if (!string.IsNullOrEmpty(objectType))
                {
                    parameter = ResHelper.GetString("ObjectType." + objectType);
                }
                else
                {
                    return "-";
                }
                break;

            case "comment":
                string resultText = parameter.ToString();
                parameter = HTMLHelper.HTMLEncode(TextHelper.LimitLength(resultText, 297, "..."));
                break;
        }

        return parameter.ToString();
    }


    /// <summary>
    /// Unigrid event handler.
    /// </summary>
    /// <param name="actionName">Name of action</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGrid_OnAction(string actionName, object actionArgument)
    {
        // Edit report
        if (actionName == "edit")
        {
            if (!UseEditOnPostback)
            {
                URLHelper.Redirect(UrlResolver.ResolveUrl("AbuseReport_Edit.aspx?reportid=" + actionArgument));
            }
            else
            {
                EditReportID = ValidationHelper.GetInteger(actionArgument, 0);
            }
        }
        // Delete report
        else if (actionName == "delete")
        {
            // Check permissions            
            if (CheckPermissions("CMS.AbuseReport", "Manage"))
            {
                AbuseReportInfoProvider.DeleteAbuseReportInfo(ValidationHelper.GetInteger(actionArgument, 0));
            }
        }
        // Solve report
        else if (actionName == "solve")
        {
            // Check permissions
            if (CheckPermissions("CMS.AbuseReport", "Manage"))
            {
                AbuseReportInfo ari = AbuseReportInfoProvider.GetAbuseReportInfo(ValidationHelper.GetInteger(actionArgument, 0));
                if (ari != null)
                {
                    ari.ReportStatus = AbuseReportStatusEnum.Solved;
                    AbuseReportInfoProvider.SetAbuseReportInfo(ari);
                }
            }
        }
        // Reject report
        else if (actionName == "reject")
        {
            // Check permissions
            if (CheckPermissions("CMS.AbuseReport", "Manage"))
            {
                AbuseReportInfo ari = AbuseReportInfoProvider.GetAbuseReportInfo(ValidationHelper.GetInteger(actionArgument, 0));
                if (ari != null)
                {
                    ari.ReportStatus = AbuseReportStatusEnum.Rejected;
                    AbuseReportInfoProvider.SetAbuseReportInfo(ari);
                }
            }
        }
    }

    #endregion
}