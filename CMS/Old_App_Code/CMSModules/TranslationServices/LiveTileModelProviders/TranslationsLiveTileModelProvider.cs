using System;

using CMS.ApplicationDashboard.Web.UI;
using CMS.Core;
using CMS.Helpers;
using CMS.TranslationServices;

[assembly: RegisterLiveTileModelProvider(ModuleName.TRANSLATIONSERVICES, "Translations", typeof(TranslationsLiveTileModelProvider))]

namespace CMS.TranslationServices
{
    /// <summary>
    /// Provides live tile model for the translations submissions ready to import.
    /// </summary>
    internal class TranslationsLiveTileModelProvider : ILiveTileModelProvider
    {
        /// <summary>
        /// Loads total number of submissions waiting for import.
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
                int waitingSubmissionsCount = GetNumberOfWaitingSubmissions(liveTileContext.SiteInfo.SiteID);
                if (waitingSubmissionsCount == 0)
                {
                    return null;
                }

                return new LiveTileModel
                {
                    Value = waitingSubmissionsCount,
                    Description = ResHelper.GetString("translations.livetiledescription")
                };
            }, new CacheSettings(2, "TranslationsLiveModelProvider", liveTileContext.SiteInfo.SiteID));
        }


        /// <summary>
        /// Gets number of submissions waiting for import.
        /// </summary>
        /// <param name="siteId">Site the submissions belongs to</param>
        /// <returns>Total number of waiting submissions</returns>
        private int GetNumberOfWaitingSubmissions(int siteId)
        {
            var submissions = TranslationSubmissionInfo.Provider.Get()
                .Column("SubmissionID")
                .OnSite(siteId)
                .WhereEquals("SubmissionStatus", TranslationStatusEnum.TranslationReady);

            return submissions.Count;
        }
    }
}