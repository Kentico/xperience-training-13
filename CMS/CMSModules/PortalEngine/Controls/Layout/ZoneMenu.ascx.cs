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


public partial class CMSModules_PortalEngine_Controls_Layout_ZoneMenu : CMSAbstractPortalUserControl
{
    #region "Variables"

    // Column names
    private string columnVariantID = String.Empty;
    private string columnVariantDisplayName = String.Empty;
    private string columnVariantZoneID = String.Empty;
    private string columnVariantPageTemplateID = String.Empty;
    private string columnVariantInstanceGUID = String.Empty;
    private string updateCombinationPanelScript = String.Empty;
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
        if (!currentUser.IsAuthorizedPerResource("cms.contentpersonalization", "Manage"))
        {
            plcAddCPVariant.Visible = false;
        }

        if (!currentUser.IsAuthorizedPerResource("cms.mvtest", "Manage"))
        {
            plcAddMVTVariant.Visible = false;
        }

        // Main menu
        lblNewWebPart.Text = ResHelper.GetString("ZoneMenu.IconNewWebPart", UICulture);
        pnlNewWebPart.Attributes.Add("onclick", "ContextNewWebPart();");

        // Configure
        lblConfigureZone.Text = ResHelper.GetString("ZoneMenu.IconConfigureWebpartZone", UICulture);
        pnlConfigureZone.Attributes.Add("onclick", "ContextConfigureWebPartZone();");

        // Move to
        lblMoveTo.Text = ResHelper.GetString("ZoneMenu.IconMoveTo", UICulture);

        // Copy all web parts
        lblCopy.Text = ResHelper.GetString("ZoneMenu.CopyAll", UICulture);
        pnlCopyAllItem.Attributes.Add("onclick", "ContextCopyAllWebParts();");

        // Paste web part(s)
        lblPaste.Text = ResHelper.GetString("ZoneMenu.paste", UICulture);
        pnlPaste.Attributes.Add("onclick", "ContextPasteWebPartZone();");
        pnlPaste.ToolTip = ResHelper.GetString("ZoneMenu.pasteTooltip", UICulture);

        // Delete all web parts
        lblDelete.Text = ResHelper.GetString("ZoneMenu.RemoveAll", UICulture);
        pnlDelete.Attributes.Add("onclick", "ContextRemoveAllWebParts();");

        // Add new MVT variants
        lblAddMVTVariant.Text = ResHelper.GetString("ZoneMenu.AddZoneVariant", UICulture);

        // Add new Content personalization variant
        lblAddCPVariant.Text = ResHelper.GetString("ZoneMenu.AddZoneVariant", UICulture);

        // Add new variant
        pnlAddMVTVariant.Attributes.Add("onclick", "ContextAddWebPartZoneMVTVariant();");
        pnlAddCPVariant.Attributes.Add("onclick", "ContextAddWebPartZoneCPVariant();");

        // List all variants
        lblMVTVariants.Text = ResHelper.GetString("ZoneMenu.ZoneMVTVariants", UICulture);
        lblCPVariants.Text = ResHelper.GetString("ZoneMenu.ZonePersonalizationVariants", UICulture);

        // No MVT variants
        lblNoZoneMVTVariants.Text = ResHelper.GetString("ZoneMenu.NoVariants", UICulture);

        // No CP variants
        lblNoZoneCPVariants.Text = ResHelper.GetString("ZoneMenu.NoVariants", UICulture);

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
            repMoveToZoneVariants.ItemDataBound += repZoneVariants_ItemDataBound;

