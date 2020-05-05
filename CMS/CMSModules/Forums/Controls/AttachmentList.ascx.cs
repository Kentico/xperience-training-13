using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_AttachmentList : ForumViewer
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DeleteConfirmation", ScriptHelper.GetScript(
            "function DeleteConfirm() { return confirm(" + ScriptHelper.GetString(GetString("forums.attachment.deleteconfirmation")) + "); } "));

        if ((ForumContext.CurrentPost != null) && (ForumContext.CurrentPost.PostId > 0))
        {
            btnUpload.Text = GetString("general.upload");
            btnBack.Text = GetString("general.back");

            if (ForumContext.CurrentForum != null)
            {
                if (ForumContext.CurrentForum.ForumAttachmentMaxFileSize > 0)
                {
                    ShowInformation(GetString("ForumAttachment.MaxFileSizeInfo").Replace("##SIZE##", ForumContext.CurrentForum.ForumAttachmentMaxFileSize.ToString()));
                }
            }

            if (ControlsHelper.IsInUpdatePanel(this))
            {
                ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnUpload);
            }

            if (!RequestHelper.IsPostBack())
            {
                // Get post attachments
                DataSet attachments = ForumAttachmentInfoProvider.GetForumAttachments(ForumContext.CurrentPost.PostId, false);
                if (!DataHelper.DataSourceIsEmpty(attachments))
                {
                    listAttachment.DataSource = attachments;
                    listAttachment.DataBind();
                }
                else
                {
                    plcListHeader.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Handles delete button action - deletes user favorite.
    /// </summary>
    protected void btnDelete_OnCommand(object sender, CommandEventArgs e)
    {
        // Check permissions
        if (!IsAvailable(ForumContext.CurrentForum, ForumActionType.Attachment))
        {
            ShowError(GetString("ForumNewPost.PermissionDenied"));
            return;
        }

        if (e.CommandName == "delete")
        {
            int attachmentId = ValidationHelper.GetInteger(e.CommandArgument, 0);

            // Get forum attachment info
            ForumAttachmentInfo fai = ForumAttachmentInfoProvider.GetForumAttachmentInfo(attachmentId);
            if (fai != null)
            {
                // Delete attachment
                ForumAttachmentInfoProvider.DeleteForumAttachmentInfo(fai);
            }

            //Reload page
            URLHelper.Redirect(RequestContext.CurrentURL);
        }
    }


    /// <summary>
    /// Handles file upload.
    /// </summary>
    protected void btnUpload_OnClick(object sender, EventArgs e)
    {
        if (ForumContext.CurrentForum == null)
        {
            return;
        }

        // Check permissions
        if (!IsAvailable(ForumContext.CurrentForum, ForumActionType.Attachment))
        {
            ShowError(GetString("ForumNewPost.PermissionDenied"));
            return;
        }

        if (fileUpload.HasFile)
        {
            // Check max attachment size
            if ((ForumContext.CurrentForum.ForumAttachmentMaxFileSize > 0) && ((fileUpload.PostedFile.InputStream.Length / 1024) >= ForumContext.CurrentForum.ForumAttachmentMaxFileSize))
            {
                ShowError(GetString("ForumAttachment.AttachmentIsTooLarge"));
                return;
            }

            // Check attachment extension
            if (!ForumAttachmentInfoProvider.IsExtensionAllowed(fileUpload.FileName, SiteName))
            {
                ShowError(GetString("ForumAttachment.AttachmentIsNotAllowed"));
                return;
            }

            ForumAttachmentInfo attachmentInfo = new ForumAttachmentInfo(fileUpload.PostedFile, 0, 0, ForumContext.CurrentForum.ForumImageMaxSideSize);
            attachmentInfo.AttachmentPostID = ForumContext.CurrentPost.PostId;
            ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(ForumContext.CurrentForum.ForumGroupID);
            if (fgi != null)
            {
                attachmentInfo.AttachmentSiteID = fgi.GroupSiteID;

                //Save to DB
                ForumAttachmentInfoProvider.SetForumAttachmentInfo(attachmentInfo);
                DataSet ds = ForumAttachmentInfoProvider.GetForumAttachments(ForumContext.CurrentPost.PostId, false);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    listAttachment.DataSource = ds;
                    listAttachment.DataBind();
                    plcListHeader.Visible = true;
                }
            }
        }
    }


    /// <summary>
    /// Returns the url of attachment file.
    /// </summary>
    /// <param name="attachmeentGuid">Guid of attachment</param>    
    protected string GetAttachmentUrl(object attachmeentGuid)
    {
        Guid guid = ValidationHelper.GetGuid(attachmeentGuid, Guid.Empty);

        // Guid is ok
        if (guid != Guid.Empty)
        {
            // Return attachment url
            return ResolveUrl("~/CMSPages/GetForumAttachment.aspx?fileguid=" + guid);
        }
        else
        {
            return "#";
        }
    }


    /// <summary>
    /// Handles Back button click.
    /// </summary>
    protected void btnBack_OnClick(object sender, EventArgs e)
    {
        URLHelper.Redirect(ClearURL());
    }
}
