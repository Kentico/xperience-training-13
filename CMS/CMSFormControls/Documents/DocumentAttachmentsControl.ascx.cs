using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.FormEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;


public partial class CMSFormControls_Documents_DocumentAttachmentsControl : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return documentAttachments.MessagesPlaceHolder;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return documentAttachments.Enabled;
        }
        set
        {
            documentAttachments.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return documentAttachments.Value;
        }
        set
        {
            documentAttachments.Value = value;
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
            documentAttachments.FieldInfo = value;
        }
    }


    /// <summary>
    /// Indicates if control is placed on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return documentAttachments.IsLiveSite;
        }
        set
        {
            documentAttachments.IsLiveSite = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize control
        documentAttachments.Form = Form;
        documentAttachments.CheckPermissions = false;
        documentAttachments.AllowChangeOrder = ValidationHelper.GetBoolean(GetValue("changeorder"), true);

        if (Form != null)
        {
            documentAttachments.OnUploadFile += Form.RaiseOnUploadFile;
            documentAttachments.OnDeleteFile += Form.RaiseOnDeleteFile;
        }

        var pageSize = ValidationHelper.GetString(GetValue("pagingsize"), "5,10,25,50,100,##ALL##");
        documentAttachments.PageSize = pageSize;

        // Get default page size as first number in page size values
        string defaultPageSize = pageSize.Contains(",") ? pageSize.Substring(0, pageSize.IndexOf(',')) : pageSize;
        
        documentAttachments.DefaultPageSize = ValidationHelper.GetInteger(GetValue("defaultpagesize"), ValidationHelper.GetInteger(defaultPageSize, 0)); 
        documentAttachments.AllowPaging = ValidationHelper.GetBoolean(GetValue("paging"), true);

        if (FieldInfo != null)
        {
            documentAttachments.GroupGUID = FieldInfo.Guid;
        }

        // Set allowed extensions
        string extensions = ValidationHelper.GetString(GetValue("extensions"), null);
        string allowedExtensions = ValidationHelper.GetString(GetValue("allowed_extensions"), null);
        if ((extensions == "custom") && (!String.IsNullOrEmpty(allowedExtensions)))
        {
            documentAttachments.AllowedExtensions = allowedExtensions;
        }

        // Set image auto resize dimensions
        if (FieldInfo != null)
        {
            int attachmentsWidth;
            int attachmentsHeight;
            int attachmentsMaxSideSize;
            ImageHelper.GetAutoResizeDimensions(FieldInfo.Settings, SiteContext.CurrentSiteName, out attachmentsWidth, out attachmentsHeight, out attachmentsMaxSideSize);
            documentAttachments.ResizeToWidth = attachmentsWidth;
            documentAttachments.ResizeToHeight = attachmentsHeight;
            documentAttachments.ResizeToMaxSideSize = attachmentsMaxSideSize;
        }

        if (Form != null)
        {
            // Get node
            TreeNode node = (TreeNode)Form.EditedObject;

            if (Form.IsInsertMode)
            {
                documentAttachments.FormGUID = Form.FormGUID;
                var parent = Form.ParentObject as TreeNode;
                if (parent != null)
                {
                    documentAttachments.NodeParentNodeID = parent.NodeID;
                }
                else if (node != null)
                {
                    documentAttachments.NodeParentNodeID = node.NodeParentID;
                }

                if (node != null)
                {
                    documentAttachments.NodeClassName = node.NodeClassName;
                }
            }
            else if (node != null)
            {
                // Set appropriate control settings
                documentAttachments.DocumentID = node.DocumentID;
                documentAttachments.NodeParentNodeID = node.NodeParentID;
                documentAttachments.NodeClassName = node.NodeClassName;
            }
        }

        // Set control styles
        if (!String.IsNullOrEmpty(ControlStyle))
        {
            documentAttachments.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
        if (!String.IsNullOrEmpty(CssClass))
        {
            documentAttachments.CssClass = CssClass;
            CssClass = null;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (FieldInfo != null)
        {
            documentAttachments.ID = FieldInfo.Name;
        }
    }

    #endregion
}