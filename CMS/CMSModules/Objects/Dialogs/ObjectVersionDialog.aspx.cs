using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.UIControls;


public partial class CMSModules_Objects_Dialogs_ObjectVersionDialog : CMSModalDesignPage
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Check the license
        if (DataHelper.GetNotEmpty(RequestContext.CurrentDomain, "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.ObjectVersioning);
        }

        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(GetString("dialogs.badhashtitle"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get query string parameters
        string objectType = QueryHelper.GetString("objecttype", String.Empty);
        int objectId = QueryHelper.GetInteger("objectid", 0);

        // Set version list control
        versionList.ObjectID = objectId;
        versionList.ObjectType = objectType;
        versionList.IsLiveSite = false;

        // Register refresh script to refresh wopener
        StringBuilder script = new StringBuilder();
        script.Append(@"
function RefreshContent() {
  if(wopener != null) {
    if (wopener.RefreshPage != null) {
      wopener.RefreshPage();
    }
    else if (wopener.Refresh != null) {
      wopener.Refresh();
    }
  }
}");
        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script.ToString()));

        
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        string title = String.Format(GetString("objectversioning.objectversiondialog.title"), GetString("objecttype." + versionList.ObjectType.Replace(".", "_")), (versionList.Object != null) ? HTMLHelper.HTMLEncode(versionList.Object.Generalized.ObjectDisplayName) : String.Empty);

        // Set title and close button
        SetTitle(title);
    }
}
