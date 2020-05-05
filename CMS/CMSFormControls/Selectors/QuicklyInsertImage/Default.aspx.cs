using System;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_QuicklyInsertImage_Default : CMSDeskPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Validate query string
        if (!QueryHelper.ValidateHash("hash", "containerid"))
        {
            fileUploaderElem.StopProcessing = true;
        }
        else
        {
            // Ensure additional styles
            CurrentMaster.HeadElements.Visible = true;
            CurrentMaster.HeadElements.Text += CssHelper.GetStyle("*{direction:ltr !important;}body{background:transparent !important;}input,input:focus,input:hover,input:active{border:none;border-color:transparent;outline:none;}");

            // Initialize uploader control properties by query string values
            fileUploaderElem.ImageWidth = 16;
            fileUploaderElem.ImageHeight = 18;
            fileUploaderElem.ImageUrl = "~/CMSAdminControls/CKeditor/plugins/CMSPlugins/images/QuicklyInsertImage.gif";
            fileUploaderElem.Text = HTMLHelper.EncodeForHtmlAttribute(QueryHelper.GetString("innerdivhtml", String.Empty));
            fileUploaderElem.InnerElementClass = QueryHelper.GetText("innerdivclass", String.Empty);
            fileUploaderElem.AdditionalStyle = QueryHelper.GetText("additionalstyle", String.Empty);
            fileUploaderElem.LoadingImageUrl = GetImageUrl("Design/Preloaders/preload16.gif");

            fileUploaderElem.FormGUID = QueryHelper.GetGuid("formguid", Guid.Empty);
            fileUploaderElem.NodeParentNodeID = QueryHelper.GetInteger("parentid", 0);
            fileUploaderElem.DocumentID = QueryHelper.GetInteger("documentid", 0);

            fileUploaderElem.SourceType = MediaSourceEnum.DocumentAttachments;

            string siteName = SiteContext.CurrentSiteName;
            string allowed = QueryHelper.GetString("allowedextensions", String.Empty);
            if (String.IsNullOrEmpty(allowed))
            {
                allowed = SettingsKeyInfoProvider.GetValue(siteName + ".CMSUploadExtensions");
            }
            fileUploaderElem.AllowedExtensions = allowed;

            fileUploaderElem.ResizeToWidth = QueryHelper.GetInteger("autoresize_width", 0);
            fileUploaderElem.ResizeToHeight = QueryHelper.GetInteger("autoresize_height", 0);
            fileUploaderElem.ResizeToMaxSideSize = QueryHelper.GetInteger("autoresize_maxsidesize", 0);

            fileUploaderElem.AfterSaveJavascript = "InsertImageOrMedia";
            fileUploaderElem.InsertMode = true;
            fileUploaderElem.IsLiveSite = false;

            fileUploaderElem.FileUploadControl.Attributes["title"] = HTMLHelper.EncodeForHtmlAttribute(GetString("wysiwyg.ui.quicklyinsertimage"));

            ScriptHelper.RegisterStartupScript(this, typeof(String), "DirectFileUpload_" + ClientID, "window.uploaderFocused = false;", true);
        }
    }
}
