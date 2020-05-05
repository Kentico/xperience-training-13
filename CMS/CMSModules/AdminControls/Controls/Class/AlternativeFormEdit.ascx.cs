using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_AlternativeFormEdit : CMSUserControl
{
    #region "Properties"

    /// <summary>
    /// OnBeforeSave event.
    /// </summary>
    public event EventHandler OnBeforeSave
    {
        add
        {
            form.OnBeforeSave += value;
        }
        remove
        {
            form.OnBeforeSave -= value;
        }
    }


    /// <summary>
    /// OnAfterSave event.
    /// </summary>
    public event EventHandler OnAfterSave
    {
        add
        {
            form.OnAfterSave += value;
        }
        remove
        {
            form.OnAfterSave -= value;
        }
    }


    /// <summary>
    /// The URL to which the engine should redirect after creation of the new object.
    /// </summary>
    public string RedirectUrlAfterCreate
    {
        get
        {
            return form.RedirectUrlAfterCreate;
        }
        set
        {
            form.RedirectUrlAfterCreate = value;
        }
    }


    /// <summary>
    /// Edited object.
    /// </summary>
    public new AlternativeFormInfo EditedObject
    {
        get
        {
            return (AlternativeFormInfo)form.EditedObject;
        }
        set
        {
            form.EditedObject = value;
        }
    }


    /// <summary>
    /// Indicates if special panel for User class is visible.
    /// </summary>
    public bool ShowCombineUsersSettings
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if control is enabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return form.Enabled;
        }
        set
        {
            form.Enabled = value;
            // Turn off automatic enabling
            form.EnabledByLockState = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        OnAfterSave += form_OnAfterSave;
        OnBeforeSave += form_OnBeforeSave;

        if (ShowCombineUsersSettings)
        {
            pnlCombineUserSettings.Visible = true;

            if (!form.IsInsertMode)
            {
                chkCombineUserSettings.Enabled = false;
                chkCombineUserSettings.Checked = EditedObject.FormCoupledClassID > 0;
            }
        }
    }


    private void form_OnAfterSave(object s, EventArgs ea)
    {
        ScriptHelper.RefreshTabHeader(Page);
    }


    protected void form_OnBeforeSave(object sender, EventArgs e)
    {
        if (chkCombineUserSettings.Checked)
        {
            // Set coupled class ID
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(UserSettingsInfo.OBJECT_TYPE);
            if (dci != null)
            {
                EditedObject.FormCoupledClassID = dci.ClassID;
            }
        }

        if (form.IsInsertMode)
        {
            // Mark alternative form as custom if module is not in development and development mode is off
            ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(QueryHelper.GetInteger("moduleid", 0));
            form.Data["FormIsCustom"] = !SystemContext.DevelopmentMode && ((resource != null) && !resource.ResourceIsInDevelopment);
        }
    }

    #endregion
}
