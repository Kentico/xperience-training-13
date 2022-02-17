using System;
using System.Linq;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;

using MedioClinic.Customizations.Cookies;

namespace CMSApp.CMSFormControls.MedioClinic
{
    public partial class CookieLevelSelector : FormEngineUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!StopProcessing)
            {
                SetupControl();
            }
        }

        protected void SetupControl()
        {
            drpCookieLevel.Items.Clear();
            drpCookieLevel.Items.Add(new ListItem(string.Empty, CookieManager.NullIntegerValue.ToString()));
            var cookieManager = Service.Resolve<ICookieManager>();
            var cookieLevels = cookieManager.GetCookieLevels();
            drpCookieLevel.Items.AddRange(cookieLevels.Select(level => new ListItem(level.Name ?? level.Level.ToString(), level.Level.ToString())).ToArray());
        }

        public override object Value 
        {
            get => 
                drpCookieLevel.SelectedItem != null 
                        && !drpCookieLevel.SelectedItem.Value.Equals(CookieManager.NullIntegerValue.ToString(), StringComparison.OrdinalIgnoreCase) 
                        && int.TryParse(drpCookieLevel.SelectedItem.Value, out var parsed) 
                    ? parsed 
                    : CookieManager.NullIntegerValue;
            set
            {
                ValidationHelper.GetInteger(value, CookieManager.NullIntegerValue);
                drpCookieLevel.SelectedValue = ValidationHelper.GetString(value, CookieManager.NullIntegerValue.ToString());
            }
        }
    }
}