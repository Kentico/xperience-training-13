using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;

public partial class CMSWebParts_DocumentWizard_DocumentWizardStepAction : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Sets the step action type.
    /// </summary>
    public string ActionType
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ActionType"), String.Empty);
        }
        set
        {
            this.SetValue("ActionType", value);
        }
    }


    /// <summary>
    /// Sets the action condition. If the condition is true the selected step action is performed..
    /// </summary>
    public string ActionCondition
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ActionCondition"), "");
        }
        set
        {
            this.SetValue("ActionCondition", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Step loaded event
    /// </summary>
    protected override void StepLoaded(object sender, StepEventArgs e)
    {
        base.StepLoaded(sender, e);

        if (!StopProcessing)
        {
            // Check whether condition is defined
            if (!String.IsNullOrEmpty(ActionCondition))
            {
                // Check condition value
                var res = ContextResolver.ResolveMacroExpression(ActionCondition, true);
                if ((res != null) && ValidationHelper.GetBoolean(res.Result, false))
                {
                    // Ensure action
                    switch (ActionType.ToLowerCSafe())
                    {
                        // Skip
                        case "skip":
                            e["RaiseEvents"] = ValidationHelper.GetBoolean(GetValue("ValidateSkip"), false);
                            e.Skip = true;
                            break;

                        //Next
                        case "next":
                            ComponentEvents.RequestEvents.RaiseComponentEvent(this, e, "PageWizardManager", ComponentEvents.NEXT);
                            break;

                        // Previous
                        case "previous":
                            ComponentEvents.RequestEvents.RaiseComponentEvent(this, e, "PageWizardManager", ComponentEvents.PREVIOUS);
                            break;
                    }
                }
            }
        }
    }

    #endregion
}



