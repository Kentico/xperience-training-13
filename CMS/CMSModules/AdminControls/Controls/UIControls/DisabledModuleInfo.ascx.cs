using System;

using CMS.Helpers;

using System.Linq;

using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_DisabledModuleInfo : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Set keys for module check.
    /// </summary>
    public String SettingKeys
    {
        get
        {
            return GetStringContextValue("SettingKeys");
        }
        set
        {
            SetValue("SettingKeys", value);
        }
    }


    /// <summary>
    /// Set keys for module check.
    /// </summary>
    public DisabledModuleScope KeyScope
    {
        get
        {
            return EnumStringRepresentationExtensions.ToEnum<DisabledModuleScope>(GetStringContextValue("KeyScope", "Both"));
        }
        set
        {
            SetValue("SettingKeys", value);
        }
    }


    public String SiteButtonText
    {
        get
        {
            return GetStringContextValue("SiteButtonText");
        }
        set
        {
            SetValue("SiteButtonText", value);
        }
    }


    public String GlobalButtonText
    {
        get
        {
            return GetStringContextValue("GlobalButtonText");
        }
        set
        {
            SetValue("GlobalButtonText", value);
        }
    }


    public String DisabledModuleInfoText
    {
        get
        {
            return GetStringContextValue("DisabledModuleInfoText");
        }
        set
        {
            SetValue("DisabledModuleInfoText", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(SettingKeys))
        {
            dModule.TestSettingKeys = SettingKeys;
            dModule.KeyScope = KeyScope;
            dModule.SiteButtonText = SiteButtonText;
            dModule.GlobalButtonText = GlobalButtonText;
            dModule.InfoText = DisabledModuleInfoText;
        }
        else
        {
            Visible = false;
            StopProcessing = true;
        }
    }

    #endregion
}
