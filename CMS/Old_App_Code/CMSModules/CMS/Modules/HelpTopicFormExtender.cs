using System;
using System.Linq;

using CMS;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;

[assembly: RegisterCustomClass("HelpTopicFormExtender", typeof(HelpTopicFormExtender))]

/// <summary>
/// Help topic form extender
/// </summary>
public class HelpTopicFormExtender : ControlExtender<UIForm>
{
    #region "Properties"

    /// <summary>
    /// Gets resource (module) ID.
    /// </summary>
    private int ResourceID
    {
        get
        {
            return QueryHelper.GetInteger("moduleid", 0);
        }
    }


    /// <summary>
    /// Gets edited help topic parent.
    /// </summary>
    private UIElementInfo ParentUIElement
    {
        get
        {
            return Control.UIContext.EditedObjectParent as UIElementInfo;
        }
    }

    #endregion


    #region "Public methods"

    public override void OnInit()
    {
        Control.OnAfterDataLoad += Control_OnAfterDataLoad;
    }

    #endregion


    #region "Private methods"

    private void Control_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Allow editing only help topics within selected module and only for custom UI elements (does not apply in development mode).
        if (ParentUIElement != null)
        {
            Control.Enabled = (!UIElementInfoProvider.AllowEditOnlyCurrentModule || ((ResourceID == ParentUIElement.ElementResourceID) && ParentUIElement.ElementIsCustom));
        }
        else
        {
            Control.Enabled = false;
        }
    }

    #endregion
        
}
