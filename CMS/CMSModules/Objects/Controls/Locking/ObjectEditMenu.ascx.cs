using System;
using System.Text;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine.Web.UI;
using CMS.Synchronization;

public partial class CMSModules_Objects_Controls_Locking_ObjectEditMenu : ObjectEditMenu, IObjectEditMenu, IExtensibleEditMenu
{
    #region "Variables"

    // Actions
    protected SaveAction save = null;
    protected HeaderAction checkin = null;
    protected HeaderAction checkout = null;
    protected HeaderAction undocheckout = null;

    private CMSObjectManager mObjectManager;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Menu control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return menu;
        }
    }


    /// <summary>
    /// Show the check in with comment button.
    /// </summary>
    public bool ShowCheckInWithComment
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the menu is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return menu.Enabled;
        }
        set
        {
            menu.Enabled = value;
        }
    }


    /// <summary>
    /// Object instance.
    /// </summary>
    public override BaseInfo InfoObject
    {
        get
        {
            return ObjectManager.InfoObject;
        }
    }


    /// <summary>
    /// Object manager control
    /// </summary>
    public CMSObjectManager ObjectManager
    {
        get
        {
            if (mObjectManager == null)
            {
                mObjectManager = CMSObjectManager.GetCurrent(this);
                if (mObjectManager == null)
                {
                    throw new Exception("[ObjectEditMenu.ObjectManager]: Missing object manager.");
                }
            }

            return mObjectManager;
        }
    }


    /// <summary>
    /// Gets the associated object manager control.
    /// </summary>
    public ICMSObjectManager AbstractObjectManager
    {
        get
        {
            return ObjectManager;
        }
    }


    /// <summary>
    /// If true, the access permissions to the items are checked.
    /// </summary>
    public override bool CheckPermissions
    {
        get
        {
            return ObjectManager.CheckPermissions;
        }
        set
        {
            ObjectManager.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return ObjectManager.StopProcessing;
        }
        set
        {
            ObjectManager.StopProcessing = value;
        }
    }

    #endregion


    #region "Constructors"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Objects_Controls_Locking_ObjectEditMenu()
    {
        RefreshInterval = 500;
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Perform full post-back if not in update panel
        menu.PerformFullPostBack = !ControlsHelper.IsInUpdatePanel(this);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadMenu();

        var showMenu = menu.HasAnyVisibleAction() || plcDevices.Visible || plcAdditionalControls.Visible;
        if (showMenu)
        {
            if (PreviewMode)
            {
                pnlContainer.CssClass = "cms-edit-menu";
            }

            var infoObj = InfoObject;

            if (SynchronizationHelper.UseCheckinCheckout && ObjectManager.ShowPanel && (infoObj != null) && (infoObj.Generalized.ObjectID > 0) && infoObj.TypeInfo.SupportsLocking)
            {
                ObjectSettingsInfo settings = infoObj.ObjectSettings;

                int currentUserId = CurrentUser.UserID;

                // Get edited info object type and name for use with info messages
                var objectType = TypeHelper.GetNiceObjectTypeName(infoObj.TypeInfo.ObjectType, ResourceCulture);
                var objectName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(infoObj.Generalized.ObjectDisplayName, ResourceCulture));

                // Object is checked out by another user, disable editing for current user
                if (currentUserId != settings.ObjectCheckedOutByUserID)
                {
                    // Disable actions for current user
                    if (save != null)
                    {
                        save.Enabled = false;
                    }

                    if (infoObj.Generalized.IsCheckedOut)
                    {
                        if (checkin != null)
                        {
                            checkin.Visible = false;
                        }
                        if (checkout != null)
                        {
                            checkout.Visible = false;
                        }
                        if (undocheckout != null)
                        {
                            // Always allow undo-checkout for Global Admin
                            undocheckout.Visible = MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
                        }

                        string userName = null;
                        UserInfo ui = UserInfoProvider.GetUserInfo(settings.ObjectCheckedOutByUserID);
                        if (ui != null)
                        {
                            userName = HTMLHelper.HTMLEncode(ui.GetFormattedUserName(IsLiveSite));
                        }

                        AddInfoText(string.Format(GetString("ObjectEditMenu.CheckedOutByAnotherUser", ResourceCulture), objectType, objectName, userName));
                    }
                    else
                    {
                        AddInfoText(string.Format(GetString("ObjectEditMenu.CheckOutToEdit", ResourceCulture), objectType, objectName));
                    }
                }
                else if (infoObj.Generalized.IsCheckedOut)
                {
                    AddInfoText(string.Format(GetString("ObjectEditMenu.CheckInToSubmit", ResourceCulture), objectType, objectName));
                }
            }

            RegisterActionScripts();
        }

        // Hide menu if required
        pnlContainer.Visible &= showMenu;

        // Set the information text
        if (!String.IsNullOrEmpty(InformationText))
        {
            lblInfo.Text = InformationText;
            lblInfo.Visible = true;
            pnlInfoWrapper.Visible = true;
        }
    }


    /// <summary>
    /// Adds information text to the information label
    /// </summary>
    /// <param name="text"></param>
    private void AddInfoText(string text)
    {
        if (!String.IsNullOrEmpty(InformationText))
        {
            InformationText = text + "<br />" + InformationText;
        }
        else
        {
            InformationText = text;
        }
    }


    /// <summary>
    /// Registers action scripts
    /// </summary>
    private void RegisterActionScripts()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("function CheckConsistency_", ClientID, "() { ", ObjectManager.GetJSFunction("CONS", null, null), "; } \n");

        if ((checkin != null) && checkin.Visible && ShowCheckInWithComment)
        {
            sb.Append("function AddComment_", ClientID, "(name, objectType, objectId, menuId) { ", ObjectManager.GetJSFunction(ComponentEvents.COMMENT, "name|objectType|objectId|menuId", null), "; } \n");
        }

        // Register the script
        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "AutoMenuActions" + ClientID, ScriptHelper.GetScript(sb.ToString()));
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds additional control to pnlAdditionalControls panel that shows this control in right part of panel.
    /// </summary>
    /// <param name="control">Control to be added</param>
    /// <exception cref="ArgumentNullException"><paramref name="control"/> is null</exception>
    public void AddAdditionalControl(Control control)
    {
        if (control == null)
        {
            throw new ArgumentNullException("control");
        }

        plcAdditionalControls.Visible = true;
        plcAdditionalControls.Controls.Add(control);
    }


    private void ClearProperties()
    {
        // Clear actions
        save = null;
        checkin = null;
        checkout = null;
        undocheckout = null;

        // Clear security result
        ObjectManager.ClearProperties();
    }


    private void ReloadMenu()
    {
        if (StopProcessing)
        {
            return;
        }

        bool displayObjectMenu = false;
        BaseInfo editInfo = InfoObject ?? ModuleManager.GetReadOnlyObject(ObjectManager.ObjectType);
        if (editInfo != null)
        {
            // Do not display items when object does not support locking and when there is no associated UIForm
            displayObjectMenu = editInfo.TypeInfo.SupportsLocking && ObjectManager.ShowPanel;
        }

        if (displayObjectMenu)
        {
            // Handle several reloads
            menu.ActionsList.Clear();
            ClearProperties();

            if (!HideStandardButtons)
            {
                // Handle save action
                if (ShowSave)
                {
                    save = new SaveAction
                    {
                        Tooltip = ResHelper.GetString("EditMenu.Save", ResourceCulture),
                        Enabled = AllowSave,
                        EventName = "",
                        CommandName = "",
                        Index = -2
                    };

                    if (AllowSave)
                    {
                        string script = RaiseGetClientActionScript(ComponentEvents.SAVE);
                        script += RaiseGetClientValidationScript(ComponentEvents.SAVE, ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null));
                        save.OnClientClick = script;
                    }

                    AddAction(save);
                }

                // Object update
                if (SynchronizationHelper.UseCheckinCheckout && (ObjectManager.Mode == FormModeEnum.Update))
                {
                    if (InfoObject != null)
                    {
                        if (ShowCheckOut)
                        {
                            checkout = new HeaderAction
                            {
                                Tooltip = ResHelper.GetString("ObjectEditMenu.Checkout", ResourceCulture),
                                Text = ResHelper.GetString("EditMenu.IconCheckout", ResourceCulture),
                                Enabled = AllowCheckOut
                            };

                            if (AllowCheckOut)
                            {
                                string script = RaiseGetClientActionScript(ComponentEvents.CHECKOUT);
                                script += RaiseGetClientValidationScript(ComponentEvents.CHECKOUT, ObjectManager.GetJSFunction(ComponentEvents.CHECKOUT, null, null));
                                checkout.OnClientClick = script;
                            }

                            AddAction(checkout);
                        }

                        if (ShowUndoCheckOut)
                        {
                            undocheckout = new HeaderAction
                            {
                                Tooltip = ResHelper.GetString("ObjectEditMenu.UndoCheckOut", ResourceCulture),
                                Text = ResHelper.GetString("EditMenu.IconUndoCheckout", ResourceCulture),
                                Enabled = AllowUndoCheckOut
                            };

                            if (AllowUndoCheckOut)
                            {
                                string script = RaiseGetClientActionScript(ComponentEvents.UNDO_CHECKOUT);
                                script += RaiseGetClientValidationScript(ComponentEvents.UNDO_CHECKOUT, ObjectManager.GetJSFunction(ComponentEvents.UNDO_CHECKOUT, null, null));
                                undocheckout.OnClientClick = script;
                            }

                            AddAction(undocheckout);
                        }

                        if (ShowCheckIn)
                        {
                            checkin = new HeaderAction
                            {
                                Tooltip = ResHelper.GetString("ObjectEditMenu.Checkin", ResourceCulture),
                                Text = ResHelper.GetString("EditMenu.IconCheckin", ResourceCulture),
                                Enabled = AllowCheckIn
                            };

                            if (AllowCheckIn)
                            {
                                string script = RaiseGetClientActionScript(ComponentEvents.CHECKIN);
                                script += RaiseGetClientValidationScript(ComponentEvents.CHECKIN, ObjectManager.GetJSFunction(ComponentEvents.CHECKIN, null, null));
                                checkin.OnClientClick = script;
                            }

                            if (ShowCheckInWithComment)
                            {
                                AddCommentAction(ComponentEvents.CHECKIN, checkin);
                            }
                            else
                            {
                                AddAction(checkin);
                            }
                        }
                    }
                }
            }
        }

        // Add extra actions
        if (ObjectManager.ShowPanel && (mExtraActions != null))
        {
            foreach (HeaderAction action in mExtraActions)
            {
                AddAction(action);
            }
        }
    }


    /// <summary>
    /// Adds menu action.
    /// </summary>
    /// <param name="action">Action</param>
    protected void AddAction(HeaderAction action)
    {
        if (action != null)
        {
            // Action
            menu.ActionsList.Add(action);
        }
    }


    /// <summary>
    /// Adds comment action.
    /// </summary>
    /// <param name="name">Action name</param>
    /// <param name="action">Current action</param>
    private void AddCommentAction(string name, HeaderAction action)
    {
        ObjectManager.RenderScript = true;

        AddAction(action);

        CommentAction comment = new CommentAction(name)
        {
            Text = ResHelper.GetString("ObjectEditMenu.Comment" + name, ResourceCulture),
            OnClientClick = string.Format("AddComment_{0}('{1}','{2}',{3},'{0}');", ClientID, name, InfoObject.TypeInfo.ObjectType, InfoObject.Generalized.ObjectID),
        };
        action.AlternativeActions.Add(comment);
    }

    #endregion
}