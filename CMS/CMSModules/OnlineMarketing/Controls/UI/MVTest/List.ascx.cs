using System;
using System.Collections.Generic;
using System.Data;

using CMS.Base;

using System.Linq;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Helpers.Markup;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

using GridAction = CMS.UIControls.UniGridConfig.Action;

public partial class CMSModules_OnlineMarketing_Controls_UI_MVTest_List : CMSAdminListControl
{
    #region "Variables"

    private string mEditPage = "Edit.aspx";
    private string mAliasPath = String.Empty;
    private bool mShowDeleteAction = true;
    private bool mShowPageColumn = true;

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
    /// Indicates if the column with original page should be displayed.
    /// </summary>
    public bool ShowPageColumn
    {
        get
        {
            return mShowPageColumn;
        }
        set
        {
            mShowPageColumn = value;
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
    /// If true, object menu is shown in grid.
    /// </summary>
    public bool ShowObjectMenu
    {
        get
        {
            return gridElem.ShowObjectMenu;
        }
        set
        {
            gridElem.ShowObjectMenu = value;
        }
    }


    /// <summary>
    /// If true, delete action is shown in grid.
    /// </summary>
    public bool ShowDeleteAction
    {
        get
        {
            return mShowDeleteAction;
        }
        set
        {
            mShowDeleteAction = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            gridElem.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Alias path of the document which this MVT tests belongs to.
    /// </summary>
    public string AliasPath
    {
        get
        {
            return mAliasPath;
        }
        set
        {
            mAliasPath = value;
        }
    }


    /// <summary>
    /// If true delayed reload is used for grid.
    /// </summary>
    public bool DelayedReload
    {
        get
        {
            return gridElem.DelayedReload;
        }
        set
        {
            gridElem.DelayedReload = value;
        }
    }


    /// <summary>
    /// Test edit page
    /// </summary>
    public String EditPage
    {
        get
        {
            return mEditPage;
        }
        set
        {
            mEditPage = value;
        }
    }


    /// <summary>
    /// NodeID of the current document. (Used for checking the access permissions).
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Enables or Disables grid filter.
    /// </summary>
    public bool ShowFilter
    {
        get
        {
            return gridElem.GridOptions.DisplayFilter;
        }
        set
        {
            gridElem.GridOptions.DisplayFilter = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetWhereCondition();

        if (!ShowPageColumn)
        {
            HideGridColumns(new[]
            {
                "MVTestPage"
            });
        }

        string url = UIContextHelper.GetElementUrl("CMS.MVTest", "Detail");

        url = URLHelper.AddParameterToUrl(url, "objectid", "{0}");
        url = URLHelper.AddParameterToUrl(url, "aliasPath", AliasPath);
        if (NodeID > 0)
        {
            url = URLHelper.AddParameterToUrl(url, "NodeID", NodeID.ToString());
        }

        gridElem.EditActionUrl = url;
    }


    /// <summary>
    /// Sets where condition to grid.
    /// </summary>
    private void SetWhereCondition()
    {
        // Add site dependency to where condition
        gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "MVTestSiteID = " + SiteContext.CurrentSiteID);

        if (AliasPath != String.Empty)
        {
            // Add alias path condition - used in document depending ABtest
            gridElem.WhereCondition = SqlHelper.AddWhereCondition(gridElem.WhereCondition, "MVTestPage = N'" + SqlHelper.EscapeQuotes(AliasPath) + "'");
        }
    }


    /// <summary>
    /// Hides any grid column.
    /// </summary>
    /// <param name="columnSourceNames">Column source name</param>
    private void HideGridColumns(IEnumerable<string> columnSourceNames)
    {
        if ((gridElem.GridColumns != null) && (columnSourceNames != null) && (columnSourceNames.Any()))
        {
            var columns = gridElem.GridColumns.Columns.Where(t => columnSourceNames.Contains(t.Source));

            foreach (var column in columns)
            {
                column.Visible = false;
            }
        }
    }


    /// <summary>
    /// Removes any action from UniGrid.
    /// </summary>
    /// <param name="actionNames">Action name</param>
    private void RemoveGridActions(IEnumerable<string> actionNames)
    {
        if ((gridElem.GridActions != null) && (gridElem.GridActions.Actions != null) && (actionNames != null) && (actionNames.Any()))
        {
            var actions = gridElem.GridActions.Actions.Cast<GridAction>().Where(t => actionNames.Contains(t.Name)).ToList();

            foreach (var action in actions)
            {
                gridElem.GridActions.Actions.Remove(action);
            }
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!ShowDeleteAction)
        {
            RemoveGridActions(new[]
            {
                "#delete"
            });
        }

        if (DelayedReload)
        {
            gridElem.ReloadData();
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Handles Unigrid's OnExternalDataBound event.
    /// </summary>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "mvteststatus":
                {
                    int testID = ValidationHelper.GetInteger(parameter, 0);
                    MVTestInfo testInfo = MVTestInfoProvider.GetMVTestInfo(testID);
                    return GetFormattedStatus(testInfo);
                }

            case "maxconversions":
                {
                    DataRowView drv = (DataRowView)parameter;
                    string conversions = ValidationHelper.GetString(drv["MVTestMaxConversions"], "");
                    if (!String.IsNullOrEmpty(conversions))
                    {
                        string type = ValidationHelper.GetString(drv["MVTestTargetConversionType"], "").ToLowerCSafe();
                        return conversions + " (" + GetString("om.conversion." + type) + ")";
                    }
                    return string.Empty;
                }
        }
        return parameter;
    }


    /// <summary>
    /// Gets formatted status string.
    /// </summary>
    private FormattedText GetFormattedStatus(MVTestInfo testInfo)
    {
        if (testInfo == null)
        {
            throw new ArgumentNullException("testInfo");
        }

        if (!testInfo.MVTestEnabled)
        {
            return new FormattedText(GetString("general.disabled")).ColorRed();
        }

        var status = new FormattedText(GetString("mvtest.status." + MVTestInfoProvider.GetMVTestStatus(testInfo)));
        if (MVTestInfoProvider.MVTestIsRunning(testInfo))
        {
            status.ColorGreen();
        }
        return status;
    }

    #endregion
}