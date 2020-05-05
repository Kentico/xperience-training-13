using System;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.OnlineMarketing.Web.UI;
using CMS.UIControls;

using Action = CMS.UIControls.UniGridConfig.Action;


public partial class CMSModules_OnlineMarketing_Dialogs_ContentPersonalizationVariantList : CMSVariantDialogPage
{
    #region "Variables"

    /// <summary>
    /// Indicates whether editing a web part or a zone variant.
    /// </summary>
    private VariantTypeEnum variantType = VariantTypeEnum.Zone;

    // Setup the list control properties
    private int pageTemplateId;
    private string zoneId = string.Empty;
    private Guid instanceGuid = Guid.Empty;
    private string aliasPath = string.Empty;
    private string webPartId = string.Empty;

    #endregion


    #region "Page methods"

    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.contentpersonalization", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "Read", "Content personalization"));
        }

        variantType = listElem.VariantType = VariantTypeFunctions.GetVariantTypeEnum(QueryHelper.GetString("varianttype", string.Empty));

        // Get the alias path of the current node
        if (Node == null)
        {
            listElem.StopProcessing = true;
        }

        // Setup the list control properties
        pageTemplateId = listElem.PageTemplateID = QueryHelper.GetInteger("templateid", 0);
        zoneId = listElem.ZoneID = QueryHelper.GetText("zoneid", string.Empty);
        instanceGuid = listElem.InstanceGUID = QueryHelper.GetGuid("instanceguid", Guid.Empty);
        aliasPath = QueryHelper.GetString("aliaspath", string.Empty);
        webPartId = QueryHelper.GetString("webpartid", string.Empty);
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        RegisterEscScript();

        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterWOpenerScript(this);

        // Setup the title, image, help
        PageTitle title = PageTitle;

        title.TitleText = GetString("contentpersonalizationvariant.list");
        // Set the dark header (+ dark help icon)
        CurrentMaster.PanelBody.CssClass += " DialogsPageHeader";
        title.IsDialog = true;

        // Get Edit action button
        Action editAction = listElem.Grid.GridActions.Actions[0] as Action;
        editAction.OnClick = "OpenVariantProperties({0}); return false;";

        // Set NodeID in order to check the access to the document
        listElem.NodeID = NodeID;
        listElem.AllowSorting = false;
        listElem.OnAction += listElem_OnAction;
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();

        // Choose the correct javascript method for opening web part/zone properties
        string functionName = string.Empty;
        // Set a script to open the web part properties modal dialog
        string parameters = string.Empty;
        // Set the script to show the corresponding variant
        string setVariant = string.Empty;

        ScriptHelper.RegisterScriptFile(Page, "DesignMode/PortalManager.js");

        switch (variantType)
        {
            case VariantTypeEnum.WebPart:
                functionName = "ConfigureWebPart";
                parameters = "new webPartProperties('" + ScriptHelper.GetString(zoneId, false) + "','" + ScriptHelper.GetString(webPartId, false) + "','" + ScriptHelper.GetString(aliasPath, false) + "','" + instanceGuid + "'," + pageTemplateId + ")";
                setVariant = "SetVariant('Variant_WP_" + instanceGuid.ToString("N") + "', variantId);";
                break;

            case VariantTypeEnum.Widget:
                functionName = "ConfigureWidget";
                parameters = "new webPartProperties('" + ScriptHelper.GetString(zoneId, false) + "','" + ScriptHelper.GetString(webPartId, false) + "','" + ScriptHelper.GetString(aliasPath, false) + "','" + instanceGuid + "'," + pageTemplateId + ")";
                setVariant = "SetVariant('Variant_WP_" + instanceGuid.ToString("N") + "', variantId);";
                break;

            case VariantTypeEnum.Zone:
                functionName = "ConfigureWebPartZone";
                parameters = "new zoneProperties('" + ScriptHelper.GetString(zoneId, false) + "','" + ScriptHelper.GetString(aliasPath, false) + "'," + pageTemplateId + ",''," + QueryHelper.GetBoolean("islayoutzone", false).ToString().ToLowerCSafe() + ")";
                setVariant = "SetVariant('Variant_Zone_" + ScriptHelper.GetString(zoneId, false) + "', variantId);";
                break;
        }

        // Setup the script for opening web part/zone properties
        string script = @"
            function OpenVariantProperties(variantId)
            {
                CloseDialog();

                if ((wopener." + functionName + @") && (wopener.SetVariant))
                {
                    wopener." + setVariant + @"
                    wopener." + functionName + "(" + parameters + @");
                }
            } ";

        ltrScript.Text = ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Handles the OnAction event of the listElem control. This events is raised when deleting a variant in the grid or changing its position (moving up/down)
    /// </summary>
    protected void listElem_OnAction(object sender, EventArgs e)
    {
        // Set the flag that a variant has been deleted or moved up/down
        ScriptManager.RegisterStartupScript(this, typeof(string), "refreshWOpener", "window.refreshPageOnClose = true;", true);
    }

    #endregion
}
