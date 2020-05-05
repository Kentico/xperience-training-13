using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("om.activitydetals.viewrecorddetail")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Controls_UI_ActivityDetails_CustomTableDetails : CMSModalPage
{
    /// <summary>
    /// Page init event handler
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        // Check permissions
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        int tableID = QueryHelper.GetInteger("tableid", 0);
        int itemID = QueryHelper.GetInteger("itemid", 0);

        if ((tableID <= 0) || (itemID <= 0))
        {
            return;
        }

        var customTable = DataClassInfoProvider.GetDataClassInfo(tableID);
        if (customTable == null)
        {
            return;
        }

        if (!customTable.CheckPermissions(PermissionsEnum.Read, SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
        {
            // Insufficient permissions to read item data
            ShowError(String.Format(GetString("customtable.permissiondenied.read"), customTable.ClassName));
            return;
        }

        form.CustomTableId = tableID;
        form.ItemID = itemID;
    }


    /// <summary>
    /// Page PreRender event handler
    /// </summary>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (form != null)
        {
            form.SubmitButton.Visible = false;
        }
    }
}