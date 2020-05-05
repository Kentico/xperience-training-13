using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSModules_OnlineMarketing_Controls_Content_MenuContentPersonalizationVariants : CMSAbstractPortalUserControl
{
    #region "Variables"

    private string columnVariantID = "VariantID";
    private string columnVariantDisplayName = "VariantDisplayName";
    private string columnVariantZoneID = "VariantZoneID";
    private string columnVariantPageTemplateID = "VariantPageTemplateID";
    private string columnVariantInstanceGUID = "VariantInstanceGUID";
    private bool isZone = false;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlWebPartMenu.Attributes.Add("onmouseover", "ActivateParentBorder();");
        pnlWebPartMenu.Attributes.Add("onmouseout", "DeactivateParentBorder();");

        menuWebPartCPVariants.LoadingContent = new ContextMenuItem { ResourceString = "ContextMenu.Loading" }.GetRenderedHTML();
        menuWebPartCPVariants.OnReloadData += menuWebPartCPVariants_OnReloadData;
        repWebPartCPVariants.ItemDataBound += repWebPartCPVariants_ItemDataBound;
    }


    /// <summary>
    /// Handles the OnReloadData event of the menuWebPartCPVariants control.
    /// </summary>
    protected void menuWebPartCPVariants_OnReloadData(object sender, EventArgs e)
    {
        // Check permissions
        if ((MembershipContext.AuthenticatedUser == null)
            || (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.ContentPersonalization", "Read")))
        {
            return;
        }

        string parameters = ValidationHelper.GetString(menuWebPartCPVariants.Parameter, string.Empty);
        string[] items = parameters.Split(new char[] { ',' }, 7);

        if ((items == null) || (items.Length < 4))
        {
            return;
        }

        string zoneId = ValidationHelper.GetString(items[0], string.Empty);
        string webpartName = ValidationHelper.GetString(items[1], string.Empty);
        string aliasPath = ValidationHelper.GetString(items[2], string.Empty);
        Guid instanceGuid = ValidationHelper.GetGuid(items[3], Guid.Empty);

        isZone = (instanceGuid == Guid.Empty);

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
                originalVariant[columnVariantDisplayName] = ResHelper.GetString(isZone ? "ZoneMenu.OriginalZone" : "WebPartMenu.OriginalWebPart");
                originalVariant[columnVariantZoneID] = zoneId;
                originalVariant[columnVariantPageTemplateID] = templateId;
                originalVariant[columnVariantInstanceGUID] = instanceGuid;
                table.Rows.InsertAt(originalVariant, 0);

                resultTable = table.DefaultView.ToTable();
            }

            repWebPartCPVariants.DataSource = resultTable;
            repWebPartCPVariants.DataBind();
        }
    }


    /// <summary>
    /// Handles the ItemDataBound event of the repWebPartVariants control.
    /// </summary>
    protected void repWebPartCPVariants_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Panel pnlVariantItem = (Panel)e.Item.FindControl("pnlVariantItem");
        if (pnlVariantItem != null)
        {
            Label lblVariantName = pnlVariantItem.FindControl("lblVariantItem") as Label;
            if (lblVariantName != null)
            {
                lblVariantName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString((((DataRowView)e.Item.DataItem)[columnVariantDisplayName]).ToString()));
            }

            // Get unique web part code
            Guid instanceGuid = ValidationHelper.GetGuid(((DataRowView)e.Item.DataItem)[columnVariantInstanceGUID], Guid.Empty);
            int variantId = ValidationHelper.GetInteger(((DataRowView)e.Item.DataItem)[columnVariantID], 0);
            string itemCode = String.Empty;
            if (isZone)
            {
                string zoneId = ValidationHelper.GetString(((DataRowView)e.Item.DataItem)[columnVariantZoneID], String.Empty);
                itemCode = "Variant_Zone_" + zoneId;
            }
            else
            {
                itemCode = "Variant_WP_" + instanceGuid.ToString("N");
            }

            // Display the web part variant when clicked
            pnlVariantItem.Attributes.Add("onclick", "variantSliderChanged=true; UpdateVariantPosition('" + itemCode + "', " + variantId + "); RefreshPage();");
        }
    }

    #endregion
}