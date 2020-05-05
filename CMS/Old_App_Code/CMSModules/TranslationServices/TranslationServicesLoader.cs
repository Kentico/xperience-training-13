using CMS;
using CMS.DocumentEngine.Web.UI;
using CMS.Base;
using CMS.TranslationServices;
using CMS.DataEngine;

[assembly: RegisterModule(typeof(TranslationServicesMethodsModule))]

/// <summary>
/// Registers translation services methods.
/// </summary>
public class TranslationServicesMethodsModule : Module
{
    public TranslationServicesMethodsModule()
        : base("CMS.TranslationServicesMethods")
    {
    }


    protected override void OnInit()
    {
        Extend<TransformationNamespace>.With<TranslationServicesMethods>();
    }
}
