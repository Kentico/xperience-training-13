using System;

using CMS.ApplicationDashboard.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;

[assembly: RegisterLiveTileModelProvider("Licenses", "Licenses", typeof(LicensesLiveTileModelProvider))]

namespace CMS.LicenseProvider
{
    /// <summary>
    /// Provides live model for the Licenses dashboard tile.
    /// </summary>
    internal class LicensesLiveTileModelProvider : ILiveTileModelProvider
    {
        /// <summary>
        /// Loads model for the dashboard live tile.
        /// </summary>
        /// <param name="liveTileContext">Context of the live tile. Contains information about the user and the site the model is requested for</param>
        /// <exception cref="ArgumentNullException"><paramref name="liveTileContext"/> is null</exception>
        /// <returns>Live tile model</returns>
        public LiveTileModel GetModel(LiveTileContext liveTileContext)
        {
            if (liveTileContext == null)
            {
                throw new ArgumentNullException("liveTileContext");
            }
            
            return CacheHelper.Cache(() =>
            {                
                var licensesRemainingDays = GetLicensesRemainingDays();
                
                if (licensesRemainingDays != 0)
                {
                    return new LiveTileModel
                    {
                        Value = licensesRemainingDays,
                        Description = ResHelper.GetString("licenses.livetiledescription"),
                    };
                }
                else return null;

            }, new CacheSettings(10, "LicensesLiveTileModelProvider", liveTileContext.SiteInfo.SiteID));
        }


        /// <summary>
        /// Gets days until the license for current domain expires.
        /// </summary>
        /// <returns>Days until the license for current domain expires</returns>
        private static int GetLicensesRemainingDays()
        {
            int remainingDays = 0;
            // Check current domain
            if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, string.Empty) != string.Empty)
            {
                // Get license key info for current domain
                LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(RequestContext.CurrentDomain);
                if ((lki != null) && (lki.ExpirationDateReal != LicenseKeyInfo.TIME_UNLIMITED_LICENSE))
                {
                    // Substract days for correct days substraction - last day is not a valid day
                    TimeSpan expiration = lki.ExpirationDateReal.Subtract(DateTime.Now.AddDays(-1));
                    if (expiration.Days <= 60)
                    {
                        remainingDays = expiration.Days;
                    }
                }
            }
            return remainingDays;
        }
    }
}