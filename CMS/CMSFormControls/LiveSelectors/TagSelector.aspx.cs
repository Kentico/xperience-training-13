using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSFormControls_LiveSelectors_TagSelector : CMSLiveModalPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register jQuery
        ScriptHelper.RegisterJQuery(Page);

        // Page title
        PageTitle.TitleText = GetString("tags.tagselector.title");

        btnOk.Click += btnOk_Click;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        tagSelectorDialog.SaveTags();
    }

    #endregion
}
