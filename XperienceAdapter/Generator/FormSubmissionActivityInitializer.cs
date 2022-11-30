using CMS.Activities;
using CMS.Base;
using CMS.Base.Internal;
using CMS.ContactManagement;
using CMS.Core;
using CMS.Membership;
using CMS.OnlineForms;

using System;
using System.Collections.Generic;
using System.Text;

namespace XperienceAdapter.Generator
{
    public class FormSubmissionActivityInitializer : IActivityInitializer
    {
        private readonly ActivityTitleBuilder _activityTitleBuilder = new ActivityTitleBuilder();

        private readonly ContactInfo _contactInfo;

        private readonly ITreeNode _treeNode;

        private readonly BizFormItem _bizFormItem;

        public string ActivityType => PredefinedActivityType.BIZFORM_SUBMIT;

        public string SettingsKeyName => "CMSCMBizFormSubmission";

        public FormSubmissionActivityInitializer(ContactInfo contactInfo, ITreeNode treeNode, BizFormItem formItem)
        {
            _contactInfo = contactInfo ?? throw new ArgumentNullException(nameof(contactInfo));
            _treeNode = treeNode ?? throw new ArgumentNullException(nameof(treeNode));
            _bizFormItem = formItem ?? throw new ArgumentNullException(nameof(formItem));
        }

        public void Initialize(IActivityInfo activity)
        {
            if (activity is null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            var form = _bizFormItem.BizFormInfo;
            activity.ActivityItemID = form.FormID;
            activity.ActivityItemDetailID = _bizFormItem.ItemID;
            activity.ActivityTitle = _activityTitleBuilder.CreateTitle(ActivityType, form.FormDisplayName);
            activity.ActivityNodeID = _treeNode.NodeID;
            activity.ActivityCulture = _treeNode.DocumentCulture;
            var httpContext = Service.ResolveOptional<IHttpContextRetriever>()?.GetContext();
            var referrerUrl = httpContext?.Request?.UrlReferrer?.AbsoluteUri;
            activity.ActivityURL = referrerUrl ?? string.Empty;
            activity.ActivityContactID = _contactInfo.ContactID;
        }
    }
}
