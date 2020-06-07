using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_OnlineMarketing_Controls_Content_CombinationPanel : CMSAbstractPortalUserControl, ICallbackEventHandler
{
    #region "Variables"

    private const string ITEM_ENABLED_CLASS = "DropDownItemEnabled";
    private const string ITEM_DISABLED_CLASS = "DropDownItemDisabled";
    private string callbackValue = string.Empty;
    private string cookieTestName = string.Empty;
    private ViewModeEnum viewMode = ViewModeEnum.Unknown;
    private CurrentUserInfo currentUser;

    /// <summary>
    /// Indicates whether processing should be stopped.
    /// </summary>
    private bool stopProcessing;

    #endregion


    #region "Methods"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        combinationSelector.UniSelector.UseUniSelectorAutocomplete = false;
        currentUser = MembershipContext.AuthenticatedUser;

        // Set the selected combination (from cookie by default)
        MVTestInfo mvTestInfo = MVTestInfoProvider.GetRunningTest(DocumentContext.CurrentAliasPath, SiteContext.CurrentSiteID, DocumentContext.CurrentDocumentCulture.CultureCode);

        // Get the cookie name
        if (mvTestInfo != null)
        {
            // Get a cookie name for the mvt test
            cookieTestName = CookieName.GetMVTCookieName(mvTestInfo.MVTestName);
        }
        else
        {
            // Get a template cookie name (used just in CMSDesk when no test is running)
            cookieTestName = CookieName.GetNoMVTCookieName(0);
        }

        // Move cookies expiration to next 30 days
        var cookieMVTTest = CookieHelper.GetExistingCookie(cookieTestName);
        if (cookieMVTTest != null)
        {
            CookieHelper.SetValue(cookieMVTTest.Name, cookieMVTTest.Value, cookieMVTTest.Path, DateTime.Now.AddDays(30), false);
        }

        base.OnInit(e);

        viewMode = PortalContext.ViewMode;

        // Check permissions
        if ((currentUser == null) || (!currentUser.IsAuthorizedPerResource("CMS.MVTest", "Read")))
        {
            stopProcessing = true;
        }
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (stopProcessing)
        {
            Visible = false;
            return;
        }

        // Show the warning panel when there is a running MVT test
        if ((viewMode != ViewModeEnum.Preview)
            && MVTestInfoProvider.ContainsRunningMVTest(DocumentContext.CurrentAliasPath, SiteContext.CurrentSiteID, DocumentContext.CurrentPageInfo.DocumentCulture))
        {
            DocumentManager.MessagesPlaceHolder.ShowWarning(ResHelper.GetString("mvtest.runningtestwarning", MembershipContext.AuthenticatedUser.PreferredUICultureCode));
        }

        if (RequestHelper.IsPostBack())
        {
            // Reload the combination panel because one of the combination could have been removed
            ReloadData(true);
        }

        // Set the OnChange attribute => Save the variant slider configuration into a cookie and raise a postback
        combinationSelector.UniSelector.OnBeforeClientChanged = "SaveCombinationPanelSelection(); " + Page.ClientScript.GetPostBackEventReference(this, "combinationchanged") + ";";

        // Show the combination panel only if there are any combinations for the document
        pnlMvtCombination.Enabled = combinationSelector.HasData;

        // Display the "set as result" button when there any MVT variants in the page
        plcUseCombination.Visible = combinationSelector.DropDownSelect.Items.Count > 1;
    }


    /// <summary>
    /// Creates child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (!stopProcessing)
        {
            SetupControl();
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    /// <param name="forceLoad">If set to <c>true</c>, reload the control even if the control has been already reloaded</param>
    protected void ReloadData(bool forceLoad)
    {
        combinationSelector.ReloadData(forceLoad);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setups this control.
    /// </summary>
    private void SetupControl()
    {
        // Check permissions
        if ((currentUser == null)
            || (!currentUser.IsAuthorizedPerResource("CMS.MVTest", "Manage")))
        {
            plcEditCombination.Visible = false;
        }

        // Set callback events
        btnChange.OnClientClick = Page.ClientScript.GetCallbackEventReference(this, "MVTSetCustomName()", "MVTReceiveServerData", null) + ";return false;";
        chkEnabled.Attributes.Add("onclick", Page.ClientScript.GetCallbackEventReference(this, "MVTSetEnabled()", "MVTReceiveServerData", null) + ";return true;");

        btnUseCombination.OnClientClick = "if (!confirm(" + ScriptHelper.GetLocalizedString("mvt.usecombinationconfirm") + ")) { return false; }";

        if ((DocumentContext.CurrentPageInfo != null) && (DocumentContext.CurrentPageInfo.UsedPageTemplateInfo != null))
        {
            // Get the current document ID
            int documentId = 0;
            if (DocumentContext.CurrentDocument != null)
            {
                documentId = DocumentContext.CurrentDocument.DocumentID;
            }

            // Display the combinations only for the current document
            combinationSelector.DocumentID = documentId;
            combinationSelector.PageTemplateID = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;
        }

        // Setup the localized strings
        string prefferedUICode = currentUser.PreferredUICultureCode;
        lblCombination.Text = ResHelper.GetString("mvtcombination.name", prefferedUICode);
        chkEnabled.Text = ResHelper.GetString("general.enabled", prefferedUICode);
        lblCustomName.Text = ResHelper.GetString("mvtcombination.customName", prefferedUICode);
        btnChange.Text = ResHelper.GetString("general.change", prefferedUICode);
        lblSaved.Text = ResHelper.GetString("mvtvariant.customnamesaved", prefferedUICode);
        btnUseCombination.Text = ResHelper.GetString("mvt.usecombination", prefferedUICode);

        // Hide label "Saved"
        lblSaved.Style.Add("display", "none");

        // Setup the combination uniselector
        combinationSelector.UniSelector.OrderBy = "MVTCombinationName";
        combinationSelector.UniSelector.AllowEmpty = false;

        // Ensure a full postback for the uniselector (this is set manually due to the Update panel in the uniselector)
        ScriptManager scr = ScriptManager.GetCurrent(Page);
        scr.RegisterPostBackControl(combinationSelector.DropDownSelect);

        // Register page javascript
        StringBuilder sb = new StringBuilder();
        sb.Append(@"
            function MVTSetCustomName() {
                var ddlCombinations = document.getElementById('", combinationSelector.DropDownSelect.ClientID, @"');
                var combinationId = ddlCombinations.options[ddlCombinations.selectedIndex].value;
                var txtCustomName = document.getElementById('", txtCustomName.ClientID, @"');
                return combinationId + ';cname;' + txtCustomName.value;
            }

            function MVTSetEnabled() {
                var ddlCombinations = document.getElementById('", combinationSelector.DropDownSelect.ClientID, @"');
                var combinationId = ddlCombinations.options[ddlCombinations.selectedIndex].value;
                var chkEnabled = document.getElementById('", chkEnabled.ClientID, @"');
                return combinationId + ';enabled;' + chkEnabled.checked;
            }

            function MVTReceiveServerData(value) {
                var ddlCombinations = document.getElementById('", combinationSelector.DropDownSelect.ClientID, @"');
                var listitem = ddlCombinations.options[ddlCombinations.selectedIndex];
                if (value.length > 0) {
                    /* Custom name changed */
                    listitem.text = value;

                    /* Update the combinationsArray array */
                    combinationsArray[ddlCombinations.selectedIndex][2] = document.getElementById('", txtCustomName.ClientID, @"').value;

                    document.getElementById('", lblSaved.ClientID, @"').style.display = 'inline-block';
                    setTimeout(""document.getElementById('", lblSaved.ClientID, @"').style.display = 'none'"", 4000);
                }
                else {
                    /* Enabled changed */
                    var chkEnabled = document.getElementById('", chkEnabled.ClientID, @"');
                    if (listitem != null) {
                        if (chkEnabled.checked) {
                            listitem.className = '", ITEM_ENABLED_CLASS, @"';
                        }
                        else {
                            listitem.className = '", ITEM_DISABLED_CLASS, @"';
                        }
                    }

                    /* Update the combinationsArray array */
                    combinationsArray[ddlCombinations.selectedIndex][1] = chkEnabled.checked;
                }
            }

            function SaveCombinationPanelSelection() {
                var ddlCombinations = document.getElementById('", combinationSelector.DropDownSelect.ClientID, @"');
                var listitem = ddlCombinations.options[ddlCombinations.selectedIndex];
                var combinationName = listitem.value;
                if (combinationName.length > 0) {
                    $cmsj.cookie('", cookieTestName, @"', combinationName, { expires: 7, path: '/' });
                }
                else {
                    $cmsj.cookie('", cookieTestName, @"', null);
                }
            }
        ");

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "MVTCombinationPanel", sb.ToString(), true);

        ReloadData(false);
    }

    #endregion


    #region "Callback handling"

    /// <summary>
    /// Callback event handler.
    /// </summary>
    /// <param name="argument">Callback argument</param>
    public void RaiseCallbackEvent(string argument)
    {
        // Check permissions
        if ((currentUser == null)
            || (!currentUser.IsAuthorizedPerResource("CMS.MVTest", "Manage"))
            || stopProcessing)
        {
            return;
        }

        // Get arguments
        if (!string.IsNullOrEmpty(argument))
        {
            string[] args = argument.Split(new[] { ';' }, 3);
            if (args.Length == 3)
            {
                string combinationName = ValidationHelper.GetString(args[0], string.Empty);
                string action = args[1].ToLowerCSafe();
                string newValue = args[2];

                // Get the combination info
                MVTCombinationInfo mvtcInfo = MVTCombinationInfoProvider.GetMVTCombinationInfo(combinationSelector.PageTemplateID, combinationName);
                if (mvtcInfo != null)
                {
                    switch (action)
                    {
                        case "cname":
                            // Custom name changed
                            mvtcInfo.MVTCombinationCustomName = newValue;
                            if (string.IsNullOrEmpty(newValue))
                            {
                                newValue = mvtcInfo.MVTCombinationName;
                            }
                            // return the new value (when newValue=="", then return combination code name)
                            callbackValue = newValue;
                            break;

                        case "enabled":
                            // combination Enabled changed
                            mvtcInfo.MVTCombinationEnabledOriginal = mvtcInfo.MVTCombinationEnabled;
                            mvtcInfo.MVTCombinationEnabled = ValidationHelper.GetBoolean(newValue, true);
                            callbackValue = string.Empty;
                            break;

                        default:
                            return;
                    }

                    MVTCombinationInfoProvider.SetMVTCombinationInfo(mvtcInfo);

                    // Synchronize widget variants if enabling combination
                    if ((mvtcInfo.MVTCombinationDocumentID > 0)
                        || (!mvtcInfo.MVTCombinationEnabledOriginal && mvtcInfo.MVTCombinationEnabled
                           ))
                    {
                        // Log synchronization
                        TreeProvider tree = new TreeProvider(currentUser);
                        TreeNode node = tree.SelectSingleDocument(mvtcInfo.MVTCombinationDocumentID);

                        if (node != null)
                        {
                            DocumentSynchronizationHelper.LogDocumentChange(node, TaskTypeEnum.UpdateDocument, tree);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        // Return combination custom name or combination code name
        return callbackValue;
    }

    #endregion
}
