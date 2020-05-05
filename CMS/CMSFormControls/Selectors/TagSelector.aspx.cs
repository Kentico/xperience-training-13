using System;

using CMS.Base.Web.UI;
using CMS.UIControls;


public partial class CMSFormControls_Selectors_TagSelector : CMSModalPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        SetSaveResourceString("general.select");

        // Register jQuery
        ScriptHelper.RegisterJQuery(Page);

        // Page title
        PageTitle.TitleText = GetString("tags.tagselector.title");

        Save += btnOk_Click;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        tagSelectorDialog.SaveTags();
    }

    #endregion
}
