using System;

using CMS.Membership;
using CMS.UIControls;


public partial class CMSModules_Content_CMSDesk_Properties_Relateddocs_Add : CMSPropertiesPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.RelatedDocs"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "Properties.RelatedDocs");
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initializes page breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Relationship.RelatedDocs"),
            RedirectUrl = "~/CMSModules/Content/CMSDesk/Properties/Relateddocs_List.aspx?nodeid=" + NodeID,
            Target = "propedit"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Relationship.AddRelatedDocs")
        });

        // Set edited document
        addRelatedDocument.TreeNode = Node;
    }

    #endregion
}