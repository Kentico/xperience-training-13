using System;

using CMS.Core;
using CMS.Newsletters;
using CMS.Newsletters.Web.UI;
using CMS.UIControls;


/// <summary>
/// Displays a table of clicked links.
/// </summary>
[UIElement(ModuleName.NEWSLETTER, "Newsletter.Issue.Reports.Clicks")]
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_TrackedLinks : CMSNewsletterDialog
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        PageTitle.TitleText = GetString("newsletter.issue.clicks");
        
        issueLinks.IssueID = Issue.IssueID;
        issueLinks.NoDataText = GetString("newsletter.issue.noclicks");

        if (Issue.IssueSentEmails <= 0)
        {
            ShowInformation(GetString("newsletter.issue.overviewnotsentyet"));

            pnlContent.Visible = false;
        }
    }
}