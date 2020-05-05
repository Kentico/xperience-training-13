using CMS;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("CreateInstallationPackageExtender", typeof(CreateInstallationPackageExtender))]

/// <summary>
/// Extender sets up the control that creates th installation package and places FooterControl to the Footer of the UIPage
/// </summary>
public class CreateInstallationPackageExtender : ControlExtender<ExportModuleControl>
{
    public override void OnInit()
    {
        QueryHelper.ValidateHash("hash");
        Control.ResourceID = QueryHelper.GetInteger("resourceid", -1);

        CMSUIPage page = (CMSUIPage)Control.Page;


        page.DialogFooter.HideCancelButton();
        if (Control.FooterControl != null)
        {
            page.DialogFooter.AddControl(Control.FooterControl);
        }
    }
}