using System;

using CMS.ContactManagement;
using CMS.UIControls;


/// <summary>
/// Displays number of contacts in contact group and ratio of that to the number of all contacts.
/// Used on Contact group Overview (General) tab.
/// </summary>
public partial class CMSModules_ContactManagement_Controls_UI_ContactGroup_ContactGroupContactCounter : CMSUserControl
{
    private ContactGroupInfo ContactGroup
    {
        get
        {
            return EditedObject as ContactGroupInfo;
        }
    }
    

    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Check that the control is included in CMSPage (otherwise an exception is thrown on the Design tab)
        var page = Page as CMSPage;
        if (page == null || ContactGroup == null)
        {
            return;
        }

        // Disable refresh when status is ready as performance optimization
        timRefresh.Enabled = ContactGroup.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding;

        if (ContactGroup.ContactGroupStatus == ContactGroupStatusEnum.Rebuilding)
        {
            lblRatio.Visible = false;
            lblCount.Visible = false;
            return;
        }

        int numberOfContacts = ContactGroupMemberInfoProvider.GetNumberOfContactsInGroup(ContactGroup.ContactGroupID);

        // Display number of contacts
        lblCount.InnerText = String.Format(GetString("om.contactgroup.numberofcontacts"), numberOfContacts);

        // Display ratio of the number of contacts
        int totalContactCount = ContactInfo.Provider.Get().Count;

        double ratio = (totalContactCount == 0) ? 0 : (double)numberOfContacts / totalContactCount * 100;
        lblRatio.InnerText = String.Format(GetString("om.contactgroup.numberofcontacts.ratio"), ratio);
    }
}