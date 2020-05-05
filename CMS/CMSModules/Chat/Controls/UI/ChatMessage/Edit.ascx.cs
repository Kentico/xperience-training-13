using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatMessage_Edit : CMSAdminEditControl
{
    #region "Properties"
    
    /// <summary>
    /// Checks if current message is new (true = is new, false = exists)
    /// </summary>
    private bool IsChatMessageNew
    {
        get
        {
            var editedMessage = UIContext.EditedObject as ChatMessageInfo;
            return (editedMessage == null) || (editedMessage.ChatMessageID <= 0);
        }
    }


    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return this.EditForm;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            this.EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get 
        { 
             return base.IsLiveSite;
        }
        set 
        { 
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Presets ChatRoomID of this message to this value.
    /// </summary>
    public int ChatRoomID { get; set; }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string url = "Edit.aspx?messageid={%EditedObject.ID%}&saved=1";

        url = URLHelper.AddParameterToUrl(url, "roomid", ChatRoomID.ToString());
        
        UIFormControl.RedirectUrlAfterCreate = url;

        UIFormControl.OnBeforeSave += UIFormControl_OnBeforeSave;
        UIFormControl.OnCheckPermissions += UIFormControl_OnCheckPermissions;
    }


    void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        ((CMSChatPage)Page).CheckModifyPermission(((ChatRoomInfo)UIContext.EditedObjectParent).ChatRoomSiteID);
    }


    void UIFormControl_OnBeforeSave(object sender, EventArgs e)
    {
        // Set default values if message is new
        if (IsChatMessageNew)
        {
            UIFormControl.Data["ChatMessageIPAddress"] = RequestContext.UserHostAddress;

            // GETDATE() will be used on SQL Server side
            UIFormControl.Data["ChatMessageCreatedWhen"] = DateTime.Now;
            UIFormControl.Data["ChatMessageUserID"] = null;
            UIFormControl.Data["ChatMessageRoomID"] = ChatRoomID;
            UIFormControl.Data["ChatMessageRejected"] = false;
            UIFormControl.Data["ChatMessageSystemMessageType"] = ChatMessageTypeEnum.Announcement;
        }
    }

    #endregion
}

