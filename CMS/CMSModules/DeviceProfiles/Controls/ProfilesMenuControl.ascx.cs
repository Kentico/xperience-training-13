using System;

using CMS.Base;
using CMS.Helpers;

using System.Text;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.DeviceProfiles;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;
using SubMenuItem = CMS.UIControls.UniMenuConfig.SubItem;

public partial class CMSModules_DeviceProfiles_Controls_ProfilesMenuControl : CMSUserControl
{
    #region "Variables"

    private DeviceProfileInfo currentDevice;
    private string mResourceCulture;
    private bool isRotated =  false;
    private string activeButtonCssClass = "active";
    private const string LAYOUT_HORIZONTAL = "h";
    private const string LAYOUT_VERTICAL = "v";


    #endregion


    #region "Properties"

    /// <summary>
    /// Resource strings culture
    /// </summary>
    protected string ResourceCulture
    {
        get
        {
            if (string.IsNullOrEmpty(mResourceCulture))
            {
                mResourceCulture = HTMLHelper.HTMLEncode(IsLiveSite ? LocalizationContext.PreferredCultureCode : MembershipContext.AuthenticatedUser.PreferredUICultureCode);
            }

            return mResourceCulture;
        }
        set
        {
            mResourceCulture = value;
        }
    }


    /// <summary>
    /// If true, small buttons are used for menu.
    /// </summary>
    public bool UseSmallButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseSmallButton"), false);
        }
        set
        {
            SetValue("UseSmallButton", value);
        }
    }


    /// <summary>
    /// CSS class for big button design
    /// </summary>
    public string BigButtonCssClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("BigButtonCssClass"), "BigButton");
        }
        set
        {
            SetValue("BigButtonCssClass", value);
        }
    }


    /// <summary>
    /// Gets, sets device.
    /// </summary>
    public String SelectedDevice
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedDevice"), null);
        }
        set
        {
            SetValue("SelectedDevice", value);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the device rotation buttons should be displayed.
    /// </summary>
    public bool DisplayRotateButtons
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayRotateButtons"), false);
        }
        set
        {
            SetValue("DisplayRotateButtons", value);
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        currentDevice = (SelectedDevice == null) ? DeviceProfileInfoProvider.GetCurrentDeviceProfileInfo(SiteContext.CurrentSiteName, true) : DeviceProfileInfoProvider.GetDeviceProfileInfo(SelectedDevice);
        LoadDevicesMenu();

        if (UseSmallButton
            && (currentDevice != null)
            && ((PortalContext.ViewMode == ViewModeEnum.Preview) || DisplayRotateButtons)
            )
        {
            bool isHorizontalDevice = (currentDevice.ProfilePreviewWidth > currentDevice.ProfilePreviewHeight);
            bool isHorizontalOrientation = isHorizontalDevice;

            // Define the rotation buttons
            CMSButtonGroupAction hButton = new CMSButtonGroupAction { UseIconButton = true, Name = LAYOUT_HORIZONTAL, OnClientClick = "return false;", IconCssClass = "icon-rectangle-o-h", ToolTip = GetString("device.landscape") };
            CMSButtonGroupAction vButton = new CMSButtonGroupAction { UseIconButton = true, Name = LAYOUT_VERTICAL, OnClientClick = "return false;", IconCssClass = "icon-rectangle-o-v", ToolTip = GetString("device.portrait") };

            // Append the rotation buttons 
            rotationButtons.Actions.Add(hButton);
            rotationButtons.Actions.Add(vButton);
            rotationButtons.Visible = true;

            // Get the current device rotation state
            isRotated = ValidationHelper.GetBoolean(CookieHelper.GetValue(CookieName.CurrentDeviceProfileRotate), false);
            if (isRotated)
            {
                isHorizontalOrientation = !isHorizontalOrientation;
            }

            // Highlight the currently selected rotation button
            rotationButtons.SelectedActionName = (isHorizontalOrientation) ? LAYOUT_HORIZONTAL : LAYOUT_VERTICAL;

            StringBuilder sb = new StringBuilder();
            sb.Append(@"
var CMSDeviceProfile = {
    Rotated: ", isRotated.ToString().ToLowerInvariant(), @",
    Initialized: false,
    OnRotationFunction: null,
    PreviewUrl: null,

    Init: function () {
        if (!this.Initialized) {
            this.Initialized = true;
            var rotateBtns = $cmsj('.device-rotation').find('button');
            var activeClass = '", activeButtonCssClass, @"';
            rotateBtns.bind('click', function () {
                var jThis = $cmsj(this);
                var selected = jThis.hasClass(activeClass);
                if (!selected) {
                    rotateBtns.removeClass(activeClass);
                    jThis.addClass(activeClass);
                    $cmsj.cookie('", CookieName.CurrentDeviceProfileRotate, @"', !CMSDeviceProfile.Rotated, { path: '/' } );

                    if (CMSDeviceProfile.OnRotationFunction != null) {
                        CMSDeviceProfile.OnRotationFunction(this, !CMSDeviceProfile.Rotated);
                    }
                }
            });
        }
    }
}

$cmsj(document).ready(function () {
    CMSDeviceProfile.Init();
});");

            ScriptHelper.RegisterClientScriptBlock(this, typeof(String), "deviceProfileScript", sb.ToString(), true);
        }
    }


    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        DeviceProfileInfo oldDevice = (currentDevice != null ? currentDevice.Clone() : null);
        currentDevice = DeviceProfileInfoProvider.GetCurrentDeviceProfileInfo(SiteContext.CurrentSiteName, true);

        // Reload device menu if profile changed by postback. Apply this, only if no device was selected from outside environment via 'SelectedDevice' property.
        if ((SelectedDevice == null) && DeviceChanged(oldDevice, currentDevice))
        {
            buttons.Buttons.Clear();
            LoadDevicesMenu();
            buttons.ReloadButtons();
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Returns true if device changed.
    /// </summary>
    /// <param name="oldDevice">Old device profile info</param>
    /// <param name="newDevice">Current device profile info</param>
    private bool DeviceChanged(DeviceProfileInfo oldDevice, DeviceProfileInfo newDevice)
    {
        if (oldDevice != null)
        {
            if (newDevice != null)
            {
                return oldDevice.ProfileName != newDevice.ProfileName;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (newDevice != null)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Loads device profile menu.
    /// </summary>
    private void LoadDevicesMenu()
    {
        if (UseSmallButton)
        {
            plcSmallButton.Visible = true;
        }
        else
        {
            plcBigButton.Visible = true;
        }

        string defaultString = HTMLHelper.HTMLEncode(GetString("deviceselector.default", ResourceCulture));
        MenuItem devMenuItem = null;

        if (!UseSmallButton)
        {
            devMenuItem = new MenuItem
            {
                Text = defaultString,
                Tooltip = defaultString,
            };

            // Display icon in On-site editing
            if ((Page is PortalPage) || (Page is TemplatePage))
            {
                devMenuItem.IconClass = "icon-monitor-smartphone";
            }

            SetStyles(devMenuItem);
            buttons.Buttons.Add(devMenuItem);
        }

        var enabledProfiles = CacheHelper.Cache(cs => 
        {
            // Load enabled profiles
            var result = DeviceProfileInfoProvider.GetDeviceProfiles()
                .WhereTrue("ProfileEnabled")
                .OrderBy("ProfileOrder")
                .TypedResult;

            cs.CacheDependency = CacheHelper.GetCacheDependency(DeviceProfileInfo.OBJECT_TYPE + "|all");

            return result;
        }, new CacheSettings(10, "DeviceProfileSelector", "EnabledProfiles"));

        if (!DataHelper.DataSourceIsEmpty(enabledProfiles))
        {
            // Create default item
            SubMenuItem defaultMenuItem = new SubMenuItem
                                              {
                                                  Text = defaultString,
                                                  Tooltip = defaultString,
                                                  OnClientClick = "ChangeDevice('');",
                                              };

            if (UseSmallButton)
            {
                // Insert the current device profile to the button
                btnProfilesSelector.Actions.Add(new CMSButtonAction()
                {
                    OnClientClick = String.Format("ChangeDevice({0}); return false;", ScriptHelper.GetString(defaultString)),
                    Text = defaultString
                });
            }
            else
            {
                // Insert the current device profile to the context menu
                devMenuItem.SubItems.Add(defaultMenuItem);
            }

            // Load the profiles list
            foreach (DeviceProfileInfo profileInfo in enabledProfiles.Items)
            {
                string profileName = GetString(profileInfo.ProfileDisplayName, ResourceCulture);
                CMSButtonAction deviceButton = null;

                if (UseSmallButton)
                {
                    deviceButton = new CMSButtonAction()
                    {
                        OnClientClick = String.Format("ChangeDevice({0}); return false;", ScriptHelper.GetString(profileInfo.ProfileName)),
                        Text = profileName,
                        Name = profileName
                    };

                    btnProfilesSelector.Actions.Add(deviceButton);
                }
                else
                {
                    SubMenuItem menuItem = new SubMenuItem
                                               {
                                                   Text = HTMLHelper.HTMLEncode(profileName),
                                                   Tooltip = HTMLHelper.HTMLEncode(profileName),
                                                   OnClientClick = String.Format("ChangeDevice({0});", ScriptHelper.GetString(profileInfo.ProfileName))
                                               };
                    devMenuItem.SubItems.Add(menuItem);
                }

                // Update main button if current device profile is equal currently processed profile
                if ((currentDevice != null) && (currentDevice.ProfileName.CompareToCSafe(profileInfo.ProfileName, true) == 0))
                {
                    if (UseSmallButton)
                    {
                        btnProfilesSelector.SelectedActionName = profileName;
                    }
                    else
                    {
                        devMenuItem.Text = HTMLHelper.HTMLEncode(profileName);
                        devMenuItem.Tooltip = HTMLHelper.HTMLEncode(profileName);
                    }
                }

            }
        }
    }


    /// <summary>
    /// Applies styles to menu item according to UseSmallButton property.
    /// </summary>
    /// <param name="menuItem">Menu item to apply styles to.</param>
    private void SetStyles(MenuItem menuItem)
    {
        menuItem.CssClass = BigButtonCssClass;
        menuItem.ImageAlign = ImageAlign.Top;
        menuItem.MinimalWidth = 48;
    }

    #endregion
}



