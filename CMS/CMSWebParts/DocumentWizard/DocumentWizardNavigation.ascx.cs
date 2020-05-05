using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using CMS.DocumentEngine.Web.UI;
using CMS.Base.Web.UI;
using CMS.PortalEngine.Web.UI;
using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;

public partial class CMSWebParts_DocumentWizard_DocumentWizardNavigation : CMSAbstractWizardWebPart
{
    #region "Variables"

    int controlTemplateIndex = 0;

    ITemplate activeTemplate = null;
    ITemplate inactiveTemplate = null;
    ITemplate currentTemplate = null;

    ITemplate alternatingTemnplate = null;
    ITemplate firstTemplate = null;
    ITemplate lastTemplate = null;
    ITemplate lastInactiveTemplate = null;

    bool autoGenerate = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Active item transformation.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("TransformationName"), "");
        }
        set
        {
            this.SetValue("TransformationName", value);
        }
    }


    /// <summary>
    /// Current item transformation.
    /// </summary>
    public string CurrentItemTranformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("CurrentItemTranformationName"), "");
        }
        set
        {
            this.SetValue("CurrentItemTranformationName", value);
        }
    }


    /// <summary>
    /// Inactive item transformation.
    /// </summary>
    public string InactiveItemTransformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("InactiveItemTransformationName"), "");
        }
        set
        {
            this.SetValue("InactiveItemTransformationName", value);
        }
    }


    /// <summary>
    /// Specifies the transformation used to display even items in the List view mode. Transformations are specified in the <class name>.<transformation name> format..
    /// </summary>
    public string AlternatingTransformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("AlternatingTransformationName"), "");
        }
        set
        {
            this.SetValue("AlternatingTransformationName", value);
        }
    }


    /// <summary>
    /// First item transformation.
    /// </summary>
    public string FirstTransformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("FirstTransformationName"), "");
        }
        set
        {
            this.SetValue("FirstTransformationName", value);
        }
    }


    /// <summary>
    /// Last item transformation name.
    /// </summary>
    public string LastTransformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LastTransformationName"), "");
        }
        set
        {
            this.SetValue("LastTransformationName", value);
        }
    }


    /// <summary>
    /// Last inactive item transformation.
    /// </summary>
    public string LastInactiveTransformationName
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("LastInactiveTransformationName"), "");
        }
        set
        {
            this.SetValue("LastInactiveTransformationName", value);
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// Init handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable web part, web part is allowed in "StepLoaded" method
        StopProcessing = true;
        Visible = false;
    }


    /// <summary>
    /// PreRender handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        if (!StopProcessing)
        {
            // Loads ITemplates
            LoadTemplates();

            // Check whether exists at least one transformation
            if (!autoGenerate)
            {
                // Loop thru steps
                foreach (DocumentWizardStep step in WizardManager.Steps)
                {
                    bool isInactive = WizardManager.RestrictStepOrder && (step.StepIndex > (WizardManager.LastConfirmedStepIndex + 1));

                    // Last inactive item
                    if ((WizardManager.LastStep == step) && isInactive && (lastInactiveTemplate != null))
                    {
                        BindData(lastInactiveTemplate, step.StepData);
                    }
                    // Last item
                    else if ((WizardManager.LastStep == step) && (lastTemplate != null))
                    {
                        BindData(lastInactiveTemplate, step.StepData);
                    }
                    // First item
                    else if ((WizardManager.FirstStep == step) && (firstTemplate != null))
                    {
                        BindData(firstTemplate, step.StepData);
                    }
                    // Current item
                    else if ((WizardManager.CurrentStep == step) && (currentTemplate != null))
                    {
                        BindData(currentTemplate, step.StepData);
                    }
                    // Inactive template
                    else if (isInactive && (inactiveTemplate != null))
                    {
                        BindData(inactiveTemplate, step.StepData);
                    }
                    // General template
                    else if (activeTemplate != null)
                    {
                        BindData(activeTemplate, step.StepData);
                    }
                }
            }
            // Auto-generate default navigation of no template selected
            else
            {
                StringBuilder sb = new StringBuilder();

                // Envelope div
                sb.Append(@"<div class=""DocWizardNav"">");

                // Loop thru steps
                foreach (DocumentWizardStep step in WizardManager.Steps)
                {
                    bool isInactive = WizardManager.RestrictStepOrder && (step.StepIndex > (WizardManager.LastConfirmedStepIndex + 1));

                    // Item envelope
                    sb.Append(@"<div class=""DocWizardItem");
                    if (WizardManager.CurrentStep == step)
                    {
                        sb.Append(@"Current"">");
                    }
                    else if (WizardManager.FirstStep == step)
                    {
                        sb.Append(@"First"">");
                    }
                    else if (WizardManager.LastStep == step)
                    {
                        sb.Append(@"Last"">");
                    }
                    else
                    {
                        sb.Append(@""">");
                    }

                    if (isInactive)
                    {
                        sb.Append("<span>");
                        sb.Append(HTMLHelper.HTMLEncode(Convert.ToString(step.StepData["DocumentName"])));
                        sb.Append("</span>");
                    }
                    else
                    {
                        sb.Append(@"<a href=""");
                        sb.Append(@""">");
                        sb.Append(HTMLHelper.HTMLEncode(Convert.ToString(step.StepData["DocumentName"])));
                        sb.Append("</a>");
                    }

                    // Close item envelope
                    sb.Append(@"</div>");
                }

                // Close envelope div
                sb.Append(@"</div>");

                // Save as string and display
                ltlAuto.Text = sb.ToString();
                ltlAuto.Visible = true;

            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Loads ITemplates from transformation names
    /// </summary>
    private void LoadTemplates()
    {
        // Transformation
        if (!String.IsNullOrEmpty(TransformationName))
        {
            activeTemplate = TransformationHelper.LoadTransformation(this, TransformationName);
            autoGenerate = false;
        }

        // Inactive
        if (!String.IsNullOrEmpty(InactiveItemTransformationName))
        {
            inactiveTemplate = TransformationHelper.LoadTransformation(this, InactiveItemTransformationName);
            autoGenerate = false;
        }

        // Current
        if (!String.IsNullOrEmpty(CurrentItemTranformationName))
        {
            currentTemplate = TransformationHelper.LoadTransformation(this, CurrentItemTranformationName);
            autoGenerate = false;
        }

        // Alternating
        if (!String.IsNullOrEmpty(AlternatingTransformationName))
        {
            alternatingTemnplate = TransformationHelper.LoadTransformation(this, AlternatingTransformationName);
            autoGenerate = false;
        }

        // First
        if (!String.IsNullOrEmpty(FirstTransformationName))
        {
            firstTemplate = TransformationHelper.LoadTransformation(this, FirstTransformationName);
            autoGenerate = false;
        }

        // Last
        if (!String.IsNullOrEmpty(LastTransformationName))
        {
            lastTemplate = TransformationHelper.LoadTransformation(this, LastTransformationName);
            autoGenerate = false;
        }

        // Last inactive
        if (!String.IsNullOrEmpty(LastInactiveTransformationName))
        {
            lastInactiveTemplate = TransformationHelper.LoadTransformation(this, LastInactiveTransformationName);
            autoGenerate = false;
        }
    }


    /// <summary>
    /// Step loaded
    /// </summary>
    protected override void StepLoaded(object sender, StepEventArgs e)
    {
        base.StepLoaded(sender, e);

        if (WizardManager.Steps.Count != 0)
        {
            Visible = true;
            StopProcessing = false;
        }
    }


    /// <summary>
    /// BindData
    /// </summary>
    private void BindData(ITemplate template, DataRowView dr)
    {
        RepeaterItem item = new RepeaterItem(controlTemplateIndex, ListItemType.Item);
        item.DataItem = dr;
        item.ID = "ctl" + controlTemplateIndex.ToString().PadLeft(2, '0');
        Controls.Add(item);

        template.InstantiateIn(item);
        item.DataBind();

        controlTemplateIndex++;
    }

    #endregion
}

