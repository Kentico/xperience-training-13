using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DocumentEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Content_Controls_Dialogs_Selectors_LinkMediaSelector_InnerMediaView : ContentInnerMediaView
{
    #region "Properties"

    /// <summary>
    /// Gets a UniGrid control used to display files in LIST view mode.
    /// </summary>
    public override UniGrid ListViewControl
    {
        get
        {
            return gridList;
        }
    }


    /// <summary>
    /// Gets a repeater control used to display files in THUMBNAILS view mode.
    /// </summary>
    public override BasicRepeater ThumbnailsViewControl
    {
        get
        {
            return repThumbnailsView;
        }
    }


    /// <summary>
    /// Gets a page size drop down list control used in THUMBNAILS view mode.
    /// </summary>
    public override DropDownList PageSizeDropDownList
    {
        get
        {
            return pagerElemThumbnails.PageSizeDropdown;
        }
    }


    /// <summary>
    /// Returns current page size.
    /// </summary>
    public override int CurrentPageSize
    {
        get
        {
            int pageSize = 0;

            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    pageSize = pagerElemThumbnails.CurrentPageSize;
                    break;

                case DialogViewModeEnum.ListView:
                    pageSize = (gridList.Pager.CurrentPageSize == -1) ? 0 : ValidationHelper.GetInteger(gridList.Pager.CurrentPageSize, 10);
                    break;
            }

            return pageSize;
        }
    }


    /// <summary>
    /// Returns current offset.
    /// </summary>
    public override int CurrentOffset
    {
        get
        {
            int offset = 0;

            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    offset = pagerElemThumbnails.CurrentPageSize * (pagerElemThumbnails.UniPager.CurrentPage - 1);
                    break;

                case DialogViewModeEnum.ListView:
                    offset = gridList.Pager.CurrentPageSize * (gridList.Pager.CurrentPage - 1);
                    break;
            }

            return offset;
        }
    }


    /// <summary>
    /// Gets or sets current page.
    /// </summary>
    public override int CurrentPage
    {
        get
        {
            int page = 0;

            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    page = pagerElemThumbnails.UniPager.CurrentPage;
                    break;

                case DialogViewModeEnum.ListView:
                    page = gridList.Pager.CurrentPage;
                    break;
            }

            return page;
        }
        set
        {
            switch (ViewMode)
            {
                case DialogViewModeEnum.ThumbnailsView:
                    pagerElemThumbnails.UniPager.CurrentPage = value;
                    break;

                case DialogViewModeEnum.ListView:
                    gridList.Pager.CurrentPage = value;
                    break;
            }
        }
    }


    /// <summary>
    /// Gets list of names of selected files.
    /// </summary>
    public List<string> SelectedItems
    {
        get
        {
            return GetSelectedItems();
        }
    }

    #endregion


    #region "Page events"


    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ListViewControl.OnExternalDataBound += ListViewControl_OnExternalDataBound;
        ListViewControl.GridView.RowDataBound += GridView_RowDataBound;

        ThumbnailsViewControl.ItemDataBound += ThumbnailsViewControl_ItemDataBound;

        base.OnInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = !StopProcessing;
        if (!StopProcessing)
        {
            gridList.IsLiveSite = IsLiveSite;
        }

        if (RequestHelper.IsPostBack())
        {
            Reload(true);
        }
    }


    /// <summary>
    /// Reloads control with data.
    /// </summary>
    /// <param name="forceSetup">Indicates whether the inner controls should be re-setuped</param>
    public void Reload(bool forceSetup)
    {
        Visible = !StopProcessing;
        if (Visible)
        {
            if (forceSetup)
            {
                // Initialize controls
                SetupControls();
            }

            // Load passed data
            ReloadData();
        }
    }


    /// <summary>
    /// Loads data from the data source property.
    /// </summary>
    public void ReloadData()
    {
        LoadFileData();

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                ReloadListView();
                break;

            case DialogViewModeEnum.ThumbnailsView:
                ReloadThumbnailsView();
                break;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Display information on empty data
        bool isEmpty = DataHelper.DataSourceIsEmpty(DataSource);
        if (isEmpty)
        {
            plcViewArea.Visible = false;
        }
        else
        {
            lblInfo.Visible = false;
            plcViewArea.Visible = true;
        }

        // If info text is set display it
        if (!string.IsNullOrEmpty(InfoText))
        {
            lblInfo.Text = InfoText;
            lblInfo.Visible = true;
        }
        else if (isEmpty)
        {
            lblInfo.Text = (IsCopyMoveLinkDialog ? GetString("media.copymove.empty") : CMSDialogHelper.GetNoItemsMessage(Config, SourceType));
            lblInfo.Visible = true;
        }

        // Hide column 'Update' in media libraries
        if (((SourceType == MediaSourceEnum.MediaLibraries) || (SourceType == MediaSourceEnum.Content)) && !columnUpdateVisible)
        {
            if (gridList.NamedColumns.ContainsKey("extedit"))
            {
                gridList.NamedColumns["extedit"].Visible = false;
            }
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Initializes all nested controls.
    /// </summary>
    private void SetupControls()
    {
        // Initialize displaying controls according view mode
        LoadViewControls();

        InitializeControlScripts(hdnItemToColorize.ClientID);

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                InitializeListView();
                break;

            case DialogViewModeEnum.ThumbnailsView:
                InitializeThumbnailsView();
                break;
        }

        // Register delete confirmation script
        ltlScript.Text = ScriptHelper.GetScript(String.Format("function DeleteConfirmation(){{ return confirm({0}); }} function DeleteMediaFileConfirmation(){{ return confirm({1}); }}",
                                                              ScriptHelper.GetLocalizedString("attach.deleteconfirmation"),
                                                              ScriptHelper.GetLocalizedString("general.deleteconfirmation")));
    }

    #endregion


    #region "View methods"

    /// <summary>
    /// Initializes list view controls.
    /// </summary>
    private void InitializeListView()
    {
        switch (SourceType)
        {
            case MediaSourceEnum.Content:
                gridList.OrderBy = "NodeOrder";
                break;

            case MediaSourceEnum.MediaLibraries:
                gridList.OrderBy = "FileName";
                break;

            case MediaSourceEnum.DocumentAttachments:
                gridList.OrderBy = "AttachmentOrder, AttachmentName";
                break;

            case MediaSourceEnum.MetaFile:
                gridList.OrderBy = "MetaFileName, MetaFileID";
                break;
        }

        gridList.LoadGridDefinition();
        gridList.OnBeforeDataReload += gridList_OnBeforeDataReload;
    }


    private void gridList_OnBeforeDataReload()
    {
        gridList.PagerForceNumberOfResults = TotalRecords;
        gridList.DataSource = DataSource;
    }


    /// <summary>
    /// Reloads list view according source type.
    /// </summary>
    private void ReloadListView()
    {
        // Fill the grid data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            // Disable sorting if is copy/move dialog
            if ((IsCopyMoveLinkDialog) && (DisplayMode == ControlDisplayModeEnum.Simple))
            {
                gridList.GridView.AllowSorting = false;
            }
            gridList.ReloadData();
        }
    }


    /// <summary>
    /// Ensures no item is selected.
    /// </summary>
    public void ResetListSelection()
    {
        if (gridList != null)
        {
            gridList.ResetSelection();
        }
    }


    /// <summary>
    /// Ensures first page is displayed in the control displaying the content.
    /// </summary>
    public void ResetPageIndex()
    {
        if (ViewMode == DialogViewModeEnum.ListView)
        {
            ListViewControl.Pager.UniPager.CurrentPage = 1;
        }
        else
        {
            pagerElemThumbnails.CurrentPage = 1;
        }
    }


    /// <summary>
    /// Initializes controls for the thumbnails view mode.
    /// </summary>
    private void InitializeThumbnailsView()
    {
        // Basic control properties
        repThumbnailsView.HideControlForZeroRows = true;

        // UniPager properties     
        SetupPager(pagerElemThumbnails.UniPager);

        // Initialize page size
        bool isAttachmentTab = SourceType == MediaSourceEnum.DocumentAttachments;
        pagerElemThumbnails.PageSizeOptions = isAttachmentTab ? "12,24,48,96" : "10,20,50,100";
        pagerElemThumbnails.DefaultPageSize = isAttachmentTab ? 12 : 10;
    }


    /// <summary>
    /// Loads content for media libraries thumbnails view element.
    /// </summary>
    private void ReloadThumbnailsView()
    {
        // Connects repeater with data source
        if (!DataHelper.DataSourceIsEmpty(DataSource))
        {
            // Ensures paging for thumbnails in LinkMediaSelector with search string
            int pagerForceNumberOfResults = TotalRecords;
            var pageDataSet = DataHelper.TrimDataSetPage(DataSource, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);
            repThumbnailsView.DataSource = pageDataSet;
            repThumbnailsView.PagerForceNumberOfResults = pagerForceNumberOfResults;
            repThumbnailsView.DataBind();
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Loads view controls according currently set view mode.
    /// </summary>
    public void LoadViewControls()
    {
        InfoText = "";

        // Select mode according required
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                plcListView.Visible = true;

                // Stop processing                
                plcThumbnailsView.Visible = false;
                break;

            case DialogViewModeEnum.ThumbnailsView:
                plcThumbnailsView.Visible = true;
                repThumbnailsView.DataBindByDefault = false;
                pagerElemThumbnails.UniPager.ConnectToPagedControl(repThumbnailsView);

                // Stop processing
                plcListView.Visible = false;
                break;
        }

        // Display mass actions drop-down list if displayed for MediaLibrary UI
        if (!IsCopyMoveLinkDialog && (DisplayMode == ControlDisplayModeEnum.Simple))
        {
            pnlMassAction.Visible = true;

            InitializeMassActions();
        }
        else
        {
            pnlMassAction.Visible = false;
        }
    }



    /// <summary>
    /// Initializes mass actions drop-down list with available actions.
    /// </summary>
    private void InitializeMassActions()
    {
        const string actionScript = @"
function RaiseMassAction(drpActionsClientId, drpActionFilesClientId) {
    var drpActions = document.getElementById(drpActionsClientId);
    var drpActionFiles = document.getElementById(drpActionFilesClientId);
    if((drpActions != null) && (drpActionFiles != null)) {
        var selectedFiles = drpActionFiles.options[drpActionFiles.selectedIndex];
        var selectedAction = drpActions.options[drpActions.selectedIndex];
        if((selectedAction != null) && (selectedFiles != null)) {
            var argument = selectedAction.value + '|' + selectedFiles.value;
            SetAction('massaction', argument);
            RaiseHiddenPostBack();
        }                   
    } 
}";

        ScriptHelper.RegisterStartupScript(Page, typeof(Page), "LibraryActionScript", ScriptHelper.GetScript(actionScript));

        if (drpActionFiles.Items.Count == 0)
        {
            // Actions dropdown
            drpActionFiles.Items.Add(new ListItem(GetString("media.file.list.lblactions"), "selected"));
            drpActionFiles.Items.Add(new ListItem(GetString("media.file.list.filesall"), "all"));
        }

        if (drpActions.Items.Count == 0)
        {
            // Actions dropdown
            drpActions.Items.Add(new ListItem(GetString("General.SelectAction"), ""));
            drpActions.Items.Add(new ListItem(GetString("media.file.copy"), "copy"));
            drpActions.Items.Add(new ListItem(GetString("media.file.move"), "move"));
            drpActions.Items.Add(new ListItem(GetString("General.Delete"), "delete"));
            drpActions.Items.Add(new ListItem(GetString("media.file.import"), "import"));
        }

        btnActions.OnClientClick = String.Format("if(MassConfirm('{0}', {1})) {{ RaiseMassAction('{0}', '{2}'); }} return false;", drpActions.ClientID, ScriptHelper.GetLocalizedString("General.ConfirmGlobalDelete"), drpActionFiles.ClientID);
    }


    /// <summary>
    /// Returns list of names of selected files.
    /// </summary>
    private List<string> GetSelectedItems()
    {
        switch (ViewMode)
        {
            case DialogViewModeEnum.ListView:
                return gridList.SelectedItems;

            case DialogViewModeEnum.ThumbnailsView:
                return GetThumbsSelectedItems();
        }

        return null;
    }


    /// <summary>
    /// Returns list of names of files selected in thumbnails view mode.
    /// </summary>
    private List<string> GetThumbsSelectedItems()
    {
        List<string> result = new List<string>();

        // Go through all repeater items and look for selected ones
        foreach (RepeaterItem item in repThumbnailsView.Items)
        {
            CMSCheckBox chkSelected = item.FindControl("chkSelected") as CMSCheckBox;
            if ((chkSelected != null) && chkSelected.Checked)
            {
                HiddenField hdnItemName = item.FindControl("hdnItemName") as HiddenField;
                if (hdnItemName != null)
                {
                    string alt = hdnItemName.Value;
                    result.Add(alt);
                }
            }
        }

        return result;
    }

    #endregion
}
