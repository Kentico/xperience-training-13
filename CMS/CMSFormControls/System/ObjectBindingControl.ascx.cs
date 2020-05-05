using System;
using System.Data;

using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;


public partial class CMSFormControls_System_ObjectBindingControl : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return chkBindObject.Checked;
        }
        set
        {
            chkBindObject.Checked = ValidationHelper.GetBoolean(value, false);
        }
    }


    /// <summary>
    /// Binding object type.
    /// </summary>
    public string ObjectType
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ObjectType"), string.Empty);
        }
        set
        {
            SetValue("ObjectType", value);
        }
    }


    /// <summary>
    /// Target object ID.
    /// </summary>
    public string TargetObjectID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TargetObjectID"), string.Empty);
        }
        set
        {
            SetValue("TargetObjectID", value);
        }
    }


    /// <summary>
    /// Checkbox caption.
    /// </summary>
    public string Caption
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Caption"), string.Empty);
        }
        set
        {
            SetValue("Caption", value);
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            chkBindObject.Enabled = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        chkBindObject.Text = HTMLHelper.HTMLEncode(ContextResolver.ResolveMacros(Caption));

        if (Form != null)
        {
            Form.OnAfterSave += Form_OnAfterSave;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        int targetObjectID = ValidationHelper.GetInteger(ContextResolver.ResolveMacros(TargetObjectID), 0);

        if (targetObjectID > 0)
        {
            // Set the value for existing object
            if ((Form != null) && (!Form.IsInsertMode))
            {
                GeneralizedInfo obj = (BaseInfo)Form.Data;

                if ((obj != null))
                {
                    string objectType = Form.ResolveMacros(ObjectType);

                    GeneralizedInfo info = ModuleManager.GetReadOnlyObject(objectType);
                    if (info != null)
                    {
                        // Select proper column for target object
                        var ti = obj.TypeInfo;

                        string targetObjectColumn = (info.ColumnNames.IndexOf(ti.IDColumn) == 0) ? info.ColumnNames[1] : info.ColumnNames[0];

                        DataSet ds = info.GetData(null, ti.IDColumn + " = " + obj.ObjectID + " AND " + targetObjectColumn + " = " + targetObjectID, null, 1, null, false);

                        // Check the checkbox if binding exists
                        Value = !DataHelper.DataSourceIsEmpty(ds);
                    }
                }
            }
        }
        else
        {
            // Hide form control if target object ID is not set
            chkBindObject.Visible = false;
        }
    }


    private void Form_OnAfterSave(object sender, EventArgs e)
    {
        bool bindObjects = ValidationHelper.GetBoolean(Value, false);

        if ((bindObjects || !Form.IsInsertMode) && Visible)
        {
            GeneralizedInfo obj = (BaseInfo)Form.Data;

            if (obj != null)
            {
                try
                {
                    BaseInfo bindingObj = ModuleManager.GetObject(Form.ResolveMacros(ObjectType));
                    int targetObjectID = ValidationHelper.GetInteger(Form.ResolveMacros(TargetObjectID), 0);

                    var idColumn = obj.TypeInfo.IDColumn;

                    if ((bindingObj != null) && (bindingObj.ColumnNames.Count >= 2) && (bindingObj.ContainsColumn(idColumn)) && (targetObjectID > 0))
                    {
                        // Select proper column for target object
                        string targetObjectColumn = (bindingObj.ColumnNames.IndexOf(idColumn) == 0) ? bindingObj.ColumnNames[1] : bindingObj.ColumnNames[0];

                        bindingObj.SetValue(idColumn, obj.ObjectID);
                        bindingObj.SetValue(targetObjectColumn, targetObjectID);

                        if (bindObjects)
                        {
                            // Bind objects
                            bindingObj.Insert();
                        }
                        else
                        {
                            // Remove binding
                            bindingObj.Delete();
                        }
                    }
                    else
                    {
                        Service.Resolve<IEventLogService>().LogError("ObjectBinding", "BindObject", "Object " + obj.ObjectDisplayName + " cannot be bound.");
                    }
                }
                catch (Exception ex)
                {
                    Service.Resolve<IEventLogService>().LogException("ObjectBinding", "BindObject", ex);
                }
            }
        }
    }
}