using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Validation.CSS")]
public partial class CMSModules_Content_CMSDesk_Validation_CSS : CMSValidationPage
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