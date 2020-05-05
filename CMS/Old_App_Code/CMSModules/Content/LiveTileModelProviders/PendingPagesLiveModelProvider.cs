using System;

using CMS.ApplicationDashboard.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.WorkflowEngine;

[assembly: RegisterLiveTileModelProvider(ModuleName.CONTENT, "Pending", typeof(PendingPagesLiveModelProvider))]

namespace CMS.DocumentEngine
{
    /// <summary>
    /// Provides live tile model for the pending pages dashboard tile.
    /// </summary>
    internal class PendingPagesLiveModelProvider : ILiveTileModelProvider
    {
        /// <summary>
        /// Loads total number of pages waiting for the approval.
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
                int waitingPagesCount = GetNumberOfPendingPages(liveTileContext.SiteInfo, (UserInfo)liveTileContext.UserInfo);
                if (waitingPagesCount == 0)
                {
                    return null;
                }

                return new LiveTileModel
                {
                    Value = waitingPagesCount,
                    Description = ResHelper.GetString("pendingpages.livetiledescription")
                };
            }, new CacheSettings(2, "PendingPagesLiveModelProvider", liveTileContext.SiteInfo.SiteID, liveTileContext.UserInfo.UserID));
        }


        /// <summary>
        /// Gets number of total pages waiting for the approval.
        /// </summary>
        /// <param name="siteInfo">Site the pages belongs to</param>
        /// <param name="userInfo">The user providing the approval</param>
        /// <returns>Total number of waiting pages</returns>
        private int GetNumberOfPendingPages(SiteInfo siteInfo, UserInfo userInfo)
        {
            int siteId = siteInfo.SiteID;

            // Get correct pending steps which may current user manage
            var steps = new IDQuery<WorkflowStepInfo>().Where(WorkflowStepInfoProvider.GetWorkflowPendingStepsWhereCondition(userInfo, siteId));

            var docs = new IDQuery<TreeNode>()
                    .OnSite(siteId)
                    .WhereIn("DocumentWorkflowStepID", steps);

            return docs.Count;
        }
    }
}