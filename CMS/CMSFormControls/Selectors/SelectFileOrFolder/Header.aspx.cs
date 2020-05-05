using System;
using System.Collections;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSFormControls_Selectors_SelectFileOrFolder_Header : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.ValidateHash("hash"))
        {
            bool showFolders = GetParameter("show_folders");

            PageTitle.TitleText = GetString("dialogs.header.title." + (showFolders ? "selectfolder" : "selectfiles"));
        }
        else
        {
            string url = ResolveUrl(AdministrationUrlHelper.GetErrorPageUrl("dialogs.badhashtitle", "dialogs.badhashtext", true));
            ltlScript.Text = ScriptHelper.GetScript("if (window.parent != null) { window.parent.location = '" + url + "' }");
        }
    }


    private static bool GetParameter(string parameterName)
    {
        bool showFolders = QueryHelper.GetBoolean(parameterName, false);

        var paramsGuid = QueryHelper.GetString("params", String.Empty);
        if (!String.IsNullOrEmpty(paramsGuid))
        {
            // Try to get parameters
            var dialogOptions = WindowHelper.GetItem(paramsGuid, true) as Hashtable;
            if (dialogOptions != null)
            {
                showFolders = ValidationHelper.GetBoolean(dialogOptions[parameterName], false);
            }
        }

        return showFolders;
    }
}
