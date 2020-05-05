using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.PortalEngine.Web.UI;
using CMS.UIControls;


public partial class CMSModules_Chat_Controls_UI_ChatRoom_Edit : CMSAdminEditControl
{
    #region "Private fields"

    private int? mSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// UIForm control used for editing objects properties.
    /// </summary>
    public UIForm UIFormControl
    {
        get
        {
            return EditForm;
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
            EditForm.StopProcessing = value;
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
    /// SiteID of a new room.
    /// 
    /// NULL means global.
    /// </summary>
    public int? SiteID
    {
        get
        {
            if (TypedEditedObject != null && TypedEditedObject.ChatRoomID > 0)
            {
                return TypedEditedObject.ChatRoomSiteID;
            }

            return mSiteID;
        }
        set
        {
            if (value <= 0)
            {
                value = null;
            }
            mSiteID = value;
        }
    }


    public ChatRoomInfo TypedEditedObject
    {
        get
        {
            return (ChatRoomInfo)UIContext.EditedObject;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        UIFormControl.OnBeforeDataRetrieval += UIFormControl_OnBeforeDataRetrieval;
        UIFormControl.OnAfterSave += UIFormControl_OnAfterSave;
        UIFormControl.OnCheckPermissions += UIFormControl_OnCheckPermissions;
        UIFormControl.OnItemValidation += UiFormControlOnOnItemValidation;

        string urlAfterCreate = UIContextHelper.GetElementUrl("CMS.Chat", "EditChatRoom");
        urlAfterCreate = URLHelper.AddParameterToUrl(urlAfterCreate, "roomId", "{%EditedObject.ID%}");
        urlAfterCreate = URLHelper.AddParameterToUrl(urlAfterCreate, "objectid", "{%EditedObject.ID%}");
        urlAfterCreate = URLHelper.AddParameterToUrl(urlAfterCreate, "siteid", "{?siteid?}");
        urlAfterCreate = URLHelper.AddParameterToUrl(urlAfterCreate, "saved", "1");
        urlAfterCreate = URLHelper.AddParameterToUrl(urlAfterCreate, "displaytitle", "false");
        UIFormControl.RedirectUrlAfterCreate = urlAfterCreate;

        // Allow an empty password
        EditingFormControl passwordEditingControl = UIFormControl.FieldEditingControls["chatroompassword"];
        var passwordStrengthControl = passwordEditingControl.NestedControl as CMSModules_Membership_FormControls_Passwords_PasswordStrength;
        if (passwordStrengthControl != null)
        {
            passwordStrengthControl.AllowEmpty = true;
        }
    }


    private void UiFormControlOnOnItemValidation(object sender, ref string errorMessage)
    {
        var control = sender as FormEngineUserControl;
        switch (control.Field)
        {
            case "ChatRoomDisplayName":
            case "ChatRoomDescription":
                try
                {
                    ChatProtectionHelper.CheckNameForBadWords(control.Value.ToString());
                }
                catch (ChatBadWordsException ex)
                {
                    errorMessage = ex.Message;
                }
                break;
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // If room is one to one support (support room created after user requested support), disable editing some properties.
        if ((TypedEditedObject != null) && TypedEditedObject.IsOneToOneSupport)
        {
            DisableFieldControl("ChatRoomName");
            DisableFieldControl("ChatRoomEnabled");
            DisableFieldControl("ChatRoomPrivate");
            DisableFieldControl("ChatRoomAllowAnonym");
            DisableFieldControl("ChatRoomIsSupport");
        }
    }


    void DisableFieldControl(string fieldName)
    {
        if (UIFormControl.FieldControls != null)
        {
            FormEngineUserControl fieldControl = UIFormControl.FieldControls[fieldName];

            if (fieldControl != null)
            {
                fieldControl.Enabled = false;
            }
        }
    }


    void UIFormControl_OnCheckPermissions(object sender, EventArgs e)
    {
        ((CMSChatPage)Page).CheckModifyPermission(SiteID);
    }


    void UIFormControl_OnBeforeDataRetrieval(object sender, EventArgs e)
    {
        // Set site id and other data if the room is new
        if ((UIContext.EditedObject == null) || (((ChatRoomInfo)UIContext.EditedObject).ChatRoomID <= 0))
        {
            IDataContainer data = UIFormControl.Data;
            data["ChatRoomCreatedByChatUserID"] = ChatUserHelper.GetChatUserFromCMSUser().ChatUserID;
            data["ChatRoomCreatedWhen"] = DateTime.Now; // GETDATE() will be used on SQL Server side
            data["ChatRoomSiteID"] = SiteID;

            Guid guid = Guid.NewGuid();
            data["ChatRoomGUID"] = guid;

            EditingFormControl passwordEditingControl = UIFormControl.FieldEditingControls["chatroompassword"];
            string password = passwordEditingControl.Value.ToString();
            passwordEditingControl.Value = ChatRoomHelper.GetRoomPasswordHash(password, guid);
        }
        else
        {
            ChatRoomInfo room = UIContext.EditedObject as ChatRoomInfo;
            EditingFormControl enabledControl = UIFormControl.FieldEditingControls["chatroomenabled"];
            bool enabled = (bool)enabledControl.Value;
            if (room.ChatRoomEnabled != enabled)
            {
                if (enabled)
                {
                    ChatRoomHelper.EnableChatRoom(room.ChatRoomID);
                }
                else
                {
                    ChatRoomHelper.DisableChatRoom(room.ChatRoomID);
                }
            }
        }
    }


    void UIFormControl_OnAfterSave(object sender, EventArgs e)
    {
        // Refresh header with display name in breadcrumbs
        ScriptHelper.RefreshTabHeader(Page, ((ChatRoomInfo)EditedObject).ChatRoomDisplayName);

    }

    #endregion
}
