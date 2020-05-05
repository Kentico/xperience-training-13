using System;

using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Base.Web.UI;
using CMS.Helpers;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_Purchase : ActivityDetail
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "displayModal",
                                               ScriptHelper.GetScript(
                                                   "function DisplayOrderDetails(params) { \n" +
                                                   "       modalDialog('" + ResolveUrl(@"~\CMSModules\Activities\Controls\UI\ActivityDetails\PurchaseDetail.aspx") + "' + params, 'bizdetails', 700, 600); \n" +
                                                   " } \n "));
    }


    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.PURCHASE))
        {
            return false;
        }

        int orderId = ai.ActivityItemID;
        string qs = String.Format("?orderid={0}", orderId);
        qs = URLHelper.AddUrlParameter(qs, "hash", QueryHelper.GetHash(qs));
        btnView.Visible = true;
        btnView.OnClientClick = "DisplayOrderDetails('" + qs + "'); return false;";

        return true;
    }

    #endregion
}