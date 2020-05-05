using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Attachments_DocumentAttachments_Transformations_Attachment : CMSAbstractTransformation
{
    /// <summary>
    /// Displayed attachment
    /// </summary>
    public AttachmentWithVariants Attachment
    {
        get
        {
            return (AttachmentWithVariants)DataItem;
        }
    }
    

    protected string GetAttachmentHtml(DocumentAttachment attachment, DocumentAttachment mainAttachment, int versionHistoryId)
    {
        if (mainAttachment == null)
        {
            mainAttachment = attachment;
        }

        var attachmentUrl = GetAttachmentUrl(attachment, mainAttachment, versionHistoryId);
        var displayedName = GetDisplayedName(attachment);
        var tooltip = GetTooltip(attachment, attachmentUrl, displayedName);
        var fileIcon = UIHelper.GetFileIcon(Page, attachment.AttachmentExtension);

        if (ImageHelper.IsImage(mainAttachment.AttachmentExtension))
        {
            return String.Format("<a class=\"cms-icon-link\" href=\"#\" onclick=\"javascript: window.open('{0}'); return false;\"><span id=\"{1}\" {2}>{3}{4}</span></a>", attachmentUrl, mainAttachment.AttachmentGUID, tooltip, fileIcon, displayedName);
        }

        attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "disposition", "attachment");

        // NOTE: OnClick here is needed to avoid loader to show because even for download links, the pageUnload event is fired
        return String.Format("<a class=\"cms-icon-link\" onclick=\"javascript: {5}\" href=\"{0}\"><span id=\"{1}\" {2}>{3}{4}</span></a>", attachmentUrl, mainAttachment.AttachmentGUID, tooltip, fileIcon, displayedName, ScriptHelper.GetDisableProgressScript());
    }


    private string GetAttachmentUrl(DocumentAttachment attachment, DocumentAttachment mainAttachment, int versionHistoryId)
    {
        // Get link for attachment
        string attachmentUrl;

        var attName = mainAttachment.AttachmentName;
        var documentId = mainAttachment.AttachmentDocumentID;
        var siteName = SiteContext.CurrentSiteName;

        if (IsLiveSite && (documentId > 0))
        {
            attachmentUrl =
                ApplicationUrlHelper.ResolveUIUrl(
                    AttachmentURLProvider.GetAttachmentUrl(
                        mainAttachment.AttachmentGUID, 
                        URLHelper.GetSafeFileName(attName, siteName)
                    )
                );
        }
        else
        {
            attachmentUrl = 
                ApplicationUrlHelper.ResolveUIUrl(
                    AttachmentURLProvider.GetAttachmentUrl(
                        mainAttachment.AttachmentGUID, 
                        URLHelper.GetSafeFileName(attName, siteName), 
                        null, 
                        // Do not include version history ID for temporary attachment
                        // Version history ID may be present in case of new culture version where may be mix of temporary and version attachments from source
                        (mainAttachment.AttachmentFormGUID == Guid.Empty) ? versionHistoryId : 0
                    )
                );
        }

        // Add variant identifier
        if (attachment.IsVariant())
        {
            attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "variant", attachment.AttachmentVariantDefinitionIdentifier);
        }

        // Ensure correct URL for non-temporary attachments, they need to be served from their original site (may be from linked documents)
        if (mainAttachment.AttachmentFormGUID == Guid.Empty)
        {
            var attachmentSiteName = SiteInfoProvider.GetSiteName(mainAttachment.AttachmentSiteID);
            if (attachmentSiteName != siteName)
            {
                attachmentUrl = URLHelper.AddParameterToUrl(attachmentUrl, "sitename", attachmentSiteName);
            }
        }

        attachmentUrl = URLHelper.UpdateParameterInUrl(attachmentUrl, "chset", Guid.NewGuid().ToString());

        // Add latest version requirement for live site
        if (IsLiveSite && (documentId > 0))
        {
            // Add requirement for latest version of files for current document
            string newparams = "latestfordocid=" + documentId;
            newparams += "&hash=" + ValidationHelper.GetHashString("d" + documentId, new HashSettings(""));

            attachmentUrl += "&" + newparams;
        }

        return attachmentUrl;
    }


    private static string GetDisplayedName(DocumentAttachment attachment)
    {
        var displayedName = TextHelper.LimitLength(attachment.AttachmentName, AttachmentsControl.ATTACHMENT_NAME_LIMIT);
        if (!String.IsNullOrEmpty(attachment.AttachmentVariantDefinitionIdentifier))
        {
            displayedName = ResHelper.GetAPIString("AttachmentVariant." + attachment.AttachmentVariantDefinitionIdentifier, attachment.AttachmentVariantDefinitionIdentifier);
        }

        return displayedName;
    }


    private static string GetTooltip(DocumentAttachment attachment, string attachmentUrl, string name)
    {
        string title = null;
        string description = null;

        var imageWidth = attachment.AttachmentImageWidth;
        var imageHeight = attachment.AttachmentImageHeight;

        if (String.IsNullOrEmpty(attachment.AttachmentVariantDefinitionIdentifier))
        {
            title = attachment.AttachmentTitle;
            description = attachment.AttachmentDescription;
        }

        var tooltip = UIHelper.GetTooltipAttributes(attachmentUrl, imageWidth, imageHeight, title, name, attachment.AttachmentExtension, description, null, 300);

        return tooltip;
    }
}