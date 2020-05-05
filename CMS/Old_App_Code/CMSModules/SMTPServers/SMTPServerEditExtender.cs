using System;

using CMS;
using CMS.Base.Web.UI;
using CMS.EmailEngine;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

[assembly: RegisterCustomClass("SMTPServerEditExtender", typeof(SMTPServerEditExtender))]

/// <summary>
/// SMTP server uiform extender
/// </summary>
public class SMTPServerEditExtender : ControlExtender<UIForm>
{
    public override void OnInit()
    {
        Control.OnAfterValidate += OnAfterValidate;
    }


    /// <summary>
    /// Handles after validation event of UIForm.
    /// </summary>
    protected void OnAfterValidate(object sender, EventArgs e)
    {
        if ((ValidationHelper.GetInteger(Control.GetFieldValue("ServerDeliveryMethod"), -1) == (int)SMTPServerDeliveryEnum.SpecifiedPickupDirectory) && (DataHelper.IsEmpty(Control.GetFieldValue("ServerPickupDirectory"))))
        {
            Control.StopProcessing = true;
            Control.DisplayErrorLabel("ServerPickupDirectory", ResHelper.GetString("general.requiresvalue"));
        }
    }
}