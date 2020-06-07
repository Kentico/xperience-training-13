using System;

using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_LocalizableTextBox_ResourceStringSelector : FormEngineUserControl
{
    #region "Public properties"

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
            uniSelector.Value = value;
        }
    }


    /// <summary>
    /// Gets inner uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }

    #endregion


    #region "Methods"

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


    protected override void EnsureChildControls()
    {
        if (uniSelector == null)
        {
            pnlUpdate.LoadContainer();
        }
        base.EnsureChildControls();
    }


    /// <summary>
    /// Reloads the data in the selector.
    /// </summary>
    public void ReloadData()
    {
        CultureInfo ui;
        try
        {
            ui = CultureInfo.Provider.Get(MembershipContext.AuthenticatedUser.PreferredUICultureCode);
        }
        catch
        {
            ui = CultureInfo.Provider.Get(CultureHelper.DefaultUICultureCode);
        }
        if (ui != null)
        {
            uniSelector.WhereCondition = SqlHelper.AddWhereCondition(uniSelector.WhereCondition, "CultureID = " + ui.CultureID);
            uniSelector.ReturnColumnName = "StringKey";
            uniSelector.AdditionalColumns = "StringKey, TranslationText";
            uniSelector.DialogGridName = "~/CMSFormControls/Selectors/LocalizableTextBox/ResourceStringSelector.xml";
            uniSelector.IsLiveSite = IsLiveSite;
            uniSelector.Reload(false);
            uniSelector.DialogWindowWidth = 850;
            uniSelector.ResourcePrefix = "resourcestring";
            uniSelector.UseDefaultNameFilter = false;
        }
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        ValidationError = new Validator().NotEmpty(uniSelector.Value, GetString("culture.enterakey")).IsCodeName(uniSelector.Value, GetString("culture.InvalidCodeName")).Result;
        if (!String.IsNullOrEmpty(ValidationError))
        {
            return false;
        }
        return true;
    }

    #endregion
}