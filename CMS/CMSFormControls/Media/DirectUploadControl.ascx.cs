using System;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSFormControls_Media_DirectUploadControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return directUpload.Enabled;
        }
        set
        {
            directUpload.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return directUpload.Value;
        }
        set
        {
            directUpload.Value = value;
        }
    }


    /// <summary>
    /// Field info object.
    /// </summary>
    public override FormFieldInfo FieldInfo
    {
        get
        {
            return base.FieldInfo;
        }
        set
        {
            base.FieldInfo = value;
            directUpload.FieldInfo = value;
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return directUpload.IsLiveSite;
        }
        set
        {
            directUpload.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (FieldInfo != null)
        {
            directUpload.ID = FieldInfo.Name;
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        directUpload.CheckPermissions = false;
        if (FieldInfo != null)
        {
            directUpload.GUIDColumnName = FieldInfo.Name;
            directUpload.AllowDelete = FieldInfo.AllowEmpty;
        }

        // Set allowed extensions
        string extensions = ValidationHelper.GetString(GetValue("extensions"), null);
        string allowedExtensions = ValidationHelper.GetString(GetValue("allowed_extensions"), null);
        if (extensions == "custom")
        {
            directUpload.AllowedExtensions = !String.IsNullOrEmpty(allowedExtensions) ? allowedExtensions : "";
        }

        // Set image auto resize configuration
        if (FieldInfo != null)
        {
            int width;
            int height;
            int maxSideSize;
            ImageHelper.GetAutoResizeDimensions(FieldInfo.Settings, SiteContext.CurrentSiteName, out width, out height, out maxSideSize);
            directUpload.ResizeToWidth = width;
            directUpload.ResizeToHeight = height;
            directUpload.ResizeToMaxSideSize = maxSideSize;
        }

        // Set control properties from parent Form
        if (Form != null)
        {
            directUpload.Form = Form;

            directUpload.OnUploadFile += Form.RaiseOnUploadFile;
            directUpload.OnDeleteFile += Form.RaiseOnDeleteFile;

            // Get node
            TreeNode node = (TreeNode)Form.EditedObject;

            // Insert mode
            if (Form.IsInsertMode)
            {
                directUpload.FormGUID = Form.FormGUID;
                var parent = Form.ParentObject as TreeNode;
                if (parent != null)
                {
                    directUpload.NodeParentNodeID = parent.NodeID;
                }
                else if (node != null)
                {
                    directUpload.NodeParentNodeID = node.NodeParentID;
                }

                if (node != null)
                {
                    directUpload.NodeClassName = node.NodeClassName;
                }
            }
            // Editing existing node
            else if (node != null)
            {
                // Set appropriate control settings
                directUpload.DocumentID = node.DocumentID;
                directUpload.NodeParentNodeID = node.NodeParentID;
                directUpload.NodeClassName = node.NodeClassName;
            }
        }

        // Set style properties of control
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            directUpload.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            directUpload.CssClass = CssClass;
            CssClass = null;
        }

        CheckFieldEmptiness = false;
    }


    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        // Check empty field
        if ((FieldInfo != null) && !FieldInfo.AllowEmpty && ((Form == null) || Form.CheckFieldEmptiness))
        {
            string value = ValidationHelper.GetString(directUpload.Value, string.Empty).Trim();
            if ((String.IsNullOrEmpty(value)) || (ValidationHelper.GetGuid(value, Guid.Empty) == Guid.Empty))
            {
                // Empty error
                if ((ErrorMessage != null) && !ErrorMessage.EqualsCSafe(ResHelper.GetString("BasicForm.InvalidInput"), true))
                {
                    ValidationError = ErrorMessage;
                }
                else
                {
                    ValidationError += ResHelper.GetString("BasicForm.ErrorEmptyValue");
                }
                return false;
            }
        }
        return true;
    }

    #endregion
}