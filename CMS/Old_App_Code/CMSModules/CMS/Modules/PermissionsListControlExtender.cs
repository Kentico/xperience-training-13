using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Modules;
using CMS.UIControls;

[assembly: RegisterCustomClass("PermissionsListControlExtender", typeof(PermissionsListControlExtender))]

/// <summary>
/// Permission list control extender
/// </summary>
public class PermissionsListControlExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// Gets the current resource
    /// </summary>
    public ResourceInfo Resource
    {
        get
        {
            return Control.UIContext.EditedObjectParent as ResourceInfo;
        }
    }


    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        Control.OnExternalDataBound += OnExternalDataBound;

        // Show warning when module is not editable
        Control.Load += (sender, args) =>
        {
            if ((Resource != null) && (Resource.ResourceID > 0) && !Resource.IsEditable)
            {
                Control.ShowInformation(Control.GetString("resource.permissionsinstalledresourcewarning"));
            }
        };
    }


    /// <summary>
    /// OnExternalDataBound event handler
    /// </summary>
    private object OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            // Disable delete, move and clone options when module is not editable
            case "delete":
            case "moveup":
            case "movedown":
                ((CMSGridActionButton)sender).Enabled = (Resource != null) && (Resource.ResourceID > 0) && Resource.IsEditable;
                break;
            case "#objectmenu":
                ((CMSGridActionButton)sender).Visible = (Resource != null) && (Resource.ResourceID > 0) && Resource.IsEditable;
                break;
        }

        return parameter;
    }
}