using System;

using CMS.ApplicationDashboard.Web.UI;
using CMS.Core;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.SiteProvider;

[assembly: RegisterLiveTileModelProvider(ModuleName.CMS, "Administration.EmailQueue", typeof(EmailsLiveTileModelProvider))]

namespace CMS.EmailEngine
{
    /// <summary>
    /// Provides live model for the E-mail Queue dashboard tile.
    /// </summary>
    internal class EmailsLiveTileModelProvider : ILiveTileModelProvider
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
                var emailsNotSentCount = GetEmailsNotSentCount(liveTileContext.SiteInfo);

                if (emailsNotSentCount != 0)
                {
                    return new LiveTileModel
                    {
                        Value = emailsNotSentCount,
                        Description = ResHelper.GetString("emails.livetiledescription"),
                    };
                }
                else return null;

            }, new CacheSettings(1, "EmailsLiveTileModelProvider", liveTileContext.SiteInfo.SiteID));
        }


        /// <summary>
        /// Gets number of emails which couldn't be sent.
        /// </summary>
        /// <param name="site">Tile's site</param>
        /// <returns>Number of e-mails which couldn't be sent </returns>
        private static int GetEmailsNotSentCount(SiteInfo site)
        {
            return EmailInfoProvider.GetEmailCount("EmailSiteID = " + site.SiteID + " AND EmailStatus = " + (int)(EmailStatusEnum.Waiting) + " AND EmailLastSendAttempt IS NOT NULL");
        }
    }
}