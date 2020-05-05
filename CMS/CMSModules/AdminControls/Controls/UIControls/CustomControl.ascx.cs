using System;

using CMS.Base.Web.UI;

using System.Linq;
using System.Web.UI;

using CMS.EventLog;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_UIControls_CustomControl : CMSAbstractUIWebpart
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the path of the user control.
    /// </summary>
    public string ControlPath
    {
        get
        {
            return GetStringContextValue("ControlPath");
        }
        set
        {
            SetValue("ControlPath", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
        }
    }


    /// <summary>
    /// Loads the user control.
    /// </summary>
    protected void LoadUserControl()
    {
        if (!string.IsNullOrEmpty(ControlPath))
        {
            try
            {
                Control ctrl = Page.LoadUserControl(ControlPath);
                ctrl.ID = "userControlElem";
                Controls.Add(ctrl);
            }
            catch (Exception ex)
            {
                lblError.Text = "[" + ID + "] " + GetString("WebPartUserControl.ErrorLoad") + ": " + ex.Message;
                lblError.ToolTip = EventLogProvider.GetExceptionLogMessage(ex);
                lblError.Visible = true;
            }
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // The control must load after OnInit to properly load its viewstate
        this.LoadUserControl();
    }

    #endregion
}
