using System;

using CMS.Helpers;
using CMS.UIControls;


[UIElement("CMS.Categories", "Categories")]
[Security(Resource = "CMS.Categories", Permission = "Read", ResourceSite = true)]
public partial class CMSModules_Categories_Pages_Administration_Category_List : CMSAdministrationPage
{
    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Initialize the controls
        SetupControl();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
        

        if (SiteID > 0)
        {
            CategoriesElem.DisplaySiteSelector = false;
        }

        CategoriesElem.StartInCreatingMode = QueryHelper.GetBoolean("createNew", false);

        titleElem.TitleText = GetString("Development.Categories");
    }

    #endregion
}