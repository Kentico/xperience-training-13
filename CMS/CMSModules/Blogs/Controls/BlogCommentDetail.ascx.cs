using System;
using System.Collections.Generic;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Blogs.Web.UI;
using CMS.DataEngine;
using CMS.Globalization.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;


public partial class CMSModules_Blogs_Controls_BlogCommentDetail : BlogCommentDetail, IPostBackEventHandler
{
    private CMSAdminControls_UI_UserPicture userPict;
    private CMSModules_AbuseReport_Controls_InlineAbuseReport ucInlineAbuseReport;


    /// <summary>
    /// Comment ID.
    /// </summary>
    public int CommentID
    {
        get
        {
            return Comment != null ? Comment.CommentID : 0;
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        // Load controls dynamically
        userPict = (CMSAdminControls_UI_UserPicture)LoadControl("~/CMSAdminControls/UI/UserPicture.ascx");
        plcUserPicture.Controls.Add(userPict);

        ucInlineAbuseReport = (CMSModules_AbuseReport_Controls_InlineAbuseReport)LoadControl("~/CMSModules/AbuseReport/Controls/InlineAbuseReport.ascx");
        ucInlineAbuseReport.ReportObjectType = "blog.comment";
        plcInlineAbuseReport.Controls.Add(ucInlineAbuseReport);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Controls initialization
        lnkApprove.Text = GetString("general.approve");
        lnkReject.Text = GetString("general.reject");
        lnkEdit.Text = GetString("general.edit");
        lnkDelete.Text = GetString("general.delete");

        lnkEdit.Visible = ShowEditButton;
        lnkDelete.Visible = ShowDeleteButton;
        lnkApprove.Visible = ShowApproveButton;
        lnkReject.Visible = ShowRejectButton;

        LoadData();

        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteCommentConfirmation", ScriptHelper.GetScript("function ConfirmDelete(){ return confirm(" + ScriptHelper.GetString(GetString("BlogCommentDetail.DeleteConfirmation")) + ");}"));
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public void LoadData()
    {
        if (Comment == null)
        {
            return;
        }

        SetUserPicture();

        if (!String.IsNullOrEmpty(Comment.CommentUrl))
        {
            lnkName.Text = HTMLHelper.HTMLEncode(Comment.CommentUserName);
            lnkName.NavigateUrl = Comment.CommentUrl;

            AddRelAttribute();

            lblName.Visible = false;
        }
        else
        {
            lblName.Text = HTMLHelper.HTMLEncode(Comment.CommentUserName);
            lnkName.Visible = false;
        }

        lblText.Text = HTMLHelper.HTMLEncodeLineBreaks(Comment.CommentText);
        lblDate.Text = TimeZoneUIMethods.ConvertDateTime(Comment.CommentDate, this).ToString();

        string url = "~/CMSModules/Blogs/Controls/Comment_Edit.aspx";
        if (IsLiveSite)
        {
            url = "~/CMSModules/Blogs/CMSPages/Comment_Edit.aspx";
        }

        lnkEdit.OnClientClick = String.Format("EditComment('{0}?commentID={1}'); return false;", ResolveUrl(url), CommentID);
        lnkDelete.OnClientClick = String.Format("if(ConfirmDelete()) {{ {0}; }} return false;", GetPostBackEventReference("delete"));
        lnkApprove.OnClientClick = String.Format("{0}; return false;", GetPostBackEventReference("approve"));
        lnkReject.OnClientClick = String.Format("{0}; return false;", GetPostBackEventReference("reject"));

        // Initialize report abuse
        ucInlineAbuseReport.ReportTitle = ResHelper.GetString("BlogCommentDetail.AbuseReport", CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName)) + Comment.CommentText;
        ucInlineAbuseReport.ReportObjectID = CommentID;
        ucInlineAbuseReport.CMSPanel.Roles = AbuseReportRoles;
        ucInlineAbuseReport.CMSPanel.SecurityAccess = AbuseReportSecurityAccess;
        ucInlineAbuseReport.CMSPanel.OwnerID = AbuseReportOwnerID;
    }


    private void SetUserPicture()
    {
        if (BlogProperties.EnableUserPictures)
        {
            userPict.UserID = Comment.CommentUserID;
            userPict.Width = BlogProperties.UserPictureMaxWidth;
            userPict.Height = BlogProperties.UserPictureMaxHeight;
            userPict.Visible = true;
            userPict.RenderOuterDiv = true;
            userPict.OuterDivCSSClass = "CommentUserPicture";
        }
        else
        {
            userPict.Visible = false;
        }
    }


    public void RaisePostBackEvent(string eventArgument)
    {
        var parts = eventArgument.Split(';');
        FireOnCommentAction(parts[0], parts[1]);
    }


    private string GetPostBackEventReference(string actionName)
    {
        return ControlsHelper.GetPostBackEventReference(this, String.Format("{0};{1}", actionName, CommentID));
    }

    private void AddRelAttribute()
    {
        var values = new List<string>();

        // Prevent target _blank vulnerability phishing attack
        values.Add("noopener");
        values.Add("noreferrer");

        lnkName.Attributes.Add("rel", String.Join(" ", values));
    }
}