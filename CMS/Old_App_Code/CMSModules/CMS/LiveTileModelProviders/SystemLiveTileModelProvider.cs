using System;

using CMS.ApplicationDashboard.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;

[assembly: RegisterLiveTileModelProvider(ModuleName.CMS, "Administration.System", typeof(SystemLiveTileModelProvider))]

namespace CMS.SiteProvider
{
    /// <summary>
    /// Provides live model for the System dashboard tile.
    /// </summary>
    internal class SystemLiveTileModelProvider : ILiveTileModelProvider
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
                string description;
                var timeSinceRestart = GetTimeDifferenceSinceRestart(out description);

                return new LiveTileModel
                {
                    Value = timeSinceRestart,
                    Description = description,
                };
            }, new CacheSettings(1, "SystemLiveTileModelProvider", liveTileContext.SiteInfo.SiteID));
        }

        /// <summary>
        /// Get time difference since last restart.
        /// </summary>
        /// <param name="description">Step icon description</param>
        /// <returns>Time difference since last restart and description in out parameter</returns>
        private static int GetTimeDifferenceSinceRestart(out string description)
        {
            int timeDifference = 0;

            // Count time difference since last restart
            TimeSpan timeSpan = DateTime.Now - CMSApplication.ApplicationStart;

            // Check whether to display days, hours or minutes
            if (timeSpan.Days > 0)
            {
                timeDifference = timeSpan.Days;
                description = String.Format(ResHelper.GetString("system.livetiledescription"), "Days");
            }
            else if (timeSpan.Hours > 0)
            {
                timeDifference = timeSpan.Hours;
                description = String.Format(ResHelper.GetString("system.livetiledescription"), "Hours");
            }
            else
            {
                // Ensure counting starts on 1 - rounding up
                timeDifference = timeSpan.Minutes + 1;
                description = String.Format(ResHelper.GetString("system.livetiledescription"), "Minutes");
            }

            return timeDifference;
        }
    }
}