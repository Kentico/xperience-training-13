using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSInstall_Controls_StepNavigation : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// Indicates if control should display table before and after button's panel.
    /// </summary>
    public bool NextPreviousVisible { get; set; }


    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get { return StepPrevButton; }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get { return StepNextButton; }
    }


    /// <summary>
    /// Gets or sets if JS for next/previous buttons should be included.
    /// </summary>
    public bool NextPreviousJS
    {
        get;
        set;
    }

    /// <summary>
    /// Indicates if current step is finish step.
    /// </summary>
    public bool FinishStep
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (NextPreviousVisible)
        {
            ltlTableBefore.Text =
                @"<table border='0' cellpadding='0' cellspacing='0' width='100%'>
    <tr>
        <td css='FloatLeft'>";
            ltlTableBefore.Visible = true;
            ltlTableAfter.Text =
                @"      </td>
    </tr>
</table>";
            ltlTableAfter.Visible = true;
            StepPrevButton.Visible = true;
        }
        else
        {
            ltlTableBefore.Visible = false;
            ltlTableAfter.Visible = false;
            StepPrevButton.Visible = false;
        }

        if (NextPreviousJS)
        {
            StepPrevButton.OnClientClick = "PrevStep(this,document.getElementById('buttonsDiv')); return false;";
            StepNextButton.OnClientClick = "NextStep(this,document.getElementById('buttonsDiv')); return false;";
        }

        if (FinishStep)
        {
            StepNextButton.CommandName = "Finish";
        }
    }

    #endregion
}