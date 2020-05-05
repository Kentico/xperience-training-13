using System;
using System.Text;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Pages_ObjectVersions : CMSObjectVersioningPage
{
    private bool dialogMode = false;


    protected override void OnPreInit(EventArgs e)
    {
        // Check hash
        if (!QueryHelper.ValidateHash("hash"))
        {
            RedirectToAccessDenied(GetString("dialogs.badhashtitle"));
        }

        // Set dialog mode - it is used for example in transformation versions that is edited from webpart properties
        dialogMode = QueryHelper.GetBoolean("editonlycode", false);
        if (dialogMode)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master";
        }

        // Must be called after the master page file is set
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string objectType = QueryHelper.GetString("objecttype", String.Empty);
        int objectId = QueryHelper.GetInteger("objectid", 0);

        versionList.ObjectID = objectId;
        versionList.ObjectType = objectType;
        versionList.IsLiveSite = false;
        versionList.RegisterReloadHeaderScript = !QueryHelper.GetBoolean("noreload", false);
        versionList.DialogMode = dialogMode;

        // Register refresh script to refresh wopener
        StringBuilder script = new StringBuilder();
        script.Append(@"
function RefreshContent() {
  var wopener = parent.wopener;  
  if(wopener != null) {
    if (wopener.RefreshPage != null) {
      wopener.RefreshPage();
    }
    else if (wopener.Refresh != null) {
      wopener.Refresh();
    } 
  }
  else
  {
     if((parent != null) && (parent.Refresh != null))
     {
        parent.Refresh();
     }
  }
}");
        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script.ToString()));
    }
}
