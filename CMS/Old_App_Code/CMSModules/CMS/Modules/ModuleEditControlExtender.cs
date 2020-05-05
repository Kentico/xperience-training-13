using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;

[assembly: RegisterCustomClass("ModuleEditControlExtender", typeof(ModuleEditControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class ModuleEditControlExtender : ControlExtender<UIForm>
{
    /// <summary>
    /// Gets the current resource
    /// </summary>
    public ResourceInfo Resource
    {
        get
        {
            return Control.UIContext.EditedObject as ResourceInfo;
        }
    }


    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        if ((Resource != null) && (Resource.ResourceIsInDevelopment) && !Resource.ResourceName.EqualsCSafe(ModuleName.CUSTOMSYSTEM, true))
        {
            var page = (CMSUIPage)Control.Page;
            string query = QueryHelper.BuildQuery("resourceid", Resource.ResourceID.ToString());
            string url = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "ExportModuleDialog"), query);
            var exportModuleHeaderAction = new HeaderAction()
            {
                Text = ResHelper.GetString("cms.modules.installpackage.createbutton"),
                RedirectUrl = url,
                OpenInDialog = true,
                DialogWidth = "750",
                DialogHeight = "800",
                ButtonStyle = ButtonStyle.Default,
            };
            page.AddHeaderAction(exportModuleHeaderAction);
        }

        // Set the state of new module to development
        Control.OnBeforeSave += (sender, args) =>
        {
            if ((Resource == null) || (Resource.ResourceID <= 0))
            {
                Control.Data["ResourceIsInDevelopment"] = true;
            }
        };

        // Disable editing when module is not editable
        Control.Load += (sender, args) =>
        {
            if (Resource != null)
            {
                // Display warning for installed resources
                if ((Resource.ResourceID > 0) && !Resource.IsEditable)
                {
                    Control.SubmitButton.Enabled = Control.Enabled = false;
                    Control.ShowInformation(Control.GetString("resource.installedresourcewarning"));
                }
                // Display warning for system resources
                else if (!SystemContext.DevelopmentMode && ResourceInfoProvider.IsSystemResource(Resource.ResourceName))
                {
                    Control.SubmitButton.Enabled = Control.Enabled = false;
                    Control.ShowInformation(Control.GetString("resource.cusomresourcewarning"));
                }
            }
        };
    }
}