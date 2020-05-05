using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;


public partial class CMSWebParts_Layouts_Wizard_WizardHeader : CMSAbstractWebPart
{
    #region "Variables"

    /// <summary>
    /// Flag if the controls were already loaded
    /// </summary>
    protected bool loaded = false;
   
    #endregion


    #region "Properties"

    /// <summary>
    /// Header text
    /// </summary>
    public string HeaderText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("HeaderText"), "Step {0} of {1}");
        }
        set
        {
            this.SetValue("HeaderText", value);
        }
    }


    /// <summary>
    /// Header icon
    /// </summary>
    public string Icon
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Icon"), "");
        }
        set
        {
            this.SetValue("Icon", value);
        }
    }


    /// <summary>
    /// Description text
    /// </summary>
    public string InfoText
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("InfoText"), "");
        }
        set
        {
            this.SetValue("InfoText", value);
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
            if (!RequestHelper.IsPostBack())
            {
                ReloadTexts(null, false);
            }
        }
    }


    /// <summary>
    /// Reloads the texts of the control
    /// </summary>
    /// <param name="e">Step event arguments</param>
    /// <param name="forceLoad">If true, the load is forced</param>
    protected void ReloadTexts(StepEventArgs e, bool forceLoad)
    {
        if (loaded && !forceLoad)
        {
            return;
        }
        loaded = true;

        string header = null;
        string info = null;
        string icon = null;

        int current = 1;
        int steps = 1;

        if (e != null)
        {
            // Use custom header/info/icon
            header = e["StepHeader"] as string;
            info = e["StepInfo"] as string;
            icon = e["StepIcon"] as string;
            
            current = e.CurrentStep;
            steps = e.Steps;
        }

        // Setup the default texts
        if (header == null)
        {
            header = HeaderText;
        }

        if (icon == null)
        {
            icon = Icon;
        }

        if (info == null)
        {
            info = InfoText;
        }

        // Format the header {0} = current step index, {1} = total number of steps
        header = String.Format(header, current, steps);
        info = String.Format(info, current, steps);
        icon = String.Format(icon, current, steps);

        this.ltlHeader.Text = header;
        this.ltlInfo.Text = info;
        this.imgIcon.ImageUrl = icon;
    }


    /// <summary>
    /// Fired when the step is loaded
    /// </summary>
    protected void StepLoaded(object sender, EventArgs e)
    {
        StepEventArgs args = (StepEventArgs)e;

        ReloadTexts(args, true);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Show / display components
        this.plcHeader.Visible = !String.IsNullOrEmpty(ltlHeader.Text);
        this.plcInfo.Visible = !String.IsNullOrEmpty(ltlInfo.Text);
        this.plcIcon.Visible = !String.IsNullOrEmpty(imgIcon.ImageUrl);
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



