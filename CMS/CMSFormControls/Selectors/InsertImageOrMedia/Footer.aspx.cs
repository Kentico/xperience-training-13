using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_InsertImageOrMedia_Footer : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
		// CKEditor's plugin filebrowser add custom params to url. 
		// This ensures that custom params aren't validated
		if (QueryHelper.ValidateHash("hash", "CKEditor;CKEditorFuncNum;langCode", validateWithoutExcludedParameters: true))
        {
            footerElem.InitFromQueryString();
        }
        else
        {
            footerElem.StopProcessing = true;
            footerElem.Visible = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }
}
