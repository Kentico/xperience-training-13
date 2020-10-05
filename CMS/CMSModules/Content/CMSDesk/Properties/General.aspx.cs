using System;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Globalization;
using CMS.Localization;
using CMS.Membership;
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
    
    protected bool IsAdvancedMode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
        }
        set
        {
            ViewState["IsAdvancedMode"] = value;
        }
    }

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

        if ((SiteContext.CurrentSite != null) && (usrOwner != null))
        {
            usrOwner.SetValue("SiteID", SiteContext.CurrentSite.SiteID);
        }

        StringBuilder script = new StringBuilder();

        if (Node != null)
        {
            // Redirect to information page when no UI elements displayed
            if (pnlUIOther.IsHidden && pnlUIOwner.IsHidden && pnlUIAlias.IsHidden)
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

            canEditOwner = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);            

            ReloadData();
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ModalDialogsToAdvancedSection", script.ToString(), true);

        // Reflect processing action
        pnlContent.Enabled = DocumentManager.AllowSave;
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

        SetAdvancedSection();
    }


    private void SetAdvancedSection()
    {
        icAdvanced.CssClass = IsAdvancedMode ? "icon-caret-up cms-icon-30" : "icon-caret-down cms-icon-30";
        lnkAdvanced.ResourceString = IsAdvancedMode ? "content.ui.properties.simplified" : "general.advanced";
        plcAdvanced.Visible = IsAdvancedMode;
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
                chkExcludeFromSearch.Checked = Node.DocumentSearchExcluded;
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

            lblGUID.Text = Convert.ToString(Node.NodeGUID);
            lblDocGUID.Text = (Node.DocumentGUID == Guid.Empty) ? ResHelper.Dash : Node.DocumentGUID.ToString();
            lblDocID.Text = Convert.ToString(Node.DocumentID);

            // Culture
            CultureInfo ci = CultureInfo.Provider.Get(Node.DocumentCulture);
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

            if (!canEdit)
            {
                // Disable form editing                                                            
                DisableFormEditing();
            }
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
            UserInfo ui = UserInfo.Provider.Get(userId);
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
        SaveAlias(e);
        SaveSearch(node);
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


    private void SaveSearch(TreeNode node)
    {
        if (!pnlUISearch.IsHidden)
        {
            node.DocumentSearchExcluded = chkExcludeFromSearch.Checked;
        }
    }


    /// <summary>
    /// Disables form editing.
    /// </summary>
    protected void DisableFormEditing()
    {
        // Disable all panels
        pnlOwner.Enabled = false;

        // Disable 'save button'
        menuElem.Enabled = false;
        
        usrOwner.Enabled = false;

        pnlAlias.Enabled = false;
    }


    /// <summary>
    /// Switches simple/advanced mode.
    /// </summary>
    protected void advancedLink_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = !IsAdvancedMode;

        ScriptHelper.RegisterStartupScript(this, typeof(string), "InitUpdatePanelChanges", "CMSContentManager.initChanges();", true);
    }

    #endregion
}