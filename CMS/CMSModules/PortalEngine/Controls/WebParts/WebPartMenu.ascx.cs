using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_PortalEngine_Controls_WebParts_WebPartMenu : CMSAbstractPortalUserControl
{
    #region "Variables"

    // Column names
    private string columnVariantID = string.Empty;
    private string columnVariantDisplayName = string.Empty;
    private string columnVariantZoneID = string.Empty;
    private string columnVariantPageTemplateID = string.Empty;
    private string columnVariantInstanceGUID = string.Empty;
    private string updateCombinationPanelScript = string.Empty;
    private CurrentUserInfo currentUser = null;
    private string mUICulture = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the UI culture for localized strings
    /// </summary>
    private string UICulture
    {
        get
        {
            if (string.IsNullOrEmpty(mUICulture))
            {
                mUICulture = CultureHelper.GetPreferredUICultureCode();
            }

            return mUICulture;
        }
    }

    #endregion


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        currentUser = MembershipContext.AuthenticatedUser;

        // Hide the add MVT/CP variant when Manage permission is not allowed
        if (!currentUser.IsAuthorizedPerResource("cms.mvtest", "Manage"))
        {
            plcAddMVTVariant.Visible = false;
        }

        if (!currentUser.IsAuthorizedPerResource("cms.contentpersonalization", "Manage"))
        {
            plcAddCPVariant.Visible = false;
        }

        string click = null;

        // Main menu
        iProperties.Text = ResHelper.GetString("WebPartMenu.IconProperties", UICulture);
        iProperties.Attributes.Add("onclick", "ContextConfigureWebPart();");

        // Up menu - Bottom
        iTop.Text = ResHelper.GetString("UpMenu.IconTop", UICulture);

        iForwardAll.Text = ResHelper.GetString("WebPartMenu.IconForward", UICulture);

        click = "ContextMoveWebPartTop();";
        iForwardAll.Attributes.Add("onclick", click);
        iTop.Attributes.Add("onclick", click);

        // Up
        iUp.Text = ResHelper.GetString("WebPartMenu.IconUp", UICulture);

        click = "ContextMoveWebPartUp();";
        iUp.Attributes.Add("onclick", click);

        // Down
        iDown.Text = ResHelper.GetString("WebPartMenu.IconDown", UICulture);

        click = "ContextMoveWebPartDown();";
        iDown.Attributes.Add("onclick", click);

        // Down menu - Bottom
        iBottom.Text = ResHelper.GetString("DownMenu.IconBottom", UICulture);

        iBackwardAll.Text = ResHelper.GetString("WebPartMenu.IconBackward", UICulture);

        click = "ContextMoveWebPartBottom();";
        iBackwardAll.Attributes.Add("onclick", click);
        iBottom.Attributes.Add("onclick", click);

        // Move to
        iMoveTo.Text = ResHelper.GetString("WebPartMenu.IconMoveTo", UICulture);

        // Copy
        iCopy.Text = ResHelper.GetString("WebPartMenu.copy", UICulture);
        iCopy.Attributes.Add("onclick", "ContextCopyWebPart(this);");

        // Paste
        iPaste.Text = ResHelper.GetString("WebPartMenu.paste", UICulture);
        iPaste.Attributes.Add("onclick", "ContextPasteWebPart(this);");
        iPaste.ToolTip = ResHelper.GetString("WebPartMenu.pasteTooltip", UICulture);

        // Delete
        iDelete.Text = ResHelper.GetString("general.remove", UICulture);
        iDelete.Attributes.Add("onclick", "ContextRemoveWebPart();");

        // Add new MVT variant
        lblAddMVTVariant.Text = ResHelper.GetString("WebPartMenu.AddWebPartVariant", UICulture);
        pnlAddMVTVariant.Attributes.Add("onclick", "ContextAddWebPartMVTVariant();");

        // Add new Content personalization variant
        lblAddCPVariant.Text = ResHelper.GetString("WebPartMenu.AddWebPartVariant", UICulture);
        pnlAddCPVariant.Attributes.Add("onclick", "ContextAddWebPartCPVariant();");

        // List all variants
        lblMVTVariants.Text = ResHelper.GetString("WebPartMenu.WebPartMVTVariants", UICulture);
        lblCPVariants.Text = ResHelper.GetString("WebPartMenu.WebPartPersonalizationVariants", UICulture);

        // No MVT variants
        lblNoWebPartMVTVariants.Text = ResHelper.GetString("ZoneMenu.NoVariants", UICulture);

        // No CP variants
        lblNoWebPartCPVariants.Text = ResHelper.GetString("ZoneMenu.NoVariants", UICulture);

        if (PortalManager.CurrentPlaceholder != null)
        {
            // Build the list of web part zones
            var webPartZones = new List<CMSWebPartZone>();

            if (PortalManager.CurrentPlaceholder.WebPartZones != null)
            {
                foreach (CMSWebPartZone zone in PortalManager.CurrentPlaceholder.WebPartZones)
                {
                    // Add only standard zones to the list
                    if ((zone.ZoneInstance.WidgetZoneType == WidgetZoneTypeEnum.None) && zone.AllowModifyWebPartCollection)
                    {
                        webPartZones.Add(zone);
                    }
                }
            }

            repZones.DataSource = webPartZones;
            repZones.DataBind();
        }

        if (PortalContext.MVTVariantsEnabled || PortalContext.ContentPersonalizationEnabled)
        {
            var loadingMenu = new ContextMenuItem { ResourceString = "ContextMenu.Loading" }.GetRenderedHTML();

            menuMoveToZoneVariants.LoadingContent = loadingMenu;
            menuMoveToZoneVariants.OnReloadData += menuMoveToZoneVariants_OnReloadData;
            repMoveToZoneVariants.ItemDataBound += repMoveToZoneVariants_ItemDataBound;

            // Display the MVT menu part in the Pages->Design only. Hide the context menu in the PageTemplates->Design
            if (PortalContext.MVTVariantsEnabled && (DocumentContext.CurrentPageInfo != null) && (DocumentContext.CurrentPageInfo.DocumentID > 0))
            {
                // Set Display='none' for the MVT panel. Show dynamically only if required.
                pnlContextMenuMVTVariants.Visible = true;
                pnlContextMenuMVTVariants.Style.Add("display", "none");
                menuWebPartMVTVariants.LoadingContent = loadingMenu;
                menuWebPartMVTVariants.OnReloadData += menuWebPartMVTVariants_OnReloadData;
                repWebPartMVTVariants.ItemDataBound += repWebPartVariants_ItemDataBound;

                string script = "webPartMVTVariantContextMenuId = '" + pnlContextMenuMVTVariants.ClientID + "';";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "webPartMVTVariantContextMenuId", ScriptHelper.GetScript(script));
            }

            // Display the Content personalization menu part in the Pages->Design only. Hide the context menu in the PageTemplates->Design
            if ((PortalContext.ContentPersonalizationEnabled) && (DocumentContext.CurrentPageInfo != null) && (DocumentContext.CurrentPageInfo.DocumentID > 0))
            {
                // Set Display='none' for the MVT panel. Show dynamically only if required.
                pnlContextMenuCPVariants.Visible = true;
                pnlContextMenuCPVariants.Style.Add("display", "none");
                menuWebPartCPVariants.LoadingContent = loadingMenu;
                menuWebPartCPVariants.OnReloadData += menuWebPartCPVariants_OnReloadData;
                repWebPartCPVariants.ItemDataBound += repWebPartVariants_ItemDataBound;

                string script = "webPartCPVariantContextMenuId = '" + pnlContextMenuCPVariants.ClientID + "';";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "webPartCPVariantContextMenuId", ScriptHelper.GetScript(script));
            }
        }
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repWebPartVariants control.
    /// </summary>
    protected void repWebPartVariants_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Panel pnlVariantItem = (Panel)e.Item.FindControl("pnlVariantItem");
        if (pnlVariantItem != null)
        {
            Label lblVariantName = pnlVariantItem.FindControl("lblVariantItem") as Label;
            if (lblVariantName != null)
            {
                string variantName = (((DataRowView)e.Item.DataItem)[columnVariantDisplayName]).ToString();
                lblVariantName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variantName, UICulture));
            }

            // Get unique web part code
            Guid instanceGuid = ValidationHelper.GetGuid(((DataRowView)e.Item.DataItem)[columnVariantInstanceGUID], Guid.Empty);
            int variantId = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)[columnVariantID], 0);
            string itemCode = "Variant_WP_" + instanceGuid.ToString("N");

            // Display the web part variant when clicked
            pnlVariantItem.Attributes.Add("onclick", "SetVariant('" + itemCode + "', " + variantId + "); " + updateCombinationPanelScript);
        }
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repZoneVariants control.
    /// </summary>
    protected void repMoveToZoneVariants_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Panel pnlZoneVariantItem = (Panel)e.Item.FindControl("pnlZoneVariantItem");
        if (pnlZoneVariantItem != null)
        {
            // Set the zone label
            Label lblZoneVariantItem = pnlZoneVariantItem.FindControl("lblZoneVariantItem") as Label;
            if (lblZoneVariantItem != null)
            {
                string variantName = (((DataRowView)e.Item.DataItem)[columnVariantDisplayName]).ToString();
                lblZoneVariantItem.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(variantName, UICulture));
            }

            // Get unique web part code
            string zoneId = ValidationHelper.GetString(((DataRowView)e.Item.DataItem)[columnVariantZoneID], string.Empty);
            int variantId = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)[columnVariantID], 0);
            string itemCode = "Variant_Zone_" + zoneId;

            // Select the web part variant on mouse over
            pnlZoneVariantItem.Attributes.Add("onmouseover", "SetVariant('" + itemCode + "', " + variantId + "); " + updateCombinationPanelScript);
            // Hide the context menus when clicked
            pnlZoneVariantItem.Attributes.Add("onclick", "ContextMoveWebPartToZone('" + zoneId + "', " + variantId + ");");
        }
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuWebPartVariants control.
    /// </summary>
    protected void menuWebPartMVTVariants_OnReloadData(object sender, EventArgs e)
    {
        // Check permissions
        if ((currentUser == null)
            || (!currentUser.IsAuthorizedPerResource("CMS.MVTest", "Read")))
        {
            return;
        }

        SetColumnNames(VariantModeEnum.MVT);

        string parameters = ValidationHelper.GetString(menuWebPartMVTVariants.Parameter, string.Empty);
        string[] items = parameters.Split(new char[] { ',' }, 4);
        if (items.Length != 4)
        {
            return;
        }

        string zoneId = ValidationHelper.GetString(items[0], string.Empty);
        Guid instanceGuid = ValidationHelper.GetGuid(items[3], Guid.Empty);

        if ((DocumentContext.CurrentPageInfo != null)
            && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
        {
            int templateId = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;

            // Get all MVT zone variants of the current web part
            DataSet ds = VariantHelper.GetVariants(VariantModeEnum.MVT, templateId, zoneId, instanceGuid, 0);
            DataTable resultTable = null;

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                DataTable table = ds.Tables[0].Copy();
                table.DefaultView.Sort = columnVariantID;

                // Add the original web part as the first item in the variant list
                DataRow originalVariant = table.NewRow();
                originalVariant[columnVariantID] = 0;
                originalVariant[columnVariantDisplayName] = ResHelper.GetString("WebPartMenu.OriginalWebPart", UICulture);
                originalVariant[columnVariantZoneID] = zoneId;
                originalVariant[columnVariantPageTemplateID] = templateId;
                originalVariant[columnVariantInstanceGUID] = instanceGuid;
                table.Rows.InsertAt(originalVariant, 0);

                resultTable = table.DefaultView.ToTable();

                if (DataHelper.DataSourceIsEmpty(resultTable))
                {
                    pnlNoWebPartMVTVariants.Visible = true;
                    lblNoWebPartMVTVariants.Text = ResHelper.GetString("Content.NoPermissions", UICulture);
                }
            }
            else
            {
                pnlNoWebPartMVTVariants.Visible = true;
            }

            repWebPartMVTVariants.DataSource = resultTable;
            repWebPartMVTVariants.DataBind();
        }
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuWebPartVariants control.
    /// </summary>
    protected void menuWebPartCPVariants_OnReloadData(object sender, EventArgs e)
    {
        // Check permissions
        if ((currentUser == null)
            || (!currentUser.IsAuthorizedPerResource("CMS.ContentPersonalization", "Read")))
        {
            return;
        }

        SetColumnNames(VariantModeEnum.ContentPersonalization);

        string parameters = ValidationHelper.GetString(menuWebPartCPVariants.Parameter, string.Empty);
        string[] items = parameters.Split(new char[] { ',' }, 4);
        if (items.Length != 4)
        {
            return;
        }

        string zoneId = ValidationHelper.GetString(items[0], string.Empty);
        Guid instanceGuid = ValidationHelper.GetGuid(items[3], Guid.Empty);

        if ((DocumentContext.CurrentPageInfo != null)
            && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
        {
            int templateId = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;

            DataSet ds = VariantHelper.GetVariants(VariantModeEnum.ContentPersonalization, templateId, zoneId, instanceGuid, 0);
            DataTable resultTable = null;

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                DataTable table = ds.Tables[0].Copy();
                table.DefaultView.Sort = columnVariantID;

                // Add the original web part as the first item in the variant list
                DataRow originalVariant = table.NewRow();
                originalVariant[columnVariantID] = 0;
                originalVariant[columnVariantDisplayName] = ResHelper.GetString("WebPartMenu.OriginalWebPart", UICulture);
                originalVariant[columnVariantZoneID] = zoneId;
                originalVariant[columnVariantPageTemplateID] = templateId;
                originalVariant[columnVariantInstanceGUID] = instanceGuid;
                table.Rows.InsertAt(originalVariant, 0);

                resultTable = table.DefaultView.ToTable();

                if (DataHelper.DataSourceIsEmpty(resultTable))
                {
                    pnlNoWebPartCPVariants.Visible = true;
                    lblNoWebPartCPVariants.Text = ResHelper.GetString("Content.NoPermissions", UICulture);
                }
            }
            else
            {
                pnlNoWebPartCPVariants.Visible = true;
            }

            repWebPartCPVariants.DataSource = resultTable;
            repWebPartCPVariants.DataBind();
        }
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuZoneVariants control.
    /// </summary>
    protected void menuMoveToZoneVariants_OnReloadData(object sender, EventArgs e)
    {
        if ((DocumentContext.CurrentPageInfo != null)
            && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
        {
            string targetZoneId = ValidationHelper.GetString(menuMoveToZoneVariants.Parameter, string.Empty);
            int pageTemplateId = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;
            VariantModeEnum currentVariantMode = VariantModeEnum.None;

            // Get selected zone variant mode
            if ((DocumentContext.CurrentPageInfo != null)
                && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
            {
                WebPartZoneInstance targetZone = DocumentContext.CurrentPageInfo.TemplateInstance.GetZone(targetZoneId);
                if (targetZone != null)
                {
                    currentVariantMode = targetZone.VariantMode;
                }
            }

            SetColumnNames(currentVariantMode);

            // Get all zone variants of the current web part
            DataSet ds = ds = VariantHelper.GetVariants(currentVariantMode, pageTemplateId, targetZoneId, Guid.Empty, 0);
            DataTable resultTable = null;

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                DataTable table = ds.Tables[0].Copy();
                table.DefaultView.Sort = columnVariantID;

                // Add the original web part as the first item in the variant list
                DataRow originalVariant = table.NewRow();
                originalVariant[columnVariantID] = 0;
                originalVariant[columnVariantDisplayName] = ResHelper.GetString("ZoneMenu.OriginalZone", UICulture);
                originalVariant[columnVariantZoneID] = targetZoneId;
                originalVariant[columnVariantPageTemplateID] = pageTemplateId;
                originalVariant[columnVariantInstanceGUID] = Guid.Empty;
                table.Rows.InsertAt(originalVariant, 0);

                resultTable = table.DefaultView.ToTable();

                if (DataHelper.DataSourceIsEmpty(resultTable))
                {
                    pnlNoZoneVariants.Visible = true;
                    ltlNoZoneVariants.Text = ResHelper.GetString("Content.NoPermissions", UICulture);
                }
            }

            repMoveToZoneVariants.DataSource = resultTable;
            repMoveToZoneVariants.DataBind();
        }
    }


    /// <summary>
    /// Sets the column names according to the variant mode.
    /// </summary>
    private void SetColumnNames(VariantModeEnum variantMode)
    {
        if (variantMode == VariantModeEnum.MVT)
        {
            // MVT column names
            columnVariantID = "MVTVariantID";
            columnVariantDisplayName = "MVTVariantDisplayName";
            columnVariantZoneID = "MVTVariantZoneID";
            columnVariantPageTemplateID = "MVTVariantPageTemplateID";
            columnVariantInstanceGUID = "MVTVariantInstanceGUID";
            updateCombinationPanelScript = "UpdateCombinationPanel();";
        }
        else if (variantMode == VariantModeEnum.ContentPersonalization)
        {
            // Content personalization column names
            columnVariantID = "VariantID";
            columnVariantDisplayName = "VariantDisplayName";
            columnVariantZoneID = "VariantZoneID";
            columnVariantPageTemplateID = "VariantPageTemplateID";
            columnVariantInstanceGUID = "VariantInstanceGUID";
            updateCombinationPanelScript = string.Empty;
        }
    }

    #endregion
}
