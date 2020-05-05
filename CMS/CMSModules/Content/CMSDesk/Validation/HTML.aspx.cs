using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Validation.HTML")]
public partial class CMSModules_Content_CMSDesk_Validation_HTML : CMSValidationPage
{
    #region "Properties"

    protected override DocumentValidator Validator
    {
        get
        {
            return validator;
        }
    }

    #endregion
}