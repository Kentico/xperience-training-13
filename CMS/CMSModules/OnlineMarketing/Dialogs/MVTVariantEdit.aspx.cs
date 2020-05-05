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


[EditedObject(ContentPersonalizationVariantInfo.OBJECT_TYPE, "variantid")]
[ParentObject(PageTemplateInfo.OBJECT_TYPE, "templateid")]
public partial class CMSModules_OnlineMarketing_Dialogs_MVTVariantEdit : CMSVariantDialogPage
{
    #region "Variables"

    /// <summary>
    /// Indicates whether editing a web part or a zone variant.
    /// </summary>
    private VariantTypeEnum variantType = VariantTypeEnum.Zone;


    private int mTemplateID;

    #endregion


    #region "Page methods"

    /// <summary>
    /// Raises the <see cref="E:Init"/> event.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.mvtest", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "Read", "MVT testing"));
        }

        // Register the Save and close button as the form submit button
        HeaderActions.Visible = false;
        editElem.UIFormControl.SubmitButton.Visible = false;
        btnOk.Click += (s, ea) => editElem.UIFormControl.SaveData(null);

        // Turn off update document for this page
        EnsureDocumentManager = false;

        // Set the ParentObject manually tor inherited templates
        if (editElem.UIFormControl.ParentObject == null)
        {
            var aliasPath = QueryHelper.GetString("aliaspath", string.Empty);
            var siteName = SiteContext.CurrentSiteName;
        }

        // Get information whether the control is used for a web part or zone variant
        variantType = VariantTypeFunctions.GetVariantTypeEnum(QueryHelper.GetString("varianttype", string.Empty));

        mTemplateID = QueryHelper.GetInteger("templateid", 0);

        base.OnInit(e);

        // Get the alias path of the current node
        if (Node == null)
        {
            editElem.StopProcessing = true;
        }

        editElem.UIFormControl.OnBeforeSave += UIFormControl_OnBeforeSaved;
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

        title.TitleText = GetString("mvtvariant.edit");
        // Set the dark header (+ dark help icon)
        CurrentMaster.PanelBody.CssClass += " DialogsPageHeader";
        title.IsDialog = true;
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
        var codeName = ValidationHelper.GetString(editElem.UIFormControl.FieldControls["MVTVariantName"].Value, string.Empty);
        if (!CheckUniqueCodeName(codeName))
        {
            // Do not save the variant.
            editElem.UIFormControl.StopProcessing = true;
            ShowError(string.Format(GetString("general.codenamenotunique"), GetString("objecttype.om_mvtvariant"), codeName));
            return;
        }

        // Add the properties to the window helper
        Hashtable parameters = new Hashtable();
        parameters.Add("displayname", ValidationHelper.GetString(editElem.UIFormControl.FieldControls["MVTVariantDisplayName"].Value, string.Empty));
        parameters.Add("description", ValidationHelper.GetString(editElem.UIFormControl.FieldControls["MVTVariantDescription"].Value, string.Empty));
        parameters.Add("enabled", ValidationHelper.GetBoolean(editElem.UIFormControl.FieldControls["MVTVariantEnabled"].Value, false));
        parameters.Add("codename", codeName);
        WindowHelper.Add("variantProperties", parameters);

        // Set a script to open the web part properties modal dialog
        string query = URLHelper.GetQuery(RequestContext.CurrentURL);
        query = URLHelper.RemoveUrlParameter(query, "nodeid");
        query = URLHelper.AddUrlParameter(query, "variantmode", VariantModeFunctions.GetVariantModeString(VariantModeEnum.MVT));

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
                CloseDialog();
                if (wopener." + functionName + @")
                {
                    wopener." + functionName + "('" + query + @"');
                }
            }

            window.onload = OpenVariantProperties;";

        ltrScript.Text = ScriptHelper.GetScript(script);

        // Do not save the variant. Will be saved when saving the web part/zone properties.
        editElem.UIFormControl.StopProcessing = true;
    }


    /// <summary>
    /// Checks code name in template uniqueness.
    /// </summary>
    /// <param name="codeName">New code name</param>
    /// <returns>True if code name is unique</returns>
    private bool CheckUniqueCodeName(string codeName)
    {
        var dummyMVT = new MVTVariantInfo
        {
            MVTVariantName = codeName,
            MVTVariantPageTemplateID = mTemplateID,
            MVTVariantDocumentID = (variantType == VariantTypeEnum.Widget) ? Node.DocumentID : 0
        };
        return dummyMVT.CheckUniqueValues();
    }

    #endregion
}
