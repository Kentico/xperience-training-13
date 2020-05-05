using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_Wizard_ConfirmationCheckbox : CMSAbstractWebPart
{
    #region "Page events"

    /// <summary>
    /// Initializes the control.
    /// </summary>    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        SetupControl();
    }

    #endregion


    #region "Wizard methods"

    protected override void ValidateStepData(object sender, StepEventArgs e)
    {
        base.ValidateStepData(sender, e);

        bool valid = chkAccept.Checked;

        if (!valid)
        {
            pnlError.Visible = true;
            e.CancelEvent = true;

            // Get the custom error text if filled out            
            lblError.Text = ValidationHelper.GetString(GetValue("ErrorMessage"), "");
        }
        else
        {
            pnlError.Visible = false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets up the control.
    /// </summary>
    public void SetupControl()
    {
        if (!StopProcessing)
        {
            // Show the control
            pnlCheckBox.Visible = true;
            // Get the custom text if filled out and set the custom label             
            chkAccept.Text = ValidationHelper.GetString(GetValue("AgreementText"), "");
            // Set the state
            chkAccept.Checked = ValidationHelper.GetBoolean(GetValue("AgreementDefaultState"), false);
        }
    }

    #endregion
}