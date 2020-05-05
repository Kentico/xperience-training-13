using System;

using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("Polls_New.HeaderCaption")]
[UIElement("CMS.Polls", "Polls.AddPoll")]
public partial class CMSModules_Polls_Tools_Polls_New : CMSPollsPage
{
    #region "Private variables"

    private int siteId = 0;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        siteId = QueryHelper.GetInteger("siteid", 0);
        bool createGlobal = (siteId == UniSelector.US_GLOBAL_RECORD) || (siteId == UniSelector.US_GLOBAL_AND_SITE_RECORD);

        // Init breadcrumbs - new item breadcrumb
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Polls_New.ItemListLink"),
            RedirectUrl = "~/CMSModules/Polls/Tools/Polls_List.aspx?siteid=" + siteId
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Polls_New.NewItemCaption")
        });

        PollNew.OnSaved += new EventHandler(PollNew_OnSaved);
        PollNew.IsLiveSite = false;
        PollNew.CreateGlobal = createGlobal;
    }


    /// <summary>
    /// OnSave event handler.
    /// </summary>
    private void PollNew_OnSaved(object sender, EventArgs e)
    {
        string error = null;
        // Show possible license limitation error
        if (!String.IsNullOrEmpty(PollNew.LicenseError))
        {
            error = "&error=" + PollNew.LicenseError;
        }

        string editActionUrl = URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(
            URLHelper.AddParameterToUrl(UIContextHelper.GetElementUrl("CMS.Polls", "EditPoll"), "objectid", PollNew.ItemID.ToString()), "siteid", siteId.ToString()), "displaytitle", "false"), "saved", "1");
        URLHelper.Redirect(editActionUrl + error);
    }
}