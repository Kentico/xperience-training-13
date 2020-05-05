using System;
using System.Collections;

using CMS;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;

[assembly: RegisterCustomClass("ModuleLibrarySelectionExtender", typeof(ModuleLibrarySelectionExtender))]

/// <summary>
/// Analytics page extender
/// </summary>
public class ModuleLibrarySelectionExtender : ControlExtender<CMSUserControl>
{
    private LocalizedButton btnSelect;

    private CMSUIPage page;


    public override void OnInit()
    {
        btnSelect = new LocalizedButton();
        page = ((CMSUIPage)Control.Page);
        btnSelect.ResourceString = "general.select";
        btnSelect.Click += btnSelectClick;
        btnSelect.ID = "btnSelect";
        page.DialogFooter.AddControl(btnSelect);
        page.DialogFooter.HideCancelButton();
        if (!RequestHelper.IsPostBack())
        {
            SessionHelper.Remove("DialogSelectedParameters");
        }
    }


    private void btnSelectClick(object sender, EventArgs e)
    {
        Hashtable selectedParameters = SessionHelper.GetValue("DialogSelectedParameters") as Hashtable;
        string filePath = null;

        if (selectedParameters != null)
        {
            filePath = ValidationHelper.GetString(selectedParameters[DialogParameters.ITEM_PATH], null);
        }

        if (filePath == null)
        {
            page.ShowError(ResHelper.GetString("cms.resourcelibrary.noselectionerror"), null, null, false);

            return;
        }
        
        ResourceLibraryInfo libraryInfo = new ResourceLibraryInfo
        {
            ResourceLibraryPath = filePath,
            ResourceLibraryResourceID = QueryHelper.GetInteger("resourceid", -1),
        };

        // Check if resource has the library already referenced
        if (!ResourceLibraryInfoProvider.ValidateLibraryUniqueness(libraryInfo.ResourceLibraryResourceID, libraryInfo.ResourceLibraryPath))
        {
            page.ShowError(ResHelper.GetString("cms.resourcelibrary.collision"), null, null, false);

            return;
        }
        else if (ResourceLibraryInfoProvider.IsImplicitlyIncludedLibrary(libraryInfo, ResourceInfoProvider.GetResourceInfo(libraryInfo.ResourceLibraryResourceID)))
        {
            page.ShowError(ResHelper.GetString("cms.resourcelibrary.implicitlyincludedlibrary"), null, null, false);

            return;
        }
        else
        {
            ResourceLibraryInfoProvider.SetResourceLibraryInfo(libraryInfo);
        }

        SessionHelper.Remove("DialogSelectedParameters");
        ScriptHelper.RegisterStartupScript(page, typeof(string), "Close dialog", ScriptHelper.GetScript("CloseDialog(true);"));
    }
}
