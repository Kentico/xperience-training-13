using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DocumentEngine.Internal;
using CMS.Helpers;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.OnlineForms.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;

[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("CMS.Form", "Forms.FormBuilderMVC")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_FormBuilderMVC : CMSBizFormPage
{
    private const string FORMBUILDER_ROUTE_TEMPLATE = "/Kentico.FormBuilder/Index/{0}";
    private const string BUILDER_MODE_QUERY_STRING_NAME = "builder";
    private const string FORM_BUILDER_MODE = "formBuilder";


    protected void Page_Load(object sender, EventArgs e)
    {
        var presentationUrl = new PresentationUrlRetriever().RetrieveForAdministration(SiteContext.CurrentSiteName);
        if (String.IsNullOrEmpty(presentationUrl))
        {
            ShowError(ResHelper.GetString("bizform.formBuilderMVC.presentationURLMissing"));
            return;
        }

        var uri = new Uri(presentationUrl);
        var targetOrigin = uri.GetLeftPart(UriPartial.Authority);
        var path = string.Format(FORMBUILDER_ROUTE_TEMPLATE, EditedForm.FormID);

        ScriptHelper.RegisterModule(this, "CMS.Builder/FormBuilder/Messaging", new
        {
            frameId = formBuilderFrame.ClientID,
            origin = targetOrigin
        });

        // Modify frame 'src' attribute and add administration domain into it
        ScriptHelper.RegisterModule(this, "CMS.Builder/FrameSrcAttributeModifier", new
        {
            frameId = formBuilderFrame.ClientID,
            frameSrc = URLHelper.AddParameterToUrl(presentationUrl.TrimEnd('/') + VirtualContext.GetFormBuilderPath(path, MembershipContext.AuthenticatedUser.UserGUID), BUILDER_MODE_QUERY_STRING_NAME, FORM_BUILDER_MODE),
            mixedContentMessage = GetString("builder.ui.mixedcontenterrormessage"),
            applicationPath = SystemContext.ApplicationPath
        });
        
        RegisterCookiePolicyDetection();
    }
}