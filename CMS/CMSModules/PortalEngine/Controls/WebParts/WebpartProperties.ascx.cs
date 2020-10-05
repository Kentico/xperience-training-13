using System;
using System.Collections;
using System.Data;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_Controls_WebParts_WebpartProperties : CMSUserControl
{
    #region "Variables"

    private const string CAT_OPEN_PREFIX = "cat_open_";

    protected int xmlVersion;
    protected VariantModeEnum mVariantMode = VariantModeEnum.None;


    /// <summary>
    /// Current page info.
    /// </summary>
    private PageInfo pi;

    /// <summary>
    /// Page template info.
    /// </summary>
    private PageTemplateInfo pti;

    /// <summary>
    /// Currently edited web part.
    /// </summary>
    private WebPartInstance webPartInstance;

    /// <summary>
    /// Current page template.
    /// </summary>
    private PageTemplateInstance templateInstance;

    /// <summary>
    /// Tree provider.
    /// </summary>
    private readonly TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

    /// <summary>
    /// Gets web part from instance.
    /// </summary>
    private WebPartInfo wpi;

    /// <summary>
    /// Indicates whether the new variant should be chosen when closing this dialog
    /// </summary>
    private bool selectNewVariant;

    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    private string mCultureCode;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder => plcMess;


    /// <summary>
    /// Page alias path.
    /// </summary>
    public string AliasPath { get; set; }


    /// <summary>
    /// Preferred culture code to use along with alias path.
    /// </summary>
    public string CultureCode
    {
        get
        {
            if (string.IsNullOrEmpty(mCultureCode))
            {
                mCultureCode = LocalizationContext.PreferredCultureCode;
            }
            return mCultureCode;
        }
        set
        {
            mCultureCode = value;
        }
    }


    /// <summary>
    /// Page template ID.
    /// </summary>
    public int PageTemplateID { get; set; }


    /// <summary>
    /// Zone ID.
    /// </summary>
    public string ZoneID { get; set; }


    /// <summary>
    /// Web part ID.
    /// </summary>
    public string WebPartID { get; set; }
    

    /// <summary>
    /// Instance GUID.
    /// </summary>
    public Guid InstanceGUID { get; set; }


    /// <summary>
    /// Indicates whether the web part is new (inserting) or not (updating).
    /// </summary>
    public bool IsNewWebPart { get; set; }


    /// <summary>
    /// Gets or sets the position of the inserted web part.
    /// </summary>
    public int Position { get; set; }



    /// <summary>
    /// Relative position of the web part from the left
    /// </summary>
    public int PositionLeft { get; set; }


    /// <summary>
    /// Relative position of the web part from the top
    /// </summary>
    public int PositionTop { get; set; }


    /// <summary>
    /// Indicates whether is a new variant.
    /// </summary>
    public bool IsNewVariant { get; set; }


    /// <summary>
    /// Gets or sets the actual web part variant ID.
    /// </summary>
    public int VariantID { get; set; }


    /// <summary>
    /// Gets or sets the web part zone variant ID.
    /// This property is set when adding a new webpart into the zone variant, in all other cases is set to 0.
    /// </summary>
    public int ZoneVariantID { get; set; }


    /// <summary>
    /// Gets the variant mode. Indicates whether there are MVT/ContentPersonalization/None variants active.
    /// </summary>
    public VariantModeEnum VariantMode
    {
        get
        {
            return mVariantMode;
        }
        set
        {
            mVariantMode = value;
        }
    }

    #endregion


    #region "Methods"

    public void LoadData()
    {
        if (StopProcessing)
        {
            return;
        }

        LoadForm();

        // Hide ID editing in case the instance is default configuration of the web part
        if ((webPartInstance != null) && (webPartInstance.InstanceGUID == WebPartInfo.DEFAULT_CONFIG_INSTANCEGUID))
        {
            form.FieldsToHide.Add("WebPartControlID");
        }

        // Setup info/error message placeholder
        if (MessagesPlaceHolder != null)
        {
            MessagesPlaceHolder.UseRelativePlaceHolder = false;
            form.EnsureMessagesPlaceholder(MessagesPlaceHolder);
        }

        ScriptHelper.RegisterEditScript(Page, false);
        ScriptHelper.RegisterJQuery(Page);
    }


    protected void lnkLoadDefaults_Click(object sender, EventArgs e)
    {
        // Get the web part form info
        FormInfo fi = GetWebPartFormInfo();
        if (fi != null)
        {
            // Create DataRow with default data
            var dr = fi.GetDataRow();

            // Load default values
            fi.LoadDefaultValues(dr);

            // Set web part ID
            dr["WebPartControlID"] = GetUniqueWebPartId();

            // Load to the form
            form.LoadData(dr);
        }
    }


    /// <summary>
    /// Loads the web part form.
    /// </summary>
    protected void LoadForm()
    {
        // Load settings
        if (!string.IsNullOrEmpty(Request.Form[hdnIsNewWebPart.UniqueID]))
        {
            IsNewWebPart = ValidationHelper.GetBoolean(Request.Form[hdnIsNewWebPart.UniqueID], false);
        }
        if (!string.IsNullOrEmpty(Request.Form[hdnInstanceGUID.UniqueID]))
        {
            InstanceGUID = ValidationHelper.GetGuid(Request.Form[hdnInstanceGUID.UniqueID], Guid.Empty);
        }

        // Indicates whether the new variant should be chosen when closing this dialog
        selectNewVariant = IsNewVariant;

        // Try to find the web part variant in the database and set its VariantID
        if (IsNewVariant)
        {
            Hashtable varProperties = WindowHelper.GetItem("variantProperties") as Hashtable;
            if (varProperties != null)
            {
                // Get the variant code name from the WindowHelper
                string variantName = ValidationHelper.GetString(varProperties["codename"], string.Empty);

                // Check if the variant exists in the database
                int variantIdFromDB = VariantHelper.GetVariantID(VariantMode, PageTemplateID, variantName, true);

                // Set the variant id from the database
                if (variantIdFromDB > 0)
                {
                    VariantID = variantIdFromDB;
                    IsNewVariant = false;
                }
            }
        }

        if (!String.IsNullOrEmpty(WebPartID))
        {
            // Get the page info
            pi = CMSWebPartPropertiesPage.GetPageInfo(AliasPath, PageTemplateID, CultureCode);

            if (pi == null)
            {
                ShowError(GetString("general.pagenotfound"));
                pnlExport.Visible = false;
                return;
            }

            // Get template
            pti = pi.UsedPageTemplateInfo;

            // Get template instance
            templateInstance = pti.TemplateInstance;

            if (!IsNewWebPart)
            {
                // Standard zone
                webPartInstance = templateInstance.GetWebPart(InstanceGUID, WebPartID);

                // If the web part not found, try to find it among the MVT/CP variants
                if (webPartInstance == null)
                {
                    // MVT/CP variant

                    // Clone templateInstance to avoid caching of the temporary template instance loaded with CP/MVT variants
                    var tempTemplateInstance = templateInstance.Clone();
                    tempTemplateInstance.LoadVariants(false, VariantModeEnum.None);

                    webPartInstance = tempTemplateInstance.GetWebPart(InstanceGUID, -1);

                    // Set the VariantMode according to the selected web part/zone variant
                    if (webPartInstance?.ParentZone != null)
                    {
                        VariantMode = (webPartInstance.VariantMode != VariantModeEnum.None) ? webPartInstance.VariantMode : webPartInstance.ParentZone.VariantMode;
                    }
                    else
                    {
                        VariantMode = VariantModeEnum.None;
                    }
                }
                else
                {
                    // Ensure that the ZoneVariantID is not set when the web part was found in a regular zone
                    ZoneVariantID = 0;
                }

                if ((VariantID > 0) && webPartInstance?.PartInstanceVariants != null)
                {
                    // Check OnlineMarketing permissions.
                    if (CheckPermissions("Read"))
                    {
                        webPartInstance = webPartInstance.FindVariant(VariantID);
                    }
                    else
                    {
                        // Not authorized for OnlineMarketing - Manage.
                        RedirectToInformation(String.Format(GetString("general.permissionresource"), "Read", (VariantMode == VariantModeEnum.ContentPersonalization) ? "CMS.ContentPersonalization" : "CMS.MVTest"));
                    }
                }

                if (webPartInstance == null)
                {
                    UIContext.EditedObject = null;
                    return;
                }
            }
            
            // Keep xml version
            if (webPartInstance != null)
            {
                xmlVersion = webPartInstance.XMLVersion;
            }

            // Get the form info
            FormInfo fi = GetWebPartFormInfo();

            // Get the form definition
            if (fi != null)
            {
                fi.ContextResolver.Settings.RelatedObject = templateInstance;
                form.AllowMacroEditing = true;

                // Get data row with required columns
                DataRow dr = fi.GetDataRow();

                if (IsNewWebPart || (xmlVersion > 0))
                {
                    fi.LoadDefaultValues(dr);
                }

                // Load values from existing web part
                LoadDataRowFromWebPart(dr, webPartInstance, fi);

                // Set a unique WebPartControlID for the new variant
                if (IsNewVariant || IsNewWebPart)
                {
                    dr["WebPartControlID"] = GetUniqueWebPartId();
                }

                // Init the form
                InitForm(form, dr, fi);

                DisplayExportPropertiesButton();
            }
            else
            {
                UIContext.EditedObject = null;
            }
        }
    }


    /// <summary>
    /// Loads the web part info
    /// </summary>
    private void EnsureWebPartInfo()
    {
        if (wpi != null)
        {
            return;
        }
        
        if (!IsNewWebPart)
        {
            // Get web part by code name
            wpi = WebPartInfoProvider.GetWebPartInfo(webPartInstance.WebPartType);
            form.Mode = FormModeEnum.Update;
        }
        else
        {
            // Web part instance wasn't created yet, get by web part ID
            wpi = WebPartInfoProvider.GetWebPartInfo(ValidationHelper.GetInteger(WebPartID, 0));
            form.Mode = FormModeEnum.Insert;
        }
    }


    /// <summary>
    /// Gets the form info for the given web part
    /// </summary>
    private FormInfo GetWebPartFormInfo()
    {
        EnsureWebPartInfo();

        return wpi?.GetWebPartFormInfo();
    }


    /// <summary>
    /// Checks permissions (depends on variant mode) 
    /// </summary>
    /// <param name="permissionName">Name of permission to test</param>
    private bool CheckPermissions(string permissionName)
    {
        var cui = MembershipContext.AuthenticatedUser;
        switch (VariantMode)
        {
            case VariantModeEnum.MVT:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName);

            case VariantModeEnum.ContentPersonalization:
                return cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);

            case VariantModeEnum.Conflicted:
            case VariantModeEnum.None:
                return cui.IsAuthorizedPerResource("cms.mvtest", permissionName) || cui.IsAuthorizedPerResource("cms.contentpersonalization", permissionName);
        }

        return true;
    }


    /// <summary>
    /// Loads the data row data from given web part instance.
    /// </summary>
    /// <param name="dr">DataRow to fill</param>
    /// <param name="webPart">Source web part</param>
    /// <param name="formInfo">Web part form info</param>
    private void LoadDataRowFromWebPart(DataRow dr, WebPartInstance webPart, FormInfo formInfo)
    {
        if (webPart != null)
        {
            foreach (DataColumn column in dr.Table.Columns)
            {
                try
                {
                    var safeColumnName = column.ColumnName.ToLowerInvariant();
                    bool load = true;
                    // switch by xml version
                    switch (xmlVersion)
                    {
                        case 1:
                            load = webPart.Properties.Contains(safeColumnName) || string.Equals("webpartcontrolid", safeColumnName, StringComparison.OrdinalIgnoreCase);
                            break;
                        // Version 0
                        default:
                            // Load default value for Boolean type in old XML version
                            if ((column.DataType == typeof(bool)) && !webPart.Properties.Contains(safeColumnName))
                            {
                                FormFieldInfo ffi = formInfo.GetFormField(column.ColumnName);
                                if (ffi != null)
                                {
                                    webPart.SetValue(column.ColumnName, ffi.GetPropertyValue(FormFieldPropertyEnum.DefaultValue));
                                }
                            }
                            break;
                    }

                    if (load)
                    {
                        var value = webPart.GetValue(column.ColumnName);

                        // Convert value into default format
                        if ((value != null) && (ValidationHelper.GetString(value, String.Empty) != String.Empty))
                        {
                            if (column.DataType == typeof(decimal))
                            {
                                value = ValidationHelper.GetDouble(value, 0, "en-us");
                            }

                            if (column.DataType == typeof(DateTime))
                            {
                                value = ValidationHelper.GetDateTime(value, DateTime.MinValue, "en-us");
                            }
                        }

                        DataHelper.SetDataRowValue(dr, column.ColumnName, value);
                    }
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException("WebPartProperties", "LOADDATAROW", ex);
                }
            }
        }
    }


    /// <summary>
    /// Initializes the form.
    /// </summary>
    /// <param name="basicForm">Form</param>
    /// <param name="dr">Data row with the data</param>
    /// <param name="fi">Form info</param>
    private void InitForm(BasicForm basicForm, DataRow dr, FormInfo fi)
    {
        if (basicForm != null)
        {
            basicForm.DataRow = dr;

            basicForm.MacroTable = webPartInstance != null ? webPartInstance.MacroTable : new Hashtable(StringComparer.InvariantCultureIgnoreCase);

            basicForm.SubmitButton.Visible = false;
            basicForm.SiteName = SiteContext.CurrentSiteName;
            basicForm.FormInformation = fi.Clone();
            basicForm.OnItemValidation += formElem_OnItemValidation;

            ApplySavedCategoryCollapsingState(basicForm);

            basicForm.ReloadData();
        }
    }


    /// <summary>
    /// Displays 'Export web part properties' button in web part properties dialog.
    /// </summary>
    private void DisplayExportPropertiesButton()
    {
        // Display 'Export web part properties button' only if the web part is already saved in page template, i.e. web part instance is defined.
        bool showExportPropertiesButton = (webPartInstance != null);
        lnkExport.Visible = showExportPropertiesButton;

        if (showExportPropertiesButton)
        {
            lnkExport.OnClientClick = "window.open('GetWebPartProperties.aspx?webpartid=" + webPartInstance.ControlID + "&webpartguid=" + webPartInstance.InstanceGUID + "&aliaspath=" + ScriptHelper.GetString(AliasPath, encapsulate: false) + "&zoneid=" + ScriptHelper.GetString(ZoneID, encapsulate: false) + "&templateid=" + PageTemplateID + "'); return false;";
        }
    }

    #endregion


    #region "Save methods"

    /// <summary>
    /// Raised when the Save action is required.
    /// </summary>
    public bool OnSave()
    {
        if (Save())
        {
            hdnIsNewWebPart.Value = "false";

            if (webPartInstance != null)
            {
                hdnInstanceGUID.Value = webPartInstance.InstanceGUID.ToString();

                if (selectNewVariant)
                {
                    // Select the new variant
                    string script = "SendEvent('updatevariantposition', true, { itemCode: 'Variant_WP_" + webPartInstance.InstanceGUID.ToString("N") + "', variantId: -1 }); ";

                    ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "CustomScripts", script, true);
                }
            }

            DisplayExportPropertiesButton();

            return true;
        }

        return false;
    }


    /// <summary>
    /// Saves the given form.
    /// </summary>
    /// <param name="form">Form to save</param>
    private static bool SaveForm(BasicForm form)
    {
        if ((form != null) && form.Visible)
        {
            return form.SaveData("");
        }

        return true;
    }


    /// <summary>
    /// Control ID validation.
    /// </summary>
    private void formElem_OnItemValidation(object sender, ref string errorMessage)
    {
        Control ctrl = (Control)sender;
        if (string.Equals("webpartcontrolid", ctrl.ID, StringComparison.OrdinalIgnoreCase))
        {
            FormEngineUserControl ctrlTextbox = (FormEngineUserControl)ctrl;

            // New web part control id 
            string newControlId = ValidationHelper.GetString(ctrlTextbox.Value, null);

            // Load the web part variants if not loaded yet
            if ((PortalContext.MVTVariantsEnabled && !templateInstance.MVTVariantsLoaded) ||
                (PortalContext.ContentPersonalizationEnabled && !templateInstance.ContentPersonalizationVariantsLoaded))
            {
                templateInstance.LoadVariants(false, VariantModeEnum.None);
            }

            // Check control ID validity
            if (!ValidationHelper.IsIdentifier(newControlId))
            {
                errorMessage = GetString("webpartproperties.controlid.allowedcharacters");
            }

            // New or changed web part control id
            bool checkIdUniqueness = IsNewWebPart || IsNewVariant || (webPartInstance == null) || (webPartInstance.ControlID != newControlId);

            // Try to find a web part with the same web part control id amongst all the web parts and their variants
            if (checkIdUniqueness
                && (templateInstance.GetWebPart(newControlId, true) != null))
            {
                // Error - duplicity IDs
                errorMessage = GetString("WebPartProperties.ErrorUniqueID");
            }
            else
            {
                string uniqueId = GetUniqueWebPartId(newControlId);
                if (!string.Equals(uniqueId, newControlId, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Check if there is already a widget with the same id in the page
                    WebPartInstance foundWidget = pi.TemplateInstance.GetWebPart(newControlId);
                    if ((foundWidget != null) && foundWidget.IsWidget)
                    {
                        // Error - the ID collide with another widget which is already in the page
                        errorMessage = ResHelper.GetString("WidgetProperties.ErrorUniqueID");
                    }
                }
            }
        }
    }


    /// <summary>
    /// Saves webpart properties.
    /// </summary>
    public bool Save()
    {
        // Check MVT/CP security
        if (VariantID > 0)
        {
            // Check OnlineMarketing permissions.
            if (!CheckPermissions("Manage"))
            {
                ShowError(GetString("general.modifynotallowed"));
                return false;
            }
        }

        // Save the data
        if ((pi != null) && (pti != null) && (templateInstance != null) && SaveForm(form))
        {
            if (SynchronizationHelper.IsCheckedOutByOtherUser(pti))
            {
                string userName = null;
                UserInfo ui = UserInfo.Provider.Get(pti.Generalized.IsCheckedOutByUserID);
                if (ui != null)
                {
                    userName = HTMLHelper.HTMLEncode(ui.GetFormattedUserName(false));
                }

                ShowError(string.Format(GetString("ObjectEditMenu.CheckedOutByAnotherUser"), pti.TypeInfo.ObjectType, pti.DisplayName, userName));
                return false;
            }

            // Add web part if new
            if (IsNewWebPart)
            {
                int webpartId = ValidationHelper.GetInteger(WebPartID, 0);

                // Ensure layout zone flag
                if (QueryHelper.GetBoolean("layoutzone", false))
                {
                    WebPartZoneInstance zone = pti.TemplateInstance.EnsureZone(ZoneID);
                    zone.LayoutZone = true;
                }

                webPartInstance = PortalHelper.AddNewWebPart(webpartId, ZoneID, false, ZoneVariantID, Position, templateInstance);

                // Set default layout
                if (wpi.WebPartParentID > 0)
                {
                    WebPartLayoutInfo wpli = WebPartLayoutInfoProvider.GetDefaultLayout(wpi.WebPartID);
                    if (wpli != null)
                    {
                        webPartInstance.SetValue("WebPartLayout", wpli.WebPartLayoutCodeName);
                    }
                }
            }

            webPartInstance.XMLVersion = 1;
            if (IsNewVariant)
            {
                webPartInstance = webPartInstance.Clone();
                webPartInstance.VariantMode = VariantModeFunctions.GetVariantModeEnum(QueryHelper.GetString("variantmode", String.Empty).ToLowerInvariant());
            }

            // Get basic form's data row and update web part
            SaveFormToWebPart(form);

            // Set new position if set
            if (PositionLeft > 0)
            {
                webPartInstance.SetValue("PositionLeft", PositionLeft);
            }
            if (PositionTop > 0)
            {
                webPartInstance.SetValue("PositionTop", PositionTop);
            }
            
            // Save the changes  
            CMSPortalManager.SaveTemplateChanges(pi, templateInstance, WidgetZoneTypeEnum.None, ViewModeEnum.Design, tree);

            // Reload the form (because of macro values set only by JS)
            form.ReloadData();

            // Clear the cached web part
            CacheHelper.TouchKey("webpartinstance|" + InstanceGUID.ToString().ToLowerInvariant());

            ShowChangesSaved();

            return true;
        }

        if (webPartInstance?.ParentZone?.ParentTemplateInstance != null)
        {
            // Reload the zone/web part variants when saving of the form fails
            webPartInstance.ParentZone.ParentTemplateInstance.LoadVariants(true, VariantModeEnum.None);
        }

        return false;
    }


    /// <summary>
    /// Saves the given DataRow data to the web part properties.
    /// </summary>
    /// <param name="basicForm">Form to save</param>
    private void SaveFormToWebPart(BasicForm basicForm)
    {
        if (basicForm.Visible && (webPartInstance != null))
        {
            // Keep the old ID to check the change of the ID
            string oldWebPartControlId = webPartInstance.ControlID.ToLowerInvariant();

            DataRow dr = basicForm.DataRow;
            foreach (DataColumn column in dr.Table.Columns)
            {
                var safeColumnName = column.ColumnName.ToLowerInvariant();

                webPartInstance.MacroTable[safeColumnName] = basicForm.MacroTable[safeColumnName];
                webPartInstance.SetValue(column.ColumnName, dr[column]);

                // If name changed, move the content
                if (safeColumnName.Equals("webpartcontrolid", StringComparison.OrdinalIgnoreCase))
                {
                    // Get the possibly updated web part control id
                    string newWebPartControlId = webPartInstance.ControlID.ToLowerInvariant();

                    ChangeWebPartControlId(oldWebPartControlId, newWebPartControlId);
                }
            }

            SaveCategoryCollapsingState(basicForm);
        }
    }


    /// <summary>
    /// Changes the web part control id in database tables which use this id as an identifier.
    /// </summary>
    private void ChangeWebPartControlId(string oldWebPartControlId, string newWebPartControlId)
    {
        if (IsNewVariant || IsNewWebPart)
        {
            return;
        }

        try
        {
            if (!String.Equals(oldWebPartControlId, newWebPartControlId, StringComparison.InvariantCultureIgnoreCase))
            {
                WebPartID = newWebPartControlId;

                // Move the document content if present
                ChangeEditableContentIDs(oldWebPartControlId, newWebPartControlId);

                // Change the underlying zone names if layout web part
                if ((wpi != null) && ((WebPartTypeEnum)wpi.WebPartType == WebPartTypeEnum.Layout))
                {
                    ChangeLayoutZoneIDs(oldWebPartControlId, newWebPartControlId);
                }
            }
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException("Content", "CHANGEWEBPART", ex);
        }
    }


    /// <summary>
    /// Changes the IDs of the editable content.
    /// </summary>
    /// <param name="oldId">Old web part ID</param>
    /// <param name="newId">New web part ID</param>
    private void ChangeEditableContentIDs(string oldId, string newId)
    {
        string currentContent = pi.EditableWebParts[oldId];
        if (currentContent != null)
        {
            TreeNode node = DocumentHelper.GetDocument(pi.DocumentID, tree);

            // Move the content in the page info
            pi.EditableWebParts[oldId] = null;
            pi.EditableWebParts[newId] = currentContent;

            // Update the document
            node.SetValue("DocumentContent", pi.GetContentXml());
            DocumentHelper.UpdateDocument(node, tree);
        }
    }


    private void ApplySavedCategoryCollapsingState(BasicForm basicForm)
    {
        if (webPartInstance == null)
        {
            return;
        }

        var fi = basicForm.FormInformation;

        foreach (var fci in fi.GetFields<FormCategoryInfo>())
        {
            var isCollapsible = ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, basicForm.ContextResolver), false);
            if (isCollapsible)
            {
                var isCollapsedValue = ValidationHelper.GetString(webPartInstance.GetValue(CAT_OPEN_PREFIX + fci.CategoryName), "");
                if (!String.IsNullOrEmpty(isCollapsedValue))
                {
                    var collapsedByDefault = ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, basicForm.ContextResolver), false);

                    var isCollapsed = ValidationHelper.GetBoolean(isCollapsedValue, false);
                    if (isCollapsed != collapsedByDefault)
                    {
                        fci.SetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, isCollapsedValue.ToLowerInvariant());
                    }
                }
            }
        }
    }


    private void SaveCategoryCollapsingState(BasicForm basicForm)
    {
        var fi = GetWebPartFormInfo();

        foreach (var fci in fi.GetFields<FormCategoryInfo>())
        {
            var isCollapsible = ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.Collapsible, basicForm.ContextResolver), false);
            if (isCollapsible)
            {
                var category = fci.CategoryName;
                var collapsedByDefault = ValidationHelper.GetBoolean(fci.GetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, basicForm.ContextResolver), false);

                var isCollapsed = basicForm.IsCategoryCollapsed(category);
                if (isCollapsed == collapsedByDefault)
                {
                    webPartInstance.SetValue(CAT_OPEN_PREFIX + category, null);
                }
                else
                {
                    webPartInstance.SetValue(CAT_OPEN_PREFIX + category, isCollapsed);
                }

                var formCategory = basicForm.FormInformation.GetFormCategory(category);
                if (formCategory != null)
                {
                    formCategory.SetPropertyValue(FormCategoryPropertyEnum.CollapsedByDefault, isCollapsed.ToString().ToLowerInvariant());
                }
            }
        }
    }


    /// <summary>
    /// Changes the layout zone IDs based on the change of the web part ID.
    /// </summary>
    /// <param name="oldId">Old web part ID</param>
    /// <param name="newId">New web part ID</param>
    private void ChangeLayoutZoneIDs(string oldId, string newId)
    {
        string prefix = oldId + "_";

        foreach (WebPartZoneInstance zone in pti.WebPartZones)
        {
            if (zone.ZoneID.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            {
                string newZoneId = newId + "_" + zone.ZoneID.Substring(prefix.Length);
                string oldZoneId = zone.ZoneID;

                // Change the zone prefix to the new one
                zone.ZoneID = newZoneId;

                WebPartEvents.ChangeLayoutZoneId.StartEvent(new ChangeLayoutZoneIdArgs
                {
                    OldZoneId = oldZoneId,
                    NewZoneId = newZoneId,
                    PageTemplateId = pti.PageTemplateId,
                    ZoneWebParts = zone.WebParts
                });
            }
        }
    }


    /// <summary>
    /// Returns new unique web part ID based on the web part display name.
    /// </summary>
    private string GetUniqueWebPartId()
    {
        var webPartControlId = ValidationHelper.GetCodeName(wpi.WebPartDisplayName);
        return GetUniqueWebPartId(webPartControlId);
    }


    /// <summary>
    /// Returns new unique web part ID based on the given <paramref name="controlId"/>.
    /// </summary>
    /// <param name="controlId">Id of target control</param>
    private string GetUniqueWebPartId(string controlId)
    {
        return WebPartZoneInstance.GetUniqueWebPartId(controlId, templateInstance);
    }

    #endregion
}