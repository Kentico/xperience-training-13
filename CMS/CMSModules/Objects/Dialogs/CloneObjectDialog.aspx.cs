using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Objects_Dialogs_CloneObjectDialog : CMSModalPage
{
    private BaseInfo infoToClone;
    private string objTypeName;
    private bool isErrorMode;


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Get query string parameters
        var objectType = QueryHelper.GetString("objecttype", String.Empty);
        var objectId = QueryHelper.GetInteger("objectid", 0);

        // Get the object
        infoToClone = ProviderHelper.GetInfoById(objectType, objectId);

        if (infoToClone != null)
        {
            objTypeName = GetString("objecttype." + TranslationHelper.GetSafeClassName(infoToClone.TypeInfo.ObjectType));
        }

        if (objTypeName.StartsWith("objecttype.", StringComparison.OrdinalIgnoreCase))
        {
            objTypeName = "";
            SetTitle(String.Format(GetString("clonning.dialog.title"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(infoToClone.Generalized.ObjectDisplayName))));
        }
        else
        {
            SetTitle(String.Format(GetString("clonning.dialog.title"), objTypeName));
        }

        if (infoToClone == null)
        {
            ShowInformation(GetString("clonning.dialog.objectdoesnotexist"));
            cloneObjectElem.Visible = false;
            return;
        }

        // Check permissions
        if (!infoToClone.CheckPermissions(PermissionsEnum.Read, CurrentSiteName, CurrentUser))
        {
            RedirectToAccessDenied(infoToClone.TypeInfo.ModuleName, "read");
        }

        cloneObjectElem.InfoToClone = infoToClone;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!isErrorMode)
        {
            var infoResourceString = cloneObjectElem.HasNoSettings() ? "clonning.settings.emptyinfobox" : "clonning.settings.infobox";
            ShowInformation(String.Format(GetString(infoResourceString), objTypeName, HTMLHelper.HTMLEncode(ResHelper.LocalizeString(infoToClone.Generalized.ObjectDisplayName))));
        }

        // Register refresh script to refresh wopener
        var script = @"
function RefreshContent() {
  if (wopener != null) {
    if (wopener.RefreshWOpener) {
        window.refreshPageOnClose = true;
        wopener.RefreshWOpener(window);
    } else {
        wopener.window.location.replace(wopener.window.location);
    }
  }
}";
        // Register script
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script));
    }


    protected void btnClone_Click(object sender, EventArgs e)
    {
        try
        {
            CloneResult result = cloneObjectElem.CloneObject();
            if (result != null)
            {
                if (result.Errors.Count > 0)
                {
                    ShowError(ResHelper.LocalizeString(String.Join("\n", result.Errors)));
                    SwitchToErrorMode();
                }
                else if (result.Warnings.Count > 0)
                {
                    ShowWarning(GetString("cloning.savedwithwarnings"), ResHelper.LocalizeString(String.Join("<br/>", result.Warnings)));
                    SwitchToErrorMode();
                }
                else
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloneRefresh", cloneObjectElem.CloseScript, true);
                }
            }
        }
        catch (Exception ex)
        {
            Service.Resolve<IEventLogService>().LogException((infoToClone == null) ? "System" : infoToClone.TypeInfo?.ObjectType.ToLowerInvariant(), "CLONEOBJECT", ex);
            ShowError(ex.Message);

            if (!cloneObjectElem.UseTransaction)
            {
                SwitchToErrorMode();
            }
        }
    }


    private void SwitchToErrorMode()
    {
        isErrorMode = true;
        plcForm.Visible = false;
        btnClone.Visible = false;
        SetCloseJavascript(cloneObjectElem.CloseScript + ";return false;");
    }
}
