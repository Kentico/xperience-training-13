using System;

using CMS.Core;
using CMS.FormEngine.Web.UI;

using CMS.Newsletters;
using CMS.SiteProvider;

using CMS.Helpers;

namespace CMSApp.CMSFormControls.MedioClinic
{
    public partial class NewsletterLinkSelector : FormEngineUserControl
    {
        private Guid _value = Guid.Empty;

        /// <summary>
        /// Gets or sets the enabled state of the control.
        /// </summary>
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                EnsureChildControls();
                base.Enabled = value;
                usNewsletters.Enabled = value;
                usIssues.Enabled = value;
                usLinks.Enabled = value;
            }
        }

        /// <summary>
        /// Gets value display name.
        /// </summary>
        public override string ValueDisplayName => usLinks.ValueDisplayName;

        /// <summary>
        /// Gets or sets field value.
        /// </summary>
        public override object Value
        {
            get => ValidationHelper.GetGuid(usLinks.Value, Guid.Empty);
            set => _value = ValidationHelper.GetGuid(value, Guid.Empty);
        }

        protected override void CreateChildControls()
        {
            if (usNewsletters == null)
            {
                pnlUpdate.LoadContainer();
            }

            base.CreateChildControls();
            
            if (!StopProcessing)
            {
                ReloadData();
            }
        }

        /// <summary>
        /// Reloads dropdown lists.
        /// </summary>
        protected void ReloadData()
        {
            usNewsletters.WhereCondition = "NewsletterSiteID = " + SiteContext.CurrentSiteID;
            usNewsletters.ReturnColumnName = "NewsletterID";
            usNewsletters.OnSelectionChanged += new EventHandler(NewslettersDropDownSingleSelect_SelectedIndexChanged);
            usNewsletters.DropDownSingleSelect.AutoPostBack = true;

            usIssues.WhereCondition = GetIssueWhereCondition(usNewsletters.Value);
            usIssues.ReturnColumnName = "IssueID";
            usIssues.OnSelectionChanged += new EventHandler(IssuesDropDownSingleSelect_SelectedIndexChanged);
            usIssues.DropDownSingleSelect.AutoPostBack = true;

            usLinks.WhereCondition = GetLinksWhereCondition(usIssues.Value);
            usLinks.ReturnColumnName = "LinkGUID";

            if (_value != Guid.Empty)
            {
                var link = Service.Resolve<ILinkInfoProvider>().Get(_value);
                int issueId = link?.LinkIssueID ?? 0;

                IssueInfo issue = null;
                if (issueId > 0)
                {
                    issue = Service.Resolve<IIssueInfoProvider>().Get(issueId);
                }

                int issueNewsletterID = issue?.IssueNewsletterID ?? 0;

                usNewsletters.Value = issueNewsletterID;
                usIssues.WhereCondition = GetIssueWhereCondition(issueNewsletterID); ;
                usIssues.Reload(true);

                if (issueId > 0)
                {
                    usIssues.DropDownSingleSelect.SelectedValue = issueId.ToString();
                }

                usLinks.WhereCondition = GetLinksWhereCondition(issueId); ;
                usLinks.Reload(true);
                usLinks.DropDownSingleSelect.SelectedValue = _value.ToString();
                usLinks.Value = _value;
            }
        }

        protected void NewslettersDropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load issues according to selected newsletter
            usIssues.WhereCondition = GetIssueWhereCondition(usNewsletters.Value);
            usIssues.Reload(true);
        }

        protected void IssuesDropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load links according to selected issue
            usLinks.WhereCondition = GetLinksWhereCondition(usIssues.Value);
            usLinks.Reload(true);
        }

        /// <summary>
        /// Returns a WHERE condition for issue dropdown list
        /// </summary>
        /// <param name="value">Newsletter ID</param>
        private string GetIssueWhereCondition(object value)
        {
            string where = "IssueNewsletterID=" + ValidationHelper.GetInteger(value, 0);

            return where;
        }

        /// <summary>
        /// Returns WHERE condition for links dropdown list
        /// </summary>
        /// <param name="value">Issue ID</param>
        private string GetLinksWhereCondition(object value)
        {
            string where = "LinkIssueID=" + ValidationHelper.GetInteger(value, 0);

            return where;
        }
    }
}