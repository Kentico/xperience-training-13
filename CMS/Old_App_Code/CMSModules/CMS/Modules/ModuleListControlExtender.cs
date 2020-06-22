using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Modules;
using CMS.UIControls;


[assembly: RegisterCustomClass("ModuleListControlExtender", typeof(ModuleListControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class ModuleListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnAction += OnAction;
        Control.OnExternalDataBound += Control_OnExternalDataBound;
    }


    /// <summary>
    /// OnExternalDataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="sourceName"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton button = null;
        switch (sourceName.ToLowerCSafe())
        {
            case "delete":
                string moduleName = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["ResourceName"], "");
                if (ResourceInfoProvider.IsSystemResource(moduleName))
                {
                    button = ((CMSGridActionButton)sender);
                    button.Enabled = false;
                }
                break;

            case "#objectmenu":
                moduleName = ValidationHelper.GetString(((DataRowView)((GridViewRow)parameter).DataItem).Row["ResourceName"], "");
                if (ResourceInfoProvider.IsSystemResource(moduleName))
                {
                    ((CMSGridActionButton)sender).Visible = false;
                }
                break;
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw the event</param>
    /// <param name="actionArgument">ID (value of Primary key) of the corresponding data row</param>
    protected void OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            int resourceId = ValidationHelper.GetInteger(actionArgument, 0);

            // Check if module has any classes (including page types...)
            var classes = DataClassInfoProvider.GetClasses().Where("ClassResourceID", QueryOperator.Equals, resourceId);
            var settings = SettingsCategoryInfo.Provider.Get().Where("CategoryResourceID", QueryOperator.Equals, resourceId);
            var elements = UIElementInfo.Provider.Get().Where("ElementResourceID", QueryOperator.Equals, resourceId);

            if (!classes.HasResults() && !settings.HasResults() && !elements.HasResults())
            {
                ResourceInfo.Provider.Delete(ResourceInfo.Provider.Get(resourceId));
            }
            else
            {
                Control.ShowError(Control.GetString("cms_resource.deleteerror"));
            }
        }
    }
}