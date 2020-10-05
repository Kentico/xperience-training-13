using System;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Classes_SelectTransformation : FormEngineUserControl
{
    #region "Variables"

    private bool mDisplayClearButton = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            UniSelector.Enabled = value;
        }
    }


    /// <summary>
    /// If true, control is in site manager.
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return UniSelector.IsSiteManager;
        }
        set
        {
            UniSelector.IsSiteManager = value;
        }
    }


    /// <summary>
    /// If true selector shows hierarchical transformation.
    /// </summary>
    public bool ShowHierarchicalTransformation
    {
        get
        {
            return GetValue("ShowHierarchicalTransformation", false);
        }
        set
        {
            SetValue("ShowHierarchicalTransformation", value);
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with transformation.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return UniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Name of the edit window.
    /// </summary>
    public string EditWindowName
    {
        get
        {
            return UniSelector.EditWindowName;
        }
        set
        {
            UniSelector.EditWindowName = value;
        }
    }


    /// <summary>
    /// Path to the dialog for uniselector.
    /// </summary>
    public string NewDialogPath
    {
        get
        {
            return GetValue("NewDialogPath", "~/CMSModules/DocumentTypes/Pages/Development/DocumentType_Edit_Transformation_New.aspx");
        }
        set
        {
            SetValue("NewDialogPath", value);
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return UniSelector.Value;
        }
        set
        {
            UniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the value which determines, whether to display Clear button.
    /// </summary>
    public bool DisplayClearButton
    {
        get
        {
            return mDisplayClearButton;
        }
        set
        {
            mDisplayClearButton = value;
            UniSelector.AllowEmpty = value;
        }
    }


    /// <summary>
    /// Gets or sets the codename of setting key with default value
    /// </summary>
    public string WatermarkValueSettingKey
    {
        get
        {
            return ValidationHelper.GetString(GetValue("WatermarkValueSettingKey"), null);
        }
        set
        {
            SetValue("WatermarkValueSettingKey", value);
        }
    }


    /// <summary>
    /// Specifies aditional where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return GetValue("WhereCondition", string.Empty);
        }
        set
        {
            SetValue("WhereCondition", value);
        }
    }


    /// <summary>
    /// Gets current selector.
    /// </summary>
    private UniSelector UniSelector
    {
        get
        {
            EnsureChildControls();
            return uniSelector;
        }
    }


    /// <summary>
    /// Gets underlying form control.
    /// </summary>
    protected override FormEngineUserControl UnderlyingFormControl
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        uniSelector.IsLiveSite = IsLiveSite;
        uniSelector.ButtonClear.Visible = false;
        uniSelector.AllowEmpty = DisplayClearButton;
        uniSelector.SetValue("FilterMode", TransformationInfo.OBJECT_TYPE);
        uniSelector.EditDialogWindowWidth = 1200;
     
        // Set default value from settings as textbox watermark
        if (!String.IsNullOrEmpty(WatermarkValueSettingKey))
        {
            string watermark = SettingsKeyInfoProvider.GetValue(SiteContext.CurrentSiteName + "." + WatermarkValueSettingKey);
            if (!String.IsNullOrEmpty(watermark))
            {
                uniSelector.TextBoxSelect.WatermarkText = watermark;
            }
        }

        // Check if user can edit the transformation
        var currentUser = MembershipContext.AuthenticatedUser;
        bool deskAuthorized = currentUser.IsAuthorizedPerUIElement("CMS.Content", "Content");

        if (deskAuthorized)
        {
            // Alias path for preview transformation
            string aliasPath = QueryHelper.GetString("aliaspath", String.Empty);
            string aliasPathParam = (aliasPath == String.Empty) ? "" : "&aliaspath=" + aliasPath;

            // Instance GUID
            string instanceGUID = QueryHelper.GetString("instanceGUID", String.Empty);
            string instanceGUIDParam = (instanceGUID == String.Empty) ? "" : "&instanceguid=" + instanceGUID;

                string isSiteManagerStr = IsSiteManager ? "&siteManager=true" : String.Empty;
                string query = String.Format("objectid=##ITEMID##{0}&editonlycode=1&dialog=1", isSiteManagerStr) + aliasPathParam + instanceGUIDParam;

                string editUrl = UIContextHelper.GetElementUrl("CMS.DocumentEngine", "EditTransformation");
                uniSelector.EditItemPageUrl = URLHelper.AppendQuery(editUrl, query);

                string newUrl = NewDialogPath + "?editonlycode=1&dialog=1" + isSiteManagerStr + "&selectedvalue=##ITEMID##" + aliasPathParam + instanceGUIDParam;
                uniSelector.NewItemPageUrl = URLHelper.AddParameterToUrl(newUrl, "hash", QueryHelper.GetHash("?editonlycode=1"));
                uniSelector.EditDialogWindowHeight = 760;
        }

        string where = null;

        if (!string.IsNullOrEmpty(WhereCondition))
        {
            where = SqlHelper.AddWhereCondition(where, WhereCondition);
        }

        if (where != null)
        {
            uniSelector.WhereCondition = where;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // If macro or special value, do not validate
        string value = uniSelector.TextBoxSelect.Text.Trim();
        if (!MacroProcessor.ContainsMacro(value) && (value != string.Empty))
        {
            // Check if culture exists
            TransformationInfo ti = TransformationInfoProvider.GetTransformation(value);
            if (ti == null)
            {
                ValidationError = GetString("formcontrols_selecttransformation.notexist").Replace("%%code%%", value);
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    #endregion
}