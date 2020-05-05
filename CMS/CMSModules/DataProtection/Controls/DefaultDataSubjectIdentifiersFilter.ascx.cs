using System.Collections.Generic;

using CMS.Helpers;
using CMS.UIControls;

/// <summary>
/// Default filter control for data subject identifiers input. Provides the user with email input.
/// </summary>
public partial class CMSModules_DataProtection_Controls_DefaultDataSubjectIdentifiersFilter : DataSubjectIdentifiersFilterControl
{
    /// <summary>
    /// Fills the <paramref name="filter"/> with email.
    /// </summary>
    public override IDictionary<string, object> GetFilter(IDictionary<string, object> filter)
    {
        filter.Add("email", txtEmail.Text);
        
        return filter;
    }


    /// <summary>
    /// Returns <c>true</c> if valid email address is entered. Otherwise, returns <c>false</c> and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        if (!ValidationHelper.IsEmail(txtEmail.Text))
        {
            AddError(GetString("dataprotection.app.validation.emailrequired"));

            return false;
        }
        
        return true;
    }
}
