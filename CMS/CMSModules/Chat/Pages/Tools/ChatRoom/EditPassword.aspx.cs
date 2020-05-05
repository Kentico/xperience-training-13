using System;

using CMS.Chat;
using CMS.Chat.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[EditedObject(ChatRoomInfo.OBJECT_TYPE, "roomId")]
[ParentObject(ChatRoomInfo.OBJECT_TYPE, "roomId")]

public partial class CMSModules_Chat_Pages_Tools_ChatRoom_EditPassword : CMSChatRoomPage
{
    #region "Private fields"

    private const string hiddenPassword = "********";
    private int? mSiteID;

    #endregion


    #region  "Public properties"

    /// <summary>
    /// SiteID of a new room.
    /// 
    /// NULL means global.
    /// </summary>
    public int? SiteID
    {
        get
        {
            if (TypedEditedObject != null)
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
            return UIContext.EditedObject as ChatRoomInfo;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnSetPassword.Text = GetString("chat.setpassword");
        btnRemovePassword.Text = GetString("chat.removepassword");

        if (!RequestHelper.IsPostBack())
        {
            if (TypedEditedObject != null)
            {
                if (TypedEditedObject.ChatRoomPassword.Length > 0)
                {
                    InformAboutPassword(true);
                }
            }
        }

        if (passStrength.Text.Length > 0)
        {
            btnRemovePassword.Visible = true;
        }

        if ((TypedEditedObject != null) && TypedEditedObject.IsOneToOneSupport)
        {
            btnSetPassword.Enabled = false;
            btnRemovePassword.Enabled = false;
        }
    }
    
    #endregion
    
    
    #region "Event handlers"

    /// <summary>
    /// Sets password of current room.
    /// </summary>
    protected void ButtonSetPassword_Click(object sender, EventArgs e)
    {
        // Check modify permission
        ((CMSChatPage)Page).CheckModifyPermission(SiteID);

        string result = "";

        if (TypedEditedObject != null)
        {
            if (TextBoxConfirmPassword.Text == passStrength.Text)
            {
                //password has been changed
                if (passStrength.Text != hiddenPassword) 
                {                    
                    TypedEditedObject.ChatRoomPassword = ChatRoomHelper.GetRoomPasswordHash(passStrength.Text, TypedEditedObject.ChatRoomGUID);
                    TypedEditedObject.Update();

                    // Show actual information to the user
                    if (passStrength.Text != String.Empty)
                    {
                        InformAboutPassword(true);
                    }
                    else
                    {
                        InformAboutPassword(false);
                    }

                    ShowChangesSaved();
                }
            }
            else
            {
                result = GetString("chat.passwordsdonotmatch");
            }
        }

        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
    }

    /// <summary>
    /// Removes password (sets it to "") of current room.
    /// </summary>
    protected void ButtonRemovePassword_Click(object sender, EventArgs e)
    {
        // Check modify permission
        ((CMSChatPage)Page).CheckModifyPermission(SiteID);

        if (TypedEditedObject != null)
        {
            TypedEditedObject.ChatRoomPassword = "";
            TypedEditedObject.Update();
            InformAboutPassword(false);
            ShowChangesSaved();
        }
    }

    #endregion

    
    #region "Private methods"

    private void InformAboutPassword(bool hasPassword)
    {
        if (hasPassword)
        {
            passStrength.TextBoxAttributes.Add("value", hiddenPassword);
            TextBoxConfirmPassword.Attributes.Add("value", hiddenPassword);
            btnRemovePassword.Visible = true;
        }
        else
        {
            passStrength.TextBoxAttributes.Add("value", "");
            TextBoxConfirmPassword.Attributes.Add("value", "");
            btnRemovePassword.Visible = false;
        }
    }

    #endregion
}