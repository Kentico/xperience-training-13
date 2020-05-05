using System;

using CMS.Activities;
using CMS.Activities.Web.UI;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.Helpers;


public partial class CMSModules_Activities_Controls_UI_ActivityDetails_CustomTableSubmit : ActivityDetail
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "displayModal",
                                               ScriptHelper.GetScript(
                                                   "function DisplayCustomTableItemDetails(params) { \n" +
                                                   "       modalDialog('" + ResolveUrl(@"~\CMSModules\Activities\Controls\UI\ActivityDetails\CustomTableDetails.aspx") + "' + params, 'ctabledetails', 720, 450); \n" +
                                                   " } \n "));
    }


    /// <summary>
    /// Loads data
    /// </summary>
    /// <param name="ai">Activity info</param>
    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.CUSTOM_TABLE_SUBMIT))
        {
            return false;
        }

        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(ai.ActivityItemID);
        if (customTable == null)
        {
            return false;
        }

        string qs = String.Format("?tableid={0}&itemid={1}", ai.ActivityItemID, ai.ActivityItemDetailID);
        qs = URLHelper.AddUrlParameter(qs, "hash", QueryHelper.GetHash(qs));
        btnView.Visible = true;
        btnView.OnClientClick = "DisplayCustomTableItemDetails('" + qs + "'); return false;";
        return true;
    }

    #endregion
}