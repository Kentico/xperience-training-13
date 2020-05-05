using System;
using System.Linq;

using CMS;
using CMS.Base.Web.UI;
using CMS.Modules;
using CMS.PortalEngine.Web.UI;


[assembly: RegisterCustomClass("PermissionEditControlExtender", typeof(PermissionEditControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class PermissionEditControlExtender : ControlExtender<UIForm>
{
    /// <summary>
    /// Gets the edited permission.
    /// </summary>
    public PermissionNameInfo Permission
    {
        get
        {
            return Control.UIContext.EditedObject as PermissionNameInfo;
        }
    }


    /// <summary>
    /// Gets the parent resource of the edited permission.
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
        Control.OnAfterSave += (sender, args) =>
        {
            UpdatePermission(Permission);

            // Clear resources permission list
            if (Resource != null)
            {
                Resource.PermissionNames = null;
            }
        };

        Control.OnBeforeRedirect += (sender, args) => UpdatePermission(Permission);

        // Disable editing when module is not editable
        Control.Load += (sender, args) =>
        {
            if ((Resource != null) && (Resource.ResourceID > 0) && !Resource.IsEditable)
            {
                Control.SubmitButton.Enabled = Control.Enabled = false;
                Control.ShowInformation(Control.GetString("resource.installedresourcewarning"));
            }
        };
    }


    /// <summary>
    /// Updates the specified permission.
    /// </summary>
    private void UpdatePermission(PermissionNameInfo permission)
    {
        if (permission == null)
        {
            return;
        }

        permission.ClassId = 0;
    }
}