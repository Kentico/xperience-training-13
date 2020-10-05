using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Objects_Controls_CloneObject : CMSUserControl
{
    private string mCloseScript = null;
    private CloneSettingsControl customProperties = null;
    private ObjectTypeInfo typeInfo = null;
    private int mDisplayNameMaxLength;

    private ICollection<string> excludedChildren = new List<string>();
    private ICollection<string> excludedBindings = new List<string>();
    private ICollection<string> excludedOtherBindings = new List<string>();


    /// <summary>
    /// Returns script which should be run when cloning is successfully finished.
    /// </summary>
    public string CloseScript
    {
        get
        {
            if (!string.IsNullOrEmpty(mCloseScript))
            {
                return mCloseScript;
            }

            return "RefreshContent(); CloseDialog();";
        }
    }


    /// <summary>
    /// Gets or sets BaseInfo object to be cloned.
    /// </summary>
    public BaseInfo InfoToClone
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if user required transaction to be used within cloning.
    /// </summary>
    public bool UseTransaction
    {
        get
        {
            return chkUseTransaction.Checked;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (InfoToClone == null)
        {
            return;
        }

        ScriptHelper.RegisterJQuery(Page);

        typeInfo = InfoToClone.TypeInfo;

        SetLabel(lblDisplayName, "displaynamelabel", "clonning.newdisplayname");
        SetLabel(lblCodeName, "codenamelabel", "clonning.newcodename");

        lblKeepFieldsTranslated.ToolTip = GetString("clonning.settings.keepfieldstranslated.tooltip");
        lblCloneUnderSite.ToolTip = GetString("clonning.settings.cloneundersite.tooltip");
        lblMetafiles.ToolTip = GetString("clonning.settings.metafiles.tooltip");
        lblMaxRelativeLevel.ToolTip = GetString("clonning.settings.maxrelativelevel.tooltip");

        FormInfo formInfo = FormHelper.GetFormInfo(typeInfo.ObjectClassName, false);

        SetUpCodeNameControl(formInfo);
        SetUpDisplayNameControl(formInfo);

        customProperties = LoadCustomProperties(typeInfo.ObjectType, typeInfo.OriginalObjectType);
        if (customProperties != null)
        {
            SetUpControlsByCustomProperties();
        }

        siteElem.AllowGlobal = typeInfo.SupportsGlobalObjects;
        plcCloneUnderSite.Visible = ShowCloneUnderSite();

        RemoveSiteBindingFromBindings();
        SetUpChildrenControl();
        SetUpSiteBindingsControls();

        // Allow meta files control if any
        if (InfoToClone.MetaFiles?.Count > 0)
        {
            plcMetafiles.Visible = true;
        }

        if (!RequestHelper.IsPostBack())
        {
            // Preselect site of the object as a "clone under site" option
            if (plcCloneUnderSite.Visible)
            {
                siteElem.SiteName = InfoToClone.Generalized.ObjectSiteName;
            }

            // Exception for cultures for assigning to current site (for cultures the default value should be false)
            if (typeInfo.ObjectType == CultureInfo.OBJECT_TYPE)
            {
                chkAssignToCurrentSite.Checked = false;
            }
        }

        if (plcChildren.Visible)
        {
            LoadMaxRelativeLevel();
        }

    }

    /// <summary>
    /// Clones the object to the DB according to provided settings.
    /// </summary>
    /// 
    public CloneResult CloneObject()
    {
        if (InfoToClone == null)
        {
            return null;
        }

        TransferExcludedTypes();

        // Check code name
        if (plcCodeName.Visible)
        {
            bool checkCodeName = customProperties?.ValidateCodeName ?? true;
            if (checkCodeName && !ValidationHelper.IsCodeName(txtCodeName.Text))
            {
                ShowError(GetString("general.invalidcodename"));

                return null;
            }
        }

        // Check display name length
        if (plcDisplayName.Visible && (txtDisplayName.Text.Length > mDisplayNameMaxLength))
        {
            ShowError(string.Format(GetString("cloning.displayname.maxlengthexceed"), mDisplayNameMaxLength));

            return null;
        }

        // Check permissions
        string targetSiteName = SiteContext.CurrentSiteName;
        if (plcCloneUnderSite.Visible && siteElem.Visible && (siteElem.SiteID > 0))
        {
            targetSiteName = SiteInfoProvider.GetSiteName(siteElem.SiteID);
        }

        // Check object permissions (Create & Modify)
        try
        {
            InfoToClone.CheckPermissions(PermissionsEnum.Create, targetSiteName, CurrentUser, true);
            InfoToClone.CheckPermissions(PermissionsEnum.Modify, targetSiteName, CurrentUser, true);
        }
        catch (PermissionCheckException ex)
        {
            RedirectToAccessDenied(ex.ModuleName, ex.PermissionFailed);
        }

        CloneSettings settings = InitializeCloneSettings();

        if (settings == null)
        {
            return null;
        }

        var result = new CloneResult();
        BaseInfo clone;

        if (chkUseTransaction.Checked)
        {
            using (var transaction = new CMSTransactionScope())
            {
                clone = InfoToClone.Generalized.InsertAsClone(settings, result);
                transaction.Commit();
            }
        }
        else
        {
            clone = InfoToClone.Generalized.InsertAsClone(settings, result);
        }

        string script = customProperties?.CloseScript;
        if (!string.IsNullOrEmpty(script))
        {
            mCloseScript = script.Replace("{0}", clone.Generalized.ObjectID.ToString());
        }

        return result;
    }


    protected string GetCustomParametersTitle()
    {
        if (InfoToClone != null)
        {
            return string.Format(GetString("clonning.settings.customparameters"), GetString($"objecttype.{TranslationHelper.GetSafeClassName(InfoToClone.TypeInfo.ObjectType)}"));
        }

        return string.Empty;
    }


    /// <summary>
    /// Sets up display name control if any.
    /// </summary>
    private void SetUpDisplayNameControl(FormInfo formInfo)
    {
        plcDisplayName.Visible = (typeInfo.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN) && !string.Equals(typeInfo.DisplayNameColumn, typeInfo.CodeNameColumn, StringComparison.OrdinalIgnoreCase);
        if (plcDisplayName.Visible)
        {
            txtDisplayName.MaxLength = mDisplayNameMaxLength = GetMaxLengthOrDefault(formInfo, typeInfo.DisplayNameColumn, 200);

            if (!RequestHelper.IsPostBack())
            {
                txtDisplayName.Text = InfoToClone.Generalized.GetUniqueDisplayName(false);
            }
        }
    }


    /// <summary>
    /// Sets up code name control if any.
    /// </summary>
    /// <param name="formInfo"></param>
    private void SetUpCodeNameControl(FormInfo formInfo)
    {
        plcCodeName.Visible = (typeInfo.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN);
        if (plcCodeName.Visible)
        {
            txtCodeName.MaxLength = GetMaxLengthOrDefault(formInfo, typeInfo.CodeNameColumn, 100);

            if (!RequestHelper.IsPostBack())
            {
                txtCodeName.Text = InfoToClone.Generalized.GetUniqueCodeName();
            }
        }
    }


    /// <summary>
    /// Sets site bindings.
    /// </summary>
    /// <remarks>For objects with SiteID column allow site bindings only for global versions of the object (for example polls).</remarks>
    private void SetUpSiteBindingsControls()
    {
        bool isGlobalObject = !string.IsNullOrEmpty(typeInfo.SiteBinding)
            && (InfoToClone.Generalized.ObjectGroupID == 0)
            && (typeInfo.SiteIDColumn == ObjectTypeInfo.COLUMN_NAME_UNKNOWN) || (InfoToClone.Generalized.ObjectSiteID == 0);

        if (isGlobalObject)
        {
            lblAssignToCurrentSite.ToolTip = GetString("clonning.settings.assigntocurrentsite.tooltip");
            plcAssignToCurrentSite.Visible = true;

            lblSiteBindings.ToolTip = GetCloneHelpText(typeInfo.SiteBinding);
            plcSiteBindings.Visible = true;

            if (!MembershipContext.AuthenticatedUser?.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) ?? false)
            {
                chkAssignToCurrentSite.Checked = true;
                chkAssignToCurrentSite.Enabled = false;

                chkSiteBindings.Checked = false;
                chkSiteBindings.Enabled = false;
            }
        }
    }


    /// <summary>
    /// Sets children control if any.
    /// </summary>
    private void SetUpChildrenControl()
    {
        if (typeInfo.ChildObjectTypes?.Any() ?? false)
        {
            int itemNumber;
            lblChildren.ToolTip = GetCloneHelpText(typeInfo.ChildObjectTypes, excludedChildren, out itemNumber);

            if (itemNumber == 1)
            {
                lblChildren.Text = lblChildren.ToolTip;
                lblChildren.ToolTip = string.Empty;
            }
            else
            {
                lblChildren.Text = GetString("clonning.settings.children");
            }

            plcChildren.Visible = (itemNumber > 0);
            plcChildrenLevel.Visible = ShowChildrenLevel(excludedChildren);
        }
    }


    /// <summary>
    /// Removes site binding from initialized bindings.
    /// </summary>
    private void RemoveSiteBindingFromBindings()
    {
        var bindings = (typeInfo.BindingObjectTypes ?? Enumerable.Empty<string>())?
            .Union(typeInfo.OtherBindingObjectTypes ?? Enumerable.Empty<string>())?
            .ToList();

        if (!bindings?.Any() ?? true)
        {
            return;
        }

        if (typeInfo.SiteBinding != null)
        {
            bindings.Remove(typeInfo.SiteBinding);
        }

        var excludedTypes = excludedBindings
            .Concat(excludedOtherBindings)
            .ToList();

        int itemNumber;
        lblBindings.ToolTip = GetCloneHelpText(bindings, excludedTypes, out itemNumber);

        if (itemNumber == 1)
        {
            lblBindings.Text = lblBindings.ToolTip;
            lblBindings.ToolTip = string.Empty;
        }
        else
        {
            SetLabel(lblBindings, "bindingslabel", "clonning.settings.bindings");
        }

        plcBindings.Visible = itemNumber > 0;
    }


    /// <summary>
    /// Sets controls defined by the custom properties.
    /// </summary>
    private void SetUpControlsByCustomProperties()
    {
        headCustom.Text = GetCustomParametersTitle();
        customProperties.ID = "customProperties";
        customProperties.InfoToClone = InfoToClone;

        plcCustomParameters.Controls.Add(customProperties);
        plcCustomParametersBox.Visible = customProperties.DisplayControl;

        if (customProperties.HideDisplayName)
        {
            plcDisplayName.Visible = false;
        }
        if (customProperties.HideCodeName)
        {
            plcCodeName.Visible = false;
        }

        if (!RequestHelper.IsPostBack())
        {
            TransferExcludedTypes();
        }
    }


    /// <summary>
    /// Returns max <paramref name="columnName"/> length to value defined in <paramref name="formInfo"/> definition, if it's not defined returns <paramref name="defaultValue"/>.
    /// </summary>
    private int GetMaxLengthOrDefault(FormInfo formInfo, string columnName, int defaultValue)
    {
        var maxLength = formInfo?.GetFormField(columnName)?.Size ?? 0;
        return (maxLength > 0) ? maxLength : defaultValue;
    }


    /// <summary>
    /// Determines whether the children objects have their own children.
    /// </summary>
    /// <param name="excludedTypes">Excluded child types</param>
    private bool ShowChildrenLevel(ICollection<string> excludedTypes)
    {
        ObjectTypeInfo typeInfo = InfoToClone.TypeInfo;
        if (typeInfo.ChildObjectTypes == null)
        {
            return false;
        }

        return typeInfo.ChildObjectTypes
            .Where(type => (excludedTypes == null) || !excludedTypes.Contains(type))
            .Select(type => ModuleManager.GetReadOnlyObject(type).TypeInfo.ChildObjectTypes)
            .Any(childTypes => (childTypes != null) && childTypes.Any());
    }


    /// <summary>
    /// Indicates if any setting is relevant (and therefore visible) for the given object.
    /// </summary>
    public bool HasNoSettings()
    {
        return !(plcMetafiles.Visible || plcCloneUnderSite.Visible || plcCodeName.Visible || plcCustomParameters.Visible || plcDisplayName.Visible || plcChildren.Visible || plcSiteBindings.Visible);
    }


    /// <summary>
    /// Creates tooltip for given list of object types.
    /// </summary>
    /// <param name="objectType">Object type</param>
    private string GetCloneHelpText(string objectType)
    {
        int dummy;
        return GetCloneHelpText(new [] { objectType }, null, out dummy);
    }


    /// <summary>
    /// Creates tooltip for given list of object types.
    /// </summary>
    /// <param name="objTypes">Object types list</param>
    /// <param name="excludedTypes">Object types which should be excluded</param>
    /// <param name="itemNumber">Number of items</param>
    private string GetCloneHelpText(IEnumerable<string> objTypes, ICollection<string> excludedTypes, out int itemNumber)
    {
        var types = objTypes
            .Where(type => (excludedTypes == null) || !excludedTypes.Contains(type))
            .Select(type => GetString($"objecttype.{TranslationHelper.GetSafeClassName(type)}"))
            .ToList();

        itemNumber = types.Count;
        if (itemNumber == 1)
        {
            string baseName = types.FirstOrDefault();
            if (baseName?.Length > 2)
            {
                if (Char.IsUpper(baseName[0]) && !Char.IsUpper(baseName[1]))
                {
                    baseName = Char.ToLowerInvariant(baseName[0]) + baseName.Substring(1);
                }
            }
            return string.Format(GetString("clonning.settings.oneitemhelp"), baseName.Trim());
        }

        return string.Format(GetString("clonning.settings.tooltiphelp"), string.Join(", ", types));
    }


    /// <summary>
    /// Load dropdown with MaxRelativeLevel.
    /// </summary>
    private void LoadMaxRelativeLevel()
    {
        if (drpMaxRelativeLevel.Items.Count == 0)
        {
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.all"), "-1"));
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.1"), "1"));
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.2"), "2"));
            drpMaxRelativeLevel.Items.Add(new ListItem(GetString("clonning.settings.level.3"), "3"));
        }
    }


    /// <summary>
    /// Returns initialized clone settings based on filled controls.
    /// </summary>
    private CloneSettings InitializeCloneSettings()
    {
        var settings = new CloneSettings
        {
            KeepFieldsTranslated = chkKeepFieldsTranslated.Checked,
            CloneBase = InfoToClone,
            CodeName = txtCodeName.Text,
            DisplayName = txtDisplayName.Text,
            IncludeBindings = chkBindings.Checked,
            IncludeOtherBindings = chkBindings.Checked,
            IncludeChildren = chkChildren.Checked,
            IncludeMetafiles = chkMetafiles.Checked,
            IncludeSiteBindings = chkSiteBindings.Checked,
            MaxRelativeLevel = ValidationHelper.GetInteger(drpMaxRelativeLevel.SelectedValue, -1),
            CloneToSiteID = (plcCloneUnderSite.Visible && siteElem.Visible) ? siteElem.SiteID : InfoToClone.Generalized.ObjectSiteID
        };

        if (plcAssignToCurrentSite.Visible)
        {
            settings.AssignToSiteID = (chkAssignToCurrentSite.Checked ? SiteContext.CurrentSiteID : 0);
        }

        if (customProperties != null)
        {
            if (customProperties.IsValid(settings))
            {
                var customParameters = customProperties.CustomParameters;
                if (customParameters != null)
                {
                    settings.CustomParameters = customParameters;
                }

                settings.ExcludedChildTypes.AddRange(excludedChildren);
                settings.ExcludedBindingTypes.AddRange(excludedBindings);
                settings.ExcludedOtherBindingTypes.AddRange(excludedOtherBindings);
            }
            else
            {
                return null;
            }
        }

        if (InfoToClone.Parent != null)
        {
            settings.ParentID = InfoToClone.Parent.Generalized.ObjectID;
        }

        return settings;
    }


    private void TransferExcludedTypes()
    {
        if (customProperties != null)
        {
            string children = customProperties.ExcludedChildTypes;
            string bindings = customProperties.ExcludedBindingTypes;
            string otherBindings = customProperties.ExcludedOtherBindingTypes;
            char[] separator = { ';' };
            if (!string.IsNullOrEmpty(children))
            {
                excludedChildren = children.Split(separator, StringSplitOptions.None);
            }
            if (!string.IsNullOrEmpty(bindings))
            {
                excludedBindings = bindings.Split(separator, StringSplitOptions.None);
            }
            if (!string.IsNullOrEmpty(otherBindings))
            {
                excludedOtherBindings = otherBindings.Split(separator, StringSplitOptions.None);
            }
        }
    }


    /// <summary>
    /// Tries to load clone settings control of <paramref name="objectType"/> or <paramref name="originalObjectType"/>.
    /// </summary>
    /// <param name="objectType">Object type of current cloned object.</param>
    /// <param name="originalObjectType">Original object type of current cloned object.</param>
    /// <returns>Clone settings control, if any. Returns <c>null</c> otherwise.</returns>
    private CloneSettingsControl LoadCustomProperties(string objectType, string originalObjectType)
    {
        CloneSettingsControl customProperties = LoadCustomProperties(objectType);

        if ((customProperties == null) && (objectType != originalObjectType))
        {
            // Try get original object type settings control
            customProperties = LoadCustomProperties(originalObjectType);
        }

        return customProperties;
    }


    /// <summary>
    /// Returns <c>true</c> only for Admin and for controls which have SiteID (and are not under group or any other parent) and are not from E-Commerce module. Returns <c>false</c> otherwise.
    /// </summary>
    private bool ShowCloneUnderSite()
    {
        int sitesCount = SiteInfoProvider.GetSitesCount();

        bool isAdmin = MembershipContext.AuthenticatedUser?.CheckPrivilegeLevel(UserPrivilegeLevelEnum.Admin) ?? false;

        bool isSupportedModule = !ModuleName.ECOMMERCE.Equals(typeInfo.ModuleName, StringComparison.OrdinalIgnoreCase);

        bool isSupportedObject = ((typeInfo.SupportsGlobalObjects && (sitesCount > 0)) || (sitesCount > 1))
                    && (typeInfo.SiteIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
                    && (typeInfo.OriginalObjectType != CategoryInfo.OBJECT_TYPE);

        bool hasNoBindings = (InfoToClone.Generalized.ObjectParentID == 0)
                    && (InfoToClone.Generalized.ObjectGroupID == 0);

        return typeInfo.SupportsCloneToOtherSite
                    && hasNoBindings
                    && isAdmin
                    && isSupportedModule
                    && isSupportedObject;
    }


    /// <summary>
    /// Loads custom object type properties control.
    /// </summary>
    /// <param name="objectType">Object type of current cloned object</param>
    private CloneSettingsControl LoadCustomProperties(string objectType)
    {
        Type customCloneControl;
        if (CustomCloneSettings.TryGetControl(objectType, out customCloneControl))
        {
            return Activator.CreateInstance(customCloneControl) as CloneSettingsControl;
        }

        string fileName = $"{TranslationHelper.GetSafeClassName(objectType)}Settings.ascx";
        string generalControlFile = "~/CMSModules/Objects/FormControls/Cloning/" + fileName;
        string moduleControlFile = (string.IsNullOrEmpty(InfoToClone.TypeInfo.ModuleInfo?.ModuleRootPath)
            ? generalControlFile
            : $"{InfoToClone.TypeInfo.ModuleInfo.ModuleRootPath.TrimEnd('/')}/FormControls/Cloning/{fileName}");

        if (customProperties == null)
        {
            try
            {
                customProperties = LoadUserControl(moduleControlFile) as CloneSettingsControl;
            }
            catch { }
        }

        if (customProperties == null)
        {
            try
            {
                customProperties = LoadUserControl(generalControlFile) as CloneSettingsControl;
            }
            catch { }
        }

        return customProperties;
    }


    private void SetLabel(LocalizedLabel label, string suffix, string defaultString)
    {
        string stringPrefixName = $"cloning.settings.{TranslationHelper.GetSafeClassName(InfoToClone.TypeInfo.ObjectType)}.";
        string newString = stringPrefixName + suffix;

        label.ResourceString = (GetString(newString) != newString) ? newString : defaultString;
    }
}
