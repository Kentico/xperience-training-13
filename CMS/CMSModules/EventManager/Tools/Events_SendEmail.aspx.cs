using System;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_EventManager_Tools_Events_SendEmail : CMSEventManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int eventId = QueryHelper.GetInteger("eventId", 0);

        // Check if requested event exists
        TreeProvider mTree = new TreeProvider();
        TreeNode mEventNode = mTree.SelectSingleNode(eventId);
        if ((mEventNode == null) || (mEventNode.NodeClassName.ToLowerCSafe() != "cms.bookingevent"))
        {
            EditedObject = null;
        }

        emailSender.EventID = eventId;
        if (!RequestHelper.IsPostBack())
        {
            emailSender.ReloadData(true);
        }
        emailSender.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(emailSender_OnCheckPermissions);
    }


    private void emailSender_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        // Check READ permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.EventManager", permissionType))
        {
            RedirectToAccessDenied("CMS.EventManager", permissionType);
        }
    }
}