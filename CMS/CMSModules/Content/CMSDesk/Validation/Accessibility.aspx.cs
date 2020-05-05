using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Validation.Accessibility")]
public partial class CMSModules_Content_CMSDesk_Validation_Accessibility : CMSValidationPage
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