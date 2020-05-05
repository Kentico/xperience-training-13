using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_InsertImageOrMedia_Tabs_Email : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.WYSIWYGEditor", "InsertLink"))
        {
            RedirectToUIElementAccessDenied("CMS.WYSIWYGEditor", "InsertLink");
        }
        else if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.MediaDialog", "EmailTab"))
        {
            RedirectToUIElementAccessDenied("CMS.MediaDialog", "EmailTab");
        }

		// CKEditor's plugin filebrowser add custom params to url. 
		// This ensures that custom params aren't validated
		if (QueryHelper.ValidateHash("hash", "CKEditor;CKEditorFuncNum;langCode", validateWithoutExcludedParameters: true))
        {
            ScriptHelper.RegisterJQuery(Page);
            CMSDialogHelper.RegisterDialogHelper(Page);
        }
        else
        {
            emailProperties.StopProcessing = true;
            emailProperties.Visible = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }
}
