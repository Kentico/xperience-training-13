using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Globalization;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;
using TreeNode = CMS.DocumentEngine.TreeNode;

[UIElement(ModuleName.CONTENT, "Properties.General")]
public partial class CMSModules_Content_CMSDesk_Properties_General : CMSPropertiesPage
{
    #region "Variables"

    protected bool canEditOwner = false;
    protected bool canEdit = true;

    protected FormEngineUserControl usrOwner = null;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        // Culture independent data
        SplitModeAllwaysRefresh = true;

        // Non-versioned data are modified
        DocumentManager.UseDocumentHelper = false;

        base.OnInit(e);

        // Check UI element permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.General"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.General");
        }

        // Init document manager events
        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        EnableSplitMode = true;

        // Set user control properties
        usrOwner = Page.LoadUserControl("~/CMSModules/Membership/FormControls/Users/selectuser.ascx") as FormEngineUserControl;
        if (usrOwner != null)
        {
            usrOwner.ID = "ctrlUsrOwner";
            usrOwner.IsLiveSite = false;
            usrOwner.SetValue("ShowSiteFilter", false);
            usrOwner.StopProcessing = pnlUIOwner.IsHidden;
            plcUsrOwner.Controls.Add(usrOwner);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterLoader(Page);
        ScriptHelper.RegisterTooltip(Page);
        ScriptHelper.RegisterDialogScript(this);

        // Set default item value
        ctrlSiteSelectStyleSheet.AddDefaultRecord = false;
        ctrlSiteSelectStyleSheet.CurrentSelector.SpecialFields.AllowDuplicates = true;

        if (PortalContext.CurrentSiteStylesheet != null)
        {
            ctrlSiteSelectStyleSheet.CurrentSelector.SpecialFields.Add(new SpecialField { Text = GetString("general.defaultchoice"), Value = GetDefaultStylesheet() });
        }
        else
        {
            ctrlSiteSelectStyleSheet.CurrentSelector.AllowEmpty = true;
        }

        ctrlSiteSelectStyleSheet.ReturnColumnName = "StyleSheetID";
        ctrlSiteSelectStyleSheet.SiteId = SiteContext.CurrentSiteID;

        if ((SiteContext.CurrentSite != null) && (usrOwner != null))
        {
            usrOwner.SetValue("SiteID", SiteContext.CurrentSite.SiteID);
        }

        StringBuilder script = new StringBuilder();

        if (Node != null)
        {
            // Redirect to information page when no UI elements displayed
            if (pnlUIDesign.IsHidden && pnlUIOther.IsHidden && pnlUIOwner.IsHidden && pnlUIAlias.IsHidden)
            {
                RedirectToUINotAvailable();
            }

            if (Node.IsRoot())
            {
                valAlias.Visible = false;
                txtAlias.Enabled = false;
                valAlias.Enabled = false;
            }
            else
            {
                txtAlias.Enabled = !TreePathUtils.AutomaticallyUpdateDocumentAlias(Node.NodeSiteName);
            }

            valAlias.ErrorMessage = GetString("GeneralProperties.RequiresAlias");

            txtAlias.MaxLength = TreePathUtils.MaxAliasLength;

            // Get strings for headings
            headOtherProperties.Text = GetString("GeneralProperties.OtherGroup");

            if (PortalContext.CurrentSiteStylesheet != null)
            {
                script.Append(@"
var currentStyleSheetId;
// Function raised before opening the Edit dialog of the CSS style sheet control. When 'default' style sheet is chosen, translate this value to the default site style sheet id.
function US_GetEditedItemId_", ctrlSiteSelectStyleSheet.ValueElementID, @"(selectedValue) {
    currentStyleSheetId = selectedValue;
    if (selectedValue == ""default"") {
        return ", PortalContext.CurrentSiteStylesheet.StylesheetID, @";
    }

    return selectedValue;
}

// Function raised from New/Edit dialog after save action. When 'default' style is used, the new/edit dialog will try to choose a real style sheet id (which was edited), but it is necessary keep the selected value to be 'default'.
function US_GetNewItemId_", ctrlSiteSelectStyleSheet.ValueElementID, @"(newStyleSheetId) {
    if ((currentStyleSheetId == ""default"") && (newStyleSheetId == ", PortalContext.CurrentSiteStylesheet.StylesheetID, @")) {
        return currentStyleSheetId;
    }

    return newStyleSheetId;
}");
            }

            canEditOwner = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);
            ctrlSiteSelectStyleSheet.AliasPath = Node.NodeAliasPath;

            ReloadData();
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ModalDialogsToAdvancedSection", script.ToString(), true);

        // Reflect processing action
        pnlContent.Enabled = DocumentManager.AllowSave;

        if (chkCssStyle.Checked && (PortalContext.CurrentSiteStylesheet != null))
        {
            // Enable the edit button
            ctrlSiteSelectStyleSheet.ButtonEditEnabled = true;
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (RequestHelper.IsPostBack())
        {
            ReloadData();
        }
    }

    #endregion


    #region "Private methods"

    private void ReloadData()
    {
        if (Node != null)
        {
            // Check modify permission
            canEdit = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) != AuthorizationResultEnum.Denied);

            // Show owner editing only when authorized to change the permissions
            if (canEditOwner)
            {
                lblOwner.Visible = false;
                usrOwner.Visible = true;
                usrOwner.SetValue("AdditionalUsers", new[] { Node.NodeOwner });
            }
            else
            {
                usrOwner.Visible = false;
            }

            if (!RequestHelper.IsPostBack())
            {
                if (canEditOwner)
                {
                    usrOwner.Value = Node.GetValue("NodeOwner");
                }

                txtAlias.Text = Node.NodeAlias;
            }

            // Load the data
            lblName.Text = HttpUtility.HtmlEncode(Node.GetDocumentName());

            lblAliasPath.Text = Convert.ToString(Node.NodeAliasPath);
            string typeName = DataClassInfoProvider.GetDataClassInfo(Node.NodeClassName).ClassDisplayName;
            lblType.Text = HttpUtility.HtmlEncode(ResHelper.LocalizeString(typeName));
            lblNodeID.Text = Convert.ToString(Node.NodeID);

            // Modifier
            SetUserLabel(lblLastModifiedBy, "DocumentModifiedByUserId");

            // Get modified time
            TimeZoneInfo usedTimeZone;
            DateTime lastModified = ValidationHelper.GetDateTime(Node.GetValue("DocumentModifiedWhen"), DateTimeHelper.ZERO_TIME);
            lblLastModified.Text = TimeZoneHelper.GetCurrentTimeZoneDateTimeString(lastModified, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, out usedTimeZone);
            ScriptHelper.AppendTooltip(lblLastModified, TimeZoneHelper.GetUTCLongStringOffset(usedTimeZone), "help");

            if (!canEditOwner)
            {
                // Owner
                SetUserLabel(lblOwner, "NodeOwner");
            }

            // Creator
            SetUserLabel(lblCreatedBy, "DocumentCreatedByUserId");
            DateTime createdWhen = ValidationHelper.GetDateTime(Node.GetValue("DocumentCreatedWhen"), DateTimeHelper.ZERO_TIME);
            lblCreated.Text = TimeZoneHelper.GetCurrentTimeZoneDateTimeString(createdWhen, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, out usedTimeZone);
            ScriptHelper.AppendTooltip(lblCreated, TimeZoneHelper.GetUTCLongStringOffset(usedTimeZone), "help");

            // URL
            if (Node.HasUrl())
            {
                string liveUrl = DocumentURLProvider.GetAbsoluteUrl(Node);

                if (!string.IsNullOrEmpty(liveUrl))
                {
                    plcLive.Visible = true;
                    lnkLiveURL.HRef = liveUrl;
                    lnkLiveURL.InnerText = liveUrl;
                }

                // Hide preview URL for root node
                if (!Node.IsRoot())
                {
                    var previewUrl = Node.GetPreviewLink(CurrentUser.UserName, embededInAdministration: false);
                    if (!string.IsNullOrEmpty(previewUrl))
                    {
                        plcPreview.Visible = true;
                        btnResetPreviewGuid.ToolTip = GetString("GeneralProperties.InvalidatePreviewURL");
                        btnResetPreviewGuid.Click += btnResetPreviewGuid_Click;
                        btnResetPreviewGuid.OnClientClick = "if(!confirm(" + ScriptHelper.GetLocalizedString("GeneralProperties.GeneratePreviewURLConf") + ")){return false;}";

                        SetPreviewUrl(previewUrl);
                    }
                }
            }

            lblGUID.Text = Convert.ToString(Node.NodeGUID);
            lblDocGUID.Text = (Node.DocumentGUID == Guid.Empty) ? ResHelper.Dash : Node.DocumentGUID.ToString();
            lblDocID.Text = Convert.ToString(Node.DocumentID);

            // Culture
            CultureInfo ci = CultureInfoProvider.GetCultureInfo(Node.DocumentCulture);
            lblCulture.Text = ((ci != null) ? ResHelper.LocalizeString(ci.CultureName) : Node.DocumentCulture);

            if (Node.IsPublished)
            {
                lblPublished.Text = GetString("General.Yes");
                lblPublished.CssClass += " DocumentPublishedYes";
            }
            else
            {
                lblPublished.CssClass += " DocumentPublishedNo";
                lblPublished.Text = GetString("General.No");
            }


            if (!RequestHelper.IsPostBack())
            {
                if (Node.IsRoot())
                {
                    chkCssStyle.Visible = false;
                }

                var defaultStylesheet = GetDefaultStylesheet();

                if (Node.DocumentInheritsStylesheet && !Node.IsRoot())
                {
                    chkCssStyle.Checked = true;

                    // Get stylesheet from the parent node
                    string value = GetStylesheetParentValue();
                    ctrlSiteSelectStyleSheet.Value = String.IsNullOrEmpty(value) ? defaultStylesheet : value;
                }
                else
                {
                    // Get stylesheet from the current node
                    var stylesheetId = Node.DocumentStylesheetID;
                    ctrlSiteSelectStyleSheet.Value = (stylesheetId == 0) ? defaultStylesheet : stylesheetId.ToString();
                }
            }

            // Disable new button if document inherit stylesheet
            bool disableCssSelector = (!Node.IsRoot() && chkCssStyle.Checked);
            ctrlSiteSelectStyleSheet.Enabled = !disableCssSelector;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = !disableCssSelector;

            // Initialize Rating control
            RefreshCntRatingResult();

            double rating = 0.0f;
            if (Node.DocumentRatings > 0)
            {
                rating = Node.DocumentRatingValue / Node.DocumentRatings;
            }
            ratingControl.MaxRating = 10;
            ratingControl.CurrentRating = rating;
            ratingControl.Visible = true;
            ratingControl.Enabled = false;

            // Initialize Reset button for rating
            btnResetRating.OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("GeneralProperties.ResetRatingConfirmation")) + ")) return false;";

            if (!canEdit)
            {
                // Disable form editing                                                            
                DisableFormEditing();
            }
        }
        else
        {
            btnResetRating.Visible = false;
        }
    }


    /// <summary>
    /// Initializes the label with specified user text.
    /// </summary>
    private void SetUserLabel(Label label, string columnName)
    {
        // Get the user ID
        int userId = ValidationHelper.GetInteger(Node.GetValue(columnName), 0);
        if (userId > 0)
        {
            // Get the user object
            UserInfo ui = UserInfoProvider.GetUserInfo(userId);
            if (ui != null)
            {
                label.Text = HTMLHelper.HTMLEncode(ui.FullName);
            }
        }
        else
        {
            label.Text = GetString("general.selectnone");
        }
    }


    private void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        SaveDocumentOwner(node);
        SaveDocumentStylesheet(node);
        SaveAlias(e);
    }


    private void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        txtAlias.Text = Node.NodeAlias;
    }


    private void SaveAlias(DocumentManagerEventArgs e)
    {
        if (pnlUIAlias.IsHidden)
        {
            return;
        }

        var node = e.Node;
        if (!String.IsNullOrEmpty(txtAlias.Text.Trim()) || node.IsRoot())
        {
            node.NodeAlias = txtAlias.Text.Trim();
        }
        else
        {
            e.IsValid = false;
            e.ErrorMessage = GetString("general.errorvalidationerror");
        }
    }


    private void SaveDocumentOwner(TreeNode node)
    {
        if (pnlUIOwner.IsHidden)
        {
            return;
        }

        int ownerId = ValidationHelper.GetInteger(usrOwner.Value, 0);
        node.SetValue("NodeOwner", (ownerId > 0) ? usrOwner.Value : null);
    }


    private void SaveDocumentStylesheet(TreeNode node)
    {
        if (pnlUIDesign.IsHidden)
        {
            return;
        }

        if (!chkCssStyle.Checked)
        {
            // Set style sheet
            int selectedCssId = ValidationHelper.GetInteger(ctrlSiteSelectStyleSheet.Value, 0);
            node.DocumentStylesheetID = (selectedCssId > 0) ? selectedCssId : 0;
            node.DocumentInheritsStylesheet = false;

            ctrlSiteSelectStyleSheet.Enabled = true;
        }
        else
        {
            ctrlSiteSelectStyleSheet.Enabled = false;

            node.DocumentInheritsStylesheet = true;
            node.DocumentStylesheetID = 0;
        }
    }


    protected void btnResetPreviewGuid_Click(object sender, EventArgs e)
    {
        if (Node == null)
        {
            return;
        }

        // Check modify permissions
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
        {
            return;
        }

        using (new CMSActionContext { LogEvents = false })
        {
            Node.DocumentWorkflowCycleGUID = Guid.NewGuid();
            Node.Update();
        }

        ShowConfirmation(GetString("GeneralProperties.PreviewLinkGenerated"));
        SetPreviewUrl(Node.GetPreviewLink(CurrentUser.UserName, embededInAdministration: false));
    }


    /// <summary>
    /// Disables form editing.
    /// </summary>
    protected void DisableFormEditing()
    {
        // Disable all panels
        pnlDesign.Enabled = false;
        pnlOwner.Enabled = false;

        // Disable 'save button'
        menuElem.Enabled = false;

        // Disable rating and owner selector
        btnResetPreviewGuid.Enabled = false;
        btnResetPreviewGuid.CssClass = "Disabled";
        btnResetRating.Enabled = false;
        usrOwner.Enabled = false;

        ctrlSiteSelectStyleSheet.Enabled = false;
        ctrlSiteSelectStyleSheet.ButtonNewEnabled = false;

        pnlAlias.Enabled = false;
    }


    protected void chkCssStyle_CheckedChanged(object sender, EventArgs e)
    {
        if (chkCssStyle.Checked)
        {
            // Set stylesheet to stylesheet selector
            ctrlSiteSelectStyleSheet.Enabled = false;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = false;

            string value = GetStylesheetParentValue();
            if (String.IsNullOrEmpty(value))
            {
                ctrlSiteSelectStyleSheet.Value = GetDefaultStylesheet();
            }
            else
            {
                try
                {
                    ctrlSiteSelectStyleSheet.Value = value;
                }
                catch
                {
                }
            }
        }
        else
        {
            ctrlSiteSelectStyleSheet.Enabled = true;
            ctrlSiteSelectStyleSheet.ButtonNewEnabled = true;
        }
    }


    /// <summary>
    /// Refreshes current rating result.
    /// </summary>
    protected void RefreshCntRatingResult()
    {
        string msg = null;

        // Avoid division by zero
        if ((Node != null) && (Node.DocumentRatings > 0))
        {
            msg = String.Format(GetString("GeneralProperties.ContentRatingResult"), (Node.DocumentRatingValue * 10) / Node.DocumentRatings, Node.DocumentRatings);
        }

        // Document wasn't rated
        if (msg == null)
        {
            msg = GetString("generalproperties.contentratingnoresult");
        }

        lblContentRatingResult.Text = msg;
    }


    /// <summary>
    /// Resets content rating score.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Args</param>
    protected void btnResetRating_Click(object sender, EventArgs e)
    {
        if (Node == null)
        {
            return;
        }

        // Check modify permissions
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
        {
            return;
        }

        // Reset rating
        TreeProvider.ResetRating(Node);
        RefreshCntRatingResult();
        ratingControl.CurrentRating = 0.0;
        ratingControl.ReloadData();

        ShowChangesSaved();
    }


    private void SetPreviewUrl(string url)
    {
        lnkPreviewURL.Attributes.Add("href", url);
    }


    /// <summary>
    /// Gets the default style sheet for the current site.
    /// </summary>
    private string GetDefaultStylesheet()
    {
        // If default stylesheet exists
        return (PortalContext.CurrentSiteStylesheet != null) ? "default" : null;
    }


    /// <summary>
    /// Gets stylesheet identifier value from parent node
    /// </summary>
    private string GetStylesheetParentValue()
    {
        var where = new WhereCondition().WhereNotEquals("DocumentInheritsStylesheet", true);

        return PageInfoProvider.GetParentProperty<string>(Node.NodeSiteID, Node.NodeAliasPath, "DocumentStylesheetID", Node.DocumentCulture, where);
    }

    #endregion
}