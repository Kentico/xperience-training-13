using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


[Title("reportsubscription.new")]
public partial class CMSModules_Reporting_Dialogs_EditSubscription : CMSModalPage
{
    #region "Variables"

    bool edit;

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        edit = (QueryHelper.GetString("mode", String.Empty).ToLowerCSafe() == "edit");
        if (edit)
        {
            subEdit.SimpleMode = false;
            PageTitle.TitleText = GetString("reportsubscription.edit");
        }

        base.CreateChildControls();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        Save += btnOK_Click;
    }


    protected void btnOK_Click(object sender, EventArgs ea)
    {
        if (subEdit.Save())
        {
            String alertScript = edit ? String.Empty : "wopener.window.alert(" + ScriptHelper.GetLocalizedString("reportsubscription.subscribed") + ");";
            String refreshScript = edit ? "if (wopener.refreshCurrentPage != null) { wopener.refreshCurrentPage(); }" : String.Empty;
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "CloseScript", ScriptHelper.GetScript(refreshScript + "CloseDialog();" + alertScript));
        }
    }

    #endregion
}
