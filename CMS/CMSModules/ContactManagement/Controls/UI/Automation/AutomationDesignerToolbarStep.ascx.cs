using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.UIControls.UniMenuConfig;


public partial class CMSModules_ContactManagement_Controls_UI_Automation_AutomationDesignerToolbarStep : CMSUserControl
{
    public Item StepItem
    {
        get;
        set;
    }


    protected override void OnLoad(EventArgs e)
    {
        if (StepItem == null)
        {
            Visible = false;
        }
        else
        {
            stepTitle.Text = StepItem.Text;
            pnlStep.ToolTip = StepItem.Tooltip;

            SetStepImage();
        }

        base.OnLoad(e);
    }


    private void SetStepImage()
    {
        stepIcon.Visible = true;
        string iconHtml;

        if (!String.IsNullOrEmpty(StepItem.ImagePath))
        {
            var altText = String.IsNullOrEmpty(StepItem.ImageAltText) ? StepItem.Text : StepItem.ImageAltText;
            iconHtml = $"<img src=\"{ResolveUrl(StepItem.ImagePath)}\" alt=\"{GetString(altText)}\" />";
        }
        else if (!String.IsNullOrEmpty(StepItem.IconClass))
        {
            iconHtml = $"<i aria-hidden=\"true\" class=\"{HTMLHelper.EncodeForHtmlAttribute(StepItem.IconClass)}\"></i>";
        }
        else
        {
            stepIcon.Visible = false;
            return;
        }

        stepIcon.Text = $"<div class='automation-step-icon {HTMLHelper.EncodeForHtmlAttribute(StepItem.IconWrapperClass)}'><div class='cms-icon-container'>{iconHtml}</div></div>";
    }
}
