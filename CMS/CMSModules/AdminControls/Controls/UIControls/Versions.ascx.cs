using System.Text;

using CMS.Base.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_Versions : CMSAbstractUIWebpart
{
    public override void OnContentLoaded()
    {
        LoadData();
        base.OnContentLoaded();
    }


    private void LoadData()
    {
        string objectType = UIContextHelper.GetObjectType(UIContext);

        versionList.ObjectID = UIContext.ObjectID;
        versionList.ObjectType = objectType;
        versionList.IsLiveSite = false;
        versionList.RegisterReloadHeaderScript = !QueryHelper.GetBoolean("noreload", false);

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