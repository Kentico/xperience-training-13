using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_Wizard_Wizard : CMSAbstractLayoutWebPart
{
    #region "Variables"

    /// <summary>
    /// Current step index
    /// </summary>
    protected int currentStep = 0;

    /// <summary>
    /// Step container
    /// </summary>
    protected PlaceHolder mStepContainer = null;

    /// <summary>
    /// Step headers
    /// </summary>
    protected string[] headers = null;

    /// <summary>
    /// Step icons
    /// </summary>
    protected string[] icons = null;

    /// <summary>
    /// Step descriptions
    /// </summary>
    protected string[] descriptions = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// First row width
    /// </summary>
    public string Width
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Width"), "");
        }
        set
        {
            this.SetValue("Width", value);
        }
    }


    /// <summary>
    /// Number of the steps
    /// </summary>
    public int Steps
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("Steps"), 3);
        }
        set
        {
            this.SetValue("Steps", value);
        }
    }


    /// <summary>
    /// Active step number
    /// </summary>
    public int ActiveStep
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("ActiveStep"), 1);
        }
        set
        {
            this.SetValue("ActiveStep", value);
        }
    }


    /// <summary>
    /// Show header
    /// </summary>
    public bool ShowHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowHeader"), true);
        }
        set
        {
            this.SetValue("ShowHeader", value);
        }
    }


    /// <summary>
    /// Use automatic header
    /// </summary>
    public bool UseAutomaticHeader
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("UseAutomaticHeader"), true);
        }
        set
        {
            this.SetValue("UseAutomaticHeader", value);
        }
    }


    /// <summary>
    /// First row height
    /// </summary>
    public string HeaderHeight
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("HeaderHeight"), "");
        }
        set
        {
            this.SetValue("HeaderHeight", value);
        }
    }


    /// <summary>
    /// First row CSS class
    /// </summary>
    public string HeaderCSSClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("HeaderCSSClass"), "");
        }
        set
        {
            this.SetValue("HeaderCSSClass", value);
        }
    }


    /// <summary>
    /// Step headers
    /// </summary>
    public string StepHeaders
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("StepHeaders"), "");
        }
        set
        {
            this.SetValue("StepHeaders", value);
        }
    }


    /// <summary>
    /// Custom step icons
    /// </summary>
    public string StepIcons
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("StepIcons"), "");
        }
        set
        {
            this.SetValue("StepIcons", value);
        }
    }


    /// <summary>
    /// Custom step descriptions
    /// </summary>
    public string StepDescriptions
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("StepDescriptions"), "");
        }
        set
        {
            this.SetValue("StepDescriptions", value);
        }
    }


    /// <summary>
    /// Second row height
    /// </summary>
    public string ContentHeight
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ContentHeight"), "");
        }
        set
        {
            this.SetValue("ContentHeight", value);
        }
    }


    /// <summary>
    /// Second row CSS class
    /// </summary>
    public string ContentCSSClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ContentCSSClass"), "");
        }
        set
        {
            this.SetValue("ContentCSSClass", value);
        }
    }


    /// <summary>
    /// Show footer
    /// </summary>
    public bool ShowFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("ShowFooter"), true);
        }
        set
        {
            this.SetValue("ShowFooter", value);
        }
    }


    /// <summary>
    /// Use automatic footer
    /// </summary>
    public bool UseAutomaticFooter
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("UseAutomaticFooter"), true);
        }
        set
        {
            this.SetValue("UseAutomaticFooter", value);
        }
    }


    /// <summary>
    /// Third row height
    /// </summary>
    public string FooterHeight
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("FooterHeight"), "");
        }
        set
        {
            this.SetValue("FooterHeight", value);
        }
    }


    /// <summary>
    /// Third row CSS class
    /// </summary>
    public string FooterCSSClass
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("FooterCSSClass"), "");
        }
        set
        {
            this.SetValue("FooterCSSClass", value);
        }
    }


    /// <summary>
    /// Current step index, indexed from 1
    /// </summary>
    public int CurrentStepIndex
    {
        get
        {
            if (currentStep == 0)
            {
                currentStep = ActiveStep;

                // In design mode, take the default active step from request
                if (RequestHelper.IsPostBack())
                {
                    currentStep = ValidationHelper.GetInteger(Request.Form[UniqueID + "$current"], currentStep);
                }
                else if (IsDesign)
                {
                    currentStep = GetLastCurrentItem(currentStep);
                }
            }

            // Ensure the correct step index
            if (currentStep > Steps)
            {
                currentStep = Steps;
            }
            if (currentStep < 1)
            {
                currentStep = 1;
            }

            return currentStep;
        }
        set
        {
            currentStep = value;
        }
    }


    /// <summary>
    /// Redirect to URL when finished
    /// </summary>
    public string FinishRedirectUrl
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("FinishRedirectUrl"), "");
        }
        set
        {
            this.SetValue("FinishRedirectUrl", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Register the events
            ComponentEvents.RequestEvents.RegisterForComponentEvent(this.WebPartID, ComponentEvents.NEXT, NextStep);
            ComponentEvents.RequestEvents.RegisterForComponentEvent(this.WebPartID, ComponentEvents.PREVIOUS, PreviousStep);
            ComponentEvents.RequestEvents.RegisterForComponentEvent(this.WebPartID, ComponentEvents.FINISH, Finish);
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Load the initial step
        if (!RequestHelper.IsPostBack())
        {
            LoadStep(null, CurrentStepIndex);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        hdnCurrent.Value = CurrentStepIndex.ToString();
        hdnItems.Value = Steps.ToString();

        hdnCurrent.ID = "current";
        hdnItems.ID = "items";

        // Ensure the current item acknowledged by Javascript
        if (IsDesign)
        {
            RegisterChangeLayoutItemScript(0);
        }
    }


    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        mLayoutContainer = plcL;

        // Prepare the main markup
        StartLayout();

        Append("<div class=\"Wizard\"");

        // Width
        string width = Width;
        if (!String.IsNullOrEmpty(width))
        {
            Append(" style=\"width: ", width, "\"");
        }

        if (IsDesign)
        {
            Append(" id=\"", ShortClientID, "_env\">");

            Append("<table class=\"LayoutTable\" cellspacing=\"0\" style=\"width: 100%;\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\" colspan=\"2\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td id=\"", ShortClientID, "_info\" style=\"width: 100%;\">");
        }
        else
        {
            Append(">");
        }

        // Add the wizard sections (header, content, footer)
        if (ShowHeader)
        {
            CreateSection("Header", false, UseAutomaticHeader, "~/CMSWebParts/Layouts/Wizard/WizardHeader.ascx");
        }

        // Steps
        CreateSection("Content", true, false, null);

        // Footer
        if (ShowFooter)
        {
            CreateSection("Footer", false, UseAutomaticFooter, "~/CMSWebParts/Layouts/Wizard/WizardButtons.ascx");
        }

        if (IsDesign)
        {
            Append("</td>");

            if (AllowDesignMode)
            {
                // Width resizer
                Append("<td class=\"HorizontalResizer\" onmousedown=\"" + GetHorizontalResizerScript("env", "Width", false, "info") + " return false;\">&nbsp;</td>");
            }

            Append("</tr>");

            // Footer
            if (AllowDesignMode)
            {
                Append("<tr><td class=\"LayoutFooter cms-bootstrap\" colspan=\"2\"><div class=\"LayoutFooterContent\">");

                Append("<div class=\"LayoutLeftActions\">");

                // Steps actions
                AppendAddAction(ResHelper.GetString("Layout.AddStep"), "Steps");
                if (Steps > 1)
                {
                    AppendRemoveAction(ResHelper.GetString("Layout.RemoveStep"), "Steps");
                }

                Append("</div><div class=\"LayoutRightActions\">");

                // Previous, next step
                AppendPreviousItemAction(ResHelper.GetString("Layout.PreviousStep"));
                AppendNextItemAction(ResHelper.GetString("Layout.NextStep"));

                Append("</div><div class=\"ClearBoth\"></div></div></td></tr>");
            }

            Append("</table>");
        }

        Append("</div>");

        // Finalize
        FinishLayout();
    }


    /// <summary>
    /// Creates the section within the wizard
    /// </summary>
    /// <param name="name">Section name</param>
    /// <param name="renderSteps">If true, this section renders steps</param>
    /// <param name="loadControl">If true, the given control is rendered instead of the web part zone</param>
    /// <param name="path">Path to the control to load</param>
    private void CreateSection(string name, bool renderSteps, bool loadControl, string path)
    {
        // Set the property name
        string heightPropertyName = name + "Height";

        string height = ValidationHelper.GetString(GetValue(heightPropertyName), "");

        string rowId = "section_" + name;

        if (IsDesign)
        {
            Append("<table cellspacing=\"0\" cellpadding=\"0\" style=\"width: 100%\"><tr><td id=\"", ShortClientID, "_", rowId, "\"");
        }
        else
        {
            Append("<div");
        }

        string style = "vertical-align: top;";

        // Section height
        if (!String.IsNullOrEmpty(height))
        {
            style += "height: " + height + ";";
        }

        // Append style
        if (!String.IsNullOrEmpty(style))
        {
            Append(" style=\"", style, "\"");
        }

        // Cell class
        string thisRowClass = ValidationHelper.GetString(GetValue(name + "CSSClass"), "");
        if (!String.IsNullOrEmpty(thisRowClass))
        {
            Append(" class=\"", thisRowClass, "\"");
        }
        else
        {
            Append(" class=\"Wizard", name , "\"");
        }

        Append(">");

        // Add the zone
        if (renderSteps)
        {
            int currentStep = CurrentStepIndex;

            if (IsDesign)
            {
                // Render all steps
                for (int i = 1; i <= Steps; i++)
                {
                    AppendItemStart(currentStep, i);

                    // Render the step
                    RenderStep(i, mStepContainer);

                    AppendItemEnd();
                }
            }
            else
            {
                PortalContext.EditableControlsHidden = true;

                // Prepare the container for steps
                mStepContainer = new PlaceHolder();
                mStepContainer.ID = "plcStep";

                AddControl(mStepContainer);

                // Render only current step
                RenderStep(currentStep, mStepContainer);
            }
        }
        else
        {
            if (loadControl)
            {
                // Load the user control instead of zone
                AddControl(path);
            }
            else
            {
                // Render the section zone
                AddZone(ID + "_" + name, "[" + name + "]");
            }
        }

        if (IsDesign)
        {
            Append("</td></tr>");

            // Resizers
            if (AllowDesignMode)
            {
                Append("<tr><td class=\"VerticalResizer\" onmousedown=\"", GetVerticalResizerScript(rowId, heightPropertyName), " return false;\">&nbsp;</td></tr>");
            }

            Append("</table>");
        }
        else
        {
            Append("</div>");
        }
    }


    /// <summary>
    /// Renders a single wizard step
    /// </summary>
    /// <param name="index">Step index</param>
    /// <param name="container">Step container</param>
    private CMSWebPartZone RenderStep(int index, Control container)
    {
        return AddZone(ID + "_" + index, "[Step " + index + "]", container);
    }


    /// <summary>
    /// Changes the current step
    /// </summary>
    /// <param name="offset">Offset by which the step should move</param>
    /// <param name="raiseEvents">Indicates whether validate and finish events should be raised</param>
    protected void ChangeStep(int offset, bool raiseEvents = true)
    {
        if (offset == 0)
        {
            return;
        }

        int steps = Steps;
        int original = CurrentStepIndex;

        // Fire the event to validate current step
        var args = new StepEventArgs(steps, original);

        if (raiseEvents)
        {
            ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.VALIDATE_STEP);

            if (args.CancelEvent)
            {
                // Cancel if not validated
                return;
            }

            // Fire the event to finish current step
            ComponentEvents.RequestEvents.RaiseEvent(this, args, ((offset > 0) ? ComponentEvents.FINISH_STEP : ComponentEvents.CANCEL_STEP));

            if (args.CancelEvent)
            {
                // Cancel if not finished
                return;
            }
        }

        // Reevaluate the current item index
        int current = original + offset;
        if (current < 1)
        {
            current = 1;
        }
        if (current > Steps)
        {
            current = Steps;
        }

        if (IsDesign)
        {
            RegisterChangeLayoutItemScript(offset);

            // Load the step
            LoadStep(args, current);
        }
        else
        {
            // Move the step by the given index
            CurrentStepIndex = current;

            // Change the step
            if (original != current)
            {
                mStepContainer.Controls.Clear();

                // Create the zone
                var zone = RenderStep(current, NO_CONTAINER);

                zone.Page = this.Page;

                // Load the zone content
                var zoneInstance = this.PagePlaceholder.TemplateInstance.GetZone(ID + "_" + current);

                zone.LoadWebParts(zoneInstance);
                zone.LoadWebPartsContent(false);
                zone.OnContentLoaded();
                zone.LoadRegionsContent(true);

                // Add to the controls collection so it catches up the control cycle
                mStepContainer.Controls.Add(zone);

                // Load the step
                LoadStep(args, current);
            }
        }
    }


    /// <summary>
    /// Loads the given step
    /// </summary>
    /// <param name="args">Step arguments</param>
    /// <param name="current">Step index</param>
    private void LoadStep(StepEventArgs args, int current)
    {
        // Ensure the event arguments
        if (args == null)
        {
            args = new StepEventArgs(this.Steps, current);
        }

        args.FinalStepUrl = FinishRedirectUrl;
        args.CurrentStep = current;

        // Fire the event to finish current step
        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.LOAD_STEP);

        // Skip the current step if requested
        if (args.Skip)
        {
            ChangeStep(1);
        }
        else
        {
            // Use the custom step header if set
            string header = GetHeader(current);
            if (!String.IsNullOrEmpty(header))
            {
                args["StepHeader"] = header;
            }

            // Use the custom step icon if set
            string icon = GetIcon(current);
            if (!String.IsNullOrEmpty(icon))
            {
                args["StepIcon"] = icon;
            }

            // Use the custom step description if set
            string description = GetDescription(current);
            if (!String.IsNullOrEmpty(description))
            {
                args["StepInfo"] = description;
            }

            // Handle the next button display
            string finishUrl = String.IsNullOrEmpty(args.FinalStepUrl) ? FinishRedirectUrl : args.FinalStepUrl;
            if ((args.Steps == current) && String.IsNullOrEmpty(finishUrl))
            {
                args.HideNextButton = true;
            }

            // Fire the event to notify that the step was loaded
            ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.STEP_LOADED);
        }
    }


    /// <summary> 
    /// Moves the wizard to the next step
    /// </summary>
    protected void NextStep(object sender, EventArgs e)
    {
        ChangeStep(1);
    }


    /// <summary>
    /// Moves the wizard to the next step
    /// </summary>
    protected void PreviousStep(object sender, EventArgs e)
    {
        ChangeStep(-1);
    }


    /// <summary>
    /// Finishes the wizard
    /// </summary>
    protected void Finish(object sender, EventArgs e)
    {
        // Fire the event to validate current step
        var args = new StepEventArgs(Steps, CurrentStepIndex);

        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.VALIDATE_STEP);

        if (args.CancelEvent)
        {
            // Cancel if not validated
            return;
        }

        // Fire the event to finish current step
        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.FINISH_STEP);

        if (args.CancelEvent)
        {
            // Cancel if not finished
            return;
        }

        // Redirect to the given URL
        string url = String.IsNullOrEmpty(args.FinalStepUrl) ? FinishRedirectUrl : args.FinalStepUrl;
        if (!String.IsNullOrEmpty(url))
        {
            URLHelper.Redirect(UrlResolver.ResolveUrl(url));
        }
    }


    /// <summary>
    /// Gets the header for the given step
    /// </summary>
    /// <param name="stepIndex">Step index</param>
    protected string GetHeader(int stepIndex)
    {
        if (headers == null)
        {
            headers = this.StepHeaders.Split('\n');
        }

        // Get the header from the list
        stepIndex--;
        if (headers.Length <= stepIndex)
        {
            return null;
        }
        return headers[stepIndex].Trim();
    }


    /// <summary>
    /// Gets the icon for the given step
    /// </summary>
    /// <param name="stepIndex">Step index</param>
    protected string GetIcon(int stepIndex)
    {
        if (icons == null)
        {
            icons = this.StepIcons.Split('\n');
        }

        // Get the icon from the list
        stepIndex--;
        if (icons.Length <= stepIndex)
        {
            return null;
        }
        return icons[stepIndex].Trim();
    }


    /// <summary>
    /// Gets the description for the given step
    /// </summary>
    /// <param name="stepIndex">Step index</param>
    protected string GetDescription(int stepIndex)
    {
        if (descriptions == null)
        {
            descriptions = this.StepDescriptions.Split('\n');
        }

        // Get the description from the list
        stepIndex--;
        if (descriptions.Length <= stepIndex)
        {
            return null;
        }
        return descriptions[stepIndex].Trim();
    }

    #endregion
}

