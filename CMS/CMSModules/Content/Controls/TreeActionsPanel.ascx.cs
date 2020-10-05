using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_TreeActionsPanel : CMSUserControl, ICallbackEventHandler
{
    #region "Variables"

    private Dictionary<string, string> mJsModuleData;
    private string mNodePreviewUrl = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// Dictionary containing button's arguments for JavaScript module
    /// </summary>
    public Dictionary<string, string> JsModuleData
    {
        get
        {
            return mJsModuleData ?? (mJsModuleData = new Dictionary<string, string>());
        }
    }


    /// <summary>
    /// Mode to be considered as selected.
    /// </summary>
    public string SelectedMode
    {
        get
        {
            string mode = ValidationHelper.GetString(GetValue("SelectedMode"), null);
            if (String.IsNullOrEmpty(mode))
            {
                mode = ValidationHelper.GetString(UIContext[UIContextDataItemName.SELECTEDMODE], "edit");
            }
            return mode;
        }
        set
        {
            SetValue("SelectedMode", value);
        }
    }


    /// <summary>
    /// Indicates if control is placed in Ecommerce UI and some buttons should not be visible.
    /// </summary>
    public bool ShowModeMenu
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowModeMenu"), true);
        }
        set
        {
            SetValue("ShowModeMenu", value);
        }
    }

    #endregion


    #region "Methods & Events"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControls();
        }

        string getPreviewUrlScript = $@"function GetCurrentNodePreviewUrl() {{
            var nodeId = GetSelectedNodeId();
            var cultureCode = GetSelectedCulture();
            var argument = nodeId + ';' + cultureCode;

            { Page.ClientScript.GetCallbackEventReference(this, "argument", "OpenInNewTabCallback", null) }
        }}
";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "GetPreviewUrlScript", getPreviewUrlScript, true);
    }


    /// <summary>
    /// Setups controls.
    /// </summary>
    private void SetupControls()
    {
        if (ShowModeMenu)
        {
            InitViewModeButtons();
        }
        else
        {
            plcContentButtons.Visible = false;
        }

        InitActionButtons();
    }


    /// <summary>
    /// Initializes action button's tooltips and screen reader description.
    /// </summary>
    private void InitActionButtons()
    {
        btnNew.ToolTip = GetString("content.ui.new");
        btnDelete.ToolTip = GetString("content.ui.delete");
        btnCopy.ToolTip = GetString("content.ui.copy");
        btnMove.ToolTip = GetString("content.ui.move");
        btnUp.ToolTip = GetString("content.ui.up");
        btnDown.ToolTip = GetString("content.ui.down");
        btnRefresh.ToolTip = GetString("content.ui.refresh");

        // Hide search button in Ecommerce
        if (ShowModeMenu)
        {
            btnSearch.ToolTip = GetString("general.search");
        }
        else
        {
            btnSearch.Visible = false;
        }
    }


    /// <summary>
    /// Initializes view mode switchers.
    /// </summary>
    private void InitViewModeButtons()
    {
        btnEdit.Attributes.Add("data-view-mode", "edit");
        btnPreview.Attributes.Add("data-view-mode", "preview");
        btnListing.Attributes.Add("data-view-mode", "listing");

        btnEdit.ToolTip = GetString("mode.edittooltip");
        btnPreview.ToolTip = GetString("mode.previewtooltip");
        btnListing.ToolTip = GetString("mode.listingtooltip");

        btnEdit.Text = GetString("content.ui.edit");
        btnPreview.Text = GetString("content.ui.preview");
        btnListing.Text = GetString("content.ui.list");

        string elemsIds = "#" + btnEdit.ClientID + "," + "#" + btnPreview.ClientID + "," + "#" + btnListing.ClientID;

        // Add parameters for JavaScript module
        JsModuleData.Add("elemsSelector", elemsIds);

        string defaultClientID;
        switch (SelectedMode)
        {
            case "preview":
                defaultClientID = btnPreview.ClientID;
                break;

            case "listing":
                defaultClientID = btnListing.ClientID;
                break;

            default:
                defaultClientID = btnEdit.ClientID;
                break;
        }

        JsModuleData.Add("defaultSelection", "#" + defaultClientID);

        ScriptHelper.RegisterModule(Page, "CMS/ContentMenu", JsModuleData);
    }


    /// <summary>
    /// Callback event handler.
    /// </summary>
    /// <param name="argument">Callback argument</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (String.IsNullOrEmpty(eventArgument))
        {
            return;
        }

        var parts = eventArgument.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            mNodePreviewUrl = DocumentUIHelper.GetDocumentPageUrl(new UIPageURLSettings
            {
                NodeID = ValidationHelper.GetInteger(parts[0], 0),
                Culture = parts[1],
                Mode = ViewModeEnum.Preview.ToString(),
                AllowViewValidate = false,
            });
        }

        mNodePreviewUrl = mNodePreviewUrl ?? DocumentUIHelper.GetPageNotAvailableUrl();
    }


    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        return mNodePreviewUrl;
    }

    #endregion
}
