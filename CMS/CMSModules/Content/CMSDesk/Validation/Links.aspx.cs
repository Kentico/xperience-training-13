using CMS.UIControls;


[Security(Resource = "CMS.Content", UIElements = "Validation.Links")]
public partial class CMSModules_Content_CMSDesk_Validation_Links : CMSValidationPage
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