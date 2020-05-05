using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Reporting_LiveDialogs_EditSubscription : CMSLiveModalPage
{
    #region "Variables"

    bool liveEdit = false;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (liveEdit)
        {
            PageTitle.TitleText = GetString("reportsubscription.edit");
        }

        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.close");
    }


    protected override void CreateChildControls()
    {
        String mode = QueryHelper.GetString("mode", String.Empty);
        liveEdit = (mode.ToLowerCSafe() == "liveedit");
        subEdit.SimpleMode = !liveEdit;

        base.CreateChildControls();
    }


    protected void btnOK_Click(object sender, EventArgs ea)
    {
        if (subEdit.Save())
        {
            String alert = liveEdit ? String.Empty : "wopener.window.alert(" + ScriptHelper.GetLocalizedString("reportsubscription.subscribed") + ")";
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "CloseScript", ScriptHelper.GetScript("CloseDialog(); if (wopener.refreshCurrentPage != null) { wopener.refreshCurrentPage(); }" + alert));
        }
    }

    #endregion
}
