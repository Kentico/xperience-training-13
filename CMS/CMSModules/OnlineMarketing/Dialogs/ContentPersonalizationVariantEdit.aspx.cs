using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.OnlineMarketing.Web.UI;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(ContentPersonalizationVariantInfo.OBJECT_TYPE, "variantid")]
[ParentObject(PageTemplateInfo.OBJECT_TYPE, "templateid")]
public partial class CMSModules_OnlineMarketing_Dialogs_ContentPersonalizationVariantEdit : CMSVariantDialogPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "personalization_variants_conditions";

    #endregion


    #region "Variables"

    /// <summary>
    /// Indicates whether editing a web part or a zone variant.
    /// </summary>
    private VariantTypeEnum variantType = VariantTypeEnum.Zone;

    #endregion


    #region "Page methods"

    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.contentpersonalization", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "Read", "Content personalization"));
        }

        // Set the ParentObject manually tor inherited templates
        if (editElem.UIFormControl.ParentObject == null)
        {
            var aliasPath = QueryHelper.GetString("aliaspath", string.Empty);
            var siteName = SiteContext.CurrentSiteName;
        }

        // Get information whether the control is used for a web part or zone variant
        variantType = VariantTypeFunctions.GetVariantTypeEnum(QueryHelper.GetString("varianttype", string.Empty));

        base.OnInit(e);
        
        // Get the alias path of the current node
        if (Node == null)
        {
            editElem.StopProcessing = true;
        }

        editElem.UIFormControl.SubmitButton.Visible = false;
        editElem.UIFormControl.OnBeforeSave += UIFormControl_OnBeforeSaved;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        RegisterEscScript();

        // Setup the title, image, help
        PageTitle title = PageTitle;
        title.TitleText = GetString("contentpersonalizationvariant.edit");
        title.HelpTopicName = HELP_TOPIC_LINK;

        // Set the dark header (+ dark help icon)
        CurrentMaster.PanelBody.CssClass += " DialogsPageHeader";
        title.IsDialog = true;

        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterWOpenerScript(this);
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();
    }


    /// <summary>
    /// Handles the OnSaved event of the editElem control.
    /// </summary>
    protected void UIFormControl_OnBeforeSaved(object sender, EventArgs e)
    {
        if (editElem.ValidateData())
        {
            var editedObject = (ContentPersonalizationVariantInfo)editElem.UIFormControl.EditedObject;

            // Add the properties to the window helper
            Hashtable parameters = new Hashtable();
            parameters.Add("displayname", editedObject.VariantDisplayName);
            parameters.Add("codename", editedObject.VariantName);
            parameters.Add("description", editedObject.VariantDescription);
            parameters.Add("enabled", editedObject.VariantEnabled);
            parameters.Add("condition", editedObject.VariantDisplayCondition);
            WindowHelper.Add("variantProperties", parameters);

            // Set a script to open the web part properties modal dialog
            string query = URLHelper.GetQuery(RequestContext.CurrentURL);
            query = URLHelper.RemoveUrlParameter(query, "nodeid");
            query = URLHelper.AddUrlParameter(query, "variantmode", VariantModeFunctions.GetVariantModeString(VariantModeEnum.ContentPersonalization));

            // Choose the correct javascript method for opening web part/zone properties
            string functionName = string.Empty;

            switch (variantType)
            {
                case VariantTypeEnum.WebPart:
                    functionName = "OnAddWebPartVariant";
                    break;

                case VariantTypeEnum.Widget:
                    functionName = "OnAddWidgetVariant";
                    query = URLHelper.RemoveUrlParameter(query, "varianttype");
                    string widgetId = QueryHelper.GetString("webpartid", string.Empty);
                    query = URLHelper.RemoveUrlParameter(query, "webpartid");
                    query = URLHelper.AddParameterToUrl(query, "widgetid", widgetId);
                    break;

                case VariantTypeEnum.Zone:
                    functionName = "OnAddWebPartZoneVariant";
                    break;
            }

            // Setup the script for opening web part/zone properties
            string script = @"
            function OpenVariantProperties()
            {
                if (wopener." + functionName + @")
                {
                    wopener." + functionName + "('" + query + @"');
                }
                CloseDialog();
            }

            window.onload = OpenVariantProperties;";

            ltrScript.Text = ScriptHelper.GetScript(script);
        }

        // Do not save the variant. Will be saved when saving the web part/zone properties.
        editElem.UIFormControl.StopProcessing = true;
    }

    #endregion


    #region "Control events"

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        editElem.UIFormControl.SaveData(null);
    }

    #endregion
}
