using System;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[assembly: RegisterCustomClass("HelpTopicUniGridExtender", typeof(HelpTopicUniGridExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class HelpTopicUniGridExtender : ControlExtender<UniGrid>
{
    #region "Properties"

    /// <summary>
    /// Resource (module) ID.
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


    /// <summary>
    /// Gets editing enabled state.
    /// Editing is allowed only for help topics within selected module and only for custom UI elements (does not apply in development mode).
    /// </summary>
    private bool EditingEnabled
    {
        get
        {
            return (ParentUIElement != null) && (!UIElementInfoProvider.AllowEditOnlyCurrentModule || ((ResourceID == ParentUIElement.ElementResourceID) && ParentUIElement.ElementIsCustom));
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += Control_OnExternalDataBound;
        Control.ShowObjectMenu = EditingEnabled;
        Control.HeaderActions.Enabled = EditingEnabled;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Handles external data bound.
    /// Renders link to help page and enables/disables grid editing buttons.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "helptopiclink":
                {
                    string linkUrl = (string)parameter;
                    linkUrl = DocumentationHelper.GetDocumentationTopicUrl(linkUrl);

                    return String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", linkUrl, HTMLHelper.HTMLEncode(linkUrl));
                }
            case "delete_modify":
            case "move_modify":
                CMSGridActionButton button = (CMSGridActionButton)sender;
                button.Enabled = EditingEnabled;
                break;
        }

        return parameter;
    }

    #endregion

}