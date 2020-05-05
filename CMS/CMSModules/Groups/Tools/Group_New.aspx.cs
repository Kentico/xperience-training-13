using System;

using CMS.Community.Web.UI;
using CMS.Core;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;


[UIElement(ModuleName.GROUPS, "NewGroup")]
public partial class CMSModules_Groups_Tools_Group_New : CMSGroupPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Pagetitle
        PageTitle.TitleText = GetString("Group.NewHeaderCaption");
        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Group.ItemListLink"),
            RedirectUrl = ResolveUrl("~/CMSModules/Groups/Tools/Group_List.aspx"),
        });
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Group.NewItemCaption"),
        });

        if (SiteContext.CurrentSite != null)
        {
            groupEditElem.SiteID = SiteContext.CurrentSite.SiteID;
        }

        groupEditElem.OnSaved += new EventHandler(groupEditElem_OnSaved);
        groupEditElem.DisplayAdvanceOptions = true;
        groupEditElem.IsLiveSite = false;
    }


    private void groupEditElem_OnSaved(object sender, EventArgs e)
    {
        // Redirect to edit page
        string editUrl = UIContextHelper.GetElementUrl("CMS.Groups", "EditGroup");
        editUrl = URLHelper.AddParameterToUrl(editUrl, "objectid", groupEditElem.GroupID.ToString());
        editUrl = URLHelper.AddParameterToUrl(editUrl, "displaytitle", "false");
        URLHelper.Redirect(editUrl);
    }
}