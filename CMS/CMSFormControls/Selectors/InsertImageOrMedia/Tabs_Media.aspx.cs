using System;
using System.Web.UI;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_InsertImageOrMedia_Tabs_Media : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string source = QueryHelper.GetString("source", "");
        MediaSourceEnum src = CMSDialogHelper.GetMediaSource(source);

        // Check UIProfile
        string output = QueryHelper.GetString("output", "");
        bool checkUI = (output != "copy") && (output != "move") && (output != "link") && (output != "linkdoc") && (output != "relationship") && (output != "selectpath");

        if (checkUI)
        {
            string errorMessage = "";

            OutputFormatEnum outputFormat = CMSDialogHelper.GetOutputFormat(output, QueryHelper.GetBoolean("link", false));
            if ((outputFormat == OutputFormatEnum.HTMLLink) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertLink"))
            {
                errorMessage = "InsertLink";
            }
            else if ((outputFormat == OutputFormatEnum.HTMLMedia) && !MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertImageOrMedia"))
            {
                errorMessage = "InsertImageOrMedia";
            }

            if (errorMessage != "")
            {
                RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", errorMessage);
                return;
            }

            switch (src)
            {
                case MediaSourceEnum.DocumentAttachments:
                case MediaSourceEnum.Attachment:
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "AttachmentsTab"))
                    {
                        errorMessage = "AttachmentsTab";
                    }
                    break;

                case MediaSourceEnum.MetaFile:
                    if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "MetafilesTab"))
                    {
                        errorMessage = "MetafilesTab";
                    }
                    break;
            }
            if (errorMessage != "")
            {
                RedirectToUIElementAccessDenied("CMS.MediaDialog", errorMessage);
                return;
            }
        }

		// CKEditor's plugin filebrowser add custom params to url. 
		// This ensures that custom params aren't validated
		if (QueryHelper.ValidateHash("hash", "CKEditor;CKEditorFuncNum;langCode", validateWithoutExcludedParameters: true))
        {
            CMSDialogHelper.RegisterDialogHelper(Page);
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "InitResizers", ScriptHelper.GetScript("InitResizers();"));

            linkMedia.InitFromQueryString();
        }
        else
        {
            linkMedia.StopProcessing = true;
            linkMedia.Visible = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }
}
