using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSWebParts_Layouts_Wizard_WizardButtons : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Show back button
    /// </summary>
    public bool BackEnabled
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("BackEnabled"), true);
        }
        set
        {
            this.SetValue("BackEnabled", value);
            btnBack.Visible = value;
        }
    }


    /// <summary>
    /// Show back as button
    /// </summary>
    public bool BackShowAsButton
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("BackShowAsButton"), btnBack.ShowAsButton);
        }
        set
        {
            this.SetValue("BackShowAsButton", value);
            btnBack.ShowAsButton = value;
        }
    }


    /// <summary>
    /// Back button text
    /// </summary>
    public string BackText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("BackText"), btnBack.LinkText);
        }
        set
        {
            this.SetValue("BackText", value);
            btnBack.LinkText = value;
        }
    }


    /// <summary>
    /// Back button image
    /// </summary>
    public string BackImage
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("BackImage"), btnBack.ImageUrl);
        }
        set
        {
            this.SetValue("BackImage", value);
            btnBack.ImageUrl = value;
        }
    }


    /// <summary>
    /// Show next as button
    /// </summary>
    public bool NextShowAsButton
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("NextShowAsButton"), btnNext.ShowAsButton);
        }
        set
        {
            this.SetValue("NextShowAsButton", value);
            btnNext.ShowAsButton = value;
        }
    }


    /// <summary>
    /// Next button text
    /// </summary>
    public string NextText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("NextText"), btnNext.LinkText);
        }
        set
        {
            this.SetValue("NextText", value);
            btnNext.LinkText = value;
        }
    }


    /// <summary>
    /// Next button image
    /// </summary>
    public string NextImage
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("NextImage"), btnNext.ImageUrl);
        }
        set
        {
            this.SetValue("NextImage", value);
            btnNext.ImageUrl = value;
        }
    }


    /// <summary>
    /// Use extra button for finish
    /// </summary>
    public bool FinishExtraButton
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("FinishExtraButton"), true);
        }
        set
        {
            this.SetValue("FinishExtraButton", value);
            btnFinish.Visible = value;
        }
    }


    /// <summary>
    /// Show finish as button
    /// </summary>
    public bool FinishShowAsButton
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("FinishShowAsButton"), btnFinish.ShowAsButton);
        }
        set
        {
            this.SetValue("FinishShowAsButton", value);
            btnFinish.ShowAsButton = value;
        }
    }


    /// <summary>
    /// Finish button text
    /// </summary>
    public string FinishText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("FinishText"), btnFinish.LinkText);
        }
        set
        {
            this.SetValue("FinishText", value);
            btnFinish.LinkText = value;
        }
    }


    /// <summary>
    /// Finish button image
    /// </summary>
    public string FinishImage
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("FinishImage"), btnFinish.ImageUrl);
        }
        set
        {
            this.SetValue("FinishImage", value);
            btnFinish.ImageUrl = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register the events
        ComponentEvents.RequestEvents.RegisterForComponentEvent(this.WebPartID, ComponentEvents.STEP_LOADED, StepLoaded);
    }


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
            // Load the buttons
            LoadButton(btnBack, "Back");
            btnBack.LinkEvent = ComponentEvents.PREVIOUS;

            LoadButton(btnNext, "Next");
            btnNext.LinkEvent = ComponentEvents.NEXT;

            LoadButton(btnFinish, "Finish");
            btnFinish.LinkEvent = ComponentEvents.FINISH;
        }
    }


    /// <summary>
    /// Loads the properties to the given button
    /// </summary>
    /// <param name="prefix">Property prefix</param>
    protected void LoadButton(UniButton btn, string prefix)
    {
        btn.ShowAsButton = ValidationHelper.GetBoolean(GetValue(prefix + "ShowAsButton"), btn.ShowAsButton);
        btn.LinkText = GetStringValue(prefix + "Text", btn.LinkText);
        btn.ImageUrl = GetStringValue(prefix + "Image", btn.ImageUrl);

        string cssClass = GetStringValue(prefix + "CssClass", btn.CssClass);
        if (!String.IsNullOrEmpty(cssClass))
        {
            btn.CssClass = cssClass;
        }
    }


    /// <summary>
    /// Fired when the step is loaded
    /// </summary>
    protected void StepLoaded(object sender, EventArgs e)
    {
        StepEventArgs args = (StepEventArgs)e;

        btnBack.Visible = (this.BackEnabled && (args.CurrentStep > 1) && !args.HideBackButton);
        btnFinish.Visible = (this.FinishExtraButton && (args.CurrentStep >= args.Steps) && !args.HideNextButton);
        btnNext.Visible = ((args.CurrentStep < args.Steps) && !args.HideNextButton);
    }


    /// <summary>
    /// Reloads the control data
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}



