using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_BizForms_FormControls_SelectBizForm : FormEngineUserControl
{
    #region "Variables"

    private bool mSetupFinished;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets Value display name.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return uniSelector.ValueDisplayName;
        }
    }


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
            if (uniSelector != null)
            {
                uniSelector.Enabled = value;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the textbox with selected bizforms.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return uniSelector.TextBoxSelect.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniSelector.Value;
        }
        set
        {
            if (uniSelector == null)
            {
                pnlUpdate.LoadContainer();
            }

            SetupSelector();

            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets the ID of the site for which the bizforms should be returned. 0 means current site.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets if site filter should be shown or not.
    /// </summary>
    public bool ShowSiteFilter
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowSiteFilter"), true);
        }
        set
        {
            SetValue("ShowSiteFilter", value);
        }
    }


    /// <summary>
    /// Indicates selection mode of the control.
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            SelectionModeEnum result = SelectionModeEnum.SingleDropDownList;

            // Value may be stored as integer or enum string
            object value = GetValue("SelectionMode");

            if (value is int || value is SelectionModeEnum)
            {
                result = (SelectionModeEnum)value;
            }
            else if (value is string)
            {
                Enum.TryParse(value.ToString(), true, out result);
            }

            return result;
        }
        set
        {
            SetValue("SelectionMode", (int)value);
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows empty selection. If the dialog allows empty selection,
    /// it displays the (none) field in the DDL variant and Clear button in the Textbox variant (default true).
    /// For multiple selection it returns empty string, otherwise it returns 0.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmpty"), true);
        }
        set
        {
            SetValue("AllowEmpty", value);
        }
    }


    /// <summary>
    /// Gets the inner UniSelector control.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return GetValue("UniSelector") as UniSelector;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SetValue("UniSelector", uniSelector);
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            uniSelector.StopProcessing = true;
        }
        else
        {
            SetupSelector();
        }
    }


    /// <summary>
    /// Configures the selector.
    /// </summary>
    private void SetupSelector()
    {
        if (mSetupFinished)
        {
            return;
        }

        uniSelector.SelectionMode = SelectionMode;
        uniSelector.AllowEmpty = AllowEmpty;

        // If current control context is widget or livesite or SingleDropDownList mode hide site selector
        if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES)
            || SelectionMode == SelectionModeEnum.SingleDropDownList)
        {
            ShowSiteFilter = false;
        }

        uniSelector.IsLiveSite = IsLiveSite;

        // Return form name or ID according to type of field (if no field specified form name is returned)
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            uniSelector.ReturnColumnName = "FormID";
            ShowSiteFilter = false;
            uniSelector.AllowEmpty = true;
        }
        else
        {
            uniSelector.ReturnColumnName = "FormName";
        }

        // Add sites filter
        if (ShowSiteFilter)
        {
            uniSelector.FilterControl = "~/CMSFormControls/Filters/SiteFilter.ascx";
            uniSelector.SetValue("DefaultFilterValue", (SiteID > 0) ? SiteID : SiteContext.CurrentSiteID);
            uniSelector.SetValue("FilterMode", "bizform");
        }
        // Select bizforms depending on a site if not filtered by uniselector site filter
        else
        {
            int siteId = (SiteID == 0) ? SiteContext.CurrentSiteID : SiteID;
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, "FormSiteID = " + siteId);
        }

        mSetupFinished = true;
    }


    /// <summary>
    /// Returns WHERE condition for selected form.
    /// </summary>
    public override string GetWhereCondition()
    {
        // Return correct WHERE condition for integer if none value is selected
        if ((FieldInfo != null) && DataTypeManager.IsInteger(TypeEnum.Field, FieldInfo.DataType))
        {
            int id = ValidationHelper.GetInteger(uniSelector.Value, 0);
            if (id > 0)
            {
                return base.GetWhereCondition();
            }
        }
        return null;
    }

    #endregion
}