using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("EmailTemplateListExtender", typeof(EmailTemplateListExtender))]

/// <summary>
/// Email template unigrid extender
/// </summary>
public class EmailTemplateListExtender : ControlExtender<UniGrid>
{
    private readonly EmailTemplateTypeRegister mEmailTemplateTypeRegister = EmailTemplateTypeRegister.Current;


    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;
    }


    /// <summary>
    /// Handles external databound event of unigrid.
    /// </summary>
    protected object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "type":
                string type = ValidationHelper.GetString(parameter, string.Empty);
                if (string.IsNullOrEmpty(type))
                {
                    type = EmailModule.GENERAL_EMAIL_TEMPLATE_TYPE_NAME;
                }

                var item = mEmailTemplateTypeRegister.GetTemplateType(type);

                return HTMLHelper.HTMLEncode(item != null ? ResHelper.GetString(item.DisplayNameResourceString) : type);
        }

        return parameter;
    }
}