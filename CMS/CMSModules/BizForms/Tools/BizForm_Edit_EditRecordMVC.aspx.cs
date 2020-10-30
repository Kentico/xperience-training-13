using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_BizForms_Tools_BizForm_Edit_EditRecordMVC : CMSBizFormPage
{
    private const string FORM_ITEM_EDIT_ROUTE_TEMPLATE = "/Kentico.FormBuilder/FormItem/Edit/{0}";

    private int formId;
    private int recordId;


    protected void Page_Load(object sender, EventArgs e)
    {
        formId = QueryHelper.GetInteger("formid", 0);
        recordId = QueryHelper.GetInteger("formRecordId", 0);
        var presentationUrl = new PresentationUrlRetriever().RetrieveForAdministration(SiteContext.CurrentSiteName);
        if (String.IsNullOrEmpty(presentationUrl))
        {
            ShowError(ResHelper.GetString("bizform.formBuilderMVC.presentationURLMissing"));
            return;
        }

        var path = String.Format(FORM_ITEM_EDIT_ROUTE_TEMPLATE, formId);
        if (recordId > 0)
        {
            path += $"/{recordId}";
        }

        // Modify frame 'src' attribute and add administration domain into it
        ScriptHelper.RegisterModule(this, "CMS.Builder/FrameSrcAttributeModifier", new
        {
            frameId = formBuilderFrame.ClientID,
            frameSrc = presentationUrl.TrimEnd('/') + VirtualContext.GetFormBuilderPath(path, MembershipContext.AuthenticatedUser.UserGUID),
            mixedContentMessage = GetString("builder.ui.mixedcontenterrormessage"),
            applicationPath = SystemContext.ApplicationPath
        });

        RegisterCookiePolicyDetection();
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Create breadcrumbs
        CreateBreadcrumbs();

        base.OnPreRender(e);
    }


    /// <summary>
    /// Creates breadcrumbs for form item
    /// </summary>
    private void CreateBreadcrumbs()
    {
        string text = GetString(recordId > 0 ? "BizForm_Edit_EditRecord.EditRecord" : "BizForm_Edit_EditRecord.NewRecord");

        // Initializes page title
        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = GetString("bizform.data"),
            RedirectUrl = "~/CMSModules/BizForms/Tools/BizForm_Edit_Data.aspx?formid=" + formId
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = text
        });

        // Do not include type as breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix("");
    }
}