            // Display the MVT menu part in the Pages->Design only. Hide the context menu in the PageTemplates->Design
            if (PortalContext.MVTVariantsEnabled && (DocumentContext.CurrentPageInfo != null) && (DocumentContext.CurrentPageInfo.DocumentID > 0) && currentUser.IsAuthorizedPerResource("cms.mvtest", "read"))
            {
                // Set Display='none' for the MVT panel. Show dynamically only if required.
                pnlContextMenuMVTVariants.Visible = true;
                pnlContextMenuMVTVariants.Style.Add("display", "none");
                menuZoneMVTVariants.LoadingContent = loadingMenu;
                menuZoneMVTVariants.OnReloadData += menuZoneMVTVariants_OnReloadData;
                repZoneMVTVariants.ItemDataBound += repVariants_ItemDataBound;

                string script = "zoneMVTVariantContextMenuId = '" + pnlContextMenuMVTVariants.ClientID + "';";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "zoneMVTVariantContextMenuId", ScriptHelper.GetScript(script));
            }

            // Display the Content personalization menu part in the Pages->Design only. Hide the context menu in the PageTemplates->Design
            if ((PortalContext.ContentPersonalizationEnabled) && (DocumentContext.CurrentPageInfo != null) && (DocumentContext.CurrentPageInfo.DocumentID > 0) && currentUser.IsAuthorizedPerResource("cms.contentpersonalization", "read"))
            {
                // Set Display='none' for the MVT panel. Show dynamically only if required.
                pnlContextMenuCPVariants.Visible = true;
                pnlContextMenuCPVariants.Style.Add("display", "none");
                menuZoneCPVariants.LoadingContent = loadingMenu;
                menuZoneCPVariants.OnReloadData += menuZoneCPVariants_OnReloadData;
                repZoneCPVariants.ItemDataBound += repVariants_ItemDataBound;

                string script = "zoneCPVariantContextMenuId = '" + pnlContextMenuCPVariants.ClientID + "';";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "zoneCPVariantContextMenuId", ScriptHelper.GetScript(script));
            }
        }
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repVariants control.
    /// </summary>
    protected void repVariants_ItemDataBound(object sender, RepeaterItemEventArgs e)
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

            // Get unique zone code
            string zoneId = ValidationHelper.GetString(((DataRowView)e.Item.DataItem)[columnVariantZoneID], string.Empty);
            int variantId = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)[columnVariantID], 0);
            string itemCode = "Variant_Zone_" + zoneId;

            // Display the zone variant when clicked
            pnlVariantItem.Attributes.Add("onclick", "SetVariant('" + itemCode + "', " + variantId + "); " + updateCombinationPanelScript);
        }
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repZoneVariants control.
    /// </summary>
    protected void repZoneVariants_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
            pnlZoneVariantItem.Attributes.Add("onclick", "ContextMoveWebPartsToZone('" + zoneId + "', " + variantId + ");");
        }
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuWebPartVariants control.
    /// </summary>
    protected void menuZoneMVTVariants_OnReloadData(object sender, EventArgs e)
    {
        // Check permissions
        if ((currentUser == null)
            || (!currentUser.IsAuthorizedPerResource("CMS.MVTest", "Read")))
        {
            return;
        }

        SetColumnNames(VariantModeEnum.MVT);

        string zoneId = ValidationHelper.GetString(menuZoneMVTVariants.Parameter, string.Empty);

        if ((DocumentContext.CurrentPageInfo != null)
            && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
        {
            int templateId = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;

            // Get all MVT zone variants of the current web part
            DataSet ds = VariantHelper.GetVariants(VariantModeEnum.MVT, templateId, zoneId, Guid.Empty, 0);
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
                originalVariant[columnVariantInstanceGUID] = Guid.Empty;
                table.Rows.InsertAt(originalVariant, 0);

                resultTable = table.DefaultView.ToTable();

                if (DataHelper.DataSourceIsEmpty(resultTable))
                {
                    pnlNoZoneMVTVariants.Visible = true;
                    lblNoZoneMVTVariants.Text = ResHelper.GetString("Content.NoPermissions", UICulture);
                }
            }
            else
            {
                pnlNoZoneMVTVariants.Visible = true;
            }

            repZoneMVTVariants.DataSource = resultTable;
            repZoneMVTVariants.DataBind();
        }
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuWebPartVariants control.
    /// </summary>
    protected void menuZoneCPVariants_OnReloadData(object sender, EventArgs e)
    {
        // Check permissions
        if ((currentUser == null)
            || (!currentUser.IsAuthorizedPerResource("CMS.ContentPersonalization", "Read")))
        {
            return;
        }

        SetColumnNames(VariantModeEnum.ContentPersonalization);

        string zoneId = ValidationHelper.GetString(menuZoneCPVariants.Parameter, string.Empty);

        if ((DocumentContext.CurrentPageInfo != null)
            && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
        {
            int templateId = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;

            DataSet ds = VariantHelper.GetVariants(VariantModeEnum.ContentPersonalization, templateId, zoneId, Guid.Empty, 0);
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
                originalVariant[columnVariantInstanceGUID] = Guid.Empty;
                table.Rows.InsertAt(originalVariant, 0);

                resultTable = table.DefaultView.ToTable();

                if (DataHelper.DataSourceIsEmpty(resultTable))
                {
                    pnlNoZoneCPVariants.Visible = true;
                    lblNoZoneCPVariants.Text = ResHelper.GetString("Content.NoPermissions", UICulture);
                }
            }
            else
            {
                pnlNoZoneCPVariants.Visible = true;
            }

            repZoneCPVariants.DataSource = resultTable;
            repZoneCPVariants.DataBind();
        }
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuZoneVariants control.
    /// </summary>
    protected void menuMoveToZoneVariants_OnReloadData(object sender, EventArgs e)
    {
        // Check permissions
        if (currentUser == null)
        {
            return;
        }

        if ((DocumentContext.CurrentPageInfo != null)
            && (DocumentContext.CurrentPageInfo.TemplateInstance != null))
        {
            VariantModeEnum currentVariantMode = VariantModeEnum.None;
            string targetZoneId = ValidationHelper.GetString(menuMoveToZoneVariants.Parameter, string.Empty);
            int pageTemplateId = DocumentContext.CurrentPageInfo.UsedPageTemplateInfo.PageTemplateId;

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
            DataTable resultTable = null;
            DataSet ds = VariantHelper.GetVariants(currentVariantMode, pageTemplateId, targetZoneId, Guid.Empty, 0);

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
            updateCombinationPanelScript = String.Empty;
        }
    }

    #endregion
}
