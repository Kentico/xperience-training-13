using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.Taxonomy;
using CMS.UIControls;


public partial class CMSModules_Categories_CMSPages_CategoryEdit : CMSLiveModalPage
{
    #region "Variables"

    private int categoryId = 0;
    private int parentCategoryId = -1;
    private bool createPersonal = true;
    private CurrentUserInfo currentUser = null;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentUser = MembershipContext.AuthenticatedUser;

        categoryId = QueryHelper.GetInteger("categoryId", 0);
        parentCategoryId = QueryHelper.GetInteger("parentId", -1);
        createPersonal = QueryHelper.GetBoolean("personal", true);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string titleText = GetString("categories.properties");
        if (categoryId == 0)
        {
            catEdit.ParentCategoryID = parentCategoryId;
            catEdit.UserID = createPersonal ? MembershipContext.AuthenticatedUser.UserID : 0;
            catEdit.ShowEnabled = false;

            CheckReadPermission(catEdit.ParentCategory);
            titleText = GetString("multiplecategoriesselector.newcategory");
        }
        else
        {
            catEdit.CategoryID = categoryId;
            CheckReadPermission(catEdit.Category);
        }

        // Init buttons
        btnCancel.Attributes.Add("onclick", "CloseDialog();");
        btnOk.Click += btnOk_Click;

        PageTitle.TitleText = titleText;
        Page.Title = titleText;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (catEdit.EditingForm.SaveData(null))
        {
            string param = "edit";
            if (categoryId == 0)
            {
                param = "new|" + catEdit.CategoryID;
            }

            // Refresh opener and close dialog
            ltlScript.Text = ScriptHelper.GetScript("wopener.Refresh('" + param + "'); CloseDialog();");
        }
    }


    private void CheckReadPermission(CategoryInfo category)
    {
        if ((category != null) && (category.CategoryUserID != currentUser.UserID))
        {
            // Check read permission for Categories module
            if (!currentUser.IsAuthorizedPerResource("CMS.Categories", "Read"))
            {
                RedirectToAccessDenied("CMS.Categories", "Read");
            }
        }
    }

    #endregion
}
