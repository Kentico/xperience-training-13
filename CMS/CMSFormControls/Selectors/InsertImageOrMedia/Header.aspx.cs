using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_InsertImageOrMedia_Header : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.HelpName = "lnkMediaSelectorHelp";

		// CKEditor's plugin filebrowser add custom params to url. 
		// This ensures that custom params aren't validated
		if (QueryHelper.ValidateHash("hash", "CKEditor;CKEditorFuncNum;langCode", validateWithoutExcludedParameters: true))
        {
            header.CurrentMaster = CurrentMaster;
            header.InitFromQueryString();
        }
        else
        {
            header.StopProcessing = true;
            header.Visible = false;
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "errorRedirect", ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }"));
        }
    }
}
