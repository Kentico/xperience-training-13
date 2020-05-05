using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSModules_Ecommerce_FormControls_QuickAdd : CMSEcommerceModalPage
{
    #region "Variables"

    private BaseInfo mEmptyObject;
    private bool? mAllowGlobalObjects;

    #endregion


    #region "Properties"

    /// <summary>
    /// ObjectType of created object.
    /// </summary>
    private string ObjectType
    {
        get
        {
            return QueryHelper.GetString("objectType", string.Empty);
        }
    }


    /// <summary>
    /// Indicates if creating new object to be bound to global object.
    /// </summary>
    private bool NewForGlobalObject
    {
        get
        {
            return QueryHelper.GetInteger("site", 1) == 0;
        }
    }


    /// <summary>
    /// Indicates whether global objects are allowed for current site.
    /// </summary>
    private bool AllowGlobalObjects
    {
        get
        {
            if (!mAllowGlobalObjects.HasValue)
            {
                mAllowGlobalObjects = ECommerceSettings.AllowGlobalObjects(CurrentSiteName, ObjectType);
            }

            return mAllowGlobalObjects.Value;
        }
    }


    /// <summary>
    /// Indicates if objects can be combined site and global within one site. 
    /// False means that site can use only global or only site records.
    /// </summary>
    private bool AllowCombineSiteAndGlobal
    {
        get
        {
            return EmptyObject.TypeInfo.NameGloballyUnique;
        }
    }


    /// <summary>
    /// Read only object of type specified by ObjectType.
    /// </summary>
    private BaseInfo EmptyObject
    {
        get
        {
            return mEmptyObject ?? (mEmptyObject = ModuleManager.GetReadOnlyObject(ObjectType));
        }
    }


    /// <summary>
    /// The name of SiteID column of created object.
    /// </summary>
    private string SiteIDColumn
    {
        get
        {
            return EmptyObject.TypeInfo.SiteIDColumn;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Check hash
        QueryHelper.ValidateHash("hash", "saved;selectorid;selectedvalue", null, true);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        HeaderActions.Visible = false;
        EditForm.SubmitButton.Visible = false;

        ScriptHelper.RegisterWOpenerScript(this);

        Save += btnOK_Click;

        // Figure out icon, heading and prepare title
        SetTitle(GetTitleText());

        // Hide site or global selector if no choice
        if (NewForGlobalObject || !AllowGlobalObjects || !AllowCombineSiteAndGlobal)
        {
            FormFieldInfo siteField = EditForm.FormInformation.GetFormField(SiteIDColumn);
            siteField.SetPropertyValue(FormFieldPropertyEnum.VisibleMacro, "{%false%}", true);
        }
    }

    #endregion


    #region "Event handlers"

    protected void OnFormCreate(object sender, EventArgs e)
    {
        // Init object type
        EditForm.ObjectType = ObjectType;

        // Check if special alternative form for quick add exists
        if (FormHelper.GetFormInfo(ObjectType + ".quickadd", true) != null)
        {
            EditForm.AlternativeFormName = "quickadd";
        }

        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
        EditForm.OnAfterSave += EditForm_OnAfterSave;
    }


    protected void OnAfterDataLoad(object sender, EventArgs e)
    {
        // Set site ID of new object (default current site)
        EditForm.Data.SetValue(SiteIDColumn, SiteContext.CurrentSiteID);
        EnsureFormSiteID();
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        if (EditForm.ValidateData())
        {
            EditForm.SaveData("");
        }
    }


    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        EnsureFormSiteID();

        // Check permissions and redirect to access denied page when check failed
        if (!((BaseInfo)EditForm.Data).CheckPermissions(PermissionsEnum.Create, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(GetString("general.actiondenied"));
        }
    }


    private void EditForm_OnAfterSave(object sender, EventArgs e)
    {
        BaseInfo newObject = EditForm.Data as BaseInfo;
        if (newObject != null)
        {
            RegisterChangeSelector(newObject.Generalized.ObjectID);
        }

        // Set data to database without redirection
        EditForm.RedirectUrlAfterCreate = "";
        EditForm.RedirectUrlAfterSave = "";
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates heading for title.
    /// </summary>
    private string GetTitleText()
    {
        var objectName = TypeHelper.GetNiceObjectTypeName(ObjectType).ToLowerInvariant();
        return string.Format(GetString("com.quickadd.titlepattern"), objectName);
    }


    /// <summary>
    /// Sets SiteID to forms data, when there is no choice.
    /// </summary>
    private void EnsureFormSiteID()
    {
        if (NewForGlobalObject || !AllowCombineSiteAndGlobal)
        {
            int siteId = NewForGlobalObject || (!AllowCombineSiteAndGlobal && AllowGlobalObjects) ? 0 : SiteContext.CurrentSiteID;

            // Set SiteID to data
            EditForm.Data.SetValue(SiteIDColumn, (siteId > 0) ? (object)siteId : null);

            // Set SiteID to control when present
            var siteFormControl = EditForm.FieldEditingControls[SiteIDColumn];
            if (siteFormControl != null)
            {
                siteFormControl.Value = siteId == 0 ? (object)null : siteId;
            }
        }
    }


    /// <summary>
    /// Registers script for changing the selector.
    /// </summary>
    /// <param name="newObjectId">The address id.</param>
    private void RegisterChangeSelector(int newObjectId)
    {
        // Get selector ID
        var selector = QueryHelper.GetControlClientId("selectorId", string.Empty);

        // Check for selector ID
        if (!string.IsNullOrEmpty(selector) && (newObjectId > 0))
        {
            // Add selector refresh
            string script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{wopener.US_SelectNewValue_{0}('{1}'); }} CloseDialog();",
                                          selector, newObjectId);
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "UpdateSelector", ScriptHelper.GetScript(script));
        }
    }

    #endregion
}
