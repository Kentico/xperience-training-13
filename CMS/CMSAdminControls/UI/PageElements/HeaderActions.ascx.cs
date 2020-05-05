using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSAdminControls_UI_PageElements_HeaderActions : HeaderActions, IPostBackEventHandler
{
    #region "Properties"

    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ComponentName"], base.ComponentName);
        }
        set
        {
            base.ComponentName = value;
            ViewState["ComponentName"] = value;
        }
    }


    /// <summary>
    /// Update panel
    /// </summary>
    public override CMSUpdatePanel UpdatePanel
    {
        get
        {
            return pnlUp;
        }
    }


    /// <summary>
    /// Panel for actions.
    /// </summary>
    protected override Panel ActionsPanel
    {
        get
        {
            return pnlActions;
        }
    }


    /// <summary>
    /// Panel for additional controls.
    /// </summary>
    protected override Panel AdditionalControlsPanel
    {
        get
        {
            return pnlAdditionalControls;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page init.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        ReloadAdditionalControls();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();

        // Hide processed base buttons
        ProcessedBaseButtons.ForEach(b =>
        {
            b.EnableViewState = false;
            b.Visible = false;
        });

        // Add shadow below header actions
        ScriptHelper.RegisterModule(Page, "CMS/HeaderShadow");

        // Hide menu if not visible
        plcMenu.Visible = HasAnyVisibleAction();
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if ((Context != null) && RenderContainer)
        {
            string cssClass = (UseBasicStyles || IsLiveSite) ? "PageHeaderLine" : "cms-edit-menu";
            writer.Write("<div class\"{0}\">", cssClass);
        }

        base.Render(writer);

        if ((Context != null) && RenderContainer)
        {
            writer.Write("</div>");
        }

        if (ShortcutAction != null)
        {
            PostBackOptions opt = new PostBackOptions(this, ShortcutAction.CommandArgument)
            {
                PerformValidation = true,
                ValidationGroup = ShortcutAction.ValidationGroup
            };

            Page.ClientScript.RegisterForEventValidation(opt);
        }
    }


    /// <summary>
    /// Indicates if given action can be added to the actions panel.
    /// </summary>
    /// <param name="action">Action to be checked</param>
    protected override bool IsActionVisible(HeaderAction action)
    {
        if (!base.IsActionVisible(action))
        {
            return false;
        }

        var formButton = action.BaseButton as FormSubmitButton;
        return (formButton == null) || formButton.RegisterHeaderAction;
    }

    #endregion


    #region "IPostBackEventHandler Members"

    /// <summary>
    /// Raise post-back event
    /// </summary>
    /// <param name="eventArgument">Argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument))
        {
            string[] argValues = eventArgument.Split(';');
            if (argValues.Length == 2)
            {
                CommandEventArgs args = new CommandEventArgs(argValues[0], argValues[1]);
                RaiseActionPerformed(this, args);

                // Update parent update panel in conditional mode
                var up = ControlsHelper.GetUpdatePanel(this);
                if ((up != null) && (up.UpdateMode == UpdatePanelUpdateMode.Conditional))
                {
                    up.Update();
                }
            }
        }
    }

    #endregion
}
