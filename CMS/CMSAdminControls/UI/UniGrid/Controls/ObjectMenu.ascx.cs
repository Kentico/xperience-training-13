using System;

using CMS.Helpers;

using System.Text;

using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSAdminControls_UI_UniGrid_Controls_ObjectMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterApplicationConstants(Page);

        // Get the object type
        string param = ContextMenu.Parameter;
        string objectType = null;
        bool showMoveActions = false;
        if (param != null)
        {
            string[] parameters = param.Split(';');
            objectType = parameters[0];

            showMoveActions = (parameters.Length >= 3) && ValidationHelper.GetBoolean(parameters[2], false);
        }

        // Get empty info
        GeneralizedInfo emptyObject = null;
        ObjectTypeInfo ti = null;
        var uiContext = UIContextHelper.GetUIContext(this);

        if (objectType != null)
        {
            var uiContextSiteId = ValidationHelper.GetInteger(uiContext["SiteID"], 0);
            emptyObject = UniGridFunctions.GetEmptyObjectWithSiteID(objectType, uiContextSiteId);

            ti = emptyObject.TypeInfo;

            // Get correct info for listings
            if (ti.IsListingObjectTypeInfo)
            {
                emptyObject = UniGridFunctions.GetEmptyObjectWithSiteID(ti.OriginalObjectType, uiContextSiteId);
            }
        }

        if (emptyObject == null)
        {
            Visible = false;
            return;
        }

        var curUser = MembershipContext.AuthenticatedUser;
        string curSiteName = SiteContext.CurrentSiteName;

        string menuId = ContextMenu.MenuID;

        if (ti.OrderColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN && showMoveActions)
        {
            iMoveUp.Attributes.Add("onclick", "ContextMoveObject_" + ClientID + "('#moveup', GetContextMenuParameter('" + menuId + "'))");
            iMoveDown.Attributes.Add("onclick", "ContextMoveObject_" + ClientID + "('#movedown', GetContextMenuParameter('" + menuId + "'))");
        }
        else
        {
            iMoveUp.Visible = false;
            iMoveDown.Visible = false;
        }

        // Export
        if (ti.ImportExportSettings.AllowSingleExport)
        {
            if (curUser.IsAuthorizedPerResource("cms.globalpermissions", "ExportObjects", curSiteName))
            {
                iExport.Attributes.Add("onclick", "ContextExportObject(GetContextMenuParameter('" + menuId + "'), false);");
            }
            else
            {
                iExport.Visible = false;
            }

            if (ti.GUIDColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
            {
                if (curUser.IsAuthorizedPerResource("cms.globalpermissions", "BackupObjects", curSiteName))
                {
                    iBackup.Attributes.Add("onclick", "ContextExportObject(GetContextMenuParameter('" + menuId + "'), true);");
                }
                else
                {
                    iBackup.Visible = false;
                }

                if (curUser.IsAuthorizedPerResource("cms.globalpermissions", "RestoreObjects", curSiteName))
                {
                    iRestore.Attributes.Add("onclick", "ContextRestoreObject(GetContextMenuParameter('" + menuId + "'), true);");
                }
                else
                {
                    iRestore.Visible = false;
                }
            }
            else
            {
                iBackup.Visible = false;
                iRestore.Visible = false;
            }
        }
        else
        {
            iExport.Visible = false;
            iBackup.Visible = false;
            iRestore.Visible = false;
        }
        
        // Versioning
        if (ObjectVersionManager.AllowObjectRestore(emptyObject) && UniGridFunctions.ObjectSupportsDestroy(emptyObject) && curUser.IsAuthorizedPerObject(PermissionsEnum.Destroy, emptyObject, curSiteName))
        {
            iDestroy.Attributes.Add("onclick", "ContextDestroyObject_" + ClientID + "(GetContextMenuParameter('" + menuId + "'))");
        }
        else
        {
            iDestroy.Visible = false;
        }

        // Clonning
        if ((emptyObject.AllowClone) && (curUser.IsAuthorizedPerObject(PermissionsEnum.Modify, emptyObject, curSiteName)) && (curUser.IsAuthorizedPerObject(PermissionsEnum.Create, emptyObject, curSiteName)))
        {
            iClone.Attributes.Add("onclick", "ContextCloneObject" + "(GetContextMenuParameter('" + menuId + "'))");
        }
        else
        {
            iClone.Visible = false;
        }

        bool ancestor = (iClone.Visible || iDestroy.Visible);
        sepCloneDestroy.Visible = ancestor;
        sepExport.Visible = (iBackup.Visible || iRestore.Visible || iExport.Visible) && ancestor;
        ancestor |= (iBackup.Visible || iRestore.Visible || iExport.Visible);
        sepMove.Visible = (iMoveUp.Visible || iMoveDown.Visible) && ancestor;

        Visible = iExport.Visible || iBackup.Visible || iDestroy.Visible || iClone.Visible || iMoveUp.Visible || iMoveDown.Visible;
    }
    

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (Visible)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
function ContextExportObject(definition, backup) {
    var query = ''; 
    if (backup) {
        query += '&backup=true';
    }
    modalDialog(applicationUrl + 'CMSModules/ImportExport/Pages/ExportObject.aspx?objectType=' + escape(definition[0]) + '&objectId=' + definition[1] + query, 'ExportObject', " + CMSPage.EXPORT_OBJECT_WIDTH + ", " + CMSPage.EXPORT_OBJECT_HEIGHT +  @");
}

function ContextRestoreObject(definition, backup) {
    var query = '&ug=UG_", ContextMenu.ParentElementClientID, @"';

    if (backup) {
        query += '&backup=true';
    }
    modalDialog(applicationUrl + 'CMSModules/ImportExport/Pages/RestoreObject.aspx?objectType=' + escape(definition[0]) + '&objectId=' + definition[1] + query, 'RestoreObject', 750, 400);
}

function ContextCloneObject(definition) {
    modalDialog(applicationUrl + 'CMSModules/Objects/Dialogs/CloneObjectDialog.aspx?objectType=' + escape(definition[0]) + '&objectId=' + definition[1], 'CloneObject', 750, 470);
}");

            // Register general export scripts
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ObjectMenuExportScripts", sb.ToString(), true);

            if (iDestroy.Visible)
            {
                sb = new StringBuilder();
                sb.Append(@"
function ContextDestroyObject_", ClientID, @"(definition) {
   if(confirm(", ScriptHelper.GetLocalizedString("objectversioning.destroyobjectconfirmation"), @")) {
      var ug = window.CMS.UG_", ContextMenu.ParentElementClientID, @";
      if (ug.destroy && definition && definition.length == 2) {
          ug.destroy(definition[1]);
      }
   }
}");

                // Register destroy script for particular menu
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ObjectMenuDestroyScript_" + ClientID, sb.ToString(), true);
            }

            if (iMoveUp.Visible || iMoveDown.Visible)
            {
                sb = new StringBuilder();
                sb.Append(@"
function ContextMoveObject_", ClientID, @"(commandName, definition) {
    var ug = window.CMS.UG_", ContextMenu.ParentElementClientID, @";
    if (ug.command && definition && definition.length == 2) {
        ug.command(commandName, definition[1]);
    }
}");
                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ObjectMenuMoveScript_" + ClientID, sb.ToString(), true);
            }
        }
    }
}