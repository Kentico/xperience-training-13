using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSAdminControls_Basic_ThreeStateCheckBox : FormEngineUserControl
{
    #region "Constants"

    public const string DEFAULT_KEY = "##DEFAULT##";

    #endregion


    #region "Variables"

    private object mPositiveValue = 1;
    private object mNegativeValue = 0;
    private object mNotSetValue = -1;
    private object mDefaultValue;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets control representing three-state positive choice
    /// </summary>
    public CMSRadioButton PositiveChoice
    {
        get
        {
            return rbPositive;
        }
    }


    /// <summary>
    /// Gets control representing three-state negative choice
    /// </summary>
    public CMSRadioButton NegativeChoice
    {
        get
        {
            return rbNegative;
        }
    }


    /// <summary>
    /// Gets control representing three-state not set choice
    /// </summary>
    public CMSRadioButton NotSetChoice
    {
        get
        {
            return rbNotSet;
        }
    }


    /// <summary>
    /// Indicates if postback should be performed
    /// </summary>
    public bool AutoPostBack
    {
        get
        {
            return PositiveChoice.AutoPostBack;
        }
        set
        {
            PositiveChoice.AutoPostBack = value;
            NegativeChoice.AutoPostBack = value;
            NotSetChoice.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Gets or sets value representing three-state not set choice
    /// </summary>
    public object NotSetValue
    {
        get
        {
            return mNotSetValue;
        }
        set
        {
            mNotSetValue = value;
        }
    }


    /// <summary>
    /// Gets or sets value representing three-state positive choice
    /// </summary>
    public object PositiveValue
    {
        get
        {
            return mPositiveValue;
        }
        set
        {
            mPositiveValue = value;
        }
    }


    /// <summary>
    /// Gets or sets value representing three-state negative choice
    /// </summary>
    public object NegativeValue
    {
        get
        {
            return mNegativeValue;
        }
        set
        {
            mNegativeValue = value;
        }
    }


    /// <summary>
    /// Gets or sets value of the control
    /// </summary>
    public override object Value
    {
        get
        {
            if (PositiveChoice.Checked)
            {
                return PositiveValue;
            }
            else if (NegativeChoice.Checked)
            {
                return NegativeValue;
            }
            else
            {
                return NotSetValue;
            }
        }
        set
        {
            if (value != null)
            {
                int hash = value.GetHashCode();
                if (hash == PositiveValue.GetHashCode())
                {
                    PositiveChoice.Checked = true;
                    NegativeChoice.Checked = false;
                    NotSetChoice.Checked = false;
                }
                else if (hash == NegativeValue.GetHashCode())
                {
                    NegativeChoice.Checked = true;
                    PositiveChoice.Checked = false;
                    NotSetChoice.Checked = false;
                }
                else
                {
                    NotSetChoice.Checked = true;
                    NegativeChoice.Checked = false;
                    PositiveChoice.Checked = false;
                }
            }
        }
    }


    /// <summary>
    /// Setting key code name used to initialize not set choice value
    /// </summary>
    public string SettingKeyName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SettingKeyName"), null);
        }
    }


    /// <summary>
    /// Gets or sets if control is enabled
    /// </summary>
    public override bool Enabled
    {
        get
        {
            base.Enabled = PositiveChoice.Enabled && NegativeChoice.Enabled && NotSetChoice.Enabled;
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            PositiveChoice.Enabled = NegativeChoice.Enabled = NotSetChoice.Enabled = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event for control state change
    /// </summary>
    public event EventHandler CheckedChanged;

    #endregion


    #region "Control events"

    /// <summary>
    /// Page init event
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            if (!NotSetChoice.Checked && !NegativeChoice.Checked && !PositiveChoice.Checked)
            {
                NotSetChoice.Checked = true;
            }           
        }

        NotSetChoice.Text = GetString("general.notset") + (string.IsNullOrEmpty(SettingKeyName) ? null : " (" + DEFAULT_KEY + ")");
        PositiveChoice.Text = GetString("general.yes");
        NegativeChoice.Text = GetString("general.no");

        if (!string.IsNullOrEmpty(SettingKeyName))
        {
            SetDefaultValue(SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + "." + SettingKeyName));
        }

        base.OnInit(e);
    }


    /// <summary>
    /// Page PreRender event
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        NotSetChoice.CheckedChanged += RaiseCheckedChanged;
        PositiveChoice.CheckedChanged += RaiseCheckedChanged;
        NegativeChoice.CheckedChanged += RaiseCheckedChanged;
    }


    protected void RaiseCheckedChanged(object sender, EventArgs e)
    {
        if (CheckedChanged != null)
        {
            CheckedChanged(sender, e);
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Gets actual value considering actual default value
    /// </summary>
    public object GetActualValue()
    {
        object val = Value;
        if (val == NotSetValue)
        {
            return mDefaultValue;
        }

        return val;
    }


    /// <summary>
    /// Replace text ##DEFAULT## with default value in text description of not set RadioButton control
    /// </summary>
    /// <param name="defaultValue">Default value</param>
    public void SetDefaultValue(object defaultValue)
    {
        mDefaultValue = defaultValue;
        string replaceString = null;
        int defaultHash = (defaultValue != null) ? defaultValue.GetHashCode() : 0;
        int positiveHash = (PositiveValue != null) ? PositiveValue.GetHashCode() : 0;
        int negativeHash = (NegativeValue != null) ? NegativeValue.GetHashCode() : 0;
        if (defaultHash == positiveHash)
        {
            replaceString = PositiveChoice.Text;
        }
        else if (defaultHash == negativeHash)
        {
            replaceString = NegativeChoice.Text;
        }
        NotSetChoice.Text = NotSetChoice.Text.Replace(DEFAULT_KEY, replaceString);
    }


    /// <summary>
    /// Set specified ForumInfo object property according to specified value
    /// </summary>
    /// <param name="infoObj">Object to update with new value</param>
    /// <param name="propertyName">Property to be set</param>
    public void SetThreeStateValue(BaseInfo infoObj, string propertyName)
    {
        int value = ValidationHelper.GetInteger(Value, -1);
        if (value == -1)
        {
            infoObj.SetValue(propertyName, null);
        }
        else
        {
            infoObj.SetValue(propertyName, ValidationHelper.GetBoolean(value, false));
        }
    }


    /// <summary>
    /// Get ForumInfo object value to initialize ThreeStateControl object
    /// </summary>
    /// <param name="infoObj">Object used to initialize ThreeStateControl</param>
    /// <param name="propertyName">Object property used for initialization</param>
    /// <returns>Integer value used to initialize ThreeStateControl </returns>
    public void InitFromThreeStateValue(BaseInfo infoObj, string propertyName)
    {
        object dbValue = infoObj.GetValue(propertyName);
        Value = (dbValue == null) ? -1 : ValidationHelper.GetBoolean(dbValue, true) ? 1 : 0;
    }

    #endregion
}