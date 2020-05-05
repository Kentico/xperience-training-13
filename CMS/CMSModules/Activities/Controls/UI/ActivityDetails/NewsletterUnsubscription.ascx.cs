using System;

using CMS.Activities;
using CMS.Activities.Web.UI;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_NewsletterUnsubscription : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        return uc.LoadData(ai);
    }

    #endregion
}