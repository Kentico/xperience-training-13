using System;
using System.Data;

using CMS.Forums;
using CMS.Forums.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Forums_Controls_AttachmentDisplayer : ForumViewer
{
    private int mPostID;
    private int mPostAttachmentCount;
    protected string mAttachmentSeparator = "&emsp;";


    #region "Public variables"

    /// <summary>
    /// Gets or sets ID of forum posts.
    /// </summary>
    public int PostID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["AttachmentDisplayer_PostID"], mPostID);
        }
        set
        {
            if (value > 0)
            {
                ViewState["AttachmentDisplayer_PostID"] = value;
                mPostID = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets attachments count.
    /// </summary>
    public int PostAttachmentCount
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["AttachmentDisplayer_PostAttachmentCount"], mPostAttachmentCount);
        }
        set
        {
            if (value > 0)
            {
                ViewState["AttachmentDisplayer_PostAttachmentCount"] = value;
                mPostAttachmentCount = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the attachment separator.
    /// </summary>
    public string AttachmentSeparator
    {
        get
        {
            return mAttachmentSeparator;
        }
        set
        {
            mAttachmentSeparator = value;
        }
    }

    #endregion


    /// <summary>
    /// OnPreRender reload data.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Returns HTML code with link to attachment.
    /// </summary>
    /// <param name="data">DataRowView with attachment info data.</param>
    protected string GetAttachmentLink(object data)
    {
        // try retype data as datarow view
        DataRowView drv = data as DataRowView;

        if (drv != null)
        {
            string guid = ValidationHelper.GetString(drv.Row["AttachmentGUID"], "");
            string name = HTMLHelper.HTMLEncode(ValidationHelper.GetString(drv.Row["AttachmentFileName"], ""));
            string mime = ValidationHelper.GetString(drv.Row["AttachmentMimeType"], "");
            string url = ResolveUrl("~/CMSPages/GetForumAttachment.aspx?fileguid=" + guid);

            if (DisplayAttachmentImage && ImageHelper.IsMimeImage(mime))
            {
                return "<a target=\"_blank\" title=\"" + name + "\" href=\"" + url + "\">" +
                       "<img src=\"" + url + "&maxsidesize=" + AttachmentImageMaxSideSize + "\" border=\"none\" alt=\"" + name + "\" /></a>";
            }
            else
            {
                return "<a target=\"_blank\" title=\"" + name + "\" href=\"" + url + "\">" + name + "</a>";
            }
        }

        return "";
    }


    public override void ReloadData()
    {
        // Try to copy forum viewer properties from parent
        CopyValuesFromParent(this);

        // Retrieve from DB only if post has some attachments
        if ((PostAttachmentCount > 0) && ((PostID > 0)))
        {
            ltlHeader.Text = GetString("forums.postattachments");

            DataSet ds = ForumAttachmentInfoProvider.GetForumAttachments(PostID, false);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                rptAttachments.DataSource = ds;
                rptAttachments.DataBind();
                plcPostAttachments.Visible = true;
            }
        }
        // Hide if there is no attachment
        else
        {
            plcPostAttachments.Visible = false;
        }

        base.ReloadData();
    }


    /// <summary>
    /// Clear viewestate data.
    /// </summary>
    public void ClearData()
    {
        ViewState["AttachmentDisplayer_PostID"] = null;
        ViewState["AttachmentDisplayer_PostAttachmentCount"] = null;
    }
}